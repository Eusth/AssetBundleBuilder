﻿Shader "UnlitColorCutout"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Ignore Color", Color) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent+1000"  "IgnoreProjector" = "True" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				if (all(col == _Color)) {
					discard;
				}
				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
}
