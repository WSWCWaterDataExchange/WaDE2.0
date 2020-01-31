namespace WesternStatesWater.WaDE.Accessors.Tests
{
    internal class Utility
    {
        public static int NthTriangle(int start)
        {
            return start == 0 ? 0 : start + NthTriangle(start - 1);
        }
    }
}