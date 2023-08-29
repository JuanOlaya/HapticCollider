using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnCollision : MonoBehaviour
{
    public GameObject fracturedBottle;
    void OnTriggerEnter(Collider collision)
    {
        GameObject.Instantiate(fracturedBottle,transform.position,transform.rotation);
        Destroy(gameObject);
        Debug.Log("****** Bottle collision detected ******");
    }
}
