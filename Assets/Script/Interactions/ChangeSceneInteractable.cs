using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneInteractable : Interactable
{
    public string sceneName;

    public override void onInteract()
    {
        base.onInteract();
        SceneManager.LoadScene(sceneName);

    }

}
