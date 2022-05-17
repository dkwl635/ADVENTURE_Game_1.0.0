using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemRoot : MonoBehaviour
{
    public PlayerInventory m_Buyer;
    public ItemData m_SellItem; //팔고 있는 아이템
    public Slot m_ItemSlot; //팔고 있는 아이템이 들어 있는 슬롯
    public Text m_ItemName_Txt; // 아이템 이름 txt
    public Text m_Price_Txt; // 가격txt
    public Button m_Buy_Btn;    //구매 버튼
    public InputField m_InputField; //구매 수량
    public GameObject m_Lable;

    int m_Count = 0;    //현재 판매갯수
    int m_MaxCount = 99;
    int m_Price; //개당 판매 가격

    // Start is called before the first frame update
    void Start()
    {      
        //첫 셋팅
        m_InputField.text = "1";
        m_Count = 1;
      

        if (m_InputField != null)
            m_InputField.onValueChanged.AddListener(InputFieldOnChange);

        if(m_Buy_Btn != null)
            m_Buy_Btn.onClick.AddListener(BuyItem);

    }


    public void SetSellItem(ItemData a_ItemData  , int a_Price , PlayerInventory a_Buyer)
    {
        m_Buyer = a_Buyer;
        m_SellItem = a_ItemData;
        m_ItemSlot.SetSlot( a_ItemData );
        m_Price = a_Price;
        m_Price_Txt.text = a_Price.ToString();
        
        m_ItemName_Txt.text = a_ItemData.m_Name;

        //수량이 있는 아이템 구분
      
        if(m_SellItem.m_MaxCount != 1)
        {        
            m_InputField.gameObject.SetActive(true); 
            m_Lable.SetActive(true);
        }
        else
        { 
            m_InputField.gameObject.SetActive(false);
            m_Lable.SetActive(false);
        }


    }

    void InputFieldOnChange(string a_Count)
    {
        if(string.IsNullOrEmpty(a_Count) || a_Count =="")
        {
            m_Price_Txt.text = "0";
            return;
        }

        m_Count = int.Parse(a_Count);   
        m_Price_Txt.text = (m_SellItem.m_Price * int.Parse(a_Count)).ToString();
    }
    


    void BuyItem()
    {
        if (m_Count <= 0)
            return;

        if (m_Buyer == null)
            return;

        if (m_Buyer.m_Coin < m_Price * m_Count)
        {
            ShopMgr.Inst.OnLogBox("코인이\n부족합니다");
            return;
        }    
      

        ItemData newItem = ItemMgr.Inst.InstantiateItem(m_SellItem.m_ItemCode, m_Count);
        if (m_Buyer.AddNewItem(newItem) == false)
        {
            ShopMgr.Inst.OnLogBox("인벤토리 창고가\n부족합니다.");          
        }

        SoundMgr.Inst.PlaySound("LostCoin");
        m_Buyer.m_Coin -= m_Price * m_Count;
    }


    public void Clear()
    {
        m_InputField.text = "1";
    }

}
