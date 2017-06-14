// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//UnlitShadowReceive
 Shader "Custom/background" 
 { 
     Properties 
     { 
         _MainTex("Base (RGB)", 2D) = "white" {} 

 		[HideInInspector]
		_TheWhite ("White Texture", 2D) = "black" {}
     } 
 
     SubShader 
     {     
         Pass 
         { 
             Tags { "LightMode" = "ForwardBase" } 

             CGPROGRAM 

             #pragma vertex vert 
             #pragma fragment frag 

             #include "UnityCG.cginc" 
             #pragma multi_compile_fwdbase 
             #include "AutoLight.cginc" 

             sampler2D _MainTex;
             float4 _MainTex_ST;
             sampler2D _TheWhite;

             struct v2f 
             { 
                 float4 pos : SV_POSITION; 
                 LIGHTING_COORDS(0,1) 
                 float2 uv : TEXCOORD2;
             }; 
 
             v2f vert(appdata_base v) 
             { 
                 v2f o; 
                 o.pos = UnityObjectToClipPos(v.vertex); 
                 o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                 TRANSFER_VERTEX_TO_FRAGMENT(o); 
                 return o; 
             } 
 
             fixed4 frag(v2f i) : SV_Target
             { 
                float attenuation = LIGHT_ATTENUATION(i); 
             	fixed4 col_bg = tex2D(_MainTex, i.uv);
             	col_bg *= attenuation;

				fixed4 col_white = tex2D(_TheWhite, i.uv);
				fixed4 col = lerp(col_bg, col_white, col_white.a);

                return col;
             } 
             ENDCG 
         } 
 
         Pass 
         { 
             Blend One One
             Tags { "LightMode" = "ForwardAdd" } 
             CGPROGRAM 
             #pragma vertex vert 
             #pragma fragment frag 
             #include "UnityCG.cginc" 
             #pragma multi_compile_fwdadd_fullshadows 
             #include "AutoLight.cginc" 
             sampler2D _MainTex;
             float4 _MainTex_ST;
 
             struct v2f { 
                 float4 pos : SV_POSITION; 
                 LIGHTING_COORDS(0,1) 
                 float2 uv : TEXCOORD2;
             }; 
 
             v2f vert(appdata_base v) { 
                 v2f o; 
                 o.pos = UnityObjectToClipPos(v.vertex); 
                 o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                 TRANSFER_VERTEX_TO_FRAGMENT(o); 
                 return o; 
             } 
 
             fixed4 frag(v2f i) : COLOR
             { 
                 float attenuation = LIGHT_ATTENUATION(i); 
                 return tex2D (_MainTex, i.uv) * attenuation; 
             } 
             ENDCG 
         } 
     } 
     Fallback "VertexLit" 
 }