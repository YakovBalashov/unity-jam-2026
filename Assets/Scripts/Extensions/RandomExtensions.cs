using System;

namespace RacingGame.Extensions
{
    public static class RandomExtensions
    {
        public static float Range(this Random random, float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
        
        public static int Range(this Random random, int min, int max)
        {
            return random.Next(min, max);
        }
        
        public static float RandomizeValue(this Random random, float baseValue, float offset)
        {
            return random.Range(baseValue - offset, baseValue + offset);
        }
    }
}
