using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    None = 0,
    Item = 1,
    Equipment = 2,
    Portion = 3,

    NormalItem = 99,
    EquipmentItem = 102,
}

public enum PartType
{
    None = 0,
    Weapon = 1,
    Head = 2,
    ShoulderPad = 3,
    Cloth = 4,
    Belt = 5,
    Glove = 6,
    Shoe = 7,
    Last = 8,
}
public enum EquipmentAddType
{
    None = 0,
    HairHalf = 1,
    NoFace = 2,
}

public enum CountItemType
{
    Nomarl = 0,
    Portion = 1,
}

public enum PortionType
{
    None = 0,
    HpPortion = 1,
}


[System.Serializable]
public class ItemData         //아이템 클래스 뿌리
{
    public int m_ItemCode = -1;   //아이템 코드
    public int m_SlotNum = -1;      //만약 아이템를 가지고 있다면 인벤토리 몇번 슬롯에 있는지   
 
    public ItemType m_ItemType = ItemType.None; //아이템 타입
    public string m_Name = "";      //아이템 이름
    public string m_ItemInfo = "";  //아이템 설명
    public int m_Price = 100;             //가격
    public string m_Grade = "노말";
    public Vector3 m_DropMeshPos = Vector3.zero;
    //public float m_DropItemPosY = 0.0f; //땅에 드롭되을때 위치보정 값
    public int m_CurCount = 1;     //현재 가지고 있는 갯수
    public int m_MaxCount = 99;    //최대 가질수 있는 갯수
    public Sprite m_ItemSprite = null;  //아이템 스프라이트
    //바닥에 떨어질때 사용할 메쉬와 메테리얼
    public Mesh m_ItemMesh = null; //아이템 메쉬
    public Material m_ItemMatrl = null; //아이템 메테리얼

    public void ItemClear()
    {
        m_ItemCode = -1;   //아이템 코드
        m_SlotNum = -1;      //만약 아이템를 가지고 있다면 인벤토리 몇번 슬롯에 있는지   

        m_ItemType = ItemType.None; //아이템 타입
        m_Name = "";      //아이템 이름
        m_ItemInfo = "";  //아이템 설명
        m_Price = 100;             //가격
        m_Grade = "";


        m_DropMeshPos = Vector3.zero;
        m_ItemSprite = null;  //아이템 스프라이트
        m_ItemMesh = null; //아이템 메쉬
        m_ItemMatrl = null; //아이템 메테리얼  
    }
   
}


[System.Serializable]
public class EquipmentItemData : ItemData   //장비 아이템 정보
{
    public PartType m_PartType = PartType.None;
    public EquipmentAddType m_AddType = EquipmentAddType.None;       //추가 셋팅이 필요한 타입
    public int m_AttPw = 1;            //공격력
    public int m_DefPw = 1;           //방어력
    public int m_Star = 0;              //등급
    public int m_MaxStar = 1;        //최대등급

}

public class WeaponData : EquipmentItemData
{
    public Vector3 m_HandPos;  //무기 손잡이 위치 보정
}

[System.Serializable]
public class UseItem : ItemData
{
    public int m_UesSlotNum = -1;      //퀵슬롯 저장 번호
}

[System.Serializable]
public class PortionItem : UseItem
{
    public PortionType m_PortionType = PortionType.None;    //포션 타입
    public float m_Value = 10;                                             //수치 
}

