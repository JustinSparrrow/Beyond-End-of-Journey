using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITask
{
    void StartTask();
    void UpdateTaskProgress(int amount);
    void CompleteTask();
    bool IsTaskComplete();
    string GetTaskDescription();
}
