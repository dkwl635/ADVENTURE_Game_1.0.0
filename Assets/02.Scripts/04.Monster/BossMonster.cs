using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossAttackType
{
    None,
    Attack01,
    Attack02,
    Attack03,
    Count,
}


public class BossMonster : MonsterCtrl
{
    [SerializeField] protected BossAttackType m_BossAttackType = BossAttackType.None;

    public virtual void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();                    //충돌체  
        animator = GetComponent<Animator>();                                          //에니메이션              
        navMeshAgent = GetComponent<NavMeshAgent>();   //네비메쉬

        m_HpBarObj = InGameMgr.Inst.SetHpBarObj();           //체력바 생성
        m_HpBarCtrl = m_HpBarObj.GetComponent<MonHpBarCtrl>();         //체력바 컴포넌트
        m_Skin = GetComponentInChildren<SkinnedMeshRenderer>(); //스킨 찾아오기

        navMeshAgent.stoppingDistance = m_AttackDist - 0.1f;
        if (navMeshAgent.speed == 0)
            navMeshAgent.speed = m_Speed;

        m_OrginMtrl = m_Skin.material;           //원래 쓰는 머터리얼 

        m_Target = GameObject.Find("Player").GetComponentInChildren<Player>().transform;           //타겟 설정

        Spawn();

        StartCoroutine(Think());
        StartCoroutine(Action());
    }

    public virtual IEnumerator Think() { yield return null; } // 생각하기 무슨 행동을 할지 생각하는 코루틴 함수      
    public virtual IEnumerator Action() { yield return null; }   // 상태에 맞는 행동을 함
 
    public override void ObjDestory()
    {       
        Destroy(m_HpBarObj);
        Destroy(this.gameObject);
    }
    
}
