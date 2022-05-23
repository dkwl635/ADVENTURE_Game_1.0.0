using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue 
{
    public static string player1 = "";
    public static string player2 = "";
    public static string player3 = "";


    public static SaveData playerData1;
    public static SaveData playerData2;
    public static SaveData playerData3;

    public static int playerNum;

    public static float StartTimer;
 
   static public void LoadData()
    {
        player1 =  PlayerPrefs.GetString("ItemData1", "");
        player2 =  PlayerPrefs.GetString("ItemData2", "");
        player3 =  PlayerPrefs.GetString("ItemData3", "");
    }

    static public string SetStartBtn(int num)
    {
        string dataStr = "";
        
        switch (num)
        {
            case 1: dataStr = player1; break;
            case 2: dataStr = player2; break;
            case 3: dataStr = player3; break;
        }

      

        string str = "";
        str += num + "번 슬롯  ";

        if(string.IsNullOrEmpty(dataStr))
        {
            str += " : 비어 있음 ";
            return str;
        }

        SaveData playerData = null;
        playerData = JsonUtility.FromJson<SaveData>(dataStr);        
        str += " : Lv " + playerData.m_PlayerStatus.m_Lv;   
        
        str+= "\n\n코인 :" + playerData.coin;

        int minute = (int)playerData.playtime / 60;
        int second = (int)playerData.playtime - (minute * 60);
            
        int second1 = second / 10;
        int second2 = second % 10;
        str += "\n플레이 타임 :" + minute.ToString() + "분 " + second1.ToString() + second2.ToString() +"초"; 

        return str;
    }
    static public void ResetData(int num)
    {   
        switch (num)
        {
            case 1: PlayerPrefs.SetString("ItemData1", ""); break;
            case 2: PlayerPrefs.SetString("ItemData2", ""); break;
            case 3: PlayerPrefs.SetString("ItemData3", ""); break;
        }

        switch (num)
        {
            case 1: player1 = PlayerPrefs.GetString("ItemData1", ""); break;
            case 2: player2 = PlayerPrefs.GetString("ItemData2", ""); break;
            case 3: player3 = PlayerPrefs.GetString("ItemData3", ""); break;
        }
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