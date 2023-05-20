using UnityEngine;

namespace Fyrvall.Input
{
    [CreateAssetMenu(fileName = "New Keyboard", menuName = "MageQuest/Input/Keyboard Device")]
    public class KeyboardDevice : ScriptableObject, IInputDevice
    {
        public MouseMarker MouseMarkerPrefab;

        private int GroundLayerMask;

        private static readonly int[] ButtonIndices = new int[] {
            (int)Button.Action00,
            (int)Button.Action01,
            (int)Button.Action02,
            (int)Button.Action03,
            (int)Button.Action04,
            (int)Button.Action05,
            (int)Button.Action06,
            (int)Button.Action07,
            (int)Button.TriggerLeft,
            (int)Button.TriggerRight,
            (int)Button.BumberLeft,
            (int)Button.BumberRight,
            (int)Button.Start,
            (int)Button.Select,
            (int)Button.Accept,
            (int)Button.Cancel
        };

        private static readonly int[] DirectionalButtonIndices = new int[] {
            (int)DirectionButton.Up,
            (int)DirectionButton.Down,
            (int)DirectionButton.Left,
            (int)DirectionButton.Right,
        };

        [InputAxis]
        public string MouseHorizontal;
        [InputAxis]
        public string MouseVertical;

        [Header("Button Mappings")]
        public KeyCode Action00 = KeyCode.Alpha1;
        public KeyCode Action01 = KeyCode.Alpha1;
        public KeyCode Action02 = KeyCode.Alpha3;
        public KeyCode Action03 = KeyCode.Alpha4;
        public KeyCode Action04 = KeyCode.E;
        public KeyCode Action05 = KeyCode.Q;
        public KeyCode Action06 = KeyCode.None;
        public KeyCode Action07 = KeyCode.None;

        public KeyCode LeftBumper = KeyCode.LeftShift;
        public KeyCode RightBumper = KeyCode.RightShift;

        public KeyCode LeftTrigger = KeyCode.Q;
        public KeyCode RightTrigger = KeyCode.E;

        public KeyCode Start = KeyCode.Escape;
        public KeyCode Select = KeyCode.Return;

        public KeyCode Accept = KeyCode.Return;
        public KeyCode Cancel = KeyCode.Escape;

        public KeyCode UpStickButton = KeyCode.W;
        public KeyCode DownStickButton = KeyCode.S;
        public KeyCode LeftStickButton = KeyCode.A;
        public KeyCode RightStickButton = KeyCode.D;

        public KeyCode UpButton = KeyCode.UpArrow;
        public KeyCode DownButton = KeyCode.DownArrow;
        public KeyCode LeftButton = KeyCode.LeftArrow;
        public KeyCode RightButton = KeyCode.RightArrow;

        private KeyCode[] ButtonMappings;
        private KeyCode[] DirectionArrowButtonMappings;
        private KeyCode[] DirectionWasdButtonMappings;

        private InputState InputState;

        public string Name {
            get { return name; }
        }

        public InputDeviceType GetDeviceType()
        {
            return InputDeviceType.Keyboard;
        }

        public InputDeviceInstance CreateInstance(IInputHandler inputHandler, IIconSetProvider iconSetProvider, InputManager inputManager)
        {
            var gameObject = new GameObject($"KeyBoardDeviceInstance-{Name}");
            DontDestroyOnLoad(gameObject);
            var result = gameObject.AddComponent<KeyboardDeviceInstance>();
            result.SetDevice(this, inputHandler, iconSetProvider, inputManager);

            return result;
        }

        public bool Setup()
        {
            GroundLayerMask = LayerMask.GetMask("Ground");
            InputState = new InputState(InputType.Keyboard);

            try {
                ButtonMappings = SetupKeyButtonMappings();
                DirectionArrowButtonMappings = SetupArrowKeyDirectionButtonMappings();
                DirectionWasdButtonMappings = SetupWasdKeyDirectionsButtonMappings();

                foreach (var button in ButtonIndices) {
                    InputState.ButtonStates[button] = ButtonState.Up;
                }

                foreach (var button in DirectionalButtonIndices) {
                    InputState.DirectionButtonStates[button] = ButtonState.Up;
                }

                return true;
            } catch (System.Exception e) {
                Debug.LogError($"Failed to setup KeyboardInputDevice: {e.Message}");
                return false;
            }
        }

        public InputState UpdateInputState(InputDeviceInstance instance)
        {
            var keyboardInstance = (KeyboardDeviceInstance)instance;
            InputState.LeftStick = ReadLeftStick();
            InputState.MousePosition = InWorldMousePosition(instance.InWorldPosition?.GetPosition());
            InputState.RightStick = ReadRightStick(InputState.MousePosition, instance.InWorldPosition?.GetPosition());
            ReadButtonState(InputState.ButtonStates);
            ReadDirectionButtonState(InputState.DirectionButtonStates);

            if (keyboardInstance.MouseMarker != null) {
                keyboardInstance.MouseMarker.SetPosition(InputState.MousePosition);
            }

            return InputState;
        }

