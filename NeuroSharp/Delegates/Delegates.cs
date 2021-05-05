using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp
{
    /// <summary>
    /// Represents an <see cref="Action{T}"/> that accepts and may mutate the parameters.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="arg1"></param>
    public delegate void ReferenceAction<U>(ref U arg1);

    /// <summary>
    /// Represents an <see cref="Action{T1,T2}"/> that accepts and may mutate the parameters.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="arg1"></param>
    public delegate void ReferenceAction<T1, T2>(ref T1 arg1, ref T2 arg2);

    /// <summary>
    /// Represents an <see cref="Action{T1, T2, T3}"/> that accepts and may mutate the parameters.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="arg1"></param>
    public delegate void ReferenceAction<T1, T2, T3>(ref T1 arg1, ref T2 arg2, ref T3 arg3);

    /// <summary>
    /// Represents an <see cref="Action{T1, T2, T3, T4}"/> that accepts and may mutate the parameters.
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="arg1"></param>
    public delegate void ReferenceAction<T1, T2, T3, T4>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4);

    /// <summary>
    /// Represents an <see cref="Func{T,U}"/> that accepts and mutates a by-reference <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate TResult ReferenceFunc<T, TResult>(ref T value);

    /// <summary>
    /// Represents an <see cref="Func{T1,T2,TResult}"/> that accepts and mutates a by-reference <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate TResult ReferenceFunc<T1, T2, TResult>(ref T1 arg1, ref T2 arg2);

    /// <summary>
    /// Represents an <see cref="Func{T1,T2,T3,TResult}"/> that accepts and mutates a by-reference <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate TResult ReferenceFunc<T1, T2, T3, TResult>(ref T1 arg1, ref T2 arg2, ref T3 arg3);

    /// <summary>
    /// Represents an <see cref="Func{T1,T2,T3,T4,TResult}"/> that accepts and mutates a by-reference <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate TResult ReferenceFunc<T1, T2, T3, T4, TResult>(ref T1 arg1, ref T2 arg2, ref T3 arg3, ref T4 arg4);
}
