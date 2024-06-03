using UnityEngine;

public class NPCDialogueTrigger : MonoBehaviour
{
    public string dialogue; // NPC的对话内容
    public string taskDialogue; // 任务对话内容
    private IDialogue dialogueManager;
    private bool isPlayerInRange = false; // 用于检测玩家是否在触发器范围内

    void Start()
    {
        // 获取实现了IDialogue接口的组件
        dialogueManager = FindObjectOfType<DialogueManager>() as IDialogue;
        if (dialogueManager == null)
        {
            Debug.LogError("No DialogueManager found in the scene!");
        }
    }

    public void TriggerDialogue(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 当玩家进入触发器时显示提示信息
            dialogueManager.ShowPrompt("Press 'R' to view task");
            isPlayerInRange = true;
        }
    }

    public void EndDialogue(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 当玩家离开触发器时隐藏提示信息和对话框
            dialogueManager.HidePrompt();
            dialogueManager.HideDialogue();
            isPlayerInRange = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TriggerDialogue(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        EndDialogue(other);
    }

    void Update()
    {
        // 检测玩家是否按下了“R”键并且在触发器范围内
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.R))
        {
            TaskManager taskManager = FindObjectOfType<TaskManager>();
            if (taskManager.HasActiveTask())
            {
                dialogueManager.ShowDialogue("请先完成当前任务。");
            }
            else
            {
                dialogueManager.ShowTaskDialogue(taskDialogue);
            }
        }
    }
}