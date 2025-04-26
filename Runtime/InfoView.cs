using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Narazaka.Unity.InfoViewShader
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class InfoView : MonoBehaviour
#if NARAZAKA_INFOVIEWSHADER_HAS_NDMF_1_7
        , nadena.dev.ndmf.INDMFEditorOnly
#elif VRC_SDK_VRCSDK3
        , VRC.SDKBase.IEditorOnly
#endif
    {
        public Texture2D mainTex;
        public Color color = Color.white;
        public float cutoff = 0.5f;
        public Texture2D lineMainTex;
        public Color lineColor = Color.red;
        public float lineCutoff = 0.5f;
        public Vector2 offset = Vector2.one;
        public Vector2 scale = Vector2.one;
        public float lineWidth = 0.01f;
        public bool hideByDistance;
        public float hideDistance = 10f;
        public float hideDistanceFadeArea = 1f;
        public bool hideInLocal = false;
        public bool showInLocalHandCamera = true;
        public RenderSetting renderSetting = new RenderSetting();
        public ShaderSetting shaderSetting = new ShaderSetting { renderQueue = 2999 };
        public ShaderSetting lineShaderSetting = new ShaderSetting { renderQueue = 2998 };
        public bool gpuInstancing = false;

        public (ShaderSetting plate, ShaderSetting line) effectiveShaderSettingPair
        {
            get
            {
                var pair = renderSetting.GetShaderSettingPair();
                if (pair == null)
                {
                    return (shaderSetting, lineShaderSetting);
                }
                return pair.Value;
            }
        }

        [Serializable]
        public class ShaderSetting
        {
            public enum ShaderType
            {
                Opaque,
                Cutout,
                Transparent,
            }

            public ShaderType shaderType = ShaderType.Opaque;
            public bool zWrite = true;
            public CompareFunction zTest = CompareFunction.LessEqual;
            public BlendMode srcBlend = BlendMode.SrcAlpha;
            public BlendMode dstBlend = BlendMode.OneMinusSrcAlpha;
            public int stencilRef = 0;
            public CompareFunction stencilComp = CompareFunction.Always;
            public StencilOp stencilPass = StencilOp.Keep;
            public int stencilReadMask = 255;
            public int stencilWriteMask = 255;
            public int renderQueue;
        }

        [Serializable]
        public class RenderSetting
        {
            public enum RenderSetupMode
            {
                Custom,
                Opaque,
                Transparent,
            }

            public RenderSetupMode renderSetupMode = RenderSetupMode.Opaque;
            public bool zTestAlways = true;
            public int baseRenderQueue = 2999;
            public bool maskRef = true;
            public int stencilRef = 128;

            public (ShaderSetting plate, ShaderSetting line)? GetShaderSettingPair()
            {
                if (renderSetupMode == RenderSetupMode.Custom)
                {
                    return null;
                }
                if (renderSetupMode == RenderSetupMode.Opaque && zTestAlways)
                {
                    return (new ShaderSetting
                    {
                        shaderType = ShaderSetting.ShaderType.Opaque,
                        zWrite = true,
                        zTest = CompareFunction.Always,
                        srcBlend = BlendMode.SrcAlpha,
                        dstBlend = BlendMode.OneMinusSrcAlpha,
                        stencilRef = 0,
                        stencilComp = CompareFunction.Always,
                        stencilPass = StencilOp.Keep,
                        stencilReadMask = 255,
                        stencilWriteMask = 255,
                        renderQueue = baseRenderQueue,
                    }, new ShaderSetting
                    {
                        shaderType = ShaderSetting.ShaderType.Opaque,
                        zWrite = true,
                        zTest = CompareFunction.Always,
                        srcBlend = BlendMode.SrcAlpha,
                        dstBlend = BlendMode.OneMinusSrcAlpha,
                        stencilRef = 0,
                        stencilComp = CompareFunction.Always,
                        stencilPass = StencilOp.Keep,
                        stencilReadMask = 255,
                        stencilWriteMask = 255,
                        renderQueue = baseRenderQueue - 1
                    });
                }
                var shaderType = renderSetupMode == RenderSetupMode.Opaque
                    ? ShaderSetting.ShaderType.Opaque
                    : ShaderSetting.ShaderType.Transparent;
                var zTest = zTestAlways ? CompareFunction.Always : CompareFunction.LessEqual;
                var stencilMask = maskRef ? stencilRef : 255;
                return (new ShaderSetting
                {
                    shaderType = shaderType,
                    zWrite = true,
                    zTest = zTest,
                    srcBlend = BlendMode.SrcAlpha,
                    dstBlend = BlendMode.OneMinusSrcAlpha,
                    stencilRef = stencilRef,
                    stencilComp = CompareFunction.Always,
                    stencilPass = StencilOp.Replace,
                    stencilReadMask = stencilMask,
                    stencilWriteMask = stencilMask,
                    renderQueue = baseRenderQueue - 1,
                }, new ShaderSetting
                {
                    shaderType = shaderType,
                    zWrite = true,
                    zTest = zTest,
                    srcBlend = BlendMode.SrcAlpha,
                    dstBlend = BlendMode.OneMinusSrcAlpha,
                    stencilRef = stencilRef,
                    stencilComp = CompareFunction.NotEqual,
                    stencilPass = StencilOp.Zero,
                    stencilReadMask = stencilMask,
                    stencilWriteMask = stencilMask,
                    renderQueue = baseRenderQueue,
                });
            }
        }
    }
}
