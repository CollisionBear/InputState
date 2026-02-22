using UnityEngine;

namespace CollisionBear.InputState {
    [CreateAssetMenu(fileName = "New InputState configuration", menuName = Utils.AssetRoot + "Configuration")]
    public class InputStateConfiguration : ScriptableObject {
        public KeyboardDevice KeyboarDevice;
        public InputDeviceConfiguration DefaultGamepadConfigration;
    }
}