using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpecterBoss : BossMonster
{
    public GameObject Attack01RangeEff;
    public Vector3 Attack01Range;

    public GameObject Attack02RangeEff;
    public Vector3 Attack02Range;
    public GameObject[] Attack02ShineEffs;

    public GameObject Attack03RangeEff;
    public Vector3 Attack03Range;

    public override void Start()
    {
        base.Start();       
    }
    private void LateUpdate()
    {
        //체력바 위치 선정
        m_HpBarObj.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 4.0f);
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
                yield return new WaitForSeconds(0.5f);  // 공격사거리에 들어오면 0.5초 후에 공격
                if (m_MonsterState != MonsterState.AttackIdle)  //만약 대기후에 공격사거리에서 벗어나면 초기화
                    continue;

                //this.transform.LookAt(m_Target.transform);  //공격타이밍에 타겟 바라보기
                //animator.SetTrigger("Attack");                      //애니메이션 실행
                m_MonsterState = MonsterState.Attack; // 공격 상태 바꾸기
            }
            else if (m_MonsterState == MonsterState.Attack) //공격 상태
            {
                m_BossAttackType = (BossAttackType)Random.Range((int)BossAttackType.Attack01, (int)BossAttackType.Count);
                transform.LookAt(m_Target);

                if (m_BossAttackType == BossAttackType.Attack01)         //1번 공격 기본공격
                {
                    animator.SetTrigger("Attack01");        //애니메이션 실행
                    Attack01RangeEff.SetActive(true);         //공격범위 보여주기
                    animator.speed = 0.1f;                      //공격속도 낮추기
                    yield return new WaitForSeconds(1.0f);
                    animator.speed = 1.0f;                      //본 공격속도
                    Attack01RangeEff.SetActive(false);          //공격범위 숨기기
                    RaycastHit[] hits = Physics.BoxCastAll(Attack01RangeEff.transform.position, Attack01Range * 0.5f, transform.forward, transform.rotation, 0.0f, 1 << LayerMask.NameToLayer("PLAYER"));        // 공격범위안에 있는 콜리더 가져오기
                    if (hits.Length > 0) // 콜리더가 있다면
                    {
                        for (int i = 0; i < hits.Length; i++)
                        {
                        }

                    }

                    yield return new WaitForSeconds(0.5f);  //대기 후 끝
                }
                else if (m_BossAttackType == BossAttackType.Attack02) //전방 번개 공격
                {
                    animator.SetTrigger("Attack02");
                    Attack02RangeEff.SetActive(true);
                    yield return new WaitForSeconds(2.4f);
                    Attack02RangeEff.SetActive(false);                  //공격범위 숨기기
                    RaycastHit[] hits = Physics.BoxCastAll(Attack02RangeEff.transform.position, Attack02Range * 0.5f, transform.forward, transform.rotation, 0.0f, 1 << LayerMask.NameToLayer("PLAYER"));        // 공격범위안에 있는 콜리더 가져오기
                    if (hits.Length > 0) // 콜리더가 있다면
                    {                          //데미지 적용

                    }

                    for (int i = 0; i < Attack02ShineEffs.Length; i++)
                    {
                        yield return new WaitForSeconds(0.05f);
                        Attack02ShineEffs[i].SetActive(true);
                    }

                    for (int i = 0; i < Attack02ShineEffs.Length; i++)
                    {
                        yield return new WaitForSeconds(0.3f);
                        Attack02ShineEffs[i].SetActive(false);
                    }

                    yield return new WaitForSeconds(0.5f);  //대기 후 끝
                }
                else if (m_BossAttackType == BossAttackType.Attack03)       // 전방 돌격
                {
                    Attack03RangeEff.SetActive(true);
                    yield return new WaitForSeconds(1.5f);
                    boxCollider.isTrigger = true;
                    Attack03RangeEff.SetActive(false);
                    float range = 15.0f;
                    float temp = 0.0f;
                    Vector3 curVector = transform.position;
                    Vector3 nextpos = transform.position + (transform.forward * range);

                    animator.SetTrigger("Attack03");
                    while (temp < 1)    //목표 돌진 
                    {
                        transform.position = Vector3.Lerp(curVector, nextpos, temp);


                        temp += Time.deltaTime;
                        yield return null;
                    }

                    boxCollider.isTrigger = false;
                    transform.LookAt(curVector);
                    Attack03RangeEff.SetActive(true);

                    yield return new WaitForSeconds(1.0f);

                    boxCollider.isTrigger = true;
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
                    boxCollider.isTrigger = false;

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
        boxCollider.enabled = false; //충돌체 끄기
        navMeshAgent.enabled = false;
        StopAllCoroutines();
        Invoke("ObjDestory", 2.0f);
    }

    public override void OnDamge(int a_Damage = 0, Player a_Attacker = null)
    {
        m_MonsterStatus.m_CurHp -= a_Damage; //데미지 적용
        InGameMgr.Inst.SpanwDamageTxt(m_HpBarObj.transform.position, a_Damage, TxtType.Damage); //데미지 숫자 이펙트
        m_HpBarCtrl.SetHpBar(m_MonsterStatus.m_CurHp, m_MonsterStatus.m_MaxHp);    //hpbar 적용

        if (navMeshAgent != null)
            navMeshAgent.isStopped = true;  //추적정지

        if (m_MonsterStatus.m_CurHp <= 0) //체력이 0이면
        {
            Die();
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
        m_MonsterStatus.SetStatue(1, 0, 500, 30, 10);
        transform.position = m_SpawnPos;
        m_HpBarCtrl.SetHpBar(m_MonsterStatus.m_CurHp, m_MonsterStatus.m_MaxHp);    //HP바 적용
        boxCollider.enabled = true; //충돌체 켜기        
        m_MonsterState = MonsterState.Idle;   //상태 변경
    }

    void AllOffEffect()
    {
        Attack01RangeEff.SetActive(false);
        Attack02RangeEff.SetActive(false);
        Attack03RangeEff.SetActive(false);

        foreach (var Eff in Attack02ShineEffs)
        {
            Eff.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

        }
    }

}
