using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DialogueSystem : MonoBehaviour
{
    [Header("UI���")]
    public Text textLabel;
    public Image faceImage;

    [Header("�ı��ļ�")]
    public TextAsset textFile;
    public int index;
    public float textSpeed;

    bool textFinished;//�Ƿ���ɴ���
    bool cancelTyping;//ȡ������

    [Header("ͷ��")]
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
    void GetTextFromFile(TextAsset file)//��ȡ�ĵ�
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

        switch(textList[index])//�ж��ĵ�һ���е���������ͷ��
        {
            case "A":
                faceImage.sprite=face01;
                index++;//ȥ���жϵ���һ��
                break;
            case "B":
                faceImage.sprite = face02;
                index++;
                break;
        }

    
        int letter = 0;
        while (!cancelTyping && letter < textList[index].Length-1)//ȡ������ֱ����ʾ
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
