// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Radius Shader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Radius("Radius", Float) = 1
		_Mid("Midpoint", Vector) = (0,0,0,0)
		_Width("Width", Range(0,1)) = 0.1
	}
		SubShader{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		Pass{
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	struct v2f {
		float4 pos : SV_POSITION;
		float3 objPos : TEXCOORD0;
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.objPos = v.vertex;
		return o;
	}

	half _Radius;
	float3 _Mid;
	half _Width;
	fixed4 _Color;

	half4 frag(v2f i) : SV_Target
	{
		half circle = pow(i.objPos.x - _Mid.x, 2) + pow(i.objPos.y - _Mid.y, 2) + pow(i.objPos.z - _Mid.z, 2);
	if (circle <= pow(_Radius, 2) && circle >= pow(_Radius - _Width, 2))
		return _Color;
	else
		return half4 (0,0,0,0);
	}
		ENDCG
	}
	}
		FallBack "Diffuse"
}
