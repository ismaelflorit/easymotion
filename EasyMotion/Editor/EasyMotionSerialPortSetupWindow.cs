/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Configuration window to guide user through Dropdown menu dependency.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EasyMotionSerialPortSetupWindow : EditorWindow
{
    private Dropdown[] availableDropdowns;
    private Dictionary<GameObject, Texture2D> dropdownGameobjectsAndImagesDictionary = new Dictionary<GameObject, Texture2D>();
    private Vector2 scrollPosition;
    private int selectionGridIndex = -1;
    private GUISkin skin;
    private Texture2D dropdownIcon;
    private string currentScene;
    EasyMotionSerialPortDropdownController[] dropdownControllers;

    [MenuItem("EasyMotion/In-Game Menu References/Serial Port Setup",false, 14)]
    static void Init()
    {
        EasyMotionSerialPortSetupWindow window = (EasyMotionSerialPortSetupWindow)EditorWindow.GetWindow(typeof(EasyMotionSerialPortSetupWindow));
        window.Show();
        window.titleContent = new GUIContent("Serial Port Setup", "Select the dropdown the player will use to select their motion platform's serial port");
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 900);
    }

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load(EasyMotionConstants.pluginSkin);
        dropdownControllers = EasyMotionUtility.FindActiveEnabledDropdownControllers();
        dropdownIcon = (Texture2D)Resources.Load("dropdown");
        availableDropdowns = EasyMotionUtility.FindActiveDropdowns();
        GeneratePreviews();
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void OnHierarchyChange()
    {
        CheckIfSceneChanged();
        CheckIfDropdownCountChanged();
    }

    private void CheckIfSceneChanged()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            Close();
        }
    }

    private void CheckIfDropdownCountChanged()
    {
        Dropdown[] currentDropdowns = EasyMotionUtility.FindActiveDropdowns();
        if (availableDropdowns.Length != currentDropdowns.Length)
        {
            EasyMotionUtility.ReOpen<EasyMotionSerialPortSetupWindow>(
                "Serial Port Setup",
                "Select the dropdown the player will use to select their motion platform's serial port");
        }
    }


    private void GeneratePreviews()
    {
        foreach(Dropdown dropdown in availableDropdowns)
        {
            if (!dropdownGameobjectsAndImagesDictionary.ContainsKey(dropdown.gameObject))
            {
                dropdownGameobjectsAndImagesDictionary.Add(dropdown.gameObject, dropdownIcon);
            }
        }
    }

    private void Update()
    {
        if (availableDropdowns.Length != dropdownGameobjectsAndImagesDictionary.Count)
        {
            GeneratePreviews();
        }
    }

    private void OnGUI()
    {
        DrawLogoBox();
        DrawScroll();
        DrawButtons();
        EditorGUILayout.Space();
    }

    void DrawLogoBox()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Box("", skin.GetStyle("Logo"));
        EditorGUILayout.Space();        
    }

    private void DrawScroll()
    {
        if(availableDropdowns.Length > 0 && dropdownControllers.Length < 1)
        {
            int rowCapacity = Mathf.FloorToInt(position.width / (80f));
            GUILayout.Label("Select the Dropdown which will allow a player to select their platform's COM serial port: ", GetAvailableLabelStyle());
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            selectionGridIndex = GUILayout.SelectionGrid(
            selectionGridIndex,
            GetAvailableDropdownPreviews(dropdownGameobjectsAndImagesDictionary),
            rowCapacity,
            GetSelectionGridStyle());
            GUILayout.EndScrollView();
        }
        else
        {
            if (availableDropdowns.Length < 1)
            {                
                GUILayout.Label("No dropdowns were found on this scene.", GetAvailableLabelStyle());
                GUILayout.FlexibleSpace();
            }
        }
    }

    private GUIStyle GetIntroStyle()
    {
        GUIStyle intro = new GUIStyle(GUI.skin.box);
        intro.alignment = TextAnchor.LowerLeft;
        intro.margin = new RectOffset(8, 10, 10, 8);
        intro.padding = new RectOffset(5, 5, 5, 5);
        return intro;
    }

    private GUIStyle GetAvailableLabelStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
        guiStyle.margin = new RectOffset(8, 10, 10, 10);
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.wordWrap = true;
        return guiStyle;
    }

    private GUIStyle GetSelectionGridStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.alignment = TextAnchor.LowerCenter;
        guiStyle.imagePosition = ImagePosition.ImageAbove;
        guiStyle.fixedWidth = 83;
        guiStyle.fixedHeight = 40;
        guiStyle.margin = new RectOffset(10, 10, 10, 10);
        guiStyle.padding = new RectOffset(5, 5, 5, 5);
        return guiStyle;
    }

    private void DrawButtons()
    {
        if (dropdownControllers.Length < 1 && availableDropdowns.Length > 0)
        {
            GUILayout.FlexibleSpace();
            DrawApplyButton();
        }
        else
        {
            if (dropdownControllers.Length > 0) {
                EditorGUILayout.HelpBox("Dropdown already assigned! ", MessageType.Info);
                DrawLocateButton();
                GUILayout.FlexibleSpace();
                DrawResetButton();
            }
        }
        DrawCloseButton();
    }

    private void DrawLocateButton()
    {
        if (GUILayout.Button("Locate assigned Dropdown", skin.button))
        {
            EditorGUIUtility.PingObject(dropdownControllers[0].gameObject);
            Selection.activeGameObject = dropdownControllers[0].gameObject;
        }
    }

    private void DrawApplyButton()
    {

        if (GUILayout.Button("Apply", skin.button))
        {
            try {
            availableDropdowns[selectionGridIndex].gameObject.AddComponent<EasyMotionSerialPortDropdownController>();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorGUIUtility.PingObject(availableDropdowns[selectionGridIndex].gameObject);
            Selection.activeGameObject = availableDropdowns[selectionGridIndex].gameObject;
            this.Close();
            } catch (IndexOutOfRangeException)
            {
                EditorUtility.DisplayDialog("EasyMotion - Dropdown Selection", "\n\nPlease select a dropdown.", "Ok");
            }
        }
    }

    private void DrawResetButton()
    {
        if (GUILayout.Button("Reset", skin.button))
        {
            if (EditorUtility.DisplayDialog("EasyMotion - Serial Port Reset", "This will remove the tracked dropdown for SerialPort selection.\n\nReset?", "Ok", "Cancel"))
            {
                ResetPlugin();
            }
        }
    }

    private void ResetPlugin()
    {
        DestroyAllEasyMotionSerialPortDropdownInstances();
    }

    private void DrawCloseButton()
    {
        if (GUILayout.Button("Close", skin.button))
        {
            this.Close();
        }
    }

    private void DestroyAllEasyMotionSerialPortDropdownInstances()
    {
        foreach (EasyMotionSerialPortDropdownController script in dropdownControllers)
        {
            DestroyImmediate(script);
        }
        dropdownControllers = EasyMotionUtility.FindActiveEnabledDropdownControllers();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    private GUIContent[] GetAvailableDropdownPreviews(Dictionary<GameObject, Texture2D> dictionary)
    {
        List<GUIContent> images = new List<GUIContent>();
        if (availableDropdowns.Length == dropdownGameobjectsAndImagesDictionary.Count)
        {
            foreach (KeyValuePair<GameObject, Texture2D> entry in dictionary)
            {
                GUIContent DropdownGUIContent = new GUIContent();
                DropdownGUIContent.text = entry.Key.name;
                DropdownGUIContent.image = entry.Value;
                images.Add(DropdownGUIContent);
            }
        }
        return images.ToArray();
    }
}
