using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace CollisionBear.InputState
{
    [CreateAssetMenu(fileName = "New Input Device Configuration", menuName = "InputState/Gamepad Configuration")]
    public class InputDeviceConfiguration : ScriptableObject
    {
        [Header("Action buttons")]
        public GamepadButton Action00 = GamepadButton.A;
        public GamepadButton Action01 = GamepadButton.B;
        public GamepadButton Action02 = GamepadButton.X;
        public GamepadButton Action03 = GamepadButton.Y;
        public GamepadButton Action04 = GamepadButton.A;
        public GamepadButton Action05 = GamepadButton.B;
        public GamepadButton Action06 = GamepadButton.X;
        public GamepadButton Action07 = GamepadButton.Y;
        public GamepadButton Action08 = GamepadButton.A;
        public GamepadButton Action09 = GamepadButton.B;
        public GamepadButton Action10 = GamepadButton.X;
        public GamepadButton Action11 = GamepadButton.Y;
        public GamepadButton Action12 = GamepadButton.X;
        public GamepadButton Action13 = GamepadButton.Y;

        [Header("Bumpers")]
        public GamepadButton BumperLeft = GamepadButton.LeftShoulder;
        public GamepadButton BumperRight = GamepadButton.RightShoulder;

        public GamepadButton TriggerLeft = GamepadButton.LeftTrigger;
        public GamepadButton TriggerRight = GamepadButton.RightTrigger;

        public GamepadButton StickLeft = GamepadButton.LeftStick;
        public GamepadButton StickRight = GamepadButton.RightStick;

        public GamepadButton Start = GamepadButton.Start;
        public GamepadButton Select = GamepadButton.Select;

        public GamepadButton Accept = GamepadButton.A;
        public GamepadButton Cancel = GamepadButton.B;

        public GamepadButton[] GetButtonMapping()
        {
            var result = new GamepadButton[ButtonUtils.ButtonCount];

            result[(int)Button.Action00] = Action00;
            result[(int)Button.Action01] = Action01;
            result[(int)Button.Action02] = Action02;
            result[(int)Button.Action03] = Action03;
            result[(int)Button.Action04] = Action04;
            result[(int)Button.Action05] = Action05;
            result[(int)Button.Action06] = Action06;
            result[(int)Button.Action07] = Action07;
            result[(int)Button.Action08] = Action08;
            result[(int)Button.Action09] = Action09;
            result[(int)Button.Action10] = Action10;
            result[(int)Button.Action11] = Action11;
            result[(int)Button.Action12] = Action12;
            result[(int)Button.Action13] = Action13;

            result[(int)Button.BumberLeft] = BumperLeft;
            result[(int)Button.BumberRight] = BumperRight;

            result[(int)Button.TriggerLeft] = TriggerLeft;
            result[(int)Button.TriggerRight] = TriggerRight;

            result[(int)Button.StickLeft] = StickLeft;
            result[(int)Button.StickRight] = StickRight;

            result[(int)Button.Start] = Start;
            result[(int)Button.Select] = Select;

            result[(int)Button.Accept] = Accept;
            result[(int)Button.Cancel] = Cancel;

            return result;
        }
    }
}
