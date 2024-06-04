using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveControl : MonoBehaviour
{
    private Rigidbody2D m_rigidbody; // 角色的Rigidbody2D组件
    private GameObject m_owner; //箭矢的发射者和目标
    private UnitControl m_oControl, m_tControl; //发射者的单位脚本和目标的单位脚本
    public GameObject m_particlePrefab;
    public Vector2 m_targetPosition;
    public int m_targetLayerMask;
    public float m_harmRadius, m_damage, m_endHarmRadius, m_endDamage, m_range;
    private HashSet<GameObject> m_harmedObject_set = new HashSet<GameObject>(); //已经伤害过的对象不会被再次伤害

    public void Init(GameObject owner, Vector2 targetPosition, int targetLayerMask, float harmRadius = 2.0f, float damage = 30.0f, 
        float endHarmRadius = 3.0f, float endDamage = 60.0f, float range = 6.0f, float speed = 5.0f)
    {
        m_owner = owner;
        m_targetPosition = targetPosition;
        m_oControl = m_owner.GetComponent<UnitControl>();
        m_harmRadius = harmRadius;
        m_targetLayerMask = targetLayerMask;
        m_damage = damage;
        m_endHarmRadius = endHarmRadius;
        m_endDamage = endDamage;
        m_range = range;
    }

    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D组件
        Vector2 direction = (m_targetPosition - (Vector2)transform.position).normalized;
        m_targetPosition = (Vector2)transform.position + direction * m_range;
        m_rigidbody.velocity = direction * 3.0f;
    }

    void Update()
    {
        if (m_oControl == null || MathUtil.Distance(transform.position, m_targetPosition) < 0.15)  // 已经到达目标地点，销毁
        {
            Constant.LaunchParticle(Constant.instance.bigExplosionPrefab, transform.position);
            RayCastUtil.CircleDamanage(m_oControl, transform.position, m_endHarmRadius, m_targetLayerMask, m_endDamage);
            Destroy(gameObject); // 销毁当前对象，下面的代码将不会执行
            return;
        }
        RayCastUtil.CircleDamanage(m_oControl, transform.position, m_harmRadius, m_targetLayerMask, m_damage, m_harmedObject_set);
        /*
        RaycastHit2D[] hits = RayCastUtil.CircleHit(transform.position, m_harmRadius, m_targetLayerMask);
        if (hits == null) return;
        foreach(RaycastHit2D hit in hits)
        {
            GameObject obj = hit.collider.gameObject;
            if (!m_harmedObject_set.Contains(obj))
            {
                m_harmedObject_set.Add(obj);
                m_oControl.AttackOn(obj, m_damage);
            }
        }
        */
    }
}
