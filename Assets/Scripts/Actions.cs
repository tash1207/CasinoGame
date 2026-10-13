using System;

public static class Actions
{
    public static Action<StartGameManager.Game> OnStartGame;
    public static Action<ItemManager.Item> PickUpItem;
}
