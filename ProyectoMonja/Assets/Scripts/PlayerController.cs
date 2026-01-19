using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public CharacterController controller;
    public float speed   = 12f;
    public float gravity = -9.81f;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float jumpHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        if (Keyboard.current != null)
        {
            float x = 0;
            float z = 0;

            if (Keyboard.current.wKey.isPressed) z = 1;
            if (Keyboard.current.sKey.isPressed) z = -1;
            if (Keyboard.current.aKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed) x = 1;
            // Agacharse
            transform.localScale = new Vector3(1f, (Keyboard.current.qKey.isPressed) ? 0.5f : 1f, 1f);

            if (Keyboard.current.leftShiftKey.isPressed)
                speed = 20f;
            else
                speed = 12f;

            moveInput = new Vector2(x, z);

            if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);    // jumpHeight = nº metros
            }
        }
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMovement = (move * speed) + (Vector3.up * velocity.y); 
        controller.Move(finalMovement * Time.deltaTime);
    }
}
