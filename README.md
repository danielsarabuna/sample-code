# Unity RPG Idle Game Sample

## Overview
This project is a Unity-based RPG game framework that provides a solid foundation for building role-playing games with character progression, enemy AI, pet systems, and more. The framework is designed with modern software development principles in mind, offering a modular, extensible architecture.

**⚠️ NOTE: This project is still in active development. Many features are incomplete or may change significantly. Contributions and feedback are welcome!**

## Features

### Current Implementation
- **Character System**
  - Hero creation with class-based attributes
  - Stats system with progression (health, attack, defense)
  - Experience and level management
  
- **Enemy System**
  - Dynamic enemy generation based on type and level
  - Faction-based organization (Bandits, Beasts, Undead, Elementals, Demons)
  - Customizable behavior parameters
  - Loot table integration

- **Battle System**
  - Turn-based combat mechanics
  - Unit management and state handling
  - Skill and ability execution

- **Progression System**
  - Level-based stat scaling with multiple progression types
  - Skill point allocation
  - Unlockable abilities

### Architecture Highlights
- **Service-oriented Architecture** for maintainable and testable code
  - Core services for game functionality (ProgressionService, InventoryService, etc.)
  - Infrastructure services for technical concerns (DebugService, AnalyticService)
- **MVVM Pattern** for clean separation of UI and game logic
- **Reactive Programming** for responsive UI updates and game state management
- **Dependency Injection** using VContainer for better testability and modularity
- **Factory Pattern** implementation for creating game entities (UnitFactory, ActorModelFactory)
- **Command Pattern** for handling game actions (AddCoinsCommand, OpenUrlCommand)
- **Async/Await Programming** using UniTask for efficient operations
- **Addressable Asset System** for optimized content loading and management

## Technical Details

### Technologies
- Unity Game Engine
- .NET Framework 4.7.1
- C# 9.0
- **UniTask** for async/await support optimized for Unity
- **Addressables** for advanced asset management
- **VContainer** (Dependency Injection framework)
- **MVVM Framework** for UI architecture
- NUnit for testing

### Services Architecture
The project follows a service-oriented approach where each service has a specific responsibility:

#### Core Game Services
- **ProgressionService**: Handles character leveling and stat progression with various calculation methods:
  - Linear progression
  - Exponential progression
  - Logarithmic progression
  - Polynomial progression

```csharp
// From ProgressionService.cs
public sealed class ProgressionService
{
    private readonly Dictionary<string, ICalculator> _calculatorRegistry = new();

    public ProgressionService()
    {
        AddProgressionCalculator("health-value", Polynomial, 25, 1.2F, 0F, 1.2F);
        AddProgressionCalculator("attack-value", Polynomial, 10, 1.3F, 5F, 1.4F);
        AddProgressionCalculator("defense-value", Polynomial, 5, 1.5F, 0F, 1.3F);

        AddProgressionCalculator("health-cost", Exponential, 100, 1.15F, 0F, 0F);
        AddProgressionCalculator("attack-cost", Exponential, 100, 1.15F, 0F, 0F);
        AddProgressionCalculator("defense-cost", Exponential, 100, 1.15F, 0F, 0F);

        AddProgressionCalculator("experience", Exponential, 100, 1.2F, 0F, 0F);
    }

    public TCalculator Get<TCalculator>(string key) where TCalculator : ICalculator
    {
        if (_calculatorRegistry.TryGetValue(key, out var value))
        {
            return (TCalculator)value;
        }

        throw new ArgumentException($"{key}");
    }
}
```

#### Infrastructure Services
- **DebugService**: Manages debug-related functionality
- **AnalyticService**: Collects gameplay metrics and statistics

### Asynchronous Programming with UniTask
The project uses UniTask for efficient asynchronous operations:

```csharp
// Example from DebugService.cs
public async UniTask InitializeAsync(CancellationToken cancellation)
{
    // Initialization logic
}

// Example from command pattern implementation
public UniTask<Status> Execute()
{
    // Command execution logic
}
```

