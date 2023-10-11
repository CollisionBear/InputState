using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace CollisionBear.InputState
{
    public class InputSystemDevice : IInputDevice
    {
        public const float TriggerTreshold = 0.8f;
        public const float DirectionButtonTreshold = 0.6f;

        public Gamepad GamePad;
        public string Name => GamePad.name;

        protected InputState InputState;

        public InputSystemDevice(Gamepad gamePad)
        {
            GamePad = gamePad;
        }

        public InputDeviceType GetDeviceType()
        {
            if (GamePad is UnityEngine.InputSystem.DualShock.DualShockGamepad) {
                return InputDeviceType.Playstation;
            } else if (GamePad is UnityEngine.InputSystem.XInput.XInputController) {
                return InputDeviceType.Xbox;
            }

            // Fallback
            return InputDeviceType.Xbox;
        }

        public void SetColor(InputDeviceInstance instance, Color color)
        {
            if (GamePad is UnityEngine.InputSystem.DualShock.DualShockGamepad) {
                var dualShockController = GamePad as UnityEngine.InputSystem.DualShock.DualShockGamepad;
                dualShockController.SetLightBarColor(color);
            }
        }

        public InputDeviceInstance CreateInstance(IInputHandler inputHandler, IIconSetProvider iconSetProvider, InputManager inputManager)
        {
            var gameObject = new GameObject($"InputSystemDeviceInstance-{GamePad.name}");
            GameObject.DontDestroyOnLoad(gameObject);
            var result = gameObject.AddComponent<InputDeviceInstance>();
            result.SetDevice(this, inputHandler, iconSetProvider, inputManager);

            return result;
        }

        public bool Setup()
        {
            // Create a reusable InputState object
            InputState = new InputState(InputType.GamePad);

            return true;
        }

        public InputState UpdateInputState(InputDeviceInstance instance)
        {
            InputState.LeftStick = GamePad.leftStick.ReadValue();
            InputState.RightStick = GamePad.rightStick.ReadValue();

            ReadButtonStates(InputState.ButtonStates);
            ReadDirectionButtonStates(InputState.DirectionButtonStates);
            return InputState;
        }

        public InputState GetInputState()
        {
            return InputState;
        }

        protected void ReadButtonStates(ButtonState[] buttonStates)
        {
            buttonStates[(int)Button.TriggerLeft].SetStateFromTrigger(GamePad.leftTrigger, TriggerTreshold);
            buttonStates[(int)Button.TriggerRight].SetStateFromTrigger(GamePad.rightTrigger, TriggerTreshold);

            buttonStates[(int)Button.BumberLeft].SetState(GamePad.leftShoulder);
            buttonStates[(int)Button.BumberRight].SetState(GamePad.rightShoulder);

            buttonStates[(int)Button.StickLeft] = SetState(GamePad.leftStickButton);
            buttonStates[(int)Button.StickRight] = SetState(GamePad.rightStickButton);

            buttonStates[(int)Button.Start].SetState(GamePad.startButton);
            buttonStates[(int)Button.Select].SetState(GamePad.selectButton);
            buttonStates[(int)Button.Action00].SetState(GamePad.aButton);
            buttonStates[(int)Button.Action01].SetState(GamePad.bButton);
            buttonStates[(int)Button.Action02].SetState(GamePad.xButton);
            buttonStates[(int)Button.Action03].SetState(GamePad.yButton);
            buttonStates[(int)Button.Action04].SetState(GamePad.dpad.up);
            buttonStates[(int)Button.Action05].SetState(GamePad.dpad.down);
            buttonStates[(int)Button.Action06].SetState(GamePad.dpad.left);
            buttonStates[(int)Button.Action07].SetState(GamePad.dpad.right);
            buttonStates[(int)Button.Accept].SetState(GamePad.aButton);
            buttonStates[(int)Button.Cancel].SetState(GamePad.bButton); 
        }

        protected void ReadDirectionButtonStates(ButtonState[] buttonStates)
        {
            var readLeftStick = GamePad.leftStick.ReadValue();

            buttonStates[(int)DirectionButton.Up].SetState(GamePad.dpad.up, readLeftStick.y, DirectionButtonTreshold);
            buttonStates[(int)DirectionButton.Down].SetState(GamePad.dpad.down, -readLeftStick.y, DirectionButtonTreshold);
            buttonStates[(int)DirectionButton.Left].SetState(GamePad.dpad.left, -readLeftStick.x, DirectionButtonTreshold);
            buttonStates[(int)DirectionButton.Right].SetState(GamePad.dpad.right, readLeftStick.x, DirectionButtonTreshold);
        }
    }
}