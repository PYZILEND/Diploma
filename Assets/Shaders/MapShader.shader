// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/MapShader"
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
     }
     
     CGINCLUDE
     #include "UnityCG.cginc"
     #include "AutoLight.cginc"
     #include "Lighting.cginc"
     ENDCG
 
  SubShader
  {
      LOD 200
      Pass { 
             Lighting On
             Tags { "RenderType" = "Opaque"  "LightMode" = "ForwardBase"}
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile_fwdbase
			#pragma shader_feature RIM_ON 
			#pragma shader_feature SPECULAR_ON
			#pragma shader_feature LOWCALC_ON
			#pragma shader_feature WATER_SPEC_ON 
 

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
 
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
			};

             struct v2f
             {
                /* half4     pos                :    SV_POSITION;
                 fixed4     lightDirection    :    TEXCOORD1;
                 fixed3     viewDirection    :    TEXCOORD2;
                 fixed3     normalWorld        :    TEXCOORD3;*/
				 float4 pos : SV_POSITION;
				 half3 normal : NORMAL;
				 fixed4 color : COLOR1;
				 fixed4 diff : COLOR0;
				 float2 uv : TEXCOORD0;
				 float3 viewDir : TEXCOORD1;
				 float3 lightDir : TEXCOORD2;
				 float NdotL : TEXTCOORD3;
				 float NdotV : TEXTCOORD4;
                 LIGHTING_COORDS(5,6)
             };
 
              v2f vert (appdata v)
             {
                 v2f o;
                 
                /* half4 posWorld = mul( _Object2World, v.vertex );
                 o.normalWorld = normalize( mul(half4(v.normal, 0.0), _World2Object).xyz );
                 o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                 o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);*/
				 half4 posWorld = mul(unity_ObjectToWorld, v.vertex);

				 o.pos = UnityObjectToClipPos(v.vertex);
				 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				 o.normal = UnityObjectToWorldNormal(v.normal);
				 o.color = v.color;
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
                 TRANSFER_VERTEX_TO_FRAGMENT(o);
                 
                 return o;
             }
             
             half4 frag (v2f i) : COLOR
             {
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
      }
      FallBack "Diffuse"
  }
