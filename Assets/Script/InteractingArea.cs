using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractingArea : MonoBehaviour
{
    private bool isInteracting = false; 
    public List<GameObject> interactableList = new List<GameObject>();
    [SerializeField] private InputActionReference interactActionsReference;

    
    void Update()
    {
        if (interactableList.Count == 0)
        {
            return;
        }
        else if(interactableList.Count > 1)
        {
            SortInteractableByDistance();
        }
        

    }
    void Start()
    {
        TopDownManager.Instance.RegisterInteractingArea(this);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        interactableList.Add(collision.gameObject);
     
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        interactableList.Remove(collision.gameObject);

       
    }

    void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (isInteracting == false & interactableList.Count > 0)
        {
            Interactable currentInteractable = interactableList[0].GetComponent<Interactable>();
            currentInteractable.onInteract();
            TopDownManager.Instance.InitiateInteraction();
            
            
        }
    }

    void OnEnable()
    {
        interactActionsReference.action.Enable();
        interactActionsReference.action.started += OnInteractPressed;
    }

    void OnDisable()
    {
        interactActionsReference.action.started -= OnInteractPressed;
        interactActionsReference.action.Disable();
    }

    public void SetInteraction(bool newValue)
    {
        isInteracting = newValue;
    }

    private void SortInteractableByDistance()
    {
        interactableList.Sort((a,b ) =>
        {
            float distanceA = Vector3.Distance(transform.position, a.transform.position);
            float distanceB = Vector3.Distance(transform.position, b.transform.position);
            return distanceA.CompareTo(distanceB);
        });
    }
    public void UpdateInteractionAreaPosition(Vector2 pos)
    {


        if (pos.x > 0)
        {
            transform.rotation = Quaternion.Euler(0,0,270);
        }
        else if (pos.x < 0)
        {
            transform.rotation = Quaternion.Euler(0,0,90);
        }
        else if(pos.y < 0)
        {
            transform.rotation = Quaternion.Euler(0,0,180);
        }
        else if(pos.y > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        
    }
}
