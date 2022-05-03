using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIMgr : MonoBehaviour
{
    public static InventoryUIMgr Inst;
    public Player Player;
    public PlayerInventory PlayerInventory;
    

    public GameObject m_InventoryUI_Panel = null;
    public GameObject m_EquipmentUI;
    public GameObject m_ItemUI;
    public GameObject m_UserInfoUI;
    public List<Slot> m_EquipmentSlots = new List<Slot>();  //장비아이템 슬롯
    public List<Slot> m_ItemSlots = new List<Slot>();           //일반아이템 슬롯

    public GameObject m_EquiqPartSlot; //장착슬롯이 모여있는 곳
    public Dictionary<PartType, EquipmentSlot> m_PartSlot = new Dictionary<PartType, EquipmentSlot>();    //장착장비 슬롯
    public GameObject m_UseItemSlot;    //아이템 퀵슬롯이 모여있는곳
    public Dictionary<ItemData, UseItemSlot> DicUseItemSlots = new Dictionary<ItemData, UseItemSlot>(); //사용아이템 퀵슬롯



    [Header("Button")]
    public Button m_EquipBtn = null;        //장비아이템인벤토리를 여는 버튼
    Text m_EquipLable = null;
    public Button m_ItemBtn = null;         //일반아이템 인벤토리를 여는 버튼
    Text m_ItemLable = null;
    public Button m_UserInfoBtn = null;
    Text m_UserInfoLable = null;

    public Text m_CoinText = null;             //가지고 있는 코인 를 나타낼 텍스트
    public Text m_UserInfoText = null;


    private void Awake()
    {
        Inst = this;
        Player = GameObject.FindObjectOfType<Player>();
        PlayerInventory = GameObject.FindObjectOfType<PlayerInventory>();

        //슬롯 리스트 적용
        Slot[] slots = m_InventoryUI_Panel.GetComponentsInChildren<Slot>();
        if(slots.Length >0)
        {
            for (int i = 0; i < slots.Length; i++)
            {            
                if (slots[i].m_SlotType == SlotType.Equipment)
                    m_EquipmentSlots.Add(slots[i]);
                else if (slots[i].m_SlotType == SlotType.Item)
                    m_ItemSlots.Add(slots[i]);
            }
        }

        //슬롯 초기화
        for (int i = 0; i < m_EquipmentSlots.Count; i++)
        {         
            m_EquipmentSlots[i].m_SlotNum = i;
            m_EquipmentSlots[i].SetSlot(null);
        }

        for (int i = 0; i < m_ItemSlots.Count; i++)
        {
            m_ItemSlots[i].m_SlotNum = i;
            m_ItemSlots[i].SetSlot(null);
        }

       
        EquipmentSlot[] equipmentSlots = m_EquiqPartSlot.GetComponentsInChildren<EquipmentSlot>();
        {
            for (int i = 0; i < equipmentSlots.Length; i++)
            {
                m_PartSlot.Add(equipmentSlots[i].m_SlotItemType, equipmentSlots[i]);                
            }
        }  

        m_EquipLable = m_EquipBtn.GetComponentInChildren<Text>();
        m_ItemLable = m_ItemBtn.GetComponentInChildren<Text>();
        m_UserInfoLable = m_UserInfoBtn.GetComponentInChildren<Text>();

    }

    private void Start()
    {      
        m_EquipBtn.onClick.AddListener(() => { OpenBox(m_EquipmentUI); });
        m_ItemBtn.onClick.AddListener(() => { OpenBox(m_ItemUI); });
        m_UserInfoBtn.onClick.AddListener(() => { OpenBox(m_UserInfoUI); });

        //아이템 퀵슬롯 초기화
        UseItemSlot[] useItemSlots = m_UseItemSlot.GetComponentsInChildren<UseItemSlot>();
        {
            for(int i = 0; i < useItemSlots.Length; i++)
            {
                useItemSlots[i].SetSlot( null );
            }
        }

        SetUserInfo();
        OpenBox(m_EquipmentUI);
    }

    private void Update()
    {    
        //코인업데이트
        m_CoinText.text = PlayerInventory.m_Coin.ToString();

        //OnOff UI
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (m_InventoryUI_Panel.activeSelf)
                OffInvenUI();
            else
                OnInvenUI();
          
        }
    }


    

    public void OnInvenUI()
    {
        if (!m_InventoryUI_Panel.activeSelf)
        {
            OpenBox(m_EquipmentUI);
        }

        m_InventoryUI_Panel.SetActive(true);
        m_InventoryUI_Panel.transform.SetAsLastSibling();
        SetUserInfo();       
    }

    void OffInvenUI()
    {
        m_InventoryUI_Panel.SetActive(false);
    }

    public void SetEquipSlot(int idx, ItemData a_ItemData)
    {
        m_EquipmentSlots[idx].SetSlot(a_ItemData);
    }

    public void SetItemSlot(int idx, ItemData a_ItemData)
    {
        m_ItemSlots[idx].SetSlot(a_ItemData);
    }

    public void SetPartSlot(PartType a_PartType, EquipmentItemData a_EquipmentItemData)
    {
        m_PartSlot[a_PartType].SetSlot(a_EquipmentItemData);
    }

    void SetUserInfo()
    {
        m_UserInfoText.text = "";
        m_UserInfoText.text += "Player Hp : " + Player.m_PlayerStatus.m_MaxHp ;
        m_UserInfoText.text += "\n\n";
        m_UserInfoText.text += "Player AttackPw : " + Player.m_PlayerAttPw + "\n"; 
        m_UserInfoText.text += "Player DefencePw : " + Player.m_PlayerDefPw + "\n";
        m_UserInfoText.text += "\n\n";
        m_UserInfoText.text += "Player SkillPoint : " + Player.m_SkillPoint+"\n";

    }

    void OpenBox(GameObject a_Box)
    {
        if (m_ItemUI == a_Box)
        {
            m_ItemLable.color = Color.yellow;
            m_EquipLable.color = Color.white;
            m_UserInfoLable.color = Color.white;
            m_ItemUI.SetActive(true);
            m_EquipmentUI.SetActive(false);
            m_UserInfoUI.SetActive(false);
        }
        else if (m_EquipmentUI == a_Box)
        {
            m_EquipLable.color = Color.yellow;
            m_ItemLable.color = Color.white;
            m_UserInfoLable.color = Color.white;
            m_EquipmentUI.SetActive(true);
            m_ItemUI.SetActive(false);
            m_UserInfoUI.SetActive(false);
        }
        else if(m_UserInfoUI == a_Box)
        {
            SetUserInfo();
            m_UserInfoLable.color = Color.yellow;
            m_EquipLable.color = Color.white;
            m_ItemLable.color = Color.white;
            m_UserInfoUI.SetActive(true);
            m_EquipmentUI.SetActive(false);
            m_ItemUI.SetActive(false);            
        }
    }


}
