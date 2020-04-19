Shader "Ferr/Flatlit Sway" {
	Properties {
		_MainTex ("Base (RGB)", 2D   ) = "white" {}
		_Color   ("Color",      Color) = (1,1,1,1)
		
		_SwayScale    ("Sway Scale",     float) = 0.1
		_SwaySpeed    ("Sway Speed",     float) = 1.5708
		_SwayCoherence("Sway Coherence", float) = 0.03
		
		_PeturbScale    ("Peturb Scale",     float) = 0.03
		_PeturbSpeed    ("Peturb Speed",     float) = 3
		_PeturbCoherence("Peturb Coherence", float) = 0.8
		
		_WindDirection ("Wind Direction", Vector) = (1,0.5,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Flatlit vertex:vert

		sampler2D _MainTex;
		float4    _Color;
		
		float _SwayScale;
		float _SwaySpeed;
		float _SwayCoherence;
		float _PeturbCoherence;
		float _PeturbSpeed;
		float _PeturbScale;
		float4 _WindDirection;

		struct Input {
			float2 uv_MainTex;
		};
		
		void vert (inout appdata_full v) {
			float4 world = mul(unity_ObjectToWorld, v.vertex);
			
			float time       = (_Time.y + (world.x + world.z) * _SwayCoherence ) * _SwaySpeed;
			float peturbTime = (_Time.y + (world.x + world.z) * _PeturbCoherence) * _PeturbSpeed;
			world.xz += cos(time      ) * _WindDirection * (v.vertex.z * v.vertex.z) * _SwayScale;
			world.xz += cos(peturbTime) * _WindDirection * (v.vertex.z * v.vertex.z) * _PeturbScale;
			
			v.vertex = mul(unity_WorldToObject, world);
		}
		
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