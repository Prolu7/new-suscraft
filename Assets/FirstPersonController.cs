using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    //references
    Transform camTf;
    Camera playerCam;

    [Header("Camera")]
    [SerializeField] float sensitivity = 1f;
    float yRot = 0f;
    float mouseX;
    float mouseY;

    [Header("Walking")]
    [SerializeField] CharacterController controller;
    [SerializeField] float gravity = -9.81f;
    public float acceleration;
    [SerializeField] float groundDistance = .2f;
    public bool Grounded;
    float horizontal;
    float vertical;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpHeight = 20f;
    [SerializeField] float jumpCooldown = .3f;
    float jumpTimer = 0f;
    float speed;
    public bool Sprinting;
    public Vector3 move;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCam = GetComponentInChildren<Camera>();
        camTf = playerCam.transform;
        if(controller == null)
            controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //inputting
        mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //cameraring
        transform.Rotate(Vector3.up * mouseX);
        yRot += mouseY;
        yRot = Mathf.Clamp(yRot, -90f, 90f);
        camTf.localRotation = Quaternion.Euler(Vector3.left * yRot);

        //jumparing
        jumpTimer += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && Grounded && jumpCooldown < jumpTimer)
        { 
            acceleration += jumpHeight;
            jumpTimer = 0f;
        }

        //gravitating
        float maxAccel = 78.4f;
        acceleration = Mathf.Clamp(acceleration, -maxAccel, maxAccel);
        float sphereCastRadius = controller.radius - .1f;
        float castLength = groundDistance + sphereCastRadius;
        Vector3 rayOrigin = transform.position + transform.up * sphereCastRadius * 2f;

        //groundating

        //good ground check
        //Grounded = Physics.Raycast(rayOrigin, transform.up * -1f, groundDistance + .2f);

        Grounded = Physics.SphereCast(rayOrigin, sphereCastRadius, Vector3.down, out RaycastHit hitInfo, castLength);
        
        if (!Grounded)
            acceleration += gravity * Time.deltaTime;
        else if (acceleration < 0)
            acceleration = -1f;

        controller.Move(Vector3.up * acceleration * Time.deltaTime);

        //runnaring
        Sprinting = Input.GetKey(KeyCode.LeftControl) && vertical > 0;
        if (Sprinting)
            speed = runSpeed;
        else
            speed = walkSpeed;

        //controllaring
        move = horizontal * transform.right + vertical * transform.forward;
        controller.Move(move.normalized * Time.deltaTime * speed);
    }
}
