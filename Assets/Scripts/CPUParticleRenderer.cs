using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUParticleRenderer : MonoBehaviour {

    public Material particleMaterial;
    public int particleCount = 1000;

    private ComputeBuffer buffer;

    private void OnEnable() {
        buffer = new ComputeBuffer(particleCount, 4 * 3, ComputeBufferType.Default);
    }

    private void OnDisable() {
        buffer.Release();
    }
    
    void OnRenderObject() {
        particleMaterial.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, particleCount);
    }

    public void SetPositions(Vector3[] particlePositions) {
        buffer.SetData(particlePositions);
        particleMaterial.SetBuffer("_PositionBuffer", buffer);
    }
}
