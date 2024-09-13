using UnityEngine;

namespace CollisionBear.InputState
{
    public interface IInputDevice
    {
        string Name { get; }
        InputDeviceInstance CreateInstance(IInputHandler inputHandler, IIconSetProvider iconSetProvider, InputManager inputManager);
        bool Setup();
        InputState UpdateInputState(InputDeviceInstance instance);
        InputState GetInputState();
        void LateUpdate(InputDeviceInstance instance);
        InputDeviceType GetDeviceType();
        void SetColor(InputDeviceInstance instance, Color color);
    }
}