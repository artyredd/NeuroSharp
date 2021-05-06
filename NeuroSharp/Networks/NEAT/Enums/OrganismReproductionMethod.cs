using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Defines how pairs are selected for cross-breeding in a species of organisms
    /// </summary>
    public enum OrganismReproductionMethod
    {
        /// <summary>
        /// Two parent organisms are chosen randomly
        /// <para>
        /// Example, parents in oder by fitness
        /// </para>
        /// <code>
        /// 6 5 4 3 2 1 0
        /// </code>
        /// Parent Groups:
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// <code>
        /// (random organism), (random organism)
        /// </code>
        /// </summary>
        Random,
        /// <summary>
        /// Each organism starting from greatest performing, is randomly cross bred with any other organism
        /// <para>
        /// Example, parents in oder by fitness
        /// </para>
        /// <code>
        /// 6 5 4 3 2 1 0
        /// </code>
        /// Parent Groups:
        /// <code>
        /// 6, (random organism)
        /// </code>
        /// <code>
        /// 5, (random organism)
        /// </code>
        /// <code>
        /// 4, (random organism)
        /// </code>
        /// <code>
        /// 3, (random organism)
        /// </code>
        /// <code>
        /// 2, (random organism)
        /// </code>
        /// <code>
        /// 1, (random organism)
        /// </code>
        /// <code>
        /// 0, (random organism)
        /// </code>
        /// </summary>
        RandomSequential,
        /// <summary>
        /// One parent is chosen alternating greatest and least. With each iteration moving closer to the middle performing organism. The other parent is chosen randomly
        /// <para>
        /// Example, parents in oder by fitness
        /// </para>
        /// <code>
        /// 6 5 4 3 2 1 0
        /// </code>
        /// Parent Groups:
        /// <code>
        /// 6, (random organism)
        /// </code>
        /// <code>
        /// 0, (random organism)
        /// </code>
        /// <code>
        /// 5, (random organism)
        /// </code>
        /// <code>
        /// 1, (random organism)
        /// </code>
        /// <code>
        /// 4, (random organism)
        /// </code>
        /// <code>
        /// 2, (random organism)
        /// </code>
        /// <code>
        /// 3, (random organism)
        /// </code>
        /// </summary>
        RandomXOR,
        /// <summary>
        /// The two parent organisms are chosen one after another in order of fitnesses greatest to least, every organism is cross bred with the organism to the left and to the right of it
        /// <para>
        /// Example, parents in oder by fitness
        /// </para>
        /// <code>
        /// 6 5 4 3 2 1 0
        /// </code>
        /// Parent Groups:
        /// <code>
        /// 6, 5
        /// </code>
        /// <code>
        /// 5, 4
        /// </code>
        /// <code>
        /// 4, 3
        /// </code>
        /// <code>
        /// 3, 2
        /// </code>
        /// <code>
        /// 2, 1
        /// </code>
        /// <code>
        /// 1, 0
        /// </code>
        /// </summary>
        Sequential,
        /// <summary>
        /// The two parent organism are chosen by opposite fitnesses, greatest with least, second greatest with second least
        /// <para>
        /// Example, parents in oder by fitness
        /// </para>
        /// <code>
        /// 6 5 4 3 2 1 0
        /// </code>
        /// Parent Groups:
        /// <code>
        /// 6, 0
        /// </code>
        /// <code>
        /// 5, 1
        /// </code>
        /// <code>
        /// 4, 2
        /// </code>
        /// <code>
        /// 3, 2 OR 3, 4
        /// </code>
        /// </summary>
        XOR
    }
}
