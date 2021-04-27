using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultPhenotypeGenerator<T> : IPhenotypeGenerator<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        // Key = the id of the node that sends to int[] nodes
        readonly Dictionary<int, int[]> Senders = new();

        // Key = the id of the node that receives from int[] nodes
        readonly Dictionary<int, int[]> Recipients = new();

        // Key = the id of the node, Value = the int layer the node is in
        // 0 = output layer
        // 1 = input layer
        // > 1 Hidden node layers
        readonly ReversableConcurrentDictionary<int, int> LayerDict = new();

        // keep track of which nodes need further computation to figure out their exact layer, so we can avoid unessesary complexity
        private int[] hiddenNodes = Array.Empty<int>();

        void AddRecipientToSender(int node, int recipient) => Senders.AddValueToArray(node, recipient);

        void AddSenderToRecipient(int node, int sender) => Recipients.AddValueToArray(node, sender);

        async Task SetLayer(int node, int layer) => await LayerDict.Add(node, layer);

        async Task<int> GetAndSetHiddenLayer(int hiddenNodeId, INeatNetwork network)
        {
            // first check to see if we have already calculated the layer
            if (await LayerDict.ContainsKey(hiddenNodeId))
            {
                return await LayerDict.Get(hiddenNodeId);
            }

            // rules for finding the exact layer for the node
            /*
                if all of the nodes it receives from are input nodes, it's layer is 2 (1 above the input layer)
                if ANY od the nodes it receives from are hidden nodes, it's layer is above 2
                    -> if child has an entry the parent hidden node's layer is the child nodes layer + 1
                    -> if child doesn't have an entry, recurse until a all nodes received from are input nodes
            */

            // this represents what layer all of the children are in, when this is layer 2(1 above input layer) the the highest layer is 1 and our layer is that + 1
            // when any of our children are hidden we should get their height in the network recursively as well and record their height here. We will always be +1 to our highest child in the network
            int HighestLayerOfChildren = 1;

            // iterate through all fo the nodes that provide to this node and check if they are all input nodes
            for (int i = 0; i < Recipients[hiddenNodeId].Length; i++)
            {
                int id = Recipients[hiddenNodeId][i];

                // check to see if we have already found the layer of the child node
                if (await LayerDict.ContainsKey(id))
                {
                    // if the layer dict contains the key it means the id is either a input or an output node, or a hidden node that has already recursively found it's layer, make sure its not 0, since that would be an output node and that would be a circular reference and break everything
                    int val = await LayerDict.Get(id);

                    if (val >= 1)
                    {
                        // since this is a hidden node we should make sure we keep track of it's height since our height will be the highest child + 1
                        if (val > HighestLayerOfChildren)
                        {
                            // since it's larger set our largest child to that + 1
                            HighestLayerOfChildren = val;
                        }
                    }
                    else
                    {
                        int id_ByValue = id;
                        // a node should not be able to 'receieve input' from an output node (in this context at least) - for cyclical DAGs and networks
                        // they should implement a cycle that is not explicitly represented in the innovation genome. idk tho i never went to college
                        throw Networks.Exceptions.CircularInnovationReference(network.Innovations.Where(x => x.Id == id_ByValue).FirstOrDefault());
                    }
                }
                else
                {
                    // if we werent able to find the node in the dict we should recursively check to see if all their children are inputs, and if they are
                    // our layer would be 1 above theirs(or the highest child in our tree)
                    int val = await GetAndSetHiddenLayer(id, network);

                    // check to see if they are taller than all the rest of our children
                    if (val > HighestLayerOfChildren)
                    {
                        HighestLayerOfChildren = val;
                    }
                }
            }

            // at this point either all of the nodes we receive from were input nodes (HighestLayer of 1) or we found the tallest child
            // our height is always + 1 to the tallest node we recieve from

            // set our layer in the dictionary

            HighestLayerOfChildren += 1;

            await SetLayer(hiddenNodeId, HighestLayerOfChildren);

            // return so we recurse properly
            return HighestLayerOfChildren;
        }

        async Task<NodeType> GetAndStoreNodeType(int nodeId, INeatNetwork network)
        {
            if (nodeId < (network.InputNodeCount + network.OutputNodeCount))
            {
                // check to see if it's an output node
                // node array scheme is always input, output, hidden ...
                // layer scheme is 0= outputm 1 = input 1+ hidden
                if (nodeId >= network.InputNodeCount)
                {
                    // must be output node
                    await SetLayer(nodeId, 0);
                    return NodeType.Output;
                }
                else
                {
                    // must be input node
                    await SetLayer(nodeId, 1);
                    return NodeType.Input;
                }
            }

            // add to the hiddenNodes array so we can figure out it's actual layer later
            Extensions.Array.ResizeAndAdd(ref hiddenNodes, nodeId);

            return NodeType.Hidden;
        }


        /// <summary>
        /// Compiles the layers found using <see cref="GenerateMatrices(INeatNetwork)"/> into a multi dimensional array where each index is an individual layer of nodes.
        /// <code>
        /// [0] is the output layer
        /// </code>
        /// <code>
        /// [1] is the input layer
        /// </code>
        /// <code>
        /// [n] is the nth layer
        /// </code>
        /// </summary>
        /// <returns></returns>
        public async Task<int[][]> GetLayers(INeatNetwork network) => await GetLayers(await DecodeGenome(network));

        /// <summary>
        /// Compiles the layers found using <see cref="GenerateMatrices(INeatNetwork)"/> into a multi dimensional array where each index is an individual layer of nodes.
        /// <code>
        /// [0] is the output layer
        /// </code>
        /// <code>
        /// [1] is the input layer
        /// </code>
        /// <code>
        /// [n] is the nth layer
        /// </code>
        /// </summary>
        /// <returns></returns>
        public async Task<int[][]> GetLayers(ReversableConcurrentDictionary<int, int> DecodedGenotype)
        {
            // reverse the dict
            IDictionary<int, int[]> reversed = await DecodedGenotype.ReverseAsync();

            int[][] ConvertToMutliDimensionalArray(IDictionary<int, int[]> dict)
            {
                int[][] layers = new int[dict.Count][];

                Span<int[]> result = new(layers);
                foreach (var item in dict)
                {
                    result[item.Key % result.Length] = item.Value;
                }
                return result.ToArray();
            }

            return ConvertToMutliDimensionalArray(reversed);
        }

        /// <summary>
        /// Decodes the Genome of a <see cref="INeatNetwork"/> into a <see cref="ReversableConcurrentDictionary{KeyType, ValueType}"/> (a concurrent wrapper with a reverse method for <see cref="Dictionary{TKey, TValue}"/>). Convertable to <see cref="IDictionary{TKey, TValue}"/> and <see cref="Dictionary{TKey, TValue}"/>.
        /// <code>
        /// Complexity: O(n)
        /// </code>
        /// <code>
        /// Details: WORST: O(2n-2) -> 1 input node, 1 output node BEST: O(n) -> 0 hidden nodes EXACT: O(n) + O(input*hidden),
        /// noticable slow downs with n -> 10,000
        /// </code>
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public async Task<ReversableConcurrentDictionary<int, int>> DecodeGenome(INeatNetwork network)
        {
            // we should generate the phenotype from innovations alone

            /*
                Innovation scheme:

                  innovation id(#)
                    from -> to
                      Weight
                     ENABLED

            using the above information we should be able to explictly represent the network

            */

            // clear the old phenotype stuff out
            Senders.Clear();
            Recipients.Clear();
            await LayerDict.Clear();

            // ~ O(n)
            for (int i = 0; i < network.Innovations.Length; i++)
            {
                IInnovation inn = network.Innovations[i];

                // only enabled genes show up in the phenotype
                if (inn.Enabled is false)
                {
                    continue;
                }

                // record where the 'from' node sends to
                AddRecipientToSender(inn.InputNode, inn.OutputNode);

                // records where the 'to' receives from
                AddSenderToRecipient(inn.OutputNode, inn.InputNode);

                // determine if the node is an input, output, or hidden node, store that value
                await GetAndStoreNodeType(inn.InputNode, network);
                await GetAndStoreNodeType(inn.OutputNode, network);
            }

            // ~ O(i * n)
            // go through the hidden and find their exact layer
            for (int i = 0; i < hiddenNodes.Length; i++)
            {
                await GetAndSetHiddenLayer(hiddenNodes[i], network);
            }

            // now that we have what our input nodes, output nodes, and hidden nodes are and their exact location we can construct the matrix to represent the data
            // reverse the dictionary and return it
            return LayerDict;
        }

        [DoesNotReturn]
        public IMatrix<T>[] GenerateMatrices(INeatNetwork network)
        {

            throw new NotImplementedException();
        }
    }
}
