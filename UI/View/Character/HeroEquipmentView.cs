using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Cysharp.Threading.Tasks;
using Model;
using ObservableCollections;
using Proxy;
using UnityEngine;

namespace View
{
    public abstract class HeroEquipmentView : CharacterViewBase<HeroEquipmentModelProxy>
    {
        [SerializeField] private HumanoidSkinView _humanoidSkinView;

        private void OnDestroy()
        {
            if (Model == null) return;
            Model.ObservableEquipmentSlots.CollectionChanged -= HandleEquipmentSlotChange;
        }

        public override async UniTask Initialize(HeroEquipmentModelProxy model)
        {
            await base.Initialize(model);
            foreach (var pair in model.EquipmentSlots.Where(pair => pair.Value != null))
                LoadEquipmentIcon(pair.Key, pair.Value);

            Model.ObservableEquipmentSlots.CollectionChanged += HandleEquipmentSlotChange;
        }

        private void HandleEquipmentSlotChange(
            in NotifyCollectionChangedEventArgs<KeyValuePair<EquipmentSlot, EquipmentModel>> eventArgs)
        {
            if (eventArgs.Action != NotifyCollectionChangedAction.Replace) return;
            if (eventArgs.NewItem.Value != null)
            {
                LoadEquipmentIcon(eventArgs.NewItem.Key, eventArgs.NewItem.Value);
            }
            else
            {
                RefreshEquipmentDisplay(eventArgs.NewItem.Key, new List<Sprite>());
            }
        }

        private async void LoadEquipmentIcon(EquipmentSlot slot, EquipmentModel equipmentModel)
        {
            var sprites = new List<Sprite>();
            foreach (var assetName in GetEquipmentAssetNames(slot, equipmentModel))
            {
                var sprite = await AssetManagement.LoadAssetAsync<Sprite>(assetName);
                sprites.Add(sprite);
            }

            RefreshEquipmentDisplay(slot, sprites);
        }

        private List<string> GetEquipmentAssetNames(EquipmentSlot slot, EquipmentModel equipmentModel)
        {
            var assetNames = new List<string>();
            var root = equipmentModel.AssetPath;

            switch (slot)
            {
                case EquipmentSlot.Helmet:
                    assetNames.Add(root);
                    break;
                case EquipmentSlot.Chest:
                    assetNames.Add($"{root}-ArmL");
                    assetNames.Add($"{root}-ArmR");
                    assetNames.Add($"{root}-ForearmL");
                    assetNames.Add($"{root}-Pelvis");
                    assetNames.Add($"{root}-SleeveR");
                    assetNames.Add($"{root}-Torso");
                    break;
                case EquipmentSlot.Legs:
                    assetNames.Add($"{root}-Shin");
                    assetNames.Add($"{root}-Leg");
                    break;
                case EquipmentSlot.RightHand:
                case EquipmentSlot.LeftHand:
                    assetNames.Add($"{root}");
                    break;
                case EquipmentSlot.Gloves:
                    assetNames.Add($"{root}-HandL");
                    assetNames.Add($"{root}-HandR");
                    assetNames.Add($"{root}-Finger");
                    break;
            }

            return assetNames;
        }

        private void RefreshEquipmentDisplay(EquipmentSlot slot, List<Sprite> sprites)
        {
            _humanoidSkinView.ApplyEquipment(slot, sprites);
        }
    }
}