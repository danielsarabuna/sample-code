using NUnit.Framework;
using Gameplay.Battle.Abilities;
using System.Threading.Tasks;
using Gameplay.Battle;

[TestFixture]
public class AbilitiesTests
{
    private Unit _user;
    private Unit _target;
    private readonly AbilityFactory _factory = new AbilityFactory();

    [SetUp]
    public void Setup()
    {
        _user = new Unit("TestUser")
        {
            Health = 100,
            Mana = 100,
        };

        _target = new Unit("TestTarget")
        {
            Health = 100,
            Mana = 100,
        };
    }

    [Test]
    public async Task DamageAbilityShouldDealCorrectDamage()
    {
        // Arrange
        var damage = 20f;
        var ability = _factory.CreateDamage("TestDamage", 5f, damage);
        var initialHealth = _target.Health;

        // Act
        await ability.Execute(_user, _target);

        // Assert
        Assert.AreEqual(initialHealth - damage, _target.Health);
    }

    [Test]
    public async Task AbilityShouldHealCorrectAmount()
    {
        // Arrange
        var healAmount = 30f;
        var ability = new HealAbility("TestHeal", 5f, healAmount);
        _user.Health = 50f; // Set initial health lower
        var initialHealth = _user.Health;

        // Act
        await ability.Execute(_user, _target);

        // Assert
        Assert.AreEqual(initialHealth + healAmount, _user.Health);
    }

    [Test]
    public async Task AbilityShouldApplyEffect()
    {
        // Arrange
        var duration = 5f;
        var damagePerSecond = 10f;
        var ability = new DotAbility("TestDot", 5f, duration, damagePerSecond);

        // Act
        await ability.Execute(_user, _target);

        // Assert
        Assert.That(_target.Effects, Has.Count.EqualTo(1));
        var effect = _target.Effects[0];
        Assert.AreEqual("TestDot", effect.Name);
    }

    [Test]
    public async Task ShouldDealBasePlusBonusDamage()
    {
        // Arrange
        var baseDamage = 20f;
        var additionalDamage = 10f;
        var baseAbility = new DamageAbility("BaseAttack", 5f, baseDamage);
        var ability = new DamageAdditionAbility("EnhancedAttack", 5f, baseAbility, additionalDamage);
        var initialHealth = _target.Health;

        // Act
        await ability.Execute(_user, _target);

        // Assert
        Assert.AreEqual(initialHealth - (baseDamage + additionalDamage), _target.Health);
    }

    [Test]
    public void Ability_ShouldRespectCooldown()
    {
        // Arrange
        var cooldown = 5f;
        var ability = new DamageAbility("TestDamage", cooldown, 20f);

        // Act
        ability.Execute(_user, _target);

        // Assert
    }

    [Test]
    public async Task DotAbility_EffectShouldHaveCorrectValues()
    {
        // Arrange
        var duration = 5f;
        var damagePerSecond = 10f;
        var ability = new DotAbility("TestDot", 5f, duration, damagePerSecond);

        // Act
        await ability.Execute(_user, _target);

        // Assert
    }
}