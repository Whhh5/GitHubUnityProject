Shader "Unlit/UIUnBLock" 
{
    Properties
    {
        // _MainTex            ("Main Tex",                2D)              =   "white" {}
        [HDR]_MainColor     ("Main Color",              Color)              =   (1, 1, 1, 1)
        _MainColorValue     ("Main Color Value",        Range(0, 1))        =   1
        _MainColorIndex     ("Main Color Index",        Range(0, 5))        =   1

        [HDR]_StarColor     ("Star Color",              Color)              =   (1, 1, 1, 1)
        _StarColorValue     ("Star Color Value",        Range(0, 1))        =   1
        _StarRandomPow      ("Star Random Pow",         Range(0, 50))       =   5
        _starRangeChange    ("Star Range Change",       Range(0, 1))        =   1
    
        [HDR]_CrossColor    ("Cross Color",             Color)              =   (0, 0.5, 0.5, 1)
        _CrossVlaue         ("Cross Value",             Range(0, 1))        =   1
        _CrossWidth         ("Cross Width",             Range(0, 20))       =   0.2
        _CrossColorValue    ("Cross Color Value",       Range(0, 10))       =   0.2
        _CrossAngle         ("Cross Angle",             Range(-180, 180))   =   45
        
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}// "RenderPipeline"="UniversalPipline"}

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // sampler2D _MainTex;
            // float4 _MainTex_ST;
            // sampler2D _StarTex;
            fixed4 _MainColor;
            fixed4 _StarColor;
            fixed4 _CrossColor;
            fixed _MainColorValue;
            fixed _StarColorValue;
            float _CrossWidth;
            float _CrossVlaue;
            fixed _CrossColorValue;
            fixed _MainColorIndex;
            float _CrossAngle;
            float _StarRandomPow;
            fixed _starRangeChange;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f fragData) : SV_Target
            {
                fixed2  localCentreUV   =   (fragData.uv - fixed2(0.5, 0.5)) * 2;

                fixed4  endColor        =   _MainColor;
                // Region Main Color
                fixed   toCentreDis     =   max(abs(pow(pow(fragData.uv.x - 0.5, 2) + pow(fragData.uv.y - 0.5, 2), 0.5)), 0);
                // endColor                =   _MainColorValue * (1 - endColor.a) + _MainColor;
                endColor                =   fixed4(endColor.rgb, pow(min(endColor.a * toCentreDis, 1), _MainColorIndex) * _MainColorValue);
                // EndRegion

                // Region Cross
                fixed4  crossColor      =   _CrossColor;
                fixed   minValue        =   min(min(abs(localCentreUV.x), abs(localCentreUV.y)), 1);
                fixed   maxValue        =   max(max(abs(localCentreUV.x), abs(localCentreUV.y)), 0);
                fixed   dis             =   pow(pow(localCentreUV.x, 2) + pow(localCentreUV.y, 2), 0.5);
                fixed   x               =   dis * cos(atan(localCentreUV.y / localCentreUV.x) + _CrossAngle / 180.0 * UNITY_PI);
                fixed   y               =   dis * sin(atan(localCentreUV.y / localCentreUV.x) + _CrossAngle / 180.0 * UNITY_PI);
                fixed   maxXY           =   abs(x) > abs(y) ? abs(x) : abs(y);
                fixed   minXY           =   abs(x) < abs(y) ? abs(x) : abs(y);
                crossColor              =   fixed4(crossColor.rgb, crossColor.a * pow((1 - (abs(x) + abs(y)) * 0.5), _CrossColorValue));
                float   lineDis         =   1 / (maxXY * _CrossWidth);
                float   colorScale      =   1 - max(min(minXY / lineDis, 1), 0);
                crossColor              =   fixed4(crossColor.rgb, crossColor.a * pow(colorScale, 3) * _CrossVlaue);
                // EndRegion

                // Region Star
                fixed4  starColor       =   fixed4(_StarColor.rgb, _StarColor.a * _StarColorValue);
                fixed   randomX         =   pow(frac(sin(dot(fragData.uv.xy, float2(12.9898, 78.233) + _starRangeChange)) * 43758.5453123), _StarRandomPow);
                starColor               =   starColor * randomX;
                // EndRegion

                fixed4  endColor2       =   starColor + crossColor * (1 - starColor.a);
                fixed4  endColor3       =   endColor2 + endColor * (1 - endColor2.a);


                return endColor3;
                // return starColor;
            }
            ENDCG
        }
    }
}
