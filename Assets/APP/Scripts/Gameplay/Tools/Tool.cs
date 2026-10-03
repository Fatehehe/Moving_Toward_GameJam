using Modules.SoundSystems;
using UnityEngine;

public enum SoundType
{
    Continuous,
    Once
}

public abstract class Tool : MonoBehaviour, IInteractable, IToolObject, IPressable
{
    protected bool isUsed;
    private int currentAudioId = -1;

    [Header("Audio settings")]
    [SerializeField] protected AudioKey audioKey;
    [SerializeField] protected SoundType soundType;

    [Header("Visual Settings")]
    [SerializeField] protected Renderer toolRenderer; // Komponen MeshRenderer atau SkinnedMeshRenderer
    [SerializeField] protected Material materialA;    // Material saat tidak digunakan (Idle/Return)
    [SerializeField] protected Material materialB;    // Material saat digunakan (Use)

    public bool IsUsed => isUsed;

    private void OnEnable()
    {
        // TutorialService.OnTutorialHighlightOn += HandleTutorialHighlightOn;
        // TutorialService.OnTutorialHighlightOff += HandleTutorialHighlightOff;
    }

    private void OnDisable()
    {
        // TutorialService.OnTutorialHighlightOn -= HandleTutorialHighlightOn;
        // TutorialService.OnTutorialHighlightOff -= HandleTutorialHighlightOff;
    }

    //IInteractable
    public void OnInteractDetected() => InteractDetected();
    public void OnInteractEnded() => InteractEnded();

    //IPressable
    public void OnHoldCompleted() { }
    public void OnHoldCanceled() { }
    public void OnPressStarted() => PressStarted();
    public void OnPressEnded() => PressEnded();

    //IToolObject
    public void PlaySfx(bool isPlaying)
    {
        if (soundType == SoundType.Once)
        {
            if (isPlaying)
            {
                SoundSystem.Instance.PlayAudio(audioKey, 1f, false, true, false);
            }
        }
        else
        {
            if (isPlaying)
            {
                bool isAlreadyPlaying = false;
                if (currentAudioId != -1)
                {
                    Audio activeAudio = SoundSystem.Instance.GetAudio(currentAudioId);
                    if (activeAudio != null && activeAudio.IsPlaying)
                    {
                        isAlreadyPlaying = true;
                    }
                }

                if (!isAlreadyPlaying)
                {
                    currentAudioId = SoundSystem.Instance.PlayAudio(audioKey, 1f, true, true, false);
                }
            }
            else
            {
                if (currentAudioId != -1)
                {
                    SoundSystem.Instance.StopAudio(currentAudioId);
                    currentAudioId = -1;
                }
            }
        }
    }

    public void PlayVfx(bool isPlaying) => ToolVFX(isPlaying);

    public void Use()
    {
        isUsed = true;
        ChangeMaterial(materialB); // Ganti ke Material B saat digunakan
    }

    public void Return()
    {
        isUsed = false;
        ChangeMaterial(materialA); // Kembali ke Material A saat dilepas
    }

    // Fungsi helper untuk mengganti material
    private void ChangeMaterial(Material targetMaterial)
    {
        if (toolRenderer != null && targetMaterial != null)
        {
            toolRenderer.material = targetMaterial;
        }
    }

    // abstract class
    protected abstract void InteractDetected();
    protected abstract void InteractEnded();
    protected abstract void PressStarted();
    protected abstract void PressEnded();
    protected abstract void ToolVFX(bool isPlaying);
}