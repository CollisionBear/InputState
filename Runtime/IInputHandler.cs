namespace CollisionBear.InputState
{
    public interface IInputHandler
    {
        void TakeInput(InputState inputState, InputDeviceInstance deviceInstance);
    }
}