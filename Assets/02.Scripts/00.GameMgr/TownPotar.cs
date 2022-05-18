using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownPotar : MonoBehaviour
{   
    public GameObject m_Canvas;
    Player player;

    public Button m_BackBtn;
    public Button m_Dungen1Btn;
    public Button m_Dungen2Btn;

    private void Start()
    {
        m_BackBtn.onClick.AddListener(OffCanvas);
        m_Dungen1Btn.onClick.AddListener(() => { LoadScene("WoodStage", 3); });
        m_Dungen2Btn.onClick.AddListener(() => { LoadScene("DesertStage", 3); });
    }

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<Player>();
        player.bIsMove = false;
        m_Canvas.SetActive(true);


    }

    public void LoadScene(string m_SceneName , int a_Lv)
    {
        if (player.m_PlayerStatus.m_Lv < a_Lv)
        {
            //레벨부족
            return;
        }
          

        player.bIsMove = true;
        player.gameObject.SetActive(false);
        SceneManager.LoadScene(m_SceneName);
        SoundMgr.Inst.OffSound();    
    }

    void OffCanvas()
    {
        player.bIsMove = true;
        m_Canvas.SetActive(false);
    }
}
