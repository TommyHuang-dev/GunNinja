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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        accel = 1.0f;
        maxSpeed = 15;  // max horizontal speed
        distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // horizontal speed
        float hSpeed = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
        bool isGrounded = OnGround();
            
        if (isGrounded)
        {
            rb.drag = 10;
            accel = 1.0f;
            // stop character from flying going over small ramps
            rb.AddForce(Physics.gravity * (rb.mass * rb.mass));
        } 
        else
        {
            rb.drag = 0.2f;
            accel = 0.1f;
        }

        if (isGrounded && Input.GetKeyDown("space"))
        {
            rb.velocity = new Vector3(0, 8, 0);
        }
        
        if(hSpeed < maxSpeed)
        {
            if (Input.GetKey("w"))
            {
                rb.velocity += new Vector3(accel, 0, 0);
            }
            if (Input.GetKey("s"))
            {
                rb.velocity += new Vector3(-accel, 0, 0);
            }
            if (Input.GetKey("a"))
            {
                rb.velocity += new Vector3(0, 0, accel);
            }
            if (Input.GetKey("d"))
            {
                rb.velocity += new Vector3(0, 0, -accel);
            }

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
        return Physics.Raycast(rb.transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
