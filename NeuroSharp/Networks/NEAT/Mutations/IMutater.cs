using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public interface IMutater
    {
        Task<MutationResult> Mutate(INeatNetwork network);
    }
}