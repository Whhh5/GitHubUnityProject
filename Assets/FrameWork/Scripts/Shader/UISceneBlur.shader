Shader "UIShader/UISceneBlur"
{
    Properties
    {
        _MainTex        ("MainTex",         2D)             =   "white" {}
        _Coefficient    ("Coefficient",     Int)            =   20
        _Radius         ("Radius",          Range(0, 1))    =   0.15
        _ShpereScaleY   ("Shpere Scale Y",  Range(0, 5))    =   2
        _StartBlurRadius("Start Blur Radius",Range(0, 1))   =   0

        // CS 动态改变的数据
        _HeroUVX        ("Hero UV X",       Range(0, 1))    =   0.5
        _HeroUVY        ("Hero UV Y",       Range(0, 1))    =   0.5
        //--
    }
    SubShader
    {
        Pass
        {
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            int _Coefficient;
            fixed _HeroUVX;
            fixed _HeroUVY;
            fixed _Radius;
            fixed _ShpereScaleY;
            fixed _StartBlurRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float   currPixelX      =   floor(i.uv.x * _MainTex_TexelSize.z);
                float   currOixelY      =   floor(i.uv.y * _MainTex_TexelSize.w);

                _Coefficient            =   min(max(_Coefficient, 0), min(_MainTex_TexelSize.z, _MainTex_TexelSize.w));

                float4  pixelColor      =   float4(0, 0, 0, 0);
                float   offsetX         =   0;
                float   offsetY         =   0;
                float   pixelX          =   0;
                float   pixelY          =   0;
                for(int k = -_Coefficient; k <= _Coefficient; k++)
                {
                    for(int g = -_Coefficient; g <= _Coefficient; g++)
                    {
                        offsetX         =   min(max(currPixelX + k, 0), _MainTex_TexelSize.z);
                        offsetY         =   min(max(currOixelY + g, 0), _MainTex_TexelSize.w);
                        pixelX          =   offsetX * _MainTex_TexelSize.x;
                        pixelY          =   offsetY * _MainTex_TexelSize.y;
                        pixelColor      +=  tex2D(_MainTex, fixed2(pixelX, pixelY));
                    }
                }
                fixed   heroUVX     =   min(max(_HeroUVX, 0), 1);
                fixed   heroUVY     =   min(max(_HeroUVY, 0), 1);


                fixed   disX        =   max(abs((i.uv.x - heroUVX)), 0);
                fixed   disY        =   max(abs(i.uv.y - heroUVY) * (_MainTex_TexelSize.w / _MainTex_TexelSize.z) , 0);
                float   lineK       =   disY / disX;
                fixed   dis         =   pow(pow(disX, 2) + pow(disY, 2), 0.5);

                fixed   lineMaxX    =   pow(pow(_ShpereScaleY, 2) * pow(_Radius, 2) / (pow(min(lineK, 100000), 2) + pow(_ShpereScaleY, 2)), 0.5);
                fixed   lineMaxY    =   lineK * lineMaxX;
                fixed   lineMaxDis  =   pow(pow(lineMaxX, 2) + pow(lineMaxY, 2), 0.5);

                fixed   blurValue   =   min(max((dis / lineMaxDis - _StartBlurRadius)/ (1 - _StartBlurRadius), 0), 1);

                // if(dis > lineMaxDis) {blurValue = 1;}

                col                 =   col * (1 - blurValue) + (pixelColor / (pow((_Coefficient * 2.0 + 1), 2))) * blurValue;
                
                return col;
            }
            ENDCG
        }
    }
}
