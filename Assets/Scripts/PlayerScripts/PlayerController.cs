using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    public GameObject playerCam;

    public float walkSpeed, sprintSpeed, sneakSpeed;
    public float rotationSpeed;
    public float jumpHeight;
    float h, v, speed, mouseX, mouseY, rotY;
    bool sprinting, sneaking, jumping, isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        GetInputs();
        MovePlayer();
        PlayerCam();
        Jump();
    }

    void GetInputs()
    {
        sprinting = Input.GetKey(KeyCode.LeftControl);
        sneaking = Input.GetKey(KeyCode.LeftShift);

        if (sprinting && sneaking || !sprinting && !sneaking)
            speed = walkSpeed;
        else if (sprinting)
            speed = sprintSpeed;
        else
            speed = sneakSpeed;

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        v = Input.GetAxisRaw("Vertical");

        mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        rotY -= mouseY;
        rotY = Mathf.Clamp(rotY, -90f, 90f);

        jumping = Input.GetKey(KeyCode.Space);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Chunk"));
    }
    void MovePlayer()
    {
        Vector3 dir = (transform.forward * v + transform.right * h).normalized * speed;
        rb.velocity = new Vector3(dir.x, rb.velocity.y, dir.z);
    }
    void PlayerCam()
    {
        transform.Rotate(0f, mouseX, 0f);
        playerCam.transform.localEulerAngles = new Vector3(rotY, 0f, 0f);
    }
    void Jump()
    {
        if (jumping && isGrounded)
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
    }
}
