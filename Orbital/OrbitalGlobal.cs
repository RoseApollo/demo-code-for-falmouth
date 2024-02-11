using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalGlobal : MonoBehaviour
{
    // shaderOrb shit pls god kill me plz

    public ComputeShader compOrbCalc;
    public ComputeShader compVecAdd;

    private int? shaderOrb;
    private int? shaderAdd;

    private void Start()
    {
        shaderOrb = compOrbCalc.FindKernel("CSMain");
        shaderAdd = compVecAdd.FindKernel("CSMain");
    }

    private void FixedUpdate()
    {
        if (shaderOrb == null)
            throw new System.Exception("FUCK ME SHADER SHOULD EXIS, FUCK YOU");

        if (Orbital.orbitalObjects.Count <= 0)
            return;

        int shaderObs = Orbital.orbitalObjects.Count;

        List<GravOpDat> data = new List<GravOpDat>();

        foreach (Orbital orbA in Orbital.orbitalObjects)
        {
            GravOpDat me = new GravOpDat();

            me.objectA = orbA.transform.position;

            me.massA = orbA.mass;

            me.strength = orbA.gravitationalStrength;

            data.Add(me);
        }

        ComputeBuffer buffInput, buffResult, buffAddResult;

        int tot = ((shaderObs * shaderObs) - shaderObs);

        buffInput = new ComputeBuffer(shaderObs, (3 * sizeof(float)) + (2 * sizeof(float)));
        buffResult = new ComputeBuffer(tot, sizeof(float) * 3);

        buffAddResult = new ComputeBuffer(shaderObs * shaderObs, 3 * sizeof(float));

        // set vals

        buffInput.SetData<GravOpDat>(data);

        compOrbCalc.SetBuffer((int)shaderOrb, "Result", buffResult);
        compOrbCalc.SetBuffer((int)shaderOrb, "Inputs", buffInput);

        compOrbCalc.SetFloat("DeltaTime", Time.deltaTime);
        compOrbCalc.SetInt("InputLen", shaderObs);

        compOrbCalc.Dispatch((int)shaderOrb, shaderObs, shaderObs, 1);
        
        compVecAdd.SetBuffer((int)shaderAdd, "values", buffResult);
        compVecAdd.SetBuffer((int)shaderAdd, "result", buffAddResult);
        compVecAdd.SetInt("num", shaderObs);

        compVecAdd.Dispatch((int)shaderAdd, shaderObs * shaderObs, 1, 1);

        Vector3[] addResult = new Vector3[shaderObs * shaderObs];
        buffAddResult.GetData(addResult);

        for (int i = 0; i < shaderObs; i++)
        {
            Orbital.orbitalObjects[i].addForce(addResult[i], ForceMode.Force);
            Orbital.orbitalObjects[i].lastGravityScore = addResult[i].magnitude;
        }

        buffInput.Release();
        buffResult.Release();

        buffAddResult.Release();
    }

    struct GravOpDat
    {
        public Vector3 objectA;

        public float massA;

        public float strength;
    }
}
