using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GPUParticleRenderer : MonoBehaviour
{
    public Material particleMaterial;

    private ComputeBuffer _buffer;
    private ComputeBuffer _linesBuffer;
    private void OnRenderObject()
    {
        particleMaterial.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, _buffer.count);
        particleMaterial.SetPass(1);
        Graphics.DrawProcedural(MeshTopology.Points, _linesBuffer.count);
    }

    public void SetBuffers(ComputeBuffer buffer, ComputeBuffer linesBuffer)
    {
        _buffer = buffer;
        _linesBuffer = linesBuffer;
        particleMaterial.SetBuffer("_PositionBuffer", _buffer);
        particleMaterial.SetBuffer("_LineBuffer", _linesBuffer);
    }
}
