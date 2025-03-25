using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MyGame
{
    public static class EventHashGenerator
    {
        private static Dictionary<string, long> m_dictHash = new Dictionary<string, long>();
        private static long m_curGenHash = 0;
        public static long StringToHash(this string input)
        {
            long hash;
            if (m_dictHash.TryGetValue(input,out hash))
            {
                return hash;
            }

            m_curGenHash++;
            m_dictHash.Add(input,m_curGenHash);
            return m_curGenHash;
        }

        public static long GenHash()
        {
            m_curGenHash++;
            return m_curGenHash;
        }
    }
}