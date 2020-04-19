Shader "Ferr/Flatlit Color Diffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D   ) = "white" {}
		_Color   ("Color",      Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Flatlit

		sampler2D _MainTex;
		float4    _Color;

		struct Input {
			float2 uv_MainTex;
		};
		
		half4 LightingFlatlit (SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (atten * 2);
			c.a   = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha  = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}