using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.NEAT;

namespace NeuroSharp.Tests
{
    public class NeatNetworkComparerTests
    {
        static readonly DefaultNetworkComparer comparer = new();

        [Fact]
        public void AlignGenomesWorks()
        {
            // create two networks that are the same and create a connection between them
            NeatNueralNetwork left = new(1, 1);

            NeatNueralNetwork right = new(1, 1);

            var aligned = comparer.AlignGenomes(left, right);

            // we should expect a single aligned innovtion(actual count should be 2 since the array scheme holds both left and rights aligned innovations
            Assert.Equal(2, aligned.Aligned.Length);

            // make sure the connections are in the right order
            // array scheme for aligned is:
            // leftInnovation, rightInnovation, left...,Right...
            Assert.Equal(left.Innovations[0].Weight, aligned.Aligned[0].Weight);
            Assert.Equal(right.Innovations[0].Weight, aligned.Aligned[1].Weight);

            // now we should make the genome for left bigger causing excess
            // new left nn should be input --> hidden --> output
            ((DefaultMutater)(left.Mutater)).AddNode(left).Wait();

            // compare the genomes
            aligned = comparer.AlignGenomes(left, right);

            // when we compare these two its these two genomes compared:
            /*
                        left
                1          2         3
                0 -> 1   0 -> 2   3 -> 1
                disable   enable  enable

                        right
                1     
                0 -> 1
                enable
            */
            // expected 2 excess for left and 1 aligned and no disjoint
            Assert.Equal(2, aligned.Aligned.Length);

            Assert.Equal(2, aligned.Excess.Length);

            // now we should make sure the disjointed can be for the right as well
            aligned = comparer.AlignGenomes(right, left);

            Assert.Equal(2, aligned.Aligned.Length);

            Assert.Equal(2, aligned.Excess.Length);
        }

        [Fact]
        public void AlignGenomeAdvancedWorks()
        {
            // exmaple is https://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.28.5457&rep=rep1&type=pdf
            // on page 11

            // this is a duplicate test that tests disjoint as the other does not
            // create two neurual networks manually to ensure consistent results
            NeatNueralNetwork left = new(3, 1);
            left.Innovations = new IInnovation[] {
                new Innovation(){
                    Id = 1,
                    InputNode = 1,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 2,
                    InputNode = 2,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 4,
                    InputNode = 2,
                    OutputNode = 5,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 5,
                    InputNode = 3,
                    OutputNode = 5,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 6,
                    InputNode = 4,
                    OutputNode = 5,
                    Enabled = true,
                    Weight = 1
                },
            };
            // we use hashes to verify genes make sure to hash them
            foreach (var item in left.Innovations)
            {
                left.InnovationHashes.Add(item.Hash());
            }

            NeatNueralNetwork right = new(3, 1);
            right.Innovations = new IInnovation[] {
                new Innovation(){
                    Id = 1,
                    InputNode = 1,
                    OutputNode = 4,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 2,
                    InputNode = 2,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 3,
                    InputNode = 3,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 4,
                    InputNode = 2,
                    OutputNode = 5,
                    Enabled = false,
                    Weight = 1
                },
                new Innovation(){
                    Id = 6,
                    InputNode = 4,
                    OutputNode = 5,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 7,
                    InputNode = 1,
                    OutputNode = 6,
                    Enabled = true,
                    Weight = 1
                },
                new Innovation(){
                    Id = 8,
                    InputNode = 6,
                    OutputNode = 4,
                    Enabled = true,
                    Weight = 1
                },
            };

            // we use hashes to verify genes make sure to hash them
            foreach (var item in right.Innovations)
            {
                right.InnovationHashes.Add(item.Hash());
            }

            var aligned = comparer.AlignGenomes(left, right);

            // we should expect 4 aligned (8 count)
            Assert.Equal(8, aligned.Aligned.Length);

            // individually make sure every expected alignment was recorded
            Assert.True(aligned.Aligned[0].Id == 1);
            Assert.True(aligned.Aligned[2].Id == 2);
            Assert.True(aligned.Aligned[4].Id == 4);
            Assert.True(aligned.Aligned[6].Id == 6);

            // we should expect exactly 1 disjointed from the right parent
            Assert.Single(aligned.RightDisjoint);

            // it should have id of 3 per the example
            Assert.Equal(3, aligned.RightDisjoint[0].Id);

            // we should expect exactly 1 disjointed from the left parent
            Assert.Single(aligned.LeftDisjoint);

            // it should have an id of 5 per example
            Assert.Equal(5, aligned.LeftDisjoint[0].Id);

            // parent 2 is bigger and therefor should have excess genes
            Assert.Equal(2, aligned.Excess.Length);

            // the excess genes should have the ids of 7 and 8 per the diagram
            Assert.Equal(7, aligned.Excess[0].Id);
            Assert.Equal(8, aligned.Excess[1].Id);
        }
    }
}
