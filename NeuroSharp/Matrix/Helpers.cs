﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroSharp.Helpers
{
    public static class Random
    {
        private static readonly System.Random Rng = new();
        private static readonly SemaphoreSlim Limiter = new(1, 1);
        private static readonly int[] DoubleRange = { -1, 1 };

        /// <summary>
        /// Sleeps for 10 (or <paramref name="ms"/>) milliseconds(1/1000 of a second)
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task Sleep(int ms = 10)
        {
            await Task.Run(() => Thread.Sleep(ms));
        }

        /// <summary>
        /// Returns a random double between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static int Next(int lower = int.MinValue, int upperExclusive = int.MaxValue)
        {
            try
            {
                Limiter.Wait();
                return Rng.Next(lower, upperExclusive);
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <summary>
        /// Returns a random double between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
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

        /// <summary>
        /// Returns a random double between 0d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static double NextUDouble() => NextUDoubleAsync().Result;

        /// <summary>
        /// Returns a random double between 0d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
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

        /// <summary>
        /// Returns a random double between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task<double> NextDoubleAsync()
        {
            try
            {
                await Limiter.WaitAsync();
                return GetSignedDouble();
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <summary>
        /// Returns a random double between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        private static double GetSignedDouble()
        {
            int Sign = DoubleRange[Rng.Next(0, 2)];
            return Sign * Rng.NextDouble();
        }

        /// <summary>
        /// Returns a random double[] with each value being between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task<double[]> NextDoubleArray(int size)
        {
            try
            {
                await Limiter.WaitAsync();

                static double[] GenerateArray(int size)
                {
                    double[] result = new double[size];
                    Span<double> span = new(result);
                    for (int i = 0; i < span.Length; i++)
                    {
                        span[i] = GetSignedDouble();
                    }
                    return result;
                }

                return GenerateArray(size);
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <summary>
        /// Returns a random double[] with each value being between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task<double[]> NextUDoubleArray(int size)
        {
            try
            {
                await Limiter.WaitAsync();

                static double[] GenerateArray(int size)
                {
                    double[] result = new double[size];
                    Span<double> span = new(result);
                    for (int i = 0; i < span.Length; i++)
                    {
                        span[i] = Rng.NextDouble();
                    }
                    return result;
                }

                return GenerateArray(size);
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <summary>
        /// Returns a random double[] with each value being between -1d and 1d.
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task<int[]> NextIntArrayAsync(int size, int lower = int.MinValue, int upperExclusive = int.MaxValue)
        {
            try
            {
                await Limiter.WaitAsync();

                static int[] GenerateArray(int size, int lower, int upper)
                {
                    int[] result = new int[size];
                    Span<int> span = new(result);
                    for (int i = 0; i < span.Length; i++)
                    {
                        span[i] = Rng.Next(lower, upper);
                    }
                    return result;
                }

                return GenerateArray(size, lower, upperExclusive);
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <returns></returns>
        [DebuggerHidden]
        public static int[] NextIntArray(int size, int lower = int.MinValue, int upperExclusive = int.MaxValue)
        {
            try
            {
                Limiter.Wait();

                static int[] GenerateArray(int size, int lower, int upper)
                {
                    int[] result = new int[size];
                    Span<int> span = new(result);
                    for (int i = 0; i < span.Length; i++)
                    {
                        span[i] = Rng.Next(lower, upper);
                    }
                    return result;
                }

                return GenerateArray(size, lower, upperExclusive);
            }
            finally
            {
                Limiter.Release();
            }
        }

        /// <summary>
        /// Returns a random double between <paramref name="lower"/> and <paramref name="upper"/>
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task<int> NextAsync(int lower = int.MinValue, int upper = int.MaxValue)
        {
            await Limiter.WaitAsync();
            try
            {
                return Rng.Next(lower, upper);
            }
            finally
            {
                Limiter.Release();
            }
        }


    }

    public static class Dictionary
    {
        /// <summary>
        /// Deconstructs the <see cref="IDictionary{TKey, TValue}"/> to a <see cref="Span{}"/> where each T is a <see cref="Tuple"/> (<typeparamref name="T"/> Key, <typeparamref name="U"/> Value)
        /// </summary>
        /// <code>
        /// O(4n)
        /// </code>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static Span<(T Key, U Value)> ToPairSpan<T, U>(ref IDictionary<T, U> dictionary)
        {
            int n = dictionary.Count;

            var (Keys, Values) = DeconstructPairs(ref dictionary);

            Span<T> keys = new(Keys);
            Span<U> values = new(Values);

            var result = new (T Key, U Value)[n];
            Span<(T Key, U Value)> resultSpan = new(result);

            for (int i = 0; i < n; i++)
            {
                resultSpan[i] = (keys[i], values[i]);
            }

            return resultSpan;
        }

        /// <summary>
        /// Deconstructs the <see cref="IDictionary{TKey, TValue}"/> to a <see cref="Tuple"/> (<typeparamref name="T"/> Key, <typeparamref name="U"/> Value) [ ]
        /// </summary>
        /// <code>
        /// O(4n)
        /// </code>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static (T Key, U Value)[] ToPairs<T, U>(ref IDictionary<T, U> dictionary)
        {
            int n = dictionary.Count;

            var (Keys, Values) = DeconstructPairs(ref dictionary);

            Span<T> keys = new(Keys);
            Span<U> values = new(Values);

            var result = new (T Key, U Value)[n];
            Span<(T Key, U Value)> resultSpan = new(result);

            for (int i = 0; i < n; i++)
            {
                resultSpan[i] = (keys[i], values[i]);
            }

            return result;
        }

        /// <summary>
        /// Copies the <see cref="IDictionary{TKey, TValue}.Keys"/> and <see cref="IDictionary{TKey, TValue}.Values"/> to new arrays and constructs a tuple with the pairs.
        /// <code>
        /// O(2n)
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static (T[] Keys, U[] Values) DeconstructPairs<T, U>(ref IDictionary<T, U> dictionary)
        {
            int n = dictionary.Count;

            T[] keyArr = new T[n];
            U[] valArr = new U[n];

            dictionary.Keys.CopyTo(keyArr, 0);
            dictionary.Values.CopyTo(valArr, 0);

            return (keyArr, valArr);
        }
    }

    /// <summary>
    /// Various helper methods for manipulating arrays
    /// </summary>
    public static class Array
    {
        /// <summary>
        /// Resizes the <paramref name="array"/> then sets the last element to the <paramref name="value"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T[] AppendValue<T>(T[] array, T value) => AppendValue(ref array, ref value);

        /// <summary>
        /// Resizes the <paramref name="array"/> then sets the last element to the <paramref name="value"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T[] AppendValue<T>(ref T[] array, ref T value)
        {
            int newIndex = array.Length;

            System.Array.Resize(ref array, newIndex + 1);

            array[newIndex] = value;

            return array;
        }

        /// <summary>
        /// Resizes the <paramref name="array"/> then sets the last element to the <paramref name="value"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T[] AppendValue<T>(ref T[] array, ref T value, bool preventDuplicates)
        {
            if (preventDuplicates && array.Contains(value))
            {
                return array;
            }
            return AppendValue(ref array, ref value);
        }

        /// <summary>
        /// Shifts the value to the right and truncates the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="valueToRemove"></param>
        /// <returns></returns>
        public static ref T[] RemoveValue<T>(ref T[] array, ref T valueToRemove) where T : IComparable<T>, IEquatable<T>
        {
            Span<T> arrSpan = array;

            int lastIndex = arrSpan.Length - 1;

            bool foundValue = false;

            if (arrSpan[lastIndex].Equals(valueToRemove))
            {
                foundValue = true;
            }
            else
            {
                for (int i = 0; i < lastIndex; i++)
                {
                    ref T key = ref arrSpan[i];

                    if (foundValue || key.Equals(valueToRemove))
                    {
                        foundValue = true;
                        // shift the value to the right
                        key = arrSpan[i + 1];
                    }
                }
            }

            if (foundValue)
            {
                // truncate the array
                System.Array.Resize(ref array, lastIndex);
            }

            return ref array;
        }

        /// <summary>
        /// Shifts the value to the right and truncates the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="valueToRemove"></param>
        /// <returns></returns>
        public static ref T[] RemoveIndex<T>(ref T[] array, int index)
        {
            Span<T> arrSpan = array;

            int lastIndex = arrSpan.Length - 1;

            for (int i = index; i < lastIndex; i++)
            {
                ref T key = ref arrSpan[i];

                key = arrSpan[i + 1];
            }

            // truncate the array
            System.Array.Resize(ref array, lastIndex);

            return ref array;
        }

        public static ref T[] RemoveValues<T>(ref T[] array, ref Span<T> Values) where T : IComparable<T>, IEquatable<T>
        {
            int length = array.Length;
            Span<T> arrSpan = new(array);

            // keep track of how many values we removed, just becuase they've included values into the Values span doesn't mean those
            // values are definately in the array
            int amountOfRemovedValues = 0;

            for (int i = 0; i < arrSpan.Length; i++)
            {
                if (Values.Contains(arrSpan[i]))
                {
                    // shift all values on the right to the left
                    for (int x = i; x < (arrSpan.Length - amountOfRemovedValues); x++)
                    {
                        arrSpan[x] = arrSpan[(x + 1) % length];
                    }

                    amountOfRemovedValues++;

                    // start this loop over so we can evaluate the next value(that we moved into this spot)
                    i--;
                    continue;
                }
            }

            // truncate the dead values if there are any
            System.Array.Resize(ref array, length - amountOfRemovedValues);

            return ref array;
        }

    }
}
