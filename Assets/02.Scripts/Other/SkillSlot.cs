using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SkillSlot : Slot
{
    public Skill m_Skill; //스킬 정보
    public string m_SkillName = "";
    public KeyCode m_KeyCode;
    public Image m_CoolImg; //쿨타임을 표기할 이미지
    public Text m_CoolTxt;

    //스킬 쿨타임
    public float m_CurrTime = -1.0f;
    public float m_CoolTime = 1.5f;


    public override void Awake()
    {
        base.Awake();
        
        m_CoolTxt = transform.Find("CoolTxt").GetComponent<Text>();
        m_CoolImg = transform.Find("CoolImg").GetComponent<Image>();
    }

    public void SkillSlotUpdate()
    {
        if(m_CurrTime > 0.0f)
        {
            m_CurrTime -= Time.deltaTime;

            m_CoolImg.gameObject.SetActive(true);
            m_CoolImg.fillAmount = m_CurrTime / m_CoolTime;
            m_CoolTxt.gameObject.SetActive(true);
            m_CoolTxt.text = m_CurrTime.ToString("F1") + "s";

            if(m_CurrTime <= 0.0f)
            {
                m_CoolImg.gameObject.SetActive(false);
                m_CoolTxt.gameObject.SetActive(false);
            }
        }
    }


    public void SetSlot(Skill a_Skill) 
    {   
        if (a_Skill == null)
        {         
            //슬롯 초기화
            m_SkillName = "";
            m_Skill = null;
            m_SlotImg.sprite = null;
            m_SlotImg.gameObject.SetActive(false);
            m_ItemCountTxt.text = "";
            m_ItemCountTxt.gameObject.SetActive(false);
            m_CoolImg.gameObject.SetActive(false);
            m_CoolTxt.gameObject.SetActive(false);
            bEmpty = true;
            return;
        }

        if (m_SlotType != SlotType.SkillRoot)
            if (SkillMgr.Inst.DicSkillSlots.ContainsKey(a_Skill))
            {
                //만약 
                if (SkillMgr.Inst.DicSkillSlots[a_Skill].m_Skill == a_Skill)    
                    SkillMgr.Inst.DicSkillSlots[a_Skill].SetSlot(null);
              
            }

        //슬롯 적용
        m_Skill = a_Skill;
        m_SkillName = a_Skill.m_SkillName;
        m_SlotImg.sprite = a_Skill.m_SkillImg;
        m_ItemCountTxt.text = "";
        m_SlotImg.gameObject.SetActive(true);
        m_CoolImg.gameObject.SetActive(false);
        m_CoolTxt.gameObject.SetActive(false);

        if (m_SlotType != SlotType.SkillRoot)
            SkillMgr.Inst.DicSkillSlots[a_Skill] = this;


        bEmpty = false;
    }



}
