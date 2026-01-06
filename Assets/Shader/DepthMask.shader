Shader "Custom/SwordPortalClip"
{
    Properties {
 _Color ("Main Color", Color) = (1.000000,1.000000,1.000000,1.000000)
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
}
    SubShader
    {
        Tags{ "RenderType"="Transparent" "Queue"="Transparent"}

        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;

        // 포탈 평면 정보
        float3 _PlanePos;
        float3 _PlaneNormal;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;   
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            float3 worldPos = IN.worldPos;
            float d = dot(worldPos - _PlanePos, _PlaneNormal);
            // d < 0 이면 픽셀 버리기 (뒤쪽 면)
            clip(d);

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgba;
            o.Alpha  = 0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
