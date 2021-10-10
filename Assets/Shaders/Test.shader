Shader "Custom/Test"
{
    Properties
    {
        [popup(AlphaBlend,10,Additive,1)] Dst ("混合模式", Float) = 10

        _MainTex ("Texture", 2D) = "white" {}

        //开关测试
        [Toggle][a]_Test02 ("test01", Float) = 0
        [follow(_Test02, 1)] _Test01 ("test01", Float) = 0

        //下拉框测试
        [popup(R,0,G,1,B,2,A,3)] aphlaChleen ("test03", Float) = 0

        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }
        LOD 100

        Pass
        {
            Blend SrcAlpha [Dst]

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
                return col * _Color;
            }
            ENDCG
        }
    }

    CustomEditor "CustomShaderEditor"
}
