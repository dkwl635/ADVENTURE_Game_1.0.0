using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TalkMgr : MonoBehaviour
{
    enum TalkType
    {
        Talk,
        Shop,
        Upgrade,
        Quest,
    }
    TalkType m_CurrTalkType = TalkType.Talk;

    public static TalkMgr Inst;

    public GameObject m_TalkBtnGroup;
    Button[] m_Btns;
    public Button m_TalkBtn;    //대화 시작 버튼
    public Button m_ShopBtn;    //상점 시작 버튼
    public Button m_UpgradeBtn;    //업그레이드 시작 버튼
    public Button m_QuestBtn;    //업그레이드 시작 버튼

    public Button m_QuestOkBtn;
    public Button m_BackBtn;

    public Button m_NexTalkBtn; //다음 대화 버트
    Vector2 m_NextBtnPos = Vector2.zero;
    float m_Ypos = 0;

    public RectTransform m_TalkBox;
    public Text m_TalkTxt;
    public Text m_NameTxt;
    public Image m_PortraitImg;

    public bool m_OpenBox = false;
    public Vector2 m_OnTextBoxPos = new Vector2(-1, -480);
    public Vector2 m_OffTextBoxPos = new Vector2(-1, -800);

    int m_TalkIdx = 0;  //대화 인데스
    int m_NpcID = 0;         //대화NPC ID
    int m_QuestID = 0;  //퀘스트 주기 위해서 받아놓은 ID
    Player m_Talker;

    string teststr = "안녕하세요.:0/반갑습니다. 여기는 상점입니다.:1/감사합니다.:2";
    //string teststr = "안녕하세요.";

    public Dictionary<int, string> m_TalkTable = new Dictionary<int, string>();

    public List<string> m_TalkList = new List<string>();
    public List<Sprite> m_SpritList = new List<Sprite>();
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
        }

        m_Btns = m_TalkBtnGroup.GetComponentsInChildren<Button>(true);

    }

    private void Start()
    {
        //talk  0 : 일반 대화 , 1  : 상점 대화 , 2: 퀘스트 대화
        m_TalkTable.Add(100 + 0, "안녕하세요.:0/반갑습니다. 여기는 상점입니다.:1/감사합니다.:2");
        m_TalkTable.Add(100 + 1, "안녕하세요.:0/반갑습니다. 무언가를 파시나요.:2/감사합니다.:2");
        m_TalkTable.Add(110 + 0, "안녕하세요.:0/반갑습니다. 여기는 강화소입니다.:2/감사합니다.:2");
        m_TalkTable.Add(110 + 1, "안녕하세요.:0/반갑습니다. 무언가를 파시나요.:2/감사합니다.:2");
        m_TalkTable.Add(120 + 0, "안녕하세요.:0/Ludo 입니다..:2/저기있는 Luna에게 얘기를 들어 주세요.:2");
        m_TalkTable.Add(120 + 2, "안녕하세요.:0/Ludo 입니다..:2/저기있는 Luna에게 얘기를 들어 주세요.:2/잘부탁드립니다.:2");
        m_TalkTable.Add(130 + 0, "안녕하세요.:0/Luna 입니다..:2/저기 있는 유령몬스터 10마리만 잡아 주세요:2");
        m_TalkTable.Add(130 + 2, "안녕하세요.:0/Luna 입니다..:2/저기 있는 유령몬스터 10마리만 잡아 주세요:2/감사합니다.:2");

        m_TalkBtnGroup.SetActive(false);

        if (m_TalkBtn != null)
            m_TalkBtn.onClick.AddListener(TalkBtn);

        if (m_ShopBtn != null)
            m_ShopBtn.onClick.AddListener(ShopBtn);

        if (m_UpgradeBtn != null)
            m_UpgradeBtn.onClick.AddListener(UpgradeBtn);

        if (m_QuestBtn != null)
            m_QuestBtn.onClick.AddListener(QuestBtn);


        if (m_NexTalkBtn != null)
            m_NexTalkBtn.onClick.AddListener(NextTalk);

     


        if (m_QuestOkBtn != null)
            m_QuestOkBtn.onClick.AddListener(QuestOkBtn);

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(BackBtn);

    }

    public void Update()
    {
        if (m_OpenBox)
        {
            m_TalkBox.gameObject.SetActive(true);
            
            m_TalkBox.anchoredPosition = Vector2.Lerp(m_TalkBox.anchoredPosition, m_OnTextBoxPos, Time.deltaTime * 2);
        }
        else if (!m_OpenBox)
        {
            m_PortraitImg.gameObject.SetActive(false);
            m_TalkBox.anchoredPosition = Vector2.Lerp(m_TalkBox.anchoredPosition, m_OffTextBoxPos, Time.deltaTime * 2);

            if (m_TalkBox.anchoredPosition.y <= -700)
                m_TalkBox.gameObject.SetActive(false);
        }

        if (m_NexTalkBtn.gameObject.activeSelf)
        {
            m_Ypos = 0.1f * Mathf.Sin(Time.time * 4);
            m_NextBtnPos = m_NexTalkBtn.gameObject.transform.position;
            m_NextBtnPos.y += m_Ypos;
            m_NexTalkBtn.gameObject.transform.position = m_NextBtnPos;
        }
    }
    //대화 버튼 상자 열기
 
    public void OnTalkBtnGroup()
    {
        m_TalkBtnGroup.SetActive(true);
        
        for (int i = 0; i < m_Btns.Length; i++)
            m_Btns[i].gameObject.SetActive(false);

    }

    public void OffTalkBtnGroup()
    {
        m_TalkBtnGroup.SetActive(false);
    }

    //대화 내용 넣기
    public void SetTalkMgr(int a_Id, string a_Name, List<Sprite> a_SpritList, Player a_Talker)
    {
        m_NpcID = a_Id;
        m_SpritList = a_SpritList;
        m_NameTxt.text = a_Name;
        m_Talker = a_Talker;
    }

    //대화 내용 리스트 화 하기
    public void SetTalkList(string a_str)
    {
        m_TalkList.Clear();
        string[] str = a_str.Split('/');
        for (int i = 0; i < str.Length; i++)
        {
            m_TalkList.Add(str[i]);
        }
    }



    //일반 대화 버튼 On
    public void OnTalkBtn()
    {
        m_TalkBtn.gameObject.SetActive(true);       
    }
    //상점 대화 버튼 On
    public void OnShopBtn()
    {
        m_ShopBtn.gameObject.SetActive(true);
    }
    //업그레이드 버튼 On
    public void OnUpgradeBtn()
    {
        m_UpgradeBtn.gameObject.SetActive(true);
    }

    //퀘스트 버튼 On
    public void OnQuestBtn(int a_QuestID)
    {
        m_QuestID = a_QuestID;
        m_QuestBtn.gameObject.SetActive(true);
    }

    //일반 대화 버튼 함수
    void TalkBtn()
    {
        OffTalkBtnGroup();
        SetTalkList(m_TalkTable[m_NpcID]);
        m_CurrTalkType = TalkType.Talk;
        m_OpenBox = true;
        m_PortraitImg.gameObject.SetActive(true);
        PrintMsg();

        //퀘스트 채크
        QuestMgr.Inst.CheckTalkQuest(m_NpcID);
    }
    //상점 버튼 함수
    void ShopBtn()
    {  
        //OnOffTalkBtn(false);
        //SetTalkList(m_TalkTable[m_ID + 1]);
        //m_CurrTalkType = TalkType.Shop;
        //PrintMsg();
        ShopMgr.Inst.OpenShop();
    }

    //업그레이드 버튼 함수
    void UpgradeBtn()
    {
        UpgradeMgr.Inst.OpenUpgrade(m_Talker);
    }

    void QuestBtn()
    {
        OffTalkBtnGroup();
        SetTalkList(m_TalkTable[m_NpcID + 2]);
        m_CurrTalkType = TalkType.Quest;
        m_OpenBox = true;
        m_PortraitImg.gameObject.SetActive(true);
        PrintMsg();
    }

    //대화 출력
    void PrintMsg()
    {
        m_BackBtn.gameObject.SetActive(false);
        m_QuestOkBtn.gameObject.SetActive(false);
        m_NexTalkBtn.gameObject.SetActive(false);

       

        m_TalkTxt.text = m_TalkList[m_TalkIdx].Split(':')[0];
       
        int spriteIdx = 0;
        spriteIdx = int.Parse(m_TalkList[m_TalkIdx].Split(':')[1]);
        m_PortraitImg.sprite = m_SpritList[spriteIdx];

        if (m_TalkIdx < m_TalkList.Count - 1)
        {
            m_NexTalkBtn.gameObject.SetActive(true);
        }


        if (m_CurrTalkType.Equals(TalkType.Quest))
        {        
            if (m_TalkIdx == m_TalkList.Count - 2)
            {
                m_QuestOkBtn.gameObject.SetActive(true);
                m_BackBtn.gameObject.SetActive(true);
            }

            if(m_TalkIdx == m_TalkList.Count - 1)
            {           
                m_BackBtn.gameObject.SetActive(true);
            }

            return;
        }


        if (m_TalkIdx == m_TalkList.Count - 1)
        {
            m_BackBtn.gameObject.SetActive(true);
        }         
                 
    }
    //다음 대화 버튼 함수
    void NextTalk()
    {
        m_TalkIdx++;
        PrintMsg();    
    }
    //대화 박스 Off . 초기화
    public void OffTalkBox()
    {
        m_OpenBox = false;        
        m_TalkIdx = 0;// 대화 초기화
        m_SpritList = null;
        m_TalkList.Clear();

        for (int i = 0; i < m_Btns.Length; i++)      
            m_Btns[i].gameObject.SetActive(false);

        OffTalkBtnGroup();
    }
    
    void QuestOkBtn()
    {
        if (QuestMgr.Inst.AddQuest(m_QuestID))
            NextTalk();
        else  //이미 받으 퀘스트
        {
            EndTalk();      
        }
    }

    void EndTalk()
    {
        m_BackBtn.gameObject.SetActive(true);
        m_QuestOkBtn.gameObject.SetActive(false);
        m_NexTalkBtn.gameObject.SetActive(false);

        if (m_CurrTalkType.Equals(TalkType.Quest))
            m_TalkTxt.text = "이미 받은 퀘스트 입니다.";

        m_PortraitImg.gameObject.SetActive(false);


    }

    void BackBtn()
    {
        OffTalkBox(); 
    }


}
