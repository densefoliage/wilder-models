Shader "Custom/EstuaryShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        // Next, switch the shader to transparent mode. 
        // We have to use shader tags to indicate this:
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        // Then add the alpha keyword to the #pragma surface line. 
        // While we're at it, we can remove the fullforwardshadows 
        // keyword, as we're not casting shadows anyway.
        #pragma surface surf Standard alpha vertex:vert
        // #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #include "Water.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 streamUV; // To prevent compile error when using uv and uv2
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.streamUV = v.texcoord1.xy;
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float shore = IN.uv_MainTex.y;
			float foam = Foam(shore, IN.worldPos.xz, _MainTex);
			float waves = Waves(IN.worldPos.xz, _MainTex);
			waves *= 1 - shore;
            float shoreWater = max(foam, waves);

            float stream = Stream(IN.streamUV, _MainTex);

            float water = lerp(shoreWater, stream, IN.uv_MainTex.x);

            fixed4 c = saturate(_Color + water);
			// fixed4 c = saturate(_Color + max(foam, waves));
            // fixed4 c = saturate(_Color + stream);
            // fixed4 c = fixed4(IN.uv2_MainTex, 1, 1);
			o.Albedo = c.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
