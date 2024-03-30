using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public GameObject playerCam;
    public GameObject selectedBlock;
    Rigidbody rb;

    [Header("Movement values")]
    public float walkSpeed;
    public float sprintSpeed, sneakSpeed;
    [Space(20)]
    public float rotationSpeed;
    public float jumpHeight;

    [Header("Block Editing")]
    public float reach;
    public byte selectedBlockIndex;
    
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
        HandleSelectedBlock();
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
        rb.velocity = new Vector3(Mathf.Lerp(rb.velocity.x, dir.x, Time.deltaTime * 5f), rb.velocity.y, Mathf.Lerp(rb.velocity.z, dir.z, Time.deltaTime * 5f));
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
    void HandleSelectedBlock()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, reach, LayerMask.GetMask("Chunk")))
        {
            selectedBlock.SetActive(true);

            Vector3Int breakPos = VoxelData.FloorToInt(hit.point - (hit.normal * 0.5f));
            Vector3Int placePos = VoxelData.FloorToInt(hit.point + (hit.normal * 0.5f));
            
            selectedBlock.transform.position = VoxelData.FloorToInt(hit.point - (hit.normal * 0.5f));

            if (Input.GetMouseButtonDown(0))
                World.world.EditVoxel(breakPos, 0);
            if (Input.GetMouseButtonDown(1))
                World.world.EditVoxel(placePos, selectedBlockIndex);
            if (Input.GetMouseButtonDown(2))
                selectedBlockIndex = World.world.CheckVoxel(breakPos);
        }
        else
            selectedBlock.SetActive(false);
    }
}
