using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Responsible for mutating <see cref="INeatNetwork"/>s during runtime.
    /// </summary>
    public interface IMutater
    {
        /// <summary>
        /// The chance each individual weight has of being completely reassigned a random new value between Min(-1d) - Max(1d)
        /// <code>
        /// Rolled for every weight.
        /// </code>
        /// </summary>
        double WeightReassignmentChance { get; set; }

        /// <summary>
        /// The chance each individual weight has of being mutated by <see cref="WeightMutationModifier"/> * Min(-1d) - Max(1d).
        /// <code>
        /// Rolled for every weight.
        /// </code>
        /// </summary>
        double MutateWeightChance { get; set; }

        /// <summary>
        /// The change that a mutation will also include adding a connection. This value must be greater than <see cref="AddNodeChance"/>.
        /// <code>
        /// Rolled once per mutation
        /// </code>
        /// </summary>
        double AddConnectionChance { get; set; }

        /// <summary>
        /// The chance that a mutation wil include adding a node. This value must be less than <see cref="AddConnectionChance"/>.
        /// <code>
        /// Rolled once per mutation
        /// </code>
        /// </summary>
        double AddNodeChance { get; set; }

        /// <summary>
        /// The modifier that limits the amount a weight is preturbed when it is mutated. Only applies to <see cref="MutateWeightChance"/>
        /// </summary>
        double WeightMutationModifier { get; set; }

        /// <summary>
        /// Mutates a <see cref="INeatNetwork"/> by either changing weights, adding a connection. Or adding a node. 
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        Task<MutationResult> Mutate(INeatNetwork network);
        Task<InitializeNetworkResult> InitializeConnections(INeatNetwork network);
    }
}