Shader "Unlit/ShadeCorrectShader"
{
    Properties
    {
        _Height             ("Height",          Float)              =   1.0
        _LightCorrect       ("LightCorrect",    Range(0, 255))      =   1.0
        _ShadeCorrect       ("ShadeCorrect",    Range(0, 255))      =   1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"

            float _Height;
            float _LightCorrect;
            float _ShadeCorrect;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed2 uv       :   TEXCOORD0;
                fixed2 uv2      :   TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex   :   SV_POSITION;
                float4 lPos     :   TEXCOORD0;
                fixed2 uv       :   TEXCOORD1;
                fixed2 uv2      :   TEXCOORD2;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.lPos              =   v.vertex;
                o.uv                =   v.uv;
                o.uv2               =   v.uv2;
                o.vertex            =   UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float lHeight = (i.uv2.x + 1) * 0.5;    // 0 - 1
                fixed4 col =  lHeight;
                return col;
            }
            ENDCG
        }
    }
}
