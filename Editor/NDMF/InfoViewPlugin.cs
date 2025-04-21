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
