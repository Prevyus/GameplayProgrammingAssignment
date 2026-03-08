using UnityEngine;
using Custom;
using UnityEngine.UI;

namespace Custom
{
    public enum ShootType
    {
        fullauto,
        semiauto,
        boltaction
    }
}


public class Gun : Weapon
{
    // GUN CLASS THAT USES THE WEAPON BASE CLASS, CONTROLS THE ACTUAL SHOOTING AND RECOIL MOVEMENTS

    [Header("Stats")]
    public ShootType shootingType;
    public float range;
    public float reloadTime;
    public float aimingPrecision;
    public float hipFirePrecison;
    public float recoil;
    public int magSize;
    public int startAmmo;
    [Tooltip("Seconds between each shot, essentially, shooting delay")]
    public float fireRate;
    [HideInInspector] public float cooldown;

    bool attemptingShoot = false;

    [HideInInspector] public int inMag;
    [HideInInspector] public int inReserve;

    [Header("Refferences")]
    public Transform alignment;
    [SerializeField] Transform shootPoint;
    public Transform sight;
    [SerializeField] GameObject bulletImpact;
    [SerializeField] ShootParticles particles;

    [Header("UI")]
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject ammoCount;
    [SerializeField] Image ammoCountFill;

    public override void Start()
    {
        base.Start();

        inMag = magSize;
        ammoCountFill.fillAmount = (float)inMag / magSize;
    }

    private void FixedUpdate()
    {
        DebuggingRaycasts();

        if (cooldown > 0) cooldown -= Time.fixedDeltaTime;

        ResetRecoil();

        attemptingShoot = false;
    }

    bool AddAmmo(int amount)
    {
        inMag += amount;

        if (inMag >= 0) ammoCountFill.fillAmount = (float)inMag / magSize;
        return true;
    }


    void DebuggingRaycasts()
    {
        Debug.DrawRay(shootPoint.position, shootPoint.forward * range, Color.white, 0.001f);

        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range)) Point(hit.point, Color.red, 0.1f, 0.001f);
    }

    public override void Attack()
    { // EQUIVALENT TO PRESSING THE TRIGGER OF A GUN IRL
        base.Attack();
        attemptingShoot = true;
        if (cooldown > 0) return;

        if (!AddAmmo(-1)) return;

        cooldown = fireRate;

        ShootRaycast();

        ApplyRecoil();

        particles.Shoot();
    }

    void ShootRaycast()
    {// DOES THE RAYCAST TO DEAL DAMAGE TO WHATEVER IT HITS

        Ray ray = new Ray(shootPoint.position, shootPoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range))
        {
            CollisionDetector collider = hit.collider.GetComponent<CollisionDetector>();

            if (collider)
            {
                collider.Hit(damage);

                GameObject spawnedObj = Instantiate(bulletImpact, hit.collider.transform, true);
                spawnedObj.transform.position = hit.point;
                Debug.DrawLine(shootPoint.position, hit.point, Color.green, 1f);
            }
            else
            {
                GameObject spawnedObj = Instantiate(bulletImpact, hit.collider.transform, true);
                spawnedObj.transform.position = hit.point;
                Debug.DrawLine(shootPoint.position, hit.point, Color.red, 1f);
            }
        }
        else
        {
            Debug.DrawRay(shootPoint.position, shootPoint.forward * range, Color.red, 1f);
        }
    }

    void ResetRecoil()
    { // CONSTANTLY RETURNS GUN POSITION AND ROTATION TO THE DEFAULT

        holdableItem.current.item.localRotation = Quaternion.Slerp
            (
                holdableItem.current.item.localRotation,
                Quaternion.identity,
                Time.fixedDeltaTime * (attemptingShoot ? 8 : 10)
            );

        holdableItem.current.item.localPosition = Vector3.Lerp
        (
            holdableItem.current.item.localPosition,
            Vector3.zero,
            Time.fixedDeltaTime * (attemptingShoot ? 10 : 10)
        );

        Transform item = holdableItem.current.item;
        Transform cam = holdableItem.current.camPos;
        if (!cam) return;
        Vector3 target = new Vector3(cam.localPosition.x, cam.localPosition.y, item.localPosition.z);
        cam.localPosition = Vector3.Lerp(cam.localPosition, target, Time.deltaTime * 50);
    }

    void ApplyRecoil()
    {// MAKES GUN MOVE BACKWARDS AND ROTATE UPWARDS AND TO THE SIDES TO RECREATE RECOIL

        GameRoot.Instance.playerController.playerInput.additionalLookVerticalInput += recoil * 1.5f;

        Transform item = holdableItem.current.item;
        float randomRecoil = UnityEngine.Random.Range(recoil - (recoil/3), recoil +  (recoil/4));
        Vector3 recoilTargetPos = new Vector3(item.localPosition.x, item.localPosition.y + (randomRecoil / 20), item.localPosition.z - (randomRecoil/7));
        item.localPosition = Vector3.Lerp(item.localPosition, recoilTargetPos, Time.fixedDeltaTime * 70);

        float randomY = UnityEngine.Random.Range(-randomRecoil, randomRecoil) * 6;
        Quaternion recoilTargetRot = Quaternion.Euler(item.localEulerAngles.x - randomRecoil * 10, item.localEulerAngles.y + randomY, item.localEulerAngles.z);
        item.localRotation = Quaternion.Slerp(item.localRotation, recoilTargetRot, Time.fixedDeltaTime * 70);
    }

    public static void Point(Vector3 position, Color color, float size = 0.15f, float duration = 0f)
    { // JUST A DEBUGGING FUNCTION
        float h = size * 0.5f;

        Debug.DrawLine(position + Vector3.right * h, position - Vector3.right * h, color, duration);
        Debug.DrawLine(position + Vector3.up * h, position - Vector3.up * h, color, duration);
        Debug.DrawLine(position + Vector3.forward * h, position - Vector3.forward * h, color, duration);
    }
}
