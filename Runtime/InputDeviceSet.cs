using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.InputState
{
    [CreateAssetMenu(fileName = "New Input Device Set", menuName = "CollisionBear/Input/Device Set")]
    public class InputDeviceSet : ScriptableObject
    {
        public List<ScriptableObject> InputDevices;
    }
}