using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseCursorControl : MonoBehaviour
{
    public enum MouseCursorType
    {
        Selection = 0,
        Target = 1
    };

    public static MouseCursorControl instance { get; private set; }

    public Sprite m_enermySelection_sprite;
    public Sprite m_friendSelection_sprite;
    public Sprite m_neutralSelection_sprite;
    public Sprite m_playerSelection_sprite;

    public Sprite m_enermyTarget_sprite;
    public Sprite m_friendTarget_sprite;
    public Sprite m_neutralTarget_sprite;
    public Sprite m_playerTarget_sprite;

    public Dictionary<(int, MouseCursorType), Sprite> mouseCursorSprite_dic;

    SpriteRenderer m_spriteRenderer;
    Image m_mouseCursorImage; // 引用UI的Image组件

    void Start()
    {
        instance = this;
        m_mouseCursorImage = GetComponent<Image>();
        Cursor.visible = false; // 隐藏鼠标指针

        mouseCursorSprite_dic = new Dictionary<(int, MouseCursorType), Sprite>
        {
            {(Constant.enemyLayer, MouseCursorType.Selection), m_enermySelection_sprite},
            {(Constant.enemyLayer, MouseCursorType.Target), m_enermyTarget_sprite},

            {(Constant.friendLayer, MouseCursorType.Selection), m_friendSelection_sprite},
            {(Constant.friendLayer, MouseCursorType.Target), m_friendTarget_sprite},

            {(Constant.neutralLayer, MouseCursorType.Selection), m_neutralSelection_sprite},
            {(Constant.neutralLayer, MouseCursorType.Target), m_neutralTarget_sprite},

            {(Constant.playerLayer, MouseCursorType.Selection), m_playerSelection_sprite},
            {(Constant.playerLayer, MouseCursorType.Target), m_playerTarget_sprite},
        };

    }
    void Update()
    {
        Vector2 mousePosition = Input.mousePosition; // 获取鼠标位置并转换为屏幕坐标
        gameObject.transform.position = mousePosition + new Vector2(0.1f, 0.1f); // 设置Image组件的位置
    }

    public void setMouseCursorSprite(int layer, MouseCursorType type)
    {
        m_mouseCursorImage.sprite = mouseCursorSprite_dic[(layer, type)];
    }
}
