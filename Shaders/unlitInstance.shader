Shader "Custom/UnlitInstance"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

 SubShader
        {             
                Cull Back
				Tags { "LightMode" = "Deferred" }
                Pass
                {                      
					
				   CGPROGRAM
                    #pragma target 5.0
					#pragma vertex vert
					#pragma geometry geom
                    #pragma fragment frag
					#include "UnityCG.cginc"



					struct agentData
					{
						float3 pos;
						float3 vel;				
						float life;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;


					StructuredBuffer<agentData> _AgentData;
					StructuredBuffer<int> _AgentIndirectBuffer;   
					StructuredBuffer<int> _Indices;
					StructuredBuffer<float2> _UV;
					StructuredBuffer<float3> _VertexData;
					StructuredBuffer<float3> _NormalsData;

					struct v2f
					{
						float2 uv : TEXCOORD0;
						float3 normal : TEXCOORD1;
						float4 vertex : SV_POSITION;
					};

					v2f vert(uint vid : SV_VertexID, uint iid : SV_InstanceID)
					{
			
						agentData A = _AgentData[_AgentIndirectBuffer[iid]];
			

						int idx = _Indices[vid];
						float2 uv = _UV[idx];
						float4 pos = float4(_VertexData[idx], 1.0);
						float3 normal = _NormalsData[idx];

			
						pos.xyz += A.pos;

						v2f o;
						UNITY_INITIALIZE_OUTPUT(v2f,o);
						o.vertex = mul(UNITY_MATRIX_MVP, pos);
						o.normal = mul(unity_ObjectToWorld, normal);
						o.uv = uv;			
			
						return o;
					}

					[maxvertexcount(3)]
					void geom(triangle v2f v[3], inout TriangleStream<v2f> triStream)
					{
							for(int i =0;i<3;i++)
							{
								triStream.Append(v[i]);
							}				
					}
		


					void frag(v2f i,
							out half4 outDiffuse : SV_Target0,
							out half4 outSpecSmoothness : SV_Target1,
							out half4 outNormal : SV_Target2,
							out half4 outEmission : SV_Target3 ) 
					{		
			
						outDiffuse = tex2D(_MainTex,i.uv);
						outNormal = half4(i.normal*0.5+0.5,1.0);
						outEmission = half4(0.0,0.0,0.0,0.0);
						outSpecSmoothness = half4(1.0,1.0,1.0,1.0);
					}
                    ENDCG
                }
        }
Fallback Off
}
