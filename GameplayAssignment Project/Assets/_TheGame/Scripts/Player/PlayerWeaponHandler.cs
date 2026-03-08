using UnityEngine;

namespace Custom
{
    public class PlayerWeaponHandler : MonoBehaviour
    { // HANDLES PRESSING THE TRIGGER OF THE WEAPON THE PLAYER IS HOLDING
        bool isAlive = true;

        [Header("Refferences")]
        public Weapon weapon;
        public Gun gun;

        [Header("Values")]
        public bool inventoryOpen = false;
        public float aimingSmoothSpeed = 5f;

        bool isAttemptingToAttack = false;

        public void Death()
        {
            isAlive = false;
        }

        private void Update()
        {
            if (!gun) return;

            if (CanShoot() && isAttemptingToAttack)
            {
                if (gun.shootingType == ShootType.fullauto) Attack();
            }
        }

        public void OnNewHoldableItem(HoldableItem item)
        {
            weapon = item.GetComponent<Weapon>();
            gun = item.GetComponent<Gun>();
        }

        bool CanShoot() { return gun && !inventoryOpen && isAlive; }

        public void Attack()
        {
            gun.Attack();
        }

        public void OnAttackDown()
        {
            isAttemptingToAttack = true;

            if (!gun) return;
            if (gun.shootingType == ShootType.semiauto || gun.shootingType == ShootType.boltaction) Attack();
        }
        public void OnShootUp()
        {
            isAttemptingToAttack =false;
        }
    }

}