using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using DG.Tweening;

public class EndgameUIController : BaseMenuController
{
    [Header("Buttons")]
    [SerializeField] private Button buttonFinished;

    public override void SetActive(bool isActive)
    {
        base.SetActive(isActive);
    }

    protected override void Awake()
    {
        base.Awake();
        if (buttonFinished != null) buttonFinished.onClick.AddListener(OnButtonFinishedClick);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (buttonFinished != null) buttonFinished.onClick.RemoveListener(OnButtonFinishedClick);
    }

    private void OnButtonFinishedClick()
    {
        GameEvents.RaiseGameFinished();
    }
}
