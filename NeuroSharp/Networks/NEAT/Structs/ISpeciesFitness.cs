namespace NeuroSharp.NEAT
{
    public interface ISpeciesFitness<T>
    {
        T Fitness { get; set; }
        T[] Fitnesses { get; set; }
        int Species { get; set; }
    }
}