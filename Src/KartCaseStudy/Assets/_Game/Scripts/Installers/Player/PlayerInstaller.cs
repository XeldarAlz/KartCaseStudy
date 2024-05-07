 using Zenject;
using Game.Player;
using UnityEngine.InputSystem;

namespace Game.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerStateManager>().AsSingle().NonLazy();
        }
    }
}