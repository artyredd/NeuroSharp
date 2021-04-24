using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroSharp
{
    public static class Helpers
    {
        private static readonly Random Rng = new();
        private static readonly SemaphoreSlim Limiter = new(1, 1);
        private static int[] DoubleRange = { -1, 1 };

        /// <summary>
        /// Returns a random double between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            try
            {
                Limiter.Wait();
                int Sign = DoubleRange[Rng.Next(0, 2)];
                return Sign * Rng.NextDouble();
            }
            finally
            {
                Limiter.Release();
            }
        }

        public static async Task<double> NextUDoubleAsync()
        {
            try
            {
                await Limiter.WaitAsync();
                return Rng.NextDouble();
            }
            finally
            {
                Limiter.Release();
            }
        }

        public static async Task<double> NextDoubleAsync()
        {
            try
            {
                await Limiter.WaitAsync();
                int Sign = DoubleRange[Rng.Next(0, 2)];
                return Sign * Rng.NextDouble();
            }
            finally
            {
                Limiter.Release();
            }
        }

        public static async Task<int> NextAsync(int lower = int.MinValue, int upper = int.MaxValue)
        {
            try
            {
                await Limiter.WaitAsync();
                return Rng.Next(lower, upper);
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <summary>
        /// Determines if the given type and parameters would exceed memory limits.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Rows"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public static (bool ExceedsSize, long EstimatedSize, long AvailableMemory) PotentialSizeExceedsTotalMemory<T>(int Rows, int Columns)
        {

            long bits = Rows * Columns;

            T def = default;

            const int _byte = 3;
            const int _16bit = 4;
            const int _32bit = 5;
            const int _64bit = 6;

            switch (def)
            {
                case sbyte _:
                    bits <<= _byte;
                    break;
                case byte _:
                    bits <<= _byte;
                    break;
                case short _:
                    bits <<= _16bit;
                    break;
                case ushort _:
                    bits <<= _16bit;
                    break;
                case int _:
                    bits <<= _16bit;
                    break;
                case uint _:
                    bits <<= _32bit;
                    break;
                case long _:
                    bits <<= _64bit;
                    break;
                case ulong _:
                    bits <<= _64bit;
                    break;
                case nint _:
                    bits <<= _64bit;
                    break;
                case nuint _:
                    bits <<= _64bit;
                    break;
                case float _:
                    // 4 bytes
                    bits <<= _byte;
                    bits *= 4;
                    break;
                case double _:
                    // 8 bytes
                    bits <<= _byte;
                    bits *= 8;
                    break;
                case decimal _:
                    // 16 bytes
                    bits <<= _byte;
                    bits *= 16;
                    break;
                case bool _:
                    bits <<= _byte;
                    break;
                case char _:
                    bits <<= _16bit;
                    break;
                default:
                    break;
            }

            // get the object that contains info about the memory on the running machine
            var memoryInfo = GC.GetGCMemoryInfo();

            var AvailableMemory = memoryInfo.TotalAvailableMemoryBytes;

            // convert memory to bits
            AvailableMemory *= 8;

            if (bits * 8 >= AvailableMemory)
            {
                return (true, bits, AvailableMemory);
            }

            return (false, bits, AvailableMemory);
        }
    }
}
