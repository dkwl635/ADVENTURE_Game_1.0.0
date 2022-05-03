using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHelp : MonoBehaviour
{
    NPC m_MyNPC;
    public int m_Quest_ID = -1;

    void Start()
    {
        m_MyNPC = GetComponent<NPC>();
        if (m_MyNPC != null)
        {
            m_MyNPC.Talk += OpenQuest;
        }
    }

    public void OpenQuest(Player a_player)
    {
        TalkMgr.Inst.OnQuestBtn(m_Quest_ID);
    }

   

}
