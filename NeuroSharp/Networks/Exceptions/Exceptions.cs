using NeuroSharp.NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Networks
{
    public static class Exceptions
    {
        /// <summary>
        /// Returns exception:
        /// <code>
        /// Unexpected training data scheme provided. Expected ([12]) items but was only provided ([9]) items. Privided training data must match the number of input and/or output nodes.
        /// </code>
        /// </summary>
        /// <param name="Expected"></param>
        /// <param name="Actual"></param>
        /// <returns></returns>
        public static Exception InconsistentTrainingDataScheme(int Expected, int Actual)
        {
            return new ArgumentException($"Unexpected training data scheme provided. Expected ({Expected}) items but was only provided ({Actual}) items. Privided training data must match the number of input and/or output nodes.");
        }

        /// <summary>
        /// Returns exception:
        /// <code>
        /// Circular innovation Id(12) From(3) -> To(1). Innovations can not input from nodes above them in the network or output to nodes that are below them in the network.
        /// </code>
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns></returns>
        public static Exception CircularInnovationReference(IInnovation innovation)
        {
            return new StackOverflowException($"Circular innovation Id({innovation?.Id}) From({innovation?.InputNode}) -> To({innovation?.OutputNode}). Innovations can not input from nodes above them in the network or output to nodes that are below them in the network.");
        }

        /// <summary>
        /// Returns exception:
        /// <code>
        /// Circular innovation Id(12) From(3) -> To(1). Innovations can not input from nodes above them in the network or output to nodes that are below them in the network.
        /// </code>
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns></returns>
        public static Exception CircularInnovationReference(int innovationId)
        {
            return new StackOverflowException($"Circular innovation Id({innovationId}). Innovations can not input from nodes above them in the network or output to nodes that are below them in the network.");
        }

        /// <summary>
        /// Returns exception:
        /// <code>
        /// Node (1) Supposedly has a weight referencing OutputNode(2) but no innovation was found in the decoded genome.
        /// </code>
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns></returns>
        public static Exception MissingInnovationEntry(int inputNode, int outputNode)
        {
            return new KeyNotFoundException($"Node ({inputNode}) Supposedly has a weight referencing OutputNode({outputNode}) but no innovation was found in the decoded genome.");
        }
    }
}
