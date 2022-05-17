Shader "Unlit/userDrawUnlit"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "black" {}
        _BrushRadius ("Brush Radius", float) = 2
        _Coordinate ("Coordinate", Vector) = (0,0,0,0)
        _Color ("Draw Color", Color) = (1,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Cull Off

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha 

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
            float4 _MainTex_ST;

            float _BrushRadius;
            fixed4 _Coordinate, _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float draw = pow(saturate(1 - distance(i.uv, _Coordinate.xy)), 100);
                return draw > 1/_BrushRadius ? _Color : col;

                // return fixed4(col.xyz, mask);
            }
            ENDCG
        }
    }
}
