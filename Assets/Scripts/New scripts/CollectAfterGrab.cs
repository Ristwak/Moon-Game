using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class CollectAfterGrab : MonoBehaviour
{
    [SerializeField] float collectDelay = 1.5f;   // seconds after release
    [SerializeField] bool destroyAfter = true;    // false => just SetActive(false)
    [SerializeField] string itemId = "item";      // optional label

    XRGrabInteractable grab;
    bool collecting;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectExited.AddListener(OnReleased);  // <-- trigger collection AFTER release
    }

    void OnDestroy()
    {
        if (grab != null) grab.selectExited.RemoveListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (collecting || (GameManager.Instance && !GameManager.Instance.GameRunning)) return;
        collecting = true;
        StartCoroutine(CollectRoutine());
    }

    IEnumerator CollectRoutine()
    {
        yield return new WaitForSeconds(collectDelay);

        // Tell GameManager we collected one
        if (GameManager.Instance) GameManager.Instance.AddCollected(itemId);

        // Lock physics
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // Disable grabbing
        grab.enabled = false;

        // Hide visuals and colliders
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;

        // Remove object
        if (destroyAfter) Destroy(gameObject, 0.05f);
        else gameObject.SetActive(false);
    }
}
