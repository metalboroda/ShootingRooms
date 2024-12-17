Shader "Custom/DecalProjector"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DecalTex ("Decal Texture", 2D) = "white" {}
        _DecalPosition ("Decal Position", Vector) = (0, 0, 0, 0)
        _DecalSize ("Decal Size", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Вхідна структура для вершин
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Проміжна структура для передачі даних у фрагментний шейдер
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _DecalTex;

            float4 _MainTex_ST; // Для tiling та offset основної текстури

            float4 _DecalPosition; // Позиція декалі у world space
            float _DecalSize;      // Розмір декалі

            // Вершинний шейдер
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // Коректний UV з tiling/offset
                return o;
            }

            // Фрагментний шейдер
            fixed4 frag(v2f i) : SV_Target
            {
                // Основна текстура
                fixed4 baseColor = tex2D(_MainTex, i.uv);

                // Проєкція декалі у world space
                float3 decalDir = i.worldPos - _DecalPosition.xyz;
                float dist = length(decalDir);

                // Масштабування розміру декалі
                if (dist < _DecalSize)
                {
                    // Спроєктовані UV для декалі (XZ plane)
                    float2 decalUV = decalDir.xz / _DecalSize * 0.5 + 0.5;

                    // Перевірка, чи UV декалі в межах текстури
                    if (decalUV.x >= 0 && decalUV.x <= 1 && decalUV.y >= 0 && decalUV.y <= 1)
                    {
                        fixed4 decalColor = tex2D(_DecalTex, decalUV);
                        
                        // Альфа-блендинг: змішування декалі з основною текстурою
                        baseColor = lerp(baseColor, decalColor, decalColor.a);
                    }
                }

                return baseColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}