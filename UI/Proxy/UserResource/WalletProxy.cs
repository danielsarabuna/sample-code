using Model;
using R3;

namespace Proxy
{
    public class WalletProxy : IWallet
    {
        private readonly IWallet _wallet;

        private readonly ReactiveProperty<uint> _energy;
        private readonly ReactiveProperty<uint> _gems;
        private readonly ReactiveProperty<uint> _coins;
        public ReadOnlyReactiveProperty<uint> EnergyRO => _energy;
        public ReadOnlyReactiveProperty<uint> CoinsRO => _coins;
        public ReadOnlyReactiveProperty<uint> GemsRO => _gems;

        uint IWallet.Energy => _wallet.Energy;
        uint IWallet.Gems => _wallet.Gems;
        uint IWallet.Coins => _wallet.Coins;

        public WalletProxy(IWallet wallet)
        {
            _wallet = wallet;
            _energy = new ReactiveProperty<uint>(_wallet.Energy);
            _gems = new ReactiveProperty<uint>(_wallet.Gems);
            _coins = new ReactiveProperty<uint>(_wallet.Coins);
        }

        public void AddEnergy(uint amount)
        {
            _wallet.AddEnergy(amount);
            _energy.Value = _wallet.Energy;
        }

        public void SpendEnergy(uint amount)
        {
            _wallet.SpendEnergy(amount);
            _energy.Value = _wallet.Energy;
        }

        public void AddGems(uint amount)
        {
            _wallet.AddGems(amount);
            _gems.Value = _wallet.Gems;
        }

        public void SpendGems(uint amount)
        {
            _wallet.SpendGems(amount);
            _gems.Value = _wallet.Gems;
        }

        public void AddCoins(uint amount)
        {
            _wallet.AddCoins(amount);
            _coins.Value = _wallet.Coins;
        }

        public void SpendCoins(uint amount)
        {
            _wallet.SpendCoins(amount);
            _coins.Value = _wallet.Coins;
        }
    }
}