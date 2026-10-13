using UnityEngine;

[CreateAssetMenu(fileName = "StartGameAction", menuName = "Scriptable Objects/StartGameAction")]
public class StartGameAction : DialogAction
{
    public StartGameManager.Game game;

    public override void Execute()
    {
        Actions.OnStartGame(game);
    }
}
