// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/BasicWaterShader"
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

		[Header(Waves)]

		_WaveLength("Wave length", Float) = 0.5
			_WaveHeight("Wave height", Float) = 0.5
			_WaveSpeed("Wave speed", Float) = 1.0
			_RandomHeight("Random height", Float) = 0.5
			_RandomSpeed("Random Speed", Float) = 0.5

			/*[NoScaleOffset] _Foam("Foam texture", 2D) = "white" {}
		[NoScaleOffset] _FoamGradient("Foam gradient ", 2D) = "white" {}
		_FoamStrength("Foam strength", Range(0, 10.0)) = 1.0*/

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
		#pragma fragment frag


		#include "UnityCG.cginc"
		#include "UnityLightingCommon.cginc"
		#pragma multi_compile_fog 
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
			float4 vertex : SV_POSITION;
			half3 normal : NORMAL;
			fixed4 color : COLOR1;
			fixed4 diff : COLOR0;
			float3 viewDir : TEXCOORD6;
			float3 lightDir : TEXCOORD2;
			float fresnel : TEXCOORD3;
			float NdotL : TEXTCOORD4;
			float NdotV : TEXTCOORD5;
			//float4 ref : TEXCOORD7;
			//float2 foamuv : TEXCOORD8;
			//float2 depth : TEXCOORD9;
			UNITY_FOG_COORDS(1)
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

		float _WaveLength;
		float _WaveHeight;
		float _WaveSpeed;
		float _RandomHeight;
		float _RandomSpeed;
		

		uniform float _FoamStrength;
		uniform sampler2D _CameraDepthTexture; //Depth Texture
		sampler2D _Foam;
		sampler2D _FoamGradient;

		float rand(float3 co) {
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		float rand2(float3 co) {
			return frac(sin(dot(co.xyz, float3(19.9128, 75.2, 34.5122))) * 12765.5213);
		}

		v2f vert(appdata v)
		{
			float3 v0 = mul(unity_ObjectToWorld, v.vertex).xyz;

			float phase0 = (_WaveHeight)* sin((_Time[1] * _WaveSpeed) + (v0.x * _WaveLength) + (v0.z * _WaveLength) + rand2(v0.xzz));
			float phase0_1 = (_RandomHeight)*sin(cos(rand(v0.xzz) * _RandomHeight * cos(_Time[1] * _RandomSpeed * sin(rand(v0.xxz)))));

			v0.y += phase0 + phase0_1;
			v.vertex.xyz = mul((float3x3)unity_WorldToObject, v0) -_WaveHeight;
			v2f o;

			//from object space to camera space
			o.vertex = UnityObjectToClipPos(v.vertex);
			//o.ref = ComputeScreenPos(o.vertex);
			//UNITY_TRANSFER_DEPTH(o.depth);

			//float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			//o.foamuv = 7.0f * wpos.xz + 0.05 * float2(_SinTime.w, _SinTime.w);

			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			o.normal = UnityObjectToWorldNormal(v.normal);
			//vertex colors
			//for optimizing batch calls if needed
			o.color = v.color;

			UNITY_TRANSFER_FOG(o, o.vertex);

			//getting view direction from vertex in object space
			o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
			o.lightDir = normalize(WorldSpaceLightDir(v.vertex));
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
				//half4 foam = (tex2D(_SecondTex, (i.bumpCoords*2).xy) * tex2D(_SecondTex, (i.bumpCoords * 2).zw)) - 0.125;
				//half4 foam = Foam(_SecondTex, i.bumpCoords * 2.0);
				//col.rgb += foam.rgb *0.1 * (saturate(0 - 0.375));


				/*float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float objectZ = i.ref.z;
				float intensityFactor = 1 - saturate((sceneZ - objectZ) / _FoamStrength);
				half3 foamGradient = 1 - tex2D(_FoamGradient, float2(intensityFactor - _Time.y*0.2, 0));
				//float2 foamDistortUV = bump.xy * 0.2;
				half3 foamColor = tex2D(_Foam, i.foamuv).rgb;
				col.rgb = foamGradient * intensityFactor * foamColor;
				*/

				//UNITY_OUTPUT_DEPTH(i.depth);
				UNITY_APPLY_FOG(i.fogCoord, col);
			return col;
			}
			ENDCG
		}

		/*Pass{
			Cull Off
			ZWrite Off
			ZTest Less
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
	
			struct appdata
			{
				float4 vertex : POSITION;
				//float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{				
				//half4 foam = Foam(_ShoreTex, i.bumpCoords * 2.0);
				//col.rgb += foam.rgb * _Foam.x * (edgeBlendFactors.y + saturate(i.viewInterpolator.w - _Foam.y));
				//half depth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
				float sceneZ = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.ref)).r);
				float objectZ = i.ref.z;
				float intensityFactor = 1 - saturate((sceneZ - objectZ) / _FoamStrength);
				half3 foamGradient = 1 - tex2D(_FoamGradient, float2(intensityFactor - _Time.y*0.2, 0) + bump.xy * 0.15);
				float2 foamDistortUV = bump.xy * 0.2;
				half3 foamColor = tex2D(_Foam, i.foamuv + foamDistortUV).rgb;
				color.rgb += foamGradient * intensityFactor * foamColor;
			}
				ENDCG
		}*/
		
	}
		Fallback "Diffuse"
}
