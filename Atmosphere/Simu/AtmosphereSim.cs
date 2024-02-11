using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NDraw;

using System.Diagnostics;

public class AtmosphereSim : MonoBehaviour
{
    public static int ATMO_PLANET_SIZE = (1 * 3 * sizeof(float)) + (6 * sizeof(float));
    public static int ATMO_OBJECT_SIZE = (2 * 3 * sizeof(float)) + (2 * sizeof(float));

    public ComputeShader atmoShader;
    private int? _p1Shader, _p2Shader;

    void Start()
    {
        _p1Shader = atmoShader.FindKernel("CalculateDensity");
        _p2Shader = atmoShader.FindKernel("CalculateForces");
    }

    void FixedUpdate()
    {
        if (_p1Shader == null || _p2Shader == null)
            throw new System.Exception("FUCKING SHADER SHOULD FUCK YOU OMG I HAE YOU SO FUCKING MUCH");

        if (Atmosphere.atmospheres.Count <= 0 || AtmosphereEffect.atmosphereEffects.Count <= 0)
            return;

        AtmoPlanet[] planets = new AtmoPlanet[Atmosphere.atmospheres.Count];
        AtmoEffect[] objects = new AtmoEffect[AtmosphereEffect.atmosphereEffects.Count];

        for (int i = 0; i < Atmosphere.atmospheres.Count; i++)
        {
            planets[i] = new AtmoPlanet(Atmosphere.atmospheres[i]);
        }

        for (int i = 0; i < AtmosphereEffect.atmosphereEffects.Count; i++)
        {
            objects[i] = new AtmoEffect(AtmosphereEffect.atmosphereEffects[i]);
        }

        ComputeBuffer buffResult, buffDensities, buffPlanets, buffObjects;

        buffResult = new ComputeBuffer(AtmosphereEffect.atmosphereEffects.Count * Atmosphere.atmospheres.Count, sizeof(float) * 3);
        buffDensities = new ComputeBuffer(AtmosphereEffect.atmosphereEffects.Count * Atmosphere.atmospheres.Count, sizeof(float));
        buffPlanets = new ComputeBuffer(Atmosphere.atmospheres.Count, ATMO_PLANET_SIZE);
        buffObjects = new ComputeBuffer(AtmosphereEffect.atmosphereEffects.Count, ATMO_OBJECT_SIZE);

        buffPlanets.SetData<AtmoPlanet>(planets.ToList());
        buffObjects.SetData<AtmoEffect>(objects.ToList());

        atmoShader.SetBuffer((int)_p1Shader, "densities", buffDensities);
        atmoShader.SetBuffer((int)_p1Shader, "planets", buffPlanets);
        atmoShader.SetBuffer((int)_p1Shader, "objects", buffObjects);

        atmoShader.SetInt("planetsLen", planets.Length);
        atmoShader.SetInt("objectsLen", objects.Length);

        atmoShader.Dispatch((int)_p1Shader, planets.Length, objects.Length, 1);

        atmoShader.SetBuffer((int)_p2Shader, "results", buffResult);
        atmoShader.SetBuffer((int)_p2Shader, "densities", buffDensities);
        atmoShader.SetBuffer((int)_p2Shader, "objects", buffObjects);

        atmoShader.SetInt("planetsLen", planets.Length);
        atmoShader.SetInt("objectsLen", objects.Length);

        atmoShader.Dispatch((int)_p2Shader, objects.Length, 1, 1);

        float[] r = new float[AtmosphereEffect.atmosphereEffects.Count * Atmosphere.atmospheres.Count];
        buffDensities.GetData(r);

        buffDensities.Dispose();
        buffPlanets.Dispose();
        buffObjects.Dispose();

        Vector3[] results = new Vector3[AtmosphereEffect.atmosphereEffects.Count];
        buffResult.GetData(results);

        buffResult.Dispose();

        for (int i = 0; i < results.Length; i++)
        {
            AtmosphereEffect.atmosphereEffects[i].rb.AddForce(results[i]);
        }
    }

    struct AtmoPlanet
    {
        public Vector3 position;

        public float seaLevel;

        public float seaLevelPressure;
        public float temperature;
        public float molarMass;
        public float relativeHumidity;
        public float dewPoint;

        public AtmoPlanet(Atmosphere atmo)
        {
            this.position = atmo.transform.position;

            this.seaLevel = atmo.seaLevel;
            this.seaLevelPressure = atmo.seaLevelPressure;
            this.temperature = atmo.temperature;
            this.molarMass = atmo.molarMass;
            this.relativeHumidity = atmo.relativeHumidity;
            this.dewPoint = atmo.dewPoint;
        }
    }

    struct AtmoEffect
    {
        public Vector3 position;
        public Vector3 velocity;

        public float radius;

        public float gravity;

        public AtmoEffect(AtmosphereEffect atmoEffect)
        {
            this.position = atmoEffect.transform.position;
            this.velocity = atmoEffect.rb.velocity;

            this.radius = atmoEffect.transform.lossyScale.x / 2;

            this.gravity = atmoEffect.orb.lastGravityScore;
        }
    }
}
