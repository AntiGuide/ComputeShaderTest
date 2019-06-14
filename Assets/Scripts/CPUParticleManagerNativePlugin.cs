using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class CPUParticleManagerNativePlugin : MonoBehaviour {

    [StructLayout(LayoutKind.Sequential)]
    public struct Particle {
        public Vector3 position;
        public Vector3 velocity;
    }

    public CPUParticleRenderer particleRenderer;
    public int iterationCount;
    public int particleCount;
    private Vector3[] particlePositions;
    private Vector3[] particleVelocities;

    [DllImport("ParticleCompute")]
    public static extern int Compute(Vector3[] positions, Vector3[] velocities, Int32 particleCount, Int32 iterationCount, float dt);

    void Start() {
        particlePositions = new Vector3[particleCount];
        particleVelocities = new Vector3[particleCount];

        for (var i = 0; i < particleCount; ++i) {
            particlePositions[i] = Random.insideUnitSphere * 5;
            particleVelocities[i] = Random.insideUnitSphere * 0f;
        }
    }

    private void Update() {
        Compute(particlePositions, particleVelocities, particleCount, iterationCount, Time.deltaTime);
        particleRenderer.SetPositions(particlePositions);
    }
}
