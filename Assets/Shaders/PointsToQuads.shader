Shader "Unlit/PointsToQuads"
{
    Properties
    {
        _Color("Tint", color) = (1,1,1,1)
        _MainTex("Texture", 2D) = "white" {}
        _Size("Particle Size", float) = 0.1
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue"="Transparent"}
            LOD 100
            Blend One One
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma geometry geom
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct v2g
                {
                    float4 worldPos : SV_POSITION;
                };

                struct g2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                fixed4 _Color;
                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Size;
                StructuredBuffer<float3> _PositionBuffer;

                v2g vert(uint id : SV_VertexID)
                {
                    v2g o;
                    o.worldPos = mul(UNITY_MATRIX_M, _PositionBuffer[id]);
                    return o;
                }

                [maxvertexcount(4)]
                void geom(point v2g input[1], inout TriangleStream<g2f> tristream)
                {
                    float3 worldPos = input[0].worldPos.xyz;
                    float3 toCamera = normalize(worldPos - _WorldSpaceCameraPos);
                    float3 up = float4(0,1,0,0);
                    float3 right = cross(up, toCamera);

                    up *= _Size;
                    right *= _Size;

                    g2f o;

                    o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos - right + up,1));
                    o.uv = float2(0,1);
                    tristream.Append(o);

                    o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos + right + up,1));
                    o.uv = float2(1,1);
                    tristream.Append(o);

                    o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos - right - up,1));
                    o.uv = float2(0,0);
                    tristream.Append(o);

                    o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos + right - up,1));
                    o.uv = float2(1,0);
                    tristream.Append(o);
                }

                fixed4 frag(g2f i) : SV_Target
                {
                    // sample the texture
                    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                    return col;
                }
                ENDCG
            }
        }
}