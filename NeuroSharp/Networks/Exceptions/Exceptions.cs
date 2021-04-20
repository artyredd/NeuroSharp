using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Networks
{
    public static class Exceptions
    {
        /// <summary>
        /// Returns exception:
        /// <code>
        /// Unexpected training data scheme provided. Expected ([12]) items but was only provided ([9]) items. Privided training data must match the number of input and/or output nodes.
        /// </code>
        /// </summary>
        /// <param name="Expected"></param>
        /// <param name="Actual"></param>
        /// <returns></returns>
        public static Exception InconsistentTrainingDataScheme(int Expected, int Actual)
        {
            return new ArgumentException($"Unexpected training data scheme provided. Expected ({Expected}) items but was only provided ({Actual}) items. Privided training data must match the number of input and/or output nodes.");
        }
    }
}
