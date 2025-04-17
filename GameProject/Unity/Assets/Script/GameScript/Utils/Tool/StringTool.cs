using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EventHash
{
    public static class EventHashGenerator
    {
        private static Dictionary<string, long> dictHash = new Dictionary<string, long>();
        private static long curGenHash = 0;
        public static long StringToHash(this string input)
        {
            long hash;
            if (dictHash.TryGetValue(input,out hash))
            {
                return hash;
            }

            curGenHash++;
            dictHash.Add(input,curGenHash);
            return curGenHash;
        }

        public static long GenHash()
        {
            curGenHash++;
            return curGenHash;
        }
    }
}