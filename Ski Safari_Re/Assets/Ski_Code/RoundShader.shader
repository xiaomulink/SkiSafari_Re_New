Shader "Unlit/RoundShader"
{
	Properties
	{
		_MainTex("Base(RGB)", 2D) = "white" {}
		_RADIUSBUCE("RADIUSBUCE",Range(0,0.5)) = 0.2
	}

		SubShader
		{

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma exclude_renderers gles
				#include "UnityCG.cginc"
					float _RADIUSBUCE;
				struct v2f
				{
					float2 uv : TEXCOORD0;
					float2 RadiusBuceVU: TEXCOORD1;
					float4 pos : SV_POSITION;
				};
				sampler2D _MainTex;
				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord;
					o.RadiusBuceVU = v.texcoord - float2(0.5, 0.5);
					return o;
				}

				fixed4 frag(v2f i) : COLOR
				{
				   fixed4 col;
				   col = (0,1,1,0);
				   if (abs(i.RadiusBuceVU.x) < 0.5 - _RADIUSBUCE || abs(i.RadiusBuceVU.y) < 0.5 - _RADIUSBUCE)
				   {
					   col = tex2D(_MainTex, i.uv);
				   }
				   else
				   {
					   if (length(abs(i.RadiusBuceVU) - float2(0.5 - _RADIUSBUCE,0.5 - _RADIUSBUCE)) < _RADIUSBUCE)
					   {
						   col = tex2D(_MainTex, i.uv);
					   }
					   else
					   {
						   discard;
					   }
				   }
				   return col;
				}
				ENDCG
			}
		}
}

