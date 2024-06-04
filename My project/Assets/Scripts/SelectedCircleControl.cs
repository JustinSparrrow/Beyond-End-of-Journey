using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCircleControl : MonoBehaviour
{
    const int FRIEND_ID = 0, ENEMY_ID = 1, NEUTRAL_ID = 2;
    private Vector2 originScale;
    private SpriteRenderer spriteRenderer = null; // 指向SpriteRenderer组件
    public Sprite[] circleSprites; // 包含所有要切换的Sprite

    void Awake()
    {
        // 在Awake中获取SpriteRenderer组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        originScale = transform.localScale;
    }

    void Start()
    {
    }

    public void SwitchSprite(int targetSpriteLayer)
    {
        // 切换到下一个Sprite
        spriteRenderer.sprite = circleSprites[targetSpriteLayer - 6];
    }

    public void SetScale(float xScale, float yScale)
    {
        transform.localScale = new Vector2(xScale, yScale);
    }
}
