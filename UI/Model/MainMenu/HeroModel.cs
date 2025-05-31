using R3;

namespace Model
{
    public class HeroModel
    {
        public ReadOnlyReactiveProperty<string> HeroName;
        public ReadOnlyReactiveProperty<ulong> Level;
        public ReadOnlyReactiveProperty<ulong> Experience;
        public ReadOnlyReactiveProperty<ulong> Health;
        public ReadOnlyReactiveProperty<ulong> Attack;
        public ReadOnlyReactiveProperty<ulong> Defense;
    }
}