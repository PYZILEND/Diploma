Shader "Custom/BorderShader"
{
	Properties
	{
		//Sentinels, Dominion, Neutral
		_MainTex ("Texture", 2D) = "white" {}
		[Toggle]_ModeSentinels("Mode Sentinels",Float)=0
		[Toggle]_ModeDominion("Mode Dominion",Float) = 0
		[Toggle]_OppositeColors("Opposite colors",Float) = 0
		_ColorSentinels("Color Sentinels",Color) = (0, 0, 1,1)
		_ColorDominion("Color Dominion",Color) = (1, 0,0,1)
		_ColorNeutral("Color neutral",Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags{

			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
				Cull Off
				ZWrite Off
				ZTest Always
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag


			#include "UnityCG.cginc"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
			};

			fixed _ModeSentinels;
			fixed _ModeDominion;
			fixed _OppositeColors;
			fixed4 _ColorSentinels;
			fixed4	_ColorDominion;
			fixed4	_ColorNeutral;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if (_ModeSentinels == 0) 
				{
					if (_ModeDominion == 0) 
					{
						return _ColorNeutral;
					}

					return _ColorDominion;					
				}
				
					if (_ModeDominion == 0) 
					{
						return _ColorSentinels;
					}
					
					if (i.uv.x == 0)
					{
						if (_OppositeColors == 0)
						{
							return _ColorSentinels;
						}
						return _ColorDominion;
					}

					if (_OppositeColors == 0) 
					{
						return _ColorDominion;
					}
					return _ColorSentinels; 
				
			}

				ENDCG
		}

		
		
	}
}
