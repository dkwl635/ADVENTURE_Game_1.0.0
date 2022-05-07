using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SlotType
{
    None,   //미설정
    Item,   //일반아이템, 사용아이템
    Equipment,  //장비아이템
    Part,   //착용가능한 아이템 슬롯
    Skill,  //스킬을 사용하는 슬롯
    SkillRoot,  //스킬이 모여있는 곳
    UseItem,    //아이템 사용하는 슬롯 (포션....)
    ShopItem,   //상점용 보여주기 슬롯
}

public enum SlotNum //특수 슬롯 번호
{
    Weapon = 101,
    Head = 102,
    ShoulderPad = 103,
    Cloth = 104,
    Belt = 105,
    Glove = 106,
    Shoe = 107,

    SkillQ = 201,
    SkillW = 202,
    SkillE = 203,

    UseItem1 = 301,
    UseItem2= 302,
}

public class Slot : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler ,IBeginDragHandler,   IDragHandler, IEndDragHandler , IPointerClickHandler
{
    public SlotType m_SlotType = SlotType.None;

    [HideInInspector] public  ItemData m_ItemData = null; //슬롯이 가지고 있는 아이템 정보

    public int m_SlotNum = -1; //슬롯 위치 번호   //아이템 인벤토리의 순서와 같음
    public bool bEmpty = true;    //비어있는 슬롯인지
    public Image m_SlotImg;           //이미지
    public Text m_ItemCountTxt;     //수량을 보여주는
                                                      //[HideInInspector] public int m_ItemCount = 0;     // 

    public virtual void Awake()
    {
        m_SlotImg = gameObject.GetComponentInChildren<Image>(true);
        m_ItemCountTxt = gameObject.GetComponentInChildren<Text>(true);
    }


    public void RefreshSlot()
    {  
        if(m_ItemData == null)
        {
            m_SlotImg.sprite = null;
            m_SlotImg.gameObject.SetActive(false);
            m_ItemCountTxt.text = "";
        }
        else
        {
            if (m_ItemData.m_ItemSprite != null)
            {
                m_SlotImg.gameObject.SetActive(true);
                m_SlotImg.sprite = m_ItemData.m_ItemSprite; //이미지 적용
            }
            m_ItemCountTxt.text = m_ItemData.m_CurCount > 1 ? m_ItemData.m_CurCount.ToString() : "";
        }
    }   

    public virtual void SetSlot(ItemData a_ItemData)
    {
        if (a_ItemData == null)
        {
            m_ItemData = null;
            m_SlotImg.color = Color.white;
            m_SlotImg.sprite = null;
            m_SlotImg.gameObject.SetActive(false);
            m_ItemCountTxt.text = "";
            return;
        }
        else
        {          
            m_ItemData = a_ItemData;

            if (m_SlotType != SlotType.None)
                m_ItemData.m_SlotNum = m_SlotNum;

            if (m_ItemData.m_ItemSprite != null)
            {
                m_SlotImg.gameObject.SetActive(true);
                m_SlotImg.sprite = a_ItemData.m_ItemSprite; //이미지 적용
            }

            m_ItemCountTxt.text = a_ItemData.m_CurCount > 1 ? a_ItemData.m_CurCount.ToString() : "";
        }
    }


    //포인터가 위에 올라오면
    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseMgr.Inst.OnMouesEnterSlot(this);
    }

    public void OnDisable()
    {     
        MouseMgr.Inst.OnMouesExitSlot(this);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        MouseMgr.Inst.DragStartSlot(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseMgr.Inst.OnMouesExitSlot(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MouseMgr.Inst.DragIngSlot();
    }



    public void OnEndDrag(PointerEventData eventData)
    {       
        MouseMgr.Inst.DragEndSlot();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MouseMgr.Inst.ClickSlot(this);
    }
}


