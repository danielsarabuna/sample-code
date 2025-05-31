using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Model.Actor;
using UnityEditor;
using UnityEngine;

public class ActorTable : BaseTable<ActorModel>
{
    protected override string SaveFilePath => "Assets/Data/actor-collection.json";

    protected override void DrawItemDetailsPanel()
    {
        if (Selected == null) return;

        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;

        DrawBaseEquipmentProperties(Selected);
        switch (Selected)
        {
            case EnemyModel enemy:
                DrawEnemyProperties(enemy);
                break;

            case NpcModel npc:
                DrawNpcProperties(npc);
                break;

            case PetModel pet:
                DrawPetProperties(pet);
                break;

            default:
                EditorGUILayout.LabelField("Unknown actor type");
                break;
        }

        DrawActorData(Selected);

        if (GUILayout.Button("Save Item", GUILayout.Height(25)))
            Save();

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    private void DrawBaseEquipmentProperties(ActorModel model)
    {
        EditorGUILayout.LabelField("ID:", model.ID);
        model.Name = EditorGUILayout.TextField("Name:", model.Name);
    }

    private void DrawEnemyProperties(EnemyModel enemy)
    {
        enemy.Level = EditorGUILayout.IntField("Level:", enemy.Level);
        enemy.Faction = EditorGUILayout.TextField("Faction:", enemy.Faction);

        EditorGUILayout.LabelField("Behavior Parameters", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        var behavior = enemy.Behavior;
        behavior.AggroRange = EditorGUILayout.FloatField("Aggro Range:", behavior.AggroRange);
        behavior.AttackRange = EditorGUILayout.FloatField("Attack Range:", behavior.AttackRange);
        behavior.IsAggressive = EditorGUILayout.Toggle("Is Aggressive:", behavior.IsAggressive);
        DrawStringList("Preferred Targets", behavior.PreferredTargets);
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Loot", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        enemy.Loot.ExperienceValue = EditorGUILayout.FloatField("Experience Value:", enemy.Loot.ExperienceValue);
        DrawLootTable(enemy.Loot);
        EditorGUI.indentLevel--;
    }

    private void DrawNpcProperties(NpcModel npc)
    {
        npc.Level = EditorGUILayout.IntField("Level:", npc.Level);
        npc.Role = (NpcType)EditorGUILayout.EnumPopup("NPC Role:", npc.Role);

        EditorGUILayout.LabelField("Trade Info", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        if (npc.TradeData != null)
        {
            npc.TradeData.PriceMultiplier =
                EditorGUILayout.FloatField("Price Multiplier:", npc.TradeData.PriceMultiplier);
            npc.TradeData.SpecialCurrency =
                EditorGUILayout.TextField("Special Currency:", npc.TradeData.SpecialCurrency);
            DrawStringList("Available Items", npc.TradeData.AvailableItems);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Quest Info", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        if (npc.QuestData != null)
        {
            DrawStringList("Available Quests", npc.QuestData.AvailableQuests);
            DrawStringList("Completed Quests", npc.QuestData.CompletedQuests);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("Dialogue", EditorStyles.boldLabel);
        DrawStringList("Dialogue Tree", npc.DialogueTree);
    }

    private void DrawPetProperties(PetModel pet)
    {
        pet.Level = EditorGUILayout.IntField("Level:", pet.Level);
        pet.PetType = (PetType)EditorGUILayout.EnumPopup("Pet Type:", pet.PetType);
        pet.Stage = (GrowthStage)EditorGUILayout.EnumPopup("Growth Stage:", pet.Stage);
        pet.OwnerID = EditorGUILayout.TextField("Owner ID:", pet.OwnerID);

        EditorGUILayout.LabelField("Pet Stats", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        if (pet.Stats != null)
        {
            pet.Stats.Loyalty = EditorGUILayout.FloatField("Loyalty:", pet.Stats.Loyalty);
            pet.Stats.BondLevel = EditorGUILayout.FloatField("Bond Level:", pet.Stats.BondLevel);
            DrawStringList("Special Abilities", pet.Stats.SpecialAbilities);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawLootTable(LootTable lootTable)
    {
        if (lootTable?.PossibleLoot == null) return;

        EditorGUILayout.LabelField("Possible Loot Items:");
        for (int i = 0; i < lootTable.PossibleLoot.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            var lootEntry = lootTable.PossibleLoot[i];
            lootEntry.ItemId = EditorGUILayout.TextField("Item ID:", lootEntry.ItemId);
            lootEntry.DropChance = EditorGUILayout.Slider("Drop Chance:", lootEntry.DropChance, 0f, 1f);
            lootEntry.MinQuantity = EditorGUILayout.IntField("Min:", lootEntry.MinQuantity);
            lootEntry.MaxQuantity = EditorGUILayout.IntField("Max:", lootEntry.MaxQuantity);

            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                lootTable.PossibleLoot.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Loot Entry"))
        {
            lootTable.PossibleLoot.Add(new LootEntry());
        }
    }

    private void DrawActorData(ActorModel actor)
    {
        if (actor.Data == null) return;

        EditorGUILayout.LabelField("Additional Data", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        var keysToRemove = new List<string>();
        var dataToAdd = new Dictionary<string, object>();

        foreach (var pair in actor.Data)
        {
            if (pair.Value is List<string> stringList)
            {
                EditorGUILayout.LabelField(pair.Key, EditorStyles.boldLabel);
                DrawStringList("", stringList);
            }
            else if (pair.Value is Dictionary<string, float> floats)
            {
                EditorGUILayout.LabelField(pair.Key, EditorStyles.boldLabel);
                DrawDictionary(floats);
            }
            else if (pair.Value is Dictionary<string, string> strings)
            {
                EditorGUILayout.LabelField(pair.Key, EditorStyles.boldLabel);
                DrawDictionary(strings);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(pair.Key);

                if (pair.Value is float floatValue)
                {
                    dataToAdd[pair.Key] = EditorGUILayout.FloatField(floatValue);
                }
                else if (pair.Value is int intValue)
                {
                    dataToAdd[pair.Key] = EditorGUILayout.IntField(intValue);
                }
                else if (pair.Value is string stringValue)
                {
                    dataToAdd[pair.Key] = EditorGUILayout.TextField(stringValue);
                }
                else if (pair.Value is bool boolValue)
                {
                    dataToAdd[pair.Key] = EditorGUILayout.Toggle(boolValue);
                }

                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    keysToRemove.Add(pair.Key);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        foreach (var key in keysToRemove)
        {
            actor.Data.Remove(key);
        }

        foreach (var pair in dataToAdd)
        {
            actor.Data[pair.Key] = pair.Value;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add String"))
        {
            actor.Data["New String"] = "";
        }

        if (GUILayout.Button("Add Number"))
        {
            actor.Data["New Number"] = 0f;
        }

        if (GUILayout.Button("Add Bool"))
        {
            actor.Data["New Bool"] = false;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;
    }

    private void DrawDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        var modifications = new List<(TKey key, TValue newValue)>();
        var removals = new List<TKey>();

        foreach (var kvp in dictionary.ToList())
        {
            EditorGUILayout.BeginHorizontal();

            if (kvp.Value is string stringValue)
            {
                var newString = EditorGUILayout.TextField(kvp.Key.ToString(), stringValue);
                if (newString != stringValue)
                {
                    modifications.Add((kvp.Key, (TValue)(object)newString));
                }
            }

            if (kvp.Value is float floatValue)
            {
                var newString = EditorGUILayout.FloatField(kvp.Key.ToString(), floatValue);
                if (!Mathf.Approximately(newString, floatValue))
                {
                    modifications.Add((kvp.Key, (TValue)(object)newString));
                }
            }

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                removals.Add(kvp.Key);
            }

            EditorGUILayout.EndHorizontal();
        }

        foreach (var mod in modifications)
        {
            dictionary[mod.key] = mod.newValue;
        }

        foreach (var key in removals)
        {
            dictionary.Remove(key);
        }
    }

    private void DrawStringList(string label, List<string> list)
    {
        if (!string.IsNullOrEmpty(label)) EditorGUILayout.LabelField(label);
        EditorGUI.indentLevel++;
        for (var i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            list[i] = EditorGUILayout.TextField(list[i]);
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                list.RemoveAt(i);
                i--;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add New"))
        {
            list.Add("");
        }

        EditorGUI.indentLevel--;
    }

    protected override ActorModel CreateNewItem(Type type)
    {
        var actorModel = base.CreateNewItem(type);
        actorModel.Name = "New " + FormatGroupName(type.Name);
        actorModel.ID = Guid.NewGuid().ToString();
        return actorModel;
    }

    protected override List<ActorModel> GetFromJson(string json)
    {
        var collection = ActorCollection.FromJson(json);
        return collection?.Models == null
            ? GetSpecialEnemies()
            : collection.Models.ToList();
    }

    protected override string GetToJson()
    {
        var collection = new ActorCollection(AllItems.ToArray());
        return collection.ToJson();
    }

    protected override void DrawItemTree(ActorModel element)
    {
        if (GUILayout.Button(element.Name, _itemStyle))
            Selected = element;
    }

    protected override bool IsMatchForSearch(ActorModel element, string search)
    {
        return element.Name.ToLower().Contains(search);
    }

    private static List<ActorModel> GetSpecialEnemies()
    {
        return new List<ActorModel>
        {
            // 1. Неудержимый танк
            new EnemyModel
            {
                ID = Guid.NewGuid().ToString(),
                Name = "Juggernaut",

                Behavior = new BehaviorParams
                {
                    AggroRange = 10f,
                    AttackRange = 3f,
                    AttackCooldown = 3.5f,
                    SpecialAbilities = new List<string>
                    {
                        "Unstoppable",
                        "ShieldCharge",
                        "GroundSmash"
                    }
                },

                Loot = new LootTable
                {
                    PossibleLoot = new List<LootEntry>
                    {
                        new LootEntry
                        {
                            ItemId = "HeavyPlate",
                            DropChance = 0.4f,
                            MinQuantity = 1,
                            MaxQuantity = 3
                        },
                        new LootEntry
                        {
                            ItemId = "BruteCore",
                            DropChance = 0.2f,
                            MinQuantity = 1,
                            MaxQuantity = 1
                        },
                        new LootEntry
                        {
                            ItemId = "ReinforedShield",
                            DropChance = 0.3f,
                            MinQuantity = 1,
                            MaxQuantity = 1
                        }
                    }
                },

                Data = new Dictionary<string, object>
                {
                    { "Health", 8000 },
                    { "Damage", 200 },
                    { "Speed", 1.2F },
                    { "DetectionRange", 12F },
                    { "Type", "Elite" },
                    { "ExperienceReward", 800 },
                    { "PrefabPath", "Prefabs/Enemies/Juggernaut" },
                    { "Description", "Unstoppable force that gains momentum while moving" },
                    { "Tags", new List<string> { "Heavy", "Armored", "Elite" } },
                    {
                        "Resistances", new Dictionary<string, float>
                        {
                            { "Physical", 0.7f },
                            { "Knockback", 0.9f },
                            { "Stun", 0.8f }
                        }
                    },
                }
            },

            // 2. Паразит с механикой заражения
            new EnemyModel
            {
                ID = Guid.NewGuid().ToString(),
                Name = "Parasite",

                Behavior = new BehaviorParams
                {
                    AggroRange = 7f,
                    AttackRange = 1.5f,
                    AttackCooldown = 1f,
                    SpecialAbilities = new List<string>
                    {
                        "Infect",
                        "SpawnSpores",
                        "LifeDrain"
                    }
                },

                Loot = new LootTable
                {
                    PossibleLoot = new List<LootEntry>
                    {
                        new LootEntry
                        {
                            ItemId = "InfectedTissue",
                            DropChance = 0.6f,
                            MinQuantity = 1,
                            MaxQuantity = 3
                        },
                        new LootEntry
                        {
                            ItemId = "ParasiteEgg",
                            DropChance = 0.3f,
                            MinQuantity = 1,
                            MaxQuantity = 2
                        },
                        new LootEntry
                        {
                            ItemId = "ToxicExtract",
                            DropChance = 0.4f,
                            MinQuantity = 1,
                            MaxQuantity = 2
                        }
                    }
                },

                Data = new Dictionary<string, object>
                {
                    { "Health", 400 },
                    { "Damage", 45 },
                    { "Speed", 3.5F },
                    { "DetectionRange", 8F },
                    { "Type", "Special" },
                    { "ExperienceReward", 150 },
                    { "Id", "special_parasite" },
                    { "PrefabPath", "Prefabs/Enemies/Parasite" },
                    { "Description", "Infects other creatures and spreads disease" },
                    { "Tags", new List<string> { "Infection", "Poison", "Stealth" } },
                    {
                        "VisualEffects", new Dictionary<string, string>
                        {
                            { "OnInfect", "InfectionCloud" },
                            { "OnDeath", "SporeExplosion" }
                        }
                    }
                },
            },

            // 3. Мимик-обманщик
            new EnemyModel
            {
                ID = Guid.NewGuid().ToString(),
                Name = "Mimic",

                Behavior = new BehaviorParams
                {
                    AggroRange = 3f,
                    AttackRange = 2f,
                    AttackCooldown = 2f,
                    SpecialAbilities = new List<string>
                    {
                        "Disguise",
                        "Ambush",
                        "ChainGrab"
                    }
                },

                Loot = new LootTable
                {
                    PossibleLoot = new List<LootEntry>
                    {
                        new LootEntry
                        {
                            ItemId = "RareItem",
                            DropChance = 0.4f,
                            MinQuantity = 1,
                            MaxQuantity = 2
                        },
                        new LootEntry
                        {
                            ItemId = "MimicTooth",
                            DropChance = 0.3f,
                            MinQuantity = 1,
                            MaxQuantity = 3
                        },
                        new LootEntry
                        {
                            ItemId = "GoldCoin",
                            DropChance = 1.0f,
                            MinQuantity = 50,
                            MaxQuantity = 100
                        }
                    }
                },

                Data = new Dictionary<string, object>
                {
                    { "Health", 600 },
                    { "Damage", 120 },
                    { "Speed", 4f },
                    { "DetectionRange", 5f },
                    { "Type", "Trap" },
                    { "ExperienceReward", 200 },
                    { "Id", "trap_mimic" },
                    { "PrefabPath", "Prefabs/Enemies/Mimic" },
                    { "Description", "Disguises as a treasure chest to ambush prey" },
                    { "Tags", new List<string> { "Trap", "Treasure", "Shapeshifter" } },
                },
            },

            // 4. Король крыс
            new EnemyModel
            {
                ID = Guid.NewGuid().ToString(),
                Name = "Rat King",

                Behavior = new BehaviorParams
                {
                    AggroRange = 12f,
                    AttackRange = 3f,
                    AttackCooldown = 2.5f,
                    SpecialAbilities = new List<string>
                    {
                        "SummonRats",
                        "PlagueCloud",
                        "RatSwarm",
                        "GnawingFrenzy"
                    }
                },

                Loot = new LootTable
                {
                    PossibleLoot = new List<LootEntry>
                    {
                        new LootEntry
                        {
                            ItemId = "RatKingCrown",
                            DropChance = 0.1f,
                            MinQuantity = 1,
                            MaxQuantity = 1
                        },
                        new LootEntry
                        {
                            ItemId = "PlagueShard",
                            DropChance = 0.5f,
                            MinQuantity = 2,
                            MaxQuantity = 4
                        },
                        new LootEntry
                        {
                            ItemId = "RatTail",
                            DropChance = 1.0f,
                            MinQuantity = 5,
                            MaxQuantity = 10
                        }
                    }
                },

                Data = new Dictionary<string, object>
                {
                    { "Health", 2000 },
                    { "Damage", 75 },
                    { "Speed", 2.8f },
                    { "DetectionRange", 15f },
                    { "Type", "Boss" },
                    { "ExperienceReward", 1000 },
                    { "PrefabPath", "Prefabs/Enemies/RatKing" },
                    { "Description", "Ancient rat monarch commanding hordes of vermin" },
                    { "Tags", new List<string> { "Boss", "Swarm", "Plague" } },
                    {
                        "Resistances", new Dictionary<string, float>
                        {
                            { "Poison", 1.0f },
                            { "Disease", 1.0f },
                            { "Physical", 0.3f }
                        }
                    },
                },
            },

            // 5. Красный скорпион
            new EnemyModel
            {
                ID = Guid.NewGuid().ToString(),
                Name = "Red Scorpion",

                Behavior = new BehaviorParams
                {
                    AggroRange = 8f,
                    AttackRange = 2.5f,
                    AttackCooldown = 3f,
                    SpecialAbilities = new List<string>
                    {
                        "VenomStrike",
                        "PincerGrab",
                        "BurrowAmbush"
                    }
                },

                Loot = new LootTable
                {
                    PossibleLoot = new List<LootEntry>
                    {
                        new LootEntry
                        {
                            ItemId = "RedVenom",
                            DropChance = 0.4f,
                            MinQuantity = 1,
                            MaxQuantity = 2
                        },
                        new LootEntry
                        {
                            ItemId = "ScorpionShell",
                            DropChance = 0.6f,
                            MinQuantity = 1,
                            MaxQuantity = 3
                        },
                        new LootEntry
                        {
                            ItemId = "PoisonSac",
                            DropChance = 0.3f,
                            MinQuantity = 1,
                            MaxQuantity = 1
                        }
                    }
                },

                Data = new Dictionary<string, object>
                {
                    { "Health", 800 },
                    { "Damage", 150 },
                    { "Speed", 2.5f },
                    { "DetectionRange", 10f },
                    { "Type", "Elite" },
                    { "ExperienceReward", 300 },
                    { "Id", "elite_red_scorpion" },
                    { "PrefabPath", "Prefabs/Enemies/RedScorpion" },
                    { "Description", "Deadly desert predator with potent venom" },
                    { "Tags", new List<string> { "Poison", "Ambusher", "Desert" } },
                    {
                        "Resistances", new Dictionary<string, float>
                        {
                            { "Fire", 0.5f },
                            { "Poison", 0.8f },
                            { "Physical", 0.4f }
                        }
                    },
                },
            }
        };
    }
}