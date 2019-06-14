using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CPUParticleManagerStructs : MonoBehaviour {

    private struct Particle {
        public Vector3 position;
        public Vector3 velocity;
    }

    public int iterationCount;
    public int particleCount;
    private List<Particle> particles;

    void Start() {
        particles = new List<Particle>(particleCount);

        for (var i = 0; i < particleCount; ++i) {
            var particle = new Particle();
            particle.position = Random.insideUnitSphere * 5;
            particle.velocity = Random.insideUnitSphere;
            particles.Add(particle);
        }

        var startTime = DateTime.Now;

        for (var iteration = 0; iteration < iterationCount; ++iteration) {
            for (var i = 0; i < particleCount; ++i) {
                var current = particles[i];
                for (var j = 0; j < particleCount; ++j) {
                    var other = particles[j];
                    var fromTo = other.position - particles[i].position;
                    current.velocity += fromTo.normalized / fromTo.sqrMagnitude;
                }
                particles[i] = current;
            }

            for (var i = 0; i < particleCount; ++i) {
                var current = particles[i];
                current.position += particles[i].velocity * 0.05f;
                particles[i] = current;
            }
        }

        var endTime = DateTime.Now;
        var totalTime = endTime - startTime;
        var totalMilliseconds = totalTime.TotalMilliseconds;
        Debug.Log("Structs took : " + totalMilliseconds + " Milliseconds to compute");
    }
}
