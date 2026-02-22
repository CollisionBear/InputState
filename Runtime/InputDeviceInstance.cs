using UnityEngine;

namespace CollisionBear.InputState {
    [DefaultExecutionOrder(1)]
    public class InputDeviceInstance : MonoBehaviour {
        public int DeviceIndex = InputManager.InvalidIndex;
        public IInputDevice InputDevice;
        public IInputHandler CurrentInputHandler;
        public IInWorldPosition InWorldPosition;

        private bool IsDisabled;
        private InputManager InputManager;

        private InputState State;

        public virtual void SetDevice(IInputDevice inputDevice, IInputHandler inputHandler, IIconSetProvider iconSetProvider, InputManager inputManager) {
            InputDevice = inputDevice;
            CurrentInputHandler = inputHandler;
            InputManager = inputManager;
        }

        public void SetInWorldPosition(IInWorldPosition inWorldPosition) {
            InWorldPosition = inWorldPosition;
        }

        public void MarkAsActive() {
            if (!InputManager.ActiveDeviceInstances.Contains(this)) {
                InputManager.ActiveDeviceInstances.Add(this);
            }
        }

        public void MarkAsInactive() {
            if (InputManager.ActiveDeviceInstances.Contains(this)) {
                InputManager.ActiveDeviceInstances.Remove(this);
            }
        }

        public virtual void EnableDevice() { }
        public virtual void DisableDevice() { }

        private void Update() {
            if (IsDisabled) {
                return;
            }

            if(InputDevice.GetReadInputUpdate() == UpdateType.Update) {
                State = InputDevice.UpdateInputState(this); 
            }

            if (InputDevice.GetDelegateInputUpdate() == UpdateType.Update) {
                if(CurrentInputHandler == null) {
                    return;
                }

                CurrentInputHandler.TakeInput(State, this);
            }
        }

        private void LateUpdate() {
            if(IsDisabled) {
                return;
            }

            if (InputDevice.GetReadInputUpdate() == UpdateType.LateUpdate) {
                State = InputDevice.UpdateInputState(this);
            }

            if (InputDevice.GetDelegateInputUpdate() == UpdateType.LateUpdate) {
                if (CurrentInputHandler == null) {
                    return;
                }

                CurrentInputHandler.TakeInput(State, this);
            }

            InputDevice.InputLateUpdate(this);
        }

        public void DebugDisable() {
            IsDisabled = true;
        }

        public void DebugEnable() {
            IsDisabled = false;
        }

        public bool Disabled() => IsDisabled;
    }
}
