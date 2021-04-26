using System;
using System.Collections.Generic;
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
        readonly Dictionary<int, int> LayerDict = new();

        // keep track of which nodes need further computation to figure out their exact layer, so we can avoid unessesary complexity
        private int[] hiddenNodes = Array.Empty<int>();

        void AddRecipientToSender(int node, int recipient) => Senders.AddValueToArray(node, recipient);

        void AddSenderToRecipient(int node, int sender) => Recipients.AddValueToArray(node, sender);

        void SetLayer(int node, int layer)
        {
            if (LayerDict.ContainsKey(node) is false)
            {
                LayerDict.Add(node, layer);
                return;
            }
            LayerDict[node] = layer;
        }

        int GetAndSetHiddenLayer(int hiddenNodeId, INeatNetwork network)
        {
            // first check to see if we have already calculated the layer
            if (LayerDict.ContainsKey(hiddenNodeId))
            {
                return LayerDict[hiddenNodeId];
            }

            // rules for finding the exact layer for the node
            /*
                if all of the nodes it receives from are input nodes, it's layer is 2 (1 above the input layer)
                if ANY od the nodes it receives from are hidden nodes, it's layer is above 2
                    -> if child has an entry the parent hidden node's layer is the child nodes layer + 1
                    -> if child doesn't have an entry, recurse until a all nodes received from are input nodes
            */

            // i could do a check to make sure the key exists, but it SHOULD exist by now, and if it doesn't it should throw an error because something is wrong
            Span<int> nodesReceivedFrom = new(Recipients[hiddenNodeId]);

            // this represents what layer all of the children are in, when this is layer 2(1 above input layer) the the highest layer is 1 and our layer is that + 1
            // when any of our children are hidden we should get their height in the network recursively as well and record their height here. We will always be +1 to our highest child in the network
            int HighestLayerOfChildren = 1;

            // iterate through all fo the nodes that provide to this node and check if they are all input nodes
            for (int i = 0; i < nodesReceivedFrom.Length; i++)
            {
                ref int id = ref nodesReceivedFrom[i];

                // check to see if we have already found the layer of the child node
                if (LayerDict.ContainsKey(id))
                {
                    // if the layer dict contains the key it means the id is either a input or an output node, or a hidden node that has already recursively found it's layer, make sure its not 0, since that would be an output node and that would be a circular reference and break everything
                    if (LayerDict[id] >= 1)
                    {
                        // since this is a hidden node we should make sure we keep track of it's height since our height will be the highest child + 1
                        if (LayerDict[id] > HighestLayerOfChildren)
                        {
                            // since it's larger set our largest child to that + 1
                            HighestLayerOfChildren = LayerDict[id];
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
                    int val = GetAndSetHiddenLayer(id, network);

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

            SetLayer(hiddenNodeId, HighestLayerOfChildren);

            // return so we recurse properly
            return HighestLayerOfChildren;
        }

        NodeType GetAndStoreNodeType(int nodeId, INeatNetwork network)
        {
            if (nodeId < (network.InputNodeCount + network.OutputNodeCount))
            {
                // check to see if it's an output node
                // node array scheme is always input, output, hidden ...
                // layer scheme is 0= outputm 1 = input 1+ hidden
                if (nodeId >= network.InputNodeCount)
                {
                    // must be output node
                    SetLayer(nodeId, 0);
                    return NodeType.Output;
                }
                else
                {
                    // must be input node
                    SetLayer(nodeId, 1);
                    return NodeType.Input;
                }
            }

            // add to the hiddenNodes array so we can figure out it's actual layer later
            Extensions.Array.ResizeAndAdd(ref hiddenNodes, nodeId);

            return NodeType.Hidden;
        }

        public IMatrix<T>[] Generate(INeatNetwork network)
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
            LayerDict.Clear();

            // iterate through the innovations list and record where every node sends to and every node receives from
            Span<IInnovation> innovations = new(network.Innovations);

            // ~ O(n)
            for (int i = 0; i < innovations.Length; i++)
            {
                ref IInnovation inn = ref innovations[i];

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
                GetAndStoreNodeType(inn.InputNode, network);
                GetAndStoreNodeType(inn.OutputNode, network);
            }

            // O(i * n)
            // go through the hidden and find their exact layer
            Span<int> hiddenSpan = new(hiddenNodes);
            for (int i = 0; i < hiddenSpan.Length; i++)
            {
                GetAndSetHiddenLayer(hiddenSpan[i], network);
            }

            // now that we have what our input nodes, output nodes, and hidden nodes are and their exact location we can construct the matrix to represent the data

            throw new NotImplementedException();

            return default;
        }
    }
}
