using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleHook : MonoBehaviour
{
    float hookRange = 30;
    float pullAccel = 0.1f;
    float softMaxPullSpeed = 24f;  // beyond this point, there will be a lot more "air resistance"
    float highVelocityDrag = 0.4f;  // drag coefficient when over softMaxPullpeed
    Rigidbody rb;
    PlayerMovement otherScript;

    Vector3 grapplePoint = new Vector3(0, 0, 0);
    Vector3 relativeGrapplePoint = new Vector3(0, 0, 0);
    bool isGrapplingEnemy;
    GameObject hitObject;

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
                float camPosDiff = Mathf.Sqrt(Mathf.Pow(cam.transform.localPosition.z * rb.transform.localScale.z, 2) + Mathf.Pow(cam.transform.localPosition.x * rb.transform.localScale.x, 2));
                Vector3 startPos = cam.transform.position + cam.transform.forward * camPosDiff;
                Vector3 targetPos = startPos + cam.transform.forward * hookRange;

                Ray ray = new Ray(startPos, (targetPos - startPos).normalized);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, hookRange))
                {
                    otherScript.isGrappling = true;
                    otherScript.hasDoubleJump = true;
                    grapplePoint = rayHit.point;
                    hitObject = rayHit.collider.gameObject;

                    isGrapplingEnemy = false;
                    if (hitObject.layer == 10 && hitObject.GetComponent<Rigidbody>() != null)
                    {
                        isGrapplingEnemy = true;
                    }
                } else
                {
                    Debug.DrawRay(rb.transform.position, targetPos - rb.transform.position, Color.black, 0.2f);
                }
            }
        }

        if (otherScript.isGrappling)
        {
            // release grapple on keyup or if too far away
            if ( (rb.transform.position - grapplePoint).magnitude > hookRange * 1.2 || mouse.rightButton.wasReleasedThisFrame)
            {
                otherScript.isGrappling = false;
            }
            // otherwise pull towards point
            else
            {
                Vector3 pullVect = (grapplePoint - rb.transform.position).normalized;

                // 10: Game Physics Objects also get pulled towards the player, player might get pulled less
                if (isGrapplingEnemy)
                {
                    Rigidbody targetRB = hitObject.GetComponent<Rigidbody>();
                    float totalMass = targetRB.mass + rb.mass;
                    // velocity player pulled to target
                    rb.velocity += pullVect * pullAccel * Mathf.Clamp((targetRB.mass * 1.25f) / totalMass, 0.25f, 1);
                    // velocity target pulled to player
                    targetRB.velocity -= (pullVect * pullAccel) * Mathf.Min((rb.mass * 1.25f) / totalMass, 2);
                    grapplePoint += targetRB.velocity * Time.deltaTime;
                }
                else
                {
                    rb.velocity += pullVect * pullAccel;
                }
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
        if (grapplePoint.x != 0 && grapplePoint.y != 0 && otherScript.isGrappling)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(rb.transform.position, grapplePoint);
            if (hitObject.layer == 10)
            {
                Gizmos.color = Color.cyan;
            }
            Gizmos.DrawSphere(grapplePoint, 0.15f);
        }
    }
}
