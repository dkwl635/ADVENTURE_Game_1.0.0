using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MeumUIMgr : MonoBehaviour
{

    public Button m_MeumBtn = null;
    public RectTransform m_BtnsGroup = null;
    [SerializeField] Vector2 m_OffVector = Vector2.zero;
    [SerializeField] Vector2 m_OnVector = Vector2.zero;
    bool bOnOff = false;

    public Button m_InvenBtn = null;
    public Button m_SkillBtn = null;
    public Button m_QuestBtn = null;
    public Button m_MiniMapBtn = null;

    public GameObject m_MiniMapObj;

    private void Awake()
    {
        m_OffVector = m_BtnsGroup.anchoredPosition;
        m_OnVector = m_OffVector;
        m_OnVector.x = 0;

        

    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetMiniMap();

        m_MeumBtn.onClick.AddListener(OnOffMeum);

        m_InvenBtn.onClick.AddListener(InventoryUIMgr.Inst.OnInvenUI);
        m_SkillBtn.onClick.AddListener(SkillMgr.Inst.OnSkillUI);
        m_QuestBtn.onClick.AddListener(QuestMgr.Inst.OnQuestUI);
        m_MiniMapBtn.onClick.AddListener(OnMiniMap);
    }

    private void Update()
    {
        if(bOnOff)
            m_BtnsGroup.anchoredPosition = Vector2.Lerp(m_BtnsGroup.anchoredPosition, m_OnVector, 0.1f);
        else
            m_BtnsGroup.anchoredPosition = Vector2.Lerp(m_BtnsGroup.anchoredPosition, m_OffVector, 0.1f);


        if (Input.GetKeyDown(KeyCode.Z))
            OnOffMeum();

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (m_MiniMapObj.activeSelf)
                OffMiniMap();
            else OnMiniMap();

        }
       


    }

    void OnOffMeum()
    {
        bOnOff = !bOnOff;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMiniMap();
       
    }

    void SetMiniMap()
    {
        m_MiniMapObj = GameObject.Find("MiniCanvas").transform.GetChild(0).gameObject;
        if (m_MiniMapObj)
        {
            m_MiniMapObj.GetComponentInChildren<Button>().onClick.AddListener(OffMiniMap);
            
        }
          
    }

    void OffMiniMap()
    {
        m_MiniMapObj.SetActive(false);
    }

    void OnMiniMap()
    {
        m_MiniMapObj.SetActive(true);
    }

   


}
