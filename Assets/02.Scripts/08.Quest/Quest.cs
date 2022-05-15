using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestType
{
    None,
    Talk, //대화 하기
    Kill,   //죽이는 거
    Move,   //이동
    Collection, //수집
}

[System.Serializable]
public class Quest 
{  
    public int m_QuestId = -1;     //퀘스트 아이디
    public int m_BeforeQuestId = -1; //선행퀘스트
    public QuestType m_QuestType = QuestType.None; //퀘스트 타입
    public string m_QuestName = "";
    public string m_QuestInfo = ""; //퀘스트 설명
    public string m_QuestStatus { get { return QuestStatus(); } }    //퀘스트 상태

    public int m_RewardCoin = 0;   //퀘스트 보상 코인
    public int m_RewardExp = 0;     //퀘스트 보상 경험치

    public int m_RewardItem = 0; //퀘스트 보상 아이템 목록    //필요하다면
    public int m_RewardItemCount = 0; //퀘스트 보상 아이템 갯수 목록    //필요하다면

    [HideInInspector] public ItemData m_RewardItemData = null;


    public bool bIsSuccess = false;    //성공인지 실패 인지
    public bool bEndQuest = false;

    protected virtual string QuestStatus()
    {
        return "";
    }

}

[System.Serializable]
public class TalkQuest : Quest 
{
    public string m_GoalNpcName = "";
    public int m_GoalNPCId = -1;   //목표 NPC ID

    //대회 내용 퀘스트 확인용
    public void CheckQuest(int a_NPCId)
    {
        if (bIsSuccess.Equals(true))    //성공한 퀘스트라면
            return;

        if (a_NPCId.Equals(m_GoalNPCId))
        {
            bIsSuccess = true;
            return;
        }     
    }

    protected override string QuestStatus()
    {
        if (bIsSuccess)
        {
            return m_GoalNpcName + "와(과) 대화 하기 (완료)";
        }
        else
            return m_GoalNpcName + "와(과) 대화 하기";
    }

}

[System.Serializable]
public class KillQuest : Quest
{
    public string m_KillMonster = "";   //목표 몬스터 이름
    public int m_KillMonsterId = -1;   //목표 몬스터 ID

    public int m_CurCount = 0;      //현재 몬스터 카운트
    public int m_GoalCount = 0;    //목표 카운트
    public void CheckQuest(int a_MonsterId)
    {
        if (bIsSuccess.Equals(true))    //성공한 퀘스트라면
            return;

        if (a_MonsterId.Equals(m_KillMonsterId))
        {
            m_CurCount++;
            if (m_CurCount >= m_GoalCount)
                bIsSuccess = true;       
        }
    }

    protected override string QuestStatus()
    {
        if (bIsSuccess)
        {
            return m_KillMonster + " 처치 하기 (완료)";          
        }
        else
            return m_KillMonster + " 처치 하기 (" + m_CurCount + "/" + m_GoalCount + ")";
    }
}

public class CollectQuest : Quest
{
    public string m_GoalItemName = "";  //목표 수집 아이템 이름
    public int m_GoalItem = 0; //목표 수직아이템 코드
    public int m_CurCount = 0;
    public int m_GoalCount = 0;

    public void CheckQuest(int a_ItemCode, int a_Count)
    {
        if (bIsSuccess.Equals(true))    //성공한 퀘스트라면
            return;

        if (a_ItemCode.Equals(m_GoalItem))
        {
            m_CurCount += a_Count;
            if (m_CurCount >= m_GoalCount)
                bIsSuccess = true;
        }

    }

    protected override string QuestStatus()
    {
        if (bIsSuccess)
        {
            return m_GoalItemName + " 획득 하기 (완료)";
        }
        else
            return m_GoalItemName + "(" + m_CurCount + "/" + m_GoalCount + ") 흭득 하기";
    }

}