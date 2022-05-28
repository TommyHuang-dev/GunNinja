using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    Camera cam;
    float accel;
    float maxSpeed;
    float distToGround;

    float jumpStrength;
    bool doublejump;

    public float sensitivity = 4f;
    public float maxYAngle = 80f;
    private Vector2 currentRotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        accel = 1.2f;
        maxSpeed = 10;  // max horizontal speed
        distToGround = GetComponent<Collider>().bounds.extents.y;
        jumpStrength = 8;
        doublejump = true;
    }

    //private void Update()
    //{
    //    // get inputs

    //}

    // Update is called once per frame
    void Update()
    {
        // camera control
        currentRotation.x += Input.GetAxis("Mouse X") * sensitivity;
        currentRotation.y -= Input.GetAxis("Mouse Y") * sensitivity;
        currentRotation.x = Mathf.Repeat(currentRotation.x, 360);
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);
        rb.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        cam.transform.rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        if (Input.GetMouseButtonDown(0))
            Cursor.lockState = CursorLockMode.Locked;

        // horizontal speed
        float hSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
        bool isGrounded = OnGround();
            
        if (isGrounded)
        {
            rb.drag = 8;
            accel = 1.2f;
            doublejump = true;
            if (Input.GetKeyDown("space"))
            {
                rb.velocity = new Vector3(0, jumpStrength, 0);
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
            rb.drag = 0.2f;
            accel = 0.05f;
            if (doublejump && Input.GetKeyDown("space"))
            {
                doublejump = false;
                if (rb.velocity.y < 0) { rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); }
                rb.velocity += new Vector3(0, jumpStrength * 0.8f, 0);
                accel = 1.2f;
            }
        }

        if(hSpeed < maxSpeed)
        {
            var vect = new Vector3(0, 0, 0);
            if (Input.GetKey("w"))
            {
                vect = (vect + new Vector3(rb.transform.forward.x, 0, rb.transform.forward.z).normalized).normalized;
            }
            if (Input.GetKey("s"))
            {
                vect = (vect +new Vector3(- rb.transform.forward.x, 0, - rb.transform.forward.z).normalized).normalized;
            }
            if (Input.GetKey("a"))
            {
                vect = (vect + new Vector3(-rb.transform.right.x, 0, -rb.transform.right.z).normalized).normalized;
            }
            if (Input.GetKey("d"))
            {
                vect = (vect + new Vector3(rb.transform.right.x, 0, rb.transform.right.z).normalized).normalized;
            }
            rb.velocity += vect * accel;

            // limit running speed
            hSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
            if (hSpeed > maxSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x * maxSpeed / hSpeed, rb.velocity.y, rb.velocity.z * maxSpeed / hSpeed);
            }
        }
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
