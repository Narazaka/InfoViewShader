using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using nadena.dev.ndmf;
using nadena.dev.ndmf.preview;
using System.Collections.Immutable;
using System.Threading.Tasks;

[assembly: ExportsPlugin(typeof(Narazaka.Unity.InfoViewShader.Editor.InfoViewPlugin))]

namespace Narazaka.Unity.InfoViewShader.Editor
{
    internal class InfoViewPlugin : Plugin<InfoViewPlugin>
    {
        public override string DisplayName => "InfoViewShader";
        public override string QualifiedName => "net.narazaka.unity.info-view-shader";

        protected override void Configure()
        {
#if NARAZAKA_INFOVIEWSHADER_HAS_MA
            InPhase(BuildPhase.Generating).Run(DisplayName + " for MA", MAPass);
#endif
            InPhase(BuildPhase.Transforming).Run(DisplayName, Pass).PreviewingWith(new InfoViewPreview());
        }

        void Pass(BuildContext ctx)
        {
            var infoViews = ctx.AvatarRootObject.GetComponentsInChildren<InfoView>(true);
            foreach (var infoView in infoViews)
            {
                var renderer = infoView.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    throw new System.InvalidOperationException($"InfoView component [{infoView.name}] must be attached to a MeshRenderer.");
                }
                renderer.sharedMaterials = InfoViewGenerator.GenerateMaterials(infoView);
                Object.DestroyImmediate(infoView);
            }
        }

#if NARAZAKA_INFOVIEWSHADER_HAS_MA
        void MAPass(BuildContext ctx)
        {
            var infoViews = ctx.AvatarRootObject.GetComponentsInChildren<InfoView>(true);
            var hideInLocals = new List<MeshRenderer>();
            foreach (var infoView in infoViews)
            {
                var renderer = infoView.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    throw new System.InvalidOperationException($"InfoView component [{infoView.name}] must be attached to a MeshRenderer.");
                }
                if (infoView.hideInLocal)
                {
                    hideInLocals.Add(renderer);
                }
            }
            if (hideInLocals.Count > 0)
            {
                MakeHideInLocal(ctx.AvatarRootObject.transform, hideInLocals);
            }
        }

        void MakeHideInLocal(Transform avatarRoot, IList<MeshRenderer> meshRenderers)
        {
            var emptyClip = new AnimationClip { name = "InfoViewShader_Empty" };
            var hideClip = new AnimationClip { name = "InfoViewShader_HideInLocal" };
            foreach (var meshRenderer in meshRenderers)
            {
                var path = UnityEditor.AnimationUtility.CalculateTransformPath(meshRenderer.transform, avatarRoot);
                hideClip.SetCurve(path, typeof(MeshRenderer), "m_Enabled", new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0)));
            }
            var hideState = new UnityEditor.Animations.AnimatorState
            {
                hideFlags = HideFlags.HideInHierarchy,
                name = "Hide",
                motion = hideClip,
                transitions = new UnityEditor.Animations.AnimatorStateTransition[]
                {
                    new UnityEditor.Animations.AnimatorStateTransition
                    {
                        hasExitTime = false,
                        hasFixedDuration = true,
                        duration = 0,
                        exitTime = 0,
                        canTransitionToSelf = false,
                        isExit = true,
                        conditions = new UnityEditor.Animations.AnimatorCondition[]
                        {
                            new UnityEditor.Animations.AnimatorCondition
                            {
                                mode = UnityEditor.Animations.AnimatorConditionMode.IfNot,
                                parameter = "IsLocal",
                                threshold = 1,
                            }
                        }
                    },
                },
            };
            var idleState = new UnityEditor.Animations.AnimatorState
            {
                hideFlags = HideFlags.HideInHierarchy,
                name = "Idle",
                motion = emptyClip,
                transitions = new UnityEditor.Animations.AnimatorStateTransition[]
                {
                    new UnityEditor.Animations.AnimatorStateTransition
                    {
                        destinationState = hideState,
                        hasExitTime = false,
                        hasFixedDuration = true,
                        duration = 0,
                        exitTime = 0,
                        canTransitionToSelf = false,
                        conditions = new UnityEditor.Animations.AnimatorCondition[]
                        {
                            new UnityEditor.Animations.AnimatorCondition
                            {
                                mode = UnityEditor.Animations.AnimatorConditionMode.If,
                                parameter = "IsLocal",
                                threshold = 1,
                            }
                        }
                    },
                },
            };
            var controller = new UnityEditor.Animations.AnimatorController();
            controller.parameters = new AnimatorControllerParameter[]
            {
                new AnimatorControllerParameter
                {
                    name = "IsLocal",
                    type = AnimatorControllerParameterType.Bool,
                    defaultFloat = 0,
                    defaultInt = 0,
                    defaultBool = false,
                }
            };
            controller.layers = new UnityEditor.Animations.AnimatorControllerLayer[]
            {
                new UnityEditor.Animations.AnimatorControllerLayer
                {
                    name = "InfoViewShader_HideInLocal",
                    stateMachine = new UnityEditor.Animations.AnimatorStateMachine
                    {
                        name = "InfoViewShader_HideInLocal",
                        hideFlags = HideFlags.HideInHierarchy,
                        defaultState = idleState,
                        entryPosition = new Vector3(0, 0, 0),
                        exitPosition = new Vector3(0, 300, 0),
                        anyStatePosition = new Vector3(0, -100, 0),
                        states = new UnityEditor.Animations.ChildAnimatorState[]
                        {
                            new UnityEditor.Animations.ChildAnimatorState
                            {
                                state = idleState,
                                position = new Vector3(0, 100, 0),
                            },
                            new UnityEditor.Animations.ChildAnimatorState
                            {
                                state = hideState,
                                position = new Vector3(0, 200, 0),
                            },
                        },
                    }
                }
            };
            var mergeAnimator = avatarRoot.gameObject.AddComponent<nadena.dev.modular_avatar.core.ModularAvatarMergeAnimator>();
            mergeAnimator.layerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX;
            mergeAnimator.matchAvatarWriteDefaults = true;
            mergeAnimator.pathMode = nadena.dev.modular_avatar.core.MergeAnimatorPathMode.Absolute;
            mergeAnimator.layerPriority = int.MaxValue;
            mergeAnimator.animator = controller;
        }
