using UnityEngine;
using UnityEditor;

namespace Narazaka.Unity.InfoViewShader.Editor
{
    [CustomEditor(typeof(InfoView))]
    public class InfoViewShaderEditor : UnityEditor.Editor
    {
        SerializedProperty mainTex;
        SerializedProperty color;
        SerializedProperty cutoff;
        SerializedProperty lineMainTex;
        SerializedProperty lineCutoff;
        SerializedProperty lineColor;
        SerializedProperty offset;
        SerializedProperty scale;
        SerializedProperty lineWidth;
        SerializedProperty hideByDistance;
        SerializedProperty hideDistance;
        SerializedProperty hideDistanceFadeArea;
        SerializedProperty hideInLocal;
        SerializedProperty showInLocalHandCamera;
        RenderSettingGUI renderSetting;
        ShaderSettingGUI shaderSetting;
        ShaderSettingGUI lineShaderSetting;
        SerializedProperty gpuInstancing;

        Mesh cube;
        MeshFilter _meshFilter;
        MeshFilter meshFilter
        {
            get
            {
                if (_meshFilter == null)
                {
                    _meshFilter = (target as Component).GetComponent<MeshFilter>();
                }
                return _meshFilter;
            }
        }
        MeshRenderer _meshRenderer;
        MeshRenderer meshRenderer
        {
            get
            {
                if (_meshRenderer == null)
                {
                    _meshRenderer = (target as Component).GetComponent<MeshRenderer>();
                }
                return _meshRenderer;
            }
        }

        bool shaderSettingFoldout;

        void OnEnable()
        {
            cube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");

            mainTex = serializedObject.FindProperty(nameof(InfoView.mainTex));
            color = serializedObject.FindProperty(nameof(InfoView.color));
            cutoff = serializedObject.FindProperty(nameof(InfoView.cutoff));
            lineMainTex = serializedObject.FindProperty(nameof(InfoView.lineMainTex));
            lineColor = serializedObject.FindProperty(nameof(InfoView.lineColor));
            lineCutoff = serializedObject.FindProperty(nameof(InfoView.lineCutoff));
            offset = serializedObject.FindProperty(nameof(InfoView.offset));
            scale = serializedObject.FindProperty(nameof(InfoView.scale));
            lineWidth = serializedObject.FindProperty(nameof(InfoView.lineWidth));
            hideByDistance = serializedObject.FindProperty(nameof(InfoView.hideByDistance));
            hideDistance = serializedObject.FindProperty(nameof(InfoView.hideDistance));
            hideDistanceFadeArea = serializedObject.FindProperty(nameof(InfoView.hideDistanceFadeArea));
            hideInLocal = serializedObject.FindProperty(nameof(InfoView.hideInLocal));
            showInLocalHandCamera = serializedObject.FindProperty(nameof(InfoView.showInLocalHandCamera));
            renderSetting = new RenderSettingGUI(serializedObject.FindProperty(nameof(InfoView.renderSetting)));
            shaderSetting = new ShaderSettingGUI(serializedObject.FindProperty(nameof(InfoView.shaderSetting)));
            lineShaderSetting = new ShaderSettingGUI(serializedObject.FindProperty(nameof(InfoView.lineShaderSetting)));
            gpuInstancing = serializedObject.FindProperty(nameof(InfoView.gpuInstancing));
        }

        class T
        {
            public static istring RequireNDMF1_5 => new istring("Requires NDMF 1.5 or later", "動作のためにはNDMF 1.5以降が必要です");
            public static istring MeshIsNotCube => new istring("Mesh is not Cube!", "メッシュがCubeではありません！");
            public static istring SetCubeMesh => new istring("Set Cube mesh", "Cubeメッシュを設定");
            public static istring BoundSmall => new istring("The cube mesh behaves like a bounding box. Its scale might be small.", "Cubeメッシュはバウンディングボックスのようにふるまいます。スケールが小さいかもしれません。");
            public static istring SetScaleTo2 => new istring("Set Scale to 2", "スケールを2に設定");
            public static istring MaterialsShouldNone => new istring("Materials are already set. Please remove them to avoid interference with the thumbnail.", "マテリアルが設定されています。サムネイルの邪魔になる可能性があるので削除をおすすめします。");
            public static istring RemoveMaterials => new istring("Remove Materials", "マテリアルを削除");
            public static istring GeneralSettings => new istring("General Settings", "一般設定");
            public static istring Offset => new istring("Offset", "オフセット");
            public static istring OffsetDescription => new istring("The plate is displayed outward from the origin by the offset amount.", "オフセット分だけ原点から外側にプレートが表示されます");
            public static istring Scale => new istring("Scale", "スケール");
            public static istring ScaleDescription => new istring("Plate Scale", "プレートのスケール");
            public static istring LineWidth => new istring("Line Width", "ラインの幅");
            public static istring HideByDistance => new istring("Hide by Distance", "距離で非表示");
            public static istring HideDistance => new istring("Hide Distance", "非表示にする距離");
            public static istring HideDistanceFadeArea => new istring("Hide Distance Fade", "非表示距離フェード");
            public static istring VRChatSettings => new istring("VRChat Settings", "VRChat設定");
            public static istring HideInLocal => new istring("Hide in Local", "ローカルで非表示");
            public static istring ShowInLocalHandCamera => new istring("Show in Local Hand Camera", "ローカルハンドカメラで表示");
            public static istring Cutoff => new istring("Alpha Cutoff", "Alpha Cutoff");
            public static istring Plate => new istring("Plate", "プレート");
            public static istring Line => new istring("Line", "ライン");
            public static istring RenderSetting => new istring("Render Setting", "レンダリング設定");
            public static istring ShaderSetting => new istring("Shader Setting (Details)", "シェーダー設定 (詳細)");
            public static istring GPUInstancing => new istring("GPU Instancing", "GPU インスタンシング");
        }

