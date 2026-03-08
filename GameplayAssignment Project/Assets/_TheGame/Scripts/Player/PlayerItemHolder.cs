using Custom;
using System;
using UnityEngine;

namespace Custom
{
    public class PlayerItemHolder : MonoBehaviour
    { // PLAYER COMPONENT THAT CONTROLS THE MOVEMENT OF THE ARMS WHEN HOLDING ITEMS

        bool isAlive = true;

        [Header("Item")]
        public Object holdingObject;
        Object oldHoldingObject;

        public GameObject spawnedObject;

        public HoldableItem holdableItem;
        public event Action<HoldableItem> OnNewHoldableItem;

        [Header("Refferences")]
        public Transform cam;
        public Transform ArmsAnchor;
        public Transform cameraFollowTransform;
        public Transform neckFollowTransform;

        [Header("Settings")]
        public string armSetName = "";
        [HideInInspector] public string oldArmSetName = "";
        public int armSet = 0;
        [SerializeField] float SmoothSpeed = 1;
        [SerializeField] float camAlignmentSpeed = 10;
        [SerializeField] float breathPosAmplitude = 0.02f;
        [SerializeField] float breathPosFrequency = 1.2f;
        [SerializeField] float breathRotAmplitude = 1.5f;
        [SerializeField] float breathRotFrequency = 1.2f;

        [Header("Hand Positions")]
        public Transform rightElbow;
        public Transform rightHand;
        public Transform leftElbow;
        public Transform leftHand;

        [HideInInspector] public float LookHorizontalInput = 0;
        [HideInInspector] public float LookVerticalInput = 0;

        bool alreadySpawned = false;
        bool isAiming = false;

        public void Death()
        {
            isAlive = false;
        }

        private void Awake()
        {
            cam = GetComponentInChildren<Camera>().transform;
        }

        private void Update()
        {
            if (!isAlive) return;

            if (isAiming) armSetName = "aiming";

            SmoothResetLocalPosAndRot();

            if (oldHoldingObject != holdingObject)
            {
                OnHoldingObjectChanged();
            }

            if ((oldArmSetName != armSetName) && holdableItem != null) OnArmSetChanged();

            if (holdableItem != null) // CAMERA AND ITEM MOVEMENT WHENEVER THERES AN ITEM IN THE HANDS
            {
                CameraAlignment();
                ItemBreathing();
                ItemSway();
            }
            else cam.localPosition = Vector3.Lerp(cam.localPosition, Vector3.zero, Time.deltaTime * camAlignmentSpeed);
        }
        void ItemBreathing()
        { // GIVES THE ITEM A WAVEY MOVEMENT TO RECREATE BREATHING VISUAL

            if (holdableItem.armSets[armSet].breathing)
            {
                float time = Time.time;

                float baseWave = Mathf.Sin(time * holdableItem.armSets[armSet].breathingFrequency);
                float secondaryWave = Mathf.Cos(time * holdableItem.armSets[armSet].breathingFrequency * 0.63f + 1.3f);

                float combined = baseWave * 0.6f + secondaryWave* .5f;

                holdableItem.armSets[armSet].item.position = holdableItem.armSets[armSet].item.parent.position + Vector3.one.normalized * combined * holdableItem.armSets[armSet].breathingAmplitude;
            }
            else if (!holdableItem.armSets[armSet].swaying)
            {
                holdableItem.armSets[armSet].item.localPosition = Vector3.zero;
            }
        }
        void ItemSway()
        { // ROTATES THE ITEM WITH THE LOOK MOVEMENT
            Transform item = holdableItem.armSets[armSet].item.parent;
            float swayIntensity = holdableItem.current.swayIntensity;
            if (holdableItem.armSets[armSet].swaying)
            {
                Quaternion targetRot = Quaternion.Euler(-LookVerticalInput * swayIntensity, LookHorizontalInput * swayIntensity, 0);
                item.localRotation = Quaternion.Slerp(item.localRotation, targetRot, Time.deltaTime * 15);
            }
            else
            {

            }
        }

        void CameraAlignment()
        { //  ALIGNS THE CAMERA WITH THE SIGHT OF THE GUN
            if (holdableItem.armSets[armSet].camPos && holdableItem.armSets[armSet].moveCamera)
            {
                Transform target = holdableItem.armSets[armSet].camPos;
                cam.position = Vector3.Lerp(cam.position, target.position, Time.deltaTime * camAlignmentSpeed);
            }
            else
            {
                cam.localPosition = Vector3.Lerp(cam.localPosition, Vector3.zero, Time.deltaTime * camAlignmentSpeed);
            }
        }

        public void OnHoldingObjectChanged()
        { // WHEN THE OBJECT THE PLAYER IS HOLDING CHANGES, IT GETS RID OF THE OLD OBJECT AND SPAWNS A THE NEW ONE
            Vector3 ItemPos = holdableItem ? holdableItem.Item.transform.position : Vector3.zero;
            Quaternion ItemRot = holdableItem ? holdableItem.Item.transform.rotation : Quaternion.identity;

            if (oldHoldingObject != null && oldHoldingObject.Holdable)
            { // DESTROYS OLD OBJECT
                ResetParents();
                Destroy(spawnedObject);
                spawnedObject = null;
                alreadySpawned = false;
            }

            if (holdingObject != null && holdingObject.Holdable && !alreadySpawned)
            { // SPAWNS NEW OBJECT
                spawnedObject = Instantiate(holdingObject.InHandPrefab, neckFollowTransform.position, neckFollowTransform.rotation, neckFollowTransform);
                if (spawnedObject)
                {
                    spawnedObject.transform.localPosition = Vector3.zero;
                    spawnedObject.transform.localRotation = Quaternion.identity;

                    holdableItem = spawnedObject.GetComponent<HoldableItem>();
                    OnNewHoldableItem?.Invoke(holdableItem);
                    holdableItem.playerItemHolder = this;

                    UpdateParents();
                    alreadySpawned = true;
                }
                OnArmSetChanged(0);
            }

            oldHoldingObject = holdingObject;

            if (holdingObject != null) OnArmSetChanged();
        }

