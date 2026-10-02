using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DG.Tweening;

public class MainMenuUIController : BaseMenuController
{
    [SerializeField] private Button buttonStart;
    [SerializeField] private Button buttonCredit;

    protected override void Awake()
    {
        base.Awake();

        if (buttonStart != null) buttonStart.onClick.AddListener(OnButtonNewGameClick);
        if (buttonCredit != null) buttonCredit.onClick.AddListener(OnShowCreditClick);
    }

    private void OnButtonNewGameClick()
    {
        MainMenuEvents.TriggerNewGame();
    }

    private void OnShowCreditClick()
    {
        MainMenuEvents.TriggerNewGame();
    }
}
