using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopHelp : MonoBehaviour 
{
    NPC m_MyNPC;
    public List<SellItem> m_SellItemList = new List<SellItem>();

    void Start()
    {
        m_MyNPC = GetComponent<NPC>();
        if (m_MyNPC != null)
        { 
            m_MyNPC.Talk += OpenShop;          
        }
    }

    public void OpenShop(Player a_player)
    {        
        TalkMgr.Inst.OnShopBtn();       
        ShopMgr.Inst.SetSellItemList( m_SellItemList,a_player);
    }

    

}
