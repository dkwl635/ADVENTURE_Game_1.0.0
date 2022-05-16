using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MsgItem : MonoBehaviour
{
    public Image m_BoxBG = null;
    public Image m_Img = null;
    public Text m_InfoText = null;
    private float m_LifeTime = 3.0f;
    private Color m_ColorTemp = Color.white;
    

    public void Start()
    {
        Destroy(this.gameObject, m_LifeTime);
    }
    private void Update()
    {
        m_LifeTime -= Time.deltaTime;
        m_ColorTemp.a = m_LifeTime;
        m_Img.color = m_ColorTemp;
        m_InfoText.color = m_ColorTemp;
        if(m_ColorTemp.a < m_BoxBG.color.a)
        {
            m_BoxBG.color = m_ColorTemp;
        }

    }

    public void SetMsgItem(ItemData a_Item , int a_Count)
    {
        m_Img.sprite = a_Item.m_ItemSprite;
        m_InfoText.text = a_Item.m_Name; 
        m_InfoText.text += "X " + a_Count;
    }

}
