using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public Button LoginBtn;
    public TMP_InputField ID;
    public TMP_InputField PW;
    public Image WrongUI;
    public TMP_Text WrongText;
    private Coroutine _wrongCo;

    private void Start()
    {
        LoginBtn.onClick.AddListener(LoginCheck);

        Color color = WrongUI.color;
        color.a = 0f;
        WrongUI.color = color;
        Color color2 =WrongText.color;
        color2.a = 0f;
        WrongText.color = color2;
    }

    public void LoginCheck()
    {
        string url = "http://localhost:8081/unity/login";
        StartCoroutine(Test(url));
    }

    IEnumerator Test(string url)
    {
        string id = ID.text;
        string pw = PW.text;
        url += $"?param={UnityWebRequest.EscapeURL(id)}&param2={UnityWebRequest.EscapeURL(pw)}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                if (webRequest.downloadHandler.text.Trim() == "false")
                {
                    if (_wrongCo != null)
                        StopCoroutine(_wrongCo);

                    _wrongCo = StartCoroutine(FadeWrongPanel());
                }
                else if (webRequest.downloadHandler.text.Trim() == "true")
                {
                    SceneManager.LoadScene("Game");
                }
            }
        }
    }

    IEnumerator FadeWrongPanel()
    {
        float fadeInTime = 0.25f;
        float holdTime = 1.0f;
        float fadeOutTime = 0.25f;

        Color color = WrongUI.color;
        Color color2 = WrongText.color;
        
        // Fade In (0 ¡æ 0.8)
        float time = 0f;
        while (time < fadeInTime)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 0.8f, time / fadeInTime);
            WrongUI.color = color;
            color2.a = Mathf.Lerp(0f, 0.8f, time / fadeInTime);
            WrongText.color = color2;
            yield return null;
        }
        color.a = 0.8f;
        color2.a = 0.8f;
        WrongUI.color = color;
        WrongText.color = color2;

        // Hold
        yield return new WaitForSecondsRealtime(holdTime);

        // Fade Out (0.8 ¡æ 0)
        time = 0f;
        while (time < fadeOutTime)
        {
            time += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0.8f, 0f, time / fadeOutTime);
            WrongUI.color = color;
            color2.a = Mathf.Lerp(0.8f, 0f, time / fadeOutTime);
            WrongText.color = color2;
            yield return null;
        }
        color.a = 0f;
        color2.a = 0f;
        WrongUI.color = color;
        WrongText.color = color2;
    }
}
