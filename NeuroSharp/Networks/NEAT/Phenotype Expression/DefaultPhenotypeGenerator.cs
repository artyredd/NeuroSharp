using NeuroSharp.NEAT.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class DefaultPhenotypeGenerator<T> : IPhenotypeGenerator<double> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Defines a delegate that uses a reference integer for some action.
        /// </summary>
        /// <param name="n"></param>
        delegate void ReferenceIntAction(ref int n);

        ushort GetAndSetHiddenLayer(ref int hiddenNodeId, ref IDictionary<int, int[]> Receivers, ref IDictionary<int, ushort> NodeDict, ref IDictionary<ushort, int[]> LayerDict)
        {
            // node dictionary should have the following scheme:
            // Key: the node id
            // Value: the layer the node is in

            // receiver dictionay scheme:
            // Key: the node id
            // Value: the nodes that the key uses as input nodes

            // first check to see if we have already calculated the layer and stored it
            if (NodeDict.ContainsKey(hiddenNodeId))
            {
                return NodeDict[hiddenNodeId];
            }

            // rules for finding the exact layer for the node
            /*
                if all of the nodes it receives from are input nodes, it's layer is 2 (1 above the input layer)
                if ANY of the nodes it receives from are hidden nodes, it's layer is above 2
                    -> if child has an entry the parent hidden node's layer is the child nodes layer + 1
                    -> if child doesn't have an entry, recurse until a all nodes received from are input nodes
            */

            // this represents what layer all of the children are in, when this is layer 2(1 above input layer) the the highest layer is 1 and our layer is that + 1
            // when any of our children are hidden we should get their height in the network recursively as well and record their height here. We will always be +1 to our highest child in the network
            ushort HighestLayerOfChildren = 1;

            // we should verify that this node receives from any nodes, if it does not it could an input node, if it is then return 1
            if (Receivers.ContainsKey(hiddenNodeId) is false)
            {
                // if the node does not receive from any other nodes it's an input node(or we would hope since if it's not its some random dead node in the middle of the network)
                return 1;
            }

            // iterate through all fo the nodes that provide to this node and check if they are all input nodes
            Span<int> senderNodes = new(Receivers[hiddenNodeId]);
            for (int i = 0; i < senderNodes.Length; i++)
            {
                ref int id = ref senderNodes[i];

                // check to see if we have already found the layer of the child node
                if (NodeDict.ContainsKey(id))
                {
                    // if the layer dict contains the key it means the id is either a input or an output node, or a hidden node that has already recursively found it's layer, make sure its not 0, since that would be an output node and that would be a circular reference and break everything
                    ushort layer = NodeDict[id];

                    if (layer >= 1)
                    {
                        // since this is a hidden node we should make sure we keep track of it's height since our height will be the highest child + 1
                        if (layer > HighestLayerOfChildren)
                        {
                            // since it's larger set our largest child to that + 1
                            HighestLayerOfChildren = layer;
                        }
                    }
                    else
                    {
                        // a node should not be able to 'receieve input' from an output node (in this context at least) - for cyclical DAGs and networks
                        // they should implement a cycle that is not explicitly represented in the innovation genome. idk tho i never went to college
                        throw Networks.Exceptions.CircularInnovationReference(id);
                    }
                }
                else
                {
                    // if we werent able to find the node in the dict we should recursively check to see if all their children are inputs, and if they are
                    // our layer would be 1 above theirs(or the highest child in our tree)
                    ushort val = GetAndSetHiddenLayer(ref id, ref Receivers, ref NodeDict, ref LayerDict);

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


            // set the id of the node
            if (NodeDict.ContainsKey(hiddenNodeId) is false)
            {
                NodeDict.Add(hiddenNodeId, HighestLayerOfChildren);
            }

            // make sure we add this to the dictionary we use to get the actual layer array at the end
            LayerDict.AppendToValue(ref HighestLayerOfChildren, ref hiddenNodeId);

            // return so we recurse properly
            return HighestLayerOfChildren;
        }

        public int[][] GetLayers(ref DecodedGenome DecodedGenotype)
        {
            // the layers for a particular genome is just the layer dictionary generated when we decoded the genome, but compacted into a multi dimensional array
            int size = DecodedGenotype.LayerDictionary.Count;

            int[][] result = new int[size][];

            Span<int[]> resultSpan = new(result);

            foreach (var item in DecodedGenotype.LayerDictionary)
            {
                resultSpan[item.Key] = item.Value;
            }

            return result;
        }

        public int[][] GetLayers(INeatNetwork network)
        {
            var decodedGenome = DecodeGenome(network);
            return GetLayers(ref decodedGenome);
        }

        static int TwoThreasholdValidation(ref int id, ref int LowerThreashold, ref int UpperThreashold, ReferenceIntAction WhenLowerThreashold, ReferenceIntAction WhenUpperThreashold, ReferenceIntAction WhenNeither)
        {
            // this may seem confusing but all this is checking is to see if a particular value is within two ranges across two checks
            // this is just abstracted with delagate to prevent unessesecary copying of value types(the integers)
            // in this example this would be if we were given the range upper = 5 lower = 3
            // |  OnLower   |  OnUpper  |      OnNeither     |
            // | 0  1   2   |  3    4   |  6 9 5 8 7 n...    |
            if (id < UpperThreashold)
            {

                if (id < LowerThreashold)
                {
                    WhenLowerThreashold(ref id);
                    return 2;
                }

                WhenUpperThreashold(ref id);
                return 1;
            }

            WhenNeither(ref id);

            return 0;
        }

        public DecodedGenome DecodeGenome(INeatNetwork network)
        {
            // we should generate the phenotype from innovations alone

            /*
                Innovation scheme:

                  innovation id(#)
                    from -> to
                      Weight
                     ENABLED

            using the above information we should be able to explictly represent the network purely through innovations

            */

            // setup the diverse set of dicts we need to calculate the genome explicitly and fast
            // we should keep track of any nodes that aren't input or output nodes, we will determine their exact place in the phenotype later
            int[] hiddenNodes = Array.Empty<int>();

            IDictionary<int/* Node Id */, int[]/* Node Id's it sends to */> Senders = new Dictionary<int, int[]>();
            IDictionary<int/* Node Id */, int[]/* Node Id's it receives from */> Receivers = new Dictionary<int, int[]>();

            IDictionary<int/* Node Id */, ushort/* Layer It's In */> NodeDict = new Dictionary<int, ushort>();
            IDictionary<ushort/* Layer */, int[]/* Nodes in layer */> LayerDict = new Dictionary<ushort, int[]>();

            // setup the delegates we will use to quickly determine and complete actions on nodes depending on the type of node they are
            // the index where that any n node thereafter is input + output
            // ex. if we have 3 input and 2 output nodes and n hidden nodes it could be:
            // |  input   |  output  |      hidden     |
            // | 0  1   2 |  3    4  |  6 9 5 8 7 n... |
            //         3  +  2   =   5
            int HiddenNodeIdThreashold = network.InputNodes + network.OutputNodes;
            int InputNodeThreashold = network.InputNodes;

            void WhenInputNode(ref int n)
            {
                NodeDict.TryAdd<int, ushort>(n, 1);
                LayerDict.AppendToValue<ushort, int>(1, ref n, true);
            }

            void WhenOutputNode(ref int n)
            {
                NodeDict.TryAdd<int, ushort>(n, 0);
                LayerDict.AppendToValue<ushort, int>(0, ref n, true);
            }

            void WhenHiddenNode(ref int n)
            {
                Helpers.Array.AppendValue(ref hiddenNodes, ref n, true);
            }


            // ~ O(n)
            // carve out some memory for the innovations so we can quickly access them and avoid cache misses
            Span<IInnovation> innovationSpan = new(network.Innovations);

            // add the input nodes
            for (int i = 0; i < InputNodeThreashold; i++)
            {
                WhenInputNode(ref i);
            }

            // add output nodes
            for (int i = InputNodeThreashold; i < HiddenNodeIdThreashold; i++)
            {
                WhenOutputNode(ref i);
            }


            // for the record im sorry for the ref keywords, but they save this functions performance by a non-negligible amount between ~1-6% 
            for (int i = 0; i < innovationSpan.Length; i++)
            {
                ref IInnovation inn = ref innovationSpan[i];

                // only enabled genes show up in the phenotype UNLESS they are input or output nodes, those are always included
                if (inn.Enabled is false)
                {
                    continue;
                }

                // copy variables to stack so we can use them as reference variables(but only once since we only use them as reference types from here one)
                int input = inn.InputNode;
                int output = inn.OutputNode;

                // record the target node that the from node sends to
                // this provides us a dict to lookup any given node to see an array of all nodes it references forward in the layers
                //await AddRecipientToSender(inn.InputNode, inn.OutputNode);
                Senders.AppendToValue(ref input, ref output);

                // record the sender node and record it as a node that sends to the given 'to' node.
                // this way given any node we can use this dictionary to get an array of all nodes that send to the given node
                //await AddSenderToRecipient(inn.OutputNode, inn.InputNode);
                Receivers.AppendToValue(ref output, ref input);

                // we should check to see if any nodes references in the innovation are input nodes or output nodes
                // we will know if 
                TwoThreasholdValidation(ref input, ref InputNodeThreashold, ref HiddenNodeIdThreashold, WhenInputNode, WhenOutputNode, WhenHiddenNode);
                TwoThreasholdValidation(ref output, ref InputNodeThreashold, ref HiddenNodeIdThreashold, WhenInputNode, WhenOutputNode, WhenHiddenNode);
            }

            // ~ O(i * n)
            // go through the hidden and find their exact layer
            // carve out some memory for the arr
            Span<int> hiddenSpan = new(hiddenNodes);
            for (int i = 0; i < hiddenSpan.Length; i++)
            {
                GetAndSetHiddenLayer(ref hiddenSpan[i], ref Receivers, ref NodeDict, ref LayerDict);
            }

            // now that we have what our input nodes, output nodes, and hidden nodes are and their exact location we can construct the matrix to represent the data
            // reverse the dictionary and return it
            return new DecodedGenome()
            {
                LayerDictionary = LayerDict,
                NodeDictionary = NodeDict,
                ReceieverDictionary = Receivers,
                SenderDictionary = Senders
            };
        }

        public IMatrix<double>[] GenerateMatrices(ref DecodedGenome genome)
        {
            // in order to construct a matrix from any arbitrary network we must use the following rules
            // i couldn't actually find any scholarly or math overflow help in representing arbitrary matrices, so i will actually have to write a research paper
            // on this becuase i may be the first(doesn't mean i discovered something new, i COULD theoretically be doing it wrong or in the WORST way possible and therefor i only found a different/wrong way, and by proxy i was not able to find anything on this becuase im stupid, who knows tho idk)

            /*
                  REPRESENTING ARBITRARY NETWORKS WITH MATRICES
                - no cyclical node references
                - no node may reference any previous node, they may only reference nodes in layers higher than theirs
                - nodes may reference any layer after their own
                - nodes need not reference any other node (their summed weight just wont be used)
                - nodes referencing layers further than the next layer must have their values forwarded to the next matrix
                - there must be at last 2 layers, an input layer and an output layer, each with at least 1 node.
                
                let network be:

                         3
                   4-----|
                   |     |
                 0 - 1   2
               
                 with  Genome:  0 -> 4 (w0)
                                1 -> 4 (w1)
                                4 -> 3 (w3) 
                                2 -> 3 (w2)

                matrix series needed to represent would be:
                        n
                m | w0  w1  0 |  x | 0 | = | 4 |
                  | 0   0   1 |    | 1 |   | 2 |
                                   | 2 |
                       n
                m  | w3 w2 |     x | 4 | = | 3 |
                                   | 2 |

                The number 𝛭 matrices needed to express an arbitrary network is the height of network (𝛈 - 1)

                The above matrix has 3 layers, an input, a hidden, and an output, therefor it's height is 𝛈 = 3 and the number of distinct matrices (𝛭) needed to represent it is 𝛭 = (𝛈 - 1) or 𝛭 = 2. Which is also displayed above.
                
                A matrix from any given layer 𝚲 with the number of nodes 𝛮 and any particular node being 𝝰 is defined as 
                
                Legend:   𝚲    - Layer
                          𝛮    - Number of nodes in a layer (𝚲)
                          𝝰    - Node
                        x ⥲ y  - x references y
                         ⌈x,y⌉   - floor(x,y)
                          m    - number of rows in matrix
                          n    - number of columns of a matrix
                          x_y  - x subscript(y)
                        
                -   m_𝚲 = 𝛮_(𝚲 + 1) + ( # of 𝝰 ⥲ 𝝰_(𝚲 > (𝚲 + 1))  )

                -   n_𝚲 = ⌈ 𝛮_𝚲 , m_(𝚲 - 1) ⌉ 

                
            */

            // get an easy to use multi dimensional array of the nodes of the network
            // note: the dictionarys mostly used for construction are in genome
            int[][] LayerArr = GetLayers(ref genome);


            // get the number of matrixes we need
            int MatricesNeeded = LayerArr.Length - 1;

            IMatrix<double>[] matrices = new IMatrix<double>[MatricesNeeded];

            // get an array of all the nodes in each layer that need to be forwarded to the next
            int[][] nodesToForward = GetForwardedNodes(ref LayerArr, ref genome);

            Span<int[]> Layers = new(LayerArr);

            for (int currentMatrix = 0; currentMatrix < MatricesNeeded; currentMatrix++)
            {
                // start from 1 since the input layer is stored at index 1 not 0 (output is stored at index 0)
                int layer = currentMatrix + 1;

                // we mod this value becuase the back of the array is actually [0] since that's the output layer
                int nextLayer = (layer + 1) % Layers.Length;
                // calculate the height and width of the matrix needed

                // default to the number of nodes in the next layer
                int rows = Layers[nextLayer].Length + nodesToForward[layer - 1].Length;

                // the number of columns of the matrix is EITHER the rows of the last matrix OR the number of nodes in this layer, whichever is greater
                // normally the number of rows in the last matrix is always = to the number of columns of this one so they can be multiplied, however
                // becuase values that are not used in a given layer have their values 'forwarded' until they need them, the size might not match up naturally

                int columns = Layers[layer].Length;

                // make sure that we account for forwarded values from the last matrix
                if (currentMatrix > 1)
                {
                    // if we are not the first matrix we should check the rows of the last matrix and make sure we dont need more room
                    int lastMatrixRows = matrices[currentMatrix - 1].Rows;
                    if (lastMatrixRows != columns)
                    {
                        columns = lastMatrixRows;
                    }
                }

                // add the created matrix to the array
                matrices[currentMatrix] = new Double.Matrix(rows, columns);
            }

            return matrices;
        }

        internal int[][] GetForwardedNodes(ref int[][] Layers, ref DecodedGenome genome)
        {
            int[][] forwardedLayers = Array.Empty<int[]>();
            for (int layer = 1; layer < Layers.Length; layer++)
            {
                if (TryGetNodesReferencingFarLayers(ref layer, ref Layers, ref genome, ref forwardedLayers, out var nodes))
                {
                    // since there are nodes that reference other layers add them to the array
                    Helpers.Array.AppendValue(ref forwardedLayers, ref nodes);
                }
                else
                {
                    // add an empty array since no nodes were found that reference othe layers
                    int[] empty = Array.Empty<int>();
                    Helpers.Array.AppendValue(ref forwardedLayers, ref empty);
                }
            }
            return forwardedLayers;
        }

        /// <summary>
        /// Determines if there are any nodes in the current layer that reference any nodes in any other layer that is <c>NOT</c> next layer.
        /// <code>
        /// Complexity: O(n * m) where m is the amount of nodes in the next layer and n is the amount of nodes in the current layer
        /// </code>
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="Layers"></param>
        /// <param name="genome"></param>
        /// <param name="forwardedLayers"></param>
        /// <param name="NodesReferencingFurtherLayers"></param>
        /// <returns>
        /// <see langword="true"/> when one or more node within <paramref name="layer"/> references a layer more than 1 layer away.
        /// <para>
        /// <see langword="false"/> when no nodes reference layers further than 1 layer away.
        /// </para>
        /// out <paramref name="NodesReferencingFurtherLayers"/> where the index is [<paramref name="layer"/> - 1]. Each array at the index holds the node ids of any nodes within the <paramref name="layer"/>.
        /// </returns>
        internal bool TryGetNodesReferencingFarLayers(ref int layer, ref int[][] Layers, ref DecodedGenome genome, ref int[][] forwardedLayers, out int[] NodesReferencingFurtherLayers)
        {


            // check to see if any of them reference nodes in layers that are not the next layer
            int nextLayer = layer + 1;

            // get nodes in the current layer
            int[] nodesArray = new int[Layers[layer].Length];

            // if there were previous nodes that were forwarded we should check to see if they need to be forwarded again

            bool hasLastLayer = forwardedLayers.Length >= layer - 2 && layer != 1;
            int numberOfAdditionalNodes = 0;

            if (hasLastLayer)
            {
                numberOfAdditionalNodes = forwardedLayers[layer - 2].Length;
                Array.Resize(ref nodesArray, nodesArray.Length + numberOfAdditionalNodes);
            }

            Span<int> nodes = new(nodesArray);
            Span<int> layerSpan = new(Layers[layer]);
            layerSpan.CopyTo(nodes);

            if (hasLastLayer)
            {
                Span<int> previouslyForwardedNodes = new(forwardedLayers[layer - 2]);
                previouslyForwardedNodes.CopyTo(nodes.Slice(layerSpan.Length, numberOfAdditionalNodes));
            }

            // empty the array
            NodesReferencingFurtherLayers = Array.Empty<int>();

            bool hasNodesReferencingFurtherLayers = false;

            for (int i = 0; i < nodes.Length; i++)
            {
                ref int node = ref nodes[i];

                // get the array of the nodes that this node sends to
                if (genome.SenderDictionary.ContainsKey(node))
                {
                    // check each one and make sure it's in the next layer, if it's not resize the array and append this node to it
                    Span<int> receiverSpan = new(genome.SenderDictionary[node]);
                    for (int receiverNode = 0; receiverNode < receiverSpan.Length; receiverNode++)
                    {
                        // check if it's layer is above the next layer
                        int receiverLayer = genome.NodeDictionary[receiverSpan[receiverNode]];

                        // a node needs to be forwarded if it references a node in a layer greater than the next or if it references the last layer(0)
                        // but not if the node is the last hidden layer and it's referencing the output layer
                        if (receiverLayer > nextLayer || ((layer != Layers.Length - 1) && receiverLayer == 0))
                        {
                            hasNodesReferencingFurtherLayers = true;
                            // since it's far away we should forward this nodes value as a new row in the matrix, append it's value to the array so we can keep track of it
                            Helpers.Array.AppendValue(ref NodesReferencingFurtherLayers, ref node);
                            break;
                        }
                    }
                }
            }

            return hasNodesReferencingFurtherLayers;
        }
    }
}
