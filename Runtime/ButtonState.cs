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
            State = GetKeyStateForKey(keyCode);
        }

        public void SetState(KeyCode keyCodeA, KeyCode keyCodeB) {
            State = Max(GetKeyStateForKey(keyCodeA),  GetKeyStateForKey(keyCodeB));
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


        public void SetState(ButtonControl buttonControl) {
            State = GetKeyStateForButton(buttonControl);
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
            State = ReadTriggerValue(triggerControl, triggerThreshold);
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
                if (PreviousState == KeyState.Up) {
                    result = KeyState.Pressed;
                } else {
                    result = KeyState.Down;
                }
            } else {
                if (PreviousState == KeyState.Down) {
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