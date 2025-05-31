using ObservableCollections;
using R3;
using UI.Model;

namespace Model
{
    public class PopupEquipmentInfoModel : IModel
    {
        public readonly CharacterModel CharacterModel;
        public readonly EquipmentModel Equipment;
        public readonly string Name;
        public readonly string Description;
        public readonly ReactiveProperty<uint> Level;
        public readonly ReactiveProperty<EquipmentRarity> Rarity;
        public readonly ReactiveProperty<ulong> UpgradeCost;
        private readonly ObservableDictionary<string, double> _properties;
        private readonly ObservableList<string> _ability;
        private readonly ObservableList<string> _effects;
        public IReadOnlyObservableDictionary<string, double> Properties => _properties;
        public IReadOnlyObservableList<string> Ability => _ability;
        public IReadOnlyObservableList<string> Effects => _effects;

        public PopupEquipmentInfoModel(CharacterModel characterModel, EquipmentModel equipment)
        {
            CharacterModel = characterModel;
            Equipment = equipment;
            Name = equipment.Name;
            Description = equipment.Description;

            Level = new ReactiveProperty<uint>(equipment.Level);
            Rarity = new ReactiveProperty<EquipmentRarity>(equipment.Rarity);

            _properties = new ObservableDictionary<string, double>();
            _ability = new ObservableList<string>();
            _effects = new ObservableList<string>();

            switch (equipment)
            {
                case ArmorModel armorModel:
                    _properties.Add("Defense", armorModel.Defense);
                    _properties.Add("HealthBonus", armorModel.HealthBonus);
                    break;
                case WeaponModel weaponModel:
                    _properties.Add("Damage", weaponModel.Damage);
                    _properties.Add("AttackSpeed", weaponModel.AttackSpeed);
                    break;
            }

            foreach (var model in equipment.Effects)
                _effects.Add(Format(model));

            switch (equipment)
            {
                case AccessoryModel accessoryModel:
                    _ability.Add(accessoryModel.UniqueAbility);
                    break;
                case ArtifactModel artifactModel:
                    _ability.Add(artifactModel.UniqueAbility);
                    break;
            }

            UpgradeCost = new ReactiveProperty<ulong>(300);
        }

        public void Dispose()
        {
            // TODO release managed resources here
        }

        private string Format(EquipmentEffectModel equipment)
        {
            const string effectType = "effect-type";
            const string value = "value";
            const string duration = "duration";
            var description = equipment.Description;

            if (description.Contains(effectType))
                description = description.Replace(effectType, equipment.EffectType.ToString());

            if (description.Contains(value))
                description = description.Replace(value, equipment.Value.ToString("##"));

            if (description.Contains(duration))
                description = description.Replace(duration, equipment.Duration.ToString("##"));

            return description;
        }
    }
}