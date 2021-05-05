using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public enum OrganismTruncationMethod
    {
        /// <summary>
        /// Does not remove badly performing organisms. &#9888; <c>No Reproduction will occur if the generation has reached it's hard limit </c> &#9888; 
        /// </summary>
        None,
        /// <summary>
        /// Removes the worst performing organism only.
        /// </summary>
        Single,
        /// <summary>
        /// Removes 1/4 (25%) of the badly performing organisms
        /// </summary>
        Quarter,
        /// <summary>
        /// Removes 1/3 (33%) of the badly performing organisms
        /// </summary>
        Third,
        /// <summary>
        /// Removes 1/2 (50%) of all organisms
        /// </summary>
        Half,
        /// <summary>
        /// Removes 2/3 (66%) of badly performing organisms
        /// </summary>
        TwoThirds,
        /// <summary>
        /// Removes 3/4  (75%) of badly performing organisms
        /// </summary>
        ThreeQuarters,
        /// <summary>
        /// Each organism has a 50% chance of being truncated &#9888; <c>regardless of fitness</c>  &#9888; 	
        /// </summary>
        Random,
        /// <summary>
        /// Use the <see cref="IReproductionHandler.CustomOrganismTruncationPercentage"/> as a custom threashold to truncate organisms
        /// <code> Default = 20% (0.20d)</code>
        /// </summary>
        Custom,
        /// <summary>
        /// Uses the <see cref="IReproductionHandler.CustomOrganismTruncater"/> to determine if an organism should be removed from a species. Increased overhead from passing the fitness and organism may be incurred. 
        /// <para>
        /// Where <see langword="true"/> is <c>TRUNCATE</c> and <see langword="false"/> is <c>DO NOT TRUNCATE</c>
        /// </para>
        /// Default bahavior is <see cref="OrganismTruncationMethod.Random"/>
        /// </summary>
        Bool,
    }
}
