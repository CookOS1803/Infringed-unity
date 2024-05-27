using Infringed.InventorySystem.UI;
using UnityEngine;
using Zenject;

namespace Infringed.Injection
{
    public class FactoryInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _uiItemPrefab;
        [SerializeField] private Transform _uiItemParentPool;
        [SerializeField, Min(1)] private int _poolInitialSize = 10;

        public override void InstallBindings()
        {
            Container.BindFactory<UIItem, UIItem.Factory>().FromMonoPoolableMemoryPool(
                binder => binder.WithInitialSize(_poolInitialSize).FromComponentInNewPrefab(_uiItemPrefab).UnderTransform(_uiItemParentPool)
            );
        }
    }
}
