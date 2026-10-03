using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button nextButton; // Hanya sisa button dan canvasGroup

    private Action onNextAction;

    private void Awake()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(OnNextButtonClicked);
        }
    }

    // Menerima parameter Action saja
    public void ShowDialogue(Action onNext)
    {
        gameObject.SetActive(true);
        onNextAction = onNext;

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.3f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void HideDialogue()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnNextButtonClicked()
    {
        nextButton.transform.DOKill();
        nextButton.transform.localScale = Vector3.one;
        nextButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        onNextAction?.Invoke();
    }
}