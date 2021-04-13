using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// Defines an object that is compatible with matricies such as .
    /// <list type="">
    /// <listheader>Compatible Types:</listheader>
    /// <item><see cref="sbyte"/></item>
    /// <item><see cref="byte"/></item>
    /// <item><see cref="short"/></item>
    /// <item><see cref="int"/></item>
    /// <item><see cref="uint"/></item>
    /// <item><see cref="long"/></item>
    /// <item><see cref="ulong"/></item>
    /// <item><see cref="char"/></item>
    /// <item><see cref="float"/></item>
    /// <item><see cref="double"/></item>
    /// <item><see cref="decimal"/></item>
    /// </list>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMatrixCompatible<T> where T : unmanaged, IComparable<T>, IEquatable<T>
    {
    }
}