        void OnArmSetChanged()
        {// WHEN THE ARM STANCE OF THE PLAYER CHANCES, FOR EXAMPLE FROM IDLE TO AIMING
            if (holdableItem)
            {
                for (int i = 0; i < holdableItem.armSets.Count; i++)
                {
                    if (armSetName == holdableItem.armSets[i].armSetName)
                    {
                        oldArmSetName = armSetName;
                        armSet = i;
                        holdableItem.current = holdableItem.armSets[i];
                        OnArmSetChanged(armSet);
                        break;
                    }
                }
                UpdateParents();
            }
        }

        void OnArmSetChanged(int newValue)
        {
            armSet = newValue;
            holdableItem.current = holdableItem.armSets[newValue];
            UpdateParents();
        }

        void UpdateParents()
        { // REPARENTS ALL THE ITEM ARM POSITIONS TO BE CONNECTED WITH THE PLAYER
            try
            {
                if (!holdableItem || armSet >= holdableItem.armSets.Count) return;

                if (holdableItem.armSets[armSet].followCamera) { holdableItem.transform.SetParent(cameraFollowTransform, true); }
                else { holdableItem.transform.SetParent(neckFollowTransform, true); }

                if (holdableItem.armSets[armSet].item) { holdableItem.Item.SetParent(holdableItem.armSets[armSet].item, true); }
                else { holdableItem.Item.SetParent(holdableItem.transform, true); }

                if (holdableItem.armSets[armSet].rightElbow) { rightElbow.SetParent(holdableItem.armSets[armSet].rightElbow, true); }
                else { rightElbow.SetParent(ArmsAnchor, true); }

                if (holdableItem.armSets[armSet].rightHand) { rightHand.SetParent(holdableItem.armSets[armSet].rightHand, true); }
                else { rightHand.SetParent(ArmsAnchor, true); }

                if (holdableItem.armSets[armSet].leftElbow) { leftElbow.SetParent(holdableItem.armSets[armSet].leftElbow, true); }
                else { leftElbow.SetParent(ArmsAnchor, true); }

                if (holdableItem.armSets[armSet].leftHand) { leftHand.SetParent(holdableItem.armSets[armSet].leftHand, true); }
                else { leftHand.SetParent(ArmsAnchor, true); }
            }
            catch { Debug.Log("Error in UpdatingParents - Santi"); /*GameRoot.Instance.StopGame();*/ }
        }

        void ResetParents()
        {
            rightElbow.SetParent(ArmsAnchor, true);
            rightHand.SetParent(ArmsAnchor, true);
            leftElbow.SetParent(ArmsAnchor, true);
            leftHand.SetParent(ArmsAnchor, true);
        }

        void ResetLocalPosAndRot(Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        void SmoothResetLocalPosAndRot()
        { // MOVES THE POSITION AND ROTATIONS OF ALL THE ARM POSITIONS FOR SMOOTH ARM MOVEMENT
            try
            {
                if (holdableItem)
                {
                    holdableItem.transform.localPosition = Vector3.Lerp(holdableItem.transform.localPosition, Vector3.zero, Time.deltaTime * SmoothSpeed);
                    holdableItem.transform.localRotation = Quaternion.Lerp(holdableItem.transform.localRotation, Quaternion.identity, Time.deltaTime * SmoothSpeed);

                    holdableItem.Item.localPosition = Vector3.Lerp(holdableItem.Item.localPosition, Vector3.zero, Time.deltaTime * SmoothSpeed);
                    holdableItem.Item.localRotation = Quaternion.Lerp(holdableItem.Item.localRotation, Quaternion.identity, Time.deltaTime * SmoothSpeed);

                    if (holdableItem.armSets[armSet].rightHand)
                    {
                        rightElbow.localPosition = Vector3.Lerp(rightElbow.localPosition, Vector3.zero, Time.deltaTime * SmoothSpeed);
                        rightElbow.localRotation = Quaternion.Lerp(rightElbow.localRotation, Quaternion.identity, Time.deltaTime * SmoothSpeed);

                        rightHand.localPosition = Vector3.Lerp(rightHand.localPosition, Vector3.zero, Time.deltaTime * SmoothSpeed);
                        rightHand.localRotation = Quaternion.Lerp(rightHand.localRotation, Quaternion.identity, Time.deltaTime * SmoothSpeed);
                    }

                    if (holdableItem.armSets[armSet].leftHand)
                    {
                        leftElbow.localPosition = Vector3.Lerp(leftElbow.localPosition, Vector3.zero, Time.deltaTime * SmoothSpeed);
                        leftElbow.localRotation = Quaternion.Lerp(leftElbow.localRotation, Quaternion.identity, Time.deltaTime * SmoothSpeed);

                        leftHand.localPosition = Vector3.Lerp(leftHand.localPosition, Vector3.zero, Time.deltaTime * SmoothSpeed);
                        leftHand.localRotation = Quaternion.Lerp(leftHand.localRotation, Quaternion.identity, Time.deltaTime * SmoothSpeed);
                    }
                }
            }
            catch { Debug.Log("Error in SmoothResetLocalPosAndRot - Santi"); /*GameRoot.Instance.StopGame();*/ }
        }

        public void OnAimDown()
        {
            armSetName = "aiming";
            isAiming = true;
        }

        public void OnAimUp()
        {
            if (GameRoot.Instance.gamePaused) return;
            armSetName = "default";
            isAiming = false;
        }
    }

}
