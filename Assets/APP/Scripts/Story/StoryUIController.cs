using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryUIController : BaseMenuController
{
    [Header("Tutorial Pages")]
    [SerializeField] private GameObject[] tutorialPages;

    [Header("Navigation Buttons")]
    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;
    [SerializeField] private Button buttonFinish;
    public event Action OnTutorialFinished;

    private int currentPage = 0;

    protected override void Awake()
    {
        base.Awake();

        if (buttonLeft != null) buttonLeft.onClick.AddListener(PreviousPage);
        if (buttonRight != null) buttonRight.onClick.AddListener(NextPage);
        if (buttonFinish != null) buttonFinish.onClick.AddListener(FinishTutorial); // Listen ke tombol Finish
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (buttonLeft != null) buttonLeft.onClick.RemoveListener(PreviousPage);
        if (buttonRight != null) buttonRight.onClick.RemoveListener(NextPage);
        if (buttonFinish != null) buttonFinish.onClick.RemoveListener(FinishTutorial); // Remove listener
    }

    public void ShowTutorial()
    {
        SetActive(true);
        currentPage = 0;
        UpdatePages();
    }

    private void NextPage()
    {
        if (currentPage < tutorialPages.Length - 1)
        {
            currentPage++;
            UpdatePages();
        }
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePages();
        }
    }

    private void FinishTutorial()
    {
        SetActive(false);
        OnTutorialFinished?.Invoke();
    }

    private void UpdatePages()
    {
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if (tutorialPages[i] != null)
            {
                tutorialPages[i].SetActive(i == currentPage);
            }
        }

        bool isLastPage = currentPage == tutorialPages.Length - 1;

        if (buttonLeft != null)
        {
            buttonLeft.gameObject.SetActive(currentPage > 0);
        }

        if (buttonRight != null)
        {
            buttonRight.gameObject.SetActive(!isLastPage);
        }

        if (buttonFinish != null)
        {
            buttonFinish.gameObject.SetActive(isLastPage);
        }
    }
}