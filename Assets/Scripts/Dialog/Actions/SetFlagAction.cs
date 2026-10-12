using UnityEngine;

[CreateAssetMenu(fileName = "SetFlagAction", menuName = "Scriptable Objects/SetFlagAction")]
public class SetFlagAction : DialogAction
{
    public string flagName;
    public bool value;

    public override void Execute()
    {
        GameStateManager.Instance.SetFlag(flagName, value);
    }
}
