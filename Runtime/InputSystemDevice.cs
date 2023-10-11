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
            buttonStates[(int)Button.TriggerLeft] = ReadTriggerValue(buttonStates[(int)Button.TriggerLeft], GamePad.leftTrigger);
            buttonStates[(int)Button.TriggerRight] = ReadTriggerValue(buttonStates[(int)Button.TriggerRight], GamePad.rightTrigger);

            buttonStates[(int)Button.BumberLeft] = GetButtonstateForKey(GamePad.leftShoulder);
            buttonStates[(int)Button.BumberRight] = GetButtonstateForKey(GamePad.rightShoulder);

            buttonStates[(int)Button.StickLeft] = GetButtonstateForKey(GamePad.leftStickButton);
            buttonStates[(int)Button.StickRight] = GetButtonstateForKey(GamePad.rightStickButton);

            buttonStates[(int)Button.Start] = GetButtonstateForKey(GamePad.startButton);
            buttonStates[(int)Button.Select] = GetButtonstateForKey(GamePad.selectButton);
            buttonStates[(int)Button.Action00] = GetButtonstateForKey(GamePad.aButton);
            buttonStates[(int)Button.Action01] = GetButtonstateForKey(GamePad.bButton);
            buttonStates[(int)Button.Action02] = GetButtonstateForKey(GamePad.xButton);
            buttonStates[(int)Button.Action03] = GetButtonstateForKey(GamePad.yButton);
            buttonStates[(int)Button.Action04] = GetButtonstateForKey(GamePad.dpad.up);
            buttonStates[(int)Button.Action05] = GetButtonstateForKey(GamePad.dpad.down);
            buttonStates[(int)Button.Action06] = GetButtonstateForKey(GamePad.dpad.left);
            buttonStates[(int)Button.Action07] = GetButtonstateForKey(GamePad.dpad.right);
            buttonStates[(int)Button.Accept] = GetButtonstateForKey(GamePad.aButton);
            buttonStates[(int)Button.Cancel] = GetButtonstateForKey(GamePad.bButton);
        }

        protected ButtonState ReadTriggerValue(ButtonState previousState, ButtonControl triggerControl)
        {
            var triggerState = false;
            var triggerValue = triggerControl.ReadValue();

            if (triggerValue > TriggerTreshold) {
                triggerState = true;
            }

            ButtonState result;
            if (triggerState) {
                if (previousState == ButtonState.Up) {
                    result = ButtonState.Pressed;
                } else {
                    result = ButtonState.Down;
                }
            } else {
                if (previousState == ButtonState.Down) {
                    result = ButtonState.Released;
                } else {
                    result = ButtonState.Up;
                }
            }

            return result;
        }

        protected void ReadDirectionButtonStates(ButtonState[] buttonStates)
        {
            buttonStates[(int)DirectionButton.Up] = Max(GetButtonstateForKey(GamePad.dpad.up), GetButtonstateForLeftStick(buttonStates, DirectionButton.Up));
            buttonStates[(int)DirectionButton.Down] = Max(GetButtonstateForKey(GamePad.dpad.down), GetButtonstateForLeftStick(buttonStates, DirectionButton.Down));
            buttonStates[(int)DirectionButton.Left] = Max(GetButtonstateForKey(GamePad.dpad.left), GetButtonstateForLeftStick(buttonStates, DirectionButton.Left));
            buttonStates[(int)DirectionButton.Right] = Max(GetButtonstateForKey(GamePad.dpad.right), GetButtonstateForLeftStick(buttonStates, DirectionButton.Right));
        }

        private ButtonState GetButtonstateForKey(ButtonControl button)
        {
            if (button.wasPressedThisFrame) {
                return ButtonState.Pressed;
            } else if (button.wasReleasedThisFrame) {
                return ButtonState.Released;
            } else if (button.isPressed) {
                return ButtonState.Down;
            } else {
                return ButtonState.Up;
            }
        }

        private ButtonState Max(ButtonState a, ButtonState b) => (ButtonState)Mathf.Max((int)a, (int)b);

        private ButtonState GetButtonstateForLeftStick(ButtonState[] buttonStates, DirectionButton direction)
        {
            var readLeftStick = GamePad.leftStick.ReadValue();

            if (direction == DirectionButton.Left) {
                var currentState = buttonStates[(int)DirectionButton.Left];
                if (readLeftStick.x < -DirectionButtonTreshold) {
                    if (currentState == ButtonState.Up || currentState == ButtonState.Released) {
                        return ButtonState.Pressed;
                    } else {
                        return ButtonState.Down;
                    }
                } else {
                    if (currentState == ButtonState.Down || currentState == ButtonState.Pressed) {
                        return ButtonState.Released;
                    } else {
                        return ButtonState.Up;
                    }
                }
            } else if (direction == DirectionButton.Right) {
                var currentState = buttonStates[(int)DirectionButton.Right];
                if (readLeftStick.x > DirectionButtonTreshold) {
                    if (currentState == ButtonState.Up || currentState == ButtonState.Released) {
                        return ButtonState.Pressed;
                    } else {
                        return ButtonState.Down;
                    }
                } else {
                    if (currentState == ButtonState.Down || currentState == ButtonState.Pressed) {
                        return ButtonState.Released;
                    } else {
                        return ButtonState.Up;
                    }
                }
            } else if (direction == DirectionButton.Up) {
                var currentState = buttonStates[(int)DirectionButton.Up];
                if (readLeftStick.y > DirectionButtonTreshold) {
                    if (currentState == ButtonState.Up || currentState == ButtonState.Released) {
                        return ButtonState.Pressed;
                    } else {
                        return ButtonState.Down;
                    }
                } else {
                    if (currentState == ButtonState.Down || currentState == ButtonState.Pressed) {
                        return ButtonState.Released;
                    } else {
                        return ButtonState.Up;
                    }
                }
            } else if (direction == DirectionButton.Down) {
                var currentState = buttonStates[(int)DirectionButton.Down];
                if (readLeftStick.y < -DirectionButtonTreshold) {
                    if (currentState == ButtonState.Up || currentState == ButtonState.Released) {
                        return ButtonState.Pressed;
                    } else {
                        return ButtonState.Down;
                    }
                } else {
                    if (currentState == ButtonState.Down || currentState == ButtonState.Pressed) {
                        return ButtonState.Released;
                    } else {
                        return ButtonState.Up;
                    }
                }
            }

            return ButtonState.Up;
        }
    }
}