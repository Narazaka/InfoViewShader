Shader "InfoViewShader/BillboardWithOffset Cutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color   ("Color",  Color) = (1, 1, 1, 1)
        _Cutoff  ("Cutoff", Float) = 0.5
        _OffsetX ("OffsetX", Float) = 1
        _OffsetY ("OffsetY", Float) = 1
        _ScaleX ("ScaleX", Float) = 1
        _ScaleY ("ScaleY", Float) = 1
        [Header(Hide By Distance)]
        [Toggle(_HIDE_BY_DISTANCE)]
        _HideByDistance ("Hide By Distance", Float) = 0
        _Hide_Distance ("Hide Distance", Float) = 10
        [Header(Z Write)]
        [Space]
        [Toggle]
        __ZWrite ("Z Write", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)]
		__ZTest("Z Test", Float) = 4 // LEqual
        [Header(Stencil)]
        [Space]
        __Ref ("Ref", Float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]
        __Comp ("Comp", Float) = 8 // Always
        [Enum(UnityEngine.Rendering.StencilOp)]
        __Pass ("Pass", Float) = 0 // Keep
        __ReadMask ("Read Mask", Float) = 255
        __WriteMask ("Write Mask", Float) = 255
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="Transparent-1" "VRCFallback"="Hidden" }
        LOD 100

        ZWrite [__ZWrite]
        ZTest [__ZTest]
        
        Stencil {
            Ref [__Ref]
            Comp [__Comp]
            Pass [__Pass]
            ReadMask [__ReadMask]
            WriteMask [__WriteMask]
        }

        Pass
        {
            CGPROGRAM
            #include "./BillboardWithOffset.cginc"
            #define _MIRROR_FLIP
            #define _CUTOFF_COLOR
            #include "./Vert.cginc"
            ENDCG
        }
    }
}
