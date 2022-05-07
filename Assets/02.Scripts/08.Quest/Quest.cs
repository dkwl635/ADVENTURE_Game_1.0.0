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
    //퀘스트 완료시 ++ 해서 만약 퀘스트가 있다면 그 퀘스트를 받을 수 있다.

    public int m_QuestId = -1;     //퀘스트 아이디
    public QuestType m_QuestType = QuestType.None; //퀘스트 타입
    public string m_QuestName = "";
    public string m_QuestInfo = ""; //퀘스트 설명
    public string m_QuestStatus { get { return QuestStatus(); } }    //퀘스트 상태

    public int m_RewardCoin = 0;   //퀘스트 보상 코인
    public int m_RewardExp = 0;

    public int m_RewardItem = 0; //퀘스트 보상 아이템 목록    //필요하다면
    public int m_RewardItemCount = 0; //퀘스트 보상 아이템 갯수 목록    //필요하다면

    [HideInInspector] public ItemData m_RewardItemData = null;


    public bool bIsSuccess = false;    //성공인지 실패 인지

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