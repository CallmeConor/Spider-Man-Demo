using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer lr;
    Rigidbody rb;

    // Pendulum effect affectors
    public float dampeningEffect;
    public float gravityEffect;
    public float massOfBob;
    
    // Variables
    float angle;
    float aVel = 0.0f;
    float aAcc = 0.0f;
    float lineLength;

    Vector3 originPosition;

    public Camera mainCamera;
    public Collider MouseRayCastTarget;

    bool _swinging;

    // Init
    private void Start()
    {
        angle = 0;
        lr = transform.GetChild(0).GetComponent<LineRenderer>();
        rb = transform.GetComponent<Rigidbody>();

        _swinging = false;

        originPosition = this.transform.position;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            // Check mouse position when clicked for a "ropeable" surface
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (MouseRayCastTarget.Raycast(ray, out hitInfo, Mathf.Infinity) && !_swinging)
            {
                Debug.Log("Act like Spider-Man, woo");
                originPosition =  hitInfo.transform.gameObject.transform.position;

                lr.gameObject.SetActive(true);
                lr.SetPosition(0, this.transform.position);
                lr.SetPosition(1, originPosition);

                // Find the length of the line
                lineLength = (lr.GetPosition(1) - lr.GetPosition(0)).magnitude;

                // Find the angle between the pivot point, and the player's position
                angle = Mathf.Atan2(originPosition.y - this.transform.position.y, originPosition.x - this.transform.position.x);

                // Rotate the angle by 90 degrees
                angle -= (Mathf.PI / 2);

                StartCoroutine("ActLikeAPendulum");
            }else
            {
                lr.gameObject.SetActive(false);

                StopCoroutine("ActLikeAPendulum");

                _swinging = false;
            }
        }
    }

    IEnumerator ActLikeAPendulum()
    {
        // Should prevent the case of the previous X being the same as current X
        float previousX = this.transform.position.x+1f;

        _swinging = true;

        // Delay the effect slightly
        yield return new WaitForSeconds(0.15f);

        while (previousX != this.transform.position.x)
        {
            // Update the previousX to current pos X
            previousX = this.transform.position.x;

            { // Apply the angular acceleration to the pendulum

                //aAcc = -0.005f * Mathf.Sin(angle);

                aAcc = ((-massOfBob * (gravityEffect / lineLength)) * Mathf.Sin(angle));

                aVel += aAcc;
                aVel *= dampeningEffect;

                angle += aVel;
            }

            // Calculate the position of the pendulum over time
            this.transform.position = new Vector3((originPosition.x + lineLength * Mathf.Sin(angle)), (originPosition.y - lineLength * Mathf.Cos(angle)));

            // Draw the line between the pendulum bob and the origin point
            lr.SetPosition(0, this.transform.position);

            yield return new WaitForSeconds(0);
        }
    }
}