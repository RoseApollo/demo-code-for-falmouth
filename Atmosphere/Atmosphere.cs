using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    public static List<Atmosphere> atmospheres = new List<Atmosphere>();

    public float seaLevel;

    public float seaLevelPressure = 0.000000000003f; // moon
    public float temperature = 280f; // 10 celcius
    public float molarMass = 28.9647f; // air
    public float relativeHumidity = 0f; // percantage: 0 - 1
    public float dewPoint = 0f; // https://www.omnicalculator.com/physics/dew-point

    [HideInInspector]
    public Rigidbody rigidbody;

    [HideInInspector]
    public AtmosphereEffect? atmosphereEffect = null;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();

        lock (atmospheres)
        {
            atmospheres.Add(this);
        }

        atmosphereEffect = gameObject.GetComponent<AtmosphereEffect>();

        if (atmosphereEffect == null)
            atmosphereEffect = gameObject.AddComponent<AtmosphereEffect>();
    }

    void OnDestroy()
    {
        lock (atmospheres)
        {
            atmospheres.Remove(this);
        }
    }
}
