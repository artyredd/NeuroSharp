using System;

namespace NeuroSharp.NEAT
{
    public interface INeatNode
    {
        ushort Id { get; }

        NodeType NodeType { get; }

        IInnovation[] OutputNodes { get; set; }
        IInnovation[] InputNodes { get; set; }

        string Hash();

        INeatNode Duplicate();
    }
}