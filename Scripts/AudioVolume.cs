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
        //�X���C�_�[�𓮂��������̏�����o�^
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
        //5�i�K�␳
        value /= 9;
        //-80~0�ɕϊ�
        var volume = Mathf.Clamp(Mathf.Log10(value) * 10f, -100f, -20f);
        //audioMixer�ɑ��
        audioMixer.SetFloat("BGMVol", volume);
        Debug.Log($"BGM:{volume}");

    }

    //SE
    public void SetAudioMixerSE(float value)
    {
        titleManager.SetSEVolume(seSlider.value);
        //5�i�K�␳
        value /= 9;
        //-80~0�ɕϊ�
        var volume = Mathf.Clamp(Mathf.Log10(value) * 10f, -100f, -20f);
        //audioMixer�ɑ��
        audioMixer.SetFloat("SEVol", volume);
        Debug.Log($"SE:{volume}");


    }
}
