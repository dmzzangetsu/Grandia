using Fungus;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class DialogueHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Flowchart fungus;
    public PlayableDirector timeline;
public void PlayFungusBlock(string block)
{
    fungus.ExecuteBlock(block);
}

public void PauseTimeline()
{
        timeline.Pause(); // Halts playback completely
}

public void ResumeTimeline()
{
        timeline.Play(); // Resumes from where it was paused
}

public void ChangeScene(string sceneName)
{
    SceneManager.LoadScene(sceneName);
}

}