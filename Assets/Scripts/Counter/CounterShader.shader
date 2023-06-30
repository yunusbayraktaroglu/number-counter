Shader "Unlit/CounterShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { 
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZTest Always

		Pass
		{
			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct Attributes
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
				float uv2 : TEXCOORD1;
			};

			struct Varyings
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float uCurrentDigits[9];
			float uTargetDigits[9];
			float uTargetOffsets[9];
			float uStartTime;
			float uDuration;
			bool uNeedsAnimate = false;

			// Ease function
			static float ease(float t) {
				return t < .5 ? 4. * t * t * t : .5 * pow(2. * t - 2., 3.) + 1.;
			}

			Varyings vert(Attributes IN)
			{
				Varyings OUT;

				// Convert 0-1 uv to 0.5-0.1 as represent a number slot
				float2 customUv = IN.uv * float2(0.5, 0.1);


				// Quad index
				int i = int(IN.uv2);

				if (uCurrentDigits[i] + uTargetDigits[i] == -2.0){

					OUT.position = float4(0.0, 0.0, 0.0, 0.0);

				} else {

					OUT.position = UnityObjectToClipPos(IN.position.xyz);

					// Example: Number '9' in vertical numbers texture, 'position y' will be 0.9
					float2 _number = float2(0.0, 0.9 - uCurrentDigits[i]);

					if (uNeedsAnimate){

						// 0.0 to 1.0 progress
						float progress = ease(min((_Time.y - uStartTime) / uDuration, 1.0));

						float _offsetCompleted = uTargetOffsets[i] * progress;
						float _offsetRemaining = distance(uTargetOffsets[i], _offsetCompleted);

						_number -= float2(0.0, _offsetCompleted);

						// if ( uTargetDigits[i] < 0.0 && _offsetRemaining < 0.1 || uCurrentDigits[i] < 0.0 && _offsetCompleted < 0.1 ){
						//	_number += numberZeroSlot();
						// }
						// To avoid above if-else shader branching depends on dynamic variables:
						float targetDigitCheck = uTargetDigits[i] < 0.0 && abs(_offsetRemaining) < 0.1;
						float currentDigitCheck = uCurrentDigits[i] < 0.0 && abs(_offsetCompleted) < 0.1;

						_number += float2(0.5, 0.0) * (targetDigitCheck + currentDigitCheck);
					}

					customUv += _number;

				}

				OUT.uv = TRANSFORM_TEX(customUv, _MainTex);
				return OUT;
			}

			half4 frag(Varyings IN) : SV_Target
			{
				half4 col = tex2D(_MainTex, IN.uv);
				return col;
			}

			ENDHLSL
		}
	}
}
