using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDialogue
{
    void ShowDialogue(string dialogue);
    void HideDialogue();
    void ShowPrompt(string prompt);
    void HidePrompt();
    void ShowTaskDialogue(string taskDialogue);
    void HideTaskDialogue();
}
