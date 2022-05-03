using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SkillTpye
{
    None,
    NormalAttack,
    Crash,
    SwordAuror,
}

public class SkillMgr : MonoBehaviour
{
    //싱글턴
    public static SkillMgr Inst = null;
    //플레이어
    Player player = null;
    public int m_SkillPoint
    {
        get {return player.m_SkillPoint; }
        set { player.m_SkillPoint = value; }
    }

    //스킬오브젝트를 가지고 있는 게임오브젝트
    public GameObject SkillsObj = null;

    //전체 스킬 리스트
    [HideInInspector] public Skill[] m_SkillList;  
    //이름으로 찾을 스킬 딕셔너리
    public Dictionary<string, Skill> m_Skills;
    [HideInInspector] public bool bIsPushSkill = false;


    //마우스 위치 
    public Vector3 m_MousePos;  //마우스가 땅에 있을 경우의 위치값
    public Vector3 m_MouseDirVec;   //캐릭터에서 마우스의 까지의 방향값
    Ray m_MouseRay;
    RaycastHit hitInfo;
    LayerMask m_LayerMask = -1;

    //스킬 적용 슬롯 위치들
    public GameObject m_SlotSkillRoot;
    public SkillSlot[] m_SkillSlots;

    [Header("SkillRoot")]
    public GameObject m_SkillRootPrefab;
    public Transform m_Content;
    //스킬이 어떤 슬롯에 있는지 찾기위한 딕셔너리
    public Dictionary<Skill, SkillSlot> DicSkillSlots = new Dictionary<Skill, SkillSlot>();

    //스킬 포인트 사용시 다른 스킬레벨업 관리 창 새로고침 용
    public delegate void SkillPointRefersh();
    public SkillPointRefersh RefershSkillRoot;

    [Header("SkillUI")]
    public GameObject m_SkillUIPanel;
    public Text m_SkillPoint_Txt = null;
    [HideInInspector]public bool bOpne = false;

    private void Awake()
    {
        if (Inst == null)
            Inst = this;

        if (player == null)
            player = GameObject.Find("Player").GetComponent<Player>();


        m_SkillList = SkillsObj.GetComponentsInChildren<Skill>();
        m_Skills = new Dictionary<string, Skill>();

        m_LayerMask = 1 << LayerMask.NameToLayer("GROUND");
    }

    void Start()
    {
        m_SkillPoint = 10;
        m_SkillPoint_Txt.text = "SP : " + m_SkillPoint.ToString();

        //스킬 이름별로 딕셔너리로 저장
        for (int i = 0; i < m_SkillList.Length; i++)
        {
            m_Skills[m_SkillList[i].m_SkillName] = m_SkillList[i];
            m_SkillList[i].Init(player);
        }

        //UI 스킬슬롯 초기화 시키기
        if (m_SlotSkillRoot != null)
            m_SkillSlots = m_SlotSkillRoot.GetComponentsInChildren<SkillSlot>();
        if (m_SkillSlots != null)
            for (int i = 0; i < m_SkillSlots.Length; i++)
                m_SkillSlots[i].SetSlot(null);

        for (int i = 1; i < m_SkillList.Length; i++)
        {
            GameObject skillRoot = Instantiate(m_SkillRootPrefab, m_Content);
            skillRoot.GetComponent<SkillRoot>().m_Skill = m_SkillList[i];
        }

    }

    private void Update()
    {
        Skill_Update();
        KeyDown_Update();
        KeyUp_Update();


        m_SkillPoint_Txt.text = "SP : " + m_SkillPoint.ToString(); 

        //기본 공격 
        //클릭시 발동
        if (!player.bIsHit && player.bIsWeapon)
        {
            if (Input.GetMouseButtonDown(0) && !InGameMgr.IsPointerOverUIObject())
            {
                m_Skills["NormalAttack"].UseSkill();
                bIsPushSkill = false;
            }
        }

        m_Skills["NormalAttack"].CoolTimeUpdate();

        //스킬 캔슬
        if (Input.GetMouseButtonDown(1) && !InGameMgr.IsPointerOverUIObject())
        {
            bIsPushSkill = false;
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            if (!m_SkillUIPanel.activeSelf)
                OnSkillUI();
            else
                OffSkillUI();
        }

    }

  
    public void OnSkillUI()
    {
        m_SkillUIPanel.SetActive(true);
        m_SkillUIPanel.transform.SetAsLastSibling();
    }
    void OffSkillUI()
    {
        m_SkillUIPanel.SetActive(false);
    }
    void KeyDown_Update()
    {
        if (player.bIsHit || !player.bIsWeapon)
            return;

        if (m_SkillSlots.Length > 0)
            for (int i = 0; i < m_SkillSlots.Length; i++)
                if (Input.GetKeyDown(m_SkillSlots[i].m_KeyCode))
                    if (m_SkillSlots[i].m_Skill != null)
                        m_SkillSlots[i].m_Skill.BoolShowMark();                                
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_MousePos, 1);
    }

    void Skill_Update()
    {
        //스킬의 방향을 정하는 
        m_MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(m_MouseRay, out hitInfo, Mathf.Infinity, m_LayerMask))
        {
            m_MousePos = hitInfo.point;
            m_MouseDirVec = m_MousePos - player.transform.position;
            m_MouseDirVec = m_MouseDirVec.normalized;
            m_MouseDirVec.y = 0;

        }

        //슬롯에 장착된 스킬만 쿨타임 및 업데이트 사용
        if (m_SkillSlots.Length > 0)    
            for (int i = 0; i < m_SkillSlots.Length; i++)
            {
                m_SkillSlots[i].SkillSlotUpdate();

                if (m_SkillSlots[i].m_Skill == null)
                    continue;

              
                m_SkillSlots[i].m_Skill.CoolTimeUpdate();

                m_SkillSlots[i].m_Skill.ShowSkillMark(m_MouseDirVec);
            }
        

    }

    void KeyUp_Update()
    {
        if (player.bIsHit || !player.bIsWeapon)
            return;

            if (m_SkillSlots.Length > 0)
            for (int i = 0; i < m_SkillSlots.Length; i++)
                if (Input.GetKeyUp(m_SkillSlots[i].m_KeyCode))
                    if (m_SkillSlots[i].m_Skill != null)
                        m_SkillSlots[i].m_Skill.UseSkill();
    }


}
