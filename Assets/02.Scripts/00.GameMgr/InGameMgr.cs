using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameMgr : MonoBehaviour
{
    static public InGameMgr Inst;

    public GameObject m_HpBarPrefab = null;
    public Canvas m_HpBarCanvas = null;


    [Header("DamageTxt_ObjectFool")]
    public Canvas m_DamageCanvas = null;
    string m_DamageTxt = "DamageTxt";
    public GameObject m_DamageTxtObj;



    [Header("FX")]
    public List<GameObject> FX_PrefabList = new List<GameObject>();
    Dictionary<string, GameObject> DicFxPrefab = new Dictionary<string, GameObject>();

    //Dictionary<string, Stack<GameObject>> ObjPoolStacks = new Dictionary<string, Stack<GameObject>>();        
    public int m_ObjPoolInit = 20; //오브젝트 풀 첫 생성 갯수

    Stack<DamageTxt> m_DamageTxtPool = new Stack<DamageTxt>();

    bool bNewUser = false;
    public Text m_StartMsg;

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



        Application.targetFrameRate = 60;


        //ObjPoolStacks[m_DamageTxt] = new Stack<GameObject>();    //데미지 텍스트 이펙트 관련
        //ObjPoolStacks["FX_BloodSplatter"] = new Stack<GameObject>();    //데미지 텍스트 이펙트 관련

        InitObjPool();
    }

    private void Start()
    {       
        ItemMgr.Inst.SpawnDropItem(new Vector3(-5.13f, 0, -18.68f), 104, 1); //아이템 떨구기
        ItemMgr.Inst.SpawnDropItem(new Vector3(-5.13f, 0, -18.68f), 501, 1); //아이템 떨구기

        if (bNewUser)
            StartCoroutine(FirstStartMsg());


        StartCoroutine(LoadData_Co());
    }

    void InitObjPool()
    {
        if (m_DamageTxtObj != null)
            for (int i = 0; i < m_ObjPoolInit; i++)
            {
                DamageTxt txtObj = Instantiate(m_DamageTxtObj, m_DamageCanvas.transform).GetComponent<DamageTxt>();
                txtObj.gameObject.SetActive(false);
                m_DamageTxtPool.Push(txtObj);
          }     
    }

    public GameObject SetHpBarObj()
    {
        return Instantiate(m_HpBarPrefab);//, m_HpBarCanvas.transform);
    }


    static public bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
   for (int i = 0; i < Input.touchCount; ++i)
   {
    a_EDCurPos.position = Input.GetTouch(i).position;  
    results.Clear();
    EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
   }
   return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 

    public void SpanwDamageTxt(Vector2 a_Pos, TxtType a_TxtType, int a_Value = 0)
    {
        DamageTxt text = null;
        if (m_DamageTxtPool.Count > 0)
            text = m_DamageTxtPool.Pop().GetComponent<DamageTxt>();
        else
        {
            text = Instantiate(m_DamageTxtObj, m_DamageCanvas.transform).GetComponent<DamageTxt>();
        }

        text.transform.position = a_Pos;
        text.OnDamageText(a_Value, a_TxtType);
    }

    public void PushBackDamageTxt(DamageTxt a_text)
    {
        m_DamageTxtPool.Push(a_text);
    }

    IEnumerator FirstStartMsg()
    {
        yield return new WaitForEndOfFrame();
        float timer = 3.0f;
        Color color = m_StartMsg.color;
        if (bNewUser)
        {
            m_StartMsg.gameObject.SetActive(true);
            Player player = GameObject.FindObjectOfType<Player>();
            player.bIsMove = false;
            while (true)
            {
                yield return null;
                if (timer > 0)
                {
                    color.a = Mathf.PingPong(Time.time, 1);
                    m_StartMsg.color = color;

                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        m_StartMsg.gameObject.SetActive(false);
                        break;
                    }
                      
                }
            }
            yield return new WaitForSeconds(0.5f);

            timer = 2.0f;
            m_StartMsg.text = "앞에 있는 NPC에게\n퀘스트를 받으세요.";
            color = Color.cyan;
            m_StartMsg.gameObject.SetActive(true);
            while (true)
            {
                yield return null;
                if (timer > 0)
                {
                    color.a = Mathf.PingPong(Time.time, 1);
                    m_StartMsg.color = color;

                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        m_StartMsg.gameObject.SetActive(false);
                        break;
                    }

                }
            }

            player.bIsMove = true;
        }


    }


    public void SaveData()
    {
        string itemdata = "";
        Player player = FindObjectOfType<Player>(true);
        SaveData saveData = new SaveData();
        saveData.m_PlayerStatus = player.m_PlayerStatus;
        #region 아이템 정보
        //장비 아이템
        List<SaveEqItem> eqItems = new List<SaveEqItem>();
        {
            ItemData[] eqdatas = player.m_PlayerInventory.PlayerEquipmentItemInven;
            for (int i = 0; i < eqdatas.Length; i++)
            {
                EquipmentItemData equipmentItemData = eqdatas[i] as EquipmentItemData;
                if(equipmentItemData != null && equipmentItemData.m_ItemCode != -1)
                {
                    SaveEqItem saveEqItem = new SaveEqItem();
                    saveEqItem.m_ItemCode = equipmentItemData.m_ItemCode;
                    saveEqItem.m_InvenIdx = i;
                    saveEqItem.m_AttPw = equipmentItemData.m_AttPw;
                    saveEqItem.m_DefPw = equipmentItemData.m_DefPw;
                    saveEqItem.m_Star = equipmentItemData.m_Star;

                    eqItems.Add(saveEqItem);
                }
            }
        }
        

        saveData.EqItemData = eqItems.ToArray();
      
        //일반 아이템
        List<SaveNoItem> Items = new List<SaveNoItem>();
        {
            ItemData[] itemDatas = player.m_PlayerInventory.PlayerItemInven;
            for (int i = 0; i < itemDatas.Length; i++)
            {
                if (itemDatas[i] != null && itemDatas[i].m_ItemCode != -1)
                {
                    SaveNoItem saveNoItem = new SaveNoItem();
                    saveNoItem.m_ItemCode = itemDatas[i].m_ItemCode;
                    saveNoItem.m_InvenIdx = i;
                    saveNoItem.m_Count = itemDatas[i].m_CurCount;


                    Items.Add(saveNoItem);
                }
            }
        }
        saveData.ItemData = Items.ToArray();


        // 장착중인 아이템
        List<SaveEqItem> equipmentParts = new List<SaveEqItem>();
        foreach (var Item in player.m_PlayerPartItem)
        {
            if (Item.Value.Equipment != null && Item.Value.Equipment.m_ItemCode != -1)
            {
                SaveEqItem saveEqItem = new SaveEqItem();
                saveEqItem.m_ItemCode = Item.Value.Equipment.m_ItemCode;
                saveEqItem.m_InvenIdx = (int)Item.Key;
     
                saveEqItem.m_AttPw = Item.Value.Equipment.m_AttPw;
                saveEqItem.m_DefPw = Item.Value.Equipment.m_DefPw;
                saveEqItem.m_Star = Item.Value.Equipment.m_Star;

                equipmentParts.Add(saveEqItem);
            }        
        }
        //무기 별도 체크
        if (player.weapon.m_WeaponData != null && player.weapon.m_WeaponData.m_ItemCode != -1)
        {
            SaveEqItem saveEqItem = new SaveEqItem();
            saveEqItem.m_ItemCode = player.weapon.m_WeaponData.m_ItemCode;
            saveEqItem.m_InvenIdx = (int)PartType.Weapon;
            saveEqItem.m_AttPw = player.weapon.m_WeaponData.m_AttPw;
            saveEqItem.m_DefPw = player.weapon.m_WeaponData.m_DefPw;
            saveEqItem.m_Star = player.weapon.m_WeaponData.m_Star;

            equipmentParts.Add(saveEqItem);
        }
        saveData.equipmentParts = equipmentParts.ToArray();

        // 퀵슬롯에 장착된 아이템 정보
        for (int i = 0; i < InventoryUIMgr.Inst.m_UseItemSlots.Length; i++)
        {
            if (InventoryUIMgr.Inst.m_UseItemSlots[i].m_ItemData != null)
                saveData.useItemSlotData[i] = InventoryUIMgr.Inst.m_UseItemSlots[i].m_ItemData.m_SlotNum;
            else
                saveData.useItemSlotData[i] = -1;
        }

        #endregion

        #region 스킬정보
        List<SaveSkill> skills = new List<SaveSkill>();
        for (int i = 0; i < SkillMgr.Inst.m_SkillList.Length; i++)
        {
            SaveSkill saveSkill = new SaveSkill();
            saveSkill.m_Lv = SkillMgr.Inst.m_SkillList[i].m_Lv;
            saveSkill.m_NeedSP = SkillMgr.Inst.m_SkillList[i].m_NeedSP;
            skills.Add(saveSkill);
        }

        saveData.skillData = skills.ToArray();

        string[] skillSlotData = new string[4] { "", "", "", "" };
        for (int i = 0; i < SkillMgr.Inst.m_SkillSlots.Length; i++)
        {
            if (SkillMgr.Inst.m_SkillSlots[i].m_Skill == null)
                continue;

            skillSlotData[i] = SkillMgr.Inst.m_SkillSlots[i].m_SkillName;           
        }
        saveData.skillSlotData = skillSlotData;
        #endregion

        #region 퀘스트
        List<SaveQuest> saveQuests = new List<SaveQuest>();
        for (int i = 0; i < QuestMgr.Inst.m_AllQuestList.Count; i++)
        {
            SaveQuest saveQuest = new SaveQuest();
            saveQuest.m_QuestID = QuestMgr.Inst.m_AllQuestList[i].m_QuestId;
            saveQuest.bEndQuest = QuestMgr.Inst.m_AllQuestList[i].bEndQuest;      
            saveQuest.bIsSuccess = QuestMgr.Inst.m_AllQuestList[i].bIsSuccess;      
         

            if (QuestMgr.Inst.m_AllQuestList[i].m_QuestType.Equals(QuestType.Kill))
            {
                saveQuest.count = (QuestMgr.Inst.m_AllQuestList[i] as KillQuest).m_CurCount;
            }
            else if (QuestMgr.Inst.m_AllQuestList[i].m_QuestType.Equals(QuestType.Collection))
            {
                saveQuest.count = (QuestMgr.Inst.m_AllQuestList[i] as CollectQuest).m_CurCount;
            }

            saveQuests.Add(saveQuest);
        }
        saveData.saveQuests = saveQuests.ToArray();
        #endregion



        itemdata = JsonUtility.ToJson(saveData);
        Debug.Log(itemdata);
        PlayerPrefs.SetString("ItemData", itemdata);    
    }

    public void  LoadData()
    {
        //SaveData saveData = GlobalValue.player01Data;
        string str = PlayerPrefs.GetString("ItemData", "");
        SaveData saveData = JsonUtility.FromJson<SaveData>(str);
        Player player = FindObjectOfType<Player>(true);
        player.m_PlayerStatus = saveData.m_PlayerStatus;
        player.SetHpUI();
        player.SetExpUI();

        #region 아이템 정보
        //장비아이템 등록
        for (int i = 0; i < saveData.EqItemData.Length; i++)
        {
            ItemData item = ItemMgr.Inst.InstantiateItem(saveData.EqItemData[i].m_ItemCode);
            EquipmentItemData eqItem = item as EquipmentItemData;

            eqItem.m_AttPw = saveData.EqItemData[i].m_AttPw;
            eqItem.m_DefPw = saveData.EqItemData[i].m_DefPw;
            eqItem.m_Star = saveData.EqItemData[i].m_Star;

            player.m_PlayerInventory.PlayerEquipmentItemInven[saveData.EqItemData[i].m_InvenIdx] = eqItem;
            InventoryUIMgr.Inst.SetEquipSlot(saveData.EqItemData[i].m_InvenIdx, eqItem);
        }

        //일반 아이템 등록
        for (int i = 0; i < saveData.ItemData.Length; i++)
        {
            ItemData item = ItemMgr.Inst.InstantiateItem(saveData.ItemData[i].m_ItemCode, saveData.ItemData[i].m_Count);
            player.m_PlayerInventory.PlayerItemInven[saveData.ItemData[i].m_InvenIdx] = item;
            InventoryUIMgr.Inst.SetItemSlot(saveData.ItemData[i].m_InvenIdx, item);
        }

        //장착한 장비아이템 등록
        for (int i = 0; i < saveData.equipmentParts.Length; i++)
        {
            ItemData item = ItemMgr.Inst.InstantiateItem(saveData.equipmentParts[i].m_ItemCode);
            EquipmentItemData eqItem = item as EquipmentItemData;

            eqItem.m_AttPw = saveData.equipmentParts[i].m_AttPw;
            eqItem.m_DefPw = saveData.equipmentParts[i].m_DefPw;
            eqItem.m_Star = saveData.equipmentParts[i].m_Star;

            player.m_PlayerInventory.EquipItem(eqItem);
        }

        //사용아이템 슬롯 등록
        for (int i = 0; i < saveData.useItemSlotData.Length; i++)
        {
            if (saveData.useItemSlotData[i] == -1)
                continue;
               
            InventoryUIMgr.Inst.m_UseItemSlots[i].SetSlot(player.m_PlayerInventory.PlayerItemInven[saveData.useItemSlotData[i]]);
        }
        #endregion

        #region 스킬정보
        for (int i = 0; i < SkillMgr.Inst.m_SkillList.Length; i++)
        {
            SkillMgr.Inst.m_SkillList[i].m_Lv = saveData.skillData[i].m_Lv;
            SkillMgr.Inst.m_SkillList[i].m_NeedSP = saveData.skillData[i].m_NeedSP;                 
        }
        //스킬 퀵슬롯
        for (int i = 0; i < SkillMgr.Inst.m_SkillSlots.Length; i++)
        {
            if (saveData.skillSlotData[i] == "")
                continue;
         
            SkillMgr.Inst.m_SkillSlots[i].SetSlot(SkillMgr.Inst.m_Skills[saveData.skillSlotData[i]]);

        }

        #endregion

        #region 퀘스트        
        for (int i = 0; i < saveData.saveQuests.Length; i++)
        {
            Quest quest = QuestMgr.Inst.DicQuest[saveData.saveQuests[i].m_QuestID];     
            quest.bEndQuest = saveData.saveQuests[i].bEndQuest;
            quest.bIsSuccess = saveData.saveQuests[i].bIsSuccess;

            //////////////////////////////////////////////////////////////퀘스트 추가
            if (!quest.bEndQuest)
                QuestMgr.Inst.AddQuest(saveData.saveQuests[i].m_QuestID);
            else
                QuestMgr.Inst.m_AllQuestList.Add(quest);

            if (!quest.bIsSuccess)
            {
                if (quest.Equals(QuestType.Kill))
                {
                    (quest as KillQuest).m_CurCount = saveData.saveQuests[i].count;
                }
                else if (quest.Equals(QuestType.Collection))
                {
                   (quest as CollectQuest).m_CurCount = saveData.saveQuests[i].count;           
                }

              
            }
           
            QuestMgr.Inst.QuestNPCRefresh();
        }
      

        #endregion

    }

    IEnumerator LoadData_Co()
    {
        yield return new WaitForEndOfFrame();
        LoadData();
    }
}

