using UnityEngine;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] private CharacterController contoller;
        [SerializeField] private float defaultSpeed = 12f;
        private Vector2 horizontalInput;


        private void Update()
        {
            Vector3 horizontalVelocity = transform.right * horizontalInput.x + transform.forward * horizontalInput.y;
            horizontalVelocity = horizontalVelocity * defaultSpeed + Physics.gravity;
            contoller.Move(horizontalVelocity * Time.deltaTime);
        }


        public void ReciveInput(Vector2 horizontalInput) => this.horizontalInput = horizontalInput;
    }
}
