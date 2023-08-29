using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{

    private Vector3 posCollision = Vector3.zero;
    private Quaternion rotationCollision = Quaternion.identity;
    public GameObject hammerNoid;
    private GameObject instatiatedHammer;
    private bool enter = false;
    public Vector3 lastPosition = Vector3.zero;


    //void Update()
    void LateUpdate()
    {
        lastPosition = gameObject.transform.position;
    }
    void Update()
    {
        if(enter)
        {
            if(gameObject.transform.position.x > posCollision.x)
            {
                Destroy(instatiatedHammer);

                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                gameObject.GetComponent<Renderer>().enabled = true;
                enter = false;
            }
        }
        
    }
    void OnCollisionEnter(Collision otherColllider)
    {
        if(!enter)
        {
            enter = true;

            posCollision = gameObject.transform.position;
            rotationCollision = gameObject.transform.rotation;
            instatiatedHammer = Instantiate(hammerNoid, lastPosition, rotationCollision);

            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            gameObject.GetComponent<Renderer>().enabled = false;

            Debug.Log("CurrentPosition: "+posCollision+" PreviousPosition: "+ lastPosition);
        }

    }
}
