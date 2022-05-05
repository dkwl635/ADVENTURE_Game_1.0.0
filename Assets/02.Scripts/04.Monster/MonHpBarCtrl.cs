using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonHpBarCtrl : MonoBehaviour
{
    public GameObject m_HpBarObj = null;
    public Image m_Hpbar = null;

    Transform cam; 

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void Update()
    {
        transform.LookAt(cam);
    }


    public void SetHpBar(float a_CurHp, float a_MaxHp)
    {
        m_Hpbar.fillAmount = a_CurHp / a_MaxHp;
        if ((a_CurHp == a_MaxHp)  || a_CurHp <= 0)
            gameObject.SetActive(false);
        else if(a_CurHp < a_MaxHp)
            gameObject.SetActive(true);                  
    }

}
