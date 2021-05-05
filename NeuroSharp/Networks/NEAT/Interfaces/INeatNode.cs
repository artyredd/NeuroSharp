using System;

namespace NeuroSharp.NEAT
{
    [Obsolete("Use INeatNetwork.NodeLayers and INeatNetwork.NodeDictionary")]
    public interface INeatNode
    {
        /// <summary>
        /// The node id
        /// </summary>
        ushort Id { get; }

        /// <summary>
        /// Gets the node type.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        /// The value of the node
        /// </summary>
        double? Value { get; set; }

        /// <summary>
        /// Gets; Sets; the innovations (nodes) that this node outputs to
        /// </summary>
        IInnovation[] OutputNodes { get; set; }

        /// <summary>
        /// Gets or sets the innovations that this node consumes from
        /// </summary>
        IInnovation[] InputNodes { get; set; }

        /// <summary>
        /// Hashes this node for comparisons with other nodes.
        /// </summary>
        /// <returns></returns>
        string Hash();

        /// <summary>
        /// Duplicates the node by value.
        /// </summary>
        /// <returns></returns>
        INeatNode Duplicate();
    }
}