using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellItemRoot : MonoBehaviour
{
    public PlayerInventory m_Seller;
    public ItemData m_SellItem; //팔고 있는 아이템
    public Slot m_ItemSlot;         //팔려는있는 아이템이 들어 있는 슬롯
    public Text m_ItemName_Txt; // 아이템 이름 txt
    public Text m_Price_Txt; // 가격txt

    public Button m_Sell_Btn;    //팔기 버튼
    public Button m_Cancel_Btn;    //취소 버튼
    public InputField m_InputField; //판매 수량
    public GameObject m_Lable;


    int m_Count = 0;    //현재 판매갯수
    int m_MaxCount {
        get {
            if (m_SellItem == null)
                return 0;
            else
                return m_SellItem.m_CurCount;
        } 
    }

    public int m_Price; //전체 판매 가격

    private void Start()
    {
        if (m_Sell_Btn != null)
            m_Sell_Btn.onClick.AddListener(SellItem);

        if (m_Cancel_Btn != null)
            m_Cancel_Btn.onClick.AddListener(Cancel);

        if (m_InputField != null)
            m_InputField.onValueChanged.AddListener(InputFieldOnChange);
    }


    public void SetSellItem(ItemData a_ItemData, PlayerInventory a_Seller)
    {
        m_Seller = a_Seller;
        m_SellItem = a_ItemData;
        m_ItemSlot.SetSlot(a_ItemData);
        m_Count = a_ItemData.m_CurCount;
        m_Price_Txt.text = a_ItemData.m_Price.ToString();
        m_ItemName_Txt.text = a_ItemData.m_Name;

        //수량체크
        if (m_SellItem.m_CurCount != 1)
        {
            m_InputField.text = a_ItemData.m_CurCount.ToString();
            m_InputField.gameObject.SetActive(true);
            m_Lable.SetActive(true);       
        }
        else
        {
            m_InputField.gameObject.SetActive(false);
            m_Lable.SetActive(false);
        }

        m_Price = a_ItemData.m_Price * m_Count;
        m_Price_Txt.text = m_Price.ToString();
    }
    void InputFieldOnChange(string a_Count)
    {
        if (string.IsNullOrEmpty(a_Count) || a_Count == "")
        {
            m_Price_Txt.text = "0";
            return;
        }

        m_Count = int.Parse(a_Count);

        if(m_Count > m_MaxCount)
        {
            m_Count = m_MaxCount;
            m_Price_Txt.text = m_Count.ToString();         
        }
        m_InputField.text = m_Count.ToString();

      
        m_Price = m_SellItem.m_Price * m_Count;
        m_Price_Txt.text = m_Price.ToString();
        ShopMgr.Inst.SetAllSellItemCoin();
    }

    public void SellItem()
    {        
        m_Seller.AddCoin(m_Price);
     
        if (m_SellItem.m_ItemType == ItemType.Equipment)
        {           
            m_Seller.DestoryEquipmentItem(m_SellItem.m_SlotNum);
        }
        else
            m_Seller.DestroyItem(m_SellItem.m_SlotNum, m_Count);

        ShopMgr.Inst.OffSellItemBox(m_SellItem);     
    }

    public void Cancel()
    {
        ShopMgr.Inst.OffSellItemBox(m_SellItem); 
    }
}
