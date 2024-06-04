using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant : MonoBehaviour
{
    public static Constant instance;
    public static int groundLayer = 1, destructiveLayer = 2, enemyLayer = 6, friendLayer = 7, neutralLayer = 8, playerLayer = 9;
    public static int groundLayerMask = 1 << 1, destructiveLayerMask = 1 << 2, enemyLayerMask = 1 << 6, friendLayerMask = 1 << 7, 
        neutralLayerMask = 1 << 8, playerLayerMask = 1 << 9;
    public static int kindLayerMask = friendLayerMask + playerLayerMask, evilLayerMask = enemyLayerMask,
        unitLayerMask = kindLayerMask + evilLayerMask + neutralLayer;
    public static Dictionary<int, int> enermyMaskDictionary = new Dictionary<int, int>() //对于不同阵营的不同敌人表
    {
        { playerLayer, evilLayerMask },
        { friendLayer, evilLayerMask },
        { enemyLayer, kindLayerMask },
        { neutralLayer, 0},
    };

    public GameObject energyExplosionPrefab; //能量爆发特效
    public GameObject tinyExplosionPrefab; //小爆炸特效
    public GameObject bigExplosionPrefab; //大爆炸特效
    public GameObject plasmaExplosionPrefabs; //闪电爆炸特效
    public GameObject tinyFlamePrefab; //小火焰特效
    public GameObject flameStream; //火焰爆发特效
    public GameObject sparksPrefab; //火花特效
    public GameObject fireBallPrefab; //大火球

    void Awake()
    {
        instance = this;
    }

    public static void LaunchParticle(GameObject particlePrefab, Vector2 position)
    {
        GameObject particleObject = Instantiate(particlePrefab, position, Quaternion.identity); //创建特效
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();

        // 确保粒子系统组件存在
        if (particleSystem == null)
        {
            Debug.LogError("ParticleSystem component is missing on m_particleObject!");
            return;
        }
        var main = particleSystem.main;
        particleSystem.Play();
        Destroy(particleObject, main.duration - 0.2f); //根据持续时间来销毁粒子
    }
}