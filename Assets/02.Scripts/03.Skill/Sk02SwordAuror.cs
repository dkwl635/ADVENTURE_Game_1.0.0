using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sk02SwordAuror : Skill
{   
    
    public GameObject m_SkillMark = null;   //범위 표시 마크
    public GameObject m_SkillSwordAfterimagePrefab = null;  //투사체
    private Vector3 m_tempPos = Vector3.zero;       //계산용
    private float m_Range = 2.5f;                    //사거리
    private float m_ZOffset = 3.0f;

    private float m_SkillDamage
    {
        get { return (m_Damage * m_Lv) + player.m_PlayerAttPw * 0.8f; }
    }

    public override string SkillInfoTxt()
    {
        return  "\n총 스킬 데미지 : " + m_SkillDamage.ToString();
    }

    public override void ShowSkillMark(Vector3 a_MouesDir)
    {
        base.ShowSkillMark(a_MouesDir);


        if (!m_Show)
        {
            m_SkillMark.SetActive(false); //사거리 비표시
            return;
        }


        m_SkillMark.SetActive(true);
        m_tempPos = playerTr.position + a_MouesDir * m_ZOffset;
        m_tempPos.y += 0.4f;
        m_SkillMark.transform.SetPositionAndRotation(m_tempPos, Quaternion.LookRotation(a_MouesDir));
        m_SkillMark.transform.position = m_tempPos;
    }


    public override IEnumerator SkillStart()
    {
        m_Show = false;
        playerTr.LookAt(SkillMgr.Inst.m_MousePos);
        animator.SetTrigger(m_SkillName);

        m_SkillMark.SetActive(false);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(m_SkillName))//교체 시간 대기                   
            yield return null;

        SoundMgr.Inst.PlaySound("Sk_02");
        GameObject effect = (GameObject)Instantiate(m_SkillSwordAfterimagePrefab);
        effect.transform.SetPositionAndRotation(playerTr.position + Vector3.up, playerTr.rotation);
        SkillEffect skilleffect = effect.GetComponent<SkillEffect>();
        if (skilleffect != null)
        {
            skilleffect.InitSkillEffect((int)m_SkillDamage, player, 0.4f);
            skilleffect.rigidbody.velocity = skilleffect.transform.forward * 20.0f;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(m_SkillName))//종료 까지 대기                   
            yield return null;

        player.bIsAttack = false;
        yield return null;
    }
}
