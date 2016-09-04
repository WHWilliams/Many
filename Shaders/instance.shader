Shader "Custom/instance"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Scale("unit scale",Float) = 1.0
	}
	CGINCLUDE
					#include "UnityCG.cginc"
					#include "Assets/shaders/Quaternion.cginc"



					struct agentData
					{
						float3 pos;
						float3 vel;				
						float life;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					float _Scale;

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
						float4 pos = float4(_VertexData[idx]*_Scale, 1.0);
						float3 normal = _NormalsData[idx];

						float4 rot;
						float3 zNorm = float3(0.0f,0.0f,1.0f);

						float3 a = cross(zNorm,A.vel);
						rot.xyz = a;
						rot.w = length(A.vel) + dot(zNorm, A.vel);
						rot = normalize(rot);
					
						pos.xyz = rotateWithQuaternion(pos.xyz,rot);
						pos.xyz += A.pos;

						normal = rotateWithQuaternion(normal, rot);

						v2f o;
						o.vertex = mul(UNITY_MATRIX_MVP, pos);
						o.normal = mul(unity_ObjectToWorld, normal);
						o.uv = uv;		
			
						return o;
					}
					struct pOut {
						half4 diffuse           : SV_Target0; // RT0: diffuse color (rgb), occlusion (a)
						half4 spec_smoothness   : SV_Target1; // RT1: spec color (rgb), smoothness (a)
						half4 normal            : SV_Target2; // RT2: normal (rgb), --unused, very low precision-- (a) 
						half4 emission          : SV_Target3; // RT3: emission (rgb), --unused-- (a)
					};

					pOut frag(v2f i) 
					{		
						pOut o;
						o.diffuse = tex2D(_MainTex,i.uv);
						o.spec_smoothness = 0.2;
						o.normal = half4(i.normal*0.5+0.5,1.0);
						o.emission = 0.0;
						
						return o;
					}
	ENDCG

 SubShader
        {             
		

                Pass
                {                      
					Tags { "LightMode" = "Deferred" }
					Stencil{
						Comp Always
						Pass Replace
						Ref 128
					}
					CGPROGRAM
                    #pragma target 5.0
					#pragma vertex vert
                    #pragma fragment frag

                    ENDCG
                }
	
        }
Fallback Off
}
