using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPos : MonoBehaviour
{
    public Player player;
    public string m_BGM_Name;
    void Start()
    { 

        StartCoroutine(SetPlayerPos());
    }

    IEnumerator SetPlayerPos()
    {
        yield return null;

        player =  FindObjectOfType<Player>(true);

        if (player)
        {
            player.transform.position = this.transform.position;
            player.gameObject.SetActive(true);

            Camera.main.GetComponent<CameraCtrl>().InitCamera();
        }

        SoundMgr.Inst.ChangeBGM(m_BGM_Name);

    }


}
