using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitOrbit : MonoBehaviour
{
    public Vector3 force;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(force);
    }
}
