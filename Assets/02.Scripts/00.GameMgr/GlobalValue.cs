using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue 
{
    public static string player01 = "";
    public static SaveData player01Data;
  

   static public void LoadData()
    {
        player01 =  PlayerPrefs.GetString("ItemData", "");
    }

    static public string SetStartBtn(int num)
    {

        string str = "";

        player01Data = JsonUtility.FromJson<SaveData>(player01);

        str += "플레이어 레벨 : " + player01Data.m_PlayerStatus.m_Lv;
        str+= "\n플레이어 코인 :" + player01Data.coin;
        str+= "\n스킬 포인트 :" + player01Data.sk_point;
        str+= "\n플레이 타임 :" + player01Data.playtime;

        return str;
    }
    
}

public class SaveData
{
    public Status m_PlayerStatus;
    public int coin = 0;
    public int sk_point = 0;
    public float playtime = 0;

    public SaveEqItem[] EqItemData;
    public SaveNoItem[] ItemData;
    public SaveEqItem[] equipmentParts;
    public int[] useItemSlotData = new int[4];

    public SaveSkill[] skillData;
    public string[] skillSlotData = new string[4] { "", "", "", "" };

    public SaveQuest[] saveQuests;

}

[System.Serializable]
public class SaveEqItem
{
    public int m_ItemCode;
    public int m_InvenIdx;
    public int m_AttPw = 1;            //공격력
    public int m_DefPw = 1;           //방어력
    public int m_Star = 0;              //등급
}

[System.Serializable]
public class SaveNoItem
{
    public int m_ItemCode;
    public int m_InvenIdx;
    public int m_Count;
}

[System.Serializable]
public class SaveSkill
{
    public int m_Lv = 1;       //스킬 레벨
    public int m_NeedSP = 5;   //레벨 업 필요 스킬 포인트
}

[System.Serializable]
public class SaveQuest
{
    public int m_QuestID;
    public bool bIsSuccess;
    public bool bEndQuest;
    public int questType;

    public int count;
}