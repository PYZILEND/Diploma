Shader "Custom/BasicShader"
{
	Properties
	{
		[Header(Main Texture)]
		_MainTex("Texture", 2D) = "white" {}
		_SecondTex("Texture", 2D) = "white" {}
		_Tint("Tint",Color) = (1,1,1,1)

		[Header(Rim)]
		[Toggle(RIM_ON)]_Rim("Enable Rim",Float) = 1
		[Toggle]_FullRim("Full Rim",Float) = 1
		_RimWidth("Rim width", Range(0,5)) = 0.7
		_RimIntensity("Rim intensity",Range(1,10)) = 2
		_RimColor("Rim Color",Color) = (1,1,1,1)

		[Header(Specular)] 
		[Toggle(SPECULAR_ON)]_Spec("Enable Specular",Float) = 1
		[Toggle(WATER_SPEC_ON)] _WaterSpec("Enable Water Specular",Float) = 1
		_Shiness("Shiness",Float) = 10
		_SpecCol("Specular Color", Color) = (0.5, 0.5, 0.5)
		_SpecValue("Specular Intensity",Range(0,1)) = 1

		[Toggle(LOWCALC_ON)] _LowCalc("Enable Low Calc",Float) = 1

		/*[Header(Fresnel)]
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelBias("Fresnel Bias", Float) = 0
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 1*/

		[Header(Checkers)]
		_CheckerColor("Checker Color",Color) = (1,0,0,1)
		_CheckerZoom("Checker Zoom",Range(1,2)) = 1.5
		_CheckerValue("Checker Value",Range(2,10)) = 4
		[Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 4
	}
		SubShader
	{
		//

		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
	{
		Tags{

			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}
		Cull Off
		ZWrite Off
		ZTest[_ZTest]
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag


#include "UnityCG.cginc"
	struct appdata
	{
		float4 vertex : POSITION;
	};
	 
	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD1;
	};

	fixed4 _CheckerColor;
	float _CheckerZoom;
	int _CheckerValue;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float2 c = i.vertex.xy*_CheckerZoom;
		c = floor(c) / _CheckerValue;
		float checker = frac(c.x + c.y) * _CheckerValue;
		if (checker == 1) {
			return _CheckerColor;
		}
		return (0, 0, 0, 0);
	}

		ENDCG
	}

		Pass
	{
		Lighting On
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" 
			
			"IgnoreProjector" = "True" }
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag


		#include "UnityCG.cginc"
		//#include "UnityStandardBRDF.cginc"
		//#include "UnityStandardUtils.cginc"
		#include "UnityLightingCommon.cginc"
     #include "AutoLight.cginc"
		//	#pragma multi_compile_fog 
             #pragma multi_compile_fwdbase
		#pragma shader_feature RIM_ON 
		#pragma shader_feature SPECULAR_ON
		#pragma shader_feature LOWCALC_ON
		#pragma shader_feature WATER_SPEC_ON 
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float3 normal : NORMAL;
			fixed4 color : COLOR;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 pos : SV_POSITION;
			half3 normal : NORMAL;
			fixed4 color : COLOR1;
			fixed4 diff : COLOR0;
			float3 viewDir : TEXCOORD1;
			float3 lightDir : TEXCOORD2;
			float NdotL : TEXTCOORD3;
			float NdotV : TEXTCOORD4;
			LIGHTING_COORDS(5, 6)
			//UNITY_FOG_COORDS(0)

		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _SecondTex;
		fixed4 _Tint;
		fixed _RimWidth;
		half _RimIntensity;
		fixed4 _RimColor;
		half _Shiness;
		fixed4 _SpecCol;
		fixed _SpecValue;
		fixed _FullRim;

		/*fixed4 _FresnelColor;
		fixed _FresnelBias;
		fixed _FresnelScale;
		fixed _FresnelPower;*/


		v2f vert(appdata v)
		{
			v2f o;

			//from object space to camera space
			o.pos = UnityObjectToClipPos(v.vertex);
			//o.worldPos = v.vertex.xyz;
			//o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.normal = UnityObjectToWorldNormal(v.normal);
			//vertex colors
			//for optimizing batch calls if needed
			o.color = v.color;

			//UNITY_TRANSFER_FOG(o, o.vertex);
			//half nl = max(0, dot(o.normal, _WorldSpaceLightPos0.xyz));
			//o.diff = nl* _LightColor0;
			//o.diff.rgb += ShadeSH9(half4(o.normal, 1));

			//getting view direction from vertex in object space
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
			o.lightDir = normalize(WorldSpaceLightDir(v.vertex));
			//o.fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(o.viewDir, v.normal), _FresnelPower);
			//float ndotv = 1 - dot(o.normal, o.viewDir);
			//o.col = smoothstep(1 - _RimWidth, 1.0, ndotv);
			o.diff = 0;
			#if LOWCALC_ON
				o.NdotL = saturate(dot(o.normal, _WorldSpaceLightPos0.xyz));
				o.NdotV = 1 - dot(o.normal, o.viewDir);
			#endif
			#if SPECULAR_ON && LOWCALC_ON
				float3 specRef = o.NdotL*pow(saturate(dot(reflect(-o.lightDir, o.normal), o.viewDir)), _Shiness);
				float3 specRef2 = (1 - o.NdotL)*pow(saturate(dot(reflect(o.lightDir, o.normal), o.viewDir)), _Shiness);
				o.diff.rgb += specRef2* _SpecCol*_LightColor0*_SpecValue / 5;
				o.diff.rgb += specRef* _SpecCol*_LightColor0*_SpecValue;
			#endif
			#if LOWCALC_ON && RIM_ON
				float3 cl = smoothstep(1 - _RimWidth, 1.0, o.NdotV) / _RimIntensity;
				if (_FullRim == 1) {
					o.diff.rgb += cl* _RimColor;
				}
				else {
					o.diff.rgb += cl*o.NdotL * _RimColor;
				}
			#endif

				TRANSFER_VERTEX_TO_FRAGMENT(o);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			//sampling texture
			//setting tint and setting vertex color
			fixed4 col = tex2D(_MainTex, i.uv)*_Tint* i.color;
			//diffuse
			fixed4 diff = 1;
			half nl; 
			#if !LOWCALC_ON
				nl = saturate(dot(i.normal, _WorldSpaceLightPos0.xyz));
				diff = nl* _LightColor0;
			#else
				diff = i.NdotL* _LightColor0;
			#endif
			//ambitient light
					half atten = LIGHT_ATTENUATION(i);
					diff.rgb *= atten;

			diff.rgb += ShadeSH9(half4(i.normal, 1));
			col *= diff;
			col.a = 1;
			//rim light
			#if RIM_ON && !LOWCALC_ON
				fixed ndotv = 1 - dot(i.normal, i.viewDir);
				float3 cl = smoothstep(1 - _RimWidth, 1.0, ndotv) / _RimIntensity;
				if (_FullRim == 1) {
					col.rgb += cl* _RimColor;
				}
				else {
					col.rgb += cl*nl* _RimColor;
				}
			#endif
			//specular
			#if SPECULAR_ON && !LOWCALC_ON
				float3 specRef = nl*pow(saturate(dot(reflect(-i.lightDir,i.normal), i.viewDir)), _Shiness);
				float3 specRef2 = (1 - nl)*pow(saturate(dot(reflect(i.lightDir, i.normal), i.viewDir)), _Shiness);
				col.rgb += specRef2* _SpecCol*_LightColor0*_SpecValue / 5;
				col.rgb += specRef* _SpecCol*_LightColor0*_SpecValue;
			#endif

			//water spec
			#if WATER_SPEC_ON && !LOWCALC_ON && !SPECULAR_ON
				float3 specRef = pow(saturate(dot(normalize(i.normal*2), i.viewDir)), _Shiness);
				col.rgb += specRef* _SpecCol*_LightColor0*_SpecValue;
			#endif

			#if LOWCALC_ON
				col.rgb += i.diff.rgb;
			#endif
			//col.rgb = lerp(col, _FresnelColor, 1 - i.fresnel);
			//col = pow((1.0 - saturate(dot(normalize(i.normal), normalize(i.viewDir)))), _Shiness);
			//col.rgb = i.lightDir*0.5+0.5;
				//col.a = i.color.a;
			//	UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

	}
	Fallback "VertexLit"
}
