using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    protected Player player = null;           //플레이어 관련 
    protected Animator animator = null;   //애니메이터 
    protected Rigidbody rigidbody = null; //물리
    protected Transform playerTr = null;  //위치
    protected LayerMask m_SkillTargetLayer = -1;    //타겟 레이어
    public bool m_Show = false;                        //범위 확인
                                        
    
    public Sprite m_SkillImg;                           //스킬 UI 이미지
    public string m_SkillName = "빈 스킬";    //스킬이름
    public int m_Lv = 1;                                 //스킬 레벨
    public int m_NeedSP = 5;            //레벨 업 필요 스킬 포인트
    public int m_Damage = 10;   //기본 데미지
    public string m_SkillInfo; //스킬 설명
    public string SKillInfo
    {
        get { return m_SkillInfo + SkillInfoTxt(); }
    }

    //스킬 쿨타임
    public float m_CurrTime = -1.0f;
    public float m_CoolTime = 1.0f;

    static bool IsIput = false; //중복 키 입력을 막기 위해


    public virtual void Init(Player a_player)   //기본 셋팅
    {
        player = a_player;
        animator = a_player.animator;
        rigidbody = a_player.rigidbody;
        playerTr = a_player.transform;

       
        //타겟 레이어 셋팅
        m_SkillTargetLayer = 1 << LayerMask.NameToLayer("MONSTER");
    }

    public virtual string SkillInfoTxt()
    {
        return "";
    }

    public virtual void BoolShowMark()  //사거리를 보여줄지 체크
    {
       
        if (m_CurrTime < 0.0f && player.bIsAttack == false) //쿨타임 및 플레이가 공격 중인지 체크
        {
            if (SkillMgr.Inst.bIsPushSkill == true) //다른 스킬을 누루고 있으면
                return;
            else //SkillMgr.Inst.bIsPushSkill == false
                SkillMgr.Inst.bIsPushSkill = true;

            m_Show = true;  //스킬 범위 및 사거리 보여주기
        }
    }
    public virtual void ShowSkillMark(Vector3 a_MouesDir)
    {
        if (SkillMgr.Inst.bIsPushSkill == false)
        {
            m_Show = false;
            return;
        }//false 로 변경될경우 스킬 중단               
    }

    public virtual void UseSkill()  //스킬 사용 
    {   
        if (m_CurrTime < 0.0f && player.bIsAttack == false && m_Show)
        {
            player.bIsAttack = true;    //스킬 사용시 움직임을 막을 수 있음
            m_CurrTime = m_CoolTime;

            //슬롯에도 쿨타임 UI 보여주기
            if(SkillMgr.Inst.DicSkillSlots.ContainsKey(this) == true)
            {
                SkillMgr.Inst.DicSkillSlots[this].m_CurrTime = this.m_CurrTime;
                SkillMgr.Inst.DicSkillSlots[this].m_CoolTime = this.m_CoolTime;
            }   

            StartCoroutine(SkillStart());
            SkillMgr.Inst.bIsPushSkill = false; //돌려놓기
        }      
    }

    public virtual void CoolTimeUpdate()
    {
        if (m_CurrTime > 0.0f)
            m_CurrTime -= Time.deltaTime;
    }

    public virtual IEnumerator SkillStart()
    {      
        yield break;
    }

}
