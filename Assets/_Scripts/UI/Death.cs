using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Death : MonoBehaviour
{
    public Image _panel;
    public GameObject RespawnTextGO;
    public TMP_Text RespawnText;
    public GameObject RespawnTimerGO;
    public TMP_Text RespawnTimer;
    private float _timer = 3f;
    private float _untilTime = 0f;
    private bool _loading = false;

    void Start()
    {
        RespawnTextGO.SetActive(false);
        RespawnTimerGO.SetActive(false);
        var color = _panel.color;
        color.a = 0f;
        _panel.color = color;
        
        StartCoroutine(SetText());
    }

    IEnumerator SetText()
    {
        if (_loading) yield break;
        _loading = true;

        var gm = GameManager.Instance;
        yield return StartCoroutine(SetAlpha());
        RespawnTextGO.SetActive(true);
        RespawnTimerGO.SetActive(true);
        StartCoroutine(SetTextDot());
        while(_timer > _untilTime)
        {
            RespawnTimer.text = ((int)_timer+1).ToString();
            _timer -= Time.deltaTime;
            yield return null;
        }
        gm.IsPlayerDead = false;
        gm.CloseDeathUI();
        SceneManager.LoadScene("Game");
    }

    IEnumerator SetTextDot()
    {
        int dotCount = 0;
        string baseText = "Respawn";

        while (true)
        {
            dotCount++;

            if (dotCount > 3)
                dotCount = 0;

            RespawnText.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(0.5f);
        }
    }


    IEnumerator SetAlpha()
    {
        Color color = _panel.color;
        while (color.a < 1f)
        {
            color.a += 0.01f;
            _panel.color = color;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        color.a = 1f;
        _panel.color = color;

        yield return new WaitForSecondsRealtime(1f);
    }

}