### Project Structure
The codebase follows a modular architecture with clear separation between:

- **Model**: Data structures representing game entities (Hero, Enemy, Pet)
- **View**: UI components and visual representations
- **ViewModel**: Bridges between Views and Models, handling UI logic
- **Service**: Business logic for game systems organized by domain
- **Factory**: Creation logic for complex game objects (UnitFactory, ActorModelFactory)
- **Gameplay**: Implementation of game mechanics and systems (BattleState, BattleSystem)

## Factory Pattern Implementation

### ActorModelFactory

This factory is responsible for creating game models for actors like heroes, enemies, and pets:

```csharp
public class ActorModelFactory
{
    private ProgressionService _progression;

    public ActorModelFactory(IObjectResolver resolver)
    {
        _progression = resolver.Resolve<ProgressionService>();
    }

    public HeroModel CreateHeroModel(IUserGameDetails userGameDetails, string name, HeroClass heroClass)
    {
        var stats = CreateBaseStatsForClass(heroClass);
        var progression = CreateProgressionForHero();
        var model = new HeroModel(name, 1, stats, progression, new List<string>(), new List<string>());
        var stateData = userGameDetails.GameState.StateData;
        var healthLevel = Convert.ToUInt32(stateData["health"]);
        var attackLevel = Convert.ToUInt32(stateData["attack"]);
        var defenseLevel = Convert.ToUInt32(stateData["defense"]);

        model.Data["Health"] = CalculateStatCurrentValue("health", healthLevel);
        model.Data["Attack"] = CalculateStatCurrentValue("attack", attackLevel);
        model.Data["Defense"] = CalculateStatCurrentValue("defense", defenseLevel);
        return model;
    }

    private ulong CalculateStatCurrentValue(string stat, uint level)
    {
        return _progression.Get<IProgressionCalculator>($"{stat}-value").GetValue(level);
    }

    public EnemyModel CreateEnemyModel(string enemyType, int level)
    {
        var enemy = new EnemyModel($"{enemyType}_{level}", level, CreateBehaviorForType(enemyType),
            CreateLootTableForType(enemyType, level), DetermineFaction(enemyType));
        return enemy;
    }

    private string DetermineFaction(string enemyType)
    {
        return enemyType.ToLower() switch
        {
            "goblin" or "orc" or "bandit" => "Bandit", // Humanoid enemies
            "wolf" or "bear" or "spider" => "Beast", // Wild beasts
            "skeleton" or "zombie" or "lich" => "Undead", // Undead
            "fire elemental" or "ice elemental" or "storm elemental" => "Elemental", // Elementals
            "imp" or "demon lord" or "succubus" => "Demon", // Demons
            _ => "Enemy"
        };
    }

    public PetModel CreatePetModel(PetType type, string ownerID)
    {
        var petStats = new PetStats
        {
            Loyalty = 0,
            BondLevel = 0,
            SpecialAbilities = GetInitialAbilitiesForPetType(type)
        };
        return new PetModel("", 1, petStats, string.Empty, type, GrowthStage.Young);
    }
}
```

### UnitFactory

This factory creates game units that connect models with their visual representations:

```csharp
public class UnitFactory
{
    UnitFactory(IObjectResolver resolver)
    {
        // Initialization
    }
    
    public async UniTask<PlayerUnit> CreatePlayer(PlayerEntityView view)
    {
        // Creates a player unit by connecting the model with the view
    }
    
    public async UniTask<EnemyUnit> CreateEnemy(EnemyModel model)
    {
        // Creates an enemy unit using the provided model
    }
}
```

## Progression System

The `ProgressionService` provides various calculation methods for character progression:

