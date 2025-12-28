namespace BlazeEngine;

public static class Random
{
    public static System.Random random;

    public static void Init()
    {
        random = new System.Random();
    }
    
    public static int NextInt()
    {
        return random.Next();
    }
    /// <param name="min">Inclusive</param>
    /// <param name="max">Exclusive</param>
    /// <returns>Random value between min and max</returns>
    public static int NextInt(int min, int max)
    {
        return random.Next(min, max);
    }
    public static float NextFloat()
    {
        return random.NextSingle();
    }
    public static float NextFloat(float min, float max)
    {
        return NextFloat() * (max - min) + min;
    }
    public static long NextLong()
    {
        return random.NextInt64();
    }
    public static double NextDouble()
    {
        return random.NextDouble();
    }
}