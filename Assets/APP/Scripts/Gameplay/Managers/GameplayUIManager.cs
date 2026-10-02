using System;
using UnityEngine;
using VContainer;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameplayUIController gameplayUIController;
    [SerializeField] private EndgameUIController endGameUIController;
    public GameplayUIController GameplayUIController => gameplayUIController;
    public EndgameUIController EndGameUIController => endGameUIController;

    private void Awake()
    {
        ShowGameplayUI();
    }

    public void ShowEndgameUI()
    {
        endGameUIController.SetActive(true);
        gameplayUIController.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        endGameUIController.SetActive(false);
        gameplayUIController.SetActive(true);
    }
}