```csharp
public enum ProgressionType
{
    Linear,      // Linear progression
    Exponential, // Geometric progression
    Logarithmic, // Logarithmic progression
    Polynomial   // Polynomial progression
}

public class ProgressionCalculator : IProgressionCalculator
{
    private readonly ProgressionConfig _config;

    public ProgressionCalculator(ProgressionConfig config)
    {
        _config = config;
    }

    public ulong GetValue(ulong level)
    {
        var calculatedValue = _config.Type switch
        {
            Linear => _config.BaseValue + _config.GrowthFactor * level,
            Exponential => _config.BaseValue * (float)Math.Pow(_config.GrowthFactor, level),
            Logarithmic => _config.BaseValue * (float)Math.Log(level + _config.Offset),
            Polynomial => _config.BaseValue * (float)Math.Pow(level, _config.Power) + _config.Offset,
            _ => throw new ArgumentOutOfRangeException()
        };
        return ToUInt64(calculatedValue);
    }
}
```

## Command Pattern with UniTask

Several commands in the project use UniTask for asynchronous execution:

```csharp
// OpenUrlCommand.cs
public UniTask<Status> Execute()
{
    // Opens URL and returns status
}

// AddCoinsCommand.cs
public UniTask<Status> Execute()
{
    // Adds coins to player account and returns status
}

// SetGraphicCommand.cs
UniTask IServe.Run()
{
    // Changes graphic settings
}
```

## Services with Asynchronous Initialization

Services in the project implement asynchronous initialization:

```csharp
// DebugService.cs
public async UniTask InitializeAsync(CancellationToken cancellation)
{
    // Initialization logic
}

// AnalyticService.cs
UniTask IService.InitializeAsync(CancellationToken cancellation)
{
    // Service initialization logic
    return UniTask.CompletedTask;
}
```

## Actor Model Structure

The project uses a robust actor model system for game entities:

```csharp
// HeroModel class structure
public class HeroModel
{
    public string Name { get; }
    public int Level { get; }
    public Stats Stats { get; }
    public Progression Progression { get; }
    public List<string> Inventory { get; }
    public List<string> Equipment { get; }
    public Dictionary<string, ulong> Data { get; }
    
    // Constructor and methods
}

// EnemyModel class structure
public class EnemyModel
{
    public string Name { get; }
    public int Level { get; }
    public BehaviorParams Behavior { get; }
    public LootTable LootTable { get; }
    public string Faction { get; }
    
    // Constructor and methods
}

// PetModel class structure
public class PetModel
{
    public string Name { get; }
    public int Level { get; }
    public PetStats Stats { get; }
    public string OwnerId { get; }
    public PetType Type { get; }
    public GrowthStage Stage { get; }
    
    // Constructor and methods
}
```

## Faction-Based Enemy System

The project implements a faction-based system for enemies:

```csharp
private string DetermineFaction(string enemyType)
{
    return enemyType.ToLower() switch
    {
        "goblin" or "orc" or "bandit" => "Bandit",       // Humanoid enemies
        "wolf" or "bear" or "spider" => "Beast",         // Wild beasts
        "skeleton" or "zombie" or "lich" => "Undead",    // Undead
        "fire elemental" or "ice elemental" or "storm elemental" => "Elemental", // Elementals
        "imp" or "demon lord" or "succubus" => "Demon",  // Demons
        _ => "Enemy"
    };
}
```

## Battle System Structure

The battle system manages combat encounters between player units and enemies:

```csharp
// BattleState class (partial)
public class BattleState : IGameState
{
    private readonly BattleSystem _battleSystem;
    
    // Methods to manage the battle state
    public void Enter()
    {
        // Initialize battle
    }
    
    public void Exit()
    {
        // Clean up after battle
    }
    
    // Battle flow control methods
}

// BattleSystem class (partial)
public class BattleSystem
{
    private List<PlayerUnit> _playerUnits;
    private List<EnemyUnit> _enemyUnits;
    
    // Battle mechanics methods
    public void InitiateCombat()
    {
        // Start combat sequence
    }
    
    public void ExecuteTurn()
    {
        // Process a single turn in combat
    }
    
    public void ApplyDamage(Unit attacker, Unit defender, int damage)
    {
        // Damage calculation and application
    }
}
```

