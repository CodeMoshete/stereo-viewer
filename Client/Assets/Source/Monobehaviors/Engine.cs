using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Engine : MonoBehaviour
{
    private const float PLAYER_POS_OFFSET = 1f;
    public const float SCREEN_MOVE_AMT = 1f;
    public const float DIVERGENCE_MOVE_AMT = 0.1f;

    public OVRCameraRig CameraRig;
    public float TestSensitivity = 10f;

    public GameObject VrPrefabs;
    public GameObject UiControlPanel;
    public Transform Body;
    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;

    public Transform ScreenLeft;
    public Material LeftMaterial;
    public Transform ScreenRight;
    public Material RightMaterial;

    public TextAsset LocalManifest;
    public DownloadManager DownloadManager;
    private ImageLoader imageLoader;

    private IControlScheme currentControlScheme;

    void Start()
    {
        VrPrefabs.SetActive(true);
        Service.EventManager.AddListener(EventId.MoveScreenCloser, MoveScreenCloser);
        Service.EventManager.AddListener(EventId.MoveScreenFurther, MoveScreenFurther);
        Service.EventManager.AddListener(EventId.IncreaseScreenDivergence, IncreaseScreenDivergence);
        Service.EventManager.AddListener(EventId.DecreaseScreenDivergence, DecreaseScreenDivergence);
        Service.EventManager.AddListener(EventId.SetScreenDistance, SetScreenDistance);
        Service.EventManager.AddListener(EventId.SetScreenDivergence, SetScreenDivergence);

        currentControlScheme = new TheaterControlScheme();
        currentControlScheme.Initialize(CameraRig, CameraRig.transform, TestSensitivity);

        Body.transform.Translate(Vector3.zero);
        Service.EventManager.AddListener(EventId.ToggleUI, OnUIToggled);
        Service.EventManager.AddListener(EventId.ToggleSubs, OnVRSubsToggled);
        UiControlPanel.transform.Translate(Vector3.zero);

        imageLoader = new ImageLoader(
            LeftMaterial, RightMaterial, ScreenLeft, ScreenRight, DownloadManager, LocalManifest);
    }

    private bool OnUIToggled(object cookie)
    {
        bool isUiToggled = (bool)cookie;
        UiControlPanel.SetActive(isUiToggled);
        currentControlScheme.Deactivate();

        if (isUiToggled)
        {
            currentControlScheme = new UIControlScheme();
        }
        else
        {
            currentControlScheme = new TheaterControlScheme();
        }
        currentControlScheme.Initialize(CameraRig, CameraRig.transform, TestSensitivity);
        return false;
    }

    private bool OnVRSubsToggled(object cookie)
    {
        return true;
    }

    private bool OnPCSubsToggled(object cookie)
    {
        return true;
    }

    private bool MoveScreenCloser(object cookie)
    {
        ScreenLeft.Translate(new Vector3(0f, 0f, -SCREEN_MOVE_AMT));
        ScreenRight.Translate(new Vector3(0f, 0f, -SCREEN_MOVE_AMT));
        return false;
    }

    private bool MoveScreenFurther(object cookie)
    {
        ScreenLeft.Translate(new Vector3(0f, 0f, SCREEN_MOVE_AMT));
        ScreenRight.Translate(new Vector3(0f, 0f, SCREEN_MOVE_AMT));
        return false;
    }

    private bool IncreaseScreenDivergence(object cookie)
    {
        ScreenLeft.Translate(new Vector3(-DIVERGENCE_MOVE_AMT, 0f, 0f));
        ScreenRight.Translate(new Vector3(DIVERGENCE_MOVE_AMT, 0f, 0f));
        return false;
    }

    private bool DecreaseScreenDivergence(object cookie)
    {
        ScreenLeft.Translate(new Vector3(DIVERGENCE_MOVE_AMT, 0f, 0f));
        ScreenRight.Translate(new Vector3(-DIVERGENCE_MOVE_AMT, 0f, 0f));
        return false;
    }

    private bool SetScreenDistance(object cookie)
    {
        float newDistance = (float)cookie * SCREEN_MOVE_AMT;
        ScreenLeft.position = new Vector3(ScreenLeft.position.x, ScreenLeft.position.y, newDistance);
        ScreenRight.position = new Vector3(ScreenRight.position.x, ScreenRight.position.y, newDistance);
        return false;
    }

    private bool SetScreenDivergence(object cookie)
    {
        float newDivergence = (float)cookie * DIVERGENCE_MOVE_AMT;
        ScreenLeft.position = new Vector3(-newDivergence, ScreenLeft.position.y, ScreenLeft.position.z);
        ScreenRight.position = new Vector3(newDivergence, ScreenRight.position.y, ScreenRight.position.z);
        return false;
    }
}
