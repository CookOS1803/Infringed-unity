using Infringed.InventorySystem.UI;
using UnityEngine;
using Zenject;

namespace Infringed.Injection
{
    public class FactoryInstaller : MonoInstaller
    {
        [SerializeField] private GameObject uiItemPrefab;

        public override void InstallBindings()
        {
            var uiItemParentPool = FindObjectOfType<UIInventory>().transform;

            Container.BindFactory<UIItem, UIItem.Factory>().FromMonoPoolableMemoryPool(
                binder => binder.WithInitialSize(10).FromComponentInNewPrefab(uiItemPrefab).UnderTransform(uiItemParentPool)
            );
        }
    }
}
