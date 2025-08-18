using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPickable : MonoBehaviour
{
    [HideInInspector] public RandomObjectSpawnerVR spawner;
    private bool collected = false;

    void OnEnable()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrab);
    }

    void OnDisable()
    {
        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        if (collected) return;

        collected = true;
        spawner.ObjectCollected(gameObject);
    }
}
