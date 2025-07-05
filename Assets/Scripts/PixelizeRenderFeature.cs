using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PixelizeRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader _pixelizeShader;
    private PixelizeRenderPass _renderPass;

    public override void Create()
    {
        if (_pixelizeShader == null)
        {
            _pixelizeShader = Shader.Find("Hidden/PixelizeEffect");
            if (_pixelizeShader == null)
            {
                Debug.LogError("Pixelize shader not found!");
                return;
            }
        }

        _renderPass = new PixelizeRenderPass(_pixelizeShader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_renderPass != null && renderingData.cameraData.postProcessEnabled)
        {
            renderer.EnqueuePass(_renderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
        _renderPass?.Dispose();
        _renderPass = null;
    }
}