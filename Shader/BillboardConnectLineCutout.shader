Shader "InfoViewShader/BillboardConnectLine Cutout"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color   ("Color",  Color) = (1, 0, 0, 1)
        _Cutoff  ("Cutoff", Float) = 0.5
        _OffsetX ("End X", Float) = 1
        _OffsetY ("End Y", Float) = 1
        _LineWidth ("Line Width", Float) = 0.01
        [Header(Hide By Distance)]
        [Toggle(_HIDE_BY_DISTANCE)]
        _HideByDistance ("Hide By Distance", Float) = 0
        _HideDistance ("Hide Distance", Float) = 10
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
		__ZTest("Z Test", Float) = 8 // Always
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
        Tags { "RenderType"="TransparentCutout" "Queue"="Transparent-2" "VRCFallback"="Hidden" }
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
            #include "./BillboardConnectLine.cginc"
            #define _CUTOFF_COLOR
            #include "./Vert.cginc"
            ENDCG
        }
    }
}
