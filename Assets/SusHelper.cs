public static class SusHelper
{
    public static int PosiMod(int x, int m)
    {
        return (x % m + m) % m;
    }
}