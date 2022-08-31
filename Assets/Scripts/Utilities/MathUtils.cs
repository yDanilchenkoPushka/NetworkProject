namespace Utilities
{
    public static class MathUtils
    {
        public static int IndexFromMask(int mask)
        {
            for (int i = 0; i < 32; ++i)
            {
                if ((1 << i) == mask)
                    return i;
            }
            
            return -1;
        }
    }
}