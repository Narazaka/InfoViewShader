Shader "InfoViewShader/BillboardWithOffset Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color   ("Color",  Color) = (1, 1, 1, 1)
        _Cutoff  ("Cutoff", Float) = 0.0
        _OffsetX ("OffsetX", Float) = 1
        _OffsetY ("OffsetY", Float) = 1
        _ScaleX ("ScaleX", Float) = 1
        _ScaleY ("ScaleY", Float) = 1
        [Header(Z Write)]
        [Space]
        [Toggle]
        __ZWrite ("Z Write", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)]
		__ZTest("Z Test", Float) = 4 // LEqual
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
        Tags { "Queue"="Transparent-1" "IgnoreProjector"="True" "RenderType"="Transparent" "VRCFallback"="Hidden" }
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

            #include "./BillboardWithOffset.cginc"
            #include "./MirrorFlip.cginc"

            float4 _Color;
            float _Cutoff;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, mirrorFlip(i.uv)) * _Color;
                clip(col.a - _Cutoff);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
