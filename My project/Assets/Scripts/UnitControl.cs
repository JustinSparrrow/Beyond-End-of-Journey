using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitControl : MonoBehaviour
{
    public GameObject m_arrowPrefab; // 箭矢精灵预制件（远程攻击才需要）
    public float verticalOffset = 0.7f; //生命条垂直偏移量， 自己设置

    public enum AttackType
    {
        Melee = 0,    // 近战
        Ranged = 1,   // 远程
        Projectile = 2 // 投射
    }

    public enum OperationType
    {
        Stop = 0,      // 停止
        Move = 1,      // 移动
        Attack = 2,    // 攻击
        Hold = 3, // 保持原位(自动攻击范围内敌人）
        Spell = 4, // 施法
        Invincible = 5 // 无敌
    }

    public enum AnimationType
    {
        Idle = 0,     //静止
        Move = 1,     //移动
        Attack = 2,   //攻击
        Spell = 3,    //施法
        Die = 4       //死亡
    }
    public Dictionary<AnimationType, String> animationType_str_dic = new Dictionary<AnimationType, string>() 
    {
        { AnimationType.Idle, "Idle" },
        { AnimationType.Move, "Move" },
        { AnimationType.Attack, "Attack" },
        { AnimationType.Spell, "Spell" },
        { AnimationType.Die, "Die" }
    };

    public class MoveProperties //移动属性
    {
        public Vector2 m_moveDirection; // 移动方向
        public float m_accelerateSpeed, m_maxnSpeed, m_rotateSpeed; // 加速度、最大速度、转向速度
        public float m_currentSpeed = 0f;
        public MoveProperties(float accelerateSpeed = 10f, float maxnSpeed = 3f, float rotateSpeed = 20f, Vector2 moveDirection = new Vector2())
        {
            m_accelerateSpeed = accelerateSpeed;
            m_maxnSpeed = maxnSpeed;
            m_rotateSpeed = rotateSpeed;
            m_moveDirection = moveDirection;
        }
    }

    public class CombatProperties
    {
        public AttackType m_attackType; //攻击种类
        public float m_maxnHealth, m_birthHealth, m_armor, m_autoHealingSpeed, m_warningRadius; //最大生命、初始生命、护甲、自动恢复生命速度、警戒半径
        public float m_damage, m_range, m_attackInterval, m_anticipation, m_recovery; // 攻击伤害、射程、攻击间隔、出手前摇、后摇
        public float m_health, m_attackCooldown = 0f, m_anti_time = 0f, m_reco_time = 0f;
        public bool m_readyToAttack = false, m_onRecovering = false, m_onDead = false;
        public CombatProperties(AttackType attackType = AttackType.Ranged, float maxnHealth = 100f, float birthHealth = -1, float armor = 0, 
            float damage = 10f, float range = 3.0f, float attackInterval = 1f, float autoHealingSpeed = 1f, float warningRadius = 4.0f, float anticipation = 0.1f, 
            float recovery = 0.4f)
        {
            m_attackType = attackType;
            m_maxnHealth = maxnHealth;
            m_health = birthHealth == -1 ? maxnHealth : birthHealth; // -1代表未定义的初始生命，默认为最大生命
            m_armor = armor;
            m_damage = damage;
            m_range = range;
            m_attackInterval = attackInterval;
            m_autoHealingSpeed = autoHealingSpeed;
            m_warningRadius = warningRadius;
            m_anticipation = anticipation;
            m_recovery = recovery;
        }
    }
    public class OperationProperties
    {
        public OperationType m_operationType; //当前操作种类
        public GameObject m_operationTarget; //操作目标单位（如果有的话）
        public Vector2 m_targetPosition; //目标地点（如果有的话）
        public OperationProperties(OperationType operationType = OperationType.Stop, GameObject operationTarget = null, Vector2 targetPosition = new Vector2())
        {
            m_operationType = operationType;
            m_operationTarget = operationTarget;
            m_targetPosition = targetPosition;
        }
    }

    public class Spell //: MonoBehaviour //法术基类
    {
        public float m_hit, m_anticipation, m_range;
        public int m_level = 1, m_acceptLayerMask;
        // 不带参数的方法
        public virtual void Task(UnitControl ctrl)
        {
            Debug.Log("Executing base spell task without parameters.");
        }

        // 带一个GameObject参数的方法
        public virtual void Task(UnitControl ctrl, GameObject target)
        {
            Debug.Log($"Executing base spell task targeting {target.name}.");
        }

        // 带一个Vector2参数的方法
        public virtual void Task(UnitControl ctrl, Vector2 targetPosition)
        {
            Debug.Log($"Executing base spell task at position {targetPosition}.");
        }
    }

    public class SpellInfo
    {
        public class FireSpell : Spell
        {
            public FireSpell()
            {
                m_acceptLayerMask = Constant.enemyLayerMask;
                m_anticipation = 1.0f;
                m_range = 6.0f;
            }

            public override void Task(UnitControl ctrl, GameObject target)
            {
                // 实现带GameObject参数的FireSpell效果
                Debug.Log($"Casting Fire Spell targeting {target.name}.");
                ctrl.AttackOn(target, 50);
                Constant.LaunchParticle(Constant.instance.plasmaExplosionPrefabs, target.transform.position);
            }
        }


        public class IceSpell : Spell
        {
            public IceSpell()
            {
                m_acceptLayerMask = Constant.groundLayerMask;
                m_anticipation = 1.0f;
                m_range = 6.0f;
            }

            public override void Task(UnitControl ctrl, Vector2 targetPosition)
            {
                // 实现带Vector2参数的FireSpell效果
                GameObject waveObject = Instantiate(Constant.instance.fireBallPrefab, ctrl.m_rigidbody.position + Vector2.up * 0.5f, Quaternion.identity);
                waveObject.GetComponent<ShockWaveControl>().Init(ctrl.gameObject, targetPosition, Constant.enermyMaskDictionary[ctrl.gameObject.layer],
                    2.0f, 50.0f, 5.0f);
            }
        }
    }


    public class SpellProperties
    {
        public Dictionary<KeyCode, Spell> m_spellDictionary = new Dictionary<KeyCode, Spell>(); //技能表
        public Spell m_currentSpell; //当前施放的法术
        public GameObject m_spellTarget; //以单位为目标
        public Vector2 m_spellTargetPosition; //以地点目标
        public float m_anti_time; //出手前摇，可被其他动作打断
    }

    private GameObject m_healthBar; // 生命条

    public AudioClip m_attackBeginClip; //出手音效
    public AudioClip m_attackOnClip;    //命中音效
    public AudioClip m_deathClip;    //死亡音效

    private Rigidbody2D m_rigidbody; // 角色的Rigidbody2D组件
    private AudioSource m_audioSource; //角色的AudioSource组件
    private HealthBarControl m_healthBarControl; //生命条控制脚本

    public MoveProperties moveProps;
    public CombatProperties combatProps;
    public OperationProperties opProps;
    public SpellProperties spellProps;

    private Animator m_animator;
    private AnimationType m_animationType = AnimationType.Idle; //当前播放的动画状态

    public Dictionary<Spell, GameObject> m_spellDictionary = new Dictionary<Spell, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>(); // 获取Rigidbody2D组件
        m_audioSource = this.gameObject.AddComponent<AudioSource>();

        //初始化生命条
        const string healthBarPath = "Assets/Prefabs/UnitComponent/HealthBarBK.prefab";
        m_healthBar = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(healthBarPath), Vector3.zero, Quaternion.identity);
        m_healthBar.name = name + "HealthBar";
        m_healthBarControl = this.m_healthBar.GetComponent<HealthBarControl>();

        moveProps = new MoveProperties();
        combatProps = new CombatProperties();
        opProps = new OperationProperties(OperationType.Stop, null, transform.localPosition);
        spellProps = new SpellProperties();

        spellProps.m_spellDictionary[KeyCode.Q] = new SpellInfo.FireSpell();
        spellProps.m_spellDictionary[KeyCode.W] = new SpellInfo.IceSpell();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(gameObject.name + "当前的OP是：" + opProps.m_operationType);
        if (combatProps.m_onDead)
        {
            return;
        }
        if(combatProps.m_health <= 0) //正在播放死亡动画，即将销毁
        {
            combatProps.m_onDead = true;
            ChangeAnimation(AnimationType.Die);
            return;
        }
        combatProps.m_health = Mathf.Min(combatProps.m_maxnHealth, combatProps.m_health + combatProps.m_autoHealingSpeed * Time.deltaTime);
        // 减少攻击CD
        if (combatProps.m_attackCooldown > 0f)
        {
            combatProps.m_attackCooldown -= Time.deltaTime;
        }
        UpdateHealthBar();
        if(combatProps.m_onRecovering)
        {
            combatProps.m_reco_time -= Time.deltaTime;
            if (combatProps.m_reco_time > 0) return;
            combatProps.m_onRecovering = false;
            ChangeAnimation(AnimationType.Move);
        }
        // 目标死亡，取消目标
        if(opProps.m_operationTarget != null && opProps.m_operationTarget.GetComponent<UnitControl>().combatProps.m_health <= 0)
        {
            opProps.m_operationTarget = null;
            ChangeOpType(OperationType.Stop);
        }

        switch (opProps.m_operationType)
        {
            case OperationType.Stop:
                // 实现停止操作的逻辑
                GameObject new_target;
                if((new_target = FindFirstAttackTargetInWarnningRadius()) != null)
                {
                    opProps.m_operationTarget = new_target;
                    ChangeOpType(OperationType.Attack);
                    break;
                }
                StopOperation();
                break;
            case OperationType.Move:
                // 实现移动操作的逻辑
                MoveOperation();
                break;
            case OperationType.Attack:
                // 实现攻击操作的逻辑
                AttackOperation();
                break;
            case OperationType.Hold:
                // 实现保持原位操作的逻辑
                HoldOperation();
                break;
            case OperationType.Spell:
                // 实现施法操作的逻辑
                SpellOperation();
                break;
            case OperationType.Invincible:
                // 实现无敌操作的逻辑
                InvincibleOperation();
                break;
            default:
                // 默认情况下可以不执行任何操作，或者处理未知的OperationType
                // 也可以添加日志记录或其他错误处理机制
                Debug.Log("未知的状态???");
                break;
        }
    }

    private GameObject FindFirstAttackTargetInWarnningRadius() //在警戒半径中找到第一个可以攻击的对象
    {
        RaycastHit2D[] hits = RayCastUtil.CircleHit(transform.position, combatProps.m_warningRadius, Constant.enermyMaskDictionary[gameObject.layer]);
        if(hits != null)
        {
            return hits[0].collider.gameObject;
        }
        return null;
    }

    private void UpdateHealthBar() //更新生命条
    {
        m_healthBarControl.SetByRatio(this.combatProps.m_health / this.combatProps.m_maxnHealth - 1);
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider)
        {
            m_healthBar.transform.position = new Vector2(collider.bounds.center.x, collider.bounds.center.y + verticalOffset); // 设置生命条的位置为碰撞盒子中心点的正上方加上垂直偏移量
        }
        else
        {
            Debug.Log("Error: 没有设置碰撞盒子！！");
        }
        //m_healthBar.transform.position = m_healthBarBias + (Vector2)transform.position;
    }

    private void StopOperation() // 停止操作的具体实现
    {
        moveProps.m_currentSpeed = 0f;
        m_rigidbody.velocity = Vector2.zero;
        opProps.m_targetPosition = transform.position;
    }

    private void ChangeFacingDirection(Vector2 position)
    {
        if (m_animator != null)
        {
            // 将移动方向标准化为单位向量
            Vector3 normalizedMoveDirection = position.normalized;
            // 设置Animator的参数
            m_animator.SetFloat("MoveX", normalizedMoveDirection.x);
            m_animator.SetFloat("MoveY", normalizedMoveDirection.y);
        }
        else
        {
            transform.up = Vector3.Slerp(transform.up, position, moveProps.m_rotateSpeed * Time.deltaTime); //更新角色朝向
        }
    }

    private void MoveOperation()// 移动操作的具体实现
    {
        if (MathUtil.Distance(transform.position, opProps.m_targetPosition) < 0.15) 
        {
            ChangeOpType(UnitControl.OperationType.Stop); // 如果角色已到达目标地点，停止移动
            return;
        }
        ChangeAnimation(AnimationType.Move);
        moveProps.m_moveDirection = (opProps.m_targetPosition - (Vector2)transform.position).normalized; // 计算移动方向
        ChangeFacingDirection(moveProps.m_moveDirection); //更新角色朝向
        moveProps.m_currentSpeed = Mathf.Min(moveProps.m_maxnSpeed, moveProps.m_currentSpeed + moveProps.m_accelerateSpeed * Time.deltaTime);// 增加速度
        Vector2 velocity = moveProps.m_currentSpeed * moveProps.m_moveDirection;
        m_rigidbody.velocity = velocity; // 设置Rigidbody2D的速度，让角色移动
    }

    private void AttackOperation() // 攻击操作的具体实现
    {
        if(opProps.m_operationTarget == null) //丢失对象，可能原因是已死亡或传送
        {
            ChangeOpType(OperationType.Stop);
            return;
        }

        if(MathUtil.Distance(this.gameObject, opProps.m_operationTarget) > combatProps.m_range)
        {
            opProps.m_targetPosition = opProps.m_operationTarget.transform.position;
            MoveOperation(); //距离不够，移动
            return;
        }
        Vector2 targetPosition = opProps.m_operationTarget.transform.position;
        moveProps.m_moveDirection = ((Vector2)targetPosition - (Vector2)transform.position).normalized; // 计算方向
        ChangeFacingDirection(moveProps.m_moveDirection);
        //transform.up = Vector3.Slerp(transform.up, moveProps.m_moveDirection, moveProps.m_rotateSpeed * Time.deltaTime); //更新角色朝向

        StopOperation();////在攻击范围内，停止移动
        if (combatProps.m_attackCooldown > 0) //攻击在冷却
        {
            return;
        }
        if(!combatProps.m_readyToAttack)
        {
            ChangeAnimation(AnimationType.Attack);
            combatProps.m_readyToAttack = true;
            combatProps.m_anti_time = combatProps.m_anticipation;
        }

        combatProps.m_anti_time -= Time.deltaTime;
        if (combatProps.m_anti_time > 0) return; //前摇未结束

        if (combatProps.m_attackType == AttackType.Melee) //近战，直接命中
        {
            AttackOn(opProps.m_operationTarget, combatProps.m_damage);
        }
        else //远程，发射箭矢
        {
            PlayAudioClip(m_attackBeginClip);
            GameObject arrowObject = Instantiate(m_arrowPrefab, m_rigidbody.position + Vector2.up * 0.5f, Quaternion.identity);
            if(arrowObject.GetComponent<ArrowControl>() == null)
            {
                arrowObject.AddComponent<ArrowControl>();
            }
            ArrowControl arrowControl = arrowObject.GetComponent<ArrowControl>();
            arrowControl.Init(this.gameObject, opProps.m_operationTarget);
        }
        combatProps.m_attackCooldown = combatProps.m_attackInterval; //进入攻击CD
        combatProps.m_readyToAttack = false;
        combatProps.m_onRecovering = true;
        combatProps.m_reco_time = combatProps.m_recovery;
    }

    private void HoldOperation()
    {
        // 保持原位操作的具体实现
    }

    private void SpellOperation()
    {
        if(spellProps.m_currentSpell == null)
        {
            Debug.Log("spell null");
        }
        Debug.Log("m_acceptLayerMask, groundLayerMask = " + spellProps.m_currentSpell.m_acceptLayerMask + " , " + Constant.groundLayerMask);
        if((spellProps.m_currentSpell.m_acceptLayerMask &  Constant.groundLayerMask) != 0) //地点为目标
        {
            if (MathUtil.Distance(this.gameObject.transform.position, spellProps.m_spellTargetPosition) > spellProps.m_currentSpell.m_range)
            {
                MoveOperation(); //距离不够，移动
                return;
            }
        }
        else
        {
            if (MathUtil.Distance(this.gameObject, spellProps.m_spellTarget) > spellProps.m_currentSpell.m_range)
            {
                opProps.m_targetPosition = opProps.m_operationTarget.transform.position;
                MoveOperation(); //距离不够，移动
                return;
            }
        }
        spellProps.m_anti_time -= Time.deltaTime;
        StopOperation();
        if (spellProps.m_anti_time > 0) return; //继续等待前摇
        LaunchSpell();
        ChangeOpType(OperationType.Stop);
    }

    public void ReadyToSpell(Spell spell)
    {
        spellProps.m_currentSpell = spell;
        spellProps.m_anti_time = spell.m_anticipation;
        spellProps.m_spellTarget = opProps.m_operationTarget;
        spellProps.m_spellTargetPosition = opProps.m_targetPosition;
        opProps.m_operationType = OperationType.Spell;
        ChangeAnimation(AnimationType.Attack);
    }

    public void LaunchSpell() //施法
    {
        ChangeAnimation(AnimationType.Idle);
        Spell spell = spellProps.m_currentSpell;
        if (spell.m_acceptLayerMask == 0) //无目标
        {
            spell.Task(this);
        }
        else if ((spell.m_acceptLayerMask & Constant.groundLayerMask) != 0) //选择地面
        {
            spell.Task(this, spellProps.m_spellTargetPosition);
        }
        else if ((spell.m_acceptLayerMask & Constant.unitLayerMask) != 0) //选择单位
        {
            spell.Task(this, spellProps.m_spellTarget);
        }
        spellProps.m_currentSpell = null;
    }


    private void InvincibleOperation()
    {
        // 无敌操作的具体实现
    }

    public void ChangeAnimation(AnimationType animationType) //切换动画
    {
        if (m_animator == null || m_animationType == animationType) return;
        String cmd = animationType_str_dic[m_animationType] + "To" + animationType_str_dic[animationType]; //触发器指令
        m_animator.SetTrigger(cmd);
        m_animationType = animationType;
    }

    public void ChangeOpType(OperationType operationType)
    {
        if(operationType == OperationType.Attack)
        {
            combatProps.m_readyToAttack = false; //前摇重置
        }
        if(operationType == OperationType.Stop)
        {
            ChangeAnimation(AnimationType.Idle);
        }
        opProps.m_operationType = operationType;
    }

    public void AttackOn(GameObject target, float damage) //命中
    {
        if (target == null) //丢失对象，可能原因是已死亡或传送
        {
            if(opProps.m_operationTarget == target)
                ChangeOpType(UnitControl.OperationType.Stop); // 停止行动
            return;
        }
        
        UnitControl tControl = target.GetComponent<UnitControl>();
        CombatProperties tcProps = tControl.combatProps;

        PlayAudioClip(m_attackOnClip);
        if(combatProps.m_damage > tcProps.m_armor) //大于护甲，造成伤害
        {
            if (tControl.combatProps.m_health <= 0)
            {
                return;
            }
            tControl.combatProps.m_health -= damage - combatProps.m_armor;
            //Debug.Log(target.name + "受到攻击，剩余生命值为" + tControl.combatProps.m_health);
            if (tControl.combatProps.m_health <= 0)
            {
                tControl.Death();
            }
        }
    }

    private void PlayAudioClip(AudioClip audioClip)
    {
        if (audioClip == null) return;
        m_audioSource.Stop();
        m_audioSource.clip = audioClip;
        m_audioSource.Play();
    }

    // 当发生碰撞时调用
    void OnCollisionStay2D(Collision2D collision)
    {
        // 大幅减弱当前物体移动速度
        //opProps.m_targetPosition = (Vector2)this.transform.localPosition;
        //if(opProps.m_operationType == OperationType.Move)
        //    ChangeOpType(UnitControl.OperationType.Stop);
    }

    void Death() //血量为0死亡
    {
        //Debug.Log(name + " death");
        PlayAudioClip(m_deathClip);
        gameObject.layer = 2;
        if (gameObject.GetComponent<BoxCollider2D>())
        {
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }
        Destroy(m_healthBar); //摧毁生命条
        Destroy(this.gameObject, m_deathClip.length + 0.3f); //等待死亡动画和音效播放完
    }
}
