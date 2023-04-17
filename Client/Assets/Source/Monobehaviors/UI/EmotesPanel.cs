using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonContainer
{
    public string[] ResourceNames;
    public RandomMessageCollection Messages;
    private GameObject button;

    public ButtonContainer(Button sourceButton)
    {
        button = sourceButton.gameObject;
        sourceButton.onClick.AddListener(OnButtonClick);
    }

    public ButtonContainer(UIButton sourceButton)
    {
        button = sourceButton.gameObject;
        sourceButton.PressCallback = OnButtonClick;
    }
    
    public void SetActive(bool active)
    {
        button.gameObject.SetActive(active);
    }

    public void SetText(string text)
    {
        button.GetComponentInChildren<Text>().text = text;
    }

    private void OnButtonClick()
    {
        string message = Messages != null ? Messages.GetRandomMessage() : string.Empty;
        PlayEmoteEvent eventData = new PlayEmoteEvent(ResourceNames, message);
        Service.EventManager.SendEvent(EventId.PlayEmote, eventData);
    }
}

public class EmotesPanel : MonoBehaviour
{
    [SerializeField]
    public List<Button> EmoteButtons;

    [SerializeField]
    public List<UIButton> EmoteUiButtons;

    [SerializeField]
    public EmotesCollection EmotesCollection;

    public Button NextPageButton;
    public Button PrevPageButton;
    public Button CloseButton;

    public UIButton NextPageUiButton;
    public UIButton PrevPageUiButton;
    public UIButton CloseUiButton;

    private GameObject prevButtonObj;
    private GameObject nextButtonObj;

    private List<Emote> EmoteEvents;
    private List<ButtonContainer> buttonContainers;
    private int pageNum;
    private int numPages;
    private int numButtons;

    void Start()
    {
        EmoteEvents = EmotesCollection.Emotes;
        numButtons = Mathf.Max(EmoteButtons.Count, EmoteUiButtons.Count);
        numPages = Mathf.CeilToInt(EmoteEvents.Count / numButtons);
        buttonContainers = new List<ButtonContainer>();

        if (NextPageButton != null && PrevPageButton != null && CloseButton != null)
        {
            for (int i = 0; i < numButtons; ++i)
            {
                buttonContainers.Add(new ButtonContainer(EmoteButtons[i]));
            }

            prevButtonObj = PrevPageButton.gameObject;
            nextButtonObj = NextPageButton.gameObject;
            NextPageButton.onClick.AddListener(NextPage);
            PrevPageButton.onClick.AddListener(PrevPage);
            CloseButton.onClick.AddListener(() => { ShowPanel(false); });
        } 
        else if (NextPageUiButton != null && PrevPageUiButton != null && CloseUiButton != null)
        {
            for (int i = 0; i < numButtons; ++i)
            {
                buttonContainers.Add(new ButtonContainer(EmoteUiButtons[i]));
            }

            prevButtonObj = PrevPageUiButton.gameObject;
            nextButtonObj = NextPageUiButton.gameObject;
            NextPageUiButton.PressCallback = NextPage;
            PrevPageUiButton.PressCallback = PrevPage;
            CloseUiButton.PressCallback = () => { ShowPanel(false); };
        }

        Service.EventManager.AddListener(EventId.ToggleUI, OnUIToggled);

        SetPage(0);
    }

    private bool OnUIToggled(object cookie)
    {
#if UNITY_ANDROID
        bool isUiToggled = (bool)cookie;
        if (!isUiToggled)
        {
            ShowPanel(false);
        }
#endif
        return false;
    }

    private void NextPage()
    {
        SetPage(pageNum + 1);
    }

    private void PrevPage()
    {
        SetPage(pageNum - 1);
    }

    public void ShowPanel(bool show)
    {
        gameObject.SetActive(show);
    }

    private void SetPage(int pageNum)
    {
        for (int i = 0; i < numButtons; ++i)
        {
            buttonContainers[i].SetActive(false);
        }

        int startIndex = pageNum * buttonContainers.Count;
        int emotesToShow = Mathf.Min(numButtons, EmoteEvents.Count - startIndex);
        for (int i = 0; i < emotesToShow; ++i)
        {
            ButtonContainer button = buttonContainers[i];
            Emote emote = EmoteEvents[startIndex + i];
            button.ResourceNames = emote.EmoteResources;
            button.Messages = emote.Messages;
            button.SetText(emote.ButtonText);
            buttonContainers[i].SetActive(true);
        }

        this.pageNum = pageNum;
        prevButtonObj.SetActive(pageNum > 0);
        nextButtonObj.SetActive(pageNum < numPages);
    }
}
