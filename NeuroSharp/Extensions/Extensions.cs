using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using NeuroSharp.Helpers;

namespace NeuroSharp.Extensions
{
    /// <summary>
    /// Contains helper extensions to assist in handling matricies
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static Task AppendToValueAsync<T>(this IDictionary<T, T[]> dict, T key, T value) => AppendToValueAsync(dict, key, value, null);

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task AppendToValueAsync<T, U>(this IDictionary<T, U[]> dict, T key, U value, SemaphoreSlim Limiter)
        {
            if (Limiter is not null)
            {
                await Limiter.WaitAsync();
            }
            try
            {
                if (dict.ContainsKey(key))
                {
                    var arr = dict[key];

                    // i could probably remove this check since it doesnt make sense for a node to have more than one connection to the same node
                    if (arr.Contains(value) is false)
                    {
                        dict[key] = Helpers.Array.AppendValue(arr, value);
                    }
                }
                else
                {
                    dict.Add(key, new U[] { value });
                }
            }
            finally
            {
                Limiter?.Release();
            }
        }

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns>
        /// <see langword="true"/> when the key was found.
        /// <para>
        /// </para>
        /// <see langword="false"/> when the key was not found, but added.
        /// </returns>
        [DebuggerHidden]
        public static bool AppendToValue<T, U>(this IDictionary<T, U[]> dict, ref T key, ref U value, bool preventDuplicateValues = false)
        {
            if (dict.ContainsKey(key))
            {
                var arr = dict[key];

                // avoid costly .Contains by using a flag if we dont care about duplicates
                if (preventDuplicateValues && arr.Contains(value))
                {
                    return true;
                }

                dict[key] = Helpers.Array.AppendValue(arr, value);

                return true;
            }
            else
            {
                dict.Add(key, new U[] { value });
                return false;
            }
        }

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns>
        /// <see langword="true"/> when the key was found.
        /// <para>
        /// </para>
        /// <see langword="false"/> when the key was not found, but added.
        /// </returns>
        [DebuggerHidden]
        public static bool AppendToValue<T, U>(this IDictionary<T, U[]> dict, T key, ref U value, bool preventDuplicateValues = false) => AppendToValue(dict, ref key, ref value, preventDuplicateValues);

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns>
        /// <see langword="true"/> when the key was found.
        /// <para>
        /// </para>
        /// <see langword="false"/> when the key was not found, but added.
        /// </returns>
        [DebuggerHidden]
        public static bool AppendToValue<T, U>(this IDictionary<T, U[]> dict, ref T key, U value, bool preventDuplicateValues = false) => AppendToValue(dict, ref key, ref value, preventDuplicateValues);

        /// <summary>
        /// Searches the dictionary for the <paramref name="key"/>, if it does not exists it creates a new <see cref="{T}[]"/> with the <paramref name="value"/> as the first entry. If the <paramref name="key"/> is found, the array is resized and the <paramref name="value"/> is added as the last element in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="Limiter"></param>
        /// <returns>
        /// <see langword="true"/> when the key was found.
        /// <para>
        /// </para>
        /// <see langword="false"/> when the key was not found, but added.
        /// </returns>
        [DebuggerHidden]
        public static bool AppendToValue<T, U>(this IDictionary<T, U[]> dict, T key, U value, bool preventDuplicateValues = false) => AppendToValue(dict, ref key, ref value, preventDuplicateValues);
    }

    public static class SpanExtensions
    {
        /// <summary>
        /// Converts two spans into a single <see cref="Span{T}"/> of the two
        /// <para>
        /// Example:
        /// <code>
        /// var left = new Span&lt;int&gt;(someIntArray);
        /// </code>
        /// <code>
        /// var right = new Span&lt;float&gt;(someFloatArray);
        /// </code>
        /// <code>
        /// (Int Left, float Right) = left.ToTuple(ref right);
        /// </code>
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="Left"></param>
        /// <param name="Right"></param>
        /// <returns></returns>
        public static Span<(T Left, U Right)> ToTuple<T, U>(this ref Span<T> Left, ref Span<U> Right)
        {
            // intentional no length comparison to avoid index out of bounds exceptions,
            // it is assuming if you're using low level(ish) memory manipulation like spans you know better than to 
            // use two different lengthed spans, if you don't the error is not hidden and it will still be an obvious fix
            int N = Left.Length;

            Span<(T Left, U Right)> tupleSpan = new (T Left, U Right)[N];

            for (int i = 0; i < N; i++)
            {
                tupleSpan[i] = (Left[i], Right[i]);
            }

            return tupleSpan;
        }
    }
}
