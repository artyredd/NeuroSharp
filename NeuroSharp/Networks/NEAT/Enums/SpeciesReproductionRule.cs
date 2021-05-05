using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    /// <summary>
    /// Defines how a species should reproduce into the next generation of organisms
    /// </summary>
    public enum SpeciesReproductionRule
    {
        /// <summary>
        /// Allows a species to reproduce into the next generation.
        /// </summary>
        Allow,

        /// <summary>
        /// Allows the species to reproduce and cross over, but prohibits the species from mutating (other than inherited innovations)
        /// </summary>
        PreventMutation,

        /// <summary>
        /// Prevents mutation and reproduction of the species for the next generation
        /// </summary>
        Prohibit,

        /// <summary>
        /// Allows a species to mutate, but prohibits reproduction.
        /// </summary>
        ProhibitWithMutation,

        /// <summary>
        /// Randomly determines whether or not a species should be allowed to reproduce into the next generation
        /// </summary>
        Random,

        /// <summary>
        /// Prohibits the generation from reproducing and removes the species from the next generation all together.
        /// </summary>
        Remove
    }
}
