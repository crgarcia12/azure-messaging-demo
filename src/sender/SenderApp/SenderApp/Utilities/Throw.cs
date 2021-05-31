using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SenderApp
{
    public static class Throw
    {
        public static void IsNullOrWhiteSpace(string paramName, string paramValue)
        {
            if(String.IsNullOrWhiteSpace(paramValue))
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