## State Machine for Game Flow

The project uses a state machine to manage game flow:

```csharp
// GameMachine states
public enum GameState
{
    MainMenu,
    Exploration,
    Battle,
    Dialogue,
    Inventory,
    CharacterScreen,
    Shop,
    LoadingScreen,
    GameOver
}

// BattleState implementation
public class BattleState : IGameState
{
    public void Enter()
    {
        // Initialize the battle state
    }
    
    public void Exit()
    {
        // Clean up when leaving the battle state
    }
    
    public void Update()
    {
        // Handle battle state updates
    }
}
```

## Integration with Unity's Components

The project integrates the service architecture with Unity's component system:

```csharp
// Example MonoBehaviour using services
public class GameplayController : MonoBehaviour
{
    private ProgressionService _progressionService;
    private UnitFactory _unitFactory;
    
    [Inject]
    public void Construct(IObjectResolver resolver)
    {
        _progressionService = resolver.Resolve<ProgressionService>();
        _unitFactory = resolver.Resolve<UnitFactory>();
    }
    
    private async void Start()
    {
        // Initialize gameplay
        var playerUnit = await _unitFactory.CreatePlayer(_playerView);
    }
}
```

## Usage Examples

### Using the Progression System:
```csharp
// Get a progression calculator for a specific stat
var healthCalculator = _progressionService.Get<IProgressionCalculator>("health-value");

// Calculate health for a given level
ulong level = 5;
ulong health = healthCalculator.GetValue(level);
```

### Implementing Commands with UniTask:
```csharp
// From AddCoinsCommand.cs
public UniTask<Status> Execute()
{
    // Command execution logic
    return UniTask.FromResult(Status.Success);
}
```

### Service Initialization:
```csharp
// From AnalyticService.cs
UniTask IService.InitializeAsync(CancellationToken cancellation)
{
    // Service initialization logic
    return UniTask.CompletedTask;
}
```

### Creating Game Entities:
```csharp
// Create a hero model
var heroModel = _actorModelFactory.CreateHeroModel(userDetails, "Arwen", HeroClass.Ranger);

// Create an enemy
var goblinModel = _actorModelFactory.CreateEnemyModel("goblin", 3);

// Create a battle unit
var enemyUnit = await _unitFactory.CreateEnemy(goblinModel);
```

### Implementing Game States:
```csharp
// Switch to battle state
_gameMachine.SwitchState(GameState.Battle);

// Handle state transitions
public void OnBattleComplete(BattleResult result)
{
    if (result.IsVictory)
    {
        _gameMachine.SwitchState(GameState.Exploration);
    }
    else
    {
        _gameMachine.SwitchState(GameState.GameOver);
    }
}
```

## Roadmap

- [ ] Complete service architecture implementation
- [ ] Add more specialized services (AI, Weather, Economy)
- [ ] Implement combat system with CombatService
- [ ] Build inventory and equipment systems
- [ ] Develop quest and dialogue systems
- [ ] Create UI components with MVVM architecture
- [ ] Add save/load functionality with Addressables
- [ ] Optimize asynchronous operations with UniTask
- [ ] Expand enemy AI behavior options
- [ ] Implement multiplayer capabilities through NetworkService
- [ ] Add more progression options and skill trees
- [ ] Improve asset loading and management with Addressables
- [ ] Enhance UI responsiveness with reactive programming
- [ ] Add telemetry and analytics services

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments
- The Unity community for their invaluable resources
- Cysharp for UniTask library
- VContainer developers for their excellent dependency injection framework
- Unity Technologies for Addressables system
- All contributors who have helped shape this project

---

**Note:** This is a work in progress with many features still under development or in planning stages. The API and architecture may change significantly as the project evolves.