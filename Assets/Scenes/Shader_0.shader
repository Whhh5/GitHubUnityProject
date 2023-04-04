Shader "Custom/shader_0"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("_Color",color)=(0,0,1,1)
		_BackColor("_BackColor",color)=(1,1,1,1)
		pos0("Pos0",Vector)=(0.5,0.5,0.7,0.7)
		pos1("Pos1",Vector)=(0.9,0.9,0.7,0.7)
		_Dis("_Dis",float)=0.3
		_Dim("_Dim",Range(-0.1,0.1))=0.03
	}
	SubShader
	{
		Tags { "Queue" = "ALphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "DisableBatching" = "True" }
		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			Cull Off
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
			float4 _MainTex_ST;
			float4 _Color;
			float4 _BackColor;
			float4 pos0;
			float4 pos1;
			float _Dis;
			float _Dim;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			float4 frag (v2f i) : SV_Target
			{
				i.uv.x*=_ScreenParams.x/_ScreenParams.y;
				pos0.x*=_ScreenParams.x/_ScreenParams.y;
				pos1.x*=_ScreenParams.x/_ScreenParams.y;
				float temp;
				for(int j=-3; j<=3; j++)
                {
					for(int k=-3;k<=3;k++)
                    {
						float dis = distance(pos0.xy, float2(j * _Dim + i.uv.x, k * _Dim + i.uv.y));
						float dis2 = distance(pos1.xy, float2(j * _Dim + i.uv.x, k * _Dim + i.uv.y));
						temp += min(dis,dis2);
					}
				}
				temp /= 49;
				if(temp < _Dis){
					return _Color*floor(temp*50)*0.3;
				}
				clip(_Dis-temp);
				return _BackColor;
			}
			ENDCG
		}
	}
}

