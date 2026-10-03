using System;
using UnityEngine;

public class DialogueUIController : BaseMenuController
{
    [Header("Dialogue Panels")]
    [Tooltip("Masukkan 5 Dialogue UI kamu ke sini secara berurutan (Index 0 sampai 4)")]
    [SerializeField] private DialogueUI[] tutorialDialogues;

    public override void SetActive(bool isActive)
    {
        base.SetActive(isActive);
        if (!isActive)
        {
            HideAllDialogues();
        }
    }

    // Parameter text dan portrait dihilangkan, sisa index saja
    public void ShowTutorialDialogue(int index)
    {
        Debug.Log($"[DialogueUIController] ShowTutorialDialogue called with index: {index}");
        if (index < 0 || index >= tutorialDialogues.Length || tutorialDialogues[index] == null)
        {
            Debug.LogWarning($"[DialogueUIController] DialogueUI index {index} tidak ditemukan!");
            return;
        }

        HideAllDialogues();

        // Hanya mengirimkan Action (fungsinya) saja
        tutorialDialogues[index].ShowDialogue(() =>
        {
            GameEvents.RaiseTutorialCompleted(index);
            tutorialDialogues[index].HideDialogue();
        });
    }

    public void HideAllDialogues()
    {
        foreach (var dialogue in tutorialDialogues)
        {
            if (dialogue != null && dialogue.gameObject.activeInHierarchy)
            {
                dialogue.HideDialogue();
            }
        }
    }
}