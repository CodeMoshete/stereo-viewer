using UnityEngine;

public class UIConsole : MonoBehaviour
{
    public GameObject DebugLineTemplate;

    void Start()
    {
        Service.EventManager.AddListener(EventId.DebugLog, OnDebugLog);
    }

    private bool OnDebugLog(object cookie)
    {
        GameObject debugLineObj = GameObject.Instantiate(DebugLineTemplate, transform);
        DebugLine debugLine = debugLineObj.GetComponent<DebugLine>();
        debugLine.Initialize((string)cookie);
        return false;
    }
}
