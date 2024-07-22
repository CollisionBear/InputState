using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace CollisionBear.InputState
{
    public class InputSystemDevice : IInputDevice
    {
        public const float TriggerThreshold = 0.8f;
        public const float DirectionButtonThreshold = 0.6f;

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

        public Gamepad GamePad;
        public string Name => GamePad.name;

        private InputDeviceType DeviceType;

        private GamepadButton[] GamepadButtons;
        private GamepadButton[] GamepadTriggers;

        private InputState InputState;
        private ButtonState[] ButtonStates;
        private GamepadButton[] MappedButtons;

        public InputSystemDevice(Gamepad gamePad, InputDeviceConfiguration configuration)
        {
            GamePad = gamePad;
            DeviceType = ReadDeviceType();

            GamepadButtons = GetGamepadButtons();
            GamepadTriggers = GetGamepadTriggers();
            MappedButtons = configuration.GetButtonMapping();
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
            SetupButtonStates(InputState);

            return true;
        }

        private List<GamepadButton> AllButtons()
        {
            var result = new List<GamepadButton>();

            result.AddRange(GamepadButtons);
            result.AddRange(GamepadTriggers);

            return result;
        }

        private void SetupButtonStates(InputState inputState)
        {
            ButtonStates = new ButtonState[GamepadButtons.Length + GamepadTriggers.Length];
            for (int i = 0; i < GamepadButtons.Length; i ++) {
                ButtonStates[i] = new ButtonState();
            }

            for (int i = 0; i < GamepadTriggers.Length; i++) {
                ButtonStates[i + GamepadButtons.Length] = new ButtonState();
            }
            var allbuttons = AllButtons();

            foreach (var button in ButtonUtils.ButtonIndices) {
                var index = allbuttons.IndexOf(MappedButtons[(int)button]);

                 inputState.ButtonStates[(int)button] = ButtonStates[index];
            }
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
            for (int i = 0; i < GamepadButtons.Length; i++) {
                ButtonStates[i].SetState(GamePad[GamepadButtons[i]]);
            }

            for (int i = 0; i < GamepadTriggers.Length; i++) {
                ButtonStates[i + GamepadButtons.Length].SetStateFromTrigger(GamePad[GamepadTriggers[i]], TriggerThreshold);
            }
        }

        protected void ReadDirectionButtonStates(ButtonState[] buttonStates)
        {
            var readLeftStick = GamePad.leftStick.ReadValue();

            buttonStates[(int)DirectionButton.Up].SetDirectionState(GamePad.dpad.up, readLeftStick.y, DirectionButtonThreshold);
            buttonStates[(int)DirectionButton.Down].SetDirectionState(GamePad.dpad.down, -readLeftStick.y, DirectionButtonThreshold);
            buttonStates[(int)DirectionButton.Left].SetDirectionState(GamePad.dpad.left, -readLeftStick.x, DirectionButtonThreshold);
            buttonStates[(int)DirectionButton.Right].SetDirectionState(GamePad.dpad.right, readLeftStick.x, DirectionButtonThreshold);
        }
    }
}