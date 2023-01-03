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

        Pass
        {
            // ZWrite On
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
                fixed2  customUV        =   fixed2(fragData.uv.x, 1 - fragData.uv.y);
                // _DissolveScale          =   (_DissolveScale + _GradualChangeScale * 2 * _DissolveScale) * 2 - (1 + _GradualChangeScale * 2 * _DissolveScale);
                _DissolveScale          =   2 * _DissolveScale * (1 + _GradualChangeScale) - 1;
                fixed4  endColor        =   tex2D(_MainTex, fragData.uv);
                fixed4  dissolveColor   =   tex2D(_DissolveTex, customUV + (_Time.x % 1));

                // Region   Color Alpha Blend
                endColor            =   _MainColor.a * fixed4(_MainColor.rgb, 1) * ceil(endColor.a) + endColor * (1 - _MainColor.a);
                // EndRegion

                // Region   Clip
                fixed   lineY       =   _DissolveScale + fragData.uv.x;
                endColor            =   endColor * min(max(ceil(lineY - fragData.uv.y), 0), 1);
                // EndRegion

                // Region   Distance
                fixed   disX        =   (fragData.uv.x + fragData.uv.y - _DissolveScale) / 2;
                fixed   disY        =   disX + _DissolveScale;
                fixed   dis         =   min(max(pow(pow(disX - fragData.uv.x, 2) + pow(disY - fragData.uv.y, 2), 0.5), 0), 1);
                // EndRegion

                // Region Proportion Change
                fixed   scale       =   min(max(dis / _GradualChangeScale, 0), 1);
                // endColor            =   fixed4(endColor.rgb, endColor.a * scale);
                fixed   clipValue   =   dissolveColor.r - (1 - scale);
                clip(clipValue);
                // EndRegion

                // Region Edge Light
                fixed   value       =   pow(1 - scale, 2) * ceil(endColor.a) * max((_EdgeWidth - clipValue), 0) / _EdgeWidth;
                endColor            =   _EdgeColor * value + endColor * (1 - value);
                // EndRegion

                return endColor;
            }
            ENDCG
        }
    }
}
