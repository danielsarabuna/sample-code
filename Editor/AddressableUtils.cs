using System.Linq;
using UnityEditor.AddressableAssets;
using Object = UnityEngine.Object;

namespace UnityEditor.Utils
{
    public static class AddressableUtils
    {
        [MenuItem("Tools/Addressables/AddToEquipmentIcons")]
        private static void AddToEquipmentIcons()
        {
            AddToAddressable("Icons", "Equipment");
        }

        [MenuItem("Tools/Addressables/AddToEquipment")]
        private static void AddToEquipment()
        {
            AddToAddressable("Equipment", "Equipment");
        }

        private static void AddToAddressable(string groupName, string label)
        {
            var selection = Selection.assetGUIDs;
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var group = settings.groups.FirstOrDefault(x => x.Name == groupName);
            if (group == null) group = settings.CreateGroup(groupName, false, false, false, null);

            foreach (var guid in selection)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                var entry = settings.FindAssetEntry(guid);
                if (entry != null) continue;

                var assetEntry = settings.CreateOrMoveEntry(guid, group);
                assetEntry.SetLabel(label, true);
                assetEntry.SetAddress(asset.name);
            }

            AssetDatabase.Refresh();
        }
    }
}