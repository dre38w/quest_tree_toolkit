using Gameplay.System;
using Gameplay.UI;
using Service.Core;
using Service.Framework;
using Service.Framework.StatusSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    [HideInInspector]
    public UnityEvent OnInteracted = new UnityEvent();

    [SerializeField]
    private InteractableTrigger interactableObject;

    private PlayerInteractor interactor;
    private MainUI mainUI;

    public bool didInteract;

    private void Awake()
    {
        didInteract = false;
    }

    private void Start()
    {
        mainUI = ReferenceRegistry.Instance.MainUI;
        interactor = ReferenceRegistry.Instance.Player.GetComponent<PlayerInteractor>();
        interactableObject.OnEnteredTriggerObject.AddListener(OnInteracting);
        interactableObject.OnExitedTriggerObject.AddListener(OnNotInteracting);
        interactor.OnInteracted.AddListener(OnPlayerInteracted);
    }

    public void Interact(GameObject interactor)
    {
        if (!CanInteract(interactor))
        {
            return;
        }
    }

    private void OnInteracting()
    {
        mainUI.SetContextualUiVisible(true);

    }

    private void OnNotInteracting()
    {
        mainUI.SetContextualUiVisible(false);

    }

    private void OnPlayerInteracted()
    {
        didInteract = true;
        mainUI.SetContextualUiVisible(false);
        OnInteracted.Invoke();
        StartCoroutine(ResetInteracted());
    }

    /// <summary>
    /// Reset next frame to allow other systems time to do logic
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetInteracted()
    {
        yield return null;
        didInteract = false;
    }

    public bool CanInteract(GameObject interactor)
    {
        return mainUI.ContextualButtonUI.activeSelf;
    }

    private void OnDestroy()
    {
        interactableObject.OnEnteredTriggerObject.RemoveListener(OnInteracting);
        interactableObject.OnExitedTriggerObject.RemoveListener(OnNotInteracting);
        interactor.OnInteracted.RemoveListener(OnPlayerInteracted);
        didInteract = false;
    }
}
