Shader "Custom/PerlinNoiseLayer"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        _NoiseMap3D ("3D Noise Texture", 3D) = "white" {}
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 1.0
        _LayerColor ("Layer Color", Color) = (0,1,0,1)
        _NoiseContrast ("Noise Contrast", Range(0, 2)) = 1.0
        _NoiseSmoothness ("Noise Smoothness", Range(0, 1)) = 0.5
        _NoiseScale ("Noise Scale", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
            };

            sampler2D _BaseMap;
            sampler3D _NoiseMap3D;
            float4 _BaseColor;
            float4 _LayerColor;
            float _NoiseStrength;
            float _NoiseContrast;
            float _NoiseSmoothness;
            float _NoiseScale;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                o.positionWS = TransformObjectToWorld(v.positionOS).xyz;
                o.normalWS = TransformObjectToWorldNormal(float3(0, 0, 1)); // Приклад нормалізованої нормалі
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                // Базовий колір
                half4 baseColor = tex2D(_BaseMap, i.uv) * _BaseColor;

                // Тривимірний шум на основі тріпланарного маппінгу
                float3 noisePosition = i.positionWS * _NoiseScale;

                // Три проєкції для тріпланарного маппінгу
                half noiseX = tex3D(_NoiseMap3D, saturate(noisePosition.zyx)).r; // X проєкція
                half noiseY = tex3D(_NoiseMap3D, saturate(noisePosition.xzy)).r; // Y проєкція
                half noiseZ = tex3D(_NoiseMap3D, saturate(noisePosition.xyz)).r; // Z проєкція

                // Ваги для змішування переходів
                float3 blending = abs(i.normalWS);
                blending = blending / (blending.x + blending.y + blending.z + 1e-6); // Додано запобігання діленню на нуль

                // Змішування трьох проєкцій
                half noiseMask = noiseX * blending.x + noiseY * blending.y + noiseZ * blending.z;

                // Контрастність та згладжування шуму
                noiseMask = pow(noiseMask, _NoiseContrast);
                noiseMask = smoothstep(0.5 - _NoiseSmoothness, 0.5 + _NoiseSmoothness, noiseMask);

                // Колір додаткового шару (зелений)
                half4 layerColor = _LayerColor;

                // Змішування кольорів на основі шуму
                half4 finalColor = lerp(baseColor, layerColor, noiseMask * _NoiseStrength);

                return finalColor;
            }
            ENDHLSL
        }
    }
}