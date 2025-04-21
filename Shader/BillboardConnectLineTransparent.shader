Shader "InfoViewShader/BillboardConnectLine Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color   ("Color",  Color) = (1, 0, 0, 1)
        _Cutoff  ("Cutoff", Float) = 0.0
        _OffsetX ("End X", Float) = 1
        _OffsetY ("End Y", Float) = 1
        _LineWidth ("Line Width", Float) = 0.01
        [Header(Z Write)]
        [Space]
        [Toggle]
        __ZWrite ("Z Write", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)]
		__ZTest("Z Test", Float) = 8 // Always
		[Enum(UnityEngine.Rendering.BlendMode)]
		__SrcBlend("Src Blend", Float) = 5 // SrcAlpha
		[Enum(UnityEngine.Rendering.BlendMode)]
		__DstBlend("Dst Blend", Float) = 10 // OneMinusSrcAlpha
        [Header(Stencil)]
        [Space]
        __Ref ("Ref", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]
        __Comp ("Comp", Float) = 8 // Always
        [Enum(UnityEngine.Rendering.StencilOp)]
        __Pass ("Pass", Float) = 0 // Keep
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent-2" "VRCFallback"="Hidden" }
        LOD 100

        ZWrite [__ZWrite]
        ZTest [__ZTest]
        Blend [__SrcBlend] [__DstBlend]
        
        Stencil {
            Ref [__Ref]
            Comp [__Comp]
            Pass [__Pass]
        }

        Pass
        {
            CGPROGRAM
            #pragma fragment frag

            #include "./BillboardConnectLine.cginc"

            float4 _Color;
            float _Cutoff;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - _Cutoff);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
