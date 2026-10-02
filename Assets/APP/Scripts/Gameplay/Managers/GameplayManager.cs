using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayManager : IInitializable, IDisposable
{
    private GameplayUIManager uiManager;

    [Inject]
    public void Construct(GameplayUIManager uiManager)
    {
        this.uiManager = uiManager;
    }

    public void Initialize()
    {
        GameEvents.OnCleaningFinished += HandleCleaningFinished;
        GameEvents.OnAssemblingFinished += HandleAssemblingFinished;

        uiManager.GameplayUIController.ShowCleanProgress();
        uiManager.GameplayUIController.HideAssembleProgress();
        uiManager.GameplayUIController.HideHoldProgress();

        GameEvents.RaiseGameStarted();
    }

    public void Dispose()
    {
        GameEvents.OnCleaningFinished -= HandleCleaningFinished;
        GameEvents.OnAssemblingFinished -= HandleAssemblingFinished;
    }

    private void HandleCleaningFinished()
    {
        Debug.Log("Cleaning Selesai! Pindah ke fase Assemble.");

        uiManager.GameplayUIController.HideCleanProgress();
        uiManager.GameplayUIController.ShowAssembleProgress();
    }

    private void HandleAssemblingFinished()
    {
        Debug.Log("Assemble Selesai! Game Berakhir.");

        uiManager.GameplayUIController.HideAssembleProgress();
        GameEvents.RaiseGameEnded();
    }
}