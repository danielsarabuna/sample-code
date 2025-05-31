using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public interface ITab
{
    public void Init()
    {
    }

    public void Draw()
    {
    }
}

public interface ITab<TElement> : ITab
{
    TElement Selected { get; }
    IList<TElement> AllItems { get; }
}

public abstract class BaseTable<TElement> : ITab<TElement>
{
    private List<TElement> _list = new List<TElement>();
    public TElement Selected { get; protected set; }
    private Vector2 _itemTreeScrollPosition, _detailsScrollPosition;
    private readonly Dictionary<Type, bool> _foldoutStates = new Dictionary<Type, bool>();
    private Dictionary<Type, Dictionary<Type, List<TElement>>> _hierarchicalGroups;
    private List<TElement> _searchResults = new();
    private string _searchQuery = "";
    public IList<TElement> AllItems => _list;
    protected abstract string SaveFilePath { get; }
    private bool IsSearching => !string.IsNullOrEmpty(_searchQuery);
    protected abstract void DrawItemDetailsPanel();
    protected abstract void DrawItemTree(TElement element);
    protected abstract bool IsMatchForSearch(TElement element, string search);
    protected abstract List<TElement> GetFromJson(string json);
    protected abstract string GetToJson();

    private readonly GUIStyle _groupStyle = new(EditorStyles.foldout)
    {
        fontStyle = FontStyle.Bold
    };

    protected readonly GUIStyle _itemStyle = new(EditorStyles.label)
    {
        padding = { left = 20 }, normal = { textColor = Color.white },
        hover = { textColor = Color.cyan },
        richText = true
    };

    public virtual void Init()
    {
        _list = Load();
        RebuildGroups();
        InitializeFoldoutStates();
    }


    public void Draw()
    {
        DrawToolbar();
        EditorGUILayout.BeginHorizontal();
        DrawItemTreePanel();
        _detailsScrollPosition = EditorGUILayout.BeginScrollView(_detailsScrollPosition);
        DrawItemDetailsPanel();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndHorizontal();
    }

    private void InitializeFoldoutStates()
    {
        _foldoutStates.Clear();
        foreach (var parentType in _hierarchicalGroups.Keys)
        {
            _foldoutStates[parentType] = true;
            foreach (var childType in _hierarchicalGroups[parentType].Keys)
                _foldoutStates[childType] = true;
        }
    }

    private void RebuildGroups()
    {
        _hierarchicalGroups = BuildHierarchicalGroups(AllItems);
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUI.SetNextControlName("SearchField");
        var newSearch = EditorGUILayout.TextField(_searchQuery, EditorStyles.toolbarSearchField, GUILayout.Width(200));
        if (newSearch != _searchQuery)
        {
            _searchQuery = newSearch;
            UpdateSearchResults();
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Item", EditorStyles.toolbarDropDown))
        {
            var menu = new GenericMenu();

            var itemTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TElement)));

            foreach (var type in itemTypes)
                menu.AddItem(new GUIContent(FormatGroupName(type.Name)), false, () => CreateNewItem(type));

            menu.ShowAsContext();
        }

        EditorGUILayout.EndHorizontal();
    }

    protected string FormatGroupName(string typeName)
    {
        typeName = typeName.Replace("Item", "");
        return System.Text.RegularExpressions.Regex.Replace(typeName, "([A-Z])", " $1").Trim();
    }

    private void DrawSearchResults()
    {
        if (_searchResults.Count == 0)
        {
            EditorGUILayout.LabelField("No items found", EditorStyles.boldLabel);
            return;
        }

        EditorGUILayout.LabelField($"Found {_searchResults.Count} items:", EditorStyles.boldLabel);

        foreach (var item in _searchResults)
            DrawItem(item);
    }

    private void DrawItem(TElement item)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(EditorGUI.indentLevel * 15);
        DrawItemTree(item);
        EditorGUILayout.EndHorizontal();
    }

    private void UpdateSearchResults()
    {
        _searchResults.Clear();
        if (string.IsNullOrEmpty(_searchQuery)) return;
        var search = _searchQuery.ToLower();
        _searchResults = AllItems.Where(x => IsMatchForSearch(x, search)).ToList();
    }

    protected virtual TElement CreateNewItem(Type type)
    {
        var instance = Activator.CreateInstance(type);
        var newItem = (TElement)instance;
        _list.Add(newItem);
        RebuildGroups();
        Selected = newItem;
        return newItem;
    }

    private void DrawItemTreePanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(250));
        _itemTreeScrollPosition = EditorGUILayout.BeginScrollView(_itemTreeScrollPosition);
        if (IsSearching) DrawSearchResults();
        else
        {
            foreach (var (parentType, childGroups) in BuildHierarchicalGroups(AllItems))
            {
                var isParentFoldoutOpen = _foldoutStates.GetValueOrDefault(parentType, true);
                _foldoutStates[parentType] = EditorGUILayout.Foldout(isParentFoldoutOpen,
                    parentType.Name.Replace("Model", string.Empty), _groupStyle);

                if (!_foldoutStates[parentType]) continue;

                EditorGUI.indentLevel++;
                foreach (var (childType, childObjects) in childGroups)
                {
                    var isChildFoldoutOpen = _foldoutStates.GetValueOrDefault(childType, true);

                    _foldoutStates[childType] = EditorGUILayout.Foldout(isChildFoldoutOpen,
                        $"{childType.Name.Replace("Model", string.Empty)}", _groupStyle);

                    if (!_foldoutStates[childType]) continue;

                    EditorGUI.indentLevel++;
                    foreach (var item in childObjects)
                        DrawItem(item);
                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private Dictionary<Type, Dictionary<Type, List<TElement>>> BuildHierarchicalGroups(IEnumerable<TElement> objects)
    {
        if (objects == null)
            throw new ArgumentNullException(nameof(objects));

        var groups = objects.GroupBy(x => x.GetType()).ToDictionary(
            x => x.Key,
            x => x.ToList()
        );

        var hierarchy = new Dictionary<Type, Dictionary<Type, List<TElement>>>();

        foreach (var group in groups)
        {
            var parentType = group.Key.BaseType;

            if (parentType != null && !hierarchy.ContainsKey(parentType))
                hierarchy[parentType] = new Dictionary<Type, List<TElement>>();

            if (parentType != null) hierarchy[parentType].Add(group.Key, group.Value);
        }

        return hierarchy;
    }

    private List<TElement> Load()
    {
        if (!File.Exists(SaveFilePath)) return new List<TElement>();
        var json = File.ReadAllText(SaveFilePath);
        return GetFromJson(json);
    }

    protected void Save()
    {
        var json = GetToJson();

        var directory = Path.GetDirectoryName(SaveFilePath);
        if (!Directory.Exists(directory))
            if (directory != null)
                Directory.CreateDirectory(directory);

        File.WriteAllText(SaveFilePath, json);
        AssetDatabase.Refresh();
    }
}