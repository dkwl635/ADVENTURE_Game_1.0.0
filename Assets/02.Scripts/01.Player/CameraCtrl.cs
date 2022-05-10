using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Transform m_CharTr;        //추적 캐릭터 위치
    private Vector3 m_TargetPos = Vector3.zero;
    private Transform m_Tr;                  //카메라 위치

    //카메라 위치 계산용 변수
    [SerializeField] private float m_RotH = 0.0f;        //마우스 좌우 조작값 계산용
    [SerializeField] private float m_RotV = 0.0f;        //마우스 상하 조작값 계산용
    private float m_hSpeed = 5.0f;      //마우스 좌우 회전 속도 값
    private float m_vSpeed = 2.4f;      //마우스 상하 회전 속도 값
    private float m_vMinLimit = -7.0f;  //위 아래 각도 제한
    private float m_vMaxLimit = 55.0f;  //위 아래 각도 제한  
    private float m_zoomSpeed = 1.0f;   //마우스 줌 속도
    private float m_MaxDist = 50.0f;    //마우스 줌 최대 거리
    private float m_MinDist = 3.0f;     // 마우스 줌 최소 거리


    //주인공을 기준으로 한 상대적인 구좌표계 기준의 초기값
    private float m_DefaltRotH = 0.0f;  //평면 기준의 회전각도
    private float m_DefaltRotV = 45.0f; //높이 기준의 회전각도
    private float m_DefaltDist = 20.0f;  //타겟에서 카메라까지의 거리

    //계산에 필요한 변수들
    private Quaternion m_BuffRot;                   //버퍼 회전값
    private Vector3 m_BasicPos = Vector3.zero;      //
    [SerializeField] [Range(3, 50)] private float m_Distance = 20.0f;               //거리
    private Vector3 m_BuffPos;                      //버퍼 위치값
    [HideInInspector] public Vector3 navVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_Tr = GetComponent<Transform>();

        m_TargetPos = m_CharTr.transform.position;      //타겟 위치 정보 
        m_TargetPos.y = m_TargetPos.y + 1.4f;           //타겟의 머리 위쪽 위치 조정

        m_RotH = m_DefaltRotH;
        m_RotV = m_DefaltRotV;
        m_Distance = m_DefaltDist;

        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0); //각도 설정
        m_BasicPos.x = 0.0f;
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -m_Distance;     //뒤로

        m_BuffPos = (m_BuffRot * m_BasicPos) + m_TargetPos;
        transform.position = m_BuffPos;

        transform.LookAt(m_TargetPos);
    }


    private void LateUpdate()
    //private void Update()
    {
        //입력 받아오기
        if (Input.GetMouseButton(2)) //마우스 우측버튼 누르고 있는 동안
        {
            m_RotH += Input.GetAxis("Mouse X") * m_hSpeed;    //마우스 좌우 움직임을
            m_RotV += Input.GetAxis("Mouse Y") * m_vSpeed;    //마우스 위아래 움직임을

            m_RotV = ClampAngle(m_RotV, m_vMinLimit, m_vMaxLimit);
            //m_RotH = ClampAngle(m_RotH, m_hMinLimit, m_hMaxLimit);

        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && m_Distance < m_MaxDist)
        {
            m_Distance += m_zoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && m_Distance > m_MinDist)
        {
            m_Distance -= m_zoomSpeed;
        }

        m_TargetPos = m_CharTr.transform.position;
        m_TargetPos.y = m_TargetPos.y + 1.4f;

        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0); //각도 설정
        m_BasicPos.x = 0.0f;
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -m_Distance;     //뒤로

   
        m_BuffPos = (m_BuffRot * m_BasicPos) + m_TargetPos;
        if (m_BuffPos.y < m_TargetPos.y)
            m_BuffPos.y = m_TargetPos.y;

        //transform.position = m_BuffPos;
        transform.position = Vector3.SmoothDamp(transform.position, m_BuffPos, ref navVelocity, 0.0001f);
        transform.LookAt(m_TargetPos);
    }

    public float ClampAngle(float a_Angle, float a_Min, float a_Max)
    {
        if (a_Angle < -360)
            a_Angle += 360;

        if (a_Angle > 360)
            a_Angle -= 360;

        return Mathf.Clamp(a_Angle, a_Min, a_Max);
    }
}
