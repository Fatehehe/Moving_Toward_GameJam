using System;
using UnityEngine;
using VContainer;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameplayUIController gameplayUIController;
    public GameplayUIController GameplayUIController => gameplayUIController;

    private void Awake()
    {
        gameplayUIController.SetActive(true);
    }

}
