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
        [Header(Hide By Distance)]
        [Toggle(_HIDE_BY_DISTANCE)]
        _HideByDistance ("Hide By Distance", Float) = 0
        _HideDistance ("Hide Distance", Float) = 10
        _HideDistanceFadeArea ("Hide Distance Fade Area", Float) = 1
        [Header(Hide In Local)]
        [Toggle(_HIDE_IN_LOCAL)]
        _HideInLocal ("Hide In Local", Float) = 0
        _ShowInLocalHandCamera ("Show In Local Hand Camera", Float) = 1
        _IsLocal ("Is Local (set by anim etc.)", Float) = 0
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
        __ReadMask ("Read Mask", Float) = 255
        __WriteMask ("Write Mask", Float) = 255
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
            ReadMask [__ReadMask]
            WriteMask [__WriteMask]
        }

        Pass
        {
            CGPROGRAM
            #include "./BillboardWithOffset.cginc"
            #define _HIDE_DISTANCE_FADE_AREA
            #define _MIRROR_FLIP
            #define _CUTOFF_COLOR
            #include "./Vert.cginc"
            ENDCG
        }
    }
}
