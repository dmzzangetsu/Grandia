using UnityEngine;


public class Interactable : MonoBehaviour
{
    [Header ("Bubble Icon Config")]
    [SerializeField] private Texture2D bubbleIcon;
    public SpriteRenderer visual;
    [Header ("Is the interactable one time?")]
    [SerializeField] private bool isOneTime;

    

    public virtual void onInteract()
    {
        Debug.Log(name + "interacted");
    }

}
