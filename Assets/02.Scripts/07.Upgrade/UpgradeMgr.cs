using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMgr : MonoBehaviour
{
    static public UpgradeMgr Inst;

    Slot m_CurSlot;                             //현재 등록하고 있는 슬롯
    EquipmentItemData m_CurItem;    //현재 업그래이드 할려는 아이템
    Player m_Player;                           //지금 업그레이드 하고 있는 플레이어

    public Image m_CurImg;  //현재 선택중인 아이템 이미지


    public Button m_UpgradeBtn;     //업그래이드 버튼
    public Button m_BackBtn;

    public GameObject m_UpgradeUI_Panel;
    public Text Need_Text; //강화 재료 설명문
    public Text UpgradeInfo_Text; //강화확률 설명문
    public Text BeforeItemInfo_Text; //강화 전 설명문
    public Text AfterItemInfo_Text; //강화 후 설명문
    public Text Button_Text;    //버튼 텍스트
    public Text Wait_Text;  //강화 시간 텍스트


    Color m_AlphaColor = new Color(1, 1, 1, 0.5f);

    //업그레이드 시 필요 코인
    List<int> m_UpgradeUseCoin = new List<int>();
    //업그레이드 등급 별 확률
    List<int> m_UpgradePercentage = new List<int>();
    //등급별 상승치
    List<int> m_AttackRise = new List<int>();
    List<int> m_DefenceRise = new List<int>();

    Animator m_UpgradeEffect;
    bool bIsUpgrade = true;
    void Awake()
    {
        if (Inst != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Inst = this;
            DontDestroyOnLoad(this.gameObject);
            this.gameObject.transform.SetParent(GameObject.FindObjectOfType<DontDestroyOnLoadMgr>().gameObject.transform);
        }
        m_UpgradeEffect = m_UpgradeUI_Panel.GetComponentInChildren<Animator>();
    }
    void Start()
    {
        #region 업그레이드 비용, 확률 강화 상승치 테이블 작성
        m_UpgradeUseCoin.Add(80);
        m_UpgradeUseCoin.Add(100);
        m_UpgradeUseCoin.Add(200);
        m_UpgradeUseCoin.Add(300);
        m_UpgradeUseCoin.Add(400);
        m_UpgradeUseCoin.Add(500);
        m_UpgradeUseCoin.Add(800);
        m_UpgradeUseCoin.Add(1000);
        m_UpgradeUseCoin.Add(1300);

        m_UpgradePercentage.Add(90);
        m_UpgradePercentage.Add(80);
        m_UpgradePercentage.Add(70);
        m_UpgradePercentage.Add(60);
        m_UpgradePercentage.Add(50);
        m_UpgradePercentage.Add(40);
        m_UpgradePercentage.Add(30);
        m_UpgradePercentage.Add(20);
        m_UpgradePercentage.Add(10);

        m_AttackRise.Add(3);
        m_AttackRise.Add(5);
        m_AttackRise.Add(10);
        m_AttackRise.Add(12);
        m_AttackRise.Add(15);
        m_AttackRise.Add(25);
        m_AttackRise.Add(35);
        m_AttackRise.Add(40);
        m_AttackRise.Add(50);

        m_DefenceRise.Add(1);
        m_DefenceRise.Add(2);
        m_DefenceRise.Add(5);
        m_DefenceRise.Add(8);
        m_DefenceRise.Add(10);
        m_DefenceRise.Add(20);
        m_DefenceRise.Add(30);
        m_DefenceRise.Add(40);
        m_DefenceRise.Add(50);
        #endregion

        if (m_UpgradeBtn != null)
            m_UpgradeBtn.onClick.AddListener(UpgradeBtn);

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(OffUpgrade);

        SetText();
    }
    public void OpenUpgrade(Player a_Player)
    {
        m_UpgradeUI_Panel.SetActive(true);     
        InventoryUIMgr.Inst.OnInvenUI();
        m_UpgradeUI_Panel.transform.SetAsLastSibling();

        if (m_Player == null)
            m_Player = a_Player;

        if (m_Player != null)
            m_Player.bIsMove = false;
    }

    public void OnUpgradeUI()
    {
        m_UpgradeUI_Panel.SetActive(true);
        InventoryUIMgr.Inst.OnInvenUI();
        m_UpgradeUI_Panel.transform.SetAsLastSibling();
    }
    
    public void OffUpgrade()
    {
        if (!bIsUpgrade)
            return;

        if (m_CurSlot != null)
        {
            //초기화
            m_CurSlot.m_SlotImg.color = Color.white;
        }

        m_CurSlot = null;
        m_CurItem = null;
        m_CurImg.gameObject.SetActive(false);
        SetText();

        if (m_Player != null)
            m_Player.bIsMove = true;

        m_Player = null;

        InventoryUIMgr.Inst.m_InventoryUI_Panel.SetActive(false);
        m_UpgradeUI_Panel.SetActive(false);
    }
    public void SetUpgradeItem(Slot a_Slot)
    {
      
        if (!bIsUpgrade) //강화 중 이면 
            return;

        //아이템 데이터 - > 장비데이터 로 변환 해서 가져오기(만약 장비 데이터가 아니면 리턴)
        EquipmentItemData newItem = a_Slot.m_ItemData as EquipmentItemData;
        if (newItem == null)  
            return;

        if (m_CurSlot != null)
        {
            //선택되고 있는 슬롯을 원래대로
            m_CurSlot.m_SlotImg.color = Color.white;
            m_CurSlot = null;
            m_CurItem = null;
        }

        //슬롯 정보 저장하기
        m_CurItem = newItem;
        m_CurSlot = a_Slot; 
        a_Slot.m_SlotImg.color = m_AlphaColor;  //반투명으로
        m_CurImg.gameObject.SetActive(true);
        m_CurImg.sprite = m_CurItem.m_ItemSprite;

        SetText();
    }
    public void OffUpgradeItem(Slot a_Slot)
    {
        if (!bIsUpgrade)    //강화중일때
            return;

        if (m_CurSlot != a_Slot)
            return;

        m_CurSlot.m_SlotImg.color = Color.white;
        m_CurItem = null;
       
        m_CurImg.gameObject.SetActive(false);

        SetText();
    }
    void SetText()
    {       
        Button_Text.text = "강화";
        BeforeItemInfo_Text.text = "";
        AfterItemInfo_Text.text = "";
        Need_Text.text = "";
        UpgradeInfo_Text.text = "";

        if (m_CurItem == null)
            return;
     
        BeforeItemInfo_Text.text += "장비 이름 : " + m_CurItem.m_Name;
        BeforeItemInfo_Text.text += "\n장비 등급 : " + m_CurItem.m_Star;

        if(m_CurItem.m_AttPw != 0)
        BeforeItemInfo_Text.text += "\n장비 공격력 : " + m_CurItem.m_AttPw;
        if (m_CurItem.m_DefPw != 0)
            BeforeItemInfo_Text.text += "\n장비 방어력 : " + m_CurItem.m_DefPw;

        //추가 요소
        //BeforeItemInfo_Text.text += "\n장비 추가체력 : " + m_CurItem.m_AddHp;
      
        if(m_CurItem.m_Star == m_CurItem.m_MaxStar)
        {
            UpgradeInfo_Text.text = "";
            AfterItemInfo_Text.text = "최대 강화 입니다";
        }
        else
        {
            Need_Text.text = "필요 코인 : " + m_UpgradeUseCoin[m_CurItem.m_Star];
            UpgradeInfo_Text.text = "강화 확률\n" + m_UpgradePercentage[m_CurItem.m_Star] + "%";

            AfterItemInfo_Text.text += "장비 이름 : " + m_CurItem.m_Name;
            AfterItemInfo_Text.text += "\n장비 등급 : " + (m_CurItem.m_Star + 1);
            if (m_CurItem.m_AttPw != 0)
                AfterItemInfo_Text.text += "\n장비 공격력 : " + (m_CurItem.m_AttPw + m_AttackRise[m_CurItem.m_Star + 1]);
            if (m_CurItem.m_DefPw != 0)
                AfterItemInfo_Text.text += "\n장비 방어력 : " + (m_CurItem.m_DefPw + m_DefenceRise[m_CurItem.m_Star + 1]);
        }
       
    }
    void UpgradeBtn()
    {
        if (m_CurItem == null)
            return;

        if (!bIsUpgrade)
            return;

        if (m_CurItem.m_Star == m_CurItem.m_MaxStar)//최대 강화치 확인
            return;

        if (m_Player.m_PlayerInventory.m_Coin < m_UpgradeUseCoin[m_CurItem.m_Star])//코인 확인
            return;


        m_Player.m_PlayerInventory.m_Coin -= m_UpgradeUseCoin[m_CurItem.m_Star];

        int rand = Random.Range(0, 101);    

        bool upgradeSuccess = false;
        if (rand <= m_UpgradePercentage[m_CurItem.m_Star])
            upgradeSuccess = true;

        bIsUpgrade = false;
        StartCoroutine(Upgrade(upgradeSuccess));

       
    }
    IEnumerator Upgrade(bool upgradeSuccess)
    {
        
        Button_Text.text = "강화중";
        Wait_Text.text = "3";
        yield return new WaitForSeconds(1.0f);
        Wait_Text.text = "2";
        yield return new WaitForSeconds(1.0f);
        Wait_Text.text = "1";
        yield return new WaitForSeconds(1.0f);
        Wait_Text.text = "";

      

        if (upgradeSuccess)
        {
            m_UpgradeEffect.SetTrigger("Success");
            Button_Text.text = "성공";
            if (m_CurItem.m_AttPw != 0)
                m_CurItem.m_AttPw = m_CurItem.m_AttPw + m_AttackRise[m_CurItem.m_Star + 1];
            if (m_CurItem.m_DefPw != 0)
                m_CurItem.m_DefPw = m_CurItem.m_DefPw + m_DefenceRise[m_CurItem.m_Star + 1];

            m_CurItem.m_Star++;
           
        }
        else
        {
            Button_Text.text = "실패";
            m_UpgradeEffect.SetTrigger("Fail");
        }

        yield return new WaitForSeconds(1.0f);
        SetText();
        Button_Text.text = "강화";

        bIsUpgrade = true;
        yield return null;
        
    }

}
