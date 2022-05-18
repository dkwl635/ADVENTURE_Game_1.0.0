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
        Player player = FindObjectOfType<Player>();
        SaveData test = new SaveData();

        //장비 아이템 
        //test.EqItemData = player.m_PlayerInventory.PlayerEquipmentItemInven;
        ////일반 아이템
        //test.ItemData = player.m_PlayerInventory.PlayerItemInven;
        //장착중인 아이템
        int idx = 0;
        foreach (var Item in player.m_PlayerPartItem.Values)
        {
            if (Item.Equipment != null)
            {
                test.equipmentParts[idx] = Item.Equipment;
            }
            idx++;
        }
        idx = 0;
        //퀵슬롯에 장착된 아이템 정보
        //for (int i = 0; i < InventoryUIMgr.Inst.m_UseItemSlots.Length; i++)
        //{
        //    if (InventoryUIMgr.Inst.m_UseItemSlots[i].m_ItemData != null)
        //        test.useItemSlotData[i] = InventoryUIMgr.Inst.m_UseItemSlots[i].m_ItemData.m_SlotNum;
        //    else
        //        test.useItemSlotData[i] = -1;
        //}



        itemdata = JsonUtility.ToJson(test);

        Debug.Log(itemdata);

    
    }

}

public class SaveData
{
    //public ItemData[] ItemData;
    //public ItemData[] EqItemData;
    //
    public ItemData[] equipmentParts = new ItemData[7];
    //public int[] useItemSlotData = new int[4];

}