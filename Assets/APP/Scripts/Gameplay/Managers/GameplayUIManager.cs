using System;
using UnityEngine;
using VContainer;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private GameplayUIController gameplayUIController;
    public GameplayUIController GameplayUIController => gameplayUIController;

    [Inject]
    public void Construct(IObjectResolver container)
    {

    }

    private void Awake()
    {
        gameplayUIController.SetActive(true);
    }

}
