using System;
using UnityEngine;
using UnityEngine.UI;

public class StoryUIController : BaseMenuController
{
    [Header("Tutorial Pages")]
    [SerializeField] private GameObject[] tutorialPages; // Masukkan 6 GameObject tutorial ke sini

    [Header("Navigation Buttons")]
    [SerializeField] private Button buttonLeft;
    [SerializeField] private Button buttonRight;

    public event Action OnTutorialFinished;

    private int currentPage = 0;

    protected override void Awake()
    {
        base.Awake();

        if (buttonLeft != null) buttonLeft.onClick.AddListener(PreviousPage);
        if (buttonRight != null) buttonRight.onClick.AddListener(NextPage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (buttonLeft != null) buttonLeft.onClick.RemoveListener(PreviousPage);
        if (buttonRight != null) buttonRight.onClick.RemoveListener(NextPage);
    }

    public void ShowTutorial()
    {
        SetActive(true);
        currentPage = 0;
        UpdatePages();
    }

    private void NextPage()
    {
        Debug.Log($"Current Page: {currentPage}");
        // Jika belum halaman terakhir, pindah ke halaman selanjutnya
        if (currentPage < tutorialPages.Length - 1)
        {
            currentPage++;
            UpdatePages();
        }
        else
        {
            // Jika sudah di halaman terakhir dan klik tombol kanan, mulai game
            SetActive(false);
            OnTutorialFinished?.Invoke();
        }
    }

    private void PreviousPage()
    {
        Debug.Log($"Current Page: {currentPage}");
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePages();
        }
    }

    private void UpdatePages()
    {
        // Nyalakan halaman yang aktif, matikan yang lain
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            if (tutorialPages[i] != null)
            {
                tutorialPages[i].SetActive(i == currentPage);
            }
        }

        // Sembunyikan tombol Kiri jika di halaman pertama (index 0)
        if (buttonLeft != null)
        {
            buttonLeft.gameObject.SetActive(currentPage > 0);
        }
    }
}