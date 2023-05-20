using UnityEngine;

namespace CollisionBear.InputState
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class MouseMarker : MonoBehaviour
    {
        public Vector3 Offset;
        private MeshRenderer MeshRenderer;

        public void OnCreated()
        {
            MeshRenderer = GetComponentInChildren<MeshRenderer>();
            MeshRenderer.material = new Material(MeshRenderer.material);
        }

        public void SetColor(Color color)
        {
            MeshRenderer.material.color = color;
        }

        public void SetPosition(Vector3 groundPosition)
        {
            transform.position = groundPosition + Offset;
        }
    }
}