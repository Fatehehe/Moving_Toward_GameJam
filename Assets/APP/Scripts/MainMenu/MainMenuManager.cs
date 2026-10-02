using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

public class MainMenuManager : MonoBehaviour
{
    [Header("Target Scene")]
    [SerializeField] private string targetScene;
    [SerializeField] private MainMenuUIController mainMenuUIController;

    private InputSystemService input;
    private GameConfigData config;
    private SceneLoader sceneLoader;

    [Inject]
    public void Construct(
    SceneLoader sceneLoader,
    InputSystemService input,
    GameConfigData config
    )
    {
        this.sceneLoader = sceneLoader;
        this.input = input;
        this.config = config;
    }


    private void Awake()
    {

        CursorController.instance?.UnlockCursorState();
        CursorController.instance?.ClearOverrideCursor();
        CursorController.instance?.SetCursorState(CursorState.DefaultRounded);

        input.ChangeInputState(InputStateType.UI);
        mainMenuUIController.SetActive(true);

        MainMenuEvents.OnNewGame += OnRequestNewGameGame;
        MainMenuEvents.OnOpenCredits += OnRequestOpenCredits;
        MainMenuEvents.OnCloseCredits += OnRequestCloseCredits;

    }

    private void Start()
    {
        // mainMenuUIController.SetActive(true);
        AudioEvents.TriggerPlayBGMMainMenu();
    }

    private void OnDestroy()
    {
        MainMenuEvents.OnNewGame -= OnRequestNewGameGame;
        MainMenuEvents.OnOpenCredits -= OnRequestOpenCredits;
        MainMenuEvents.OnCloseCredits -= OnRequestCloseCredits;
    }

    private void OnRequestNewGameGame()
    {
        sceneLoader.LoadSceneAsync(targetScene, config.minLoadingScreenDuration);
    }

    private void OnRequestOpenCredits()
    {

    }

    private void OnRequestCloseCredits()
    {

    }
}
