using Unity.VisualScripting;
using UnityEngine;

public class TopDownManager : MonoBehaviour
{
    public static TopDownManager Instance {get;private set;}
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private InteractingArea interactingArea; 
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitiateInteraction()
    {
        
        playerMovement.SetMovement(false);
        interactingArea.SetInteraction(true);

    }

    public void FinishInteraction()
    {
        playerMovement.SetMovement(true);
        interactingArea.SetInteraction(false);
    }

    public void RegisterPlayerMovement(PlayerMovement playerMovementInstance)
    {
     playerMovement = playerMovementInstance;           
    }

    public void RegisterInteractingArea(InteractingArea interactingAreaInstance)
    {
        interactingArea = interactingAreaInstance;
    }
}
