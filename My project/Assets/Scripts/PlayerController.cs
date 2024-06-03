using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    public GameObject myBag;
    private bool isOpen;//打开背包

    private Rigidbody2D player;
    private Vector2 movement;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 获取输入
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        OpenMyBag();
    }

    private void FixedUpdate()
    {
        player.MovePosition(player.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OpenMyBag()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isOpen = !isOpen;
            myBag.SetActive(isOpen);
        }
    }
}
