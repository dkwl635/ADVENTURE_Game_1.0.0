using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;




[System.Serializable]
public class SellItem
{
    public int m_ItemCode; // 파는 아이템 코드
    public int m_Price;    //가격
}

public class ShopMgr : MonoBehaviour
{
    public static ShopMgr Inst;

    //상점 UI
    public GameObject m_ShopPanel;
    public GameObject m_PlayerInventoryUI;
    PlayerInventory m_PlayerInventory;
    Player m_Player;

    //상점에 파는 아이템 리스트
    List<SellItem> m_SellItemList;

    public Text m_SellCoin_Txt; // 팔때 코인
    [HideInInspector] public int m_SellCoin; // 팔때 코인


    public delegate void BtnAction();
    public BtnAction ShopClearBtnClick;
    public BtnAction UserSellBtnClick;
    public BtnAction UserCancelBtnClick;

    [Header("Transform")]
    public Transform m_BuyItemRootTr;
    public Transform m_SellItemRootTr;


    [Header("Prefab")]
    public GameObject m_UserSellItemRootPrefab;
    public GameObject m_ShopSellItemRootPrefab;

    List<GameObject> m_ShopSellRoots = new List<GameObject>();
    //판매 관련 내 인벤토리 슬롯 및 판매대 박스
    [HideInInspector] public List<ItemData> m_SellUserItemList = new List<ItemData>();
    [HideInInspector] public Dictionary<ItemData, SellItemRoot> m_UserSellRoots = new Dictionary<ItemData, SellItemRoot>(); 
    [HideInInspector] public Dictionary<ItemData, Slot> m_UserSellSlot = new Dictionary<ItemData, Slot>();

    [Header("Button")]
    //public Button m_AllBuyBtn;
    public Button m_ClearBtn;
    public Button m_AllSelBtn;
    public Button m_UserAllCancelBtn;
    public Button m_BackBtn;

    [Header("LOGBOX")]
    public GameObject m_LogBoxPanel;
    public GameObject m_LogBox;
    public Text m_LogBoxTxt;
    public Button m_LogOkBtn;
    RectTransform m_LogBoxRextTr;
    Vector2 m_LogBoxSize = Vector2.zero;

    Color m_AlphaColor = new Color(1, 1, 1, 0.5f);
    
    private void Awake()
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


