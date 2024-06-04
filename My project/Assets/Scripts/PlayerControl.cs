using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerControl : MonoBehaviour
{
    public static PlayerControl instance;

    private UnitControl.Spell m_selectedSpell; //施法属性
    private GameObject m_selectedUnit = null; //当前选中对象
    public GameObject m_selectedCircle, m_hoverCircle; //选择圈, 鼠标悬停圈
    private bool m_selectForSpell = false;
    private HashSet<KeyCode> m_spellHotKey_set = new HashSet<KeyCode>() { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.D }; //技能关键按键

    void Start()
    {
        instance = this;
        // 注册鼠标点击事件监听器
        if (Camera.main != null)
        {
            Camera.main.GetComponent<Camera>().eventMask |= 1 << LayerMask.NameToLayer("MapLayer"); // 注册点击事件
            Camera.main.GetComponent<Camera>().eventMask |= 1 << LayerMask.NameToLayer("PlayerLayer"); // 注册点击事件
            Camera.main.GetComponent<Camera>().eventMask |= 1 << LayerMask.NameToLayer("EnemyLayer"); // 注册点击事件
            Camera.main.GetComponent<Camera>().eventMask |= 1 << LayerMask.NameToLayer("UI"); // 注册UI点击事件
        }
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);// 获取鼠标点击位置在世界坐标系中的位置
        GameObject hittenObject = null;

        RaycastHit2D[] hits = RayCastUtil.CircleHit(new Vector2(mousePosition.x, mousePosition.y), 0.15f, Constant.unitLayerMask);
        if (hits != null)
        {
            hittenObject = hits[0].collider.gameObject;
            ShowHoverUnit(hittenObject);// 显示悬停圈 
            if (m_selectForSpell)
                MouseCursorControl.instance.setMouseCursorSprite(hittenObject.layer, MouseCursorControl.MouseCursorType.Target); //命中物体，选择物体的层级
            else
                MouseCursorControl.instance.setMouseCursorSprite(hittenObject.layer, MouseCursorControl.MouseCursorType.Selection); //命中物体，选择物体的层级
            if (Input.GetMouseButtonDown(0) && !m_selectForSpell)
            {
                SetSelectUnit(hittenObject); //设置选中
            }
        }
        else
        {
            if (m_selectForSpell)
                MouseCursorControl.instance.setMouseCursorSprite(Constant.neutralLayer, MouseCursorControl.MouseCursorType.Target); //射线不命中物体，默认选中中立
            else
                MouseCursorControl.instance.setMouseCursorSprite(Constant.neutralLayer, MouseCursorControl.MouseCursorType.Selection); //射线不命中物体，默认选中中立

            m_hoverCircle.SetActive(false);
        }

        //下面的代码都需要m_selectedUnit不为空，上面都不需要
        if (m_selectedUnit == null)
            return;
        BoxCollider2D unitBoxCollider = m_selectedUnit.GetComponent<BoxCollider2D>();
        if (unitBoxCollider)
        {
            m_selectedCircle.transform.position = unitBoxCollider.bounds.center;
        }
        else
        {
            m_selectedCircle.transform.position = m_selectedUnit.transform.position; //选择圈追踪
        }
        //下面的代码都需要m_selectedUnit为玩家单位，上面都不需要
        if (m_selectedUnit.layer != Constant.playerLayer)
            return;

        UnitControl selectedUnitControl = m_selectedUnit.GetComponent<UnitControl>();

        //判断施法按键
        foreach (KeyCode spellHotKey in m_spellHotKey_set)
        {
            if (Input.GetKeyDown(spellHotKey))
            {
                // 当按下集合中的任何一个键时执行的代码
                readyToSpell(selectedUnitControl.spellProps.m_spellDictionary[spellHotKey]);
                break; // 只需要响应一次按键，使用break退出循环
            }
        }

        if (Input.GetMouseButtonDown(0)) // 检测鼠标点击事件，选中单位或技能逻辑
        {
            if (m_selectForSpell) //设置施法
            {
                if ((m_selectedSpell.m_acceptLayerMask & Constant.groundLayerMask) != 0) //接受地面为目标
                {
                    m_selectForSpell = false;
                    selectedUnitControl.opProps.m_targetPosition = mousePosition;  // 设置目标地点
                    selectedUnitControl.ReadyToSpell(m_selectedSpell);
                }
                else if (hittenObject && (m_selectedSpell.m_acceptLayerMask & (1 << hittenObject.layer)) != 0) //接受单位为目标
                {
                    m_selectForSpell = false;
                    selectedUnitControl.opProps.m_operationTarget = hittenObject; // 设置目标单位
                    selectedUnitControl.ReadyToSpell(m_selectedSpell);
                }
                else
                {
                    Debug.Log("施法目标错误！");
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) //移动或攻击逻辑或取消选中逻辑，只有己方单位触发
        {
            if (selectedUnitControl.combatProps.m_health <= 0) //选中的单位死亡了，不执行下面的动作
            {
                m_selectedUnit = null;
                return;
            }
            if (m_selectForSpell) //取消施法
            {
                m_selectForSpell = false;
                m_selectedSpell = null;
                Debug.Log("取消施法");
                return;
            }
            // 如果射线与碰撞器相交，尝试执行攻击
            if (hits != null)
            {
                if (hittenObject.layer == Constant.enemyLayer) // 检索到敌人层，执行攻击
                {
                    selectedUnitControl.opProps.m_operationTarget = hittenObject;
                    selectedUnitControl.ChangeOpType(UnitControl.OperationType.Attack);
                }
                else //非敌人层，执行移动
                {
                    selectedUnitControl.opProps.m_targetPosition = mousePosition; // 计算移动方向
                    selectedUnitControl.ChangeOpType(UnitControl.OperationType.Move);
                }
            }
            else //点击地面，执行移动逻辑
            {
                selectedUnitControl.opProps.m_targetPosition = mousePosition; // 计算移动方向
                selectedUnitControl.ChangeOpType(UnitControl.OperationType.Move);
            }
        }
    }

    void ShowHoverUnit(GameObject hoverObject)
    {
        if (((1 << hoverObject.layer) | Constant.unitLayerMask) == 0) return;
        BoxCollider2D unitBoxCollider = hoverObject.GetComponent<BoxCollider2D>();
        SelectedCircleControl circleControl = m_hoverCircle.GetComponent<SelectedCircleControl>();
        m_hoverCircle.SetActive(true);
        if (unitBoxCollider)
        {
            m_hoverCircle.transform.position = unitBoxCollider.bounds.center;
        }
        else
        {
            m_hoverCircle.transform.position = hoverObject.transform.position; //选择圈追踪
        }
        circleControl.SwitchSprite(hoverObject.layer); //切换角色所有者：敌人、盟友、中立、玩家，切换颜色
        if (unitBoxCollider)
        {
            Vector2 colliderSize = unitBoxCollider.size;
            float circleRadius = Mathf.Max(colliderSize.x, colliderSize.y);
            circleControl.SetScale(circleRadius, circleRadius);
        }
    }

    void SetSelectUnit(GameObject selectedObject)
    {
        m_selectForSpell = false;
        m_selectedUnit = selectedObject;
        if (selectedObject == null) //取消选中逻辑
        {
            m_selectedCircle.SetActive(false);
        }
        else
        {
            m_selectedCircle.SetActive(true);
            
            SelectedCircleControl circleControl = m_selectedCircle.GetComponent<SelectedCircleControl>();
            BoxCollider2D unitBoxCollider = m_selectedUnit.GetComponent<BoxCollider2D>();
            circleControl.SwitchSprite(m_selectedUnit.layer); //切换角色所有者：敌人、盟友、中立、玩家，切换颜色
            if (unitBoxCollider)
            {
                // 获取BoxCollider2D的大小
                Vector2 colliderSize = unitBoxCollider.size;

                // 选择圈的SpriteRenderer组件
                //SpriteRenderer circleRenderer = selectedCircle.GetComponent<SpriteRenderer>();
                float circleRadius = Mathf.Max(colliderSize.x, colliderSize.y);
                circleControl.SetScale(circleRadius, circleRadius);
            }
        }
    }

    void readyToSpell(UnitControl.Spell spell)
    {
        if(spell.m_acceptLayerMask == 0) //无目标技能，直接准备释放
        {
            UnitControl selectedUnitConrol = m_selectedUnit.GetComponent<UnitControl>();
            selectedUnitConrol.ReadyToSpell(spell);
        }
        else //有目标指定技能
        {
            m_selectedSpell = spell;
            m_selectForSpell = true;
        }
    }
}
