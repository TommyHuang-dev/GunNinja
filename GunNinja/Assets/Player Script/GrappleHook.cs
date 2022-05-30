using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHook : MonoBehaviour
{
    float hookRange = 50;
    float pullAccel = 0.1f;
    float softMaxPullSpeed = 30f;  // beyond this point, there will be a lot more "air resistance"
    float highVelocityDrag = 0.4f;  // drag coefficient when over softMaxPullpeed
    Rigidbody rb;
    PlayerMovement otherScript;

    Vector3 grapplePoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grapplePoint = new Vector3(0, 0, 0);
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

        // release grapple on keyup, otherwise pull towards point
        if (otherScript.isGrappling)
        {
            if (mouse.rightButton.wasReleasedThisFrame)
            {
                otherScript.isGrappling = false;
            } else
            {
                Vector3 vect = (grapplePoint - rb.transform.position).normalized;
                rb.velocity += vect * pullAccel;
            }
        }

        // greatly slow down in the air if over a certain speed
        if (!otherScript.isGrounded || !otherScript.isGrappling) {
            if (rb.velocity.magnitude > softMaxPullSpeed)
            {
                {
                    rb.velocity = rb.velocity - (rb.velocity.normalized * Mathf.Pow(rb.velocity.magnitude - softMaxPullSpeed + 1, 2) * highVelocityDrag * 0.001f);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
         Gizmos.DrawSphere(grapplePoint, 0.15f);
    }
}
