using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class SpeciesEvaluationResult<T> : ISpeciesFitness<T>
    {
        public int Species { get; set; }
        public T Fitness { get; set; }
        public T[] Fitnesses { get; set; }
    }
}
