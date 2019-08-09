/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Configuration window to guide user through toggle menu dependency.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EasyMotionToggleSetupWindow : EditorWindow
{
    private static Toggle[] availableToggles;
    private Dictionary<GameObject, Texture2D> toggleGameobjectsAndImagesDictionary = new Dictionary<GameObject, Texture2D>();
    private Vector2 scrollPosition;
    private int selectionGridIndex = -1;
    private GUISkin skin;
    private Texture2D toggleIcon;
    private string currentScene;
    EasyMotionToggleController[] easyMotionToggleControllers;

    [MenuItem("EasyMotion/In-Game Menu References/On-Off Toggle Setup", false, 13)]
    static void Init()
    {
        EasyMotionToggleSetupWindow window = (EasyMotionToggleSetupWindow)EditorWindow.GetWindow(typeof(EasyMotionToggleSetupWindow));
        window.Show();
        window.titleContent = new GUIContent("Toggle Setup", "Select the toggle which will enable the player to enable or disable the motion platform.");
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 900);
    }

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load(EasyMotionConstants.pluginSkin);
        toggleIcon = (Texture2D)Resources.Load("toggle");
        easyMotionToggleControllers = EasyMotionUtility.FindActiveEnabledToggleControllers();
        availableToggles = EasyMotionUtility.FindActiveToggles();
        GeneratePreviews();
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void OnHierarchyChange()
    {
        CheckIfSceneChanged();
        CheckIfToggleCountChanged();
    }

    private void CheckIfToggleCountChanged()
    {
        Toggle[] currentSliders = EasyMotionUtility.FindActiveToggles();
        if (availableToggles.Length != currentSliders.Length)
        {
            EasyMotionUtility.ReOpen<EasyMotionToggleSetupWindow>(
                "Toggle Setup",
                "Select the toggle which will enable the player to enable or disable the motion platform.");
        }
    }

    private void CheckIfSceneChanged()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            Close();
        }
    }

    private void GeneratePreviews()
    {
        foreach (Toggle slider in availableToggles)
        {
            if (!toggleGameobjectsAndImagesDictionary.ContainsKey(slider.gameObject))
            {
                toggleGameobjectsAndImagesDictionary.Add(slider.gameObject, toggleIcon);
            }
        }
    }

    private void Update()
    {
        if (availableToggles.Length != toggleGameobjectsAndImagesDictionary.Count)
        {
            GeneratePreviews();
        }
    }

    private void OnGUI()
    {
        DrawLogoBox();
        DrawScroll();
        DrawButtons();
    }

    private void DrawLogoBox()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Box("", skin.GetStyle("Logo"));
        EditorGUILayout.Space();
    }

    private void DrawScroll()
    {
        if (easyMotionToggleControllers.Length < 1 && availableToggles.Length > 0)
        {
            int rowCapacity = Mathf.FloorToInt(position.width / 100f);
            GUILayout.Label("Select the Toggle which will let a player \nenable or disable the motion platform:", GetAvailableLabelStyle());
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            selectionGridIndex = GUILayout.SelectionGrid(
                selectionGridIndex,
                GetAvailableTogglePreviews(toggleGameobjectsAndImagesDictionary),
                rowCapacity,
                GetSelectionGridStyle());
            GUILayout.EndScrollView();
        }
        else
        {
            if (availableToggles.Length < 1)
            {
                GUILayout.Label("No Toggles were found on this scene.", GetAvailableLabelStyle());
                GUILayout.FlexibleSpace();
                DrawCloseButton();
            }
        }
    }

    private GUIStyle GetIntroStyle()
    {
        GUIStyle intro = new GUIStyle(GUI.skin.box);
        intro.alignment = TextAnchor.LowerLeft;
        intro.margin = new RectOffset(18, 10, 10, 10);
        intro.padding = new RectOffset(5, 5, 5, 5);
        return intro;
    }

    private GUIStyle GetAvailableLabelStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
        guiStyle.margin = new RectOffset(10, 10, 10, 10);
        guiStyle.fontStyle = FontStyle.Bold;
        return guiStyle;
    }

    private GUIStyle GetSelectionGridStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.alignment = TextAnchor.LowerCenter;
        guiStyle.imagePosition = ImagePosition.ImageAbove;
        guiStyle.fixedWidth = 75;
        guiStyle.fixedHeight = 40;
        guiStyle.margin = new RectOffset(15, 10, 10, 10);
        guiStyle.padding = new RectOffset(5, 5, 5, 5);
        return guiStyle;
    }

    private GUIContent[] GetAvailableTogglePreviews(Dictionary<GameObject, Texture2D> dictionary)
    {
        List<GUIContent> images = new List<GUIContent>();
        if (availableToggles.Length == toggleGameobjectsAndImagesDictionary.Count)
        {
            foreach (KeyValuePair<GameObject, Texture2D> entry in dictionary)
            {
                GUIContent ToggleGUIContent = new GUIContent();
                ToggleGUIContent.text = entry.Key.name;
                ToggleGUIContent.image = entry.Value;
                images.Add(ToggleGUIContent);
            }
        }
        return images.ToArray();
    }

    private void DrawButtons()
    {

        if (easyMotionToggleControllers.Length < 1 && availableToggles.Length > 0)
        {
            GUILayout.FlexibleSpace();
            DrawApplyButton();
            DrawCloseButton();
        }
        else
        {
            if (easyMotionToggleControllers.Length > 0)
            {
                EditorGUILayout.HelpBox("Toggle has already been assigned.", MessageType.Info);
                DrawLocateButton();
                GUILayout.FlexibleSpace();
                DrawResetButton();
                DrawCloseButton();
            }
        }
    }

    private void DrawLocateButton()
    {
        if (GUILayout.Button("Locate assigned Toggle", skin.button))
        {
            EditorGUIUtility.PingObject(easyMotionToggleControllers[0]);
            Selection.activeGameObject = easyMotionToggleControllers[0].gameObject;
        }
    }

    private void DrawApplyButton()
    {
        try
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", skin.button))
            {
                EasyMotionToggleController toggleScript = availableToggles[selectionGridIndex].gameObject.AddComponent<EasyMotionToggleController>();
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                toggleScript.SetSavePath(EasyMotionConstants.toggleSavePath);
                EditorGUIUtility.PingObject(availableToggles[selectionGridIndex].gameObject);
                Selection.activeGameObject = availableToggles[selectionGridIndex].gameObject;
                this.Close();
            }
        }
        catch (IndexOutOfRangeException)
        {
            EditorUtility.DisplayDialog("EasyMotion - Toggle Selection", "\n\nPlease select a Toggle.", "Ok");
        }
    }

    private void DrawResetButton()
    {
        if (GUILayout.Button("Reset", skin.button))
        {
            if (EditorUtility.DisplayDialog("EasyMotion - Toggle Reset", "This will remove the EasyMotion toggle controller.\n\nDo you wish to Reset?", "Ok", "Cancel"))
            {
                ResetToggleController();
            }
        }
    }

    private void DrawCloseButton()
    {
        if (GUILayout.Button("Close", skin.button))
        {
            this.Close();
        }
    }

    private void ResetToggleController()
    {
        DestroyAllToggleControllers();
    }

    private void DestroyAllToggleControllers()
    {        
        foreach(EasyMotionToggleController script in easyMotionToggleControllers)
        {
            DestroyImmediate(script);
        }
        easyMotionToggleControllers = EasyMotionUtility.FindActiveEnabledToggleControllers();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}
