using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.InputState
{
    [CreateAssetMenu(fileName = "New Icon set", menuName = "MageQuest/Input/Icon Set")]
    public class InputIconSet : ScriptableObject
    {
        public InputDeviceType Type = InputDeviceType.Unknown;

        [System.Serializable]
        public class IconPair
        {
            public Button Button;
            public Sprite Icon;
        }

        public List<IconPair> IconPairs = new List<IconPair>();

        public Sprite GetIconForButton(Button button)
        {
            foreach (var iconPair in IconPairs) {
                if (iconPair.Button == button) {
                    return iconPair.Icon;
                }
            }

            return null;
        }
    }
}