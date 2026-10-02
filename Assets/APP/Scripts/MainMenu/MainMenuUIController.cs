using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DG.Tweening;

public class MainMenuUIController : BaseMenuController
{
    [Header("UI Containers (Gunakan CanvasGroup)")]
    [SerializeField] private CanvasGroup playButtonContainer;
    [SerializeField] private CanvasGroup creditsContainer;

    [Header("Buttons")]
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonCreditToggle;

    [Header("Credit Button Visuals")]
    [SerializeField] private Image creditButtonImage;
    [SerializeField] private Sprite creditIconA;
    [SerializeField] private Sprite creditIconB;

    [Header("Animation Settings")]
    [SerializeField] private float animDuration = 0.4f;

    [Header("Floating Animation")]
    [SerializeField] private RectTransform floatingPhotoRect; // Masukkan RectTransform foto/kertas di sini
    [SerializeField] private float floatHeight = 15f;         // Jarak naik turun
    [SerializeField] private float floatDuration = 2f;        // Durasi 1 siklus naik/turun
    [SerializeField] private float rotationAngle = 2f;        // Derajat kemiringan rotasi
    [SerializeField] private float rotationDuration = 2.5f;   // Dibuat beda dengan floatDuration agar gerakannya terasa organik

    private bool isCreditOpen = false;

    protected override void Awake()
    {
        base.Awake();

        if (playButtonContainer != null)
        {
            playButtonContainer.gameObject.SetActive(true);
            playButtonContainer.alpha = 1f;
            playButtonContainer.transform.localScale = Vector3.one;
            playButtonContainer.interactable = true;
            playButtonContainer.blocksRaycasts = true;
        }

        if (creditsContainer != null)
        {
            creditsContainer.gameObject.SetActive(false);
            creditsContainer.alpha = 0f;
            creditsContainer.transform.localScale = Vector3.one * 0.8f;
        }

        if (creditButtonImage != null && creditIconA != null)
        {
            creditButtonImage.sprite = creditIconA;
        }

        if (buttonStart != null) buttonStart.onClick.AddListener(OnButtonNewGameClick);
        if (buttonCreditToggle != null) buttonCreditToggle.onClick.AddListener(OnCreditToggleClick);

        StartFloatingAnimation();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (buttonStart != null) buttonStart.onClick.RemoveListener(OnButtonNewGameClick);
        if (buttonCreditToggle != null) buttonCreditToggle.onClick.RemoveListener(OnCreditToggleClick);

        // Wajib matikan tween saat object hancur (misal saat pindah scene)
        if (floatingPhotoRect != null)
        {
            floatingPhotoRect.DOKill();
        }
    }

    private void StartFloatingAnimation()
    {
        if (floatingPhotoRect == null) return;

        // Gerak naik turun secara looping
        float startY = floatingPhotoRect.anchoredPosition.y;
        floatingPhotoRect.DOAnchorPosY(startY + floatHeight, floatDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Gerak rotasi miring kiri-kanan tipis secara looping
        // Set kemiringan awal ke -rotationAngle agar putarannya sinkron dari kiri ke kanan
        floatingPhotoRect.localRotation = Quaternion.Euler(0, 0, -rotationAngle);
        floatingPhotoRect.DOLocalRotate(new Vector3(0, 0, rotationAngle), rotationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnButtonNewGameClick()
    {
        playButtonContainer.interactable = false;
        MainMenuEvents.TriggerNewGame();
    }

    private void OnCreditToggleClick()
    {
        buttonCreditToggle.interactable = false;

        buttonCreditToggle.transform.DOKill(true); // Hindari bentrok animasi button
        buttonCreditToggle.transform.localScale = Vector3.one;
        buttonCreditToggle.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f).OnComplete(() =>
        {
            buttonCreditToggle.interactable = true;
        });

        isCreditOpen = !isCreditOpen;

        if (isCreditOpen)
        {
            creditButtonImage.sprite = creditIconB;
            playButtonContainer.interactable = false;
            playButtonContainer.blocksRaycasts = false;
            playButtonContainer.transform.DOScale(0.8f, animDuration).SetEase(Ease.InBack);
            playButtonContainer.DOFade(0f, animDuration).OnComplete(() =>
            {
                playButtonContainer.gameObject.SetActive(false);
            });

            creditsContainer.gameObject.SetActive(true);
            creditsContainer.transform.DOScale(1f, animDuration).SetEase(Ease.OutBack);
            creditsContainer.DOFade(1f, animDuration).OnComplete(() =>
            {
                creditsContainer.interactable = true;
                creditsContainer.blocksRaycasts = true;
            });
        }
        else
        {
            creditButtonImage.sprite = creditIconA;
            creditsContainer.interactable = false;
            creditsContainer.blocksRaycasts = false;
            creditsContainer.transform.DOScale(0.8f, animDuration).SetEase(Ease.InBack);
            creditsContainer.DOFade(0f, animDuration).OnComplete(() =>
            {
                creditsContainer.gameObject.SetActive(false);
            });

            playButtonContainer.gameObject.SetActive(true);
            playButtonContainer.transform.DOScale(1f, animDuration).SetEase(Ease.OutBack);
            playButtonContainer.DOFade(1f, animDuration).OnComplete(() =>
            {
                playButtonContainer.interactable = true;
                playButtonContainer.blocksRaycasts = true;
            });
        }
    }
}