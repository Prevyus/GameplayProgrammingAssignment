using Custom;
using UnityEngine;
using UnityEngine.Rendering;

public class GameRunningAction : ActionStack.Action
{ // ACTION THAT RUNS WHEN ALL THE GAME COMPONENTS SHOULD BE RUNNING

    public void EnableEverything(bool active)
    { // ENABLES OR DISABLES ALL THINGS IN THE GAME THAT MOVE, BASICALLY PAUSING OR RESUMING THE GAME (NO TIMESCALING)
        
        GameRoot.Instance.PausedScreen.SetActive(!active);

        EnablePlayerComponents(active);
        EnableAIComponents(active);
        EnableInteractableObjects(active);
    }

    void EnablePlayerComponents(bool active)
    {// ENABLES OR DISABLES ALL PLAYER FUNCTIONALITY

        PlayerController player = GameRoot.Instance.playerController;

        if (!player.playerInventory.inventoryOpen)
        {
            Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !active;
        }

        if (player.playerItemHolder.holdableItem) player.playerItemHolder.holdableItem.enabled = active;
        Weapon weapon = null;
        if (player.playerItemHolder.holdableItem) weapon = player.playerItemHolder.holdableItem.GetComponent<Weapon>();
        if (weapon != null) weapon.enabled = active;

        //player.playerUI.UIObject.SetActive(active);
        player.playerAnimation.animator.enabled = active;
        player.playerInput.enabled = active;
        player.characterController.enabled = active;
        player.playerMovement.enabled = active;
        player.playerInventory.enabled = active;
        player.playerItemHolder.enabled = active;
        player.playerWeaponHandler.enabled = active;
        player.playerAnimation.enabled = active;
        player.playerHealth.enabled = active;
        player.playerUI.enabled = active;
        player.playerInteraction.enabled = active;
        player.playerAnimation.enabled = active;
        player.playerModel.enabled = active;
        player.enabled = active;

    }

    void EnableAIComponents(bool active)
    {// ENABLES OR DISABLES ALL ENEMY FUNCTIONALITY

        AIManager ai = GameRoot.Instance.aiManager;

        ai.EnableAI(active);
    }

    void EnableInteractableObjects(bool active)
    {// ENABLES OR DISABLES ALL OBJECTS FUNCTIONALITY
        GroundItem[] items = GameRoot.Instance.gameObject.GetComponentsInChildren<GroundItem>();
        for (int i = 0 ; i < items.Length ; i++)
        {
            Rigidbody rb = items[i].GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = !active;
            }
        }
    }

    public override void OnBegin(bool firstTime) 
    {
        if (firstTime)
        {
            //Debug.Log("begin running first");
        }
        else
        {
            EnableEverything(true);
        }
    }

    public override void OnUpdate()
    { // CHANGES THE WEIGHT OF THE GLOBAL VOLUMES

        //Debug.Log("update running");
        Volume playingPost = GameRoot.Instance.playingPostProcessing;
        Volume pausedPost = GameRoot.Instance.pausePostProcessing;
        Volume deadPost = GameRoot.Instance.deadPostProcessing;
        playingPost.weight = Mathf.Lerp(playingPost.weight, 1, Time.deltaTime * 10);
        pausedPost.weight = pausedPost.weight < 0.01f ? 0 : Mathf.Lerp(pausedPost.weight, 0, Time.deltaTime * 10);
        deadPost.weight = deadPost.weight < 0.01f ? 0 : Mathf.Lerp(deadPost.weight, 0, Time.deltaTime * 10);
    }

    public override void OnInterrupted()
    {
        //Debug.Log("interrupted running");
    }

    public override void OnFinished()
    {
        //Debug.Log("finished running");
    }

    bool isDone = false;
    public override bool IsDone()
    {
        return isDone;
    }
}