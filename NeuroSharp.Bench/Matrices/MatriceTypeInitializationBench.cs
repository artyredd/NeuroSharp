using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    [SimpleJob(RunStrategy.Monitoring,
        warmupCount: 10, targetCount: 10)]
    public class MatriceTypeInitializationBench
    {
        [Params(10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29)]
        public int MatrixSize { get; set; }

        [Benchmark]
        public IMatrix<sbyte> SByteMatrix()
        {
            return new SByte.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<byte> ByteMatrix()
        {
            return new Byte.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<short> ShortMatrix()
        {
            return new Short.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<int> IntMatrix()
        {
            return new Int.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<long> LongMatrix()
        {
            return new Long.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<double> DoubleMatrix()
        {
            return new Double.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<float> FloatMatrix()
        {
            return new Float.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
        [Benchmark]
        public IMatrix<decimal> DecimalMatrix()
        {
            return new Decimal.Matrix(2 << MatrixSize, 2 << MatrixSize);
        }
    }
}
