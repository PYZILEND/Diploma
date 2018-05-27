Shader "Custom/BasicLowPolyShader"
{
	Properties
	{
		[Header(Main Texture)]
	_MainTex("Texture", 2D) = "white" {}
	_Tint("Tint",Color) = (1,1,1,1)
		[Header(Specular)]
		_Shiness("Shiness",Float) = 10
		_SpecCol("Specular Color", Color) = (0.5, 0.5, 0.5)
		_SpecValue("Specular Intensity",Range(0,1)) = 1

		[Header(Waves)]

	_WaveLength("Wave length", Float) = 0.5
		_WaveHeight("Wave height", Float) = 0.5
		_WaveSpeed("Wave speed", Float) = 1.0
		_RandomHeight("Random height", Float) = 0.5
		_RandomSpeed("Random Speed", Float) = 0.5
	}
		SubShader
	{
		Tags{
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
		"LightMode" = "ForwardBase"
		"IgnoreProjector" = "True"
	}

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
	{
		CGPROGRAM
#pragma vertex vert           
#pragma geometry geom
#pragma fragment frag


#include "UnityCG.cginc"
		//#include "UnityStandardBRDF.cginc"
		//#include "UnityStandardUtils.cginc"
#include "UnityLightingCommon.cginc"
	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float3 normal : NORMAL;
		fixed4 color : COLOR;
	};

	struct v2g
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 pos : TEXCOORD1;
		half3 normal : NORMAL;
		fixed4 color : COLOR;
		float3 viewDir : TEXCOORD2;
		float3 lightDir : TEXCOORD3;
	};

	struct g2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
		half3 normal : NORMAL;
		fixed3 light : TEXCOORD1;
		fixed4 color : COLOR;
	};
	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _Tint;
	half _Shiness;
	fixed4 _SpecCol;
	fixed _SpecValue;

	float _WaveLength;
	float _WaveHeight;
	float _WaveSpeed;
	float _RandomHeight;
	float _RandomSpeed;


	float rand(float3 co) {
		return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
	}

	float rand2(float3 co) {
		return frac(sin(dot(co.xyz, float3(19.9128, 75.2, 34.5122))) * 12765.5213);
	}

	v2g vert(appdata v)
	{
		float3 v0 = mul(unity_ObjectToWorld, v.vertex).xyz;

		float phase0 = (_WaveHeight)* sin((_Time[1] * _WaveSpeed) + (v0.x * _WaveLength) + (v0.z * _WaveLength) + rand2(v0.xzz));
		float phase0_1 = (_RandomHeight)*sin(cos(rand(v0.xzz) * _RandomHeight * cos(_Time[1] * _RandomSpeed * sin(rand(v0.xxz)))));

		v0.y += phase0 + phase0_1;
		//v.vertex.xyz = mul((float3x3)unity_WorldToObject, v0) - _WaveHeight;
		v.vertex.y = (mul((float3x3)unity_WorldToObject, v0) - _WaveHeight).y;

		v2g o;
		
		o.pos= v.vertex;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		o.normal = UnityObjectToWorldNormal(v.normal);
		o.color = v.color;
		o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
		o.lightDir = normalize(WorldSpaceLightDir(v.vertex));
		return o;
	}

	[maxvertexcount(3)]
	void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
	{
		g2f o;
		//normal for triangle
		float3 A = IN[1].pos.xyz - IN[0].pos.xyz;
		float3 B = IN[2].pos.xyz - IN[0].pos.xyz;		
		float3 fn = normalize(cross(A, B));

		//diffuse
		half nl = saturate(dot(fn, _WorldSpaceLightPos0.xyz)); 
		float3 lights= nl* _LightColor0;
		lights += ShadeSH9(half4(fn, 1));

		//float3 centerPos = (IN[0].pos.xyz + IN[1].pos.xyz + IN[2].pos.xyz) / 3.0;
		//float3 viewDir = normalize(_WorldSpaceCameraPos- mul(unity_ObjectToWorld, float4(centerPos, 0.0)).xyz);

		float3 viewDir = (IN[0].viewDir.xyz + IN[1].viewDir.xyz + IN[2].viewDir.xyz) / 3.0;
		float3 lightDir = (IN[0].lightDir.xyz + IN[1].lightDir.xyz + IN[2].lightDir.xyz) / 3.0;
		float3 specRef = nl*pow(saturate(dot(reflect(-lightDir, fn), viewDir)), _Shiness);
		lights += specRef*_SpecCol*_LightColor0*_SpecValue;
		//col.rgb += specRef* _SpecCol*_LightColor0*_SpecValue;
		for (int i = 0; i < 3; i++)
		{
			o.normal = fn;
			o.light = lights;
			o.color = IN[i].color;
			o.vertex = IN[i].vertex;
			o.uv = IN[i].uv;
			triStream.Append(o);
		}
	}

	fixed4 frag(g2f i) : SV_Target
	{
		//sampling texture
		//setting tint and setting vertex colo
		fixed4 col  = tex2D(_MainTex, i.uv)*_Tint*i.color;
		col.rgb  *=i.light;
		col.a = 1;
		return col;
	}
		ENDCG
	}

	}
		Fallback "Diffuse"
}
