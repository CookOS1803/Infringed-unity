using UnityEngine;
using Zenject;

namespace Infringed.Zenject
{
    [System.Obsolete]
    public class LegacyInstaller : MonoInstaller
    {
        [SerializeField] private AIManager aiManager;

        public override void InstallBindings()
        {
            Container.BindInstance(aiManager).AsSingle();
        }
    }
}
