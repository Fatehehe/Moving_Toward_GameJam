using Modules;
using UnityEngine;

public class ToolBrush : Tool, IToolBrush
{
    [Header("Brush Setting (Optional)")]
    [SerializeField] private Texture2D brushTexture;
    [Range(0, 1)][SerializeField] private float brushScale = 0.5f;
    [Range(0, 1)][SerializeField] private float brushStrength = 0.5f;
    [SerializeField] private Color paintingColor = Color.white;
    [SerializeField] private float brushDepth = 0.03f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem smokeVFX;
    [SerializeField] private ParticleSystem dustVFX;


    void Awake()
    {
        SetVfxEmission(0f, 0f);
    }

    private void Update()
    {

    }

    protected override void InteractDetected() { }
    protected override void InteractEnded() { }
    protected override void PressEnded() { }
    protected override void PressStarted() { }

    private void SetVfxEmission(float smokeRate, float dustRate)
    {
        if (smokeVFX != null)
        {
            var smokeEmission = smokeVFX.emission;
            smokeEmission.rateOverTime = smokeRate;
        }

        if (dustVFX != null)
        {
            var dustEmission = dustVFX.emission;
            dustEmission.rateOverTime = dustRate;
        }
    }

    protected override void ToolVFX(bool isPlaying)
    {
        if (isPlaying)
        {
            SetVfxEmission(2f, 8f);
        }
        else
        {
            SetVfxEmission(0f, 0f);
        }
    }

    public Texture2D GetBrush => brushTexture;
    public float BrushScale => brushScale;
    public float BrushStrength => brushStrength;
    public Color BrushColor => paintingColor;
    public Transform BrushTransform => transform;
    public float BrushDepth => brushDepth;
}