namespace CollisionBear.InputState
{
    public class ButtonState
    {
        public KeyState State;
        public KeyState PreviousState;
        public float Time;
    }

    public enum KeyState : byte
    {
        Up = 0,
        Down = 1,           // The button is in a pressed state
        Pressed = 2,        // The button was pressed this tick 
        Released = 3,       // The button was released this tick
    }
}