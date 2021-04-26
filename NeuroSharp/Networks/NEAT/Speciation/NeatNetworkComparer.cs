using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NeuroSharp.Tests")]
namespace NeuroSharp.NEAT
{
    public class NeatNetworkComparer : INeatNetworkComparer
    {
        public double ExcessCoefficient { get; set; } = 1.0d;
        public double DisjointCoefficient { get; set; } = 1.0d;
        public double WeightCoefficient { get; set; } = 1.0d;

        /// <summary>
        /// Calculates the compatibility of the left, when compared to the right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public double CalculateCompatibility(INeatNetwork left, INeatNetwork right)
        {
            var comparedGenome = AlignGenomes(left, right);

            // https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf page 11

            // The number of excess and disjoint genes between a pair of genomes is a natural measure of their com-patibility distance.  The more disjoint two genomes are, theless evolutionary history they share, and thusthe less compatible they are. Therefore, we can measure the compatibility distanceÆof different structuresin NEAT as a simple linear combination of the number of excess(E) and disjoint (D) genes, as well as theaverage weight differences of matching genes (W), including disabled genes. The coefficients, ExcessCoefficient, DisjointCoefficient, and WeightCoefficient, allow us to adjust the importance of the three factors, and the factor N, the number of genes in the larger genome, normalizes for genome size

            int N = left.Innovations.Length >= right.Innovations.Length ? left.Innovations.Length : right.Innovations.Length;

            double excess = ExcessCoefficient * comparedGenome.Excess.Length;

            double disjoint = DisjointCoefficient * (comparedGenome.LeftDisjoint.Length + comparedGenome.RightDisjoint.Length);

            double averageWeight = AverageWeightDifference(comparedGenome.Aligned);

            excess /= N;

            disjoint /= N;

            return excess + disjoint + (WeightCoefficient * averageWeight);
        }

