namespace Fyrvall.Input
{
    public static class InputHandlerExtensions
    {
        public static void SetAsGlobalInputHandler(this IInputHandler inputHandler)
        {
            InputManager.Instance.DefaultInputHandler = inputHandler;
            foreach (var inputDevice in InputManager.Instance.InputDeviceInstances) {
                inputDevice.CurrentInputHandler = inputHandler;
            }
        }
    }
}