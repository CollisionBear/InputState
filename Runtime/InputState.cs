using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.InputState
{
    public enum InputType
    {
        GamePad = 0,
        Keyboard = 1
    }

    public class InputState
    {
        public static List<KeyState> GroupedDownState = new List<KeyState> {
            KeyState.Down,
            KeyState.Pressed
        }
        ;
        public static List<KeyState> GroupedUpState = new List<KeyState> {
            KeyState.Up, 
            KeyState.Released
        };

        public InputType Type;
        public Vector2 LeftStick;
        public Vector2 RightStick;
        public Vector3 MousePosition;

        public ButtonState[] ButtonStates;
        public ButtonState[] DirectionButtonStates;

        public InputState(InputType type)
        {
            Type = type;
            ButtonStates = new ButtonState[ButtonUtils.ButtonCount];

            DirectionButtonStates = new ButtonState[ButtonUtils.ButtonCount];
            for(int i = 0; i < DirectionButtonStates.Length; i ++) {
                DirectionButtonStates[i] = new ButtonState();
            }
        }

        public ButtonState GetButtonState(Button button) => ButtonStates[(int)button];
        public float GetButtonTime(Button button) => ButtonStates[(int)button].Time;
        public bool GetButtonDown(Button button) => GroupedDownState.Contains(ButtonStates[(int)button].State);
        public bool GetButtonUp(Button button) => GroupedUpState.Contains(ButtonStates[(int)button].State);
        public bool GetButtonPressed(Button button) => ButtonStates[(int)button].State == KeyState.Pressed;
        public bool GetButtonReleased(Button button) =>ButtonStates[(int)button].State == KeyState.Released;
        public bool GetButtonDown(DirectionButton button) => GroupedDownState.Contains(DirectionButtonStates[(int)button].State);
        public bool GetButtonUp(DirectionButton button) => GroupedUpState.Contains(DirectionButtonStates[(int)button].State);
        public bool GetButtonPressed(DirectionButton button) => DirectionButtonStates[(int)button].State == KeyState.Pressed;
        public bool GetButtonReleased(DirectionButton button) => DirectionButtonStates[(int)button].State == KeyState.Released;
        public void ConsumeButton(Button button)
        {
            ButtonStates[(int)button].State = KeyState.Consumed;
        }
    }
}
