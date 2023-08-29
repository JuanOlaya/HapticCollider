using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] LayerMask layermask;
    
    RaycastHit hit;

    private bool collided = false;

    private Vector3 posCollision = Vector3.zero;
    private Quaternion rotationCollision = Quaternion.identity;
    public GameObject hammerNoid;
    private GameObject instatiatedHammer;
    private Vector3 lastPosition = Vector3.zero;
    private float hitDistance=10.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.up));
        if(Physics.Raycast (ray, out hit, 10f , layermask ,QueryTriggerInteraction.UseGlobal ))
        {
            Debug.Log("Hit Something: "+hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up)*hit.distance, Color.red);
            hitDistance = hit.distance;
        }
        else
        {
            //Debug.Log("Hit Nothing");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up)*10f, Color.red);
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
            if( /*hit.collider.tag=="hammerable" && */ hitDistance< 1.0f)
            {
                Debug.Log("Hit Distance"+hit.distance);
                collided = true;

                posCollision = gameObject.transform.position;
                rotationCollision = gameObject.transform.rotation;
                instatiatedHammer = Instantiate(hammerNoid, lastPosition /*posCollision*/, rotationCollision);

                gameObject.GetComponent<Rigidbody>().isKinematic = true;

                gameObject.GetComponent<Renderer>().enabled = false;

                Debug.Log("CurrentPosition: " + posCollision + " PreviousPosition: " + lastPosition);
            }
        }
        lastPosition = gameObject.transform.position;
    }
}
