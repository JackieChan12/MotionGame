Shader "Custom/TransparentShader" 
{ 
	Properties 
	{ 
		_Color ("Color", Color) = (1,1,1,1) 
		_MainTex ("Albedo (RGB)", 2D) = "white" {} 
	} 
	SubShader 
	{ 
		Tags 
		{ "Queue"="Transparent" "RenderType"="Transparent" } 
		LOD 200 
		
		CGPROGRAM 
		#pragma surface surf Standard alpha 
		sampler2D _MainTex; 
		fixed4 _Color; 
		struct Input 
		{ 
			float2 uv_MainTex; 
		}; 
		void surf (Input IN, inout SurfaceOutputStandard o) 
		{ 
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color; 
			o.Albedo = c.rgb;
			o.Alpha = c.a; 
		} 
		ENDCG 
	} 
	FallBack "Diffuse" 
}