using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider cdr;

    float height;

    bool _isClimbing;

    private void Start()
    {
        cdr = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();

        height = transform.localScale.y;
        Debug.Log("Height: " + height);
    }

    private void Update()
    {
        if (!_isClimbing)
            transform.position += new Vector3(Input.GetAxis("Horizontal") * 5f * Time.deltaTime, 0f, Input.GetAxis("Vertical") * 5f * Time.deltaTime);
        else
        {
            transform.position += new Vector3(0f, Input.GetAxis("Horizontal") * 5f * Time.deltaTime, Input.GetAxis("Vertical") * 5f * Time.deltaTime);
        }

        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y-(height/2f), transform.position.z), transform.right);
        Debug.DrawRay(transform.position, transform.right, Color.red);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, 0.5f))
        {
            if (hitInfo.collider.name == "Ladder")
            {
                Debug.Log("Hit the ladder");
                _isClimbing = true;
                rb.useGravity = !_isClimbing;
            }
        }
        else
        {
            _isClimbing = false;
            rb.useGravity = !_isClimbing;
        }
    }
}
