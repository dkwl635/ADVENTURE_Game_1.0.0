using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NormalAttack : Skill
{  
    
    public GameObject eff;
    public float posy = -0.7f;

    //사거리 체크
    public float angleRange = 60f;
    public float distance = 2f;
    public bool isCollision = false;

    Color _blue = new Color(0f, 0f, 1f, 0.2f);
    Color _red = new Color(1f, 0f, 0f, 0.2f);

    Vector3 direction;

    float dotValue = 0f;


    private int m_SkillDamage
    {
        get { return m_Damage + player.m_PlayerAttPw; }
    }


   
    public override void UseSkill()
    {     
        if (m_CurrTime < 0.0f && player.bIsAttack == false)
        {      
            player.bIsAttack = true;
            m_CurrTime = m_CoolTime;
            StartCoroutine(SkillStart());
        }
        
    }

    public override void CoolTimeUpdate()
    {
        if (m_CurrTime > 0.0f)
            m_CurrTime -= Time.deltaTime;
    }

   
    public override IEnumerator SkillStart()
    {
        dotValue = Mathf.Cos(Mathf.Deg2Rad * (angleRange / 2));

        animator.SetTrigger(m_SkillName);    
        int combo = 0;
        bool attack = true;      
        animator.SetInteger("Combo", combo);

        playerTr.LookAt(SkillMgr.Inst.m_MousePos);
        eff.transform.position = player.weapon.m_EffPos.position;
        if (combo.Equals(0))
        {
            eff.SetActive(true);    
            while (true)
            {          
                yield return null;
                eff.transform.position = player.weapon.m_EffPos.position;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack01"))//교체 시간 대기                   
                    continue;

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)  // 특정 구간 전까지 반복   
                {
                    if (attack.Equals(true))
                    {
                        var hits = Physics.SphereCastAll(playerTr.position, distance, Vector3.up, 0.0f, m_SkillTargetLayer); // 공격범위안에 있는 콜리더 가져오기
                        if (hits.Length > 0) // 콜리더가 있다면
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                direction = hits[i].transform.position - playerTr.position;
                                if (Vector3.Dot(direction.normalized, playerTr.forward) > dotValue)
                                {
                                    MonsterCtrl monster = hits[i].collider.GetComponent<MonsterCtrl>();
                                    if (monster != null)
                                        monster.OnDamge(m_SkillDamage, player);
                                }
                            }
                        }
                        attack = false;

                    }
                }
                

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f)  // 특정 구간 전까지 반복   
                {
                 
                    if (Input.GetMouseButtonDown(0))
                    {
                        combo = 1;
                        animator.SetInteger("Combo", combo);
                    }
                }          
                else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)  // 애니메이션 끝나기 까지 루프   
                    break;
            }                           
        }
        yield return null;
        if (combo.Equals(1))
        {
            attack = true;

   
            while (true)
            {
                yield return null;
                eff.transform.position = player.weapon.m_EffPos.position;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack02"))//교체 시간 대기                                                
                    continue;

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)  // 특정 구간 전까지 반복   
                {                   
                    if (attack.Equals(true))
                    {
                        var hits = Physics.SphereCastAll(playerTr.position, distance, Vector3.up, 0.0f, m_SkillTargetLayer); // 공격범위안에 있는 콜리더 가져오기
                        if (hits.Length > 0) // 콜리더가 있다면
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                direction = hits[i].transform.position - playerTr.position;
                                if (Vector3.Dot(direction.normalized, playerTr.forward) > dotValue)
                                {
                                    MonsterCtrl monster = hits[i].collider.GetComponent<MonsterCtrl>();
                                    if (monster != null)
                                        monster.OnDamge(m_SkillDamage, player);
                                }
                            }
                        }
                        attack = false;
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.3f && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f )  // 특정 구간 전까지 반복   
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        combo = 2;
                        animator.SetInteger("Combo", combo);
                    }
                }

               else if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)  // 애니메이션 끝나기 까지 루프   
                    break;
            }   
          
        }
        yield return null;
        if (combo.Equals(2))
        {
            attack = true;
    
            while (true)
            {
                yield return null;
                eff.transform.position = player.weapon.m_EffPos.position;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("NormalAttack03"))//교체 시간 대기                                                
                    continue;

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f)  // 특정 구간 전까지 반복   
                {
                   
                    if (attack.Equals(true))
                    {
                        var hits = Physics.SphereCastAll(playerTr.position, distance, Vector3.up, 0.0f, m_SkillTargetLayer); // 공격범위안에 있는 콜리더 가져오기
                        if (hits.Length > 0) // 콜리더가 있다면
                        {
                            for (int i = 0; i < hits.Length; i++)
                            {
                                direction = hits[i].transform.position - playerTr.position;
                                if (Vector3.Dot(direction.normalized, playerTr.forward) > dotValue)
                                {
                                    MonsterCtrl monster = hits[i].collider.GetComponent<MonsterCtrl>();
                                    if (monster != null)
                                        monster.OnDamge(m_SkillDamage, player);
                                }
                            }
                        }
                        attack = false;
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)  // 애니메이션 끝나기 까지 루프   
                    break;
            }
          
        }
        m_CurrTime = m_CoolTime;
        eff.SetActive(false);
        
        yield return null;

        player.bIsAttack = false;
        yield break;
    }

    //private void OnDrawGizmos()
    //{
    //    if(playerTr != null)
    //    {        
    //        Handles.color = isCollision ? _red : _blue;
    //        Handles.DrawSolidArc(playerTr.position, Vector3.up, playerTr.forward, angleRange / 2, distance);
    //        Handles.DrawSolidArc(playerTr.position, Vector3.up, playerTr.forward, -angleRange / 2, distance);
    //    }
    //}
}

     
  