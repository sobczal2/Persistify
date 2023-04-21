namespace Persistify.Helpers;

public class MathI
{
    public static int Max(int a, int b)
    {
        return a > b ? a : b;
    }
    
    public static int Min(int a, int b)
    {
        return a < b ? a : b;
    }
    
    public static int Abs(int a)
    {
        return a < 0 ? -a : a;
    }
    
    public static int Clamp(int value, int min, int max)
    {
        return value < min ? min : value > max ? max : value;
    }
    
    public static int Ceiling(double a)
    {
        return (int) Math.Ceiling(a);
    }
    
    public static int Floor(double a)
    {
        return (int) Math.Floor(a);
    }
}