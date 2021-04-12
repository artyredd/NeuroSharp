using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// Constraint that requires that T by multiplicable by <see cref="int"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMultiplicable
    {
        public static int operator *(IMultiplicable lvalue, IMultiplicable rvalue)
        {
            return lvalue * rvalue;
        }
    }
}