        /// <summary>
        /// Crosses two parents to derive a new genome for a new child <see cref="INeatNetwork"/>
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="fitnessState"></param>
        /// <returns></returns>
        public IInnovation[] DeriveGenome(INeatNetwork left, INeatNetwork right, FitnessState fitnessState)
        {
            // align the genomes so we can select and create a new genome
            var GenomeMatches = AlignGenomes(left, right);

            // per https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf pg 12
            //  Matching genes are inherited randomly, whereas disjoint genes(those that do not match in the middle) and excess genes (those that do not match in the end) are inherited from the more fit parent. Whith equal fitnesses, disjoint and excess genes are also inherited randomly.

            // if the right length is larger that means any excess in the genome belongs to the right
            int additionalGenomeSize = 0;

            bool inheritExcess = false;

            // check to see who is responsible for the excess genes
            bool rightExcess = right.Innovations.Length > left.Innovations.Length;

            // determine if there is any excess or disjoint genes to inherit
            if (GenomeMatches.Excess.Length != 0 || GenomeMatches.LeftDisjoint.Length != 0 || GenomeMatches.RightDisjoint.Length != 0)
            {
                switch (fitnessState)
                {
                    // if the right is more fit and the excess belongs to the right inherit the excess and the right disjoints
                    case FitnessState.RightMoreFit:

                        inheritExcess = rightExcess;

                        if (inheritExcess)
                        {
                            additionalGenomeSize = GenomeMatches.Excess.Length + GenomeMatches.RightDisjoint.Length;
                        }

                        break;

                    // if the left is more fit and the excess belongs to the left inherit the excess and the left disjoints
                    case FitnessState.LeftMoreFit:

                        inheritExcess = !rightExcess;

                        if (inheritExcess)
                        {
                            additionalGenomeSize = GenomeMatches.Excess.Length + GenomeMatches.LeftDisjoint.Length;
                        }

                        break;
                    // if both are equally fit randomly determine whether or not to inherit the excess
                    case FitnessState.EqualFitness:

                        inheritExcess = Helpers.NextUDoubleAsync().Result > 0.5d;

                        if (inheritExcess)
                        {
                            additionalGenomeSize = GenomeMatches.Excess.Length;

                            if (rightExcess)
                            {
                                additionalGenomeSize += GenomeMatches.RightDisjoint.Length;
                            }
                            else
                            {
                                additionalGenomeSize += GenomeMatches.LeftDisjoint.Length;
                            }

                        }

                        break;
                }
            }

            int startingIndex = GenomeMatches.Aligned.Length >> 1;

            IInnovation[] result = new IInnovation[startingIndex + additionalGenomeSize];

            Span<IInnovation> newGenome = new(result);

            // divide the length by 2 since the array scheme holds both left and right aligned genes
            for (int i = 0; i < startingIndex; i++)
            {
                // randomly select genes
                if (Helpers.NextUDoubleAsync().Result > 0.5d)
                {
                    // select the left gene
                    newGenome[i] = GenomeMatches.Aligned[i << 1];
                    // left right left right left right
                    //  0      1    2    3     4    5
                    // left = i * 2
                }
                else
                {
                    // select the right gene
                    newGenome[i] = GenomeMatches.Aligned[(i << 1) + 1];
                    // left right left right left right
                    //  0      1    2    3     4    5
                    // right = i * 2 + 1
                }
            }

            // now that we have inserted all of the randomly chosen aligned genes we should add all of the excess and disjoint(if we determined earlier that it's appropriate
            if (inheritExcess)
            {
                // the size of the destination array has already been assigned so we dont have to resize and carve out another coiontiguous block of memory for the array
                Span<IInnovation> excess = new(GenomeMatches.Excess);
                // copy the excess over to the destination array, we dont have to figure out who it belongs to yet becuase we only get 1 parents excess form the align genes method
                excess.CopyTo(newGenome.Slice(startingIndex, excess.Length));

                // now copy over the disjointed values
                if (rightExcess)
                {
                    Span<IInnovation> disjointed = new(GenomeMatches.RightDisjoint);
                    disjointed.CopyTo(newGenome.Slice(startingIndex + excess.Length));
                }
                else
                {
                    Span<IInnovation> disjointed = new(GenomeMatches.LeftDisjoint);
                    disjointed.CopyTo(newGenome.Slice(startingIndex + excess.Length));
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the weight differences between the two provided genome.
        /// </summary>
        /// <param name="Genomes"></param>
        /// <returns></returns>
        internal double AverageWeightDifference(Span<IInnovation> AlignedGenomes)
        {
            double averageDifference = 0.0d;

            // array scheme is left right left right left right, always even
            for (int i = 0; i < AlignedGenomes.Length - 2; i += 2)
            {
                averageDifference += AlignedGenomes[i + 1].Weight - AlignedGenomes[i].Weight;
            }

            return averageDifference / (AlignedGenomes.Length >> 1);
        }

        /// <summary>
        /// Calculates the weight differences between the two provided genome.
        /// </summary>
        /// <param name="Genomes"></param>
        /// <returns></returns>
        internal double AverageWeightDifference(IInnovation[] AlignedGenomes) => AverageWeightDifference(new Span<IInnovation>(AlignedGenomes));

        /// <summary>
        /// Aligns two genomes together so they can be compared and used to derive another genome
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        internal (IInnovation[] Aligned, IInnovation[] Excess, IInnovation[] LeftDisjoint, IInnovation[] RightDisjoint) AlignGenomes(INeatNetwork left, INeatNetwork right)
        {
            IInnovation[] Aligned = Array.Empty<IInnovation>();
            int AlignedIndex = 0;

            IInnovation[] LeftDisjoint = Array.Empty<IInnovation>();
            int LeftDisjointIndex = 0;

            IInnovation[] RightDisjoint = Array.Empty<IInnovation>();
            int RightDisjointIndex = 0;

            IInnovation[] Excess = Array.Empty<IInnovation>();
            int ExcessIndex = 0;

            Span<IInnovation> leftInnovations = new(left.Innovations);

            int index = 0;

            foreach (var item in leftInnovations)
            {
                index++;
                // check to see if the innovation is contained in both networks, if it is, then it is an aligned gene
                if (right.InnovationHashes.Contains(item.Hash()))
                {
                    // since the hash is in the others list then its aligned
                    Array.Resize(ref Aligned, ++AlignedIndex);
                    Aligned[AlignedIndex - 1] = item;


                    // leave an emtpy spot for the rights innovation
                    Array.Resize(ref Aligned, ++AlignedIndex);
                    Aligned[AlignedIndex - 1] = null;

                }
                else
                {
                    // check to see if we are past the end of the rights array, if we are then all innovations thereafter are considered disjoint
                    if (index >= right.Innovations.Length)
                    {
                        // excess
                        Array.Resize(ref Excess, ++ExcessIndex);
                        Excess[ExcessIndex - 1] = item;
                    }
                    else
                    {
                        // disjoint
                        Array.Resize(ref LeftDisjoint, ++LeftDisjointIndex);
                        LeftDisjoint[LeftDisjointIndex - 1] = item;
                    }
                }
            }

            Span<IInnovation> rightInnovations = new(right.Innovations);

            index = 0;
            AlignedIndex = 1;
            ExcessIndex = 0;

            foreach (var item in rightInnovations)
            {
                index++;
                // check to see if the innovation is contained in both networks, if it is, then it is an aligned gene
                if (left.InnovationHashes.Contains(item.Hash()))
                {
                    // since the hash is in the others list then its aligned
                    Aligned[AlignedIndex] = item;

                    AlignedIndex += 2;
                }
                else
                {
                    // check to see if we are past the end of the rights array, if we are then all innovations thereafter are considered disjoint
                    if (index >= left.Innovations.Length)
                    {
                        // excess
                        Array.Resize(ref Excess, ++ExcessIndex);
                        Excess[ExcessIndex - 1] = item;
                    }
                    else
                    {
                        // disjoint
                        Array.Resize(ref RightDisjoint, ++RightDisjointIndex);
                        RightDisjoint[RightDisjointIndex - 1] = item;
                    }
                }
            }

            return (Aligned, Excess, LeftDisjoint, RightDisjoint);
        }
    }
}
