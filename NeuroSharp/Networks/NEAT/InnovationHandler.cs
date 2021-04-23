using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("NeuroSharp.Tests")]
namespace NeuroSharp.NEAT
{
    public class InnovationHandler : IInnovationHandler
    {
        /// <summary>
        /// The index of the next innovation
        /// </summary>
        internal volatile int CurrentInnovationIndex = 0;

        internal readonly SemaphoreSlim InnovationSemaphore = new(1, 1);

        /// <summary>
        /// The actual storage location for innovations
        /// </summary>
        public IInnovation[] Innovations
        {
            get => _Innovations;
            private set
            {
                _Innovations = value;
            }
        }

        internal IInnovation[] _Innovations = Array.Empty<IInnovation>();

        /// <summary>
        /// a O(n) lookup for the index of an innovation by its hash
        /// </summary>
        public Dictionary<string, int> InnovationHashes { get; private set; } = new();

        /// <summary>
        /// Adds the given <see cref="IInnovation"/> to the global innovations dictionary.
        /// </summary>
        /// <param name="innovation"></param>
        /// <returns>
        /// <see langword="true"/> when the <see cref="IInnovation"/> was successfully added to the global dict.
        /// <para>
        /// <see langword="false"/> when the innovation already exists in the dictionary.
        /// </para>
        /// </returns>
        public async Task<int> Add(IInnovation innovation)
        {
            // get the hash of the inn so we can see if it's already in the global list
            string hash = innovation.Hash();

            await InnovationSemaphore.WaitAsync();

            try
            {
                // check to see if the hash exists in the dict
                if (InnovationHashes.ContainsKey(hash) is false)
                {
                    int innovationNumber = System.Threading.Interlocked.Increment(ref CurrentInnovationIndex);

                    Array.Resize(ref _Innovations, innovationNumber);

                    // add the actual innovation to the global list
                    Innovations[innovationNumber - 1] = innovation;

                    // create a O(1) lookup for the index of that innovation
                    InnovationHashes.Add(hash, innovationNumber);

                    return innovationNumber;
                }
                else
                {
                    // since the hash is in the dict return the index of it's innovation number
                    return InnovationHashes[hash];
                }
            }
            finally
            {
                InnovationSemaphore.Release();
            }
        }

        public int Count() => CurrentInnovationIndex;

        /// <summary>
        /// Clears the global innovation array and hashes
        /// </summary>
        /// <returns>
        /// <see langword="int"/> number of items cleared
        /// </returns>
        public async Task<int> Clear()
        {
            await InnovationSemaphore.WaitAsync();
            try
            {
                Innovations = Array.Empty<IInnovation>();

                CurrentInnovationIndex = 0;

                InnovationHashes.Clear();

                return CurrentInnovationIndex;
            }
            finally
            {
                InnovationSemaphore.Release();
            }
        }
    }
}
