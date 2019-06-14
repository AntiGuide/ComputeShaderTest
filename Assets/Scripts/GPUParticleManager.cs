using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleManager : MonoBehaviour
{
    public GPUParticleRenderer particleRenderer;
    public ComputeShader computeShader;
    
    public int maxParticleCount = 1000;
    public int iterationsCount = 1;

    private ComputeBuffer[] _positionsBuffers;
    private ComputeBuffer   _velocitiesBuffer;
    private ComputeBuffer   _linesBuffer;

    private int _updateKernelId;
    private int _generateLinesKernelId;

    
    void OnEnable()
    {
        maxParticleCount = (maxParticleCount / 8) * 8;
        _positionsBuffers =    new ComputeBuffer[2];
        _positionsBuffers[0] = new ComputeBuffer(maxParticleCount, 3*4 );
        _positionsBuffers[1] = new ComputeBuffer(maxParticleCount, 3*4 );
        _velocitiesBuffer =    new ComputeBuffer(maxParticleCount, 3*4 );
        _linesBuffer =         new ComputeBuffer(maxParticleCount, 3*4*2);
        
        Vector3[] particles = new Vector3[maxParticleCount];
        for (int i = 0; i < maxParticleCount; ++i)
        {
            particles[i] = Random.insideUnitSphere * 10;
        }
        _positionsBuffers[0].SetData(particles);
        
        _updateKernelId = computeShader.FindKernel("UpdateParticlesNoise");
        _generateLinesKernelId = computeShader.FindKernel("GenerateLineIndices");
        computeShader.SetBuffer(_updateKernelId, "_Velocities", _velocitiesBuffer);
    }

    void OnDisable()
    {
        _linesBuffer.Release();
        _velocitiesBuffer.Release();
        _positionsBuffers[1].Release();
        _positionsBuffers[0].Release();
        _positionsBuffers = null;
    }

    void Update()
    {
        computeShader.SetInt("_ParticleCount", maxParticleCount);
        for (int i = 0; i < iterationsCount; ++i)
        {
            computeShader.SetBuffer(_updateKernelId, "_InPositions",  _positionsBuffers[0]);
            computeShader.SetBuffer(_updateKernelId, "_OutPositions", _positionsBuffers[1]);
        
            computeShader.SetFloat("_Dt", Mathf.Min(Time.deltaTime, 0.033f) / iterationsCount);
        
            computeShader.Dispatch(_updateKernelId, maxParticleCount /8, 1, 1);
        
            ComputeBuffer temp = _positionsBuffers[0];
            _positionsBuffers[0] = _positionsBuffers[1];
            _positionsBuffers[1] = temp;
        }
        _linesBuffer.SetCounterValue(0);
        computeShader.SetBuffer(_generateLinesKernelId, "_InPositions", _positionsBuffers[0]);
        computeShader.SetBuffer(_generateLinesKernelId, "_Lines", _linesBuffer);
        
        computeShader.Dispatch(_generateLinesKernelId, maxParticleCount /8, 1, 1);
        
        
        particleRenderer.SetBuffers(_positionsBuffers[0], _linesBuffer);
    }
}
