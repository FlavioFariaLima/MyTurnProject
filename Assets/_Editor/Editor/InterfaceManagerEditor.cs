using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditor.AnimatedValues;

// Custom Editor using SerializedProperties.
// Automatic handling of multi-object editing, undo, and Prefab overrides.
//[CanEditMultipleObjects]
//[CustomEditor(typeof(InterfaceManager))]
public class InterfaceManagerEditor : Editor
{
    // Cameras
    bool showCameras;
    SerializedProperty charCamera;
    SerializedProperty uiCamera;

    // Character Features
    bool showCharacterFeatures;
    SerializedProperty selectedCharacterObject;

    // Character Painels
    bool showInventoryPanels;
    SerializedProperty panel_PlayerInventory;
    SerializedProperty slotsParent_PlayerInventory;

    SerializedProperty characterCraftPanel;
    SerializedProperty craftSlotsParents;

    SerializedProperty panel_playerHotbar;
    SerializedProperty slotsParent_playerHotbar;

    // Others Inventories
    bool showOtherInventoriesPanels;
    SerializedProperty panel_OtherInventory;
    SerializedProperty StationPanelPrefab_Parent;

    // Information Panels
    bool showInfoPanels;
    SerializedProperty itemInfoPanel;
    SerializedProperty recipeInfoPanel;

    // Interative Stuff
    bool showInteratives;
    SerializedProperty InterectiveMenu;
    SerializedProperty ExaminePanel;

    // Prefabs
    bool showPrefabs;
    SerializedProperty craftSlotPrefab;
    SerializedProperty StationPanelPrefab;
    SerializedProperty dropedItemPrefab;

    void OnEnable()
    {
        // Cameras
        charCamera = serializedObject.FindProperty("mainCamera");
        uiCamera = serializedObject.FindProperty("uiCamera");

        // Characters Panels
        selectedCharacterObject = serializedObject.FindProperty("selectedCharacterObject");
        panel_PlayerInventory = serializedObject.FindProperty("panel_PlayerInventory");
        slotsParent_PlayerInventory = serializedObject.FindProperty("slotsParent_PlayerInventory");
        characterCraftPanel = serializedObject.FindProperty("characterCraftPanel");
        craftSlotsParents = serializedObject.FindProperty("craftSlotsParents");
        panel_playerHotbar = serializedObject.FindProperty("panel_playerHotbar");
        slotsParent_playerHotbar = serializedObject.FindProperty("slotsParent_playerHotbar");

        // Other Inventories
        panel_OtherInventory = serializedObject.FindProperty("panel_OtherInventory");
        StationPanelPrefab_Parent = serializedObject.FindProperty("StationPanelPrefab_Parent");

        // Informations Panels
        itemInfoPanel = serializedObject.FindProperty("itemInfoPanel");
        recipeInfoPanel = serializedObject.FindProperty("recipeInfoPanel");

        // Interative Stuff
        InterectiveMenu = serializedObject.FindProperty("InterectiveMenu");
        ExaminePanel = serializedObject.FindProperty("ExaminePanel");

        // UI Prefabs
        craftSlotPrefab = serializedObject.FindProperty("craftSlotPrefab");
        StationPanelPrefab = serializedObject.FindProperty("StationPanelPrefab");
        dropedItemPrefab = serializedObject.FindProperty("dropedItemPrefab");

    }

    public override void OnInspectorGUI()
    {
        // Cameras
        showCameras = EditorGUILayout.BeginFoldoutHeaderGroup(showCameras, "Cameras");

        if (showCameras)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(charCamera, new GUIContent("Main Camera"));
            EditorGUILayout.PropertyField(uiCamera, new GUIContent("UI Camera"));
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        showCharacterFeatures = EditorGUILayout.BeginFoldoutHeaderGroup(showCharacterFeatures, "Character Features");

        if (showCharacterFeatures)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(selectedCharacterObject, new GUIContent("Character GameObject"));
            GUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // Character Panels
        showInventoryPanels = EditorGUILayout.BeginFoldoutHeaderGroup(showInventoryPanels, "Character Panels & UI Elements");

        if (showInventoryPanels)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(panel_PlayerInventory, new GUIContent("Inventory Panel"));
            EditorGUILayout.PropertyField(slotsParent_PlayerInventory, new GUIContent("Inventory Slots Parent"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(characterCraftPanel, new GUIContent("Craft Panel"));
            EditorGUILayout.PropertyField(craftSlotsParents, new GUIContent("Craft Slots Parent"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(panel_playerHotbar, new GUIContent("Hotbar Panel"));
            EditorGUILayout.PropertyField(slotsParent_playerHotbar, new GUIContent("Hotbar Slots Parent"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // Others Inventories
        showOtherInventoriesPanels = EditorGUILayout.BeginFoldoutHeaderGroup(showOtherInventoriesPanels, "Others Inventories Panels");

        if (showOtherInventoriesPanels)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(panel_OtherInventory, new GUIContent("Others Inventories Panel"));
            EditorGUILayout.PropertyField(StationPanelPrefab_Parent, new GUIContent("Others Inventories Slot Parent"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // Informations Panels
        showInfoPanels = EditorGUILayout.BeginFoldoutHeaderGroup(showInfoPanels, "Informations Panels");

        if (showInfoPanels)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(itemInfoPanel, new GUIContent("Item Information Panel"));
            EditorGUILayout.PropertyField(recipeInfoPanel, new GUIContent("Recipe Information Panel"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();


        // Interatives Panels
        showInteratives = EditorGUILayout.BeginFoldoutHeaderGroup(showInteratives, "Interative Stuff Panels");

        if (showInteratives)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(InterectiveMenu, new GUIContent("Interactive Menu"));
            EditorGUILayout.PropertyField(ExaminePanel, new GUIContent("Examine Panel"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

        // UI Prefabs
        showPrefabs = EditorGUILayout.BeginFoldoutHeaderGroup(showPrefabs, "UI Prefabs");

        if (showPrefabs)
        {
            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(craftSlotPrefab, new GUIContent("Craft Slot Prefab"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(StationPanelPrefab, new GUIContent("Others Inventories Panel Prefab"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.PropertyField(dropedItemPrefab, new GUIContent("Droped Item Prefab"));
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.Space();

    }
}