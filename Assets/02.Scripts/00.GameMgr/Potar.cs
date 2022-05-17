using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Potar : MonoBehaviour
{
    public string m_SceneName;
 
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
        SceneManager.LoadScene(m_SceneName);

        SoundMgr.Inst.OffSound();
        //StartCoroutine(LoadScene("rpgpp_lt_scene_1.0 1"));
    }


   
}
