using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{
    [SerializeField] private TitleManager titleManager;
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;

    void Start()
    {
        //スライダーを動かした時の処理を登録
        bgmSlider.onValueChanged.AddListener(SetAudioMixerBGM);
        seSlider.onValueChanged.AddListener(SetAudioMixerSE);
    }

    public void Setup(LocalData localData)
    {
        bgmSlider.value = localData.bgm;
        seSlider.value = localData.se;
        SetAudioMixerBGM(localData.bgm);
        SetAudioMixerSE(localData.se);

    }

    //BGM
    public void SetAudioMixerBGM(float value)
    {
        titleManager.SetBGMVolume(bgmSlider.value);
        //5段階補正
        value /= 9;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 10f, -100f, -20f);
        //audioMixerに代入
        audioMixer.SetFloat("BGMVol", volume);
        Debug.Log($"BGM:{volume}");

    }

    //SE
    public void SetAudioMixerSE(float value)
    {
        titleManager.SetSEVolume(seSlider.value);
        //5段階補正
        value /= 9;
        //-80~0に変換
        var volume = Mathf.Clamp(Mathf.Log10(value) * 10f, -100f, -20f);
        //audioMixerに代入
        audioMixer.SetFloat("SEVol", volume);
        Debug.Log($"SE:{volume}");


    }
}
