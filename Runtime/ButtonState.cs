using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace CollisionBear.InputState
{
    public class ButtonState
    {
        public KeyState State;
        public float Time;

        private KeyState PreviousState;

        public void SetState(KeyCode keyCode) {
            SetState(GetKeyStateForKey(keyCode));
        }

        public void SetState(KeyCode keyCodeA, KeyCode keyCodeB) {
            SetState(Max(GetKeyStateForKey(keyCodeA),  GetKeyStateForKey(keyCodeB)));
        }

        public void SetState(ButtonControl buttonControl) {
            SetState(GetKeyStateForButton(buttonControl));
        }

        public void SetState(ButtonControl buttonControl, float value, float directionButtonTreshold) {  
            SetState(Max(GetKeyStateForButton(buttonControl), GetLeftStickKeyState(value, directionButtonTreshold)));
        }

        private void SetState(KeyState newState) {
            Time += UnityEngine.Time.deltaTime;

            if(newState != PreviousState) {
                Time = 0;
            }

            PreviousState = State;
            State = newState;
        }

        private KeyState GetLeftStickKeyState(float value, float directionButtonTreshold) {
            if (value > directionButtonTreshold) {
                if (State == KeyState.Up || State == KeyState.Released) {
                    return KeyState.Pressed;
                } else {
                    return KeyState.Down;
                }
            } else {
                if (State == KeyState.Down || State == KeyState.Pressed) {
                    return KeyState.Released;
                } else {
                    return KeyState.Up;
                }
            }
        }

        private KeyState GetKeyStateForKey(KeyCode keyCode)
        {
            if (Input.GetKeyDown(keyCode)) {
                return KeyState.Pressed;
            } else if (Input.GetKeyUp(keyCode)) {
                return KeyState.Released;
            } else if (Input.GetKey(keyCode)) {
                return KeyState.Down;
            } else {
                return KeyState.Up;
            }
        }

        private KeyState GetKeyStateForButton(ButtonControl buttonControl)
        {
            if (buttonControl.wasPressedThisFrame) {
                return KeyState.Pressed;
            } else if (buttonControl.wasReleasedThisFrame) {
                return KeyState.Released;
            } else if (buttonControl.isPressed) {
                return KeyState.Down;
            } else {
                return KeyState.Up;
            }
        }

        public void SetStateFromTrigger(ButtonControl triggerControl, float triggerThreshold) {
            var newState = ReadTriggerValue(triggerControl, triggerThreshold);
            if(State != newState) {
                Time = 0;
            } else {
                Time += UnityEngine.Time.deltaTime;
            }

            State = newState;
        }

        private KeyState ReadTriggerValue(ButtonControl triggerControl, float triggerThreshold)
        {
            var triggerState = false;
            var triggerValue = triggerControl.ReadValue();

            if (triggerValue > triggerThreshold) {
                triggerState = true;
            }

            KeyState result;
            if (triggerState) {
                if (State == KeyState.Up) {
                    result = KeyState.Pressed;
                } else {
                    result = KeyState.Down;
                }
            } else {
                if (State == KeyState.Down) {
                    result = KeyState.Released;
                } else {
                    result = KeyState.Up;
                }
            }

            return result;
        }

        public KeyState Max(KeyState a, KeyState b) => (KeyState)Mathf.Max((byte)a, (byte)b);
    }

    public enum KeyState : byte
    {
        Up = 0,
        Down = 1,           // The button is in a pressed state
        Pressed = 2,        // The button was pressed this tick 
        Released = 3,       // The button was released this tick
    }
}