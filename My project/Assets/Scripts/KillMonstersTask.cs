using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillMonstersTask : ITask
{
    private int currentProgress;
    private int targetProgress;
    private bool isComplete;

    public KillMonstersTask(int target)
    {
        targetProgress = target;
        currentProgress = 0;
        isComplete = false;
    }

    public void StartTask()
    {
        currentProgress = 0;
        isComplete = false;
    }

    public void UpdateTaskProgress(int amount)
    {
        if (!isComplete)
        {
            currentProgress += amount;
            if (currentProgress >= targetProgress)
            {
                isComplete = true;
            }
        }
    }

    public void CompleteTask()
    {
        isComplete = true;
    }
    
    public bool IsTaskComplete()
    {
        return isComplete;
    }
    
    public string GetTaskDescription()
    {
        return $"击杀怪物 {currentProgress}/{targetProgress}";
    }
}
