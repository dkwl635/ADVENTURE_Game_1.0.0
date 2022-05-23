using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MeumUIMgr : MonoBehaviour
{
    public Button m_MeumBtn = null;
    public RectTransform m_BtnsGroup = null;
    Vector2 m_OffVector = Vector2.zero;
    Vector2 m_OnVector = Vector2.zero;
    bool bOnOff = false;

    public Button m_InvenBtn = null;
    public Button m_SkillBtn = null;
    public Button m_QuestBtn = null;
    public Button m_MiniMapBtn = null;
    public Button m_ConfigBtn = null;

    public GameObject m_MiniMapObj;
    public GameObject m_ConfigBoxPanel;

    public GameObject m_Skillmark;
    public GameObject m_Quesmark;
    public GameObject m_Mainmark;
    


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
        m_SkillBtn.onClick.AddListener(() => { SkillMgr.Inst.OnSkillUI(); m_Skillmark.SetActive(false); });
        m_QuestBtn.onClick.AddListener(() => { QuestMgr.Inst.OnQuestUI(); m_Quesmark.SetActive(false); });
        m_MiniMapBtn.onClick.AddListener(OnMiniMap);
        m_ConfigBtn.onClick.AddListener(OnConfigBox);

        FindObjectOfType<Player>().LevelUpEvent += () => { m_Skillmark.SetActive(true); };
        QuestMgr.Inst.QuestEvent += () => { m_Quesmark.SetActive(true); };
    }

    private void Update()
    {
        if (m_Skillmark.activeSelf || m_Quesmark.activeSelf)
            m_Mainmark.SetActive(true);
        else
            m_Mainmark.SetActive(false);


        if (bOnOff)
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

        if (Input.GetKeyDown(KeyCode.A))
            m_Quesmark.SetActive(false);

        if (Input.GetKeyDown(KeyCode.K))
            m_Skillmark.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
            OnConfigBox();
    }

    void OnOffMeum()
    {
        bOnOff = !bOnOff;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene" || scene.name == "LoadingScene")
            return;

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
        SoundMgr.Inst.PlaySound("Slide");
        m_MiniMapObj.SetActive(false);
    }

    void OnMiniMap()
    {
        SoundMgr.Inst.PlaySound("Slide");
        m_MiniMapObj.SetActive(true);
    }

   void OnConfigBox()
    {
        m_ConfigBoxPanel.SetActive(true);      
    }

    void OffConfigBox()
    {
        m_ConfigBoxPanel.SetActive(false);
    }

}
