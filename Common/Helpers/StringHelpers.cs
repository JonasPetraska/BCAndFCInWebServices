using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class StringHelpers
    {
        public static string GenerateCharacters(int numberOfCharacters, string character = " ")
        {
            var res = "";

            for (var i = 0; i < numberOfCharacters; i++)
                res += character;

            return res;
        }
    }
}