        public InputState GetInputState()
        {
            return InputState;
        }

        private KeyCode[] SetupKeyButtonMappings()
        {
            var result = new KeyCode[ButtonUtils.ButtonCount];
            result[(int)Button.Action00] = Action00;
            result[(int)Button.Action01] = Action01;
            result[(int)Button.Action02] = Action02;
            result[(int)Button.Action03] = Action03;
            result[(int)Button.Action04] = Action04;
            result[(int)Button.Action05] = Action05;
            result[(int)Button.Action06] = Action06;
            result[(int)Button.Action07] = Action07;
            result[(int)Button.TriggerLeft] = LeftTrigger;
            result[(int)Button.TriggerRight] = RightTrigger;
            result[(int)Button.Start] = Start;
            result[(int)Button.BumberLeft] = LeftBumper;
            result[(int)Button.BumberRight] = RightBumper;
            result[(int)Button.Select] = Select;
            result[(int)Button.Accept] = Accept;
            result[(int)Button.Cancel] = Cancel;

            return result;
        }

        private KeyCode[] SetupArrowKeyDirectionButtonMappings()
        {
            var result = new KeyCode[ButtonUtils.ButtonCount];
            result[(int)DirectionButton.Up] = UpButton;
            result[(int)DirectionButton.Down] = DownButton;
            result[(int)DirectionButton.Left] = LeftButton;
            result[(int)DirectionButton.Right] = RightButton;

            return result;
        }

        private KeyCode[] SetupWasdKeyDirectionsButtonMappings()
        {
            var result = new KeyCode[ButtonUtils.ButtonCount];
            result[(int)DirectionButton.Up] = UpStickButton;
            result[(int)DirectionButton.Down] = DownStickButton;
            result[(int)DirectionButton.Left] = LeftStickButton;
            result[(int)DirectionButton.Right] = RightStickButton;

            return result;
        }

        private Vector2 ReadLeftStick()
        {
            var result = new Vector2();

            if (UnityEngine.Input.GetKey(UpStickButton)) {
                result.y = 1;
            } else if (UnityEngine.Input.GetKey(DownStickButton)) {
                result.y = -1;
            }

            if (UnityEngine.Input.GetKey(LeftStickButton)) {
                result.x = -1;
            } else if (UnityEngine.Input.GetKey(RightStickButton)) {
                result.x = 1;
            }

            return result.normalized;
        }

        private Vector3 InWorldMousePosition(Vector3? characterPosition)
        {
            var gameCamera = Camera.main;
            if (characterPosition == null || gameCamera == null) {
                return Vector3.zero;
            }

            Vector3 mouseOnScreenPosition = UnityEngine.Input.mousePosition;
            var ray = gameCamera.ScreenPointToRay(mouseOnScreenPosition);


            if (!Physics.Raycast(ray, out RaycastHit raycastHit, 250, GroundLayerMask, QueryTriggerInteraction.Ignore)) {
                return Vector3.zero;
            }

            var inCameraPosition = gameCamera.WorldToViewportPoint(raycastHit.point);
            inCameraPosition.x = Mathf.Clamp01(inCameraPosition.x);
            inCameraPosition.y = Mathf.Clamp01(inCameraPosition.y);
            var validPosition = gameCamera.ViewportToWorldPoint(inCameraPosition);

            return validPosition;
        }

        private Vector2 ReadRightStick(Vector3 inWorldPosition, Vector3? characterPosition)
        {
            if (characterPosition == null) {
                return Vector2.zero;
            }

            inWorldPosition.y = characterPosition.Value.y;
            var direction = (inWorldPosition - characterPosition.Value).normalized;

            return new Vector2(direction.x, direction.z);
        }

        private void ReadButtonState(ButtonState[] buttonStates)
        {
            foreach (var button in ButtonIndices) {
                buttonStates[button] = GetButtonstateForKey(ButtonMappings[button]);
            }
        }

        private void ReadDirectionButtonState(ButtonState[] buttonStates)
        {
            foreach (var button in DirectionalButtonIndices) {
                buttonStates[button] = Max(GetButtonstateForKey(DirectionArrowButtonMappings[button]),  GetButtonstateForKey(DirectionWasdButtonMappings[button]));
            }
        }

        private ButtonState Max(ButtonState a, ButtonState b) => (ButtonState)Mathf.Max((int)a, (int)b);

        private ButtonState GetButtonstateForKey(KeyCode key)
        {
            if (UnityEngine.Input.GetKeyDown(key)) {
                return ButtonState.Pressed;
            } else if (UnityEngine.Input.GetKeyUp(key)) {
                return ButtonState.Released;
            } else if (UnityEngine.Input.GetKey(key)) {
                return ButtonState.Down;
            } else {
                return ButtonState.Up;
            }
        }

        public void SetColor(InputDeviceInstance instance, Color color)
        {
            var keyboardInstance = instance as KeyboardDeviceInstance;
            if (keyboardInstance.MouseMarker != null) {
                keyboardInstance.MouseMarker.SetColor(color);
            }
        }
    }
}