namespace IdleActionFarm.Runtime.UI
{
    public static class MathUtility
    {
        public static void AddIntToLong(this ref uint kek, int value)
        {
            if (value > 0)
            {
                kek += (uint)value;
            }
            else
            {
                kek -= (uint)value;
            }
        }
    }
}