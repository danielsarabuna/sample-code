using System;
using R3;
using UI.Model;

namespace Model
{
    public class GameModel : IModel
    {
        public readonly ReactiveProperty<uint> Energy = new();
        public readonly ReactiveProperty<uint> Gems = new();
        public readonly ReactiveProperty<uint> Coins = new();

        public readonly ReactiveProperty<string> HeroName = new();
        public readonly ReactiveProperty<ulong> Level = new();
        public readonly ReactiveProperty<ulong> Experience = new();

        public readonly ReactiveProperty<ulong> Health = new();
        public readonly ReactiveProperty<ulong> Attack = new();
        public readonly ReactiveProperty<ulong> Defense = new();

        public readonly ReactiveProperty<string> BattleSpeed = new();

        void IDisposable.Dispose()
        {
        }
    }
}