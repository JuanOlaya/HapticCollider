using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

/// 
/// <summary>
/// R   Restart counter and target (Nail/Glass/Block)    
/// C   Calibrate height target (Nail/Glass/Block)
/// E   End condition - Save File
/// I   Pseudo haptic - Visual Illusion ON/OFF
/// 
/// 1   HammerController_Nail_HN
/// 2   HammerController_Glass_HG
/// 3   ViveController_Nail_VN
/// 4   ViveController_Glass_VG
/// 
/// A   Servo: Impact
/// Z   Servo: Idle 
/// </summary>
/// 

public class BoxCasterIllusion : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 position;
    private Quaternion rotation;
    public float currentHitDistance;
    public float currentHitDistance2;
    public float maxDistance = 5.0f;
    public bool isApproachingTheTarget;
    private Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField] LayerMask layerMask;

    //public float hitCooldown = 0.3f;
    //public float hitCooldown2 = 0.3f;

    public float offsetRange;

    private float lastHitTime;
    private bool justDidHit;

    public Vector3 posCollision = Vector3.zero;
    private Quaternion rotationCollision = Quaternion.identity;
    private bool collided = false;
    private bool isOut = true;
    private bool isOutVisualShield = true;
    public bool predictionCollided = false;
    public bool predictionCollided2 = false;

    public SerialController serialController;

    public GameObject hammerNoid;
    public GameObject prefabSpark;

    private GameObject instatiatedHammer;

    private float speed = 0.0f;
    //private Vector3 currentposition = Vector3.zero;

    private Vector3 lastPosition = Vector3.zero;

    public float boxcastSpeed = 0.0f;
    //private float currentDistance = 0.0f;
    private float lastHitDistance = 0.0f;

    public float predictionDistance = 0.60f;
    public float thresholdSpeed ;
    private bool hitSomething = false;

    private Transform hitTransform;
    private Renderer hitRender;
    //private float offsetDistance = 0.04f;
    public StudyController studyController;
    public Transform glassTransform;
    public BoxCollider glassBoxCollider;
    public Renderer shieldRenderer; 
    //public int counterHammer = 0;

    [SerializeField] private GameObject _glassHolePrefab;

    public float heightHammer;
    private RaycastHit hitInfo;
    private bool illusionON = true;
    private bool hasImpacted=false;
    private float counterAfterImpact = 100;
    private Vector3 hammerImpactLocation;
    private float magnitudAfterImpact;
    private Vector3 lastDistanceImpactVSCurrent;
   
    public Text textUIcounter;
    public Text textUIcounterColumn;

    public GameObject NailGlassBlock;
    public bool isReadyToHit = true;
    public bool hasStarted = false;
    //private InputDevice XRController;

    public Transform NailPoint;
    public Transform GlassPoint;
    public Vector3 lastHammerTipPoint;

    private bool visualIllusion=true;

    void Start()
    {
        studyController = GameObject.Find("StudyManager").GetComponent<StudyController>();
        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
        //glassTransform = GameObject.Find("Glass").GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody>();
        //glassBoxCollider = GameObject.Find("Glass").GetComponent<BoxCollider>();
        //shieldRenderer = GameObject.Find("Shield").GetComponent<Renderer>();
        //XRController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    void Update()
    {
        //studyController.hammerBlowCounter = -3;
        //studyController.hammerBlowCounter = counterHammer;

        Debug.Log("HammerTipPoint: " + transform.GetChild(1).transform.position + "  " + transform.GetChild(1).name );

        if (!hasStarted)
        {
            serialController.SendSerialMessage("A");
            Invoke("resetServo", 0.070f);
            hasStarted = true;
        }

        /*
        if (shieldRenderer != null)
        {
            if (predictionCollided )
            {
                shieldRenderer.material.color = new Color(1.0f, 0.64f, 0.0f, 0.05f);
            }
            else
            {
            shieldRenderer.material.color = new Color(1.0f, 0.41f, 0.70f, 0.05f);
            }
        }
        */
        
        //}
        if (Input.GetKeyDown(KeyCode.I))
        {
           illusionON = !illusionON; 
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            //serialController.SendSerialMessage("A");
            Invoke("hitServo2", 5.00f);
        }

        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            serialController.SendSerialMessage("Z");
        }
        

        Debug.Log("studyController.studySetup.conditionType: " + studyController.studySetup.conditionType.ToString() );
        textUIcounter.text = "Condition: " + studyController.studySetup.conditionType.ToString() /*+ " hasImpacted: " + hasImpacted*/;
        textUIcounterColumn.text = "Impact #: " + studyController.hammerImpactCounter.ToString();
    }

    void hitServo2()
    {
        if(isReadyToHit)
        {
            serialController.SendSerialMessage("A");
            Invoke("resetServo", 0.170f);
            //Invoke("readyToHit", 2.000f);
        }
    }

    void FixedUpdate()
    {
        //transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
        direction = transform.TransformDirection(Vector3.forward);
        position = transform.position;
        rotation = transform.rotation;
        
        RaycastHit hit;

        if (Physics.BoxCast(position, /*halfExtents: transform.lossyScale/2*/  halfExtents: transform.lossyScale/2 , direction, out hit, rotation, maxDistance, layerMask))
        {
            currentHitDistance = hit.distance;
            
            boxcastSpeed = lastHitDistance - currentHitDistance;
            if (boxcastSpeed>0) {
                isApproachingTheTarget = true;
            }
            else {
                isApproachingTheTarget = false;
            }
            lastHitDistance = hit.distance;
            hitSomething = true;

            hitTransform = hit.collider.gameObject.GetComponent<Transform>();
            hitRender = hit.collider.gameObject.GetComponent<Renderer>();
        }
        else
        {
            currentHitDistance = maxDistance;
            hitSomething = false;
        }

        
        Vector3 position2 = new Vector3(position.x, position.y+heightHammer, position.z); // This Vector is a bit higher to impact the Glass
        if(Physics.Raycast(origin: position2, direction: direction, out hitInfo))
        {
            //Debug.DrawLine(position2, hitInfo.point);
            if(predictionDistance<hitInfo.distance)
            {
                Debug.DrawRay(position2, direction, Color.red);
            }
            else
            {
                //Debug.DrawRay(position2, direction, Color.red);
            }
            currentHitDistance2 = hitInfo.distance;
        }
        else
        {
            currentHitDistance2 = maxDistance;
        }

        if (currentHitDistance2 < 0.02f)
        {
            if (!predictionCollided2 && (speed >= thresholdSpeed) /*&& isApproachingTheTarget*/)
            {
                //if (hitTransform.gameObject.name == "Glass")
                //{
                    //GameObject obj = Instantiate(_glassHolePrefab, new Vector3(hitInfo.point.x, hitInfo.point.y -0.012f, 0.28368f), Quaternion.LookRotation(hit.normal));
                    //Debug.Log("hitInfo: " + hitInfo.point.x);
                    //Debug.Log("glassTransform.localScale.x: " + glassTransform.localScale.x);

                    /*
                    float frameThreshold = 0.008f;
                    if (hitInfo.point.x > glassTransform.position.x - glassTransform.localScale.x/2 +frameThreshold &&  hitInfo.point.x < glassTransform.position.x + glassTransform.localScale.x/2 -frameThreshold) {
                      
                        if (hitInfo.point.y > glassTransform.position.y - glassTransform.localScale.y/2 +frameThreshold && hitInfo.point.y < glassTransform.position.y + glassTransform.localScale.y/2 -frameThreshold)
                        {
                            //Debug.Log("Fissure");
                            GameObject obj = Instantiate(_glassHolePrefab, new Vector3(hitInfo.point.x, hitInfo.point.y -0.024f, 0.26005f), Quaternion.LookRotation(hit.normal));
                        }
                    }
                    */
                //}
            }
            predictionCollided2 = true;
            hasImpacted = true;
            counterAfterImpact = 100;
            hammerImpactLocation = transform.position;
        }

        /*
        if (currentHitDistance < 0.15f && hitTransform.gameObject.name == "Glass")
        {
            GameObject obj = Instantiate(_glassHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
        }
        */

        /***        PREDICTION          ***/
        if (currentHitDistance < (predictionDistance))
        {
            if (!predictionCollided && (speed >= thresholdSpeed) && isApproachingTheTarget)
            {
                //Debug.Log("Velocity: "+speed);
                if(studyController.hammerImpactCounter< 6  && isReadyToHit)
                {
                    /*isReadyToHit = false*/;
                    predictionCollided = true;

                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Nail_HN || studyController.studySetup.conditionType == Bedingung.HammerController_Glass_HG || studyController.studySetup.conditionType == Bedingung.Training)
                    {
                        serialController.SendSerialMessage("A");
                        Invoke("resetServo", 0.170f);
                        Invoke("readyToHit", 1.000f);
                    }

                    if (hitTransform.gameObject.name == "Nail")
                    { 
                        hitTransform.Translate(0f, 0f, 0.015f);
                        //hitRender.material.color = Color.red;
                    }
                    studyController.hammerImpactCounter++;
    
                    if (hitTransform.gameObject.name == "Glass")
                    {
                        
                        //GameObject obj = Instantiate(_glassHolePrefab, new Vector3(hitInfo.point.x, hitInfo.point.y -0.012f, 0.28368f), Quaternion.LookRotation(hit.normal));
                        //Debug.Log("hitInfo: " + hitInfo.point.x);
                        //Debug.Log("glassTransform.localScale.x: " + glassTransform.localScale.x);

                        float frameThreshold = 0.008f;
                        if (hit.point.x > glassTransform.position.x - glassTransform.localScale.x/2 +frameThreshold &&  hit.point.x < glassTransform.position.x + glassTransform.localScale.x/2 -frameThreshold) {
                            //Debug.Log("ENTRAIII: ");
                            if (hit.point.y > glassTransform.position.y - glassTransform.localScale.y/2 +frameThreshold && hit.point.y < glassTransform.position.y + glassTransform.localScale.y/2 -frameThreshold)
                            {
                                //Debug.Log("Fissure");
                                GameObject obj = Instantiate(_glassHolePrefab, new Vector3(hit.point.x, hit.point.y -0.024f, 0.26005f), Quaternion.LookRotation(hit.normal));
                            }
                        }
                    }
                }
                
                /*
                if (hitTransform.gameObject.name == "Glass")
                {
                    GameObject obj = Instantiate(_glassHolePrefab, hitInfo.point, Quaternion.LookRotation(hit.normal));
                }
                */
                //counterHammer++;

                //Debug.Log("HOLA: "+ counterHammer);
                if (studyController.hammerImpactCounter==1)
                {
                    studyController.hammerImpactLocation1 = hit.point;
                    studyController.hammerImpactSpeed1 = boxcastSpeed;

                    if(studyController.studySetup.conditionType == Bedingung.HammerController_Nail_HN || studyController.studySetup.conditionType == Bedingung.ViveController_Nail_VN)
                    {
                        studyController.targetPosition1 = NailPoint.position;
                    }
                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Glass_HG || studyController.studySetup.conditionType == Bedingung.ViveController_Glass_VG)
                    {
                        studyController.targetPosition1 =GlassPoint.position;
                    }

                }

                if (studyController.hammerImpactCounter == 2)
                {
                    studyController.hammerImpactLocation2 = hit.point;
                    studyController.hammerImpactSpeed2 = boxcastSpeed;

                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Nail_HN || studyController.studySetup.conditionType == Bedingung.ViveController_Nail_VN)
                    {
                        studyController.targetPosition2 = NailPoint.position;
                    }
                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Glass_HG || studyController.studySetup.conditionType == Bedingung.ViveController_Glass_VG)
                    {
                        studyController.targetPosition2 = GlassPoint.position;
                    }
                }

                if (studyController.hammerImpactCounter == 3)
                {
                    studyController.hammerImpactLocation3 = hit.point;
                    studyController.hammerImpactSpeed3 = boxcastSpeed;

                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Nail_HN || studyController.studySetup.conditionType == Bedingung.ViveController_Nail_VN)
                    {
                        studyController.targetPosition3 = NailPoint.position;
                    }
                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Glass_HG || studyController.studySetup.conditionType == Bedingung.ViveController_Glass_VG)
                    {
                        studyController.targetPosition3 = GlassPoint.position;
                    }
                }

                if (studyController.hammerImpactCounter == 4)
                {
                    studyController.hammerImpactLocation4 = hit.point;
                    studyController.hammerImpactSpeed4 = boxcastSpeed;

                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Nail_HN || studyController.studySetup.conditionType == Bedingung.ViveController_Nail_VN)
                    {
                        studyController.targetPosition4 = NailPoint.position;
                    }
                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Glass_HG || studyController.studySetup.conditionType == Bedingung.ViveController_Glass_VG)
                    {
                        studyController.targetPosition4 = GlassPoint.position;
                    }
                }

                if (studyController.hammerImpactCounter == 5)
                {
                    studyController.hammerImpactLocation5 = hit.point;
                    studyController.hammerImpactSpeed5 = boxcastSpeed;

                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Nail_HN || studyController.studySetup.conditionType == Bedingung.ViveController_Nail_VN)
                    {
                        studyController.targetPosition5 = NailPoint.position;
                    }
                    if (studyController.studySetup.conditionType == Bedingung.HammerController_Glass_HG || studyController.studySetup.conditionType == Bedingung.ViveController_Glass_VG)
                    {
                        studyController.targetPosition5 = GlassPoint.position;
                    }
                }


                if (studyController.studySetup.conditionType == Bedingung.ViveController_Glass_VG || studyController.studySetup.conditionType == Bedingung.ViveController_Nail_VN || studyController.studySetup.conditionType == Bedingung.Training)
                {
                    InputDevice XRController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                    if (XRController.isValid)
                    {
                        HapticCapabilities hapcap = new HapticCapabilities();
                        XRController.TryGetHapticCapabilities(out hapcap);

                        
                        Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC BUFFER: " + hapcap.supportsBuffer);
                        Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC IMPULSE: " + hapcap.supportsImpulse);
                        Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC CHANNELS: " + hapcap.numChannels);
                        Debug.Log("DOES XR CONTROLLER SUPPORT HAPTIC BUFFER FREQUENCY HZ: " + hapcap.bufferFrequencyHz);
                        
                        if (hapcap.supportsImpulse)
                        {
                            XRController.SendHapticImpulse(0u, 1.0f, 3.0f);
                            //XRController.SendHapticImpulse(0.6f , 0.5f);
                        }
                    }
                }
            }
        }
        else
        {
        //if (currentHitDistance >= 0.2)
        
            if (isOut &&  predictionCollided /*&& (posCollision.z > gameObject.transform.position.z) */ && !collided)  // puede ser if(collided == false)
            {
                predictionCollided = false;
                predictionCollided2 = false;
                //collided = false;

                hasImpacted = false;
            }
        }
        //}

        /***        VISUAL ILLUSION         ***/
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            visualIllusion=!visualIllusion;
        }
        Debug.Log("visualIllusion: "+visualIllusion);
        if(visualIllusion)
        {

            if (currentHitDistance < (0.05f /*+ offsetDistance*/))
            {
                if(!collided && isApproachingTheTarget && illusionON &&  studyController.hammerImpactCounter < 6)
                {
                    if( speed >= thresholdSpeed )
                    {
                        posCollision = gameObject.transform.position;
                        Instantiate(prefabSpark, posCollision, Quaternion.identity);
                    }
                    rotationCollision = gameObject.transform.rotation;
                    instatiatedHammer = Instantiate(hammerNoid, lastPosition, rotation);
                    //StartCoroutine(ShakeHammer(instatiatedHammer));
                    collided = true;
                    //gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    gameObject.GetComponent<Renderer>().enabled = false;
                }
            }
            else
            {
                if( isOutVisualShield && collided /*&& (posCollision.z > gameObject.transform.position.z)*/)
                {
                    //gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    gameObject.GetComponent<Renderer>().enabled = true;
                    Destroy(instatiatedHammer);
                    collided = false;   // consider to move up 
                    
                    if (studyController.hammerImpactCounter == 1)
                    {
                        studyController.distanceAfterImpact1 = magnitudAfterImpact;
                    }
                    if (studyController.hammerImpactCounter == 2)
                    {
                        studyController.distanceAfterImpact2 = magnitudAfterImpact;
                    }
                    if (studyController.hammerImpactCounter == 3)
                    {
                        studyController.distanceAfterImpact3 = magnitudAfterImpact;
                    }
                    if (studyController.hammerImpactCounter == 4)
                    {
                        studyController.distanceAfterImpact4 = magnitudAfterImpact;
                    }
                    if (studyController.hammerImpactCounter == 5)
                    {
                        studyController.distanceAfterImpact5 = magnitudAfterImpact;
                        Invoke("DeactivateNailGlassBlock", 0.250f);
                    }
                }
            }

        }

       

        /***  DISTANCE AFTER IMPACT  ***/
        if (hasImpacted)
        {

            if (transform.position.z <  lastPosition.z)
            //if (transform.GetChild(1).transform.position.z < lastHammerTipPoint.z)
            {
                Debug.Log("DistanceAfterIm");
                //Debug.Log("counterAfterImpact:  "+ counterAfterImpact+ "> transform.position.z: " + transform.position.z);
                counterAfterImpact = transform.position.z;
                //Debug.Log("counterAfterImpact: " + counterAfterImpact);
                magnitudAfterImpact =  Vector3.Distance(transform.position, hammerImpactLocation);
                //Debug.Log("magnitudAfterImpact: " + magnitudAfterImpact);
            }
            else
            {
                /*
                if (studyController.hammerImpactCounter == 1)
                {
                    studyController.distanceAfterImpact1 = magnitudAfterImpact;
                }
                if (studyController.hammerImpactCounter == 2)
                {
                    studyController.distanceAfterImpact2 = magnitudAfterImpact;
                }
                if (studyController.hammerImpactCounter == 3)
                {
                    studyController.distanceAfterImpact3 = magnitudAfterImpact;
                }
                if (studyController.hammerImpactCounter == 4)
                {
                    studyController.distanceAfterImpact4 = magnitudAfterImpact;
                }
                if (studyController.hammerImpactCounter == 5)
                {
                    studyController.distanceAfterImpact5 = magnitudAfterImpact;
                }
                */
            }
        }

        speed = (transform.position - lastPosition).magnitude*1000;
        lastPosition = transform.position;
        lastHammerTipPoint = transform.GetChild(1).transform.position;
        //lastDistanceImpactVSCurrent = Vector3.Distance( transform.position, hammerImpactLocation);
        //Debug.Log("Speed: "+speed);
        //Debug.Log("Position: "+transform.position);
    }

    void DeactivateNailGlassBlock()
    {
        NailGlassBlock.SetActive(false);
        GameObject[] glassHole = GameObject.FindGameObjectsWithTag("GlassHole");
        //Debug.Log("GlassHole.Length: " + glassHole.Length);
        foreach (GameObject go in glassHole)
        {
            Destroy(go);
        }

        if (studyController.studySetup.conditionType != Bedingung.Training)
        {
            studyController.endCondition();
        }
    }

    void resetServo()
    {
        serialController.SendSerialMessage("Z");
    }

    void readyToHit()
    {
        isReadyToHit = true;
    }

    /*
    void moveBackNail() {
        hitTransform.Translate(0f, 0f, -0.03f);
    }
    */

    void OnDrawGizmos()
    {
        /*
        if (hitSomething)
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.6f);
        }
        else
        {
            Gizmos.color = new Color(0.0f, 0.0f, 1.0f, 0.6f);
        }
        */
        
        if (predictionDistance < hitInfo.distance)
        {
            //Debug.DrawRay(position2, direction, Color.green);
            Gizmos.color = new Color(0.67f, 1.0f, 0.18f, 0.7f);
        }
        else
        {
            //Debug.DrawRay(position2, direction, Color.red);
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.7f);
        }

        Gizmos.DrawRay(from: transform.position, direction: direction * currentHitDistance);
        Gizmos.DrawCube(center: position + direction * currentHitDistance, size: transform.lossyScale*1.35f );
    }

    void OnTriggerEnter(Collider otherColl)
    {
        var rend = otherColl.gameObject.GetComponent<Renderer>();
        if (rend != null && (speed >= thresholdSpeed) )
        {
            //rend.material.color = Color.red;
        }

        if (otherColl.gameObject.name == "Shield") {
            isOut = false;
            //Debug.Log("Collision Shield IN");
        }

        if (otherColl.gameObject.name == "VisualShield")
        {
            isOutVisualShield = false;
        }

        if (otherColl.gameObject.name == "Nail")
        {
            //Debug.Log("Hola");
        }

        if (otherColl.gameObject.name == "NailCube2" || otherColl.gameObject.name == "NailCube3" ||  otherColl.gameObject.name == "NailCube4")
        {

            //otherColl.gameObject.transform.Translate(0f, 0f, -0.03f);
            //hitTransform = otherColl.gameObject.GetComponent<Transform>();
        }
    }

    void OnTriggerExit(Collider otherColl)
    {
        if (otherColl.gameObject.name == "Shield")
        {
            isOut = true;
            //Debug.Log("Collision Shield OUT");
        }

        if (otherColl.gameObject.name == "VisualShield")
        {
            isOutVisualShield = true;
            //hasImpacted = false;
        }

        var rend = otherColl.gameObject.GetComponent<Renderer>();
        if (rend != null)
        {
            //rend.material.color = Color.blue;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        print("Points colliding: " + other.contacts.Length);
        print("First point that collided: " + other.contacts[0].point);
    }

    /// <summary>
    /// Example for feedback for the hammer proxy.
    /// </summary>
    /// <param name="hammerProxy">Object to animate.</param>
    /// <returns></returns>
    IEnumerator ShakeHammer(GameObject hammerProxy)
    {
        Vector3 basePos = hammerProxy.transform.position;
        var startTime = Time.time;

        while(true)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange, offsetRange), Random.Range(-offsetRange, offsetRange));
            hammerProxy.transform.position = basePos + randomOffset;

            yield return null;

            if (Time.time - startTime > 0.1f)
            {
                break;
            }
        }
    }
}
