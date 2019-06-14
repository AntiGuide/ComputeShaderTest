using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUParticleRenderer : MonoBehaviour {
    public Material particleMaterial;

    private ComputeBuffer _buffer;

    void OnRenderObject() {
        particleMaterial.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, _buffer.count);
    }

    public void SetBuffer(ComputeBuffer buffer) {
        _buffer = buffer;
        particleMaterial.SetBuffer("_PositionBuffer", buffer);
    }

}
