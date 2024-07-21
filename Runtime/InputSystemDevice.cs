using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace CollisionBear.InputState
{
    public class InputSystemDevice : IInputDevice
    {
        public const float TriggerTreshold = 0.8f;
        public const float DirectionButtonTreshold = 0.6f;

        private static GamepadButton[] GetGamepadButtons() => new GamepadButton[] {
            GamepadButton.DpadUp,
            GamepadButton.DpadDown,
            GamepadButton.DpadLeft,
            GamepadButton.DpadRight,
            GamepadButton.North,
            GamepadButton.East,
            GamepadButton.South,
            GamepadButton.West,
            GamepadButton.LeftStick,
            GamepadButton.RightStick,
            GamepadButton.LeftShoulder,
            GamepadButton.RightShoulder,
            GamepadButton.Start,
            GamepadButton.Select
        };

        private static GamepadButton[] GetGamepadTriggers() => new GamepadButton[] {
            GamepadButton.LeftTrigger,
            GamepadButton.RightTrigger
        };

        private static ButtonState[] GetInternalButtonState(GamepadButton[] buttonMappings, InputState inputState)
        {
            var result = new ButtonState[64];

            foreach(var button in ButtonUtils.ActionButtonList) {
                result[(int)buttonMappings[(int)button]] = inputState.ButtonStates[(int)button];
            }
           
            return result;
        }

        public Gamepad GamePad;
        public string Name => GamePad.name;

        private InputState InputState;
        private InputDeviceType DeviceType;

        private GamepadButton[] GamepadButtons;
        private GamepadButton[] GamepadTriggers;
        private ButtonState[] InternalButtonState;

        private GamepadButton[] InternalMappingTable;

        public InputSystemDevice(Gamepad gamePad, InputDeviceConfiguration configuration)
        {
            GamePad = gamePad;
            DeviceType = ReadDeviceType();

            GamepadButtons = GetGamepadButtons();
            GamepadTriggers = GetGamepadTriggers();

            InternalMappingTable = configuration.GetButtonMapping();
        }

        private InputDeviceType ReadDeviceType()
        {
            if (GamePad is UnityEngine.InputSystem.DualShock.DualShockGamepad) {
                return InputDeviceType.Playstation;
            } else if (GamePad is UnityEngine.InputSystem.XInput.XInputController) {
                return InputDeviceType.Xbox;
            }

            // Fall back
            return InputDeviceType.Xbox;
        }

        public InputDeviceType GetDeviceType() => DeviceType;

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
            InternalButtonState = GetInternalButtonState(InternalMappingTable, InputState);

            return true;
        }

        public InputState UpdateInputState(InputDeviceInstance instance)
        {
            InputState.LeftStick = GamePad.leftStick.ReadValue();
            InputState.RightStick = GamePad.rightStick.ReadValue();

            ReadButtonStates();
            ReadDirectionButtonStates(InputState.DirectionButtonStates);

            return InputState;
        }

        public InputState GetInputState() => InputState;

        private void ReadButtonStates()
        {
            foreach (var button in GamepadButtons) {
                InternalButtonState[(int)button].SetState(GamePad[button]);
            }

            foreach (var button in GamepadTriggers) {
                InternalButtonState[(int)button].SetStateFromTrigger(GamePad[button], 0.75f);
            }
        }

        protected void ReadDirectionButtonStates(ButtonState[] buttonStates)
        {
            var readLeftStick = GamePad.leftStick.ReadValue();

            buttonStates[(int)DirectionButton.Up].SetDirectionState(GamePad.dpad.up, readLeftStick.y, DirectionButtonTreshold);
            buttonStates[(int)DirectionButton.Down].SetDirectionState(GamePad.dpad.down, -readLeftStick.y, DirectionButtonTreshold);
            buttonStates[(int)DirectionButton.Left].SetDirectionState(GamePad.dpad.left, -readLeftStick.x, DirectionButtonTreshold);
            buttonStates[(int)DirectionButton.Right].SetDirectionState(GamePad.dpad.right, readLeftStick.x, DirectionButtonTreshold);
        }
    }
}