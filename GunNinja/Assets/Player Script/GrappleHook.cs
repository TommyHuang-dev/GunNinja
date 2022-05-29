using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHook : MonoBehaviour
{
    public float hookRange = 40;
    float pullAccel = 0.08f;
    float maxPullSpeed = 40f;
    Rigidbody rb;
    PlayerMovement otherScript;

    Vector3 grapplePoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        otherScript = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        var cam = Camera.main;
        // try grapple on keydown
        if (!(otherScript.isGrappling))
        {
            if (mouse.rightButton.wasPressedThisFrame)
            {
                Vector3 targetPos = cam.transform.position + rb.transform.forward * hookRange; // since crosshair is above player
                Ray ray = new Ray(rb.transform.position, targetPos - rb.transform.position);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, hookRange))
                {
                    otherScript.isGrappling = true;
                    otherScript.hasDoubleJump = true;
                    grapplePoint = rayHit.point;
                }
                Debug.DrawRay(rb.transform.position, targetPos - rb.transform.position, color: Color.black, duration: 0.5f);
            }
        }

        // release grapple on keyup
        if (otherScript.isGrappling)
        {
            if (mouse.rightButton.wasReleasedThisFrame)
            {
                otherScript.isGrappling = false;
            } else
            {
                Vector3 vect = (grapplePoint - rb.transform.position).normalized;
                // reduce accel when near the maximum
                rb.velocity += vect * pullAccel;
                if (rb.velocity.magnitude > maxPullSpeed)
                {
                    rb.velocity = rb.velocity * (maxPullSpeed / rb.velocity.magnitude);
                }
            }
        }


    }
}
