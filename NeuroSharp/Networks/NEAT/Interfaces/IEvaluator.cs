namespace NeuroSharp.NEAT
{
    public interface IEvaluator
    {
        IActivationFunction<double> Activator { get; set; }
        IFitnessFunction<double> FitnessFunction { get; set; }

        double[] Evaluate(double[] Inputs, INeatNetwork network);
    }
}