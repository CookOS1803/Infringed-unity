using Infringed.Actions;
using Infringed.AI;
using Infringed.InventorySystem;
using Infringed.InventorySystem.UI;
using Infringed.Player;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Infringed
{
    [System.Serializable]
    public class CustomAudio
    {
        [field: SerializeField] public AudioClip WeaponSwing { get; private set; }
        [field: SerializeField] public AudioClip WeaponHit { get; private set; }
        [field: SerializeField] public AudioClip[] Steps { get; private set; }

        public AudioClip GetRandomStep()
        {
            return Steps[Random.Range(0, Steps.Length)];
        }
    }
}

namespace Infringed.Injection
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private UIInventory _uiInventory;
        [SerializeField] private PlayerController _player;
        [SerializeField] private CameraController _camera;
        [Space]
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private LayerMask _floorLayer;
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private LayerMask _enemyLayer;
        [Space]
        [SerializeField] private CustomAudio _customAudio;

        public override void InstallBindings()
        {
            var scriptables = Resources.LoadAll<ItemAction>("Items/Actions");

            foreach (var item in scriptables)
            {
                Container.QueueForInject(item);
            }

            Container.BindInstance(_player).AsSingle();
            Container.BindInstance(_camera).AsSingle();
            Container.BindInstance(_uiInventory).AsSingle();

            Container.BindInstance(_playerLayer).WithId(CustomLayer.Player);
            Container.BindInstance(_floorLayer).WithId(CustomLayer.Floor);
            Container.BindInstance(_interactableLayer).WithId(CustomLayer.Interactable);
            Container.BindInstance(_enemyLayer).WithId(CustomLayer.Enemy);

            Container.BindInstance(_customAudio).AsSingle();
        }
    }
}
