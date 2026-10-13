using UnityEngine;

[CreateAssetMenu(fileName = "PickUpItemAction", menuName = "Scriptable Objects/PickUpItemAction")]
public class PickUpItemAction : DialogAction
{
    public ItemManager.Item item;

    public override void Execute()
    {
        ItemManager.Instance.PickUpItem(item);
        ItemManager.Instance.ShowItem(item);
    }
}
