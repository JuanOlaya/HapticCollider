using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stop : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody m_Rigidbody;
    public float m_Thrust = 20f;
    public GameObject ball;

    void Start()
    {
         m_Rigidbody = GetComponent<Rigidbody>();
         m_Rigidbody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            transform.position = new Vector3(-0.0377159417f,1.05200005f,0.93599999f);
        }
    }

    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");
        if (other.gameObject.CompareTag ("BaseballBat")){
            Debug.Log("Collider");
            m_Rigidbody.useGravity = true;
            m_Rigidbody.AddForce(transform.right * m_Thrust);
            Invoke("newBall",1.5f);
        }
    }
    
    void newBall()
    {
        Instantiate(ball, new Vector3(-0.0377159417f,1.05200005f,0.93599999f), Quaternion.identity);
    }

}


//Vector3(0.274000049,1.05200005,2.11800003)