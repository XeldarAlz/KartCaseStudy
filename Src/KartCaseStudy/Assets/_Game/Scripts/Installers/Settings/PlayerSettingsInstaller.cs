using Game.Settings;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    [CreateAssetMenu(fileName = "PlayerSettingsInstaller", menuName = "Installers/PlayerSettingsInstaller")]
    public class PlayerSettingsInstaller : ScriptableObjectInstaller<PlayerSettingsInstaller>
    {
        public PlayerMovementSettings PlayerMovementSettings;
        public PlayerInteractionSettings PlayerInteractionSettings;

        public override void InstallBindings()
        {
            Container.Bind<PlayerMovementSettings>().FromInstance(PlayerMovementSettings).AsSingle();
            Container.Bind<PlayerInteractionSettings>().FromInstance(PlayerInteractionSettings).AsSingle();
        }
    }
}