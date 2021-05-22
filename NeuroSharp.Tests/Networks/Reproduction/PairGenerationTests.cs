using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests
{
    public class PairGenerationTests
    {
        private readonly IReproductionHandler<OrganismStruct> ReproductionHandler = new DefaultReproductionHandler<OrganismStruct>();

        [Theory]
        [InlineData(OrganismReproductionMethod.Sequential, 5, 1, 2, 3, 4, 5, 1, 2, 2, 3, 3, 4, 4, 5, 5, 1)]
        [InlineData(OrganismReproductionMethod.XOR, 5, 1, 2, 3, 4, 5, 1, 5, 2, 4, 3, 3, 4, 2, 5, 1)]
        public void GenerationWorks(OrganismReproductionMethod method, int count, params int[] inputsAndExpected)
        {
            var organismArray = new OrganismStruct[inputsAndExpected.Length];

            for (int i = 0; i < inputsAndExpected.Length; i++)
            {
                organismArray[i] = new OrganismStruct(inputsAndExpected[i], 1.0d);
            }

            Span<OrganismStruct> organisms = organismArray;

            Span<OrganismStruct> parents = organisms.Slice(0, count);

            var results = Span<(OrganismStruct Left, OrganismStruct Right)>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.ReproductionMethod = method;
                results = ReproductionHandler.GenerateBreedingPairs(ref parents);
            }

            // create the expected
            Span<OrganismStruct> expectedPacked = organisms[count..];

            Span<(OrganismStruct Left, OrganismStruct Right)> expected = new (OrganismStruct Left, OrganismStruct Right)[expectedPacked.Length / 2];

            int expectedIndex = 0;
            for (int i = 0; i < expectedPacked.Length; i += 2)
            {
                // 1 2 3 4 5 6 7
                expected[expectedIndex++] = (expectedPacked[i], expectedPacked[i + 1]);
            }

            Assert.Equal(-1, CompareSpans(expected, results));
        }

        [Fact]
        public void GenerationRandom()
        {
            var organismArray = new OrganismStruct[6];

            for (int i = 0; i < organismArray.Length; i++)
            {
                organismArray[i] = new OrganismStruct(i, 1.0d);
            }

            Span<OrganismStruct> organisms = organismArray;

            var results = Span<(OrganismStruct Left, OrganismStruct Right)>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.ReproductionMethod = OrganismReproductionMethod.Random;
                results = ReproductionHandler.GenerateBreedingPairs(ref organisms);
            }

            Assert.Equal(6, results.Length);

            // create sequential to comapre
            var NOTExpected = new Span<(OrganismStruct Left, OrganismStruct Right)>(new (OrganismStruct Left, OrganismStruct Right)[6]);

            int expectedIndex = 0;
            for (int i = 0; i < NOTExpected.Length; i += 2)
            {
                NOTExpected[expectedIndex++] = (organisms[i], organisms[i + 1]);
            }

            int comparison = CompareSpans(NOTExpected, results);

            Assert.NotEqual(-1, comparison);
            Assert.True(comparison <= 90);

        }

        [Fact]
        public void GenerationRandomSequential()
        {
            var organismArray = new OrganismStruct[6];

            for (int i = 0; i < organismArray.Length; i++)
            {
                organismArray[i] = new OrganismStruct(i + 1, 1.0d);
            }

            Span<OrganismStruct> organisms = organismArray;

            var results = Span<(OrganismStruct Left, OrganismStruct Right)>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.ReproductionMethod = OrganismReproductionMethod.RandomSequential;
                results = ReproductionHandler.GenerateBreedingPairs(ref organisms);
            }

            Assert.Equal(6, results.Length);

            // create sequential to comapre
            var Expected = new Span<(OrganismStruct Left, OrganismStruct Right)>(new (OrganismStruct Left, OrganismStruct Right)[]
            {
                (new OrganismStruct(1,1),new OrganismStruct(1,1)),
                (new OrganismStruct(2,1),new OrganismStruct(1,1)),
                (new OrganismStruct(3,1),new OrganismStruct(1,1)),
                (new OrganismStruct(4,1),new OrganismStruct(1,1)),
                (new OrganismStruct(5,1),new OrganismStruct(1,1)),
                (new OrganismStruct(6,1),new OrganismStruct(1,1))
            });

            // we just want to make sure that the left's are sequential
            for (int i = 0; i < Expected.Length; i++)
            {
                Assert.Equal(i + 1, results[i].Left);
            }
        }

        [Fact]
        public void GenerationRandomXOR()
        {
            var organismArray = new OrganismStruct[6];

            for (int i = 0; i < organismArray.Length; i++)
            {
                organismArray[i] = new OrganismStruct(i + 1, 1.0d);
            }

            Span<OrganismStruct> organisms = organismArray;

            var results = Span<(OrganismStruct Left, OrganismStruct Right)>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.ReproductionMethod = OrganismReproductionMethod.RandomXOR;
                results = ReproductionHandler.GenerateBreedingPairs(ref organisms);
            }

            Assert.Equal(6, results.Length);

            // make sure the left's are in XOR order(ish)
            //  1 2 3 4 5 6 7 8 9 10
            /*  1
                                  10
                    3
                              8
                        5
                          6
                            7
                      4
                                 9
                  2
            */
            Assert.Equal(1, results[0].Left);
            Assert.Equal(6, results[1].Left);
            Assert.Equal(3, results[2].Left);
            Assert.Equal(4, results[3].Left);
            Assert.Equal(5, results[4].Left);
            Assert.Equal(2, results[5].Left);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>
        /// <code>
        /// 91 if left is null/default
        /// </code>
        /// <code>
        /// 92 if right is null/default
        /// </code>
        /// <code>
        /// 93 if lengths do not match
        /// </code>
        /// <code>
        /// int index where spans don't match
        /// </code>
        /// <code>
        /// -1 Equivalent
        /// </code>
        /// </returns>
        private int CompareSpans<T>(Span<T> left, Span<T> right) where T : IComparable<T>, IEquatable<T>
        {
            if (left == null || left == default)
            {
                return 91;
            }

            if (right == null || right == default)
            {
                return 92;
            }

            if (left.Length != right.Length)
            {
                return 93;
            }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i].Equals(right[i]) is false)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
