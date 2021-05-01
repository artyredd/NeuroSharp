using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NeuroSharp.Tests.Networks
{
    public class NeuralNetworkTests
    {
        [Fact]
        public void Contrustor_Normal_Works()
        {
            var nn = new NeuralNetwork(1000, 6, 10, 7, 9, 4);

            Assert.Equal(1000, nn.InputNodes.Weights.Columns);

            Assert.Equal(10, nn.InputNodes.Weights.Rows);

            Assert.Equal(10, nn.HiddenNodes[0].Weights.Columns);
            Assert.Equal(7, nn.HiddenNodes[0].Weights.Rows);

            Assert.Equal(7, nn.HiddenNodes[1].Weights.Columns);
            Assert.Equal(9, nn.HiddenNodes[1].Weights.Rows);

            Assert.Equal(9, nn.HiddenNodes[2].Weights.Columns);
            Assert.Equal(4, nn.HiddenNodes[2].Weights.Rows);

            Assert.Equal(4, nn.HiddenNodes[3].Weights.Columns);
            Assert.Equal(6, nn.HiddenNodes[3].Weights.Rows);
        }

        [Theory]
        [InlineData(2, 2, 2)]
        [InlineData(1000, 20, 70)]
        public void CheckInput_Returns_Valid(int Inputs, int Outputs, params int[] HiddenLayers)
        {
            // create arbitrary nn
            var nn = new NeuralNetwork(Inputs, Outputs, HiddenLayers);
            Random rng = new();

            double[] input = new double[Inputs];
            for (int i = 0; i < Inputs; i++)
            {
                input[i] = rng.NextDouble();
            }

            var result = nn.CheckInput(input)?.ToOneDimension();

            Assert.NotNull(result);

            Assert.True(result.Length != 0);
            Assert.Equal(Outputs, result.Length);

            // highly unlikely to get a 0 or 1 from the final output since we use random 64bit biases and weights, it is however technically possible therefor this unit test may fail occasionally fyi
            Assert.NotEqual(1d, result[0]);

            Assert.NotEqual(0d, result[0]);

            // since the default activation function is a sigmoid verify that the results are within -1 - 1
            foreach (var item in result)
            {
                Assert.True(item > -1d);
                Assert.True(item < 1d, $"Value outise Sigmoid Bounds (-1,1) Value: {item}");
            }
        }

        //[Fact]
        //public void TrainingInputWorks()
        //{
        //    var nn = new NeuralNetwork(2, 1, 2);


        //    double[] TrainingData = { 1, 0 };
        //    double[] Expected = { 1 };

        //    _ = nn.CheckInput(TrainingData);

        //    IMatrix<double> result = nn.TrainingInput(TrainingData, Expected);

        //    _ = result;

        //    // train nn with data and  verify that it's getting better over time
        //    nn.TrainingRate = 0.05;

        //    List<double[]> TrainingDataAll = new()
        //    {
        //        new double[] { 0, 1 },
        //        new double[] { 1, 0 },
        //        new double[] { 1, 1 },
        //        new double[] { 0, 0 },
        //    };
        //    List<double[]> ExpectedData = new()
        //    {
        //        new double[] { 1 },
        //        new double[] { 1 },
        //        new double[] { 0 },
        //        new double[] { 0 },
        //    };

        //    Random rng = new();

        //    double initialCheck = 1 - nn.CheckInput(TrainingData)[0, 0];

        //    for (int i = 0; i < 16; i++)
        //    {
        //        int index = rng.Next(0, 4);
        //        nn.TrainingInput(TrainingDataAll[index], ExpectedData[index]);
        //    }

        //    var r = nn.CheckInput(TrainingData);

        //    double Check = 1 - r[0, 0];

        //    Assert.True(Check > initialCheck);
        //}
    }
}
