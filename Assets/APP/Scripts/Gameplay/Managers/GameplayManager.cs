using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameplayManager : IInitializable, IDisposable
{
    [Header("Target Scene")]
    private string targetScene = "Ending";
    private GameplayUIManager uiManager;
    private InputSystemService input;
    private GameConfigData config;
    private SceneLoader sceneLoader;

    [Inject]
    public void Construct(GameplayUIManager uiManager, InputSystemService input,
        SceneLoader sceneLoader,
        GameConfigData config
    )
    {
        this.uiManager = uiManager;
        this.input = input;
        this.sceneLoader = sceneLoader;
        this.config = config;
    }

    public void Initialize()
    {
        // input.ChangeInputState(InputStateType.Player);

        GameEvents.OnCleaningFinished += HandleCleaningFinished;
        GameEvents.OnAssemblingFinished += HandleAssemblingFinished;
        GameEvents.OnGameFinished += HandleGameFinished;

        uiManager.GameplayUIController.ShowCleanProgress();
        uiManager.GameplayUIController.HideAssembleProgress();
        uiManager.GameplayUIController.HideHoldProgress();
    }

    public void Dispose()
    {
        GameEvents.OnCleaningFinished -= HandleCleaningFinished;
        GameEvents.OnAssemblingFinished -= HandleAssemblingFinished;
        GameEvents.OnGameFinished -= HandleGameFinished;
    }

    private void HandleGameFinished()
    {
        Debug.Log("Game Selesai! Pindah ke Ending.");

        uiManager.GameplayUIController.HideCleanProgress();
        uiManager.GameplayUIController.HideAssembleProgress();
        uiManager.GameplayUIController.HideHoldProgress();
        AudioEvents.TriggerPlayCustomSFX(Modules.SoundSystems.AudioKey.SFX_Finish);
        sceneLoader.LoadSceneAsync(targetScene, config.minLoadingScreenDuration);
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
        uiManager.ShowEndgameUI();
        GameEvents.RaiseGameEnded();
        AudioEvents.TriggerPlayCustomSFX(Modules.SoundSystems.AudioKey.SFX_Finish);
    }
}