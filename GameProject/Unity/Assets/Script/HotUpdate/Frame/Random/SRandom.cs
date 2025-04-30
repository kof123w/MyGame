using System;

namespace MyGame
{
    internal class SRandom
    {
        private Random random;

        internal SRandom(int seed)
        {
            random = new Random(seed); 
        }

        internal int Next(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}