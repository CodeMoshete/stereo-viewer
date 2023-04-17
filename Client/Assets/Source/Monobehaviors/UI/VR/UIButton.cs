using System;
using UnityEngine;

[System.Serializable]
public struct ButtonEventData
{
    public bool BoolData;
    public int IntData;
    public float FloatData;
    public string StringData;
}

public class UIButton : MonoBehaviour
{
    protected const float BUTTON_DOWN_POS = 0f;
    protected readonly Color UP_COLOR = new Color(0.63f, 0.63f, 0.63f, 1f);
    protected readonly Color HOVER_COLOR = new Color(0.8f, 0.8f, 0.8f, 1f);
    protected readonly Color DOWN_COLOR = new Color(0.8f, 1f, 0.8f, 1f);

    public EventId PressEvent;
    public ButtonEventData EventData;
    public Action PressCallback;

    protected Material buttonMaterial;
    protected Vector3 upPosition;
    protected Vector3 downPosition;
    protected bool isHovering;
    protected bool isPressing;

    public void Start()
    {
        buttonMaterial = GetComponent<Renderer>().material;
        upPosition = transform.localPosition;
        downPosition = new Vector3(upPosition.x, upPosition.y, BUTTON_DOWN_POS);
    }

    public void OnHover(bool isHovering)
    {
        this.isHovering = isHovering;

        if (!isPressing)
        {
            Color hoverColor = isHovering ? HOVER_COLOR : UP_COLOR;
            buttonMaterial.SetColor("_EmissionColor", hoverColor);
        }
    }
    
    public virtual void OnPressed()
    {
        if (!isPressing)
        {
            Service.TimerManager.CreateTimer(0.2f, OnPressDone, null);
            isPressing = true;
            transform.localPosition = downPosition;
            buttonMaterial.SetColor("_EmissionColor", DOWN_COLOR);

            if (PressCallback != null)
            {
                PressCallback();
            }
            else
            {
                Service.EventManager.SendEvent(PressEvent, EventData);
            }
        }
    }

    protected virtual void OnPressDone(object cookie)
    {
        isPressing = false;
        transform.localPosition = upPosition;

        if (isHovering)
        {
            Color hoverColor = isHovering ? HOVER_COLOR : UP_COLOR;
            buttonMaterial.SetColor("_EmissionColor", hoverColor);
        }
    }
}
