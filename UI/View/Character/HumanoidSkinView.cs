using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace View
{
    public class HumanoidSkinView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _head;
        [SerializeField] private SpriteRenderer _ears;
        [SerializeField] private SpriteRenderer _hair;
        [SerializeField] private SpriteRenderer _eyebrows;
        [SerializeField] private SpriteRenderer _eyes;
        [SerializeField] private SpriteRenderer _mouth;
        [SerializeField] private SpriteRenderer _beard;
        [SerializeField] private List<SpriteRenderer> _body;
        [SerializeField] private SpriteRenderer _helmet;
        [SerializeField] private SpriteRenderer _glasses;
        [SerializeField] private SpriteRenderer _mask;
        [SerializeField] private SpriteRenderer _earrings;
        [SerializeField] private SpriteRenderer _primaryMeleeWeapon;
        [SerializeField] private SpriteRenderer _secondaryMeleeWeapon;
        [SerializeField] private List<SpriteRenderer> _armor;
        [SerializeField] private SpriteRenderer _cape;
        [SerializeField] private SpriteRenderer _back;
        [SerializeField] private SpriteRenderer _shield;
        [SerializeField] private List<SpriteRenderer> _bow;

        private void Reset()
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            var renderers = new Dictionary<string, SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
                renderers[spriteRenderer.gameObject.name] = spriteRenderer;

            _head = renderers.GetValueOrDefault("Head");
            _ears = renderers.GetValueOrDefault("Ears");
            _hair = renderers.GetValueOrDefault("Hair");
            _eyebrows = renderers.GetValueOrDefault("Eyebrows");
            _eyes = renderers.GetValueOrDefault("Eyes");
            _mouth = renderers.GetValueOrDefault("Mouth");
            _beard = renderers.GetValueOrDefault("Beard");

            _helmet = renderers.GetValueOrDefault("Helmet");
            _glasses = renderers.GetValueOrDefault("Glasses");
            _mask = renderers.GetValueOrDefault("Mask");
            _earrings = renderers.GetValueOrDefault("Earrings");

            _cape = renderers.GetValueOrDefault("Cape");
            _back = renderers.GetValueOrDefault("Back");
            _shield = renderers.GetValueOrDefault("Shield");

            _armor = spriteRenderers.Where(x => x.gameObject.name.Contains("[Armor]")).ToList();

            // Special

            var bowItemIdentifiers = new List<string> { "Riser", "Limb[1]", "Limb[2]", "Arrow" };
            _bow = spriteRenderers.Where(x => bowItemIdentifiers.Contains(x.gameObject.name)).ToList();

            var bodyItemIdentifiers = new List<string>
            {
                "ArmL", "ArmR",
                "ArmL[1]", "ArmL[2]",
                "ArmR[1]", "ArmR[2]",
                "Finger",
                "ForearmL", "ForearmR",
                "ForearmR[1]", "ForearmR[2]",
                "ForearmL[1]", "ForearmL[2]",
                "HandL", "HandR",
                "Leg[Armor]",
                "Pelvis", "Shin", "Torso",
            };
            _body = spriteRenderers.Where(x => bodyItemIdentifiers.Contains(x.gameObject.name)).ToList();
        }

        public void ApplyEquipment(EquipmentSlot slot, List<Sprite> sprites)
        {
            switch (slot)
            {
                case EquipmentSlot.Gloves:
                    var gloves = _armor.Where(x =>
                            x.name.Contains("HandR") ||
                            x.name.Contains("HandL") ||
                            x.name.Contains("Finger"))
                        .ToArray();
                    MapSprites(sprites, gloves);
                    break;
                case EquipmentSlot.Helmet:
                    _helmet.sprite = sprites is { Count: > 0 } ? sprites.First() : null;
                    break;
                case EquipmentSlot.Chest:
                    var chest = _armor.Where(x =>
                            x.name.Contains("ArmL") ||
                            x.name.Contains("ArmR") ||
                            x.name.Contains("ForearmL") ||
                            x.name.Contains("ForearmR") ||
                            x.name.Contains("Pelvis") ||
                            x.name.Contains("SleeveR") ||
                            x.name.Contains("Torso"))
                        .ToArray();
                    MapSprites(sprites, chest);
                    break;
                case EquipmentSlot.Legs:
                    MapSprites(sprites, _armor.Where(x =>
                            x.name.Contains("Leg") ||
                            x.name.Contains("Shin"))
                        .ToArray());
                    break;
                case EquipmentSlot.RightHand:
                    _primaryMeleeWeapon.sprite = sprites is { Count: > 0 } ? sprites.First() : null;
                    break;
                case EquipmentSlot.LeftHand:
                    _secondaryMeleeWeapon.sprite = sprites is { Count: > 0 } ? sprites.First() : null;
                    break;
            }
        }

        private void MapSprites(List<Sprite> sprites, params SpriteRenderer[] spriteRenderers)
        {
            foreach (var part in spriteRenderers)
                part.sprite = sprites.SingleOrDefault(x => x != null && x.name.Contains(part.name.Split('[')[0]));
        }
    }
}