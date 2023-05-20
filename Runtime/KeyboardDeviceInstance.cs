using UnityEngine;

namespace CollisionBear.InputState
{
    [System.Serializable]
    public class KeyboardDeviceInstance : InputDeviceInstance
    {
        public MouseMarker MouseMarker;

        public override void SetDevice(IInputDevice inputDevice, IInputHandler inputHandler, IIconSetProvider iconSetProvider, InputManager inputManager)
        {
            base.SetDevice(inputDevice, inputHandler, iconSetProvider, inputManager);

            var keyboardDevice = inputDevice as KeyboardDevice;

            if (keyboardDevice.MouseMarkerPrefab != null) {
                MouseMarker = Instantiate(keyboardDevice.MouseMarkerPrefab);
                MouseMarker.OnCreated();
                MouseMarker.gameObject.SetActive(false);
                DontDestroyOnLoad(MouseMarker.gameObject);
            } else {
                Debug.LogWarning("KeyboardDeivce MouseMarkerPrefab is null");
            }
        }

        public override void EnableDevice()
        {
            if (MouseMarker != null) {
                MouseMarker.gameObject.SetActive(true);
            }
        }

        public override void DisableDevice()
        {
            if (MouseMarker != null) {
                MouseMarker.gameObject.SetActive(false);
            }
        }

    }
}
