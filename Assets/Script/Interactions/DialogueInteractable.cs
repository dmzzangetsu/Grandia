using Fungus;
using UnityEngine;

public class DialogueInteractable : Interactable
{
    [SerializeField]private Flowchart flowchart;
    public string fungusBlockName = "Start";
    public override void onInteract()
    {
       base.onInteract();
       if (flowchart != null)
        {
            flowchart.ExecuteBlock(fungusBlockName);
        }

    }
}
