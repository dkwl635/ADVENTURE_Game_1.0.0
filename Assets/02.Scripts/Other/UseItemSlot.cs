using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UseItemSlot : Slot
{
    public KeyCode m_KeyCode;
    Player player;


    public override void Awake()
    {
        base.Awake();
        player = GameObject.FindObjectOfType<Player>();
        m_ItemData = null;     
    }



    private void Update()
    {
        Refresh();
    }

    public override void SetSlot( ItemData a_ItemData )
    {    
        if(a_ItemData == null)   // 빈칸으로 초기화 되는 경우
        {       
            m_ItemData = null;
            m_SlotImg.sprite = null;
            m_SlotImg.gameObject.SetActive( false );
            m_ItemCountTxt.gameObject.SetActive( false );
            bEmpty = true;
            return;
        }


        UseItem useItem = a_ItemData as UseItem;
        if (useItem == null)
            return;

        //기존에 등록된 스킬이 다른 슬롯에 있다면 그 기존 슬롯초기화
        if (InventoryUIMgr.Inst.DicUseItemSlots.ContainsKey(useItem))
        {
            if (InventoryUIMgr.Inst.DicUseItemSlots[useItem].m_ItemData == useItem)//다른 칸에 같은아이템이 있을경우       
                InventoryUIMgr.Inst.DicUseItemSlots[useItem].SetSlot(null);// 같은 아이템이 있는 슬롯 빈칸으로        
        }


        useItem.m_UesSlotNum = m_SlotNum;
        m_ItemData = useItem;
        m_SlotImg.sprite = useItem.m_ItemSprite;
        m_ItemData.m_CurCount = useItem.m_CurCount;

        m_SlotImg.gameObject.SetActive( true );

        if(m_ItemData.m_CurCount > 1)
        {
            m_ItemCountTxt.text = m_ItemData.m_CurCount.ToString();
            m_ItemCountTxt.gameObject.SetActive( true );
        }
        else
            m_ItemCountTxt.gameObject.SetActive( false );

        //새로 등록
        InventoryUIMgr.Inst.DicUseItemSlots[useItem] = this;


        bEmpty = false;
    }

    public void UseSlotItem()
    {
        if(m_ItemData != null)
            ItemMgr.Inst.UseItem( m_ItemData, player );

    }

    public void Refresh()
    {
        if (!bEmpty)
        {       
            InventoryUIMgr.Inst.m_ItemSlots[m_ItemData.m_SlotNum].RefreshSlot();

            if (m_ItemData.m_CurCount > 1)
            {
                m_ItemCountTxt.text = m_ItemData.m_CurCount.ToString();
                m_ItemCountTxt.gameObject.SetActive(true);
            }
            else
                m_ItemCountTxt.gameObject.SetActive(false);


            if (m_ItemData.m_CurCount == 0)
            {
                player.m_PlayerInventory.DestroyItem(m_ItemData.m_SlotNum);
                this.SetSlot(null);
            }
        }        
    }
}
