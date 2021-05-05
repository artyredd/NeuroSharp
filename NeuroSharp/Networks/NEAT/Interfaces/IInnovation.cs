using System.Diagnostics;
namespace NeuroSharp.NEAT
{
    public interface IInnovation
    {
        int Id { get; set; }

        bool Enabled { get; set; }

        ushort InputNode { get; init; }

        ushort OutputNode { get; init; }

        double Weight { get; set; }

        string Hash();
    }
}