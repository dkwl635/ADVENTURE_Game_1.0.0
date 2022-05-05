using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMgr : MonoBehaviour
{
    static public QuestMgr Inst;

    Player m_Player;

    Dictionary<int, Quest> DicQuest = new Dictionary<int, Quest>();
    Dictionary<Quest, GameObject> DicQuestItem = new Dictionary<Quest, GameObject>(); //퀘스트 목록를 보여주는 오브젝트
    List<Quest> m_AllQuestList = new List<Quest>(); //퀘스트 목록
    List<TalkQuest> m_TalkQuestList = new List<TalkQuest>();    //대화로 하는 퀘스트
    List<KillQuest> m_KillQuestList = new List<KillQuest>();    //잡아야 하는 퀘스트

  

    public GameObject m_QuestPanel = null;
    public GameObject m_QuestDlgBox = null;
    public Text m_QuestName = null;
    public Text m_QuestInfo = null;
    public Text m_QuestStatus = null;
    public Text m_QuestReward = null;

    public Button m_BackBtn;
    public Button m_OkBtn;

    public Transform QuestListScrollTr;
    public GameObject m_QuestItem = null;   //퀘스트 노드

    [HideInInspector]public Quest m_CurQuest = null; //현재 퀘스트
    
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

        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_CurQuest = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        TalkQuest talkQuest1 = new TalkQuest();
        talkQuest1.m_QuestId = 1;
        talkQuest1.m_QuestType = QuestType.Talk;
        talkQuest1.m_GoalNpcName = "Luna";
        talkQuest1.m_GoalNPCId = 130;
        talkQuest1.m_QuestName= "Luna와 대화 하기";
        talkQuest1.m_QuestInfo = "옆에 있는 Luna 와 대화 하세요";
        talkQuest1.m_RewardCoin = 1000;
        talkQuest1.m_RewardExp = 1000;
        talkQuest1.m_RewardItemData = ItemMgr.Inst.InstantiateItem(501, 10);

        KillQuest killQuest1 = new KillQuest();
        killQuest1.m_QuestId = 2;
        killQuest1.m_QuestType = QuestType.Kill;
        killQuest1.m_KillMonster = "Specter";
        killQuest1.m_KillMonsterId = 1;
        killQuest1.m_GoalCount = 10;
        killQuest1.m_QuestName = "Specter를 잡아줘";
        killQuest1.m_QuestInfo  = "몬스터 Specter를 10마리 잡아주세요";
        killQuest1.m_RewardCoin = 200000;
        killQuest1.m_RewardExp = 200000;
        killQuest1.m_RewardItemData = ItemMgr.Inst.InstantiateItem(102, 1);

        DicQuest[1]= talkQuest1;
        DicQuest[2] = killQuest1;

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(OffQuestUI);

        if (m_OkBtn != null)
            m_OkBtn.onClick.AddListener(QuestClear);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            if (m_QuestPanel.activeSelf)
                OffQuestUI();
            else
                OnQuestUI();
        }
    }

    public void CheckTalkQuest(int a_NPCId)
    {
        for (int i = 0; i < m_TalkQuestList.Count; i++)
        {
            if (m_TalkQuestList[i].bIsSuccess)
                continue;

            m_TalkQuestList[i].CheckQuest(a_NPCId);
           
            if (m_TalkQuestList[i].bIsSuccess)
            {
                m_TalkQuestList.RemoveAt(i);
                Debug.Log("퀘스트 성공");
            }
                
        }
    }

    public void CheckKillQuest(int a_MonsterId)
    {   

        for (int i = 0; i < m_KillQuestList.Count; i++)
        {
            if (m_KillQuestList[i].bIsSuccess)
                continue;

            m_KillQuestList[i].CheckQuest(a_MonsterId);

            if (m_KillQuestList[i].bIsSuccess)
            {
                m_KillQuestList.RemoveAt(i);
                Debug.Log("킬 몬스터 성공");
            }           
        }
    }

   
    public bool AddQuest(int a_QuestId)
    {
        if (m_AllQuestList.Contains(DicQuest[a_QuestId]))
            return false;

        m_AllQuestList.Add(DicQuest[a_QuestId]);

        if(DicQuest[a_QuestId].m_QuestType.Equals(QuestType.Talk))
        {
            TalkQuest talkQuest  = DicQuest[a_QuestId] as TalkQuest;
            m_TalkQuestList.Add(talkQuest);
        }
        else if(DicQuest[a_QuestId].m_QuestType.Equals (QuestType.Kill))
        {
            KillQuest killQuest = DicQuest[a_QuestId] as KillQuest;
            m_KillQuestList.Add(killQuest);
        }

        InstantiateQuestItem(DicQuest[a_QuestId]);

        return true;
    }



    public void OnQuestUI()
    {
        m_QuestPanel.SetActive(true);
        m_QuestPanel.transform.SetAsLastSibling();

        m_QuestDlgBox.SetActive(false);
        if (m_CurQuest != null)
            OpenQuestDlgBox(m_CurQuest);
      
    }

    void OffQuestUI()
    {
        m_QuestPanel.SetActive(false);       
    }

    void InstantiateQuestItem(Quest quest)
    {
        GameObject questItemObj =  GameObject.Instantiate(m_QuestItem, QuestListScrollTr);

        QuestItem questItem = questItemObj.GetComponent<QuestItem>();
        questItem.m_Quest = quest;
        questItem.m_QuestInfoBtn .onClick.AddListener(() => { OpenQuestDlgBox(quest); });
        DicQuestItem.Add(quest, questItemObj);
    }

    void DestoryQuestItem(Quest quest)
    {
        Destroy(DicQuestItem[quest]);
        DicQuestItem.Remove(quest);
    }

    void OpenQuestDlgBox(Quest a_Quest)
    {
        m_QuestDlgBox.SetActive(true);

        m_QuestName.text = a_Quest.m_QuestName;
        m_QuestInfo.text = a_Quest.m_QuestInfo;
        m_QuestStatus.text = a_Quest.m_QuestStatus;
        m_QuestReward.text = "코인 : " + a_Quest.m_RewardCoin;
        m_QuestReward.text += "\n경험치 : " + a_Quest.m_RewardExp;
        m_QuestReward.text += "\n아이템 : " + a_Quest.m_RewardItemData.m_Name + " x " +
                a_Quest.m_RewardItemData.m_CurCount;

        if (a_Quest.bIsSuccess)
            m_OkBtn.gameObject.SetActive(true);
        else
            m_OkBtn.gameObject.SetActive(false);

        m_CurQuest = a_Quest;
    }

    void QuestClear()
    {
        RewardQuest(m_CurQuest);
    }

    void RewardQuest(Quest a_Quest)
    {

        //아이템 보상 -> 아이템창고 부족시 실패
        if (!a_Quest.m_RewardItemData.Equals(null))
            if (m_Player.m_PlayerInventory.AddNewItem(a_Quest.m_RewardItemData).Equals(false))
            {
                Debug.Log("아이템 창고 부족");
                return;
            }

        //코인 보상
        m_Player.m_PlayerInventory.AddCoin(a_Quest.m_RewardCoin);
        //경험치 보상
        m_Player.AddExp(a_Quest.m_RewardExp);

        m_AllQuestList.Remove(a_Quest);
        DestoryQuestItem(a_Quest);

        m_QuestDlgBox.SetActive(false);
        m_CurQuest = null;
    }

}
