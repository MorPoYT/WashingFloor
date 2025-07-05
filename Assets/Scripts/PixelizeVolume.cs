using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Post-processing/Pixelize")]
public class PixelizeVolume : VolumeComponent, IPostProcessComponent
{
    [Tooltip("Pixel size (higher values = bigger pixels)")]
    public ClampedIntParameter pixelSize = new ClampedIntParameter(64, 1, 256);

    public bool IsActive() => pixelSize.value > 1 && active;
    public bool IsTileCompatible() => true;
}