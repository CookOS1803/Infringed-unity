using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private UIInventory uiInventory;
    [SerializeField] private AIManager aiManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private new CameraController camera;
    [Space]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask enemyLayer;

    public override void InstallBindings()
    {
        Container.Bind<Inventory>().FromNew().AsTransient();
        
        Container.BindInstance(player).AsSingle();
        Container.BindInstance(camera).AsSingle();
        Container.BindInstance(uiInventory).AsSingle();
        Container.BindInstance(aiManager).AsSingle();
        
        Container.BindInstance(playerLayer).WithId(CustomLayer.Player);
        Container.BindInstance(floorLayer).WithId(CustomLayer.Floor);
        Container.BindInstance(interactableLayer).WithId(CustomLayer.Interactable);
        Container.BindInstance(enemyLayer).WithId(CustomLayer.Enemy);
    }
}