        public override void OnInspectorGUI()
        {
#if !NARAZAKA_INFOVIEWSHADER_HAS_NDMF
            EditorGUILayout.HelpBox(T.RequireNDMF1_5, MessageType.Error);
#endif

            var component = target as Component;
            if (meshFilter != null && meshFilter.sharedMesh != cube)
            {
                EditorGUILayout.HelpBox(T.MeshIsNotCube, MessageType.Warning);
                if (GUILayout.Button(T.SetCubeMesh))
                {
                    Undo.RecordObject(meshFilter, T.SetCubeMesh);
                    meshFilter.sharedMesh = cube;
                }
            }
            var localScale = component.transform.localScale;
            if (localScale.x < 2 || localScale.y < 2 || localScale.z < 2)
            {
                EditorGUILayout.HelpBox(T.BoundSmall, MessageType.Warning);
                if (GUILayout.Button(T.SetScaleTo2))
                {
                    Undo.RecordObject(component.transform, T.SetScaleTo2);
                    component.transform.localScale = new Vector3(2, 2, 2);
                }
            }
            if (meshRenderer != null && meshRenderer.sharedMaterials.Length > 0)
            {
                EditorGUILayout.HelpBox(T.MaterialsShouldNone, MessageType.Warning);
                if (GUILayout.Button(T.RemoveMaterials))
                {
                    Undo.RecordObject(meshRenderer, T.RemoveMaterials);
                    meshRenderer.sharedMaterials = new Material[0];
                }
            }

            serializedObject.UpdateIfRequiredOrScript();

            DrawHeader(T.GeneralSettings);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(offset, T.Offset.GUIContent);
            EditorGUILayout.HelpBox(T.OffsetDescription, MessageType.Info);
            EditorGUILayout.PropertyField(scale, T.Scale.GUIContent);
            EditorGUILayout.HelpBox(T.ScaleDescription, MessageType.Info);
            EditorGUILayout.PropertyField(lineWidth, T.LineWidth.GUIContent);
            EditorGUILayout.PropertyField(hideByDistance, T.HideByDistance.GUIContent);
            if (hideByDistance.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(hideDistance, T.HideDistance.GUIContent);
                if (renderSetting.IsTransparentMode || shaderSetting.IsTransparent || lineShaderSetting.IsTransparent)
                {
                    EditorGUILayout.PropertyField(hideDistanceFadeArea, T.HideDistanceFadeArea.GUIContent);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;

            DrawHeader(T.Plate);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(mainTex);
            EditorGUILayout.PropertyField(color);
            if (shaderSetting.shaderType.enumValueIndex != (int)InfoView.ShaderSetting.ShaderType.Opaque)
            {
                EditorGUILayout.PropertyField(cutoff, T.Cutoff.GUIContent);
            }
            EditorGUI.indentLevel--;

            DrawHeader(T.Line);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(lineMainTex);
            EditorGUILayout.PropertyField(lineColor);
            if (lineShaderSetting.shaderType.enumValueIndex != (int)InfoView.ShaderSetting.ShaderType.Opaque)
            {
                EditorGUILayout.PropertyField(lineCutoff, T.Cutoff.GUIContent);
            }
            EditorGUI.indentLevel--;

#if NARAZAKA_INFOVIEWSHADER_HAS_MA
            DrawHeader(T.VRChatSettings);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(hideInLocal, T.HideInLocal.GUIContent);
            if (hideInLocal.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(showInLocalHandCamera, T.ShowInLocalHandCamera.GUIContent);
                EditorGUI.indentLevel--;
            }
            EditorGUI.indentLevel--;
#endif

            EditorGUILayout.Space();
            DrawHeader(T.RenderSetting);
            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            renderSetting.OnGUI();
            EditorGUILayout.PropertyField(gpuInstancing, T.GPUInstancing.GUIContent);
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                var settingPair = renderSetting.Get().GetShaderSettingPair();
                if (settingPair != null)
                {
                    shaderSetting.Set(settingPair.Value.plate);
                    lineShaderSetting.Set(settingPair.Value.line);
                }
            }
            EditorGUILayout.Space();
            shaderSettingFoldout = EditorGUILayout.Foldout(shaderSettingFoldout, T.ShaderSetting, true);
            if (shaderSettingFoldout)
            {
                EditorGUI.BeginDisabledGroup(!renderSetting.IsCustomMode);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(T.Plate, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                shaderSetting.OnGUI();
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField(T.Line, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                lineShaderSetting.OnGUI();
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            }
            serializedObject.ApplyModifiedProperties();

#if HAS_NDMF_LOCALIZATION
            nadena.dev.ndmf.ui.LanguageSwitcher.DrawImmediate();
#endif
        }

        void DrawHeader(string title)
        {
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f));
            var style = new GUIStyle(GUI.skin.box);
            style.normal.background = EditorGUIUtility.whiteTexture;
            var color = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin ? Color.black : Color.white;
            GUI.Box(rect, GUIContent.none, style);
            GUI.color = color;
            rect.xMin += 4;
            rect.xMax -= 4;
            style = EditorStyles.boldLabel;
            style.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            EditorGUI.LabelField(rect, title, style);
        }

        class RenderSettingGUI
        {
            internal SerializedProperty renderSetupMode;
            internal SerializedProperty zTestAlways;
            internal SerializedProperty baseRenderQueue;
            internal SerializedProperty maskRef;
            internal SerializedProperty stencilRef;

            public RenderSettingGUI(SerializedProperty renderSetting)
            {
                renderSetupMode = renderSetting.FindPropertyRelative(nameof(InfoView.RenderSetting.renderSetupMode));
                zTestAlways = renderSetting.FindPropertyRelative(nameof(InfoView.RenderSetting.zTestAlways));
                baseRenderQueue = renderSetting.FindPropertyRelative(nameof(InfoView.RenderSetting.baseRenderQueue));
                maskRef = renderSetting.FindPropertyRelative(nameof(InfoView.RenderSetting.maskRef));
                stencilRef = renderSetting.FindPropertyRelative(nameof(InfoView.RenderSetting.stencilRef));
            }

            public void OnGUI()
            {
                EditorGUILayout.PropertyField(renderSetupMode, T.RenderSetupMode.GUIContent);
                EditorGUILayout.HelpBox(T.RenderSetupModeDescription, MessageType.Info);
                if (IsCustomMode)
                {
                    return;
                }
                EditorGUILayout.PropertyField(zTestAlways, T.ZTestAlways.GUIContent);
                EditorGUILayout.HelpBox(T.ZTestAlwaysDescription, MessageType.Info);
                EditorGUILayout.PropertyField(baseRenderQueue, T.BaseRenderQueue.GUIContent);
                EditorGUILayout.HelpBox(T.BaseRenderQueueDescription, MessageType.Info);
                if (IsTransparentMode || !zTestAlways.boolValue)
                {
                    EditorGUILayout.PropertyField(maskRef, T.MaskRef.GUIContent);
                    EditorGUILayout.HelpBox(T.MaskRefDescription, MessageType.Info);
                    EditorGUILayout.PropertyField(stencilRef, T.StencilRef.GUIContent);
                    EditorGUILayout.HelpBox(T.StencilRefDescription, MessageType.Info);
                }
            }

            public InfoView.RenderSetting Get()
            {
                var renderSetting = new InfoView.RenderSetting
                {
                    renderSetupMode = (InfoView.RenderSetting.RenderSetupMode)renderSetupMode.enumValueIndex,
                    zTestAlways = zTestAlways.boolValue,
                    baseRenderQueue = baseRenderQueue.intValue,
                    maskRef = maskRef.boolValue,
                    stencilRef = stencilRef.intValue,
                };
                return renderSetting;
            }

            public bool IsTransparentMode => renderSetupMode.enumValueIndex == (int)InfoView.RenderSetting.RenderSetupMode.Transparent;
            public bool IsCustomMode => renderSetupMode.enumValueIndex == (int)InfoView.RenderSetting.RenderSetupMode.Custom;

            class T
            {
                public static istring RenderSetupMode => new istring("Render Setup Mode", "レンダリング設定モード");
                public static istring RenderSetupModeDescription => new istring("Custom = Manually configure all settings\nOpaque = Settings for opaque objects\nTransparent = Settings for transparent objects", "Custom=自分で全て設定\nOpaque=不透明用設定\nTransparent=半透明用設定");
                public static istring ZTestAlways => new istring("ZTest=Always", "ZTest=Always");
                public static istring ZTestAlwaysDescription => new istring("When enabled, the object will be rendered through meshes.", "オンにするとメッシュを貫通して表示されます");
                public static istring BaseRenderQueue => new istring("Base Render Queue", "ベースRender Queue");
                public static istring BaseRenderQueueDescription => new istring("This Render Queue and the one below it are used (e.g., if set to 2999, 2998 is also used).", "このRender Queueと1つ下のRender Queueが使われます（2999なら2998も）。");
                public static istring MaskRef => new istring("Masked Ref Comp", "マスクされたRef比較");
                public static istring MaskRefDescription => new istring("When enabled, the stencil Ref comparison uses a bitmask, making interference less likely.", "オンにするとステンシルのRefの比較がビットマスクを使用したものになり、干渉しにくくなります。");
                public static istring StencilRef => new istring("Stencil Ref", "ステンシルRef");
                public static istring StencilRefDescription => new istring("A non-zero value that is unlikely to conflict with others", "0以外で他とできるだけかぶらないような値");
            }
        }

        class ShaderSettingGUI
        {
            internal SerializedProperty shaderSetting;
            internal SerializedProperty shaderType;
            internal SerializedProperty zWrite;
            internal SerializedProperty zTest;
            internal SerializedProperty srcBlend;
            internal SerializedProperty dstBlend;
            internal SerializedProperty stencilRef;
            internal SerializedProperty stencilComp;
            internal SerializedProperty stencilPass;
            internal SerializedProperty stencilReadMask;
            internal SerializedProperty stencilWriteMask;
            internal SerializedProperty renderQueue;

            public ShaderSettingGUI(SerializedProperty shaderSetting)
            {
                this.shaderSetting = shaderSetting;
                shaderType = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.shaderType));
                zWrite = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.zWrite));
                zTest = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.zTest));
                srcBlend = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.srcBlend));
                dstBlend = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.dstBlend));
                stencilRef = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.stencilRef));
                stencilComp = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.stencilComp));
                stencilPass = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.stencilPass));
                stencilReadMask = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.stencilReadMask));
                stencilWriteMask = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.stencilWriteMask));
                renderQueue = shaderSetting.FindPropertyRelative(nameof(InfoView.ShaderSetting.renderQueue));
            }

            public void OnGUI()
            {
                EditorGUILayout.PropertyField(shaderType);
                EditorGUILayout.PropertyField(zWrite);
                EditorGUILayout.PropertyField(zTest);
                EditorGUILayout.PropertyField(srcBlend);
                EditorGUILayout.PropertyField(dstBlend);
                EditorGUILayout.PropertyField(stencilRef);
                if (stencilRef.intValue < 0)
                {
                    stencilRef.intValue = 0;
                }
                if (stencilRef.intValue > 255)
                {
                    stencilRef.intValue = 255;
                }
                EditorGUILayout.PropertyField(stencilComp);
                EditorGUILayout.PropertyField(stencilPass);
                EditorGUILayout.PropertyField(stencilReadMask);
                EditorGUILayout.PropertyField(stencilWriteMask);
                EditorGUILayout.PropertyField(renderQueue);
                if (renderQueue.intValue < -1)
                {
                    renderQueue.intValue = -1;
                }
                if (renderQueue.intValue > 5000)
                {
                    renderQueue.intValue = 5000;
                }
            }

            public void Set(InfoView.ShaderSetting shaderSetting)
            {
                shaderType.enumValueIndex = (int)shaderSetting.shaderType;
                zWrite.boolValue = shaderSetting.zWrite;
                zTest.enumValueIndex = (int)shaderSetting.zTest;
                srcBlend.enumValueIndex = (int)shaderSetting.srcBlend;
                dstBlend.enumValueIndex = (int)shaderSetting.dstBlend;
                stencilRef.intValue = shaderSetting.stencilRef;
                stencilComp.enumValueIndex = (int)shaderSetting.stencilComp;
                stencilPass.enumValueIndex = (int)shaderSetting.stencilPass;
                stencilReadMask.intValue = shaderSetting.stencilReadMask;
                stencilWriteMask.intValue = shaderSetting.stencilWriteMask;
                renderQueue.intValue = shaderSetting.renderQueue;
            }

            public bool IsTransparent => shaderType.enumValueIndex == (int)InfoView.ShaderSetting.ShaderType.Transparent;
        }
    }
}
