namespace Model
{
    public class ArtifactModel : EquipmentModel
    {
        public string UniqueAbility { get; set; }

        public ArtifactModel() : base(string.Empty, 1, EquipmentRarity.Common)
        {
        }

        public ArtifactModel(string name, uint level, EquipmentRarity rarity, string uniqueAbility)
            : base(name, level, rarity)
        {
            UniqueAbility = uniqueAbility;
        }
    }
}