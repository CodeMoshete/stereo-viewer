using UnityEngine.UI;

public class UIFormSubmitButton : UIButton
{
    public Text InputField;

    public override void OnPressed()
    {
        EventData.StringData = InputField.text;
        base.OnPressed();
    }
}
