using UnityEngine;

namespace View
{
    [RequireComponent(typeof(PlayerEquipmentView))]
    public class PlayerEntityView : GameEntityView
    {
        [SerializeField] private PlayerEquipmentView _playerEquipmentView;
    }
}