using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleManager : MonoBehaviour {
    public GPUParticleRenderer particleRenderer;
    public ComputeShader computeShader;

    public int maxParticleCount = 1000;
    public int iterationCount = 1;

    private ComputeBuffer[] _positionBuffers;
    private ComputeBuffer _velocitiesBuffer;

    private int _updateKernelId;

    private bool _isSwapped;

    private void OnEnable() {
        maxParticleCount = Mathf.FloorToInt((maxParticleCount / 8) * 8);
        _positionBuffers = new ComputeBuffer[2];
        _positionBuffers[0] = new ComputeBuffer(maxParticleCount, 3 * 4);
        _positionBuffers[1] = new ComputeBuffer(maxParticleCount, 3 * 4);
        _velocitiesBuffer = new ComputeBuffer(maxParticleCount, 3 * 4);

        var particles = new Vector3[maxParticleCount];
        for (var i = 0; i < maxParticleCount; i++) {
            particles[i] = Random.insideUnitSphere * 10;
        }
        _positionBuffers[0].SetData(particles);

        _updateKernelId = computeShader.FindKernel("UpdateParticlesNoise");
        computeShader.SetBuffer(_updateKernelId, "_Velocities", _velocitiesBuffer);
    }

    private void OnDisable() {
        _velocitiesBuffer.Release();
        _positionBuffers[0].Release();
        _positionBuffers[1].Release();
        _positionBuffers = null;
    }

    void Update() {
        for (var i = 0; i < iterationCount; i++) {
            computeShader.SetBuffer(_updateKernelId, "_InPositions", _positionBuffers[0]);
            computeShader.SetBuffer(_updateKernelId, "_OutPositions", _positionBuffers[1]);

            computeShader.SetInt("_ParticleCount", maxParticleCount);
            computeShader.SetFloat("_Dt", Mathf.Min(Time.deltaTime / iterationCount, 0.033f));

            computeShader.Dispatch(_updateKernelId, maxParticleCount / 8, 1, 1);

            var temp = _positionBuffers[0];
            _positionBuffers[0] = _positionBuffers[1];
            _positionBuffers[1] = temp;
        }
        particleRenderer.SetBuffer(_positionBuffers[0]);
    }
}
