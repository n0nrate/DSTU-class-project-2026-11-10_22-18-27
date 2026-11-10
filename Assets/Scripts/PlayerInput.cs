using UnityEngine;

namespace MetroPlatform
{
    public class PlayerInput : MonoBehaviour
    {
        public float InputAxisX;
        public bool InputJump;

        private void Start()
        {
        }

        private void Update()
        {
            InputAxisX = Input.GetAxisRaw("Horizontal");
            InputJump = Input.GetButtonDown("Jump");
        }
    }
}
