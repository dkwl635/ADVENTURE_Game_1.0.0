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
    Attack04,
    Count,
}


public class BossMonster : MonsterCtrl
{
    [SerializeField] protected BossAttackType m_BossAttackType = BossAttackType.None;
    public Event die;

    public virtual void Start()
    {
        Collider = GetComponentInChildren<Collider>();                    //충돌체  
        animator = GetComponent<Animator>();                                          //에니메이션              
        navMeshAgent = GetComponent<NavMeshAgent>();   //네비메쉬      
        m_Skin = GetComponentInChildren<SkinnedMeshRenderer>(); //스킨 찾아오기
     
        navMeshAgent.stoppingDistance = m_AttackDist - 0.1f;
        if (navMeshAgent.speed == 0)
            navMeshAgent.speed = m_Speed;

        m_OrginMtrl = m_Skin.material;           //원래 쓰는 머터리얼 

        m_Target = GameObject.Find("Player").GetComponentInChildren<Player>().transform;           //타겟 설정
      
    }

   
   
    public virtual IEnumerator Think() { yield return null; } // 생각하기 무슨 행동을 할지 생각하는 코루틴 함수      
    public virtual IEnumerator Action() { yield return null; }   // 상태에 맞는 행동을 함
 
  
}
