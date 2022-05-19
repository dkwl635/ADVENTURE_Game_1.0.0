
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillRoot : MonoBehaviour
{
    public Skill m_Skill; //가지고 있는 스킬 아이템
    public SkillSlot m_SkillSlot; //스킬을 등록할수 있도록 하는 스킬 슬롯
    public Text m_SkillName_Txt; // 스킬 이름 txt
    public Text m_SkillLv_Txt;  //스킬 레벨 txt

    public Text m_NeedSP_Txt; //필요 스킬포인트 txt
    public Button m_Up_Btn; //스킬 레벨 업 버튼

    public Button m_Down_Btn; //스킬 레벨 다운 버튼


    private void Awake()
    {
        m_SkillSlot = GetComponentInChildren<SkillSlot>();
        m_SkillName_Txt = transform.Find("SkillNameTxt").GetComponent<Text>();
        m_SkillLv_Txt = transform.Find("SkillLvTxt").GetComponent<Text>();
        m_NeedSP_Txt = transform.Find("NeedSkillPointTxt").GetComponent<Text>();
        m_Up_Btn = transform.Find("UpBtn").GetComponent<Button>();
        m_Down_Btn = transform.Find("DownBtn").GetComponent<Button>();
    
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_Skill != null)
            m_SkillSlot.SetSlot(m_Skill);

        if (m_SkillName_Txt != null)
            m_SkillName_Txt.text = m_Skill.m_SkillName;

        if (m_SkillLv_Txt != null)
            m_SkillLv_Txt.text = "Lv " + m_Skill.m_Lv;

        if (m_NeedSP_Txt != null)
            m_NeedSP_Txt.text = m_Skill.m_NeedSP.ToString();

        if (m_Up_Btn != null)
            m_Up_Btn.onClick.AddListener(SkillUpBtn);

        if (m_Down_Btn != null)
            m_Down_Btn.onClick.AddListener(SkillDownBtn);

        Refresh();
        SkillMgr.Inst.RefershSkillRoot += Refresh;

    }

   public void Refresh() // 
   {
        if (!SkillMgr.Inst.m_SkillUIPanel.activeSelf)
            return;

        if(SkillMgr.Inst.m_SkillPoint >= m_Skill.m_NeedSP)
        {   
            m_Up_Btn.gameObject.SetActive(true);
        }
        else
            m_Up_Btn.gameObject.SetActive(false);


        if (m_Skill.m_Lv == 1)  //최소치
            m_Down_Btn.gameObject.SetActive(false);
        else
            m_Down_Btn.gameObject.SetActive(true);

        if (m_Skill.m_Lv == 10)  //최대치
            m_Up_Btn.gameObject.SetActive(false);

    }


    void SkillUpBtn()
    {
        //스킬 레벨 업
        m_Skill.m_Lv++;

        if (m_SkillLv_Txt != null)
            m_SkillLv_Txt.text = "Lv " + m_Skill.m_Lv;

        //스킬포인트 사용
        SkillMgr.Inst.m_SkillPoint -= m_Skill.m_NeedSP;

        m_Skill.m_NeedSP += 5;
        if (m_NeedSP_Txt != null)
            m_NeedSP_Txt.text = m_Skill.m_NeedSP.ToString();


        if (m_Skill.m_Lv == 10 || SkillMgr.Inst.m_SkillPoint < m_Skill.m_NeedSP)  //최대치 , 스킬 포인트 부족시
            m_Up_Btn.gameObject.SetActive(false);
        else
        {
            m_Up_Btn.gameObject.SetActive(true);
            m_Down_Btn.gameObject.SetActive(true);
        }

        //전체 스킬Root 리프레쉬
        SkillMgr.Inst.RefershSkillRoot();
    }

    void SkillDownBtn()
    {
        //스킬 레벨 다운
        m_Skill.m_Lv--;
        if (m_SkillLv_Txt != null)
            m_SkillLv_Txt.text = "Lv " + m_Skill.m_Lv;

        m_Skill.m_NeedSP -= 5;
        if (m_NeedSP_Txt != null)
            m_NeedSP_Txt.text = m_Skill.m_NeedSP.ToString();

        //스킬포인트 돌려주기
        SkillMgr.Inst.m_SkillPoint += m_Skill.m_NeedSP;

        if (m_Skill.m_Lv == 1)  //최소치
            m_Down_Btn.gameObject.SetActive(false);
        else
        {
            m_Up_Btn.gameObject.SetActive(true);
            m_Down_Btn.gameObject.SetActive(true);
        }

        //전체 스킬Root 리프레쉬
        SkillMgr.Inst.RefershSkillRoot();
    }

}
