using Custom;
using TMPro;
using UnityEngine;

public class EnableAIButton : PhysicalButton
{
    [SerializeField] TextMeshProUGUI turnOnOffText;

    public override void Start()
    {
        base.Start();

        turnOnOffText.text = GameRoot.Instance.aiManager.running ? "On" : "Off";
    }

    public override void Activate(IInteractor interactor)
    {
        base.Activate(interactor);

        if (GameRoot.Instance.aiManager.running)
        {
            GameRoot.Instance.aiManager.EnableAI(false);
            GameRoot.Instance.aiManager.disabledByButton = true;
        }
        else
        {
            GameRoot.Instance.aiManager.disabledByButton = false;
            GameRoot.Instance.aiManager.EnableAI(true);
        }
        turnOnOffText.text = GameRoot.Instance.aiManager.running ? "On" : "Off";
    }
}
