using UnityEngine;

//데미지 텍스트 풀링 서비스
public interface IDamageTextService
{
    void SpanwDamageTxt(Vector2 a_Pos, TxtType a_TxtType, int a_Value = 0);
    void PushBackDamageTxt(DamageTxt a_text);
}
