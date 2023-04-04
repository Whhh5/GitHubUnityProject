Shader "Custom/AreaShader"
{
    Properties
    {
        _MainTex        ("MainTex",         2D)             =   "white" {}
        _Coefficient    ("Coefficient",     Int)            =   20
        _Radius         ("Radius",          Range(0, 1))    =   0.15

        _EdgeTex        ("EdgeTex",         2D)             =   "white" {}





    }
    SubShader
    {
		Tags { "Queue" = "ALphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "DisableBatching" = "True" }

        Pass
        {
			Tags{"LightMode"="ForwardBase"}
			Cull Off
            // Blend SrcAlpha OneMinusSrcAlpha
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

            sampler2D   _MainTex;
            float4      _MainTex_TexelSize;
            float4      _MainTex_ST;
            sampler2D   _EdgeTex;
            float4      _EdgeTex_TexelSize;



            float4      _ArrPoint[50];
            int         _ArrLength;
            



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex = UnityObjectToViewPos(v.vertex);
                // o.uv = v.uv;
                
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv);


                // 区域判断
                float2 vertex = _MainTex_TexelSize.zw * i.uv; 
                int rangeNum = 0;
                int centerNum = 0;
                // float rangePoint = 3;
                i.uv.x *= _ScreenParams.x / _ScreenParams.y;
                for(int k = 0; k < _ArrLength; k++)
                {
                    float4 pos1 = _ArrPoint[k];
                    pos1.x *= _ScreenParams.x / _ScreenParams.y;
                    for(int h = k; h < _ArrLength; h++)
                    {
                        float4 pos2 = _ArrPoint[h];
                        pos2.x *= _ScreenParams.x / _ScreenParams.y;
                        float dis = pos2.z + pos1.z;
                        float dim = pos2.w + pos1.w;

                        // float dis = 0.3;
                        // float dim = 0.1;
                        fixed4 col = fixed4(0, 0, 1, 1);

				        float temp;
				        for(int j = -3; j <= 3; j++)
                        {
				        	for(int k = -3; k <= 3; k++)
                            {
				        		float dis1 = distance(pos1.xy, float2(j * dim + i.uv.x, k * dim + i.uv.y));
				        		float dis2 = distance(pos2.xy, float2(j * dim + i.uv.x, k * dim + i.uv.y));
				        		temp += min(dis1, dis2);
				        	}
				        }

				        temp /= 49;//pow((3 * 2 + 1), 2);
				        if(temp < dis)
                        {
				        	return col * floor(temp * 50) * 0.3;
				        }
				        // clip(dis - temp);
                    }
                }



				return fixed4(0, 0, 0, 1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
