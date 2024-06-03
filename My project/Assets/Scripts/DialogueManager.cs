using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour, IDialogue
{
    public GameObject dialoguePanel; // 对话框的Panel
    public Text dialogueText; // 显示对话内容的Text
    public Text promptText; // 提示信息的Text
    public GameObject taskPanel; // 任务对话框的Panel
    public Text taskDialogueText; // 任务对话框的Text
    public Button acceptButton; // 接受任务按钮
    public Button declineButton; // 拒绝任务按钮

    void Start()
    {
        // 初始化时隐藏对话框和提示信息
        dialoguePanel.SetActive(false);
        promptText.gameObject.SetActive(false);
        taskPanel.SetActive(false);

        // 设置按钮点击事件
        acceptButton.onClick.AddListener(AcceptTask);
        declineButton.onClick.AddListener(DeclineTask);
    }

    public void ShowDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowPrompt(string prompt)
    {
        promptText.text = prompt;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        promptText.gameObject.SetActive(false);
    }

    public void ShowTaskDialogue(string taskDialogue)
    {
        taskDialogueText.text = taskDialogue;
        taskPanel.SetActive(true);
    }

    public void HideTaskDialogue()
    {
        taskPanel.SetActive(false);
    }

    private void AcceptTask()
    {
        TaskManager taskManager = FindObjectOfType<TaskManager>();
        taskManager.StartTask(new KillMonstersTask(25)); // 例如，击杀25个怪物的任务
        HideTaskDialogue();
    }

    private void DeclineTask()
    {
        ShowDialogue("你拒绝了任务。");
        HideTaskDialogue();
    }
}