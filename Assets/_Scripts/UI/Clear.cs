using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clear : MonoBehaviour
{
    public Image Panel;
    public Image ClearText;
    public Image ClearBtn;

    private Coroutine _showco;
    Color color;
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (_showco == null)
           _showco = StartCoroutine(ShowPanel());
        color = Panel.color;
        color.a = 0f;
        Panel.color = color;
        ClearText.color = color;
        ClearBtn.color = color;
    }

    IEnumerator ShowPanel()
    {
        yield return new WaitForSecondsRealtime(1f);

        while (color.a < 1f)
        {
            color.a += 0.015f;
            Panel.color = color;
            ClearText.color = color;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        color.a = 1f;
        Panel.color = color;
        ClearText.color = color;
        yield return new WaitForSecondsRealtime(1f);
        ClearBtn.color = color;
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임이면 종료
        Application.Quit();
#endif
    }
}
