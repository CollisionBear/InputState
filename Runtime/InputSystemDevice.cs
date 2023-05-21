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
            buttonStates[(int)Button.TriggerLeft].State = ReadTriggerValue(buttonStates[(int)Button.TriggerLeft], GamePad.leftTrigger);
            buttonStates[(int)Button.TriggerRight].State = ReadTriggerValue(buttonStates[(int)Button.TriggerRight], GamePad.rightTrigger);

            buttonStates[(int)Button.BumberLeft].State = GetKeyStateForKey(GamePad.leftShoulder);
            buttonStates[(int)Button.BumberRight].State = GetKeyStateForKey(GamePad.rightShoulder);
            buttonStates[(int)Button.Start].State = GetKeyStateForKey(GamePad.startButton);
            buttonStates[(int)Button.Select].State = GetKeyStateForKey(GamePad.selectButton);
            buttonStates[(int)Button.Action00].State = GetKeyStateForKey(GamePad.aButton);
            buttonStates[(int)Button.Action01].State = GetKeyStateForKey(GamePad.bButton);
            buttonStates[(int)Button.Action02].State = GetKeyStateForKey(GamePad.xButton);
            buttonStates[(int)Button.Action03].State = GetKeyStateForKey(GamePad.yButton);
            buttonStates[(int)Button.Action04].State = GetKeyStateForKey(GamePad.dpad.up);
            buttonStates[(int)Button.Action05].State = GetKeyStateForKey(GamePad.dpad.down);
            buttonStates[(int)Button.Action06].State = GetKeyStateForKey(GamePad.dpad.left);
            buttonStates[(int)Button.Action07].State = GetKeyStateForKey(GamePad.dpad.right);
            buttonStates[(int)Button.Accept].State = GetKeyStateForKey(GamePad.aButton);
            buttonStates[(int)Button.Cancel].State = GetKeyStateForKey(GamePad.bButton);
        }

        protected KeyState ReadTriggerValue(ButtonState buttonState, ButtonControl triggerControl)
        {
            var triggerState = false;
            var triggerValue = triggerControl.ReadValue();

            if (triggerValue > TriggerTreshold) {
                triggerState = true;
            }

            KeyState result;
            if (triggerState) {
                if (buttonState.PreviousState == KeyState.Up) {
                    result = KeyState.Pressed;
                } else {
                    result = KeyState.Down;
                }
            } else {
                if (buttonState.PreviousState == KeyState.Down) {
                    result = KeyState.Released;
                } else {
                    result = KeyState.Up;
                }
            }

            return result;
        }

        protected void ReadDirectionButtonStates(ButtonState[] buttonStates)
        {
            buttonStates[(int)DirectionButton.Up].State = Max(GetKeyStateForKey(GamePad.dpad.up), GetButtonstateForLeftStick(buttonStates, DirectionButton.Up));
            buttonStates[(int)DirectionButton.Down].State = Max(GetKeyStateForKey(GamePad.dpad.down), GetButtonstateForLeftStick(buttonStates, DirectionButton.Down));
            buttonStates[(int)DirectionButton.Left].State = Max(GetKeyStateForKey(GamePad.dpad.left), GetButtonstateForLeftStick(buttonStates, DirectionButton.Left));
            buttonStates[(int)DirectionButton.Right].State = Max(GetKeyStateForKey(GamePad.dpad.right), GetButtonstateForLeftStick(buttonStates, DirectionButton.Right));
        }

        private KeyState GetKeyStateForKey(ButtonControl button)
        {
            if (button.wasPressedThisFrame) {
                return KeyState.Pressed;
            } else if (button.wasReleasedThisFrame) {
                return KeyState.Released;
            } else if (button.isPressed) {
                return KeyState.Down;
            } else {
                return KeyState.Up;
            }
        }

        private KeyState Max(KeyState a, KeyState b) => (KeyState)Mathf.Max((byte)a, (byte)b);

        private KeyState GetButtonstateForLeftStick(ButtonState[] buttonStates, DirectionButton direction)
        {
            var readLeftStick = GamePad.leftStick.ReadValue();

            if (direction == DirectionButton.Left) {
                var currentState = buttonStates[(int)DirectionButton.Left];
                if (readLeftStick.x < -DirectionButtonTreshold) {
                    if (currentState.State == KeyState.Up || currentState.State == KeyState.Released) {
                        return KeyState.Pressed;
                    } else {
                        return KeyState.Down;
                    }
                } else {
                    if (currentState.State == KeyState.Down || currentState.State == KeyState.Pressed) {
                        return KeyState.Released;
                    } else {
                        return KeyState.Up;
                    }
                }
            } else if (direction == DirectionButton.Right) {
                var currentState = buttonStates[(int)DirectionButton.Right];
                if (readLeftStick.x > DirectionButtonTreshold) {
                    if (currentState.State == KeyState.Up || currentState.State == KeyState.Released) {
                        return KeyState.Pressed;
                    } else {
                        return KeyState.Down;
                    }
                } else {
                    if (currentState.State == KeyState.Down || currentState.State == KeyState.Pressed) {
                        return KeyState.Released;
                    } else {
                        return KeyState.Up;
                    }
                }
            } else if (direction == DirectionButton.Up) {
                var currentState = buttonStates[(int)DirectionButton.Up];
                if (readLeftStick.y > DirectionButtonTreshold) {
                    if (currentState.State == KeyState.Up || currentState.State == KeyState.Released) {
                        return KeyState.Pressed;
                    } else {
                        return KeyState.Down;
                    }
                } else {
                    if (currentState.State == KeyState.Down || currentState.State == KeyState.Pressed) {
                        return KeyState.Released;
                    } else {
                        return KeyState.Up;
                    }
                }
            } else if (direction == DirectionButton.Down) {
                var currentState = buttonStates[(int)DirectionButton.Down];
                if (readLeftStick.y < -DirectionButtonTreshold) {
                    if (currentState.State == KeyState.Up || currentState.State == KeyState.Released) {
                        return KeyState.Pressed;
                    } else {
                        return KeyState.Down;
                    }
                } else {
                    if (currentState.State == KeyState.Down || currentState.State == KeyState.Pressed) {
                        return KeyState.Released;
                    } else {
                        return KeyState.Up;
                    }
                }
            }

            return KeyState.Up;
        }
    }
}