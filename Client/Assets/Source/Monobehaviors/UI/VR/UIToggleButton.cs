using UnityEngine;

public class UIToggleButton : UIButton
{
    public override void OnPressed()
    {
        isPressing = !isPressing;
        EventData.BoolData = isPressing;
        Service.EventManager.SendEvent(PressEvent, EventData);
        transform.localPosition = isPressing ? downPosition : upPosition;
        buttonMaterial.SetColor("_EmissionColor", isPressing ? DOWN_COLOR : UP_COLOR);
    }
}
