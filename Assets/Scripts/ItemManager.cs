using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public enum Item
    {
        FREE_POKER_CHIP,
        USELESS_GARBAGE
    }
    
    public static ItemManager Instance { get; private set; }

    [SerializeField] GameObject itemCanvas;
    [SerializeField] GameObject itemImagesParent;

    private Dictionary<Item, bool> items = new Dictionary<Item, bool>();

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void PickUpItem(Item item)
    {
        items[item] = true;
        if (item == Item.FREE_POKER_CHIP)
        {
            MoneyManager.Instance.AddMoney(5);
        }
    }

    public void RemoveItem(Item item)
    {
        items[item] = false;
    }

    public bool HasItem(Item item)
    {
        if (items.TryGetValue(item, out bool value))
        {
            return value;
        }
        return false;
    }

    public void ShowItem(Item item)
    {
        ItemObject[] allItems = itemImagesParent.GetComponentsInChildren<ItemObject>();
        foreach (var itemImage in allItems)
        {
            itemImage.gameObject.SetActive(itemImage.itemType == item);
        }
        itemCanvas.SetActive(true);
    }
}
