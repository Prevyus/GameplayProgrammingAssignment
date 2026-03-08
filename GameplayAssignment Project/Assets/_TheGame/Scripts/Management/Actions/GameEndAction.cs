using UnityEngine;
using Custom;

public class GameEndAction : ActionStack.Action
{// ACTION THAT RUNS WHEN THE PLAYER DIES
    float timer = 0f;
    public override void OnBegin(bool firstTime)
    {
        if (firstTime)
        {
            //Debug.Log("begin end first");
        }
        else
        {            
            //Debug.Log("begin end");
        }

        timer = GameRoot.Instance.timeAfterDeath;
    }

    public override void OnUpdate()
    { // COUNTS DOWN A TIMER TO LOAD THE MAIN MENU AGAIN

        //Debug.Log("update end");

        if (timer > 0) timer -= Time.deltaTime;
        else OnFinished();
    }

    public override void OnInterrupted()
    {
        //Debug.Log("interrupted end");
    }

    public override void OnFinished()
    {
        //Debug.Log("finished end");
        GameRoot.Instance.ToMainMenu();

        isDone = true;
    }

    bool isDone = false;
    public override bool IsDone()
    {
        return isDone;
    }
}
