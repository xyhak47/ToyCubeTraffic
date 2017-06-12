Shader "Custom/tutorial"
{
	Properties
	{
		_MainTex ("Main Texture From Camera", 2D) = "white" {}

		_Background ("Background", 2D) = "white" {}
		_Greenlight ("Greenlight", 2D) = "white" {}
		_Redlight ("Redlight", 2D) = "white" {}

		[HideInInspector]
		_Blink ("Blink", Float) = 0
		[HideInInspector]
		_Trigger ("Trigger", Float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _Redlight;
			sampler2D _Greenlight;
			sampler2D _Background;

			float _Trigger;
			float _Blink;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);

				fixed4 bg = tex2D(_Background,  i.uv);
				fixed4 red = tex2D(_Redlight,  i.uv);
				fixed4 green = tex2D(_Greenlight,  i.uv);

				fixed4 light = lerp(red, green, _Blink);
				fixed4 mix = bg + light;

				fixed4 mixTex = lerp(mainTex, mix, mix.a);
				mainTex = lerp(mainTex, mixTex, _Trigger);

				return mainTex;
			}
			ENDCG
		}
	}
}
