Shader "Custom/characterFromPoint" 
{
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog { Mode off }
 
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
			#pragma geometry geomChar
			
			struct agentData
			{
				float3 pos;
				float3 vel;				
				float life;
			};

            uniform StructuredBuffer<agentData> agentBuffer;
			uniform StructuredBuffer<int> instanceBuffer;            
			uniform float4 col;		
			uniform float unitScale;
			

            struct v2g
            {
				float4  pos : SV_POSITION;			   
				float3 vNorm : TEXCOORD0;
            };

			struct g2f
			{
				float4 pos  : SV_POSITION;
			};
			
            v2g vert(uint id : SV_VertexID)
            {
                 v2g OUT;
				 int i = instanceBuffer[id];
				 OUT.pos = float4(agentBuffer[i].pos, 1);				 
				 OUT.vNorm = normalize(agentBuffer[i].vel);

                 return OUT;
            }
			


			// unit cube
			[maxvertexcount(14)]
			void geomChar(point v2g P[1], inout TriangleStream<g2f> stream)
			{
				g2f output;
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,-.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,-.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,-.5,-.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,.5,-.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,.5,-.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,-.5,.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,-.5,-.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,-.5,-.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(.5,.5,-.5,0));
				stream.Append(output);
				output.pos = mul(UNITY_MATRIX_VP,P[0].pos+float4(-.5,.5,-.5,0));
				stream.Append(output);
			}

            float4 frag(g2f IN) : COLOR
            {
                return col;
            }
 
            ENDCG
        }
    }
}
