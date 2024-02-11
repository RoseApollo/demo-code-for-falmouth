using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDraw;

public class Orbital : MonoBehaviour
{
    public static List<Orbital> orbitalObjects = new List<Orbital>();

    public float gravitationalStrength;

    public bool debugDots = false;

    private List<Vector3> posLog = new List<Vector3>();
    public float posLogTimeDif = 0f;
    private float posLogTime = 0f;
    public int posLogLen = 512;
    public Color debugColor = Color.white;
    public bool doDebugOffset = false;
    public Transform debugOffsetFrom;

    public Action<Vector3, ForceMode>? addForce = null;
    public float mass;

    public float lastGravityScore = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();

        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
            this.mass = rigidbody.mass;

            addForce = rigidbody.AddForce;
        }

        orbitalObjects.Add(this);
    }

    // OnDestroy is called before you 'get rekked'
    void OnDestroy()
    {
        orbitalObjects.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!debugDots)
        {
            if (posLog.Count > 0)
                posLog = new List<Vector3>();

            return;
        }

        posLogTime += Time.deltaTime;

        if (posLogTime >= posLogTimeDif)
        {
            posLogTime -= posLogTimeDif;

            posLog.Add((doDebugOffset) ? (transform.position - debugOffsetFrom.position) : transform.position);

            while (posLog.Count >= posLogLen)
            {
                posLog.RemoveAt(0);
            }
        }

        if (!DebugToggle.showDebug)
            return;

        Draw.World.SetColor(debugColor);

        for (int i = 0; i < (posLog.Count - 1); i++)
        {
            if (doDebugOffset)
            {
                Draw.World.Line(posLog[i] + debugOffsetFrom.position, posLog[i + 1] + debugOffsetFrom.position);
            }
            else
            {
                Draw.World.Line(posLog[i], posLog[i + 1]);
            }
        }
    }
}
