// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/VerShader"
{
    Properties
    {
        _Maintex    ("Main Texture", 2D)    =   "white" {}
        _MaskBase   ("Mask Base", 2D)       =   "white" {}
        _MaskChid1  ("Mask Chid1", 2D)      =   "white" {}

        _MaskBaseSize   ("Mask Base Size", float)   =   1
        _Temp   ("Temp", float)   =   1
    }
    SubShader
    {
        Tags
        {
            "RenderPipline"             =   ""  // 1. UniversalRenderPipeline   2. HightDefinitionRenderPipeline
            "Queue"                     =   "Transparent"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            sampler2D _Maintex;
            sampler2D _MaskBase;
            sampler2D _MaskChid1;

            float _MaskBaseSize;
            float _Temp;

            struct appdata
            {
                float3 vertex       :   POSITION;
                fixed2 uv           :   TEXCOORD0;
            };
            struct v2f 
            {
                float4 vertex       :   SV_POSITION;
                fixed2 uv           :   TEXCOORD0;
            };

            v2f vert (appdata appdata)
            {
                v2f o;
                o.vertex            =   UnityObjectToClipPos(appdata.vertex);
                o.uv                =   appdata.uv; //(appdata.uv + (1, 1)) * 0.5;
                return o;
            }

            fixed4 frag (v2f o) : SV_Target
            {
                fixed4 color        =   (1,1,1,1);
                fixed2 uvValue      =   (o.uv * 2 - 1) * _MaskBaseSize;
                fixed4 color1       =   tex2D(_Maintex, o.uv);
                fixed4 color2       =   tex2D(_MaskChid1, uvValue);
                color               =   color1 * color2.a;

                
                fixed4 color3       =   tex2D(_MaskBase, o.uv);
                color               =   color3 - color;

                return color;
            }
            ENDCG
        }
    }
}