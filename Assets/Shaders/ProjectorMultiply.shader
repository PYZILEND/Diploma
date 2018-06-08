// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Multiply2" {
	Properties {
		_Color("Main Color", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "gray" {}
		_Border("Border", Range(0.01, 1)) = 0.05
		_Radius("Radius", Range(0.5, 50)) = 1.5
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			float _Radius;
			float _Border;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				return o;
			}
			fixed4 _Color;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{
				//return _Color;
				float dx = 0.5 - i.uvShadow.x;
				float dy = 0.5 - i.uvShadow.y;
				float dist = sqrt(dx*dx + dy*dy);
				//float dist2 = ((dx - _Border)*(dx - _Border)
				//+ (dy - _Border)*(dy - _Border));

				if ( (dist>_Radius) && (dist < (_Radius + _Border)))
					return float4(_Color);
				else
					return float4(0, 0, 0, 1);
			}
			ENDCG
		}
	}
}
