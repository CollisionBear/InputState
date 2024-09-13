using UnityEngine;

namespace CollisionBear.InputState
{
    [CreateAssetMenu(fileName = "New Keyboard", menuName = "InputState/Keyboard Device")]
    public class KeyboardDevice : ScriptableObject, IInputDevice
    {
        public enum MouseMarkerUpdateType
        {
            Update = 0,
            LateUpdate = 1,
        }


        public MouseMarker MouseMarkerPrefab;
        public MouseMarkerUpdateType MouseMarkerUpdate = MouseMarkerUpdateType.LateUpdate;

        private int GroundLayerMask;

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
        public KeyCode Action01 = KeyCode.Alpha2;
        public KeyCode Action02 = KeyCode.Alpha3;
        public KeyCode Action03 = KeyCode.Alpha4;
        public KeyCode Action04 = KeyCode.E;
        public KeyCode Action05 = KeyCode.Q;
        public KeyCode Action06 = KeyCode.None;
        public KeyCode Action07 = KeyCode.None;
        public KeyCode Action08 = KeyCode.None;
        public KeyCode Action09 = KeyCode.None;
        public KeyCode Action10 = KeyCode.None;
        public KeyCode Action11 = KeyCode.None;
        public KeyCode Action12 = KeyCode.None;
        public KeyCode Action13 = KeyCode.None;

        public KeyCode LeftBumper = KeyCode.LeftShift;
        public KeyCode RightBumper = KeyCode.RightShift;

        public KeyCode LeftTrigger = KeyCode.Q;
        public KeyCode RightTrigger = KeyCode.E;

        public KeyCode StickLeft = KeyCode.Space;
        public KeyCode StickRight = KeyCode.LeftControl;

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
        private ButtonState[] ButtonStates;

        public string Name => name;

        public InputDeviceType GetDeviceType() =>  InputDeviceType.Keyboard;

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
            SetupButtonStates(InputState);

            try {
                ButtonMappings = SetupKeyButtonMappings();
                DirectionArrowButtonMappings = SetupArrowKeyDirectionButtonMappings();
                DirectionWasdButtonMappings = SetupWasdKeyDirectionsButtonMappings();

                foreach (var button in ButtonUtils.ButtonIndices) {
                    InputState.ButtonStates[button].State = KeyState.Up;
                }

                foreach (var button in DirectionalButtonIndices) {
                    InputState.DirectionButtonStates[button].State = KeyState.Up;
                }

                return true;
            } catch (System.Exception e) {
                Debug.LogError($"Failed to setup KeyboardInputDevice: {e.Message}");
                return false;
            }
        }

        private void SetupButtonStates(InputState inputState)
        {
            ButtonStates = new ButtonState[ButtonUtils.ButtonIndices.Length];
            for(int i = 0; i < ButtonStates.Length; i ++) {
                ButtonStates[i] = new ButtonState();

                inputState.ButtonStates[ButtonUtils.ButtonIndices[i]] = ButtonStates[i];
            }
        }

        public InputState UpdateInputState(InputDeviceInstance instance)
        {
            var keyboardInstance = (KeyboardDeviceInstance)instance;
            InputState.LeftStick = ReadLeftStick();
            InputState.MousePosition = InWorldMousePosition(instance.InWorldPosition?.GetPosition());
            InputState.RightStick = ReadRightStick(InputState.MousePosition, instance.InWorldPosition?.GetPosition());
            ReadButtonState();
            ReadDirectionButtonState(InputState.DirectionButtonStates);

            if (MouseMarkerUpdate == MouseMarkerUpdateType.Update) {
                if (keyboardInstance.MouseMarker != null) {
                    keyboardInstance.MouseMarker.SetPosition(InputState.MousePosition);
                }
            }

            return InputState;
        }

        public InputState GetInputState() => InputState;

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
            result[(int)Button.BumberLeft] = LeftBumper;
            result[(int)Button.BumberRight] = RightBumper;
            result[(int)Button.StickLeft] = StickLeft;
            result[(int)Button.StickRight] = StickRight;
            result[(int)Button.Start] = Start;
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

            if (Input.GetKey(UpStickButton)) {
                result.y = 1;
            } else if (Input.GetKey(DownStickButton)) {
                result.y = -1;
            }

            if (Input.GetKey(LeftStickButton)) {
                result.x = -1;
            } else if (Input.GetKey(RightStickButton)) {
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

            Vector3 mouseOnScreenPosition = Input.mousePosition;
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

        private void ReadButtonState()
        {
            for(int i = 0; i < ButtonStates.Length; i ++) {
                ButtonStates[i].SetState(ButtonMappings[ButtonUtils.ButtonIndices[i]]);
            }
        }

        private void ReadDirectionButtonState(ButtonState[] buttonStates)
        {
            foreach (var button in DirectionalButtonIndices) {
                buttonStates[button].SetState(DirectionArrowButtonMappings[button], DirectionWasdButtonMappings[button]);
            }
        }

        public void SetColor(InputDeviceInstance instance, Color color)
        {
            var keyboardInstance = instance as KeyboardDeviceInstance;
            if (keyboardInstance.MouseMarker != null) {
                keyboardInstance.MouseMarker.SetColor(color);
            }
        }

        public void LateUpdate(InputDeviceInstance instance)
        {
            var keyboardInstance = instance as KeyboardDeviceInstance;
            if (MouseMarkerUpdate == MouseMarkerUpdateType.LateUpdate) {
                if (keyboardInstance.MouseMarker != null) {
                    keyboardInstance.MouseMarker.SetPosition(InputState.MousePosition);
                }
            }
        }
    }
}