using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Text textLabel;
    public Image faceImage;

    [Header("文本文件")]
    public TextAsset textFile;
    public int index;
    public float textSpeed;

    bool textFinished;//是否完成打字
    bool cancelTyping;//取消打字

    [Header("头像")]
    public Sprite face01, face02;

    List<string> textList = new List<string>();

   
    void Awake()
    {
        GetTextFromFile(textFile);
       
    }
    private void OnEnable()
    {
 
        textFinished = true;
        StartCoroutine(SetTextUI());
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)&&index==textList.Count)
        {
            gameObject.SetActive(false);
            index = 0;
            return;
        }
   
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(textFinished&&!cancelTyping)
            {
                StartCoroutine(SetTextUI());
            }
            else if(!textFinished&&!cancelTyping)
            {
                cancelTyping= true;
            }
        }
    }
    void GetTextFromFile(TextAsset file)//读取文档
    {
        textList.Clear();
        index= 0;

        var lineData = file.text.Split('\n');

        foreach(var line in lineData)
        {
            textList.Add(line);
        }
    }

    IEnumerator SetTextUI()
    {
        textFinished= false;
        textLabel.text = "";

        switch(textList[index])//判断文档一整行的内容来换头像
        {
            case "A":
                faceImage.sprite=face01;
                index++;//去除判断的那一行
                break;
            case "B":
                faceImage.sprite = face02;
                index++;
                break;
        }

    
        int letter = 0;
        while (!cancelTyping && letter < textList[index].Length-1)//取消打字直接显示
        {
            textLabel.text += textList[index][letter];
            letter++;
            yield return new WaitForSeconds(textSpeed);
        }
        textLabel.text = textList[index];   
        cancelTyping = false;
        textFinished = true;   
        index++;
    }
}
