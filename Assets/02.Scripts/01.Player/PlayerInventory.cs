using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour 
{
    Player player;

    int m_Count = 42;
    ItemData[] m_PlayerEquipmentItemInven; //플레이어 장비아이템 목록
    ItemData[] m_PlayerItemInven;                //플레이어 일반아이템 목록

    public ItemData[] PlayerEquipmentItemInven
    {
        get
        {
            return m_PlayerEquipmentItemInven;
        }
    }

    public ItemData[] PlayerItemInven
    {
        get
        {
            return m_PlayerItemInven;
        }
    }


    //보유중인 코인
    [Header("Coin")]
    public int m_Coin = 0;
    public GameObject m_ColnMsgBox;
    public RawImage m_CoinBoxBG;
    public RawImage m_CoinImg;
    public Text m_CoinMsgTxt;
    public float m_TimeColorA = 1.5f;
    private int m_OnceCoin = 0;
    Color m_CoinMsgColor = Color.white;
    Animation m_CoinMsgAnim = null;

    [Header("Hair")]
    public SkinnedMeshRenderer m_HairMesh = null;  //머리카락을 담당하는 스킨
    public Mesh m_Hair = null;                                  //만약 머리 파트가 없을 경우 필요한 머리카락 메쉬 
    public Mesh m_HairHalf = null;                             //만약 머리 파트가 있을 경우 필요한 머리카락 메쉬 
    public GameObject m_Face = null;                        //필요시 얼굴(face)를 가려야 할때 사용

    List<KeyValuePair<ItemData,int>> m_AddItemData = new List<KeyValuePair<ItemData, int>>();// 흭득한 아이템 연출을 위한 리스트
    float m_UpdateAddItemTime = 0.2f;

    Outline outline;

    //정렬시 필요한 변수
    ItemComparer itemComparer = new ItemComparer();


    private void Awake()
    {
        m_PlayerEquipmentItemInven = new ItemData[m_Count];
        m_PlayerItemInven = new ItemData[m_Count];

         player = GetComponent<Player>();

        EquipmentPart[] equipmentParts = GetComponentsInChildren<EquipmentPart>();
        for(int i = 0; i < equipmentParts.Length; i++ )
        {
           player.m_PlayerPartItem.Add(equipmentParts[i].m_PartType, equipmentParts[i]);         
        }

        //코인 관련
        m_CoinMsgAnim = m_ColnMsgBox.GetComponent<Animation>();

        outline = this.gameObject.GetComponentInChildren<Outline>();
    }

    void Update()
    {  
        //흭득한 코인 연출을 위한
        CoinMsgUpdate();
        //흭득한 아이템 연출을 위한
        SendItemMsg_Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        DropItem newItem = other.GetComponent<DropItem>();
        if(newItem != null)
        {
            if (newItem.m_ItemData == null)
                return;         
            if (AddNewItem(newItem.m_ItemData))
            {           
                Destroy(other.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DropItem newItem = collision.gameObject.GetComponent<DropItem>();
        if (newItem != null)
        {
            if (newItem.m_ItemData == null)
                return;
            if (AddNewItem(newItem.m_ItemData))
            {
                Destroy(collision.gameObject);
            }
        }
    }

    public bool AddNewItem(ItemData a_NewItemData) //아이템을 추가할수 있는지 판단 하고 결과 리턴
    {
        if (a_NewItemData == null) //만약 데이터가 없다면
            return false;   
        int idx = -1; //아이템을 넣을 슬롯 번호

        //아이템 타입 체크
        //장비템
        if (a_NewItemData.m_ItemType == ItemType.Equipment)
        {
           
            //1, 빈 자리가 있는지
            idx = FindEquipmentEmptyIndex();
            Debug.Log(idx);
            //2, 아이템 추가
            if (idx == -1)  //빈자리가 없다. 그러니 실패
                return false;
            else
            {   
                m_PlayerEquipmentItemInven[idx] = a_NewItemData;
                a_NewItemData.m_SlotNum = idx;
                InventoryUIMgr.Inst.SetEquipSlot(idx, a_NewItemData);
                m_AddItemData.Add(new KeyValuePair<ItemData, int>(a_NewItemData, 1));
                return true;
            }
           
        }
        else  // 그외 아이템
        {
            bool bOverlap = true;

            //1, 중복 체크
            idx = FindItemInventory(a_NewItemData.m_ItemCode);
            if (idx == -1)  //중복된 아이템이 없으면
                bOverlap = false;

            // 1 - 1 중복되었다면 최대 가질수 있는 숫자를 체크 하고 새로 추가 할지 수량만 추가할지 
            if (bOverlap == true)
            {//중복 되었다면 
                //중복아이템 정보 가져오기
                ItemData oldItemData = m_PlayerItemInven[idx];
                //갯수 비교 후 카운트 해주기
                if (oldItemData.m_CurCount + a_NewItemData.m_CurCount > a_NewItemData.m_MaxCount)
                {// 기존 갯수 + 새로운 갯수  > 최대 갯수       
                 //빈자리 체크
                    int otherIdx = FindEmptyIndex();
                    if (otherIdx == -1)  //빈자리가 없다. 그러니 실패
                        return false;

                    int Count = a_NewItemData.m_CurCount;             
                    int addCount = 0;

                    addCount = (oldItemData.m_CurCount + Count) - oldItemData.m_MaxCount;               
                    
                    oldItemData.m_CurCount = oldItemData.m_MaxCount;
                    a_NewItemData.m_CurCount = addCount;
                    
                    m_PlayerItemInven[otherIdx] = a_NewItemData; //아이템 추가\
                    a_NewItemData.m_SlotNum = otherIdx;
                    InventoryUIMgr.Inst.SetItemSlot(otherIdx, a_NewItemData);
                    InventoryUIMgr.Inst.m_ItemSlots[idx].RefreshSlot();
                    m_AddItemData.Add(new KeyValuePair<ItemData, int>(a_NewItemData, Count));
                    return true;                  
                }
                else
                {//기존 갯수 + 새로운 갯수  <= 최대 갯수
                    oldItemData.m_CurCount += a_NewItemData.m_CurCount;
                    InventoryUIMgr.Inst.m_ItemSlots[idx].RefreshSlot();
                    m_AddItemData.Add(new KeyValuePair<ItemData, int>(a_NewItemData, a_NewItemData.m_CurCount));

                  
                    return true;    //새로운 카운트 해주고 추가 성공 리턴
                }
            }
            else
            {               
                //2, 새로운 슬롯에 추가 하기
                idx = FindEmptyIndex();
                if (idx == -1)  //빈자리가 없다. 그러니 실패
                    return false;
                else
                {
                    m_PlayerItemInven[idx] = a_NewItemData; //아이템 추가\
                    a_NewItemData.m_SlotNum = idx;
                    InventoryUIMgr.Inst.SetItemSlot(idx, a_NewItemData);
                    m_AddItemData.Add(new KeyValuePair<ItemData, int>(a_NewItemData, a_NewItemData.m_CurCount));
                    return true;
                }
            }

           
        }
    }
    private int FindEquipmentEmptyIndex()  //비어있는 장비아이템 인덱스 반환
    {
        for(int i = 0; i < m_PlayerEquipmentItemInven.Length; i++)
            if(m_PlayerEquipmentItemInven[i] == null)
                return i;

        return -1;
    }
    private int FindEmptyIndex()  //비어있는 일반아이템 인덱스 반환
    {     
        for(int i = 0; i < m_PlayerItemInven.Length; i++)
            if(m_PlayerItemInven[i] == null)
                return i;

        return -1;
    }

    bool EquipmentEmpty()
    {      
        for (int i = 0; i < m_PlayerEquipmentItemInven.Length; i++)
            if (m_PlayerEquipmentItemInven[i] != null)            
                return false;

        return true;

    }

    private int FindItemInventory(int a_ItemCode)//일반 아이템목록에서 같은 아이템 코드가 있는지 확인하고 그 아이템이 있는 번호를 반환
    {
        for (int i = 0; i < m_PlayerItemInven.Length; i++)
        {
            if (m_PlayerItemInven[i] == null)
                continue;

            if (m_PlayerItemInven[i].m_ItemCode == a_ItemCode)  //만약 같은 아이템코드가 있으면 같은 아이템
            {
                if (m_PlayerItemInven[i].m_CurCount < m_PlayerItemInven[i].m_MaxCount)
                    return i;                
            }       
        }
        return -1;
    }
    public void ChangeEquipItem(int a_BeginIdx, int a_EndIdx)
    {       
        //인벤토리 데이터 바꾸기
        ItemData tempItemData = m_PlayerEquipmentItemInven[a_EndIdx];
        m_PlayerEquipmentItemInven[a_EndIdx] = m_PlayerEquipmentItemInven[a_BeginIdx];       
            m_PlayerEquipmentItemInven[a_BeginIdx] = tempItemData;


        //바꾼 데이터 슬롯에 적용시키기
        InventoryUIMgr.Inst.SetEquipSlot(a_EndIdx, m_PlayerEquipmentItemInven[a_EndIdx]);
        InventoryUIMgr.Inst.SetEquipSlot(a_BeginIdx, m_PlayerEquipmentItemInven[a_BeginIdx]);
        

    }
    public void ChangeItem(int a_BeginIdx, int a_EndIdx)
    {
        ItemData benginItem = m_PlayerItemInven[a_BeginIdx];
        ItemData endItem = m_PlayerItemInven[a_EndIdx];
        //아이템이 있을경구
        if(endItem != null && benginItem.m_ItemCode == endItem.m_ItemCode)
        {         
            if (m_PlayerItemInven[a_BeginIdx].m_ItemCode == m_PlayerItemInven[a_EndIdx].m_ItemCode)
            {            
                if (benginItem.m_CurCount + endItem.m_CurCount <= endItem.m_MaxCount)
                {                
                    endItem.m_CurCount = benginItem.m_CurCount + endItem.m_CurCount;
                    benginItem.m_CurCount = 0;
                    m_PlayerItemInven[a_BeginIdx] = null;//비우기
                    InventoryUIMgr.Inst.m_ItemSlots[a_EndIdx].RefreshSlot();
                    InventoryUIMgr.Inst.m_ItemSlots[a_BeginIdx].SetSlot(null);
                    return;
                }
                else
                {                 
                    benginItem.m_CurCount = (benginItem.m_CurCount + endItem.m_CurCount) - benginItem.m_MaxCount;
                    endItem.m_CurCount = endItem.m_MaxCount;
                    InventoryUIMgr.Inst.m_ItemSlots[a_BeginIdx].RefreshSlot();
                    InventoryUIMgr.Inst.m_ItemSlots[a_EndIdx].RefreshSlot();
                    return;
                }
            }

        }
        //else //다른 아이템일경우

        ItemData tempItemData = endItem;
        m_PlayerItemInven[a_EndIdx] = m_PlayerItemInven[a_BeginIdx];
        m_PlayerItemInven[a_BeginIdx] = tempItemData;

        //바꾼 데이터 슬롯에 적용시키기
        InventoryUIMgr.Inst.SetItemSlot(a_BeginIdx, m_PlayerItemInven[a_BeginIdx]);
        InventoryUIMgr.Inst.SetItemSlot(a_EndIdx, m_PlayerItemInven[a_EndIdx]);
        

    }
    public void EquipItem(int  a_Idx)
    {
        if (player.bIsAttack)
            return;

        EquipmentItemData beginItemData = m_PlayerEquipmentItemInven[a_Idx] as EquipmentItemData;
        if (beginItemData == null)
            return;

        if (beginItemData.m_PartType == PartType.Weapon)
        {
            WeaponData newWeaponData = beginItemData as WeaponData;
            if (newWeaponData == null)
                return;

            if (player.weapon.m_WeaponData == null)//비어 있다면
            {
                player.weapon.ChangeWeapon(newWeaponData);

                m_PlayerEquipmentItemInven[a_Idx] = null;

                InventoryUIMgr.Inst.SetPartSlot(beginItemData.m_PartType, beginItemData);
                InventoryUIMgr.Inst.SetEquipSlot(a_Idx, null);
            }
             else //다른 무기가 있으면
            {
                WeaponData oldWeaponData = player.weapon.m_WeaponData;
                player.weapon.ChangeWeapon(newWeaponData);
                m_PlayerEquipmentItemInven[a_Idx] = oldWeaponData;
                InventoryUIMgr.Inst.SetEquipSlot(a_Idx, oldWeaponData);
                InventoryUIMgr.Inst.SetPartSlot(beginItemData.m_PartType, beginItemData);
            }

            player.animator.SetBool("UseWeapon", true);

        }
        else
        {
            if (player.m_PlayerPartItem[beginItemData.m_PartType].Equipment == null)
            {
                player.m_PlayerPartItem[beginItemData.m_PartType].ChangeEquipment(beginItemData);

                m_PlayerEquipmentItemInven[a_Idx] = null;

                InventoryUIMgr.Inst.SetPartSlot(beginItemData.m_PartType, beginItemData);
                InventoryUIMgr.Inst.SetEquipSlot(a_Idx, null);
            }
            else//장비창에 무언가 있을때
            {
                EquipmentItemData endItemData = player.m_PlayerPartItem[beginItemData.m_PartType].Equipment;

                player.m_PlayerPartItem[beginItemData.m_PartType].ChangeEquipment(beginItemData);
                m_PlayerEquipmentItemInven[a_Idx] = endItemData;

                InventoryUIMgr.Inst.SetEquipSlot(a_Idx, endItemData);
                InventoryUIMgr.Inst.SetPartSlot(beginItemData.m_PartType, beginItemData);

            }

            //머리 장비는 특이사항
            if (player.m_PlayerPartItem[beginItemData.m_PartType].m_PartType == PartType.Head)
            {
                m_HairMesh.gameObject.SetActive(true);
                m_Face.SetActive(true);
                m_HairMesh.sharedMesh = m_Hair;

                if (player.m_PlayerPartItem[beginItemData.m_PartType].Equipment.m_AddType == EquipmentAddType.HairHalf)
                {
                    m_HairMesh.sharedMesh = m_HairHalf;
                }
                else if (player.m_PlayerPartItem[beginItemData.m_PartType].Equipment.m_AddType == EquipmentAddType.NoFace)
                {
                    m_HairMesh.gameObject.SetActive(false);
                    m_Face.SetActive(false);
                }
            }
        }

        outline.MeshUpdate();
    }
    public void OffEquipment(PartType a_PartType, int a_EndIdx)
    {
        if (player.bIsAttack)
            return;

        if (m_PlayerEquipmentItemInven[a_EndIdx] == null)//비어 있는 경우
        {            
            
            if (a_PartType == PartType.Head)
            {
                m_HairMesh.gameObject.SetActive(true);
                m_Face.SetActive(true);
                m_HairMesh.sharedMesh = m_Hair;
            }

           
            if(a_PartType == PartType.Weapon)
            {
                m_PlayerEquipmentItemInven[a_EndIdx] = player.weapon.m_WeaponData;
                InventoryUIMgr.Inst.SetEquipSlot(a_EndIdx, player.weapon.m_WeaponData);
                player.weapon.OffWepon();
                player.animator.SetBool("UseWeapon", false);

            }
            else
            {
                m_PlayerEquipmentItemInven[a_EndIdx] = player.m_PlayerPartItem[a_PartType].Equipment;
                InventoryUIMgr.Inst.SetEquipSlot(a_EndIdx, player.m_PlayerPartItem[a_PartType].Equipment);
                player.m_PlayerPartItem[a_PartType].OffEquipment();
            }

            InventoryUIMgr.Inst.m_PartSlot[a_PartType].SetSlot(null);

        }
        else// 뭔가 있는 경우
        {                   
            if (a_PartType == PartType.Weapon)
            {
                WeaponData oldWeaponData = player.weapon.m_WeaponData;
                WeaponData newWeaponData = m_PlayerEquipmentItemInven[a_EndIdx] as WeaponData;
                if (newWeaponData == null)
                    return;

                player.weapon.ChangeWeapon(newWeaponData);
                m_PlayerEquipmentItemInven[a_EndIdx] = oldWeaponData;

                InventoryUIMgr.Inst.SetEquipSlot(a_EndIdx, oldWeaponData);
                InventoryUIMgr.Inst.SetPartSlot(a_PartType, newWeaponData);
            }
            else
            {
                EquipmentItemData beginItemData = player.m_PlayerPartItem[a_PartType].Equipment;
                EquipmentItemData endItemData = m_PlayerEquipmentItemInven[a_EndIdx] as EquipmentItemData;

                if (endItemData == null)
                    return;

                if (endItemData.m_PartType != beginItemData.m_PartType)    //같은 파트 장비가 아닌경우
                    return;

                if (a_PartType == PartType.Head)
                {
                    m_HairMesh.gameObject.SetActive(true);
                    m_Face.SetActive(true);
                    m_HairMesh.sharedMesh = m_Hair;

                    if (player.m_PlayerPartItem[a_PartType].Equipment.m_AddType == EquipmentAddType.HairHalf)
                    {
                        m_HairMesh.sharedMesh = m_HairHalf;
                    }
                    else if (player.m_PlayerPartItem[a_PartType].Equipment.m_AddType == EquipmentAddType.NoFace)
                    {
                        m_HairMesh.gameObject.SetActive(false);
                        m_Face.SetActive(false);
                    }
                }

                player.m_PlayerPartItem[a_PartType].ChangeEquipment(endItemData);
                m_PlayerEquipmentItemInven[a_EndIdx] = beginItemData;

                InventoryUIMgr.Inst.SetEquipSlot(a_EndIdx, beginItemData);
                InventoryUIMgr.Inst.SetPartSlot(a_PartType, endItemData);
            }
       }
    }
    public void CoinMsgUpdate()
    {
        if (m_TimeColorA > 0.0f)
            m_TimeColorA -= Time.deltaTime;

        m_CoinMsgColor.a = m_TimeColorA;
        m_CoinBoxBG.color = m_CoinMsgColor;
        m_CoinImg.color = m_CoinMsgColor;
        m_CoinMsgTxt.color = m_CoinMsgColor;

        if (m_TimeColorA <= 0.0f)
        {
            m_OnceCoin = 0;
            return;
        }

    }
    public void AddCoin(int a_Coin)
    {
        if (m_OnceCoin == 0)
            m_CoinMsgAnim.Play();

        SoundMgr.Inst.PlaySound("GetCoin");

        m_Coin += a_Coin;
        m_OnceCoin += a_Coin;
        m_TimeColorA = 3.0f;
        m_CoinMsgTxt.text = "X " + m_OnceCoin;
    }
    void SendItemMsg_Update()
    {
        if (m_UpdateAddItemTime < 0)
        {
            if (m_AddItemData.Count > 0)
            {
                SendItemMsg(m_AddItemData[0].Key, m_AddItemData[0].Value);
                QuestMgr.Inst.CheckCollectQuest(m_AddItemData[0].Key.m_ItemCode, m_AddItemData[0].Value);
                
                m_AddItemData.RemoveAt(0);

                if(m_AddItemData.Count > 10)
                    m_UpdateAddItemTime = 0.08f;
                else
                    m_UpdateAddItemTime = 0.15f;
            }
        }
        else
            m_UpdateAddItemTime -= Time.deltaTime;

    }
    public void SendItemMsg(ItemData a_Item , int a_Count)
    {
        GameObject msgBox = (GameObject)Instantiate(player.m_MsgBoxPrefab, player.m_MsgBoxTr);
        msgBox.transform.SetAsFirstSibling();
        MsgItem msg = msgBox.GetComponent<MsgItem>();
        msg.SetMsgItem(a_Item, a_Count);
    }
    public void DestroyItem(int a_Idx, int a_Count = 0)
    {
        m_PlayerItemInven[a_Idx].m_CurCount -= a_Count;

        if (m_PlayerItemInven[a_Idx].m_CurCount <= 0)
        {
            m_PlayerItemInven[a_Idx] = null;
            InventoryUIMgr.Inst.SetItemSlot(a_Idx, null);
        }
        else
            InventoryUIMgr.Inst.m_ItemSlots[a_Idx].RefreshSlot();


    }
    public void DestoryEquipmentItem(int a_Idx)
    {
        m_PlayerEquipmentItemInven[a_Idx] = null;
        InventoryUIMgr.Inst.SetEquipSlot( a_Idx, null );
    }

    public void TrimEquipItem()
    {
        if (EquipmentEmpty())
            return;

        int start = -1; //빈칸이 있는 위치
        int next = -1;   //빈칸 이후 다음 아이템
                            
        //다음 빈칸 찾기
        while (m_PlayerEquipmentItemInven[++start] != null) ;
        next = start;

        while(true)//정렬 시작
        {
            next++; //다음칸이동

            if (next >= m_PlayerEquipmentItemInven.Length)//만약 끝까지돌았을 경우
                break;

            //다음 아이템을 발견한다면
            if (m_PlayerEquipmentItemInven[next] != null)
            {
                //아이템 옮기기
                m_PlayerEquipmentItemInven[start] = m_PlayerEquipmentItemInven[next];
                m_PlayerEquipmentItemInven[next] = null;

                //인덱스 바꿔주기
                start++;
            }
        }
    }

    public void SortEquipItem()
    {
        if (EquipmentEmpty())
            return;

        int start = -1; //빈칸이 있는 위치
        int next = -1;   //빈칸 이후 다음 아이템

        //다음 빈칸 찾기
        while (m_PlayerEquipmentItemInven[++start] != null) ;
        next = start;

        while (true)//정렬 시작
        {
            next++; //다음칸이동

            if (next >= m_PlayerEquipmentItemInven.Length)//만약 끝까지돌았을 경우
                break;

            //다음 아이템을 발견한다면
            if (m_PlayerEquipmentItemInven[next] != null)
            {
                //아이템 옮기기
                m_PlayerEquipmentItemInven[start] = m_PlayerEquipmentItemInven[next];
                m_PlayerEquipmentItemInven[next] = null;

                //인덱스 바꿔주기
                start++;
            }
        }

        Array.Sort(m_PlayerEquipmentItemInven, 0, start, itemComparer);

    }
}

class ItemComparer : IComparer<ItemData>
{   
     int IComparer<ItemData>.Compare(ItemData a, ItemData b)
    {
        return a.m_ItemCode - b.m_ItemCode;
    }
}
