using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorCtrl : MonoBehaviour
{
    public Texture2D m_CursorTexture;

    public Vector2 adjustHotSpot = Vector2.zero;  



    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MyCursor());
    }

    IEnumerator MyCursor()
    {
        //모든 렌더링이 완료될 때까지 대기할테니 렌더링이 완료되면
        //깨워 달라고 유니티 엔진에 게 부탁하고 대기합니다.
        yield return new WaitForEndOfFrame();
              
        //이제 새로운 마우스 커서를 화면에 표시합니다.
        Cursor.SetCursor(m_CursorTexture, adjustHotSpot, CursorMode.Auto);
    }


}
