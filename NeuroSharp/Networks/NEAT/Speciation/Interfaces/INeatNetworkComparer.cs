namespace NeuroSharp.NEAT
{
    public interface INeatNetworkComparer
    {
        double DisjointCoefficient { get; set; }
        double ExcessCoefficient { get; set; }
        double WeightCoefficient { get; set; }

        double CalculateCompatibility(INeatNetwork left, INeatNetwork right);
        IInnovation[] DeriveGenome(INeatNetwork left, INeatNetwork right, FitnessState fitnessState);
    }
}