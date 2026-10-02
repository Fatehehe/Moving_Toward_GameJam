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
        mainMenuUIController.SetActive(true);

        MainMenuEvents.OnNewGame += OnRequestNewGameGame;
        MainMenuEvents.OnOpenCredits += OnRequestOpenCredits;
        MainMenuEvents.OnCloseCredits += OnRequestCloseCredits;

    }

    private void OnDestroy()
    {
        MainMenuEvents.OnNewGame -= OnRequestNewGameGame;
        MainMenuEvents.OnOpenCredits -= OnRequestOpenCredits;
        MainMenuEvents.OnCloseCredits -= OnRequestCloseCredits;
    }

    private void OnRequestNewGameGame()
    {
        _ = sceneLoader.LoadSceneAsync(
            targetScene,
            config.minLoadingScreenDuration
        );
    }

    private void OnRequestOpenCredits()
    {

    }

    private void OnRequestCloseCredits()
    {

    }
}
