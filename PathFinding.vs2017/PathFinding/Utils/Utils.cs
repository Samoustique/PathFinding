using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding.Utils
{
    public class Utils
    {
        public static char[] Map2DToMap1D(char[,] map)
        {
            return (from char x in map select x).ToArray();
        }
    }
}
