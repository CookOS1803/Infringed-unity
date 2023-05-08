using UnityEditor;
using UnityEngine;
using Zenject;

[System.Serializable]
public class CustomAudio
{
    [field: SerializeField] public AudioClip weaponSwing { get; private set; }
    [field: SerializeField] public AudioClip weaponHit { get; private set; }
    [field: SerializeField] public AudioClip[] steps { get; private set; }

    public AudioClip GetRandomStep()
    {
        return steps[Random.Range(0, steps.Length)];
    }
}

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
    [Space]
    [SerializeField] private CustomAudio customAudio;

    public override void InstallBindings()
    {
        var scriptables = Resources.LoadAll<ItemAction>("Items/Actions");

        foreach (var item in scriptables)
        {
            Container.QueueForInject(item);
        }

        Container.Bind<Inventory>().FromNew().AsTransient();
        
        Container.BindInstance(player).AsSingle();
        Container.BindInstance(camera).AsSingle();
        Container.BindInstance(uiInventory).AsSingle();
        Container.BindInstance(aiManager).AsSingle();
        
        Container.BindInstance(playerLayer).WithId(CustomLayer.Player);
        Container.BindInstance(floorLayer).WithId(CustomLayer.Floor);
        Container.BindInstance(interactableLayer).WithId(CustomLayer.Interactable);
        Container.BindInstance(enemyLayer).WithId(CustomLayer.Enemy);

        Container.BindInstance(customAudio).AsSingle();
    }
}