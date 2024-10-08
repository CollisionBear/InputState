using UnityEngine;

namespace CollisionBear.InputState
{
    [System.Serializable]
    public class InputDeviceInstance : MonoBehaviour
    {
        public int DeviceIndex = InputManager.InvalidIndex;
        public IInputDevice InputDevice;
        public IInputHandler CurrentInputHandler;
        public IInWorldPosition InWorldPosition;

        private bool IsDisabled;
        private InputManager InputManager;

        public virtual void SetDevice(IInputDevice inputDevice, IInputHandler inputHandler, IIconSetProvider iconSetProvider, InputManager inputManager)
        {
            InputDevice = inputDevice;
            CurrentInputHandler = inputHandler;
            InputManager = inputManager;
        }

        public void SetInWorldPosition(IInWorldPosition inWorldPosition)
        {
            InWorldPosition = inWorldPosition;
        }

        public void MarkAsActive()
        { 
            if (!InputManager.ActiveDeviceInstances.Contains(this)) {
                InputManager.ActiveDeviceInstances.Add(this);
            }
        }

        public void MarkAsInactive()
        {
            if (InputManager.ActiveDeviceInstances.Contains(this)) {
                InputManager.ActiveDeviceInstances.Remove(this);
            }
        }

        public virtual void EnableDevice() { }
        public virtual void DisableDevice() { }

        private void Update()
        {
            if(CurrentInputHandler == null || IsDisabled) {
                return;
            }

            CurrentInputHandler.TakeInput(InputDevice.UpdateInputState(this), this);
        }

        private void LateUpdate()
        {
            InputDevice.InputLateUpdate(this);
        }

        public void DebugDisable()
        {
            IsDisabled = true;
        }

        public void DebugEnable()
        {
            IsDisabled = false;
        }

        public bool Disabled() => IsDisabled;
    }
}
