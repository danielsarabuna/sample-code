using System;
using Model;
using ObservableCollections;
using R3;
using UI.Router;

namespace ViewModel
{
    public class InventoryViewModel : IDisposable
    {
        private readonly CompositeDisposable _compositeDisposable = new();
        private readonly IRouter<PopupEquipmentInfoModel> _popupEquipmentInfoRouter;
        private readonly CharacterModel _characterModel;
        private readonly InventoryModel _model;
        private readonly ObservableList<EquipmentModel> _items;
        private readonly ObservableList<EquipmentModel> _sortItems;
        public IReadOnlyObservableList<EquipmentModel> Items => _items;
        public IReadOnlyObservableList<EquipmentModel> SortItems => _sortItems;
        public ReadOnlyReactiveProperty<ushort> Capacity { get; private set; }

        public InventoryViewModel(IRouter<PopupEquipmentInfoModel> popupEquipmentInfoRouterRouter,
            CharacterModel characterModel, InventoryModel model)
        {
            _popupEquipmentInfoRouter = popupEquipmentInfoRouterRouter;
            _characterModel = characterModel;
            _model = model;

            _items = new ObservableList<EquipmentModel>(_model.Items);
            _sortItems = new ObservableList<EquipmentModel>(Items);
            Capacity = new ReactiveProperty<ushort>(50);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        public void AddItem(EquipmentModel equipment)
        {
            _model.AddItem(equipment);
            _items.Add(equipment);
        }

        public void RemoveItem(EquipmentModel equipment)
        {
            _model.RemoveItem(equipment);
            _items.Remove(equipment);
        }

        public void HandleShowPopupEquipmentInfo(EquipmentModel model)
        {
            _popupEquipmentInfoRouter.SetModel(new PopupEquipmentInfoModel(_characterModel, model));
            _popupEquipmentInfoRouter.Bind();
        }

        public void HandleAutoSelect()
        {
            throw new NotImplementedException();
        }

        public void HandleAddCapacity()
        {
            throw new NotImplementedException();
        }

        public void HandleNoSort()
        {
            _sortItems.Clear();
            _sortItems.AddRange(_items);
        }

        public void HandleWeaponSort()
        {
            Sort<WeaponModel>();
        }

        public void HandleArmorSort()
        {
            Sort<ArmorModel>();
        }

        public void HandleAccessorySort()
        {
            Sort<AccessoryModel>();
        }

        private void Sort<TEquipment>() where TEquipment : EquipmentModel
        {
            _sortItems.AddRange(_items);
            foreach (var equipmentModel in _items)
                if (equipmentModel is not TEquipment)
                    _sortItems.Remove(equipmentModel);
        }
    }
}