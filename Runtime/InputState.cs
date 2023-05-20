using System.Collections.Generic;
using UnityEngine;

namespace Fyrvall.Input
{
    public enum InputType
    {
        GamePad = 0,
        Keyboard = 1
    }

    public class InputState
    {
        public static List<ButtonState> GroupedDownState = new List<ButtonState> { ButtonState.Down, ButtonState.Pressed };
        public static List<ButtonState> GroupedUpState = new List<ButtonState> { ButtonState.Up, ButtonState.Released };

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
        }

        public ButtonState GetButtonState(Button button) => ButtonStates[(int)button];
        public bool GetButtonDown(Button button) => GroupedDownState.Contains(ButtonStates[(int)button]);
        public bool GetButtonUp(Button button) => GroupedUpState.Contains(ButtonStates[(int)button]);
        public bool GetButtonPressed(Button button) => ButtonStates[(int)button] == ButtonState.Pressed;
        public bool GetButtonReleased(Button button) =>ButtonStates[(int)button] == ButtonState.Released;
        public bool GetButtonDown(DirectionButton button) => GroupedDownState.Contains(DirectionButtonStates[(int)button]);
        public bool GetButtonUp(DirectionButton button) => GroupedUpState.Contains(DirectionButtonStates[(int)button]);
        public bool GetButtonPressed(DirectionButton button) => DirectionButtonStates[(int)button] == ButtonState.Pressed;
        public bool GetButtonReleased(DirectionButton button) => DirectionButtonStates[(int)button] == ButtonState.Released;
    }
}
