using UnityEngine;


// 게임 내의 모든 사운드(BGM, 효과음) 재생 및 볼륨을 관리하는 서비스 인터페이스입니다.
public interface IAudioService 
{
    void PlaySound(string a_Name);
    void ChangeBGM(string a_BgmName);
    void ChangeBGMVolume(float Volume);
    void ChangeEffectVolume(float Volume);
    void OffSound();
    float GetBGMVolume();
    float GetEffectVolume();

}