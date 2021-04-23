namespace NeuroSharp.NEAT
{
    public interface IInnovation
    {
        int Id { get; set; }

        bool Enabled { get; set; }

        ushort InputNode { get; init; }

        ushort OutputNode { get; init; }

        ushort InputNodeIndex { get; set; }

        ushort OutputNodeIndex { get; set; }

        double Weight { get; set; }

        string Hash();
    }
}