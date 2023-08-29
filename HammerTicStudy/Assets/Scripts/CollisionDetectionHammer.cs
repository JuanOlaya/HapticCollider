using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectionHammer : MonoBehaviour
{
    private bool isColliding=false;
    // Start is called before the first frame update
    public SerialController serialController;
    private Rigidbody rb;
    public float relativeVelocity=0;
    public float rigidbodySpeed=0;
    public float impulseCol = 0;
    public float otherCollSpeedThreshold = 0.0f;

    public float hitCooldown = 0.3f;
    public float hitCooldown2 = 0.3f;

    private float lastHitTime;
    private bool justDidHit;

    public float speed = 0;
    public Vector3 position = Vector3.zero;
    Vector3 lastPosition = Vector3.zero;

    public StudyController studyController; 

    void Start()
    {
        studyController = GameObject.Find("StudyManager").GetComponent<StudyController>();
        rb = gameObject.GetComponent<Rigidbody>();

        // Set lastHitTime once at the start
        lastHitTime = Time.time;
        //studyController = GameObject.Find("StudyManager").GetComponent < "StudyController" > ();
    }

    // Update is called once per frame
    void Update()
    {
        rigidbodySpeed = rb.velocity.magnitude;
        studyController.hammerImpactCounter=-1;
        Debug.Log("studyController.hammerBlowCounter: " + studyController.hammerImpactCounter);
        Debug.Log("Hola " );
    }
   
    void FixedUpdate()
    {
        position = transform.position;
        speed = (transform.position - lastPosition).magnitude*100;
        lastPosition = transform.position;
    }

    void OnGUI()
    {
        /*
        GUIStyle style = new GUIStyle();
        style.font.material.color = Color.red;
        GUI.Label(new Rect(10, 10, 100, 20), "Speedy: " + rigidbodySpeed, style);
        */
    }

    void OnTriggerEnter(Collider otherColl)
    {
        //studyController.hammerBlowCounter++;
        //Debug.Log("trigger enter");

        // Check if the cooldown condition is matched and hammering should be possible or not
        if (Time.time - lastHitTime < hitCooldown) return;

        //impulseCol = otherColl.impulse.magnitude;
        //Debug.Log("Get in contact with: " + otherColl.transform.name);
        if(otherColl.gameObject.tag == "Hammerable")
        {
            //if (speed> otherCollSpeedThreshold)
            //{
            justDidHit = true;

            lastHitTime = Time.time;
                serialController.SendSerialMessage("A");

            var rend = otherColl.gameObject.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.red;
            }
            //}
        }
        //otherColl.gameObject.GetComponent<Renderer>().material.color = new Color(56,61,214);

        //relativeVelocity = otherColl.relativeVelocity.magnitude;
        //Debug.Log("Relative Veloctiy: "+gameObject.relativeVelocity.magnitude);

        if (otherColl.gameObject.name == "NailCube2"){
            otherColl.gameObject.transform.Translate(0f, 0f, -0.01f);
        }
        /*
        if (otherColl.gameObject.name == "hammer")
        {
            Debug.Log("otherColl Detected");
        }
        */
        Invoke("resetServo", 0.25f);
    }

   

    void OnTriggerExit(Collider otherColl)
    {
        // Check if a hit occured earlier, as it could have been blocked by cooldown handling
        if (!justDidHit)
        {
            return;
        }

        justDidHit = false;

        //serialController.SendSerialMessage("Z");

        var rend = otherColl.gameObject.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.blue;
        }
    }

    void resetServo()
    {
        serialController.SendSerialMessage("Z");
    }
}
