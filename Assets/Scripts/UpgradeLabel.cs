using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using System;

public class UpgradeLabel : MonoBehaviour
{
    // The TextMeshPro component of the label
    private TextMeshProUGUI tmpText;

    // Text to display within UI label
    [SerializeField]
    private string labelTextHotkeyInfo = "(E) Buy"; // Displays when player's ugprade slots are not full and the player does not currently hold the labeled upgrade
    [SerializeField]
    private string labelTextHotkeyInfoSoldOut = "(E) Replace"; // Displays when player's upgrade slots are full and the player does not currently hold the labeled upgrade
    [SerializeField]
    private string labelTextHotkeyInfoLevelUp = "(E) Level Up"; // Displays when the player currently holds the labeled upgrade REGARDLESS of whether the slots are full
    [SerializeField]
    private string labelTextHotkeyInfoInsufficientFunds = "Not Enough Chips";
    [SerializeField]
    private string labelTextHotkeyInfoTrashcan = "(E) Recycle\n(MB1) Select"; // Displays when the player is looking at the trashcan
    [SerializeField]
    private string labelTextHotkeyInfoColor = "#87CEEB"; // Pale, electric blue, used for the "BUY" and "LEVEL UP" text.
    [SerializeField]
    private string labelTextHotkeyInfoSoldOutColor = "#FF2800"; // Bright ish red, used for the "Not enough chips, Replace, and Recycle" text.
    private string activeLabelTextHotkeyInfo = "(E) Buy"; // The currently displayed hotkey info text
    private string activeLabelTextHotkeyInfoColor = "#87CEEB"; // The currently displayed hotkey info text color
    [SerializeField]
    private int labelTextItemCost = 0; // How much the upgrade costs
    [SerializeField]
    private string labelTextItemCostColor = "#FFDF00"; // Gold yellow
    [SerializeField]
    private string labelTextItemName = "Default Name"; // The name of the upgrade
    [SerializeField]
    private string labelTextItemDesc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla facilisi. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Donec eget justo at ligula vehicula tincidunt. Sed auctor, velit nec efficitur, nunc sapien"; // Description of the upgrade

    // RectTransform of the label's main panel
    private RectTransform labelRectTransform;

    // LineRenderers for the lines that connect to each corner of the label
    // lrBl = "LineRenderer Bottom Left Corner", etc.
    private LineRenderer lrBl;
    private LineRenderer lrTl;
    private LineRenderer lrTr;
    private LineRenderer lrBr;

    // Positions of the corners of the RectTransform of the label's main panel
    private Vector3 labelBottomLeft;
    private Vector3 labelTopLeft;
    private Vector3 labelTopRight;
    private Vector3 labelBottomRight;

    public bool IsTrashcan { get; set; } = false; // Whether this label is for a trashcan or not

