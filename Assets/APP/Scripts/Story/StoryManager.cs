using System;
using UnityEngine;
using VContainer;

public class StoryManager : MonoBehaviour
{
    [Header("Target Scene")]
    [SerializeField] private string targetScene;
    [SerializeField] private StoryUIController uiTutorialController;

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

    private void Start()
    {
        input.ChangeInputState(InputStateType.UI);
        uiTutorialController.OnTutorialFinished += OnStoryFinished;
        uiTutorialController.ShowTutorial();
    }

    private void OnDestroy()
    {
        if (uiTutorialController != null)
        {
            uiTutorialController.OnTutorialFinished -= OnStoryFinished;
        }
    }

    private void OnStoryFinished()
    {
        sceneLoader.LoadSceneAsync(targetScene, config.minLoadingScreenDuration);
    }
}