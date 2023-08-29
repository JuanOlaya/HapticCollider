using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIllusionRay : MonoBehaviour
{
    private bool isColliding = false;
    // Start is called before the first frame update
    public SerialController serialController;
    private Rigidbody rb;
    public float relativeVelocity = 0;
    public float rigidbodySpeed = 0;
    public float impulseCol = 0;
    public float otherCollSpeedThreshold = 0.0f;

    public float hitCooldown = 0.3f;
    public float hitCooldown2 = 0.3f;

    private float lastHitTime;
    private bool justDidHit;

    public float speed = 0;
    public Vector3 position = Vector3.zero;
    //Vector3 lastPosition = Vector3.zero;

    // Raycast //
    [SerializeField] LayerMask layermask;

    RaycastHit hit;

    private bool collided = false;

    private Vector3 posCollision = Vector3.zero;
    private Quaternion rotationCollision = Quaternion.identity;
    public GameObject hammerNoid;
    private GameObject instatiatedHammer;
    private Vector3 lastPosition = Vector3.zero;
    private float hitDistance = 10.0f;


    // Raycast Sphere//
    public float sphereRadius;
    public float maxDistance;

    private Vector3 origin;
    private Vector3 direction;

    private float currentHitDistance;
    // ~Raycast Sphere~ //

    void Start()
    {
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        rb = gameObject.GetComponent<Rigidbody>();

        // Set lastHitTime once at the start
        lastHitTime = Time.time;
    }

    // Update is called once per frame
    //void Update()
    //{
        //rigidbodySpeed = rb.velocity.magnitude;   
    //}

    /*
    void FixedUpdate()
    {
        position = transform.position;
        speed = (transform.position - lastPosition).magnitude * 100;
        lastPosition = transform.position;
    }
    */

    // Update is called once per frame
    //void FixedUpdate()
    void Update()
    {
        rigidbodySpeed = rb.velocity.magnitude;

        origin = transform.position;
        direction = transform.TransformDirection(Vector3.forward);

        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.forward));

        //if (Physics.Raycast(ray, out hit, 10f, layermask, QueryTriggerInteraction.UseGlobal))
        if(Physics.SphereCast(origin, sphereRadius, direction , out hit, maxDistance, layermask, QueryTriggerInteraction.UseGlobal)) 
        {
            Debug.Log("Hit Something: " + hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            hitDistance = hit.distance;
            Debug.Log("Hit Distance" + hit.distance);
        }
        else
        {
            //Debug.Log("Hit Nothing");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10f, Color.red);
            hitDistance = 10.0f;
        }

        if (collided)
        {
            if (gameObject.transform.position.x > posCollision.x)
            {
                Destroy(instatiatedHammer);

                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                gameObject.GetComponent<Renderer>().enabled = true;
                collided = false;
            }
        }

        if (!collided)
        {
            if (  hitDistance < 0.1f)
            {
                Debug.Log("Hit Distance" + hit.distance);
                collided = true;

                posCollision = gameObject.transform.position;
                rotationCollision = gameObject.transform.rotation;
                instatiatedHammer = Instantiate(hammerNoid, lastPosition , rotationCollision);

                gameObject.GetComponent<Rigidbody>().isKinematic = true;

                gameObject.GetComponent<Renderer>().enabled = false;

                Debug.Log("CurrentPosition: " + posCollision + " PreviousPosition: " + lastPosition);
            }
        }
        
        lastPosition = gameObject.transform.position;
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
        Debug.Log("trigger enter");

        // Check if the cooldown condition is matched and hammering should be possible or not
        if (Time.time - lastHitTime < hitCooldown) return;

        //impulseCol = otherColl.impulse.magnitude;
        //Debug.Log("Get in contact with: " + otherColl.transform.name);
        if (otherColl.gameObject.tag == "Hammerable")
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

        if (otherColl.gameObject.tag == "goingInside")
        {
            otherColl.gameObject.transform.Translate(0f, 0f, -0.008f);
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

        /* CollisionIllusionRay */
        /*
        Destroy(instatiatedHammer);

        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Renderer>().enabled = true;
        collided = false;
        */
        /* ~CollisionIllusionRay~ */
    }

    void resetServo()
    {
        serialController.SendSerialMessage("Z");
    }
}
