using NeuroSharp.NEAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests
{
    public class Truncation
    {
        private readonly IReproductionHandler ReproductionHandler = new DefaultReproductionHandler();
        [Theory]
        [InlineData(OrganismTruncationMethod.None, 100, 0)]
        [InlineData(OrganismTruncationMethod.Single, 100, 1)]
        [InlineData(OrganismTruncationMethod.Quarter, 100, 75)]
        [InlineData(OrganismTruncationMethod.Third, 100, 66)]
        [InlineData(OrganismTruncationMethod.Half, 100, 50)]
        [InlineData(OrganismTruncationMethod.TwoThirds, 100, 33)]
        [InlineData(OrganismTruncationMethod.ThreeQuarters, 100, 25)]
        public void TruncateDefaultWorks(OrganismTruncationMethod method, int count, int expected)
        {
            // what were testing here is
            // make sure the truncation only truncates the set amount
            // make sure the remaining and the truncated amounts are expected

            var organismArray = new OrganismStruct[count];
            Array.Fill(organismArray, new OrganismStruct(0, 1.2f));

            // make sure it had the right elements to begin with
            Assert.Equal(count, organismArray.Length);

            Span<OrganismStruct> organisms = organismArray;

            // truncate
            Span<OrganismStruct> result = Span<OrganismStruct>.Empty;
            Span<OrganismStruct> remaining = Span<OrganismStruct>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.TrucationMethod = method;

                result = ReproductionHandler.TruncateSpecies(ref organisms, out remaining);
            }

            Assert.Equal(expected, result.Length);
            Assert.Equal(count - expected, remaining.Length);
        }

        [Theory]
        [InlineData(1.0d, 100)]
        [InlineData(0.9d, 100)]
        [InlineData(0.8d, 100)]
        [InlineData(0.7d, 100)]
        [InlineData(0.6d, 100)]
        [InlineData(0.5d, 100)]
        [InlineData(0.4d, 100)]
        [InlineData(0.3d, 100)]
        [InlineData(0.2d, 100)]
        [InlineData(0.1d, 100)]
        [InlineData(0d, 100)]
        [InlineData(1.1d, 100, 100)]
        [InlineData(1.9d, 100, 100)]
        [InlineData(1000000d, 100, 100)]
        [InlineData(-1f, 100, 0)]
        [InlineData(-99999f, 100, 0)]
        [InlineData(double.MaxValue, 100, 0)]
        [InlineData(double.MinValue, 100, 0)]
        public void CustomWorks(double percentage, int count, int? expected = null)
        {
            // what were testing here is
            // make sure the truncation only truncates the set amount
            // make sure the remaining and the truncated amounts are expected

            var organismArray = new OrganismStruct[count];
            Array.Fill(organismArray, new OrganismStruct(0, 1.2f));

            // make sure it had the right elements to begin with
            Assert.Equal(count, organismArray.Length);

            Span<OrganismStruct> organisms = organismArray;

            // truncate
            Span<OrganismStruct> result = Span<OrganismStruct>.Empty;
            Span<OrganismStruct> remaining = Span<OrganismStruct>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.TrucationMethod = OrganismTruncationMethod.Custom;

                ReproductionHandler.CustomOrganismTruncationPercentage = percentage;

                result = ReproductionHandler.TruncateSpecies(ref organisms, out remaining);
            }

            expected ??= (int)(percentage * count);

            Assert.Equal(expected, result.Length);
            Assert.Equal(count - expected, remaining.Length);
        }

        [Fact]
        public void RandomWorks()
        {
            // what were testing here is
            // make sure the truncation only truncates the set amount
            // make sure the remaining and the truncated amounts are expected

            var organismArray = new OrganismStruct[100];
            Array.Fill(organismArray, new OrganismStruct(0, 1.2f));

            // make sure it had the right elements to begin with
            Assert.Equal(100, organismArray.Length);

            Span<OrganismStruct> organisms = organismArray;

            // truncate
            Span<OrganismStruct> result = Span<OrganismStruct>.Empty;
            Span<OrganismStruct> remaining = Span<OrganismStruct>.Empty;

            lock (ReproductionHandler)
            {
                ReproductionHandler.TrucationMethod = OrganismTruncationMethod.Random;

                result = ReproductionHandler.TruncateSpecies(ref organisms, out remaining);
            }

            // just make sure the amount that is returned is proportional
            Assert.Equal(100 - result.Length, remaining.Length);
        }

        [Fact]
        public void BoolWorks()
        {
            // what were testing here is
            // make sure the truncation only truncates the set amount
            // make sure the remaining and the truncated amounts are expected

            var organismArray = new OrganismStruct[100];
            Array.Fill(organismArray, new OrganismStruct(0, 1.2f));

            // make sure it had the right elements to begin with
            Assert.Equal(100, organismArray.Length);

            Span<OrganismStruct> organisms = organismArray;

            // truncate
            Span<OrganismStruct> result = Span<OrganismStruct>.Empty;
            Span<OrganismStruct> remaining = Span<OrganismStruct>.Empty;


            int x = 0;
            lock (ReproductionHandler)
            {
                ReproductionHandler.TrucationMethod = OrganismTruncationMethod.Bool;

                ReproductionHandler.CustomOrganismTruncater = (ref OrganismStruct organism)
                =>
                {
                    return x++ < 50;
                };

                result = ReproductionHandler.TruncateSpecies(ref organisms, out remaining);
            }

            // since our custom truncator just keesp the ones before 50 and throws away the ones after 50 we should get two 50's
            Assert.Equal(result.Length, remaining.Length);
            Assert.Equal(50, result.Length);
        }
    }
}
