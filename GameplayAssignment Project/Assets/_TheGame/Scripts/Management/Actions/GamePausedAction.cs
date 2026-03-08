using Custom;
using UnityEngine;
using UnityEngine.Rendering;

public class GamePausedAction : ActionStack.Action
{// ACTION THAT RUNS WHEN ALL THE GAME COMPONENTS SHOULD BE PAUSED
    void EnableEverything(bool active)
    { // ENABLES OR DISABLES ALL PAUSE OBJECTS LIKE UI

        if (GameRoot.Instance) GameRoot.Instance.PausedScreen.SetActive(active);
    }

    public override void OnBegin(bool firstTime)
    {
        if (firstTime)
        {
            //Debug.Log("begin pause first");
        }
        else
        {
            //Debug.Log("begin pause");
        }

        GameRoot.Instance.running.EnableEverything(false);
        EnableEverything(true);

        GameRoot.Instance.PausedScreen.SetActive(true);
    }

    public override void OnUpdate()
    { // CHANGES THE WEIGHT OF THE GLOBAL VOLUMES

        //Debug.Log("update pause");
        Volume pausedPost = GameRoot.Instance.pausePostProcessing;
        Volume playingPost = GameRoot.Instance.playingPostProcessing; 
        Volume deadPost = GameRoot.Instance.deadPostProcessing;
        pausedPost.weight = Mathf.Lerp(pausedPost.weight, 1, Time.deltaTime * 10);
        playingPost.weight = playingPost.weight < 0.01f ? 0 : Mathf.Lerp(playingPost.weight, 0, Time.deltaTime * 10);        
        deadPost.weight = deadPost.weight < 0.01f ? 0 :  Mathf.Lerp(deadPost.weight, 0, Time.deltaTime * 10);
    }

    public override void OnInterrupted()
    {
        //Debug.Log("interrupted pause");

        EnableEverything(false);
    }

    public override void OnFinished()
    {
        //Debug.Log("finished pause");

        OnInterrupted();
    }

    bool isDone = false;
    public override bool IsDone()
    {
        return isDone;
    }
}