#endif
    }

    internal class InfoViewPreview : IRenderFilter
    {
        public ImmutableList<RenderGroup> GetTargetGroups(ComputeContext context)
        {
            return context.GetComponentsByType<InfoView>()
                .Select(infoView => context.GetComponent<MeshRenderer>(infoView.gameObject))
                .Select(r => RenderGroup.For(r))
                .ToImmutableList();
        }

        public Task<IRenderFilterNode> Instantiate(RenderGroup group, IEnumerable<(Renderer, Renderer)> proxyPairs, ComputeContext context)
        {
            var infoView = group.Renderers.First().GetComponent<InfoView>();
            var targetRenderer = proxyPairs.First().Item2;
            var meshFilter = targetRenderer.GetComponent<MeshFilter>();
            context.Observe(infoView);
            context.Observe(meshFilter);
            return Task.FromResult<IRenderFilterNode>(new InfoViewRenderFilterNode(InfoViewGenerator.GenerateMaterials(infoView)));
        }
    }

    internal class InfoViewRenderFilterNode : IRenderFilterNode
    {
        Material[] materials;
        public RenderAspects WhatChanged => RenderAspects.Material;

        public InfoViewRenderFilterNode(Material[] materials)
        {
            this.materials = materials;
        }

        void IRenderFilterNode.OnFrame(Renderer original, Renderer proxy)
        {
            proxy.sharedMaterials = materials;
        }

        void System.IDisposable.Dispose()
        {
            foreach (var material in materials)
            {
                Object.DestroyImmediate(material);
            }
        }
    }
}
