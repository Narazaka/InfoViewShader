using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Narazaka.Unity.InfoViewShader.Editor
{
    public static class InfoViewGenerator
    {
        static Shader _plateShader;
        static Shader plateShader => FetchShader(ref _plateShader, "InfoViewShader/BillboardWithOffset");
        static Shader _plateCutoutShader;
        static Shader plateCutoutShader => FetchShader(ref _plateCutoutShader, "InfoViewShader/BillboardWithOffset Cutout");
        static Shader _plateTransparentShader;
        static Shader plateTransparentShader => FetchShader(ref _plateTransparentShader, "InfoViewShader/BillboardWithOffset Transparent");
        static Shader _lineShader;
        static Shader lineShader => FetchShader(ref _lineShader, "InfoViewShader/BillboardConnectLine");
        static Shader _lineCutoutShader;
        static Shader lineCutoutShader => FetchShader(ref _lineCutoutShader, "InfoViewShader/BillboardConnectLine Cutout");
        static Shader _lineTransparentShader;
        static Shader lineTransparentShader => FetchShader(ref _lineTransparentShader, "InfoViewShader/BillboardConnectLine Transparent");
        static Shader FetchShader(ref Shader shader, string name) => shader == null ? shader = Shader.Find(name) : shader;

        static Shader PlateShader(InfoView.ShaderSetting.ShaderType shaderType)
        {
            switch (shaderType)
            {
                case InfoView.ShaderSetting.ShaderType.Opaque:
                    return plateShader;
                case InfoView.ShaderSetting.ShaderType.Cutout:
                    return plateCutoutShader;
                case InfoView.ShaderSetting.ShaderType.Transparent:
                    return plateTransparentShader;
                default:
                    return null;
            }
        }

        static Shader LineShader(InfoView.ShaderSetting.ShaderType shaderType)
        {
            switch (shaderType)
            {
                case InfoView.ShaderSetting.ShaderType.Opaque:
                    return lineShader;
                case InfoView.ShaderSetting.ShaderType.Cutout:
                    return lineCutoutShader;
                case InfoView.ShaderSetting.ShaderType.Transparent:
                    return lineTransparentShader;
                default:
                    return null;
            }
        }

        public static Material[] GenerateMaterials(InfoView infoView)
        {
            var (plateShaderSetting, lineShaderSetting) = infoView.effectiveShaderSettingPair;
            var plateShader = PlateShader(plateShaderSetting.shaderType);
            var plate = new Material(plateShader);
            plate.name = $"InfoView_{infoView.name}_Plate";
            plate.SetTexture("_MainTex", infoView.mainTex);
            plate.SetColor("_Color", infoView.color);
            plate.SetFloat("_Cutoff", infoView.cutoff);
            plate.SetFloat("_OffsetX", infoView.offset.x);
            plate.SetFloat("_OffsetY", infoView.offset.y);
            plate.SetFloat("_ScaleX", infoView.scale.x);
            plate.SetFloat("_ScaleY", infoView.scale.y);
            plate.SetFloat("_HideByDistance", infoView.hideByDistance ? 1 : 0);
            plate.SetKeyword(new UnityEngine.Rendering.LocalKeyword(plateShader, "_HIDE_BY_DISTANCE"), infoView.hideByDistance);
            plate.SetFloat("_HideDistance", infoView.hideDistance);
            plate.SetFloat("_HideDistanceFadeArea", infoView.hideDistanceFadeArea);
            plate.SetFloat("_HideInLocal", infoView.hideInLocal ? 1 : 0);
            plate.SetKeyword(new UnityEngine.Rendering.LocalKeyword(plateShader, "_HIDE_IN_LOCAL"), infoView.hideInLocal);
            plate.SetFloat("_ShowInLocalHandCamera", infoView.showInLocalHandCamera ? 1 : 0);
            plate.enableInstancing = infoView.gpuInstancing;
            SetShaderSetting(plateShaderSetting, plate);

            var lineShader = LineShader(lineShaderSetting.shaderType);
            var line = new Material(lineShader);
            line.name = $"InfoView_{infoView.name}_Line";
            line.SetTexture("_MainTex", infoView.lineMainTex);
            line.SetColor("_Color", infoView.lineColor);
            line.SetFloat("_Cutoff", infoView.lineCutoff);
            line.SetFloat("_OffsetX", infoView.offset.x);
            line.SetFloat("_OffsetY", infoView.offset.y);
            line.SetFloat("_LineWidth", infoView.lineWidth);
            line.SetFloat("_HideByDistance", infoView.hideByDistance ? 1 : 0);
            line.SetKeyword(new UnityEngine.Rendering.LocalKeyword(lineShader, "_HIDE_BY_DISTANCE"), infoView.hideByDistance);
            line.SetFloat("_HideDistance", infoView.hideDistance);
            line.SetFloat("_HideDistanceFadeArea", infoView.hideDistanceFadeArea);
            line.SetFloat("_HideInLocal", infoView.hideInLocal ? 1 : 0);
            line.SetKeyword(new UnityEngine.Rendering.LocalKeyword(lineShader, "_HIDE_IN_LOCAL"), infoView.hideInLocal);
            line.SetFloat("_ShowInLocalHandCamera", infoView.showInLocalHandCamera ? 1 : 0);
            line.enableInstancing = infoView.gpuInstancing;
            SetShaderSetting(lineShaderSetting, line);

            return new Material[] { plate, line };
        }

        static void SetShaderSetting(InfoView.ShaderSetting shaderSetting, Material material)
        {
            material.SetInt("__ZWrite", shaderSetting.zWrite ? 1 : 0);
            material.SetInt("__ZTest", (int)shaderSetting.zTest);
            material.SetInt("__SrcBlend", (int)shaderSetting.srcBlend);
            material.SetInt("__DstBlend", (int)shaderSetting.dstBlend);
            material.SetInt("__Ref", shaderSetting.stencilRef);
            material.SetInt("__Comp", (int)shaderSetting.stencilComp);
            material.SetInt("__Pass", (int)shaderSetting.stencilPass);
            material.SetInt("__ReadMask", shaderSetting.stencilReadMask);
            material.SetInt("__WriteMask", shaderSetting.stencilWriteMask);
            material.renderQueue = shaderSetting.renderQueue;
        }

        [MenuItem("GameObject/InfoViewShader/Create InfoView")]
        public static void CreateInfoView(MenuCommand menuCommand)
        {
            var parent = menuCommand.context is GameObject ? menuCommand.context as GameObject : null;
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (parent == null)
            {
                go.name = "InfoView";
            }
            else
            {
                go.name = GameObjectUtility.GetUniqueNameForSibling(parent.transform, "InfoView");
                go.transform.SetParent(parent.transform, false);
            }
            go.transform.localScale = new Vector3(2, 2, 2);
            Object.DestroyImmediate(go.GetComponent<BoxCollider>());
            var meshRenderer = go.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new Material[0];
            var infoView = go.AddComponent<InfoView>();
            Undo.RegisterCreatedObjectUndo(go, "Create InfoView");
            EditorGUIUtility.PingObject(go);
        }
    }
}
