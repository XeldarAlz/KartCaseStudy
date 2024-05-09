using Game.Managers;
using Game.SceneManagers;
using UnityEngine;
using Zenject;

namespace Game.Installers
{
    public class GameManagersInstaller : MonoInstaller
    {
        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameInputManager _gameInputManager;

        public override void InstallBindings()
        {
            Container.Bind<GameStateManager>().AsSingle().NonLazy();
            Container.Bind<GameEventManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GlobalGameManager>().AsSingle().NonLazy();
            Container.Bind<GameSceneManager>().FromComponentInHierarchy(_gameSceneManager).AsSingle().NonLazy();
            Container.Bind<GameInputManager>().FromComponentInHierarchy(_gameInputManager).AsSingle().NonLazy();
        }
    }
}