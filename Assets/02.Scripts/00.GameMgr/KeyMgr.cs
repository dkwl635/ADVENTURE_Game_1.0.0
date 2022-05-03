using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMgr : MonoBehaviour
{
    public static KeyMgr Inst; //싱글톤 패턴

    public delegate void KeyDownAction();   //키를 눌렀을때  저장할 델리게이트
    public delegate void KeyAction();           //키를 누르고있을때  저장할 델리게이트
    public delegate void KeyUpAction();      //키를 뗐을때 저장할 델리게이트

    //각 키코드 에 맞는 행동을 했을때
    public Dictionary<KeyCode, KeyDownAction> DicKeyDownAction = new Dictionary<KeyCode, KeyDownAction>();
    public Dictionary<KeyCode, KeyAction> DicKeyAction = new Dictionary<KeyCode, KeyAction>();
    public Dictionary<KeyCode, KeyUpAction> DicKeyUpAction = new Dictionary<KeyCode, KeyUpAction>();

    //사용 중인 키코드
    List<KeyCode> m_UseKeyCode = new List<KeyCode>();



    private void Awake()
    {
        if (Inst == null)
            Inst = this;
    }
   

    void Update()
    {
        
        if (DicKeyDownAction.Count > 0)
        {         
            foreach (KeyCode keycode in DicKeyDownAction.Keys)
            {
                if (Input.GetKeyDown(keycode))
                {
                    DicKeyDownAction[keycode]?.Invoke();  
                }                 
            }
        }    

        if (DicKeyAction.Count > 0)
        {
            foreach (KeyCode keycode in DicKeyAction.Keys)
            {
                if (Input.GetKey(keycode))
                    DicKeyAction[keycode]?.Invoke();
            }
        }

        if (DicKeyUpAction.Count > 0)
        {
            foreach (KeyCode keycode in DicKeyUpAction.Keys)
            {
                if (Input.GetKeyUp(keycode))
                { 
                    DicKeyUpAction[keycode]?.Invoke();                 
                }
            }
        }

    }


}
