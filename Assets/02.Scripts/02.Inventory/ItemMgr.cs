using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemMgr : MonoBehaviour
{
    static public ItemMgr Inst;

    public Button m_RandomSpawnItemBtn = null;

    List<Dictionary<string, object>> NomarlItemTable;   //일반아이템 테이블
    List<Dictionary<string, object>> EquipmentTable; //장비아이템 테이블
    List<Dictionary<string, object>> PortionTable;       //포션아이템 테이블
    List<Dictionary<string, object>> WeaponTable;       //장비무기아이템 테이블
  

    public GameObject m_DropItemPrefab = null;
  
    public Mesh[] m_ItemMesh;
    Dictionary<string, Mesh> m_ItemMeshTable = new Dictionary<string, Mesh>();
    public Sprite[] m_ItemSprites;
    Dictionary<string, Sprite> m_ItemSpriteTable = new Dictionary<string, Sprite>();
    public Material[] m_ItemMaterial;
    Dictionary<string, Material> m_ItemMaterialTable = new Dictionary<string, Material>();


   public GameObject m_UseSlotGroup;
    UseItemSlot[] m_UseItemSlots;

    Vector3 m_TempPos = Vector3.zero;


    private void Awake()
    {
        Inst = this;

        NomarlItemTable = new List<Dictionary<string, object>>();
        EquipmentTable = CSVReader.Read("EquipmentDataTable");
        PortionTable = CSVReader.Read("PortionItemDataTable");
        WeaponTable = CSVReader.Read("WeaponDataTable");

        //테스트 아이템 스폰 해보기
        if (m_RandomSpawnItemBtn != null)
            m_RandomSpawnItemBtn.onClick.AddListener( TestSpawnItem );

        //아이템 메쉬 테이블 등록
        if(m_ItemMesh.Length > 0)
            for(int i = 0; i < m_ItemMesh.Length; i++)
                m_ItemMeshTable.Add( m_ItemMesh[i].name, m_ItemMesh[i] );

        //아이템 스프라이트 테이블 등록
        if(m_ItemSprites.Length > 0)
            for(int i = 0; i < m_ItemSprites.Length; i++)
                m_ItemSpriteTable.Add( m_ItemSprites[i].name, m_ItemSprites[i] );

        //아이템 메테리얼 테이블 등록
        if(m_ItemMaterial.Length > 0)
            for(int i = 0; i < m_ItemMaterial.Length; i++)
                m_ItemMaterialTable.Add( m_ItemMaterial[i].name, m_ItemMaterial[i] );

        if (m_UseSlotGroup != null)
            m_UseItemSlots = m_UseSlotGroup.GetComponentsInChildren<UseItemSlot>();
         
    }

    private void Start()
    {
        if (m_UseItemSlots != null)
            for (int i = 0; i < m_UseItemSlots.Length; i++)
                m_UseItemSlots[i].SetSlot(null);
    }

    private void Update()
    {
        KeyDown_Updata();
    }


    //테스트 용 아이템 스폰 생성기
    public void TestSpawnItem()
    {
        Vector3 pos = Vector3.zero;
        for (int i = 0; i < 5; i++)
        {
            int randNum = Random.Range(101, 105);
            //int randNum = 3;

            pos.x = pos.x + 1.5f;
            SpawnDropItem(pos, randNum, 1);                                                                 
        }

        pos = Vector3.zero;
        for (int i = 0; i < 5; i++)
        {
            int randNum = Random.Range(501, 504);
            pos.y = 2.0f;
            pos.x = pos.x + 1.5f;

            SpawnDropItem(pos, randNum, 1);
        }

    }
    //드랍 아이템 생성기
    public void  SpawnDropItem(Vector3 a_ItemPos, int a_ItemCode= 1, int a_ItemCount=1)
    {      
        GameObject dropObj = Instantiate(m_DropItemPrefab); //드랍하는 아이템 만들기
        DropItem dropitem = dropObj.GetComponent<DropItem>();   //아이템 설정하기 위해서

        if (dropitem == null)
            return;

        dropObj.transform.position = a_ItemPos; //위치 설정
        dropitem.InitDropItem(InstantiateItem(a_ItemCode, a_ItemCount));    //아이템을 만들고 설정 시키기
       
      
        //위치 예외처리
        if (dropitem.m_ItemData.m_ItemType == ItemType.Equipment)
        {
            EquipmentItemData equipmentItemData = dropitem.m_ItemData as EquipmentItemData;         
            if (equipmentItemData != null && equipmentItemData.m_PartType == PartType.Glove)
            {
                dropitem.m_Mesh.transform.localScale = new Vector3(0.5f, 1, 1);  //예외처리
            }
        }
    }
    //새로운 아이템 데이터 만들기
    public ItemData InstantiateItem(int a_ItemCode, int a_Count = 1)
    {
        int idx = a_ItemCode;

        if (a_ItemCode > 0 && a_ItemCode <= 100)//장비 아이템은 코드 1 ~ 100 사용예정
        {
            EquipmentItemData newEquipData = new EquipmentItemData();    //새로운 장비 타입 아이템 생성
            #region 기본 아이템 정보 입력
            newEquipData.m_ItemCode = a_ItemCode;    //아이템 코드
            newEquipData.m_ItemType = (ItemType)((int)EquipmentTable[idx]["ItemType"]); // //아이템 타입
            newEquipData.m_Name = (string)EquipmentTable[idx]["ItemName"];      //아이템 이름
            newEquipData.m_ItemInfo = (string)EquipmentTable[idx]["ItemInfo"];  //아이템 설명
            newEquipData.m_Price = (int)EquipmentTable[idx]["Price"];           //가격

            m_TempPos = Vector3.zero;
            m_TempPos.x = (float)EquipmentTable[idx]["DropItemPosX"]; //땅에 드롭되을때 위치보정 값
            m_TempPos.y = (float)EquipmentTable[idx]["DropItemPosY"]; //땅에 드롭되을때 위치보정 값
            m_TempPos.z = (float)EquipmentTable[idx]["DropItemPosZ"]; //땅에 드롭되을때 위치보정 값
            newEquipData.m_DropMeshPos = m_TempPos;
            newEquipData.m_CurCount = a_Count;
            newEquipData.m_MaxCount = 1;
            string temp = "";
            temp = (string)EquipmentTable[idx]["ItemSprite"];
            if (m_ItemSpriteTable.ContainsKey(temp))
                newEquipData.m_ItemSprite = m_ItemSpriteTable[temp];  //아이템 스프라이트

            temp = (string)EquipmentTable[idx]["ItemMesh"];
            if (m_ItemMeshTable.ContainsKey(temp))
                newEquipData.m_ItemMesh = m_ItemMeshTable[temp]; //아이템 메쉬

            temp = (string)EquipmentTable[idx]["ItemMaterial"];
            if (m_ItemMaterialTable.ContainsKey(temp))
                newEquipData.m_ItemMatrl = m_ItemMaterialTable[temp]; //아이템 메테리얼

            #endregion
            #region 장비 아이템 정보 입력
            newEquipData.m_PartType = (PartType)((int)EquipmentTable[idx]["PartType"]); // //장비 부위 타입
            newEquipData.m_AddType = (EquipmentAddType)((int)EquipmentTable[idx]["AddType"]); //추가적으로 설정이 필요한 타입
            newEquipData.m_AttPw = (int)EquipmentTable[idx]["AttPw"]; //공격력
            newEquipData.m_DefPw = (int)EquipmentTable[idx]["DefPw"]; //방어력
            newEquipData.m_MaxStar = (int)EquipmentTable[idx]["MaxStar"]; //최대 성급
            #endregion

            return newEquipData; //만들어진 아이템정보를 반환
        }
        else if(a_ItemCode > 100 && a_ItemCode <= 200)// 무기 아이템 코드  
        {
            idx -= 100;//아이템테이블에서 순서는 1부터이므로 
            WeaponData newWeaponData = new WeaponData();    //새로운 장비 타입 아이템 생성
            #region 기본 아이템 정보 입력
            newWeaponData.m_ItemCode = a_ItemCode;    //아이템 코드
            newWeaponData.m_ItemType = (ItemType)((int)WeaponTable[idx]["ItemType"]); // //아이템 타입
            newWeaponData.m_Name = (string)WeaponTable[idx]["ItemName"];      //아이템 이름
            newWeaponData.m_ItemInfo = (string)WeaponTable[idx]["ItemInfo"];  //아이템 설명
            newWeaponData.m_Price = (int)WeaponTable[idx]["Price"];           //가격
            
            m_TempPos = Vector3.zero;
            m_TempPos.x = (float)WeaponTable[idx]["DropItemPosX"]; //땅에 드롭되을때 위치보정 값
            m_TempPos.y = (float)WeaponTable[idx]["DropItemPosY"]; //땅에 드롭되을때 위치보정 값
            m_TempPos.z = (float)WeaponTable[idx]["DropItemPosZ"]; //땅에 드롭되을때 위치보정 값
            newWeaponData.m_DropMeshPos = m_TempPos;
            newWeaponData.m_CurCount = a_Count;
            newWeaponData.m_MaxCount = 1;
            string temp = "";
            temp = (string)WeaponTable[idx]["ItemSprite"];
            if (m_ItemSpriteTable.ContainsKey(temp))
                newWeaponData.m_ItemSprite = m_ItemSpriteTable[temp];  //아이템 스프라이트
            temp = (string)WeaponTable[idx]["ItemMesh"];
            if (m_ItemMeshTable.ContainsKey(temp))
                newWeaponData.m_ItemMesh = m_ItemMeshTable[temp]; //아이템 메쉬
            temp = (string)WeaponTable[idx]["ItemMaterial"];
            if (m_ItemMaterialTable.ContainsKey(temp))
                newWeaponData.m_ItemMatrl = m_ItemMaterialTable[temp]; //아이템 메테리얼
     
            #endregion

            #region 장비 아이템 정보 입력
            newWeaponData.m_PartType = (PartType)((int)WeaponTable[idx]["PartType"]); // //장비 부위 타입
            newWeaponData.m_AddType = (EquipmentAddType)((int)WeaponTable[idx]["AddType"]); //추가적으로 설정이 필요한 타입
            newWeaponData.m_AttPw = (int)WeaponTable[idx]["AttPw"]; //공격력
            newWeaponData.m_DefPw = (int)WeaponTable[idx]["DefPw"]; //방어력
            newWeaponData.m_MaxStar = (int)WeaponTable[idx]["MaxStar"]; //최대 성급
            #endregion

            #region 무기정보 입력
            m_TempPos.x = (float)WeaponTable[idx]["HandPosX"]; //손위치 보정
          
            m_TempPos.y = (float)WeaponTable[idx]["HandPosY"];
         
            m_TempPos.z = (float)WeaponTable[idx]["HandPosZ"];
           
            newWeaponData.m_HandPos = m_TempPos;
            #endregion

            return newWeaponData; //만들어진 아이템정보를 반환

        }
        else if (a_ItemCode > 500 && a_ItemCode <= 1000)//일반 아이템은 코드 500 ~ 1000 사용예정
        {          
            idx -= 500; //아이템테이블에서 순서는 1부터이므로 

            if (a_ItemCode > 500 && a_ItemCode < 700) //포션 아이템 코드 500~700
            {               
                PortionItem newPortion = new PortionItem();           
                #region 기본 아이템 정보 입력
                newPortion.m_ItemCode = a_ItemCode;    //아이템 코드
                newPortion.m_ItemType = (ItemType)((int)PortionTable[idx]["ItemType"]); // //아이템 타입
                newPortion.m_Name = (string)PortionTable[idx]["ItemName"];      //아이템 이름
                newPortion.m_ItemInfo = (string)PortionTable[idx]["ItemInfo"];  //아이템 설명
                newPortion.m_Price = (int)PortionTable[idx]["Price"];           //가격
                m_TempPos.x = (float)PortionTable[idx]["DropItemPosX"]; //땅에 드롭되을때 위치보정 값
                m_TempPos.y = (float)PortionTable[idx]["DropItemPosY"]; //땅에 드롭되을때 위치보정 값
                m_TempPos.z = (float)PortionTable[idx]["DropItemPosZ"]; //땅에 드롭되을때 위치보정 값
                newPortion.m_DropMeshPos = m_TempPos;

                string temp = "";
                temp = (string)PortionTable[idx]["ItemSprite"];
                if (m_ItemSpriteTable.ContainsKey(temp))
                    newPortion.m_ItemSprite = m_ItemSpriteTable[temp];  //아이템 스프라이트
               
                temp = (string)PortionTable[idx]["ItemMesh"];
                if (m_ItemMeshTable.ContainsKey(temp))
                    newPortion.m_ItemMesh = m_ItemMeshTable[temp]; //아이템 메쉬

                temp = (string)PortionTable[idx]["ItemMaterial"];
                if (m_ItemMaterialTable.ContainsKey(temp))
                    newPortion.m_ItemMatrl = m_ItemMaterialTable[temp]; //아이템 메테리얼
                #endregion
                #region 포션 아이템 정보 입력
                newPortion.m_PortionType = (PortionType)((int)PortionTable[idx]["PortionType"]);    //포션 타입
                newPortion.m_Value = (int)PortionTable[idx]["Value"];   //포션 효과 값

                newPortion.m_CurCount = a_Count;
                #endregion

                return newPortion;
            }
            else
            {
                ItemData newItemData = new ItemData();
                #region 기본 아이템 정보 입력
                newItemData.m_ItemCode = a_ItemCode;    //아이템 코드
                newItemData.m_ItemType = (ItemType)((int)NomarlItemTable[idx]["ItemType"]); // //아이템 타입
                newItemData.m_Name = (string)NomarlItemTable[idx]["ItemName"];      //아이템 이름
                newItemData.m_ItemInfo = (string)NomarlItemTable[idx]["ItemInfo"];  //아이템 설명
                newItemData.m_Price = (int)NomarlItemTable[idx]["Price"];           //가격
                m_TempPos.x = (float)EquipmentTable[idx]["DropItemPosX"]; //땅에 드롭되을때 위치보정 값
                m_TempPos.y = (float)EquipmentTable[idx]["DropItemPosY"]; //땅에 드롭되을때 위치보정 값
                m_TempPos.y = (float)EquipmentTable[idx]["DropItemPosZ"]; //땅에 드롭되을때 위치보정 값
                newItemData.m_DropMeshPos = m_TempPos;

                newItemData.m_CurCount = a_Count;
                newItemData.m_MaxCount = 99;
                string temp = "";
                temp = (string)NomarlItemTable[idx]["ItemSprite"];
                if (m_ItemSpriteTable.ContainsKey(temp))
                    newItemData.m_ItemSprite = m_ItemSpriteTable[temp];  //아이템 스프라이트

                temp = (string)NomarlItemTable[idx]["ItemMesh"];
                if (m_ItemMeshTable.ContainsKey(temp))
                    newItemData.m_ItemMesh = m_ItemMeshTable[temp]; //아이템 메쉬

                temp = (string)NomarlItemTable[idx]["ItemMaterial"];
                if (m_ItemMaterialTable.ContainsKey(temp))
                    newItemData.m_ItemMatrl = m_ItemMaterialTable[temp]; //아이템 메테리얼

             
                #endregion
            
                return newItemData;
            }

        }
        return null;
    }

    void KeyDown_Updata()
    {
        if(m_UseItemSlots.Length > 0)
        {
            for (int i = 0; i < m_UseItemSlots.Length; i++)
            {
                if(Input.GetKeyDown(m_UseItemSlots[i].m_KeyCode))
                {                  
                    m_UseItemSlots[i].UseSlotItem();
                }
            }
        }
    }
    //아이템 사용하기
    public void UseItem(ItemData a_ItemData , Player a_TargetPlayer)
    {
        if(a_ItemData.m_ItemType == ItemType.Portion)
        {
            PortionItem portionItem = a_ItemData as PortionItem;
            if(portionItem != null)
                UsePortion( portionItem, a_TargetPlayer );
        }
       
    }
    //타입별 아이템 사용
    public void UsePortion(PortionItem a_Portion, Player player)
    {       
       
        if(a_Portion.m_PortionType == PortionType.HpPortion)  //Hp 회복 효과
        {
            player.m_PlayerStatus.m_CurHp += (int)a_Portion.m_Value;
            if (player.m_PlayerStatus.m_CurHp > player.m_PlayerStatus.m_MaxHp)
                player.m_PlayerStatus.m_CurHp = player.m_PlayerStatus.m_MaxHp;

            player.SetHpUI();

            a_Portion.m_CurCount--;
            //effect
            Vector2 pos = player.m_MsgBoxTr.position;
            pos.x = pos.x - 200 + ( Random.Range( 0, 20.0f ) );
            pos.y = pos.y - 50;
            InGameMgr.Inst.SpanwDamageTxt( pos , (int)a_Portion.m_Value,TxtType.Heal);
                


        }

    }


}