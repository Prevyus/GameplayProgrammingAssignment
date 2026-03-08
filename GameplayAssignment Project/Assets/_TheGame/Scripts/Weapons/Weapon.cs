using UnityEngine;
using Custom;

public class Weapon : MonoBehaviour
{ // BASE CLASS FOR WEAPONS
    public HoldableItem holdableItem;
    public PlayerItemHolder playerItemHolder;

    [Header("Weapon Stats")]
    public float damage = 10;

    public virtual void Start()
    {
        holdableItem = GetComponent<HoldableItem>();
        playerItemHolder = holdableItem.playerItemHolder;
    }


    public virtual void Attack()
    {

    }
}
