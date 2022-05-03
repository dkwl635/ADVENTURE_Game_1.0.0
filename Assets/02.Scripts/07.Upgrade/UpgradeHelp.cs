using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHelp : MonoBehaviour
{
    NPC m_MyNPC;
   
    void Start()
    {
        m_MyNPC = GetComponent<NPC>();
        if (m_MyNPC != null)
        {
            m_MyNPC.Talk += OpenUpgrade;
            m_MyNPC.TalkEnd += CloseUpgrade;
        }
    }

    public void OpenUpgrade(Player a_player)
    {
        TalkMgr.Inst.OnUpgradeBtn();    
    }

    public void CloseUpgrade(Player a_player)
    {
        UpgradeMgr.Inst.OffUpgrade();
    }
}
