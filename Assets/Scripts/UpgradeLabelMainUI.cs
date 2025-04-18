using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UpgradeLabelMainUI : MonoBehaviour
{
    // The TextMeshPro component of the label
    private TextMeshProUGUI tmpText;
    private GameObject LineBL;
    private GameObject LineTL;
    private GameObject LineTR;
    private GameObject LineBR;
    private GameObject HoverSquare;
    
    // Text to display within UI label
    [SerializeField]
    private string labelTextHeader = "Upgrade Info"; // Header for the upgrade info panel
    [SerializeField]
    private string labelTextHotkeyInfoColor = "#87CEEB"; // Pale, electric blue, used for the "BUY" and "LEVEL UP" text.  
    [SerializeField]
    private int labelTextUpgradeNameCost = 0; // How much the upgrade costs
    [SerializeField]
    private string labelTextUpgradeNameColor = "#FFDF00"; // Gold yellow
    [SerializeField]
    private string labelTextUpgradeName = "Default Name"; // The name of the upgrade
    [SerializeField]
    private string labelTextUpgradeDesc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla facilisi. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Donec eget justo at ligula vehicula tincidunt. Sed auctor, velit nec efficitur, nunc sapien"; // Description of the upgrade


    // RectTransform of the label's main panel
    private RectTransform labelRectTransform;
    private Vector3[] corners = new Vector3[4];
    [System.NonSerialized]
    public Vector3[] upgradeIconCorners = new Vector3[4];
    public void Initialize()
    {
        labelRectTransform = gameObject.GetComponent<RectTransform>();
        labelRectTransform.Find("Panel").GetComponent<RectTransform>().GetWorldCorners(corners);
        tmpText = transform.Find("Panel").gameObject.transform.Find("TMP").gameObject.GetComponent<TextMeshProUGUI>();
        LineBL = gameObject.transform.parent.Find("LineBL").gameObject;
        LineTL = gameObject.transform.parent.Find("LineTL").gameObject;
        LineTR = gameObject.transform.parent.Find("LineTR").gameObject;
        LineBR = gameObject.transform.parent.Find("LineBR").gameObject;
        HoverSquare = gameObject.transform.parent.Find("HoverSquare").gameObject;
        gameObject.SetActive(false);
        LineBL.SetActive(false);
        LineTL.SetActive(false);
        LineTR.SetActive(false);
        LineBR.SetActive(false);
        HoverSquare.SetActive(false);
    }

    // Force update/reload the canvases and TMP object to ensure the TMP object assigns its preferred height
    // Then use that height to set the height of the canvas
    public void UpdateSize()
    {
        RectTransform tmpTextRect = tmpText.GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tmpTextRect);
        labelRectTransform.sizeDelta = new Vector2(labelRectTransform.sizeDelta.x, tmpTextRect.rect.height);
        DrawLines();
    }

    public string MakeFullFormattedString(Upgrade upgrade) {
        labelTextUpgradeNameCost = upgrade.Cost;
        labelTextUpgradeName = upgrade.Name;
        labelTextUpgradeDesc = upgrade.Desc;
        string text = string.Empty;
        text += "<line-height=90%><size=110%><color=" + labelTextHotkeyInfoColor + ">" + labelTextHeader + "</size></color>\n";
        text += "<line-height=125%><size=75%><color=" + labelTextUpgradeNameColor + "> Cost: " + labelTextUpgradeNameCost.ToString() + " Chips</color></size>\n";
        text += "<line-height=95%><size=100%>" + labelTextUpgradeName + "</size>\n";
        text += "<i><size=75%>" + labelTextUpgradeDesc + "</size></i>";
        tmpText.text = text;
        UpdateSize();
        return text;
    }

    public void DrawLines() {
        labelRectTransform.Find("Panel").GetComponent<RectTransform>().GetWorldCorners(corners);
        LineBL.GetComponent<ScreenSpaceLine>().fromPos = new Vector2(corners[0][0] + 3, corners[0][1] + 3);
        LineTL.GetComponent<ScreenSpaceLine>().fromPos = new Vector2(corners[1][0] + 3, corners[1][1] - 3);
        LineTR.GetComponent<ScreenSpaceLine>().fromPos = new Vector2(corners[2][0] - 3, corners[2][1] - 3);
        LineBR.GetComponent<ScreenSpaceLine>().fromPos = new Vector2(corners[3][0] - 3, corners[3][1] + 3);
        LineBL.GetComponent<ScreenSpaceLine>().toPos = new Vector2(upgradeIconCorners[0][0], upgradeIconCorners[0][1]);
        LineTL.GetComponent<ScreenSpaceLine>().toPos = new Vector2(upgradeIconCorners[1][0], upgradeIconCorners[1][1]);
        LineTR.GetComponent<ScreenSpaceLine>().toPos = new Vector2(upgradeIconCorners[2][0], upgradeIconCorners[2][1]);
        LineBR.GetComponent<ScreenSpaceLine>().toPos = new Vector2(upgradeIconCorners[3][0], upgradeIconCorners[3][1]);
        HoverSquare.GetComponent<RectTransform>().anchoredPosition = new Vector2((upgradeIconCorners[1][0] + upgradeIconCorners[2][0]) / 2, (upgradeIconCorners[1][1] + upgradeIconCorners[0][1]) / 2);
        HoverSquare.GetComponent<RectTransform>().sizeDelta = new Vector2(upgradeIconCorners[2][0] - upgradeIconCorners[1][0], upgradeIconCorners[1][1] - upgradeIconCorners[0][1]);
    }
}
