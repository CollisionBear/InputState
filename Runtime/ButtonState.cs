namespace Fyrvall.Input
{
    public enum ButtonState : int
    {
        Up = 0,
        Down = 1,           // The button is in a pressed state
        Pressed = 2,        // The button was pressed this tick 
        Released = 3,       // The button was released this tick
    }
}