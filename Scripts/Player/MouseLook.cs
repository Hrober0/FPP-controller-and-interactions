using UnityEngine;

namespace Player
{
    public class MouseLook : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;

        [SerializeField] private float sensitivity = 3f;
        [SerializeField] [Range(0, -90)] private float minXAngle = -90f;
        [SerializeField] [Range(0, 90)] private float maxXAngle = 90f;
        private float mouseX, mouseY;
        private float xRotation = 0f, yRotation = 0f;

        void Update()
        {
            transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minXAngle, maxXAngle);
            playerCamera.localRotation = Quaternion.Euler(xRotation * sensitivity, 0, 0);

            yRotation += mouseX;
            transform.eulerAngles = new Vector2(0, yRotation * sensitivity);
        }

        public void ReciveInput(Vector2 mouseInput)
        {
            mouseX = mouseInput.x * sensitivity;
            mouseY = mouseInput.y * sensitivity;
        }
    }
}