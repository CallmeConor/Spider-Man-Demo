using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    LineRenderer lr;

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
    public Vector3 ropeEnd;

    // Init
    private void Start()
    {
        angle = 0;
        lr = transform.GetChild(0).GetComponent<LineRenderer>();

        originPosition = this.transform.position;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            // Check mouse position when clicked for a "ropeable" surface
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (MouseRayCastTarget.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                //Debug.Log("Act like Spider-Man, woo");
                originPosition =  hitInfo.transform.gameObject.transform.position;

                lr.SetPosition(0, this.transform.position);
                lr.SetPosition(1, originPosition);

                lineLength = (lr.GetPosition(1) - lr.GetPosition(0)).magnitude;

                // Find the angle between the pivot point, and the player's position - degrees
                angle = Vector3.Angle(-hitInfo.transform.gameObject.transform.up, this.transform.position);

                // Change the angle to radians
                angle *= Mathf.Deg2Rad;

                // If the player is the other side of the pivot, flip the angle
                if (lr.GetPosition(0).x < lr.GetPosition(1).x)
                    angle *= -1;

                StartCoroutine("ActLikeAPendulum");
            }
        }
    }

    IEnumerator ActLikeAPendulum()
    {
        // Should prevent the case of the previous X being the same as current X
        float previousX = this.transform.position.x+1f;

        // Delay the effect slightly
        yield return new WaitForSeconds(0.15f);

        while (previousX != this.transform.position.x)
        {
            // Update the previousX to current pos X
            previousX = this.transform.position.x;

            { // Apply the angular acceleration to the pendulum

                aAcc = ((-massOfBob * (gravityEffect / lineLength)) * Mathf.Sin(angle));
                //aAcc = -0.01f * Mathf.Sin(angle);

                aVel += aAcc;
                aVel *= dampeningEffect;

                angle += aVel;
            }

            // Calculate the position of the pendulum over time
            this.transform.position = new Vector2((originPosition.x + lineLength * Mathf.Sin(angle)), (originPosition.y - lineLength * Mathf.Cos(angle)));

            // Draw the line between the pendulum bob and the origin point
            lr.SetPosition(0, this.transform.position);
            lr.SetPosition(1, originPosition);

            yield return new WaitForSeconds(0);
        }
    }
}
