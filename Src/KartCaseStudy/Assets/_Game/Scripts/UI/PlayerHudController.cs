using Game.Managers;
using I2.Loc;
using Zenject;

namespace Game.UserInterface.InGame
{
    public class PlayerHudController : RegisterGlobalParameters
    {
        [Inject] private readonly GameEventManager _gameEventManager;
    }
}