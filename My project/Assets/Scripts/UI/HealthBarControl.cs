using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarControl : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject m_mask;
    public GameObject m_bar;

    private void Awake()
    {
        m_mask = GameObject.Find("Mask");
        m_mask.name = name + "Mask";
        m_bar = GameObject.Find("HealthBar");
        m_bar.name = name + "HealthBar";
    }

    private void Update()
    {
        m_bar.transform.position = transform.position;
    }

    public void SetByRatio(float ratio)
    {
        if(m_mask == null)
        {
            Debug.Log("mask is null");
        }
        m_mask.transform.position = transform.position + new Vector3(ratio, 0, 0);
    }
}
