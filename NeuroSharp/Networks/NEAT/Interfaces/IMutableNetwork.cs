using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Defines an neural network that can be mutated during run time
    /// </summary>
    public interface IMutableNetwork
    {
        /// <summary>
        /// Reponsible for mutating the network during runtime. This would normally include things like modifying weights ect..
        /// </summary>
        public IMutater Mutater { get; init; }

        /// <summary>
        /// Method that should mutate the network in some way using the mutater
        /// </summary>
        /// <returns></returns>
        Task<MutationResult> Mutate();
    }
}