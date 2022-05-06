using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHelp : MonoBehaviour
{
    NPC m_MyNPC;
    public int m_Quest_ID = -1;
    public GameObject m_QuestObj;


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
        if(QuestMgr.Inst.CheckQuest(m_Quest_ID))
        {
            return;
        }

        TalkMgr.Inst.OnQuestBtn(m_Quest_ID);
    }

   public void OffQuestObj()
   {
       m_QuestObj.SetActive(false);
   }

}
