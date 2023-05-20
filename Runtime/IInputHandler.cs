namespace Fyrvall.Input
{
    public interface IInputHandler
    {
        void TakeInput(InputState inputState, InputDeviceInstance deviceInstance);
    }
}