using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class CollectAfterGrab : MonoBehaviour
{
    [SerializeField] float collectDelay = 1.5f;   // seconds after grab
    [SerializeField] bool destroyAfter = true;    // false => just SetActive(false)
    [SerializeField] string itemId = "item";      // optional label

    XRGrabInteractable grab;
    bool collecting;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnGrabbed);
    }

    void OnDestroy()
    {
        if (grab != null) grab.selectEntered.RemoveListener(OnGrabbed);
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (collecting || (GameManager.Instance && !GameManager.Instance.GameRunning)) return;
        collecting = true;
        StartCoroutine(CollectRoutine(args));
    }

    IEnumerator CollectRoutine(SelectEnterEventArgs args)
    {
        yield return new WaitForSeconds(collectDelay);

        // Haptic feedback (optional)
        var controller = (args.interactorObject as XRBaseInteractor)?
                         .transform.GetComponentInParent<XRBaseController>();
        if (controller != null) controller.SendHapticImpulse(0.5f, 0.1f);

        // Politely release before disabling
        var interactor = args.interactorObject;
        if (grab.isSelected && grab.interactionManager != null)
            grab.interactionManager.SelectExit(interactor, grab);

        // Tell GameManager we collected one
        if (GameManager.Instance) GameManager.Instance.AddCollected(itemId);

        // Lock & hide, then remove
        var rb = GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;
        grab.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        foreach (var r in GetComponentsInChildren<Renderer>()) r.enabled = false;

        if (destroyAfter) Destroy(gameObject, 0.05f);
        else gameObject.SetActive(false);
    }
}
