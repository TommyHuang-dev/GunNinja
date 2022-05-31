using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDrag : MonoBehaviour
{
    [SerializeField] float groundDrag = 1.0f;
    [SerializeField] float airDrag = 0.25f;
    [SerializeField] float maxSpeed = 50f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float finalDrag = 0;
        // set base drag
        if (OnGround())
        {
            finalDrag = groundDrag;
        }
        else
        {
            finalDrag = airDrag;
        }
        // limit max speed
        if (rb.velocity.magnitude > maxSpeed)
        {
            finalDrag = Mathf.Max(2.0f, finalDrag * 5);
        }

        rb.drag = finalDrag;
    }

    bool OnGround()
    {
        var distToGround = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(rb.transform.position, -Vector3.up, distToGround + 0.1f);
    }
}
