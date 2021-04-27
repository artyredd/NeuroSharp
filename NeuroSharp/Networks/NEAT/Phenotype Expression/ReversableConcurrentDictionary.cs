using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeuroSharp.NEAT
{
    public class ReversableConcurrentDictionary<KeyType, ValueType>
    {
        private readonly SemaphoreSlim Lock = new(1, 1);

        internal readonly Dictionary<KeyType, ValueType> Dict = new Dictionary<KeyType, ValueType>();

        public async Task<KeyValuePair<KeyType, ValueType>[]> ToPairsAsync()
        {
            await Lock.WaitAsync();
            try
            {
                var pairs = new KeyValuePair<KeyType, ValueType>[Dict.Count];

                ((IDictionary<KeyType, ValueType>)Dict).CopyTo(pairs, 0);

                return pairs;
            }
            finally
            {
                Lock.Release();
            }
        }

        public async Task<ICollection<ValueType>> ValuesAsync()
        {
            await Lock.WaitAsync();
            try
            {
                return Dict.Values;
            }
            finally
            {
                Lock.Release();
            }
        }

        public async Task<IDictionary<KeyType, ValueType>> ToDictionary()
        {
            await Lock.WaitAsync();
            try
            {
                return new Dictionary<KeyType, ValueType>(Dict);
            }
            finally
            {
                Lock.Release();
            }
        }

        public async Task<IDictionary<ValueType, KeyType[]>> ReverseAsync()
        {
            var pairs = await ToPairsAsync();
            return Reverse(pairs);
        }

        private IDictionary<ValueType, KeyType[]> Reverse(KeyValuePair<KeyType, ValueType>[] pairs)
        {
            IDictionary<ValueType, KeyType[]> result = new Dictionary<ValueType, KeyType[]>();

            void AddOrUpdate(KeyValuePair<KeyType, ValueType> pair)
            {
                if (result.ContainsKey(pair.Value))
                {
                    result[pair.Value] = Extensions.Array.ResizeAndAdd(result[pair.Value], pair.Key);
                }
                else
                {
                    result.Add(pair.Value, new KeyType[] { pair.Key });
                }
            }

            Span<KeyValuePair<KeyType, ValueType>> pairSpan = new(pairs);
            for (int i = 0; i < pairSpan.Length; i++)
            {
                AddOrUpdate(pairSpan[i]);
            }

            return result;
        }

        public async Task Add(KeyType Key, ValueType Value)
        {
            await Lock.WaitAsync();
            try
            {
                // this is supposed to not have any checks for performance reasons
                // expect an error for missing key if you do not do normal checks
                if (Dict.ContainsKey(Key) is false)
                {
                    Dict.Add(Key, Value);
                }
                else
                {
                    Dict[Key] = Value;
                }
            }
            finally
            {
                Lock.Release();
            }
        }

        public async Task Clear()
        {
            await Lock.WaitAsync();
            try
            {
                Dict.Clear();
            }
            finally
            {
                Lock.Release();
            }
        }

        public async Task<ValueType> Get(KeyType Key)
        {
            await Lock.WaitAsync();
            try
            {
                // intentional no checks
                return Dict[Key];
            }
            finally
            {
                Lock.Release();
            }
        }

        public async Task<bool> ContainsKey(KeyType Key)
        {
            await Lock.WaitAsync();
            try
            {
                return Dict.ContainsKey(Key);
            }
            finally
            {
                Lock.Release();
            }
        }
    }
}
