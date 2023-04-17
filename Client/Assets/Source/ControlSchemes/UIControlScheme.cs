using UnityEngine;
using Utils;

public class UIControlScheme : IControlScheme
{
    private const float RAYCAST_DIST = 200f;
    private const string LAYER_UI_BUTTON = "UIButton";
    private const string RIGHT_CONTROLLER_NAME = "RightControllerAnchor";
    private const string LASER_PREFAB = "LaserContainer";

    private int raycastLayerMask;
    private Transform rightController;
    private Transform centerEye;
    private UIButton hoverButton;
    private GameObject laser;

#if UNITY_EDITOR
    private Vector3 lastMousePos;
    private float sensitivity;
    private Transform headObject;
    private Transform cameraObject;
#endif

    public void Initialize(OVRCameraRig body, Transform camera, float sensitivity)
    {
        raycastLayerMask = LayerMask.GetMask(LAYER_UI_BUTTON);
        rightController = UnityUtils.FindGameObject(body.gameObject, RIGHT_CONTROLLER_NAME).transform;
        centerEye = UnityUtils.FindGameObject(camera.gameObject, "CenterEyeAnchor").transform;
        laser = GameObject.Instantiate(Resources.Load<GameObject>(LASER_PREFAB), rightController);

        Service.UpdateManager.AddObserver(Update);
        Service.Controls.SetTriggerObserver(OnTriggerUpdate);
        Service.Controls.SetTouchObserver(TouchUpdate);
        Service.Controls.SetBackButtonObserver(BackUpdate);
#if UNITY_EDITOR
        headObject = body.transform;
        cameraObject = camera;
        this.sensitivity = sensitivity;
#endif
    }

    public void SetMovementEnabled(bool enabled)
    {
        // Intentionally empty.
    }

    public void Deactivate()
    {
        Service.UpdateManager.RemoveObserver(Update);
        Service.Controls.SetTriggerObserver(OnTriggerUpdate);
        Service.Controls.RemoveTouchObserver(TouchUpdate);
        Service.Controls.RemoveBackButtonObserver(BackUpdate);
        GameObject.Destroy(laser);
    }

    private void BackUpdate(BackButtonUpdate update)
    {
        if (update.BackButtonClicked)
        {
            Service.EventManager.SendEvent(EventId.ToggleUI, false);
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
            Service.EventManager.SendEvent(EventId.ToggleUI, false);
        }
#endif

        if (update.TouchpadClicked && 
            Mathf.Abs(update.TouchpadPosition.x) < 0.5f && 
            Mathf.Abs(update.TouchpadPosition.y) < 0.5f)
        {
            Service.EventManager.SendEvent(EventId.ToggleUI, false);
        }
    }

    private void Update(float dt)
    {
        Ray uiTestRay;
#if UNITY_EDITOR
        uiTestRay = new Ray(centerEye.position, centerEye.forward);
#else
        uiTestRay = new Ray(rightController.position, rightController.forward);
# endif
        RaycastHit uiHit;
        if (Physics.Raycast(uiTestRay, out uiHit, RAYCAST_DIST, raycastLayerMask))
        {
            UIButton currentButton = uiHit.collider.gameObject.GetComponent<UIButton>();
            if (currentButton != hoverButton)
            {
                currentButton.OnHover(true);
                if (hoverButton != null)
                {
                    hoverButton.OnHover(false);
                }
            }
            hoverButton = currentButton;
        }
        else if(hoverButton != null)
        {
            hoverButton.OnHover(false);
            hoverButton = null;
        }
    }

    public void OnTriggerUpdate(TriggerUpdate update)
    {
        if (update.TriggerClicked && hoverButton != null)
        {
            hoverButton.OnPressed();
        }
    }
}
