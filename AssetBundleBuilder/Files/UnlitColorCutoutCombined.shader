Shader "UnlitColorCutoutCombined"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SubTex("Secondary Texture (RGB)", 2D) = "black" {}
		_Color("Color Mask", Color) = (0, 0, 0, 0)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent+1000"  "IgnoreProjector" = "True" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _SubTex;
			float4 _SubTex_ST;
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
				fixed4 col1 = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_SubTex, i.uv);
				if (all(col1 == _Color)) {
					// Erase col1
					return col2;
				}

				return fixed4(lerp(col1.rgb, col2.rgb, col2.a), 1);
			}
			ENDCG
		}
	}
}
