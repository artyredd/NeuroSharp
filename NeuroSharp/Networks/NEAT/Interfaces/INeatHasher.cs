namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Responsible for hashing objects values so they can be tracked in genomes.
    /// </summary>
    public interface INeatHasher
    {
        string Hash(IInnovation node);
        string Hash(int InputNode, int OutputNode);
    }
}