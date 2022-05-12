using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sk03DefUp : Skill
{
    public GameObject m_prefab;
    public int m_AddDefPw = 20; //상승치

    int m_OrginDefPower = 0; //원래 방어력 저장용
    public float m_DurationTime;    //지속시간

    public override string SkillInfoTxt()
    {
        return "\n방어력 상승치 : " + m_AddDefPw + "\n지속시간 : " + m_DurationTime.ToString() + "초";
    }


    public override void UseSkill()  //스킬 사용 
    {//이스킬은 스킬사용중에도 움직일 수 있음

        if (m_CurrTime < 0.0f && !player.bIsAttack)
        {         
          m_CurrTime = m_CoolTime;

            //슬롯에도 쿨타임 UI 보여주기
            if (SkillMgr.Inst.DicSkillSlots.ContainsKey(this) == true)
            {
                SkillMgr.Inst.DicSkillSlots[this].m_CurrTime = this.m_CurrTime;
                SkillMgr.Inst.DicSkillSlots[this].m_CoolTime = this.m_CoolTime;
            }
            StartCoroutine(SkillStart());       
        }
    }

    public override IEnumerator SkillStart()
    {
        //버프 시작
        m_OrginDefPower = player.m_PlayerStatus.m_DefPw;
        player.m_PlayerStatus.m_DefPw += m_AddDefPw;


        m_prefab.transform.SetParent(playerTr);
        m_prefab.transform.localPosition = Vector3.zero;
        m_prefab.transform.position += Vector3.up;
        m_prefab.SetActive(true);
        yield return new WaitForSeconds(m_DurationTime);
        //버프 끝     
        m_prefab.SetActive(false);
        player.m_PlayerStatus.m_DefPw = m_OrginDefPower;
 


        yield return null;
    }

}
