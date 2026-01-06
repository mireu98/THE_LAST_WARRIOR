using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [Header("Q/E/R Cooldown")]
    [SerializeField] TMP_Text QCdText;
    [SerializeField] Image QCdFill;
    [SerializeField] GameObject QCdPanel;
    [SerializeField] GameObject QLock;

    [SerializeField] TMP_Text ECdText;
    [SerializeField] Image ECdFill;
    [SerializeField] GameObject ECdPanel;
    [SerializeField] GameObject ELock;

    [SerializeField] TMP_Text RCdText;
    [SerializeField] Image RCdFill;
    [SerializeField] GameObject RCdPanel;
    [SerializeField] GameObject RLock;

    [Header("Bars")]
    [SerializeField] Image HpBarFill;
    [SerializeField] TMP_Text HpText;
    [SerializeField] Image MpBarFill;
    [SerializeField] TMP_Text MpText;
    [SerializeField] Image ExpBarFill;
    [SerializeField] TMP_Text ExpText;
    [SerializeField] TMP_Text LevelText;

    [Header("Potions")]
    [SerializeField] Image HpPotionFill;
    [SerializeField] GameObject HpPotionPanel;
    [SerializeField] TMP_Text HpPotionCoolDownText;

    [SerializeField] Image MpPotionFill;
    [SerializeField] GameObject MpPotionPanel;
    [SerializeField] TMP_Text MpPotionCoolDownText;

    [SerializeField] TMP_Text MRBCdText;
    [SerializeField] GameObject MRBPanel;
    [SerializeField] Image MRBCdFill;

    public void Init()
    {
        if (MRBPanel) MRBPanel.SetActive(false);
        if (QCdPanel) QCdPanel.SetActive(false);
        if (ECdPanel) ECdPanel.SetActive(false);
        if (RCdPanel) RCdPanel.SetActive(false);
        if (HpPotionPanel) HpPotionPanel.SetActive(false);
        if (MpPotionPanel) MpPotionPanel.SetActive(false);
    }

    private void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // HUD 업데이트
        HpBarFill.fillAmount = gm.PlayerCurrentHp / gm.PlayerMaxHp;
        HpText.text = $"{(int)gm.PlayerCurrentHp}/{(int)gm.PlayerMaxHp}";

        MpBarFill.fillAmount = gm.PlayerCurrentMp / gm.PlayerMaxMp;
        MpText.text = $"{(int)gm.PlayerCurrentMp}/{(int)gm.PlayerMaxMp}";

        ExpBarFill.fillAmount = gm.PlayerCurrentExp / gm.PlayerMaxExp;
        ExpText.text = $"{(int)gm.PlayerCurrentExp}/{(int)gm.PlayerMaxExp}";

        LevelText.text = "Lv." + gm.PlayerLevel;

        // 스킬 언락 표시
        if (gm.PlayerLevel >= 3) { if (QLock) QLock.SetActive(false); }
        if (gm.PlayerLevel >= 6) { if (ELock) ELock.SetActive(false); }
        if (gm.PlayerLevel >= 10) { if (RLock) RLock.SetActive(false); }
    }

    public void ShowQCooldown(float remain, float total)
    {
        QCdPanel.SetActive(true);
        QCdText.text = ((int)remain + 1).ToString();
        QCdFill.fillAmount = remain / total;
    }
    public void HideQCooldown() => QCdPanel.SetActive(false);

    public void ShowECooldown(float remain, float total)
    {
        ECdPanel.SetActive(true);
        ECdText.text = ((int)remain + 1).ToString();
        ECdFill.fillAmount = remain / total;
    }
    public void HideECooldown() => ECdPanel.SetActive(false);

    public void ShowRCooldown(float remain, float total)
    {
        RCdPanel.SetActive(true);
        RCdText.text = ((int)remain + 1).ToString();
        RCdFill.fillAmount = remain / total;
    }
    public void HideRCooldown() => RCdPanel.SetActive(false);

    public void ShowHpPotion(float remain, float total)
    {
        HpPotionPanel.SetActive(true);
        HpPotionFill.fillAmount = remain / total;
        HpPotionCoolDownText.text = ((int)remain + 1).ToString();
    }
    public void HideHpPotion() => HpPotionPanel.SetActive(false);

    public void ShowMpPotion(float remain, float total)
    {
        MpPotionPanel.SetActive(true);
        MpPotionFill.fillAmount = remain / total;
        MpPotionCoolDownText.text = ((int)remain + 1).ToString();
    }
    public void HideMpPotion() => MpPotionPanel.SetActive(false);

    public void ShowMRBCooldown(float remain, float total)
    {
        MRBPanel.SetActive(true);
        MRBCdText.text = ((int)remain + 1).ToString();
        MRBCdFill.fillAmount = remain / total;
    }
    public void HideMRBCooldown() => MRBPanel.SetActive(false);
}
