using UnityEngine.UI;

public class EquipmentSlot : Slot
{
    public PartType m_SlotItemType;//슬롯이 가질수 있는 아이템타입   

    public override void Awake()

    {
        m_SlotImg = gameObject.GetComponentInChildren<Image>(true);
        m_ItemCountTxt = gameObject.GetComponentsInChildren<Text>(true)[1];
    }

}

  

