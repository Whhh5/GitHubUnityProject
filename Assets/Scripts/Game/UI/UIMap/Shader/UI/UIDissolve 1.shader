Shader "UIShader/UIDissolve"
{
    Properties
    {
        _MainTex            ("Texture",                 2D)             =   "white" {}
        _DissolveTex        ("Dissoive Tex",            2D)             =   "white" {}
        [HDR]_MainColor     ("Main Color",              Color)          =   (1, 1, 1, 1)
        _DissolveScale      ("Dissolve Scale",          Range(0, 1))    =   0
        _GradualChangeScale ("Gradual Change Scale",    Range(0, 1))    =   0
        _EdgeWidth          ("Edge Width",              Range(0, 0.1))  =   0
        [HDR]_EdgeColor     ("Edge Color",              Color)          =   (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}// "RenderPipeline"="UniversalPipline"}

        GrabPass { "_GrabPass_Tex" } 
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex       :   POSITION;
                float2 uv           :   TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex       :   SV_POSITION;
                float2 uv           :   TEXCOORD0;
            };

            sampler2D       _MainTex;
            sampler2D       _DissolveTex;
            sampler2D       _GrabPass_Tex;
            fixed4          _MainColor;
            fixed4          _EdgeColor;
            fixed           _DissolveScale;
            fixed           _GradualChangeScale;
            fixed           _EdgeWidth;
            // float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f fragData) : SV_Target
            {
                // fixed2 customUV = 
                fixed4 endColor = fixed4(0, 0, 0, 1);

                return tex2D(_GrabPass_Tex, fragData.uv);
            }
            ENDCG
        }

    }
}
