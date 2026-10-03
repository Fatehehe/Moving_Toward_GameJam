// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using VContainer;

// [Serializable]
// public class TutorialData
// {
//     [Tooltip("Index UI Dialog yang mau dipakai (0=Utama, 1=Radio, dst sesuai di DialogueUIController)")]
//     public int dialoguePanelIndex = 0;

//     [TextArea(3, 5)]
//     public string dialogueText;
//     public Sprite portrait;

//     public UnityEvent onNextAction;
// }

// public class TutorialManager : MonoBehaviour
// {
//     [Header("UI Reference")]
//     [SerializeField] private DialogueUIController dialogueUIController;

//     [Header("Tutorial Database")]
//     [SerializeField] private List<TutorialData> tutorialList = new List<TutorialData>();

//     void Start()
//     {
//         for (int i = 0; i < tutorialList.Count; i++)
//         {
//             int currentIndex = i;
//             if (tutorialList[currentIndex].onNextAction == null)
//             {
//                 tutorialList[currentIndex].onNextAction = new UnityEvent();
//             }
//             tutorialList[currentIndex].onNextAction.AddListener(() => TriggerEventTutorial(currentIndex));
//         }
//     }

//     public void TriggerEventTutorial(int index)
//     {
//         GameEvents.RaiseTutorialCompleted(index);
//     }

//     public void PlayTutorial(int index)
//     {
//         if (index < 0 || index >= tutorialList.Count)
//         {
//             Debug.LogWarning($"[TutorialManager] Tutorial index {index} tidak ditemukan!");
//             return;
//         }

//         TutorialData tutorial = tutorialList[index];

//         // Masukkan dialoguePanelIndex sebagai parameter tambahan di akhir fungsi
//         dialogueUIController.ShowDialogue(tutorial.dialogueText, tutorial.portrait, () =>
//         {
//             tutorial.onNextAction?.Invoke();
//             // Gunakan index panel dari data agar panel yang tepat yang ditutup
//             dialogueUIController.HideDialogue();

//         }, tutorial.dialoguePanelIndex);
//     }
// }