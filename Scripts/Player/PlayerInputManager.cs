using UnityEngine;

namespace Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;

        [SerializeField] private Movement movement;
        private Vector2 horziontalMovement;

        [SerializeField] private MouseLook mouseLook;
        private Vector2 mouseInput;
        private float mouseSensivity = 1f;

        [SerializeField] private PlayerInteractions interactions;
        private float mouseHold;

        private PlayerControls controls;

        private void Awake()
        {
            controls = new PlayerControls();

            PlayerControls.MoveActions move = controls.Move;
            move.Movement.performed += ctx => horziontalMovement = ctx.ReadValue<Vector2>();
            move.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
            move.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();

            PlayerControls.InteractionsActions interact = controls.Interactions;
            interact.Interact.performed += _ => interactions.InteractWithLookObject();
            interact.Hold.performed += ctx => mouseHold = ctx.ReadValue<float>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (interactions.CanMove && Time.timeScale > 0)
            {
                movement.ReciveInput(horziontalMovement);
                mouseLook.ReciveInput(mouseInput * mouseSensivity);
            }
            else
            {
                movement.ReciveInput(Vector2.zero);
                mouseLook.ReciveInput(Vector2.zero);
            }
            interactions.ReciveInput(mouseHold >= 0.5f);
        }

        private void OnEnable() => controls.Enable();

        private void OnDisable() => controls.Disable();

        public void SetMouseSensivity(float value) => mouseSensivity = value;
        public void SetCameraFOV(float value) => playerCamera.fieldOfView = value;
    }
}