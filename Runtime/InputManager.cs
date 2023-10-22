using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace CollisionBear.InputState
{
    public class InputManager : MonoBehaviour
    {
        public const int InvalidIndex = -1;

        public static readonly List<Button> ActionButtonList = new List<Button> {
                Button.Action00,
                Button.Action01,
                Button.Action02,
                Button.Action03,
                Button.TriggerLeft,
                Button.TriggerRight
        };

        public static InputManager Instance;
        public class InputDeviceEvent : UnityEvent<InputDeviceInstance> { }

        public static InputManager InitializeInputManager(IEnumerable<IInputDevice> initialInputDevices, IIconSetProvider iconSetProvider, IInputHandler defaultInputHandler)
        {
            if (defaultInputHandler == null) {
                throw new System.ArgumentNullException("InitialInputDevices is null");
            }

            if (defaultInputHandler == null) {
                throw new System.ArgumentNullException("DefaultInputHandler is null");
            }

            var inputManager = new GameObject("InputManager");
            Instance = inputManager.AddComponent<InputManager>();
            Instance.Initialize(initialInputDevices, iconSetProvider, defaultInputHandler);
            GameObject.DontDestroyOnLoad(inputManager);
            return Instance;
        }

        public InputDeviceEvent OnInputDeviceAdded;
        public InputDeviceEvent OnInputDeviceRemoved;
        public InputDeviceEvent OnActiveDeviceRemoved;

        public List<InputDeviceInstance> InputDeviceInstances = new List<InputDeviceInstance>();

        public List<InputDeviceInstance> ActiveDeviceInstances = new List<InputDeviceInstance>();

        public IIconSetProvider IconSetProvider;
        public IInputHandler DefaultInputHandler;

        public void Initialize(IEnumerable<IInputDevice> initialInputDevices, IIconSetProvider iconSetProvider, IInputHandler defaultInputHandler)
        {
            IconSetProvider = iconSetProvider;
            DefaultInputHandler = defaultInputHandler;
            InitializeInputDevices(initialInputDevices, iconSetProvider);

            InputSystem.onDeviceChange += OnDeviceChange;
        }

        public void OnApplicationQuit()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        public void DebugAddNewKeyboardDevice()
        {
            var keyboardDeviceInstance = GetKeyboardDeviceInstance();
            if(keyboardDeviceInstance == null) {
                Debug.LogWarning("Failed to find KeyboardInputDevice");
                return;
            }

            keyboardDeviceInstance.DebugDisable();
            var deviceInstance = keyboardDeviceInstance.InputDevice.CreateInstance(DefaultInputHandler, IconSetProvider, this);
            InputDeviceInstances.Add(deviceInstance);
            OnInputDeviceAdded.Invoke(deviceInstance);
        }

        public void DebugToggleKeyboardDeviceIndex()
        {
            var allKeyboardDeviceInstances = GetAllKeyboardDeviceInstances();

            var currentActiveDevice = allKeyboardDeviceInstances.FirstOrDefault(i => i.Disabled());
            if(currentActiveDevice == null) {
                Debug.LogWarning("No active KeyboardInputDevice");
                return;
            }

            var currentIndex = allKeyboardDeviceInstances.IndexOf(currentActiveDevice);
            var nextIndex = (currentIndex +1) % allKeyboardDeviceInstances.Count;

            allKeyboardDeviceInstances[currentIndex].Disabled();
            allKeyboardDeviceInstances[nextIndex].Disabled();
        }

        private InputDeviceInstance GetKeyboardDeviceInstance()
        {
            foreach(var deviceInstance in InputDeviceInstances) {
                if(deviceInstance.InputDevice.GetDeviceType() == InputDeviceType.Keyboard && !deviceInstance.Disabled()) {
                    return deviceInstance;
                }
            }

            return null;
        }

        private List<InputDeviceInstance> GetAllKeyboardDeviceInstances()
        {
            var result = new List<InputDeviceInstance>();
            foreach (var deviceInstance in InputDeviceInstances) {
                if (deviceInstance.InputDevice.GetDeviceType() == InputDeviceType.Keyboard) {
                    result.Add(deviceInstance);
                }
            }

            return result;
        }

        private void InitializeInputDevices(IEnumerable<IInputDevice> initialInputDevices, IIconSetProvider iconSetProvider)
        {
            OnInputDeviceAdded = new InputDeviceEvent();
            OnInputDeviceRemoved = new InputDeviceEvent();
            OnActiveDeviceRemoved = new InputDeviceEvent();

            var inputDevices = new List<IInputDevice>();

            try {
                inputDevices.AddRange(initialInputDevices);
            } catch (System.Exception) {
                Debug.LogWarning("Failed to fetch legacy input devices");
            }

            try {
                foreach (var gamePad in Gamepad.all) {
                    inputDevices.Add(new InputSystemDevice(gamePad));
                }
            } catch (System.Exception) {
                Debug.LogWarning("Failed to fetch InputSystem's input devices");
            }

            var validDevices = inputDevices
                .Where(i => i.Setup())
                .Select(i => i.CreateInstance(DefaultInputHandler, iconSetProvider, this));

            InputDeviceInstances.AddRange(validDevices);
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (IsAddDevice(change)) {
                AddDevice(device);
            } else if (IsRemoveDevice(change)) {
                RemoveDevice(device);
            }
        }

        private bool IsAddDevice(InputDeviceChange change) => change == InputDeviceChange.Added || change == InputDeviceChange.Enabled || change == InputDeviceChange.Reconnected;

        private bool IsRemoveDevice(InputDeviceChange change) => change == InputDeviceChange.Disabled || change == InputDeviceChange.Disconnected || change == InputDeviceChange.Removed;

        private void AddDevice(InputDevice device)
        {
            var changedGamePad = GetGamePadForDeviceId(device.deviceId);

            if (ContainsDeviceWithId(changedGamePad.deviceId)) {
                return;
            }

            Debug.Log($"Device {device.deviceId} connected");

            var inputDevice = new InputSystemDevice(changedGamePad);
            if (inputDevice.Setup()) {
                var deviceInstance = inputDevice.CreateInstance(DefaultInputHandler, IconSetProvider, this);
                InputDeviceInstances.Add(deviceInstance);
                OnInputDeviceAdded.Invoke(deviceInstance);
            }
        }

        private void RemoveDevice(InputDevice device)
        {
            var inputDeviceInstance = FindDeviceInstanceForGamepad(device.deviceId);
            if (inputDeviceInstance != null) {
                OnInputDeviceRemoved.Invoke(inputDeviceInstance);
                InputDeviceInstances.Remove(inputDeviceInstance);

                if (ActiveDeviceInstances.Contains(inputDeviceInstance)) {
                    ActiveDeviceInstances.Remove(inputDeviceInstance);
                    OnActiveDeviceRemoved.Invoke(inputDeviceInstance);
                }

                Destroy(inputDeviceInstance.gameObject);
            }
        }

        private Gamepad GetGamePadForDeviceId(int deviceId)
        {
            foreach (var gamePad in Gamepad.all) {
                if (gamePad.deviceId == deviceId) {
                    return gamePad;
                }
            }

            return null;
        }

        private bool ContainsDeviceWithId(int deviceId)
        {
            foreach (var inputDeviceInstance in InputDeviceInstances) {
                if (inputDeviceInstance.InputDevice is InputSystemDevice) {
                    var inputSystemDevice = inputDeviceInstance.InputDevice as InputSystemDevice;
                    if (inputSystemDevice.GamePad.deviceId == deviceId) {
                        return true;
                    }
                }
            }

            return false;
        }

        private InputDeviceInstance FindDeviceInstanceForGamepad(int deviceId)
        {
            foreach (var deviceInstance in InputDeviceInstances) {
                if (!(deviceInstance.InputDevice is InputSystemDevice)) {
                    continue;
                }

                var inputSystemDevice = deviceInstance.InputDevice as InputSystemDevice;
                if (inputSystemDevice.GamePad.deviceId == deviceId) {
                    return deviceInstance;
                }
            }

            return null;
        }
    }
}