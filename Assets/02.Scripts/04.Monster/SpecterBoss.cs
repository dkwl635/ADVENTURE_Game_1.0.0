using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpecterBoss : BossMonster
{
    [Header("Attakc01")]
    public GameObject Attack01RangeEff;
    public BoxCollider Attack01Size;
    [Header("Attakc02")]
    public GameObject Attack02RangeEff;
    public BoxCollider Attack02Size;
    public ParticleSystem[] Attack02ShineEffs;
    
    [Header("Attakc03")]
    public GameObject Attack03RangeEff;
    bool bAttak03 = false;

    public override void Start()
    {
        base.Start();       
    }
   
    public override IEnumerator Think()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.02f); // 0.1초 주기

            //실시간 거리 계산
            m_Dist = Vector3.Distance(m_Target.position, this.transform.position);  //타겟과의 거리

            if (m_MonsterState == MonsterState.Hit ||
                m_MonsterState == MonsterState.Attack
                ) //예외 처리
            {
                continue;
            }

            if (m_MonsterStatus.m_CurHp < 0)
            {
                m_MonsterState = MonsterState.Die;
            }
            else if (m_Dist <= m_AttackDist) //공격거리 범위 이내로 들어왔는지 확인
            {
                m_MonsterState = MonsterState.AttackIdle;

            }
            else if (m_Dist <= m_TraceDist) //추적거리 범위 이내로 들어왔는지 확인
            {
                m_MonsterState = MonsterState.Trace; //몬스터의 상태를 추적으로 설정              
            }
            else
            {
                m_MonsterState = MonsterState.Idle;   //몬스터의 상태를 idle 모드로 설정            
            }


       
        }
    }
    public override IEnumerator Action()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (m_MonsterState == MonsterState.Hit) // 히트 상태일때
            {
                yield return new WaitForSeconds(0.5f);  //0.5초 대기
                m_MonsterState = MonsterState.AttackIdle;
            }
            else if (m_MonsterState == MonsterState.Idle)
            {
                navMeshAgent.isStopped = true;  //추적 정지
                animator.SetBool("IsMove", false);
            }
            else if (m_MonsterState == MonsterState.Trace)
            {
                if (navMeshAgent.isStopped)
                    navMeshAgent.isStopped = false;     //추적 시작

                navMeshAgent.SetDestination(m_Target.position); //타겟 지정             
                animator.SetBool("IsMove", true);
            }
            else if (m_MonsterState == MonsterState.AttackIdle) //공격사거리에 들어온 상태
            {
                animator.SetBool("IsMove", false);
                if (navMeshAgent != null)
                    navMeshAgent.isStopped = true;  //추적 정지
                                        
                m_MonsterState = MonsterState.Attack; // 공격 상태 바꾸기
            }
            else if (m_MonsterState == MonsterState.Attack) //공격 상태
            {
                int rand = Random.Range(0, 101);

                if (rand > 0 && rand <= 50)
                    m_BossAttackType = BossAttackType.Attack01;
               else if (rand > 50 && rand <= 80)
                    m_BossAttackType = BossAttackType.Attack02;
                else if (rand > 80 && rand <= 100)
                    m_BossAttackType = BossAttackType.Attack03;

              
                transform.LookAt(m_Target);

                if (m_BossAttackType == BossAttackType.Attack01)         //1번 공격 기본공격
                {
                    animator.SetTrigger("Attack01");        //애니메이션 실행
                    Attack01RangeEff.SetActive(true);         //공격범위 보여주기
                    animator.speed = 0.1f;                      //공격속도 낮추기
                    yield return new WaitForSeconds(0.5f);
                    animator.speed = 1.0f;                      //본 공격속도
                    Attack01RangeEff.SetActive(false);          //공격범위 숨기기
                    yield return new WaitForSeconds(0.3f);

                    SoundMgr.Inst.PlaySound("SpecterBossAttack_01");
                    //공격
                    RaycastHit[] hits = Physics.BoxCastAll(Attack01RangeEff.transform.position, Attack01Size.size * 0.5f, Vector3.up, transform.rotation, 0.5f, 1 << LayerMask.NameToLayer("PLAYER"));
                    if (hits.Length > 0) // 타겟 충돌체 찾기
                    {                     
                        Player player = hits[0].collider.GetComponent<Player>();
                        float damagme = m_MonsterStatus.m_AttPw *Random.Range(0.9f, 1.4f);
                        player.OnDamge((int)damagme);  //데미지적용
                    }


                    yield return new WaitForSeconds(0.7f);  //대기 후 끝
                }
                else if (m_BossAttackType == BossAttackType.Attack02) //전방 번개 공격
                {
                    animator.SetTrigger("Attack02");
                    Attack02RangeEff.SetActive(true);
                    yield return new WaitForSeconds(2.4f);
                    Attack02RangeEff.SetActive(false);                  //공격범위 숨기기
                   
                    for (int i = 0; i < Attack02ShineEffs.Length; i++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        if (!Attack02ShineEffs[i].gameObject.activeSelf)
                            Attack02ShineEffs[i].gameObject.SetActive(true);

                        Attack02ShineEffs[i].Play();
                        SoundMgr.Inst.PlaySound("SpecterBossAttack_02");
                        if (i == Attack02ShineEffs.Length / 2)
                        {
                            //공격
                            RaycastHit[] hits = Physics.BoxCastAll(Attack02RangeEff.transform.position, Attack02Size.size * 0.5f, Vector3.up, transform.rotation, 0.5f, 1 << LayerMask.NameToLayer("PLAYER"));
                            if (hits.Length > 0) // 타겟 충돌체 찾기
                            {
                                Player player = hits[0].collider.GetComponent<Player>();
                                float damagme = m_MonsterStatus.m_AttPw * Random.Range(0.9f, 1.4f);
                                player.OnDamge((int)damagme);  //데미지적용
                            }
                        }
                    }

                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < Attack02ShineEffs.Length; i++)
                    { Attack02ShineEffs[i].gameObject.SetActive(false); }

                        yield return new WaitForSeconds(0.5f);  //대기 후 끝
                }
                else if (m_BossAttackType == BossAttackType.Attack03)       // 전방 돌격
                {
                    Attack03RangeEff.SetActive(true);
                    yield return new WaitForSeconds(0.7f);
                    bAttak03 = true;              
                    Attack03RangeEff.SetActive(false);
                    float range = 15.0f;
                    float temp = 0.0f;
                    Vector3 curVector = transform.position;
                    Vector3 nextpos = transform.position + (transform.forward * range);

                    animator.SetTrigger("Attack03");
                    while (temp < 1)    //목표 돌진 
                    {
                        Collider.isTrigger = true;
                        transform.position = Vector3.Lerp(curVector, nextpos, temp);
                        temp += Time.deltaTime;
                        yield return null;
                    }
               
                    transform.LookAt(curVector);
                    Attack03RangeEff.SetActive(true);

                    yield return new WaitForSeconds(0.7f);             
                    Attack03RangeEff.SetActive(false);
                    temp = 0;
                    transform.forward = (curVector - transform.position).normalized;

                    animator.SetTrigger("Attack03");
                    while (temp < 1) // //목표 돌진 
                    {
                        transform.position = Vector3.Lerp(nextpos, curVector, temp);
                        temp += Time.deltaTime;
                        yield return null;
                    }
                    Collider.isTrigger = false;
                    bAttak03 = false;
                    yield return new WaitForSeconds(0.5f);  //대기 후 끝

                    
                }
             
                
                m_MonsterState = MonsterState.Idle;   //상태 전환              
            }
            else if (m_MonsterState == MonsterState.Die)
            {
                Die();
            }

        }
    }


    public override void Die()
    {
        AllOffEffect(); //모든이펙트 끄기
        navMeshAgent.isStopped = true; //추적정지
        animator.SetTrigger("Die");         //죽는 애니메이션 적용
        animator.speed = 1.0f;                  //애니메이션 속도 초기화
        Collider.enabled = false; //충돌체 끄기
        navMeshAgent.enabled = false;
        //퀘스트 목록 확인
        QuestMgr.Inst.CheckKillQuest(m_MonsterId);

        StopAllCoroutines();
        Invoke("ObjDestory", 2.0f);
    }

    public override void OnDamge(int a_Damage = 0, Player a_Attacker = null)
    {
        m_MonsterStatus.m_CurHp -= a_Damage; //데미지 적용
        Vector2 canvaspos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        InGameMgr.Inst.SpanwDamageTxt(canvaspos, TxtType.Damage, a_Damage); //데미지 숫자 이펙트

        m_HpBarCtrl.SetHpBar(m_MonsterStatus.m_CurHp, m_MonsterStatus.m_MaxHp);    //hpbar 적용

        if (navMeshAgent != null)
            navMeshAgent.isStopped = true;  //추적정지

        if (m_MonsterStatus.m_CurHp <= 0) //체력이 0이면
        {
            Die();

            a_Attacker.AddExp(m_MonsterStatus.m_CurExp);
            a_Attacker.m_PlayerInventory.AddCoin(2000);

        }
        else
        {
            if (m_MonsterState != MonsterState.Attack)
            {
                animator.SetTrigger("Hit");                 //맞는 애니메이션 적용                  
            }
            StartCoroutine(SetHitMtrl());
        }
    }

    public override void Spawn()
    {       
        transform.position = m_SpawnPos;
        m_HpBarCtrl.SetHpBar(m_MonsterStatus.m_CurHp, m_MonsterStatus.m_MaxHp);    //HP바 적용
        Collider.enabled = true; //충돌체 켜기
        navMeshAgent.enabled = true;    //네비 켜기
        m_MonsterState = MonsterState.Idle;   //상태 변경


        StartCoroutine(Think());
        StartCoroutine(Action());


    }

    void AllOffEffect()
    {
        Attack01RangeEff.SetActive(false);
        Attack02RangeEff.SetActive(false);
        Attack03RangeEff.SetActive(false);

        foreach (var Eff in Attack02ShineEffs)
        {
            Eff.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && bAttak03)
        {
            Player player = other.GetComponent<Player>();         
            float damagme = m_MonsterStatus.m_AttPw * Random.Range(0.9f, 1.4f);
            player.OnDamge((int)damagme);  //데미지적용
        }
    }


    public override void ObjDestory()
    {     
        if(DieEvent != null)
        {
            DieEvent();
            DieEvent = null;
        }

        Destroy(this.gameObject);
    }
}
