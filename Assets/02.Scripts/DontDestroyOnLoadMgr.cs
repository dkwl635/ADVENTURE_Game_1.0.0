using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadMgr : MonoBehaviour
{
    static public DontDestroyOnLoadMgr inst;

    public GameObject Cam;
    public GameObject Player;
    public GameObject PlayerMark;
    public GameObject UI;

   
    private void Awake()
    {
        if(inst == null)
        {
           
            inst = this;
            DontDestroyOnLoad(this.gameObject);
            if (UI || Player || PlayerMark || Cam)
            {
                DontDestroyOnLoad(UI);
                DontDestroyOnLoad(Player);
                DontDestroyOnLoad(PlayerMark);
                DontDestroyOnLoad(Cam);
            }         
        }
        else
        {
            Destroy(this.gameObject);
            if (UI || Player || PlayerMark || Cam)
            {
                Destroy(UI);
                Destroy(Player);
                Destroy(PlayerMark);
                Destroy(Cam);
   
            }
            
        }          
    }

    public void AllDestory()
    {
       
        if (UI || Player || PlayerMark || Cam)
        {
            Destroy(UI);
            Destroy(Player);
            Destroy(PlayerMark);
            Destroy(Cam);

        }

        Destroy(this.gameObject);
    }
}
