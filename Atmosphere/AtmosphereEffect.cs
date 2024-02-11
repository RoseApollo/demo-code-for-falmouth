using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AtmosphereEffect : MonoBehaviour
{
    public static List<AtmosphereEffect> atmosphereEffects = new List<AtmosphereEffect>();

    public Rigidbody rb;
    public Orbital orb;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        orb = gameObject.GetComponent<Orbital>();

        lock (atmosphereEffects)
        {
            atmosphereEffects.Add(this);
        }
    }

    void OnDestroy()
    {
        lock (atmosphereEffects)
        {
            atmosphereEffects.Remove(this);
        }
    }
}
