using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{ // CONTROLS USER INTERFACE COMPONENTS IN THE PLAYER (NOT INVENTORY RELATED)
    bool isAlive = true;

    [Header("Refferences")]
    public GameObject UIObject;
    [SerializeField] Canvas Playing;
    [SerializeField] Image StaminaPanel;
    [SerializeField] Image StaminaFillImage;
    [SerializeField] Image HealthPanel;
    [SerializeField] Image HealthFillImage;
    public RawImage EnemyCam;

    [Header("Values")]
    [SerializeField] float staminaBarLerpSpeed = 1;
    [HideInInspector] public float stamina = 1;

    [SerializeField] float healthBarLerpSpeed = 1;
    [HideInInspector] public float health = 1;

    public void Death()
    {
        isAlive = false;
    }

    private void Update()
    {
        StaminaFillImage.fillAmount = Mathf.Lerp(StaminaFillImage.fillAmount, stamina, Time.deltaTime * staminaBarLerpSpeed);

        HealthFillImage.fillAmount = Mathf.Lerp(HealthFillImage.fillAmount, health, Time.deltaTime * healthBarLerpSpeed);
    }

    public void OnInventory(bool open)
    {
        EnemyCam.gameObject.SetActive(!open);
    }
}
