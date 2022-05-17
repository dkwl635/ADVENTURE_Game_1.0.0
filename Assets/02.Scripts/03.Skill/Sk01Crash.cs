using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sk01Crash : Skill
{
    Collider playerColl = null;
    public GameObject m_Ring = null;
    public GameObject m_Wave = null;
    private float m_radus = 1.5f;                //반지름
    private Vector3 m_tempPos = Vector3.zero;    //계산용
    private float m_Distance = 2.0f;                    //거리

    string m_CrashUp = "CrashUp";
    string m_CrashDown = "CrashDown";

    public override void Init(Player a_player)   //기본 셋팅
    {
        base.Init(a_player);
        playerColl = a_player.GetComponent<Collider>();

    }

    private float m_SkillDamage
    {
        get { return (m_Damage  * m_Lv) + player.m_PlayerAttPw; }
    }

    public override string SkillInfoTxt()
    {
        return "\n총 스킬 데미지 : " + m_SkillDamage.ToString();

    }

    public override void ShowSkillMark(Vector3 a_MouesDir)
    {
        base.ShowSkillMark(a_MouesDir);     

        if (!m_Show)
        {
            m_Ring.SetActive(false); //사거리 비표시
            return;
        }
           
      
        m_Ring.SetActive(true);
        m_tempPos = playerTr.position + a_MouesDir * m_Distance;
        m_tempPos.y += 0.4f;
        m_Ring.transform.position = m_tempPos;

    }


    public override IEnumerator SkillStart()
    {
        m_Show = false;
        playerTr.LookAt(SkillMgr.Inst.m_MousePos);
        animator.SetTrigger(m_SkillName);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(m_CrashUp))//교체 시간 대기                   
            yield return null;
   
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(m_CrashDown))//교체 시간 대기                   
            yield return null;
        
        //playerColl.isTrigger = true;
        rigidbody.mass = 1;
        rigidbody.AddForce(Vector3.up * 400.0f);    //점프 하는것 처럼 보이기
        animator.speed = 0.7f;                              //애니메이션 스피드 조정

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)  // 특정 구간 전까지 대기
            yield return null;     
        animator.speed = 1.0f;

        m_Show = false;
        m_Ring.SetActive(false); //사거리 비표시
        m_Wave.transform.position = playerTr.position + (playerTr.forward * m_Distance);     //웨이브 이펙트 위치 잡기
        m_Wave.SetActive(true);    //웨이브 이펙트 보여주기
        playerColl.isTrigger = false;
        SoundMgr.Inst.PlaySound("Sk_01");

        // 공격범위안에 있는 콜리더 가져오기
        RaycastHit[] hits = Physics.SphereCastAll(m_Wave.transform.position, m_radus,Vector3.up, 0, m_SkillTargetLayer);     
        rigidbody.mass = 1;
        if (hits.Length > 0) // 콜리더가 있다면
        {                          //데미지 적용
            for (int i = 0; i < hits.Length; i++)
            {
                hits[i].collider.GetComponent<MonsterCtrl>().OnDamge((int)m_SkillDamage, player);
            }
        }

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(m_CrashDown))//종료 까지 대기                   
            yield return null;

        player.bIsAttack = false;
        //애니메이션 종료시
        yield return new WaitForSeconds(0.5f);
        m_Wave.SetActive(false);   //웨이브 이펙트 끄기
                                          //공격끝남
        yield return null;
    }
}
