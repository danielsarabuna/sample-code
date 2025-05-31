using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay;
using Model;
using UnityEditor;
using UnityEngine;

public class EquipmentTab : BaseTable<EquipmentModel>
{
    protected override string SaveFilePath => "Assets/Data/equipment-collection.json";

    protected override void DrawItemDetailsPanel()
    {
        if (Selected == null) return;
        EditorGUILayout.BeginVertical();
        EditorGUI.indentLevel++;

        switch (Selected)
        {
            case AccessoryModel accessoryModel:
            {
                DrawAccessory(accessoryModel);
                break;
            }

            case ArmorModel armorModel:
            {
                DrawArmor(armorModel);
                break;
            }
            case PetEquipmentModel petEquipmentModel:
            {
                DrawPetEquipment(petEquipmentModel);
                break;
            }

            case WeaponModel weaponModel:
            {
                DrawWeapon(weaponModel);
                break;
            }

            case ArtifactModel artifactModel:
            {
                DrawArtifact(artifactModel);
                break;
            }
        }

        if (GUILayout.Button("Save Item", GUILayout.Height(25)))
            Save();

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    protected override EquipmentModel CreateNewItem(Type type)
    {
        var equipmentModel = base.CreateNewItem(type);
        equipmentModel.Name = "New " + FormatGroupName(type.Name);
        equipmentModel.ID = Guid.NewGuid().ToString();
        return equipmentModel;
    }

    protected override void DrawItemTree(EquipmentModel element)
    {
        if (GUILayout.Button(element.Name, _itemStyle))
            Selected = element;
    }

    protected override bool IsMatchForSearch(EquipmentModel element, string search)
    {
        return element.Name.ToLower().Contains(search) || element.Description.ToLower().Contains(search);
    }
    
    protected override List<EquipmentModel> GetFromJson(string json)
    {
        var collection = EquipmentCollection.FromJson(json);
        return collection?.Models == null
            ? new List<EquipmentModel>()
            : collection.Models.ToList();
    }

    protected override string GetToJson()
    {
        var collection = new EquipmentCollection(AllItems.ToArray());
        return collection.ToJson();
    }


    private Sprite _spriteIcon, _spriteAssetPath;

    private void DrawBaseEquipmentProperties(EquipmentModel equipment)
    {
        EditorGUILayout.LabelField("ID:", equipment.ID);
        equipment.Name = EditorGUILayout.TextField("Name:", equipment.Name);
        if (string.IsNullOrEmpty(equipment.Icon))
        {
            _spriteIcon = (Sprite)EditorGUILayout.ObjectField("Icon:", _spriteIcon, typeof(Sprite), false,
                GUILayout.Height(40));
            if (_spriteIcon != null)
            {
                equipment.Icon = _spriteIcon.name;
                _spriteIcon = null;
            }
        }
        else
        {
            equipment.Icon = EditorGUILayout.TextField("Icon:", equipment.Icon);
        }

        if (string.IsNullOrEmpty(equipment.AssetPath))
        {
            _spriteAssetPath = (Sprite)EditorGUILayout.ObjectField("Asset:", _spriteAssetPath, typeof(Sprite),
                false,
                GUILayout.Height(40));
            if (_spriteAssetPath != null)
            {
                equipment.AssetPath = _spriteAssetPath.name;
                _spriteAssetPath = null;
            }
        }
        else
        {
            equipment.AssetPath = EditorGUILayout.TextField("AssetPath:", equipment.AssetPath);
        }

        equipment.Level = (uint)EditorGUILayout.IntField("Level:", (int)equipment.Level);
        equipment.Rarity = (EquipmentRarity)EditorGUILayout.EnumPopup("Rarity:", equipment.Rarity);
        equipment.Description = EditorGUILayout.TextArea(equipment.Description, GUILayout.Height(60));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

        equipment.Effects ??= new List<EquipmentEffectModel>();

        EditorGUI.indentLevel++;
        for (var i = 0; i < equipment.Effects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField($"Effect {i + 1}");

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                equipment.Effects.RemoveAt(i);
                i--;
                continue;
            }

            EditorGUILayout.EndHorizontal();

            var effect = equipment.Effects[i];
            effect.EffectType = (EquipmentEffectType)EditorGUILayout.EnumPopup("Type:", effect.EffectType);
            effect.Value = EditorGUILayout.FloatField("Value:", effect.Value);
            effect.Duration = EditorGUILayout.FloatField("Duration:", effect.Duration);
            effect.Description = EditorGUILayout.TextArea(effect.Description, GUILayout.Height(60));

            EditorGUILayout.Space();
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Add Effect"))
            equipment.Effects.Add(new EquipmentEffectModel(EquipmentEffectType.None, 1, 1));
    }

    private void DrawWeapon(WeaponModel weapon)
    {
        EditorGUILayout.BeginVertical("box");

        DrawBaseEquipmentProperties(weapon);
        EditorGUILayout.Space();

        weapon.Type = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type:", weapon.Type);
        weapon.Damage = EditorGUILayout.FloatField("Damage:", weapon.Damage);
        weapon.AttackSpeed = EditorGUILayout.FloatField("Attack Speed:", weapon.AttackSpeed);

        EditorGUILayout.EndVertical();
    }

    private void DrawArmor(ArmorModel armor)
    {
        EditorGUILayout.BeginVertical("box");

        DrawBaseEquipmentProperties(armor);
        EditorGUILayout.Space();

        armor.Type = (ArmorType)EditorGUILayout.EnumPopup("Armor Type:", armor.Type);
        armor.Defense = EditorGUILayout.FloatField("Defense:", armor.Defense);
        armor.HealthBonus = EditorGUILayout.FloatField("Health Bonus:", armor.HealthBonus);

        EditorGUILayout.EndVertical();
    }

    private void DrawAccessory(AccessoryModel accessory)
    {
        EditorGUILayout.BeginVertical("box");

        DrawBaseEquipmentProperties(accessory);
        EditorGUILayout.Space();

        accessory.Type = (AccessoryType)EditorGUILayout.EnumPopup("Accessory Type:", accessory.Type);
        accessory.UniqueAbility = EditorGUILayout.TextField("Unique Ability:", accessory.UniqueAbility);

        EditorGUILayout.EndVertical();
    }

    private void DrawPetEquipment(PetEquipmentModel petEquipment)
    {
        EditorGUILayout.BeginVertical("box");

        DrawBaseEquipmentProperties(petEquipment);
        EditorGUILayout.Space();

        petEquipment.Type = (PetEquipmentType)EditorGUILayout.EnumPopup("Pet Equipment Type:", petEquipment.Type);

        EditorGUILayout.EndVertical();
    }

    private void DrawArtifact(ArtifactModel artifact)
    {
        EditorGUILayout.BeginVertical("box");

        DrawBaseEquipmentProperties(artifact);
        EditorGUILayout.Space();

        artifact.UniqueAbility = EditorGUILayout.TextField("Unique Ability:", artifact.UniqueAbility);

        EditorGUILayout.EndVertical();
    }
}