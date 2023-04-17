using UnityEngine;
using UnityEngine.UI;

public class UIControlPanel : MonoBehaviour
{
    private const string KEYBOARD_PREFAB = "VRKeyboard";

    public Transform KeyboardAnchor;
    public Text PlayerNameField;
    public EmotesPanel EmotesContainer;
    public UIButton EmotesButton;

    public Text DivergenceValueField;
    public Text DistanceValueField;

    private GameObject keyboard;
    private bool isShiftToggled;
    private bool isEditingName;

    private int currentDivergence;
    private int currentDistance;

    void Start()
    {
        //Service.EventManager.AddListener(EventId.EditPlayerName, ShowKeyboard);
        //Service.EventManager.AddListener(EventId.SubmitPlayerName, SubmitPlayerName);
        //Service.EventManager.AddListener(EventId.KeyboardKeyPress, OnKeyboardKeyPress);

        Service.EventManager.AddListener(EventId.MoveScreenCloser, OnScreenMovedCloser);
        Service.EventManager.AddListener(EventId.MoveScreenFurther, OnScreenMovedFurther);
        Service.EventManager.AddListener(EventId.IncreaseScreenDivergence, OnDivergenceIncrease);
        Service.EventManager.AddListener(EventId.DecreaseScreenDivergence, OnDivergenceDecrease);
        Service.EventManager.AddListener(EventId.NewPhotoLoaded, ResetDivergenceAndDistance);
        Service.EventManager.AddListener(EventId.SetScreenDistance, SetScreenDistance);
        Service.EventManager.AddListener(EventId.SetScreenDivergence, SetScreenDivergence);

        //EmotesButton.PressCallback = ShowEmotesPanel;
    }

    private bool OnScreenMovedCloser(object cookie)
    {
        ++currentDistance;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private bool OnScreenMovedFurther(object cookie)
    {
        --currentDistance;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private bool OnDivergenceIncrease(object cookie)
    {
        ++currentDivergence;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private bool OnDivergenceDecrease(object cookie)
    {
        --currentDivergence;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private bool ResetDivergenceAndDistance(object cookie)
    {
        currentDivergence = 0;
        currentDistance = 0;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private bool SetScreenDistance(object cookie)
    {
        currentDistance = (int)cookie;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private bool SetScreenDivergence(object cookie)
    {
        currentDivergence = (int)cookie;
        UpdateDivergenceAndDistanceFields();
        return false;
    }

    private void UpdateDivergenceAndDistanceFields()
    {
        DivergenceValueField.text = currentDivergence.ToString();
        DistanceValueField.text = currentDistance.ToString();
    }

    private void ShowEmotesPanel()
    {
        EmotesContainer.ShowPanel(true);
    }

    private bool ShowKeyboard(object cookie)
    {
        if (!isEditingName)
        {
            isEditingName = true;
            keyboard = GameObject.Instantiate(Resources.Load<GameObject>(KEYBOARD_PREFAB));
            keyboard.transform.position = KeyboardAnchor.position;
            PlayerNameField.text = string.Empty;
        }
        return true;
    }

    private bool SubmitPlayerName(object cookie)
    {
        if (isEditingName)
        {
            isEditingName = false;
            GameObject.Destroy(keyboard);
        }
        return false;
    }

    private bool OnKeyboardKeyPress(object cookie)
    {
        ButtonEventData eventData = (ButtonEventData)cookie;
        string key = eventData.StringData;
        if (key == "space")
        {
            PlayerNameField.text += " ";
        }
        else if (key == "delete")
        {
            PlayerNameField.text = PlayerNameField.text.Remove(PlayerNameField.text.Length - 1);
        }
        else if (key == "shift")
        {
            isShiftToggled = eventData.BoolData;
        }
        else
        {
            PlayerNameField.text += isShiftToggled ? key.ToUpper() : key;
        }
        return false;
    }
}
