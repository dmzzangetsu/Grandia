using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionUI : MonoBehaviour
{
    public Button attackButton;
    public Button defendButton;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(attackButton.gameObject);
    }

 
}