    public void Initialize(Upgrade upgrade) 
    {
        // If we are given an upgrade object, then use that object's Name and Description for the label
        if (upgrade != null)
        {
            labelTextItemName = upgrade.Name;
            labelTextItemDesc = upgrade.Desc;
            labelTextItemCost = upgrade.Cost;
        }
        // Instantiate the UI label with some text, rotation, and position
        activeLabelTextHotkeyInfo = labelTextHotkeyInfo;
        activeLabelTextHotkeyInfoColor = labelTextHotkeyInfoColor;

        // Assign the positions of the label's corners
        Vector3[] corners = new Vector3[4];
        labelRectTransform = (RectTransform)transform.Find("ItemLabelPanel");
        labelRectTransform.GetWorldCorners(corners);
        labelBottomLeft = corners[0];
        labelTopLeft = corners[1];
        labelTopRight = corners[2];
        labelBottomRight = corners[3];

        // Assign the LineRenderers for the lines to the corners of the label, and toggle off rendering for now
        lrBl = gameObject.transform.Find("LineRendererBottomLeft").GetComponent<LineRenderer>();
        lrTl = gameObject.transform.Find("LineRendererTopLeft").GetComponent<LineRenderer>();
        lrTr = gameObject.transform.Find("LineRendererTopRight").GetComponent<LineRenderer>();
        lrBr = gameObject.transform.Find("LineRendererBottomRight").GetComponent<LineRenderer>();

        lrBl.enabled = false;
        lrTl.enabled = false;
        lrTr.enabled = false;
        lrBr.enabled = false;

        tmpText = transform.Find("ItemLabelPanel/TMP").gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Either draws the lines to the-corners-of-the-label from the-center-point-of-the-top-face-of-this-ShopItem
    // Or makes them invisible, depending on whether the shop is active.
    public void DrawLinesToLabelCorners(bool isRenderingActive, Vector3 bottomLeftLineEndpoint, Vector3 topLeftLineEndpoint, Vector3 topRightLineEndpoint, Vector3 bottomRightLineEndpoint)
    {
        if (isRenderingActive)
        {
            lrBl.enabled = true;
            lrTl.enabled = true;
            lrTr.enabled = true;
            lrBr.enabled = true;

            lrBl.SetPosition(1, bottomLeftLineEndpoint);
            lrBl.SetPosition(0, labelBottomLeft);

            lrTl.SetPosition(1, topLeftLineEndpoint);
            lrTl.SetPosition(0, labelTopLeft);

            lrTr.SetPosition(1, topRightLineEndpoint);
            lrTr.SetPosition(0, labelTopRight);

            lrBr.SetPosition(1, bottomRightLineEndpoint);
            lrBr.SetPosition(0, labelBottomRight);
        }
        else
        {
            if (lrBl != null && lrTl != null && lrTr != null && lrBr != null)
            {
                lrBl.enabled = false;
                lrTl.enabled = false;
                lrTr.enabled = false;
                lrBr.enabled = false;
            }
        }

    }

    private void RecalculateCorners()
    {
        if(labelRectTransform != null)
        {
            Vector3[] corners = new Vector3[4];
            labelRectTransform.GetWorldCorners(corners);
            labelBottomLeft = corners[0];
            labelTopLeft = corners[1];
            labelTopRight = corners[2];
            labelBottomRight = corners[3];
        }
    }

    // Force update/reload the canvases and TMP object to ensure the TMP object assigns its preferred height
    // Then use that height to set the height of the canvas
    public void UpdateSize() 
    {
        RectTransform tmpText = (RectTransform)transform.Find("ItemLabelPanel").Find("TMP");
        RectTransform canvasRect = GetComponent<RectTransform>();
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(tmpText);
        canvasRect.sizeDelta = new Vector2(canvasRect.sizeDelta.x, tmpText.rect.height);
        RecalculateCorners();
    }

    public string MakeFullFormattedTextString(GameObject label, bool isHeaderDisplayed)
    {
        string text = string.Empty;
        if (isHeaderDisplayed)
        {
            if (!IsTrashcan) {
                text += "<line-height=90%><size=110%><color=" + activeLabelTextHotkeyInfoColor + ">" + activeLabelTextHotkeyInfo + "</size></color>\n";
                text += "<line-height=125%><size=75%><color=" + labelTextItemCostColor + "> Cost: " + labelTextItemCost.ToString() + " Chips</color></size>\n"; 
            } else {
                text += "<line-height=125%><size=110%><color=" + activeLabelTextHotkeyInfoColor + ">" + activeLabelTextHotkeyInfo + "</size></color>\n";
            }
        }
        text += "<line-height=95%><size=100%>" + labelTextItemName + "</size>\n";
        text += "<i><size=75%>" + labelTextItemDesc + "</size></i>";
        tmpText.text = text;
        if (label != null)
        {
            label.GetComponent<UpgradeLabel>().UpdateSize();
        }
        return text;
    }

    public void ChangeLabelTextBasedOnGameState(Upgrade upgrade) {
        if (!IsTrashcan) {
            // If player has enough currency to buy the upgrade...
            if (upgrade.Cost <= Level_manager.instance.Currency)
            {
                // If the player does not currently hold this upgrade, then...
                if (Array.IndexOf(Level_manager.instance.PlayerHeldUpgradeIds, upgrade.Id) <= -1)
                {
                    // If the player is at or exceeding the upgrade slot limit
                    if (Level_manager.instance.PlayerHeldUpgrades.Count >= Level_manager.MAX_PLAYER_UPGRADES)
                    {
                        // Set the text to indicate the slots are full, set the text color to match
                        activeLabelTextHotkeyInfoColor = labelTextHotkeyInfoSoldOutColor;
                        activeLabelTextHotkeyInfo = labelTextHotkeyInfoSoldOut;
                    }
                    else
                    {
                        // Set the text to inform the player of the hotkey and the action ("(E) Buy"), set the color to match
                        activeLabelTextHotkeyInfoColor = labelTextHotkeyInfoColor;
                        activeLabelTextHotkeyInfo = labelTextHotkeyInfo;
                    }
                }
                // Otherwise (if the player currently holds this upgrade), then...
                else
                {
                    // Set the text to inform the player of the hotkey and the action ("(E) Level Up"), set color to match.
                    activeLabelTextHotkeyInfoColor = labelTextHotkeyInfoColor;
                    activeLabelTextHotkeyInfo = labelTextHotkeyInfoLevelUp;
                }
            }
            // Otherwise, set the label's text to reflect that the player does not have enough currency.
            else
            {
                activeLabelTextHotkeyInfoColor = labelTextHotkeyInfoSoldOutColor;
                activeLabelTextHotkeyInfo = labelTextHotkeyInfoInsufficientFunds;
            }
        } else
        {
            activeLabelTextHotkeyInfoColor = labelTextHotkeyInfoSoldOutColor;
            activeLabelTextHotkeyInfo = labelTextHotkeyInfoTrashcan;
        }
    }

    public void SetNewUpgrade(Upgrade upgrade) { 
        if (upgrade != null)
        {
            labelTextItemName = upgrade.Name;
            labelTextItemDesc = upgrade.Desc;
            labelTextItemCost = upgrade.Cost;
        }
    }
    public string LabelTextHotkeyInfo
    {
        get { return labelTextHotkeyInfo; }
        set { labelTextHotkeyInfo = value; }
    }

    public string LabelTextHotkeyInfoSoldOut
    {
        get { return labelTextHotkeyInfoSoldOut; }
        set { labelTextHotkeyInfoSoldOut = value; }
    }

    public string LabelTextHotkeyInfoLevelUp
    {
        get { return labelTextHotkeyInfoLevelUp; }
        set { labelTextHotkeyInfoLevelUp = value; }
    }

    public string LabelTextHotkeyInfoColor
    {
        get { return labelTextHotkeyInfoColor; }
        set { labelTextHotkeyInfoColor = value; }
    }

    public string LabelTextHotkeyInfoSoldOutColor
    {
        get { return labelTextHotkeyInfoSoldOutColor; }
        set { labelTextHotkeyInfoSoldOutColor = value; }
    }

    public string ActiveLabelTextHotkeyInfo
    {
        get { return activeLabelTextHotkeyInfo; }
        set { activeLabelTextHotkeyInfo = value; }
    }

    public string ActiveLabelTextHotkeyInfoColor
    {
        get { return activeLabelTextHotkeyInfoColor; }
        set { activeLabelTextHotkeyInfoColor = value; }
    }

    public TextMeshProUGUI TmpText
    {
        get { return tmpText; }
        set { tmpText = value; }
    }
}
