using Game.Settings;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/GameSettingsInstaller")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public GameSettings GameSettings;

        public override void InstallBindings()
        {
            Container.Bind<GameSettings>().FromInstance(GameSettings).AsSingle();
        }
    }
}