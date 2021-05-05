using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Logic
{
    public static class Validators
    {

        /// <summary>
        /// Determines if <paramref name="value"/> is within two threasholds, if it is invokes the provided methods
        /// <para>
        /// 0 -----> Lower Threashold -----> Upper Threashold -> Neither
        /// </para>
        /// Example:
        /// <code>
        /// void Print(int Val) => Console.WritLine(Val);
        /// </code>
        /// <code>
        /// void PrintTwo(int Val) => Console.WritLine("Two({0})",Val);
        /// </code>
        /// <code>
        /// void PrintThree(int Val) => Console.WritLine($"Three({Val})");
        /// </code>
        /// <code>
        /// int X = WithinTwoLayeredThreashold(12,5,20,Print,PrintTwo,PrintThree);
        /// </code>
        /// <code>
        /// int Y = WithinTwoLayeredThreashold(3,5,20,Print,PrintTwo,PrintThree);
        /// </code>
        /// <code>
        /// int Z = WithinTwoLayeredThreashold(25,5,20,Print,PrintTwo,PrintThree);
        /// </code>
        /// <code>
        /// Console.WriteLine($"X({X}) Y({Y}) Z({Z})");
        /// </code>
        /// <para>
        /// Outputs:
        /// <code>
        /// 12
        /// </code>
        /// <code>
        /// 3
        /// </code>
        /// <code>
        /// 25
        /// </code>
        /// <code>
        /// X(1) Y(2) Z(0)
        /// </code>
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="LowerThreashold"></param>
        /// <param name="UpperThreashold"></param>
        /// <param name="WhenLowerThreashold"></param>
        /// <param name="WhenUpperThreashold"></param>
        /// <param name="WhenNeither"></param>
        /// <returns>
        /// <see langword="int"/> 0: Greater than <paramref name="UpperThreashold"/>
        /// <para>
        /// <see langword="int"/> 1: Less than <paramref name="UpperThreashold"/> but greater than <paramref name="LowerThreashold"/>
        /// </para>
        /// <para>
        /// <see langword="int"/> 1: Less than <paramref name="LowerThreashold"/>
        /// </para>
        /// </returns>
        public static int WithinTwoLayeredThreashold<T>(ref T value, ref T LowerThreashold, ref T UpperThreashold, ReferenceAction<T> WhenLowerThreashold, ReferenceAction<T> WhenUpperThreashold, ReferenceAction<T> WhenNeither) where T : IComparable<T>
        {
            // this may seem confusing but all this is checking is to see if a particular value is within two ranges across two checks
            // this is just abstracted with delagate to prevent unessesecary copying of value types(the integers)
            // in this example this would be if we were given the range upper = 5 lower = 3
            // |  OnLower   |  OnUpper  |      OnNeither     |
            // | 0  1   2   |  3    4   |  6 9 5 8 7 n...    |
            if (value.CompareTo(UpperThreashold) < 0)
            {

                if (value.CompareTo(LowerThreashold) < 0)
                {
                    WhenLowerThreashold(ref value);
                    return 2;
                }

                WhenUpperThreashold(ref value);
                return 1;
            }

            WhenNeither(ref value);

            return 0;
        }
    }
}
