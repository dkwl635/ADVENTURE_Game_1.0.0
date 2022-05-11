using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseMgr : MonoBehaviour
{
    public static MouseMgr Inst = null;

    //슬롯 드래그를 위해
    GraphicRaycaster gr;
    PointerEventData m_Point;
    List<RaycastResult> m_RayResult = new List<RaycastResult>();

    public GameObject m_ItemSlotUI; //마우스 위에 보이는 아이템 정보
    public Vector2 m_TempSize = Vector2.zero;
    RectTransform m_ItemSlotRectTr;
    RectTransform m_ItemTxtTr;
    Image m_OnImg;
    Text m_OnCount;
    Text m_OnName;
    Text m_OnStar;
    Text m_OnInfo;

    public GameObject m_DragSlot; //드래그 슬롯 오브젝트
    Slot m_BeginSlot; //현재 들고 있는 슬롯 정보
    Slot m_OnSlot; //마우스가 위에 
    Slot m_EndSlot; //마우스를 놓은곳에 슬롯
    Image m_DragImg;
    Text m_DragCount;

    float m_Screen_Height;
    float m_Screen_Width;

    bool bIsDrag = false;

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

        gr = GameObject.Find("UICanvas").GetComponent<GraphicRaycaster>();
        m_Point = new PointerEventData(null);


        if (m_ItemSlotUI != null)
        {
            m_ItemSlotRectTr = m_ItemSlotUI.GetComponent<RectTransform>();
            m_OnImg = m_ItemSlotUI.transform.Find("Item_Img").GetComponent<Image>();
            m_OnCount = m_ItemSlotUI.transform.Find("Count_Text").GetComponent<Text>();
            m_OnName = m_ItemSlotUI.transform.Find("ItemNameTxt").GetComponent<Text>();
            m_OnStar = m_ItemSlotUI.transform.Find("ItemStarTxt").GetComponent<Text>();
            m_OnInfo = m_ItemSlotUI.transform.Find("ItemInfo").GetComponent<Text>();
            if (m_OnInfo != null)
            {
                m_ItemTxtTr = m_OnInfo.GetComponent<RectTransform>();
            }
        }

        if (m_DragSlot != null)
        {
            m_DragImg = m_DragSlot.transform.Find("Item_Img").GetComponent<Image>();
            m_DragCount = m_DragSlot.transform.Find("Count_Text").GetComponent<Text>();
        }


    }

    private void Start()
    {
        m_Screen_Height = Screen.height;
        m_Screen_Width = Screen.width;
    }

    private void LateUpdate()
    {
        SetSizeRectTr();
    }

    public void OnMouesEnterSlot(Slot slot) //슬롯 위에 마우스 가 있으면
    {
        if (bIsDrag == false)
        {
            m_OnSlot = slot;
            if (m_OnSlot == null || m_OnSlot.m_SlotImg.sprite == null)
            {
                m_ItemSlotUI.gameObject.SetActive(false);
                return;
            }

            if (m_OnSlot.m_SlotType == SlotType.Item || 
               m_OnSlot.m_SlotType == SlotType.UseItem || m_OnSlot.m_SlotType == SlotType.ShopItem)

            {
                if (m_OnSlot.m_ItemData == null)
                    return;

                m_ItemSlotUI.gameObject.SetActive(true);
                m_OnImg.sprite = m_OnSlot.m_SlotImg.sprite;
                m_OnCount.text = m_OnSlot.m_ItemCountTxt.text;                           
                m_OnName.text = m_OnSlot.m_ItemData.m_Name;

                m_OnStar.text = m_OnSlot.m_ItemData.m_Grade;
                m_OnInfo.text = m_OnSlot.m_ItemData.m_ItemInfo;
                m_ItemSlotUI.transform.SetAsLastSibling();
                
            }
            else if(m_OnSlot.m_SlotType == SlotType.Equipment|| m_OnSlot.m_SlotType == SlotType.Part)
            {
                if (m_OnSlot.m_ItemData == null)
                    return;

                EquipmentItemData equip = m_OnSlot.m_ItemData as EquipmentItemData;
                if (equip.Equals(null))
                    return;


                m_ItemSlotUI.gameObject.SetActive(true);
                m_OnImg.sprite = m_OnSlot.m_SlotImg.sprite;
                m_OnCount.text = m_OnSlot.m_ItemCountTxt.text;

                if(equip.m_Star > 0)
                m_OnName.text =  "+" +equip.m_Star.ToString() + " " +   m_OnSlot.m_ItemData.m_Name;
                else
                    m_OnName.text = m_OnSlot.m_ItemData.m_Name;

                m_OnStar.text = m_OnSlot.m_ItemData.m_Grade;
                m_OnInfo.text = m_OnSlot.m_ItemData.m_ItemInfo;
                m_ItemSlotUI.transform.SetAsLastSibling();
               
            }
            else if (m_OnSlot.m_SlotType == SlotType.Skill || m_OnSlot.m_SlotType == SlotType.SkillRoot)
            {
                SkillSlot skillSlot = m_OnSlot as SkillSlot;
                if (skillSlot == null)
                    return;

                if (skillSlot.m_Skill == null)
                    return;

                m_ItemSlotUI.gameObject.SetActive(true);
                m_OnImg.sprite = skillSlot.m_SlotImg.sprite;
                m_OnCount.text = "";
                m_OnName.text = skillSlot.m_Skill.m_SkillName;
                m_OnStar.text = skillSlot.m_Skill.m_Lv + " Lv";
                m_OnInfo.text = skillSlot.m_Skill.m_SkillInfo;
                m_ItemSlotUI.transform.SetAsLastSibling();
               
            }
            else
                m_ItemSlotUI.gameObject.SetActive(false);
        }
       
    }

    public void OnMouesExitSlot(Slot slot)//슬롯 위에 마우스 가 나가면
    {
        if(m_OnSlot == slot)
        {
            m_ItemSlotUI.gameObject.SetActive(false);
            m_OnSlot = null;
        }
    }

    //Drag  & Drop
    public void DragStartSlot(Slot slot)
    {
        m_BeginSlot = slot;

        if (m_BeginSlot == null || m_BeginSlot.m_SlotImg.sprite == null)
            return;

        if (m_BeginSlot.m_SlotType == SlotType.None || m_BeginSlot.m_SlotType == SlotType.ShopItem)
            return;

        if (m_BeginSlot.m_SlotImg.color.a == 0.5f)
            return;

        if (m_BeginSlot.m_SlotType == SlotType.Skill || m_BeginSlot.m_SlotType == SlotType.SkillRoot)//스킬 관련 슬롯일 경우
        {
            SkillSlot beginSkillSlot = m_BeginSlot as SkillSlot;
            if (beginSkillSlot != null && beginSkillSlot.m_Skill.m_CurrTime > 0)//쿨타임 중일떄 
                return;
        }

        m_DragSlot.SetActive(true);
        m_DragSlot.transform.SetAsLastSibling();
        m_DragImg.sprite = m_BeginSlot.m_SlotImg.sprite;
        m_DragCount.text = m_BeginSlot.m_ItemCountTxt.text;
        bIsDrag = true;
    }
    public void DragIngSlot()
    {
        if (m_DragSlot.activeSelf)
        {
            m_DragSlot.transform.position = Input.mousePosition;        
        }
    }
    public void DragEndSlot()
    {
        if (!m_DragSlot.activeSelf)
            return;

        m_EndSlot = RaycastSlot();
        if (m_EndSlot != null)
        {
            ChangSlot(m_BeginSlot, m_EndSlot);
        }
        else if (m_BeginSlot.m_SlotType == SlotType.Skill)
        {
            SkillSlot beginSkillSlot = m_BeginSlot as SkillSlot;
            if (beginSkillSlot != null && beginSkillSlot.m_Skill != null)
                beginSkillSlot.SetSlot(null);//빈칸으로 만들기
        }
        else if (m_BeginSlot.m_SlotType == SlotType.UseItem)
        {
            m_BeginSlot.SetSlot(null);//빈칸으로 만들기
        }

        m_BeginSlot = null;
        bIsDrag = false;
        m_DragSlot.SetActive(false);
    }
    //Drag  & Drop
    Slot RaycastSlot() //밑에 있는 슬롯 체크
    {
        m_Point.position = Input.mousePosition;
        m_RayResult.Clear();
        gr.Raycast(m_Point, m_RayResult);

        if (m_RayResult.Count == 0)
            return null;

        Slot slot = m_RayResult[0].gameObject.GetComponentInParent<Slot>();
        if (slot != null)
        {
            return slot;
        }
        return null;
    }
    void SetSizeRectTr()
    {
        if(m_ItemSlotUI.activeSelf)
        {
            //패널 크기 조정
            m_TempSize = m_ItemSlotRectTr.sizeDelta;
            m_TempSize.y = 180 + m_ItemTxtTr.sizeDelta.y;
            m_ItemSlotRectTr.sizeDelta = m_TempSize;

            m_TempSize = (Vector2)Input.mousePosition;
            if (m_TempSize.y - m_ItemSlotRectTr.rect.height < 0)
            {
                m_TempSize.y = m_ItemSlotRectTr.rect.height;
            }
            else if (m_TempSize.y > m_Screen_Height)
            {
                m_TempSize.y = m_Screen_Height;
            }

            if (m_TempSize.x + m_ItemSlotRectTr.rect.width > m_Screen_Width)
            {
                m_TempSize.x = m_TempSize.x - m_ItemSlotRectTr.rect.width;
            }

            m_ItemSlotRectTr.position = m_TempSize;
        }
    
    }
    void ChangSlot(Slot a_BeginSlot, Slot a_EndSlot)//슬롯의 타입별 적용 방법

    {
        if (a_EndSlot == a_BeginSlot)
            return;

        //장비 -> 장비
        if (a_BeginSlot.m_SlotType == SlotType.Equipment && a_EndSlot.m_SlotType == SlotType.Equipment)
        {
            InventoryUIMgr.Inst.PlayerInventory.ChangeEquipItem(a_BeginSlot.m_SlotNum, a_EndSlot.m_SlotNum);
        }
        //아이템 -> 아이템
        else if (a_BeginSlot.m_SlotType == SlotType.Item && a_EndSlot.m_SlotType == SlotType.Item)
        {
            InventoryUIMgr.Inst.PlayerInventory.ChangeItem(a_BeginSlot.m_SlotNum, a_EndSlot.m_SlotNum);
        }
        //장비 -> 장착장비
        else if (a_BeginSlot.m_SlotType == SlotType.Equipment && a_EndSlot.m_SlotType == SlotType.Part)
        {
            EquipmentItemData equipmentItemData = a_BeginSlot.m_ItemData as EquipmentItemData;
            EquipmentSlot equipment = a_EndSlot as EquipmentSlot;

            if (equipment == null || equipmentItemData == null)
                return;

            if (equipment.m_SlotItemType != equipmentItemData.m_PartType)
                return;         
            InventoryUIMgr.Inst.PlayerInventory.EquipItem(a_BeginSlot.m_SlotNum);
            //a_BeginSlot.SetSlot(null);

        }
        //장착장비 -> 장비
        else if (a_BeginSlot.m_SlotType == SlotType.Part && a_EndSlot.m_SlotType == SlotType.Equipment)
        {
            EquipmentSlot equipment = a_BeginSlot as EquipmentSlot;

            if (equipment == null)
                return;          
            InventoryUIMgr.Inst.PlayerInventory.OffEquipment(equipment.m_SlotItemType, a_EndSlot.m_SlotNum);

        }
        //스킬창 -> 큇스킬
        else if (a_BeginSlot.m_SlotType == SlotType.SkillRoot && a_EndSlot.m_SlotType == SlotType.Skill)
        {
            SkillSlot beginSkillSlot = a_BeginSlot as SkillSlot;
            SkillSlot endSkillSlot = a_EndSlot as SkillSlot;
            if (beginSkillSlot == null || endSkillSlot == null)
                return;

            if (beginSkillSlot.m_CurrTime > 0 || endSkillSlot.m_CurrTime > 0)   //쿨타임 중일떄
                return;

            endSkillSlot.SetSlot(beginSkillSlot.m_Skill);
        }
        //퀵스킬 ->퀵스킬
        else if (a_BeginSlot.m_SlotType == SlotType.Skill && a_EndSlot.m_SlotType == SlotType.Skill)
        {
            SkillSlot beginSkillSlot = a_BeginSlot as SkillSlot;
            SkillSlot endSkillSlot = a_EndSlot as SkillSlot;
            if (beginSkillSlot == null || endSkillSlot == null)
                return;

            if (beginSkillSlot.m_CurrTime > 0 || endSkillSlot.m_CurrTime > 0)   //쿨타임 중일떄
                return;

            if (endSkillSlot.m_Skill == null)//빈칸일떄
            {
                endSkillSlot.SetSlot(beginSkillSlot.m_Skill);
            }
            else//다른 스킬이 등록되어있다면
            {
                Skill endSkill = endSkillSlot.m_Skill;
                endSkillSlot.SetSlot(beginSkillSlot.m_Skill);
                beginSkillSlot.SetSlot(endSkill);
            }

        }
        //아이템 -> 퀵아이템
        else if (a_BeginSlot.m_SlotType == SlotType.Item && a_EndSlot.m_SlotType == SlotType.UseItem)//아이템 창에서 퀵슬롯으로 갈떄
        {
            if (a_BeginSlot.m_ItemData == a_EndSlot.m_ItemData)
                return;

                a_EndSlot.SetSlot(a_BeginSlot.m_ItemData);
        }
        //퀵아이템 -> 퀵아이템
        else if (a_BeginSlot.m_SlotType == SlotType.UseItem && a_EndSlot.m_SlotType == SlotType.UseItem)
        {
            if (a_EndSlot.m_ItemData == null)
                a_EndSlot.SetSlot(a_BeginSlot.m_ItemData);
            else
            {
                ItemData endinItem = a_EndSlot.m_ItemData;
                a_EndSlot.SetSlot(a_BeginSlot.m_ItemData);
                a_BeginSlot.SetSlot(endinItem);
            }
        }
        else
            return;
    }
    //슬롯 클릭 할 때
   public  void ClickSlot(Slot a_BeginSlot)
    {
        if (a_BeginSlot.m_ItemData == null)
            return;

        //상점이 열려 있으면
        if(ShopMgr.Inst != null && ShopMgr.Inst.m_ShopPanel.activeSelf)
        {          
            if (a_BeginSlot.m_SlotImg.color.a == 1.0f)
            {               
                ShopMgr.Inst.SetSellItemBox(a_BeginSlot);
            }
            else
            {                 
                ShopMgr.Inst.OffSellItemBox(a_BeginSlot.m_ItemData);
            }
        }
        else if(UpgradeMgr.Inst != null && UpgradeMgr.Inst.m_UpgradeUI_Panel.activeSelf)
        {
            if (a_BeginSlot.m_SlotImg.color.a == 1.0f)
            {
                UpgradeMgr.Inst.SetUpgradeItem(a_BeginSlot);
            }
            else
            {
                UpgradeMgr.Inst.OffUpgradeItem(a_BeginSlot);
            }
        }
    }

  

}
