Shader "Toon/Lit"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Base", 2D) = "white" {}
		_Ramp("Ramp", 2D) = "gray" {}
		_AlphaMultiplier("Alpha Multiplier", range(0.0, 10.0)) = 1.0
	}

		SubShader
		{
			Tags
			{
				"RenderType" = "Opaque"
				"Queue" = "Transparent"
			}
			LOD 200

			CGPROGRAM
				#pragma surface surf ToonRamp alpha:fade

			// Input Properties
			uniform float _AlphaMultiplier;
			uniform float4 _Color;
			uniform sampler2D _Ramp;
			uniform sampler2D _MainTex;

			// Input Structs
			struct Input
			{
				float2 uv_MainTex : TEXCOORD0;
			};

			inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
			{
				#ifndef USING_DIRECTIONAL_LIGHT
				lightDir = normalize(lightDir);
				#endif

				half d = dot(s.Normal, lightDir) * 0.5 + 0.5;
				half3 ramp = tex2D(_Ramp, float2(d,d)).rgb;

				half4 c;
				c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
				c.a = s.Alpha;
				return c;
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 mainTexColor = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = mainTexColor.rgb * _Color;
				o.Alpha = mainTexColor.a * _AlphaMultiplier;
			}
		ENDCG
		}

			Fallback "Diffuse"
}