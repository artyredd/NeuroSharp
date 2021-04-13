using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NeuroSharp.Int;

namespace NeuroSharp.Tests
{
    public class IntMatrixTests
    {
        [Fact]
        public void MultiplyOperatorWorks()
        {
            var matrix = new Matrix(2, 2, new int[] { 0, 1, 2, 3 });

            matrix *= 2;

            Assert.Equal(0, matrix[0, 0]);
            Assert.Equal(2, matrix[0, 1]);
            Assert.Equal(6, matrix[1, 1]);
            Assert.Equal(4, matrix[1, 0]);
        }
    }
}
