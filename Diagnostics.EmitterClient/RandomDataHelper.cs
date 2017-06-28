using Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagnostics.EmitterClient
{
   public static class RandomDataHelper
    {
        private static Random _random = new Random((int)DateTime.Now.Ticks);
        private static Random _randomNum = new Random();
        public static char GetLetter()
        {
            return Convert.ToChar(Convert.ToInt32(Math.Floor(26 * _random.NextDouble() + 65)));
        }
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = GetLetter();
                builder.Append(ch);
            }

            return builder.ToString();
        }
        public static SeverityEnum RandomSeverity()
        {
            int num = _randomNum.Next(0, 3);
            return (SeverityEnum)num;
        }

    }
}
