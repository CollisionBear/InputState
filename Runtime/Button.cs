namespace CollisionBear.InputState
{
    public enum Button : int
    {
        Invalid = -1,
        Action00 = 0,
        Action01 = 1,
        Action02 = 2,
        Action03 = 3,
        Action04 = 4,
        Action05 = 5,
        Action06 = 6,
        Action07 = 7,
        TriggerLeft = 10,
        TriggerRight = 11,
        BumberLeft = 12,
        BumberRight = 13,
        StickLeft = 14,
        StickRight = 15,
        Start = 20,
        Select = 21,
        Accept = 30,
        Cancel = 31,
    }

    public static class ButtonUtils
    {
        public static int ButtonCount = 32;
    }
}