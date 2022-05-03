using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadMgr : MonoBehaviour
{
    static DontDestroyOnLoadMgr inst;

    public GameObject Cam;
    public GameObject Player;
    public GameObject PlayerMark;
    public GameObject UI;
    public GameObject Mgr;

   


    private void Awake()
    {
        if(inst == null)
        {
            inst = this;
            DontDestroyOnLoad(UI);
            DontDestroyOnLoad(Player);
            DontDestroyOnLoad(PlayerMark);
            DontDestroyOnLoad(Cam);
            DontDestroyOnLoad(Mgr);
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(UI);
            Destroy(Player);
            Destroy(PlayerMark);
            Destroy(Cam);
            Destroy(Mgr);
            Destroy(this.gameObject);
        }
        
             
    }

   
}
