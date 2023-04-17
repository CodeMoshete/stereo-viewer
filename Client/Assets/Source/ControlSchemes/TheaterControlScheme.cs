using UnityEngine;
using Utils;

public class TheaterControlScheme : IControlScheme
{
    private OVRCameraRig headObject;
    private Transform cameraObject;
    private OVRScreenFade screenFader;
    private Transform rightController;
    private Vector3 lastMousePos;

    private float sensitivity;
    private bool isDebugMenuActive;

    private Transform camController;
    private Transform camRig;
    private Transform cameraEye;

    private bool questClick;

    public void Initialize(OVRCameraRig body, Transform camera, float sensitivity)
    {
        headObject = body;
        cameraObject = camera;
        camController = camera.parent;
        this.sensitivity = sensitivity;

        GameObject centerEye = UnityUtils.FindGameObject(cameraObject.gameObject, "CenterEyeAnchor");
        screenFader = centerEye.GetComponent<OVRScreenFade>();
        cameraEye = centerEye.transform;

        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);
        Service.Controls.SetTriggerObserver(TriggerUpdate);
    }

    public void Deactivate()
    {
        Service.Controls.RemoveTouchObserver(TouchUpdate);
        Service.Controls.RemoveBackButtonObserver(BackUpdate);
        Service.Controls.RemoveTriggerObserver(TriggerUpdate);
    }

    public void SetMovementEnabled(bool enabled)
    {
        Debug.Log("Controls active: " + enabled);
    }

    private void TriggerUpdate(TriggerUpdate update)
    {
        if (update.TriggerClicked)
        {
            Service.EventManager.SendEvent(EventId.PlayPause, null);
        }
    }

    private void BackUpdate(BackButtonUpdate update)
    {
        if (update.BackButtonClicked)
        {
            isDebugMenuActive = !isDebugMenuActive;
            Service.EventManager.SendEvent(EventId.DebugToggleConsole, isDebugMenuActive);
            Service.EventManager.SendEvent(EventId.ToggleUI, true);
        }
    }

    private void TouchUpdate(TouchpadUpdate update)
    {
#if UNITY_EDITOR
        Vector3 euler = headObject.transform.eulerAngles;
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            float delta = Time.deltaTime;
            Vector3 mouseDelta = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            euler = headObject.transform.eulerAngles;
            euler.y += delta * -((mouseDelta.x / Screen.width) * sensitivity);
            headObject.transform.eulerAngles = euler;

            euler = cameraObject.eulerAngles;
            euler.x += delta * (mouseDelta.y / Screen.height) * sensitivity;
            cameraObject.eulerAngles = euler;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Service.EventManager.SendEvent(EventId.ToggleUI, true);
        }
#endif
        float dt = Time.deltaTime;

        bool isPressed = update.TouchpadPressState;
        bool isPressedThisFrame = update.TouchpadClicked;

        if (ControlsManager.Instance.CurrentHeadset == HeadsetModel.OculusQuest)
        {
            OVRInput.Controller activeController = OVRInput.GetActiveController();
        }

        if (Service.Controls.CurrentHeadset == HeadsetModel.OculusQuest)
        {
            if (!questClick)
            {
                if (update.TouchpadPosition.x > 0.9f)
                {
                    questClick = true;
                    Service.EventManager.SendEvent(EventId.FastForward, null);
                }
                else if (update.TouchpadPosition.x < -0.9f)
                {
                    questClick = true;
                    Service.EventManager.SendEvent(EventId.Rewind, null);
                }
                else if (update.TouchpadPosition.y < -0.9f)
                {
                    questClick = true;
                    Service.EventManager.SendEvent(EventId.VolumeDown, null);
                }
                else if (update.TouchpadPosition.y > 0.9f)
                {
                    questClick = true;
                    Service.EventManager.SendEvent(EventId.VolumeUp, null);
                }
            }

            if (update.TouchpadPosition.x < 0.5f && update.TouchpadPosition.x > -0.5f &&
                update.TouchpadPosition.y < 0.5f && update.TouchpadPosition.y > -0.5f)
            {
                questClick = false;
            }
        }
        else
        {
            if (update.TouchpadClicked)
            {
                if (update.TouchpadPosition.x >= 0.5)
                {
                    Service.EventManager.SendEvent(EventId.FastForward, null);
                }
                else if (update.TouchpadPosition.x <= -0.5)
                {
                    Service.EventManager.SendEvent(EventId.Rewind, null);
                }
                else if (update.TouchpadPosition.y <= -0.5)
                {
                    Service.EventManager.SendEvent(EventId.VolumeDown, null);
                }
                else if (update.TouchpadPosition.y >= 0.5)
                {
                    Service.EventManager.SendEvent(EventId.VolumeUp, null);
                }
                else
                {
                    Service.EventManager.SendEvent(EventId.ToggleUI, true);
                }
            }
        }
    }
}
