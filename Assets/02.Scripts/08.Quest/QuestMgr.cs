using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMgr : MonoBehaviour
{
    static public QuestMgr Inst;

    Player m_Player;

    Dictionary<int, Quest> DicQuest = new Dictionary<int, Quest>(); //모든 퀘스트가 있는 사전
    Dictionary<Quest, GameObject> DicQuestItem = new Dictionary<Quest, GameObject>(); //퀘스트 목록를 보여주는 오브젝트
    List<Quest> m_AllQuestList = new List<Quest>(); //내가 받은 퀘스트 목록

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
            this.gameObject.transform.SetParent(GameObject.FindObjectOfType<DontDestroyOnLoadMgr>().gameObject.transform);
        }

        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_CurQuest = null;

        TestQuestMake();
    }

    void TestQuestMake()
    {
        TalkQuest talkQuest1 = new TalkQuest();
        talkQuest1.m_QuestId = 1;
        talkQuest1.m_QuestType = QuestType.Talk;
        talkQuest1.m_GoalNpcName = "Luna";
        talkQuest1.m_GoalNPCId = 130;
        talkQuest1.m_QuestName = "Luna와 대화 하기";
        talkQuest1.m_QuestInfo = "옆에 있는 Luna 와 대화 하세요";
        talkQuest1.m_RewardCoin = 1000;
        talkQuest1.m_RewardExp = 1000;
        talkQuest1.m_RewardItem = 501;
        talkQuest1.m_RewardItemCount = 10;
        //talkQuest1.m_RewardItemData = ItemMgr.Inst.InstantiateItem(501, 10);

        KillQuest killQuest1 = new KillQuest();
        killQuest1.m_QuestId = 2;
        killQuest1.m_QuestType = QuestType.Kill;
        killQuest1.m_KillMonster = "Specter";
        killQuest1.m_KillMonsterId = 1;
        killQuest1.m_GoalCount = 10;
        killQuest1.m_QuestName = "Specter를 잡아줘";
        killQuest1.m_QuestInfo = "몬스터 Specter를 10마리 잡아주세요";
        killQuest1.m_RewardCoin = 10000;
        killQuest1.m_RewardExp = 10000;
        killQuest1.m_RewardItem = 102;
        killQuest1.m_RewardItemCount = 1;
        killQuest1.m_BeforeQuestId = 1;
        
        //killQuest1.m_RewardItemData = ItemMgr.Inst.InstantiateItem(102, 1);

        KillQuest killQuest2 = new KillQuest();
        killQuest2.m_QuestId = 3;
        killQuest2.m_QuestType = QuestType.Kill;
        killQuest2.m_KillMonster = "SpecterBoss";
        killQuest2.m_KillMonsterId = 2;
        killQuest2.m_GoalCount = 1;
        killQuest2.m_QuestName = "SpecterBoss를 잡아줘";
        killQuest2.m_QuestInfo = "몬스터 SpecterBoss를 1마리 잡아주세요";
        killQuest2.m_RewardCoin = 10000;
        killQuest2.m_RewardExp = 10000;
        killQuest2.m_RewardItem = 503;
        killQuest2.m_RewardItemCount = 1;
        killQuest2.m_BeforeQuestId = 2;


        DicQuest[1] = talkQuest1;
        DicQuest[2] = killQuest1;
        DicQuest[3] = killQuest2;
    }



    // Start is called before the first frame update
    void Start()
    {
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
            talkQuest.m_RewardItemData = ItemMgr.Inst.InstantiateItem(DicQuest[a_QuestId].m_RewardItem, DicQuest[a_QuestId].m_RewardItemCount);
            m_TalkQuestList.Add(talkQuest);
        }
        else if(DicQuest[a_QuestId].m_QuestType.Equals (QuestType.Kill))
        {
            KillQuest killQuest = DicQuest[a_QuestId] as KillQuest;
            killQuest.m_RewardItemData = ItemMgr.Inst.InstantiateItem(DicQuest[a_QuestId].m_RewardItem, DicQuest[a_QuestId].m_RewardItemCount);
            m_KillQuestList.Add(killQuest);
        }

        InstantiateQuestItem(DicQuest[a_QuestId]);

        //퀘스트르 받으면 퀘스트를 주는 NPC의 머리 오브젝트 없애기
        QuestHelp[] questHelps = FindObjectsOfType<QuestHelp>();
        for (int i = 0; i < questHelps.Length; i++)
        {
            if(questHelps[i].m_Quest_ID == a_QuestId)
            {
                questHelps[i].OffQuestObj();
                break;
            }
        }


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
        //퀘스트 종료
        a_Quest.bEndQuest = true;

        //퀘스트목록창UI 노드 삭제
        //m_AllQuestList.Remove(a_Quest);
        DestoryQuestItem(a_Quest);

        m_QuestDlgBox.SetActive(false);
        m_CurQuest = null;

        //씬에 있는 모든 QuestHelp를 돌며 퀘스트상태 갱신
        QuestHelp[]  questHelps= FindObjectsOfType<QuestHelp>();
        for (int i = 0; i < questHelps.Length; i++)
        {         
            questHelps[i].QuestSet();
        }

    }

    //이미 받은 퀘스트 라면 
    public bool CheckQuest(int a_Id)
    {
        if (m_AllQuestList.Contains(DicQuest[a_Id]))
        {            
            return true;
        }

        return false; 
    }

    //선행퀘스트가 존재하고 그 퀘스트가 완료된 상태 확인
    public bool ClearCheckQuest(int a_Id)
    {
        if (DicQuest[a_Id].m_BeforeQuestId.Equals(-1))
            return true;
        else
        {             
            int beQuId = DicQuest[a_Id].m_BeforeQuestId;

            if (DicQuest[beQuId].bEndQuest)
                return true;

        }    

        return false;
    }
}
