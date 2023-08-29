using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nailCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Relative Veloctiy: " + collision.relativeVelocity.magnitude);
        
        /*
        Debug.Log("Collision");
        if (collision.gameObject.name=="hammer"){
            Debug.Log("Collision Detected");
        }
        */
    }
    
}
