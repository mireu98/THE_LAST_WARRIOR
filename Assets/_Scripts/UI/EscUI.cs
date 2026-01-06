using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class EscUI : MonoBehaviour
{
    public Button ExitBtn;
    public Button SoundBtn;
    public GameObject SoundPanel;

    public Toggle ToggleBGM;
    public Toggle ToggleSFX;

    public AudioMixer audioMixer;

    private bool isClicked;

    void Start()
    {
        isClicked = false;
        SoundPanel.SetActive(false);

        ExitBtn.onClick.AddListener(ClickExitBtn);
        SoundBtn.onClick.AddListener(ClickSoundBtn);

        ToggleBGM.onValueChanged.AddListener(OnToggleBGM);
        ToggleSFX.onValueChanged.AddListener(OnToggleSFX);
    }

    void OnToggleBGM(bool isOn)
    {
        if (isOn)
            audioMixer.SetFloat("BGM", 0f);     // 정상 볼륨
        else
            audioMixer.SetFloat("BGM", -80f);  // 음소거
    }

    void OnToggleSFX(bool isOn)
    {
        if (isOn)
            audioMixer.SetFloat("SFX", 0f);
        else
            audioMixer.SetFloat("SFX", -80f);
    }

    public void ClickExitBtn()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ClickSoundBtn()
    {
        isClicked = !isClicked;
        if(!isClicked)
            SoundPanel.SetActive(true);
        else
            SoundPanel.SetActive(false);
    }
}
