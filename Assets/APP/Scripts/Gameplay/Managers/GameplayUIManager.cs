using System;
using UnityEngine;
using VContainer;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameplayUIController gameplayUIController;
    [SerializeField] private EndgameUIController endGameUIController;
    [SerializeField] private DialogueUIController dialogueUIController;
    public GameplayUIController GameplayUIController => gameplayUIController;
    public EndgameUIController EndGameUIController => endGameUIController;
    public DialogueUIController DialogueUIController => dialogueUIController;

    private InputSystemService input;

    [Inject]
    public void Construct(InputSystemService input)
    {
        this.input = input;
    }

    private void Start()
    {
        // Munculkan dialog pertama (Index 0) saat game mulai
        ShowDialogueUI(0);
    }

    private void OnEnable()
    {
        GameEvents.OnTutorialCompleted += HandleTutorialCompleted;

        // Mendaftarkan trigger tutorial lainnya
        GameEvents.OnFirstInspect += HandleFirstInspect;
        GameEvents.OnFirstSurfaceCleaned += HandleFirstSurfaceCleaned;
        GameEvents.OnCleaningFinished += HandleCleaningFinished;
        GameEvents.OnAssemblingFinished += HandleAssemblingFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnTutorialCompleted -= HandleTutorialCompleted;

        GameEvents.OnFirstInspect -= HandleFirstInspect;
        GameEvents.OnFirstSurfaceCleaned -= HandleFirstSurfaceCleaned;
        GameEvents.OnCleaningFinished -= HandleCleaningFinished;
        GameEvents.OnAssemblingFinished -= HandleAssemblingFinished;
    }

    // --- FUNGSI TRIGGER TUTORIAL ---
    private void HandleFirstInspect() => ShowDialogueUI(1);
    private void HandleFirstSurfaceCleaned() => ShowDialogueUI(2);
    private void HandleCleaningFinished() => ShowDialogueUI(3);
    private void HandleAssemblingFinished() => ShowDialogueUI(4);

    private void HandleTutorialCompleted(int index)
    {
        input.ChangeInputState(InputStateType.Player);

        // Jika tutorial yang selesai adalah tutorial terakhir (Assemble 100%),
        // Langsung munculkan Endgame UI, jangan balik ke Gameplay UI.
        if (index == 4)
        {
            ShowEndgameUI();
        }
        else
        {
            ShowGameplayUI();
        }
    }

    public void ShowEndgameUI()
    {
        endGameUIController.SetActive(true);
        gameplayUIController.SetActive(false);
        dialogueUIController.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        endGameUIController.SetActive(false);
        gameplayUIController.SetActive(true);
        dialogueUIController.SetActive(false);
    }

    public void ShowDialogueUI(int index)
    {
        input.ChangeInputState(InputStateType.UI);
        endGameUIController.SetActive(false);
        gameplayUIController.SetActive(false);
        dialogueUIController.SetActive(true);
        dialogueUIController.ShowTutorialDialogue(index);
    }
}