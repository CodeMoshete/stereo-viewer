using UnityEngine;

public interface IControlScheme
{
    void Initialize(OVRCameraRig body, Transform camera, float sensitivity);
    void SetMovementEnabled(bool enabled);
    void Deactivate();
}
