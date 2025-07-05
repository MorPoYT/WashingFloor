using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizeRenderPass : ScriptableRenderPass
{
    private Material _material;
    private PixelizeVolume _volume;
    private static readonly int PixelSizeProperty = Shader.PropertyToID("_PixelSize");

    public PixelizeRenderPass(Shader shader)
    {
        if (shader == null)
        {
            Debug.LogError("Pixelize shader is missing!");
            return;
        }
        
        _material = CoreUtils.CreateEngineMaterial(shader);
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (_material == null) return;

        var stack = VolumeManager.instance.stack;
        _volume = stack.GetComponent<PixelizeVolume>();
        if (_volume == null || !_volume.IsActive()) return;

        CommandBuffer cmd = CommandBufferPool.Get("PixelizeEffect");
        cmd.SetGlobalFloat(PixelSizeProperty, _volume.pixelSize.value);

        var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
        Blitter.BlitCameraTexture(cmd, source, source, _material, 0);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
        CoreUtils.Destroy(_material);
    }
}