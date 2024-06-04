using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour
{
    private Rigidbody2D m_rigidbody; // 角色的Rigidbody2D组件
    private GameObject m_owner, m_target; //箭矢的发射者和目标
    private UnitControl m_oControl, m_tControl; //发射者的单位脚本和目标的单位脚本
    private UnitControl.MoveProperties moveProps; //移动相关变量
    public GameObject m_particlePrefab;

    public void Init(GameObject owner, GameObject target)
    {
        m_owner = owner;
        m_target = target;
        m_oControl = m_owner.GetComponent<UnitControl>();
        m_tControl = m_target.GetComponent<UnitControl>();
        moveProps = new UnitControl.MoveProperties(10, 5, 20, 
            ((Vector2)m_target.transform.position - (Vector2)transform.position).normalized); //设置了初始朝向、速度各项属性
    }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D组件
    }

    void Update()
    {
        if (m_target == null) //目标已被摧毁或删除
        {
            Destroy(gameObject); // 销毁当前对象，下面的代码将不会执行
            return;
        }
        if (MathUtil.Distance(transform.position, m_target.transform.position) < 0.15)  // 已经触碰到目标，销毁并造成伤害
        {
            if(m_oControl != null)
                m_oControl.AttackOn(m_target, m_oControl.combatProps.m_damage);
            
            if(m_particlePrefab != null)
            {
                Constant.LaunchParticle(m_particlePrefab, m_target.transform.position);
            }
            Destroy(gameObject); // 销毁当前对象，下面的代码将不会执行
            return;
        }

        moveProps.m_moveDirection = ((Vector2)m_target.transform.position - (Vector2)transform.position).normalized; // 计算移动方向
        transform.up = Vector3.Slerp(transform.up, moveProps.m_moveDirection, moveProps.m_rotateSpeed * Time.deltaTime); //更新角色朝向
        moveProps.m_currentSpeed = Mathf.Min(moveProps.m_maxnSpeed, moveProps.m_currentSpeed + moveProps.m_accelerateSpeed * Time.deltaTime);// 增加速度
        Vector2 velocity = moveProps.m_currentSpeed * moveProps.m_moveDirection;
        m_rigidbody.velocity = velocity; // 设置Rigidbody2D的速度，让角色移动
    }
}
