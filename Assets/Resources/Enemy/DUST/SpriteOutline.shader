Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,0,0,1)
        _OutlineSize ("Outline Size", Float) = 1
        _AlphaThreshold ("Alpha Cutoff", Range(0,1)) = 0.01
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 200

        Cull Off
        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _OutlineSize;
            float _AlphaThreshold;
            fixed4 _OutlineColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float centerAlpha = tex2D(_MainTex, i.texcoord).a;

                float outline = 0.0;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * _OutlineSize * _MainTex_TexelSize.xy;
                        float alpha = tex2D(_MainTex, i.texcoord + offset).a;
                        outline = max(outline, step(_AlphaThreshold, alpha));
                    }
                }

                // 외곽선 조건: 중심은 투명하고, 주변에 픽셀이 있을 때
                if (centerAlpha <= _AlphaThreshold && outline > 0.0)
                    return _OutlineColor;

                // 그 외엔 아무것도 안 그림 (완전 투명)
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}