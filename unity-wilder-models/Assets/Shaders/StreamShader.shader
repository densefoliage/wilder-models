Shader "Custom/StreamShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _TimeFactor ("Time Factor", Range(0, 2)) = 1
        _X_Scale ("X Scale", Range(0, 1)) = 0.05
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
        #pragma surface surf Standard alpha
        // #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _TimeFactor;
        half _X_Scale;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float2 uv = IN.uv_MainTex;
			uv.x = uv.x * _X_Scale + (_Time.y * _TimeFactor * 0.05);
			uv.y -= _Time.y * _TimeFactor;
			float4 noise = tex2D(_MainTex, uv);
			
			float2 uv2 = IN.uv_MainTex;
			uv2.x = uv2.x * _X_Scale - (_TimeFactor * 0.95 * 0.05);
			uv2.y -= _Time.y * (_TimeFactor * 0.95);
			float4 noise2 = tex2D(_MainTex, uv2);
			
			fixed4 c = saturate(_Color + noise.r * noise2.a);
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
