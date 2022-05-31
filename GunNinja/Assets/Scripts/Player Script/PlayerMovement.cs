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
    float groundDrag = 8f;
    float airAccel = 0.04f;
    float airDrag = 0.2f;
    float maxRunSpeed = 10;  // soft max speed
    float distToGround;

    float jumpStrength = 10f;
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
        isGrounded = OnGround();
            
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
                rb.velocity += new Vector3(0, jumpStrength * 0.6f, 0);
                accel = groundAccel * 2;  // burst of acceleration during double jump
            }
        }

        var dirVect = new Vector3(0, 0, 0);
        if (keys.wKey.isPressed)
        {
            dirVect = (dirVect + new Vector3(rb.transform.forward.x, 0, rb.transform.forward.z).normalized).normalized;
        }
        if (keys.sKey.isPressed)
        {
            dirVect = (dirVect +new Vector3(- rb.transform.forward.x, 0, - rb.transform.forward.z).normalized).normalized;
        }
        if (keys.aKey.isPressed)
        {
            dirVect = (dirVect + new Vector3(-rb.transform.right.x, 0, -rb.transform.right.z).normalized).normalized;
        }
        if (keys.dKey.isPressed)
        {
            dirVect = (dirVect + new Vector3(rb.transform.right.x, 0, rb.transform.right.z).normalized).normalized;
        }

        Vector3 curVelVect = rb.velocity;
        Vector3 newVelVect = rb.velocity + accel * dirVect;

        // counteract drag slightly if going in same direction
        if (dirVect.magnitude != 0 && Vector3.Angle(horizVect(newVelVect), horizVect(curVelVect)) < 60)
        {
            float angleCoeff = 20f / (20f + Vector3.Angle(horizVect(newVelVect), horizVect(curVelVect)));
            float coeff = 0.075f / 100;
            if (isGrounded)
            {
                coeff *= 2f;
            }
            curVelVect += curVelVect.normalized * curVelVect.magnitude * rb.drag * coeff * angleCoeff;
        }

        // limit running / air strafing speed
        if (horizSpeed(newVelVect) > maxRunSpeed && horizSpeed(newVelVect) > horizSpeed(curVelVect))
        {
            // basically trying to "turn" the vector here instead of increasing it
            newVelVect -= accel * curVelVect.normalized;
            newVelVect.y = 0;
            newVelVect = newVelVect.normalized * horizSpeed(curVelVect); 
            newVelVect.y = curVelVect.y + accel * dirVect.y;
        }

        rb.velocity = newVelVect;

        // DEBUGGING CHEAT
        if (keys.fKey.wasPressedThisFrame)
        {
            rb.velocity += rb.transform.forward * 100;
        }
        // -----
        Debug.Log("horizontal speed :" + horizSpeed(rb.velocity));
    }
    Vector3 horizVect(Vector3 vel)
    {
        return new Vector3(vel.x, 0, vel.z);
    }

    // speed, ignoring the y direction
    float horizSpeed(Vector3 vel)
    {
        return Mathf.Sqrt(vel.x * vel.x + vel.z * vel.z);
    }


    bool OnGround()
    {
        float dist = 0.1f;
        return Physics.Raycast(rb.transform.position, -Vector3.up, distToGround + 0.1f)
            || Physics.Raycast(rb.transform.position + rb.transform.forward * dist, -Vector3.up, distToGround + 0.05f)
            || Physics.Raycast(rb.transform.position - rb.transform.forward * dist, -Vector3.up, distToGround + 0.05f)
            || Physics.Raycast(rb.transform.position + rb.transform.right * dist, -Vector3.up, distToGround + 0.05f)
            || Physics.Raycast(rb.transform.position - rb.transform.right * dist, -Vector3.up, distToGround + 0.05f);
    }
}
