using System;
using R3;
using UI.Model;

namespace Model
{
    public class MainMenuModel : IModel
    {
        public readonly ReactiveProperty<uint> Energy = new();
        public readonly ReactiveProperty<uint> Gems = new();
        public readonly ReactiveProperty<uint> Coins = new();

        public readonly ReactiveProperty<string> HeroName = new();
        public readonly ReactiveProperty<ulong> Level = new();
        public readonly ReactiveProperty<ulong> Experience = new();

        public readonly ReactiveProperty<string> CurrentPage = new();

        public readonly ReactiveProperty<bool> IsShopPageActive = new();
        public readonly ReactiveProperty<bool> IsEquipPageActive = new();
        public readonly ReactiveProperty<bool> IsGamePageActive = new();
        public readonly ReactiveProperty<bool> IsStatsPageActive = new();
        public readonly ReactiveProperty<bool> IsPvPPageActive = new();

        public readonly StatModel HealthStat = new("Health", 10);
        public readonly StatModel AttackStat = new("Attack", 10);
        public readonly StatModel DefenseStat = new("Defense", 10);
        public readonly HeroModel Hero = new HeroModel();
        public readonly InventoryModel Inventory = new InventoryModel();

        void IDisposable.Dispose()
        {
            HealthStat.Dispose();
            AttackStat.Dispose();
            DefenseStat.Dispose();
        }
    }
}