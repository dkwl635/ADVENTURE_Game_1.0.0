using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        Debug.Log("테스트 무기 떨구기");
        ItemMgr.Inst.SpawnDropItem(new Vector3(-5.13f, 0, -18.68f), 104, 1); //아이템 떨구기
        ItemMgr.Inst.SpawnDropItem(new Vector3(-5.13f, 0, -18.68f), 501, 1); //아이템 떨구기



    }

    void InitObjPool()
    {
        if (m_DamageTxtObj != null)
            for (int i = 0; i < m_ObjPoolInit; i++)
            {
                DamageTxt txtObj = Instantiate(m_DamageTxtObj, m_DamageCanvas.transform).GetComponent<DamageTxt>();
                txtObj.gameObject.SetActive(false);
                m_DamageTxtPool.Push(txtObj);

                //GameObject textObj = Instantiate(m_DamageTxtObj, m_DamageCanvas.transform);
                //textObj.SetActive(false);
                //ObjPoolStacks[m_DamageTxt].Push(textObj);
            }

        //FX ObjectPool
        //for (int i = 0; i < FX_PrefabList.Count; i++)
        //{
        //    DicFxPrefab.Add(FX_PrefabList[i].name, FX_PrefabList[i]);

        //    for (int j = 0; j < m_ObjPoolInit; j++)
        //    {
        //        GameObject fx = Instantiate(FX_PrefabList[i],this.transform);

        //        fx.SetActive(false);

        //        string str = FX_PrefabList[i].name;
        //        ObjPoolStacks[str].Push(fx);
        //    }
        //}



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

    public void SaveData()
    {
        string itemdata = "";
        Player player = FindObjectOfType<Player>();
        testSave test = new testSave();

        test.testItem = player.m_PlayerInventory.PlayerItemInven;
        itemdata = JsonUtility.ToJson(test);

        Debug.Log(itemdata);

    
    }

}

public class testSave
{
    public ItemData[] testItem;
}