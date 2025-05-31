using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;

public class DashboardWindow : EditorWindow
{
    private readonly IList<ITab> _tabs = new BindingList<ITab>();
    private ITab _tab;
    private int _currentTab = 0;

    [MenuItem("Project/Dashboard")]
    public static void ShowWindow()
    {
        var window = GetWindow<DashboardWindow>(Application.productName);
        window._tabs.Add(new EquipmentTab());
        window._tabs.Add(new ActorTable());

        window._currentTab = PlayerPrefs.GetInt("DashboardWindow._currentTab");
        foreach (var windowTab in window._tabs)
            windowTab.Init();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        for (var i = 0; i < _tabs.Count; i++)
            if (GUILayout.Toggle(_currentTab == i, _tabs[i].GetType().Name.Replace("Tab", string.Empty),
                    EditorStyles.toolbarButton))
                _currentTab = i;
        EditorGUILayout.EndHorizontal();

        var tab = _tabs[_currentTab];
        if (_tab != null && _tab.GetType().Name != tab.GetType().Name)
            PlayerPrefs.SetInt("DashboardWindow._currentTab", _currentTab);

        _tab = tab;
        _tab.Draw();
    }
}