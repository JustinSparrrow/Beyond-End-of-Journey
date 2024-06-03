using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public Text taskDescriptionText;
    private ITask currentTask;
    
    // Start is called before the first frame update
    void Start()
    {
        taskDescriptionText.gameObject.SetActive(false);
    }

    public void StartTask(ITask task)
    {
        currentTask = task;
        currentTask.StartTask();
        taskDescriptionText.gameObject.SetActive(true);
        UpdateTaskDescription();
    }
    
    public void UpdateTaskProgress(int amount)
    {
        if (currentTask != null)
        {
            currentTask.UpdateTaskProgress(amount);
            UpdateTaskDescription();
        }
    }
    
    public void CompleteTask()
    {
        if (currentTask != null && currentTask.IsTaskComplete())
        {
            currentTask.CompleteTask();
            taskDescriptionText.gameObject.SetActive(false);
            currentTask = null;
        }
    }
    
    private void UpdateTaskDescription()
    {
        if (currentTask != null)
        {
            taskDescriptionText.text = currentTask.GetTaskDescription();
        }
    }
    
    public bool HasActiveTask()
    {
        return currentTask != null && !currentTask.IsTaskComplete();
    }
}
