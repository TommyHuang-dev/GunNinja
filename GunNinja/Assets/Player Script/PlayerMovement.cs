using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Camera cam;
    float accel;

    float groundAccel = 1.0f;
    float groundDrag = 10f;
    float airAccel = 0.02f;
    float airDrag = 0.25f;
    float maxRunSpeed = 8;  // soft max speed
    float distToGround;

    float jumpStrength = 8f;
    public bool hasDoubleJump;

    public bool isGrounded = false;
    public bool isGrappling = false;

    private float sensitivity = 0.5f;
    private float maxYAngle = 80f;
    private Vector2 currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        accel = groundAccel;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        hasDoubleJump = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        var keys = Keyboard.current;
        var pointer = Pointer.current;

        // camera control
        currentRotation.x += pointer.delta.x.ReadValue() * 0.1f * sensitivity;
        currentRotation.y -= pointer.delta.y.ReadValue() * 0.1f * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        rb.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        cam.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);

        // horizontal speed
        float hSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
        isGrounded = OnGround() && !isGrappling;
            
        if (isGrounded)
        {
            rb.drag = groundDrag;
            accel = groundAccel;
            hasDoubleJump = true;
            if (keys.spaceKey.wasPressedThisFrame)
            {
                rb.velocity += new Vector3(0, jumpStrength, 0);
                rb.transform.position += new Vector3(0, 0.1f, 0);
            }
            else
            {
                // stop character from flying going over small ramps
                Ray ray = new Ray(rb.transform.position + new Vector3(0, 0.1f, 0), -Vector3.up);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, distToGround + 0.25f)) {
                    rb.MovePosition(rb.transform.position + new Vector3(0, - rayHit.distance + distToGround + 0.1f, 0));
                }
            }
        } 
        else
        {
            rb.drag = airDrag;
            accel = airAccel;
            if (hasDoubleJump && keys.spaceKey.wasPressedThisFrame)
            {
                hasDoubleJump = false;
                // soft reset if falling downwards
                if (rb.velocity.y < 0) { rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.25f, rb.velocity.z); }
                rb.velocity += new Vector3(0, jumpStrength * 0.75f, 0);
                accel = groundAccel;  // burst of acceleration during double jump
            }
        }

        // todo fix this with airstrafing
        var vect = new Vector3(0, 0, 0);
        if (keys.wKey.isPressed)
        {
            vect = (vect + new Vector3(rb.transform.forward.x, 0, rb.transform.forward.z).normalized).normalized;
        }
        if (keys.sKey.isPressed)
        {
            vect = (vect +new Vector3(- rb.transform.forward.x, 0, - rb.transform.forward.z).normalized).normalized;
        }
        if (keys.aKey.isPressed)
        {
            vect = (vect + new Vector3(-rb.transform.right.x, 0, -rb.transform.right.z).normalized).normalized;
        }
        if (keys.dKey.isPressed)
        {
            vect = (vect + new Vector3(rb.transform.right.x, 0, rb.transform.right.z).normalized).normalized;
        }
        rb.velocity += vect * accel;

        // limit running speed above the max speed
        hSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
        if (hSpeed > maxRunSpeed && isGrounded)
        {
            rb.velocity = rb.velocity - (rb.velocity.normalized * Mathf.Pow(rb.velocity.magnitude - maxRunSpeed, 2) * 0.1f);
            // rb.drag = groundDrag * 3;
        }
        // Debug.Log("horizontal speed: " + Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z));
        Debug.Log("speed :" + rb.velocity.magnitude);
    }

    bool OnGround()
    {
        return Physics.Raycast(rb.transform.position, -Vector3.up, distToGround + 0.1f)
            || Physics.Raycast(rb.transform.position + new Vector3(0.25f, 0, 0), -Vector3.up, distToGround + 0.05f)
            || Physics.Raycast(rb.transform.position + new Vector3(-0.25f, 0, 0), -Vector3.up, distToGround + 0.05f)
            || Physics.Raycast(rb.transform.position + new Vector3(0, 0, 0.25f), -Vector3.up, distToGround + 0.05f)
            || Physics.Raycast(rb.transform.position + new Vector3(0, 0, -0.25f), -Vector3.up, distToGround + 0.05f);
    }
}
