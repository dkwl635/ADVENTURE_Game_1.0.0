﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crab : MonsterCtrl
{
    public GameObject m_AttackPos;

    float m_AttackDelay = 0.0f;  //1.5f 공격 대기시간
    float m_AttackTime = 0.3f;  //공격 타이밍(공격 시작 후 0.3f 뒤 공격)
    float m_HitDelay = 0.0f;    //0.5f;
    float m_SpawnTime = 0.0f;   //5.0f

    public List<int> m_DropList;

    private void Start()
    {
        boxCollider = GetComponentInChildren<BoxCollider>();                    //충돌체
        animator = GetComponent<Animator>();                                          //애니메이터             
        navMeshAgent = GetComponent<NavMeshAgent>();   //네비
       
        m_HpBarObj = InGameMgr.Inst.SetHpBarObj();           //체력바
        m_HpBarCtrl = m_HpBarObj.GetComponent<MonHpBarCtrl>();    //체력바 컨트롤      

        m_Skin = GetComponentInChildren<SkinnedMeshRenderer>(); //스킨 머터리얼 적용을 위한
        m_OrginMtrl = m_Skin.material;                                             //원래 머터리얼

        m_Target = GameObject.Find("Player").transform;           //타겟

        //navMeshAgent.updatePosition = false;
        if (navMeshAgent.speed == 0)
            navMeshAgent.speed = m_Speed;


        Spawn();     
    }
    private void LateUpdate()
    {
        m_HpBarObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 3.0f);      
    }
    
    private void FixedUpdate()
    {
        Action_FixedUpdate();
        Think_FixedUpdate();
    }

    public void Think_FixedUpdate()
    {
        if (m_MonsterStatus.m_CurHp <= 0)
            return;

        if (m_MonsterState == MonsterState.Hit || m_MonsterState == MonsterState.Attack) //맞을 때와 공격중일때는 무시   
        {            
            return;
        }
      
        //상태 적용
        m_Dist = Vector3.Distance(m_Target.position, this.transform.position);  //거리 계산        
        if (m_Dist <= m_AttackDist) //공격거리 안에 있으면     
            m_MonsterState = MonsterState.AttackIdle;//공격준비상태      
        else if (m_Dist <= m_TraceDist) //추격거리 안에 있으면       
            m_MonsterState = MonsterState.Trace; //추격 상태로 전환       
        else      
            m_MonsterState = MonsterState.Idle;   //가만히 있기               
    }
    public void Action_FixedUpdate()
    {
       
        if (m_MonsterState == MonsterState.Idle)//가만히 있을때
        {
            OnOffNav(false);    //네비 끄기
            animator.SetBool("IsMove", false);  //애니메이션 적용
        }
        else if (m_MonsterState == MonsterState.Trace)//추격 상태
        {
            OnOffNav(true); //내비 키기
                            //떨림 현상을 막기위해                
            //transform.position = navMeshAgent.nextPosition;

            animator.SetBool("IsMove", true);
        }
        else if (m_MonsterState == MonsterState.AttackIdle) //공격준비상태
        {
            animator.SetBool("IsMove", false);
            OnOffNav(false);

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit")) //만약 맞고있으면
                return;
        
            if (m_AttackDelay > 0.0f)
            {
                m_AttackDelay -= Time.fixedDeltaTime;               
            }
            else          
            {
                //공격         
                this.transform.LookAt(m_Target.transform);  //타겟 바라보기
                animator.SetTrigger("Attack");                      //애니메이션 적용
                m_AttackTime = 0.3f;    //공격 타이밍
                m_MonsterState = MonsterState.Attack;       // 공격 상태                 
            }
        }
        else if (m_MonsterState == MonsterState.Attack) //공격 중
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("GetHit")) //만약 맞고있으면           
            {
                m_MonsterState = MonsterState.Idle;
                return;
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                return;
                
            if (m_AttackTime > 0.0f)
            {
                m_AttackTime -= Time.fixedDeltaTime;
                if (m_AttackTime <= 0.0f)
                {
                    //공격
                    RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one * 0.5f, transform.forward, transform.rotation, 1.0f, 1 << LayerMask.NameToLayer("PLAYER"));
                    if (hits.Length > 0) // 타겟 충돌체 찾기
                    {                                          
                        Player player = hits[0].collider.GetComponent<Player>();
                        player.OnDamge(m_MonsterStatus.m_AttPw);  //데미지적용

                        InGameMgr.Inst.SpawnFxEffect(m_AttackPos.transform.position, "FX_BloodSplatter");
                    }
                }
            }
         
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8f)
            {

                m_AttackDelay = 1.5f;
                m_MonsterState = MonsterState.Idle; //상태 전환
            }
                         
        }
        else if (m_MonsterState == MonsterState.Hit) // 맞았을때
        {
            if (m_HitDelay > 0.0f)
            {
                m_HitDelay -= Time.fixedDeltaTime;
                if (m_HitDelay <= 0.0f)
                {
                    m_MonsterState = MonsterState.Idle;
                }
            }
        }
        else if (m_MonsterState == MonsterState.Die) //죽으면
        {
            if (m_SpawnTime > 0.0f)
            {
                m_SpawnTime -= Time.fixedDeltaTime;
                if (m_SpawnTime <= 0.0f)
                {
                    animator.SetTrigger("Respawn");
                    Spawn();
                }
            }
        }
    }
   
    public override void OnDamge(int a_Damage = 0, Player a_Attacker = null)
    {
        if (m_MonsterStatus.m_CurHp <= 0)
            return;

        if (m_Attacker != a_Attacker)
            m_Attacker = a_Attacker;

        m_MonsterStatus.m_CurHp -= a_Damage; //데미지 적용
        InGameMgr.Inst.SpanwDamageTxt(m_HpBarObj.transform.position, a_Damage, TxtType.Damage); //데미지 텍스트 출력
        m_HpBarCtrl.SetHpBar(m_MonsterStatus.m_CurHp, m_MonsterStatus.m_MaxHp);    //체력바 적용

        OnOffNav(false);

        if (m_MonsterStatus.m_CurHp <= 0) // 체력 0 밑으로
        {
            Die();
            return;
        }
        else
        {
            animator.SetTrigger("Hit");                 //데미지 애니메이션 적용
            m_MonsterState = MonsterState.Hit;  //상태 전환
            m_HitDelay = 0.5f;
            StartCoroutine(SetHitMtrl());              //히트 코루틴 시작
        }
     
    }

    public override void Spawn()
    {
        m_MonsterStatus.m_CurHp = m_MonsterStatus.m_MaxHp;  
        m_HpBarCtrl.SetHpBar(m_MonsterStatus.m_CurHp, m_MonsterStatus.m_MaxHp);    //체력바 적용
        boxCollider.enabled = true; //충돌체 켜기
        navMeshAgent.enabled = true;
        OnOffNav(true); //네비 켜기
        transform.position = m_SpawnPos;        //스폰위치적용       
        navMeshAgent.nextPosition = m_SpawnPos;
        m_MonsterState = MonsterState.Idle;   //상태 전화
    }

    public override void Die()
    {
        m_MonsterState = MonsterState.Die;  //상태 전환
        m_HpBarObj.SetActive(false);     //체력바 끄기
        animator.SetTrigger("Die");         //애니메이션 적용      
        boxCollider.enabled = false; //충돌체 끄기    
        OnOffNav(false);
        navMeshAgent.enabled = false;
        m_SpawnTime = 5.0f;



        //만약 죽인 사람이 있으며 //보상
        if (m_Attacker != null)
        {           
            int itemnum = Random.Range(0, m_DropList.Count);
            ItemMgr.Inst.SpawnDropItem(transform.position, m_DropList[itemnum], 1); //아이템 떨구기
            int coin = Random.Range(m_MinCoin, m_MaxCoin);              //코인
            m_Attacker.m_PlayerInventory.AddCoin(coin);   //돈주기
            m_Attacker.AddExp(10);
      
            QuestMgr.Inst.CheckKillQuest(m_MonsterId);
        }

     
    }

    void OnOffNav(bool bOnOff)  //네비 OnOff
    {
        if (navMeshAgent == null)
            return;

        if (!navMeshAgent.enabled)
            return;

        if (bOnOff == true)  //On
        {   
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(m_Target.position);        
        }
        else//Off
        {              
           
            navMeshAgent.SetDestination(transform.position);
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;     
        }
    }

    public override void ObjDestory()   //오브젝트 제거용
    {
        Destroy(m_HpBarObj);
        Destroy(this.gameObject);
    }
}