        if (m_LogBox != null)
            m_LogBoxRextTr = m_LogBox.GetComponent<RectTransform>();

       
    }
    private void Start()
    {
        if (m_LogOkBtn != null)
            m_LogOkBtn.onClick.AddListener(LogBoxOkBtn);

        if (m_ClearBtn != null)
            m_ClearBtn.onClick.AddListener(ShopClear);

        if (m_AllSelBtn != null)
            m_AllSelBtn.onClick.AddListener(UserAllSel);

        if (m_UserAllCancelBtn != null)
            m_UserAllCancelBtn.onClick.AddListener(UserAllCancel);

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(OffShop);




    }
    private void Update()
    {
        m_SellCoin_Txt.text = m_SellCoin == 0 ? "" : m_SellCoin.ToString();


        
        m_LogBoxSize = m_LogBoxRextTr.sizeDelta;
        m_LogBoxSize.y = 120 + m_LogBoxTxt.rectTransform.sizeDelta.y;
        m_LogBoxRextTr.sizeDelta = m_LogBoxSize;
    }

    //상점 리스트 받아오기 , 상점이용자 인벤토리 연결하기
    public void SetSellItemList(List<SellItem> SellItems , Player a_Player)
    {
        m_SellItemList = SellItems;
        m_PlayerInventory = a_Player.m_PlayerInventory;
        m_Player = a_Player;
    }

    //상점 UI 오픈
    public void OpenShop()
    {
        m_ShopPanel.SetActive(true);
        m_PlayerInventoryUI.SetActive(true);
        m_PlayerInventoryUI.transform.SetAsLastSibling();
        m_ShopPanel.transform.SetAsLastSibling();

        //플레이어 못 움직이게
        if (m_Player != null)
            m_Player.bIsMove = false;

        //상점에서 파는 아이템 목록 갱신
        if (m_SellItemList.Count != m_ShopSellRoots.Count)
            for (int i = 0; i < m_SellItemList.Count; i++)
            {
                GameObject sellRoot = Instantiate(m_ShopSellItemRootPrefab, m_BuyItemRootTr);
                ShopItemRoot shopItemRoot = sellRoot.gameObject.GetComponent<ShopItemRoot>();
                ItemData itemData = ItemMgr.Inst.InstantiateItem(m_SellItemList[i].m_ItemCode);

                shopItemRoot.SetSellItem(itemData, m_SellItemList[i].m_Price, m_PlayerInventory);
                ShopClearBtnClick += shopItemRoot.Clear;
                m_ShopSellRoots.Add(sellRoot);
            }
    }
    
    //유저가 판매 하는 아이템 박스 생성 함수
    public void SetSellItemBox(Slot a_Slot)
    {
        if (!(a_Slot.m_SlotType == SlotType.Equipment || a_Slot.m_SlotType == SlotType.Item))
            return;

        a_Slot.m_SlotImg.color = m_AlphaColor;
        ItemData ItemData = a_Slot.m_ItemData;
        GameObject userSellRoot = Instantiate(m_UserSellItemRootPrefab, m_SellItemRootTr);
        SellItemRoot sellItemRoot = userSellRoot.gameObject.GetComponent<SellItemRoot>();
        sellItemRoot.SetSellItem(ItemData, m_PlayerInventory);
       
        //기존 슬롯과 판매 아이템박스를 연결 하기 위해서
        m_UserSellRoots[ItemData] = sellItemRoot;
        m_UserSellSlot[ItemData] = a_Slot;
        m_SellUserItemList.Add(ItemData);
       
        
        //동시 호출을 위한 연결
        UserSellBtnClick += sellItemRoot.SellItem;  //판매 하는 함수
        UserCancelBtnClick += sellItemRoot.Cancel;  //취소하는 함수
        
        //총 판매 가격 갱신
        SetAllSellItemCoin();
    }
    //유저가 판매하는 아이템 박스 삭제 함수
    public void OffSellItemBox(ItemData a_ItemData)
    {
        
        Debug.Log(m_UserSellRoots.Count);
        //기존에 연결된 판매아이템 박스를 삭제
        if (m_UserSellRoots.ContainsKey(a_ItemData))
        {
            UserSellBtnClick -= m_UserSellRoots[a_ItemData].SellItem;
            UserCancelBtnClick -= m_UserSellRoots[a_ItemData].Cancel;

            Destroy(m_UserSellRoots[a_ItemData].gameObject);
            m_UserSellRoots.Remove(a_ItemData);
        }
        Debug.Log("삭제후 " + m_UserSellRoots.Count);

        //기존에 연결된 슬롯의 잠금장치 풀기
        if (m_UserSellSlot.ContainsKey(a_ItemData))
        { 
            m_UserSellSlot[a_ItemData].m_SlotImg.color = Color.white;
            m_UserSellSlot.Remove(a_ItemData);
        }

        m_SellUserItemList.Remove(a_ItemData);

        //총판매 가격 갱신
        SetAllSellItemCoin();
    }
    //상점 UI 끄기
    public void OffShop()
    { 
        for (int i = 0; i < m_SellUserItemList.Count;)
        {         
            OffSellItemBox(m_SellUserItemList[i]);
        }       

        for (int i = 0; i < m_ShopSellRoots.Count; i++)
        {
            Destroy(m_ShopSellRoots[i]);
        }

        //딕셔너리, 리스트 초기화
        m_ShopSellRoots.Clear();
        m_UserSellRoots.Clear();
        m_UserSellSlot.Clear();

   
        //UI 끄기
        m_ShopPanel.SetActive(false);
        InventoryUIMgr.Inst.m_InventoryUI_Panel.SetActive(false);

        //플레이어 움직이게
        if (m_Player != null)
            m_Player.bIsMove = true;

        m_Player = null;
       
    }

    //유저 아이템 총 판매 금액 갱신
    public void SetAllSellItemCoin()
    {     
        m_SellCoin = 0;
       
        if (m_UserSellRoots.Count > 0)
            foreach (SellItemRoot root in m_UserSellRoots.Values)
            {
                m_SellCoin += root.m_Price;
            }
   
        m_SellCoin_Txt.text = m_SellCoin.ToString();
     
    }

    //상점 초기화 버튼
    public void ShopClear()
    {
        //등록된 슬롯들 수량 초기화 해주기
        ShopClearBtnClick?.Invoke(); 
    }

    //유저의 등록된 아이템 모두 팔기
    public void UserAllSel()
    {
        //등록된 유저 판매 박스 함수 호출
        UserSellBtnClick?.Invoke();      
    }

    //유저의 등록된 아이템 모두 취소하기
    void UserAllCancel ()
    {
        //등록된 유저 판매 박스 함수 호출
        UserCancelBtnClick?.Invoke();   
    }
    //로그박스 띄우기
    public void OnLogBox(string msg)
    {
        m_LogBoxPanel.SetActive(true);
        m_LogBoxTxt.text = msg;        
    }
    //로그박스 확인버튼 함수
    void LogBoxOkBtn()
    {
        m_LogBoxTxt.text = "";
        m_LogBoxPanel.SetActive(false);
    }
}
