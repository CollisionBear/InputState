namespace CollisionBear.InputState
{
    public enum Button : int
    {
        Invalid         = -1,
        Action00        = 0,
        Action01        = 1,
        Action02        = 2,
        Action03        = 3,
        Action04        = 4,
        Action05        = 5,
        Action06        = 6,
        Action07        = 7,
        Action08        = 8,
        Action09        = 9,
        Action10        = 10,
        Action11        = 11,
        Action12        = 12,
        Action13        = 13,
        TriggerLeft     = 20,
        TriggerRight    = 21,
        BumberLeft      = 22,
        BumberRight     = 23,
        StickLeft       = 24,
        StickRight      = 25,
        Start           = 30,
        Select          = 31,
        Accept          = 40,
        Cancel          = 41,
    }

    public static class ButtonUtils
    {
        public static int ButtonCount = 64;
    }
}