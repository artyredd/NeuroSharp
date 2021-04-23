using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroSharp.Bench
{
    public class GotoVsWhile
    {
        [Benchmark]
        public int Control()
        {
            int[] s = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int sum = 0;
            int i = 10;
            while (true)
            {
                i--;
                if (i <= 0)
                {
                    break;
                }
                sum += s[i];
            }
            return sum;
        }
        [Benchmark]
        public int Test()
        {
            int[] s = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            int sum = 0;
            int i = 10;
        Loop:
            i--;
            if (i <= 0)
            {
                goto Finish;
            }
            sum += s[i];
            goto Loop;

        Finish:
            return sum;
        }
    }
}
