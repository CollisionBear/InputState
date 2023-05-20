using System.Collections.Generic;
using UnityEngine;

namespace Fyrvall.Input
{
    [CreateAssetMenu(fileName = "New Input Device Set", menuName = "MageQuest/Input/Device Set")]
    public class InputDeviceSet : ScriptableObject
    {
        public List<ScriptableObject> InputDevices;
    }
}