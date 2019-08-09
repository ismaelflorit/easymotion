/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Configuration window to guide user through motion tracking setup.
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EasyMotionRigidbodySetupWindow : EditorWindow
{
    private static Rigidbody[] availableRigidBodies;
    private Dictionary<GameObject, Texture2D> rigidbodyGameobjectsAndImageDictionary = new Dictionary<GameObject, Texture2D>();
    Dictionary<GameObject, int> nullPreviewCount = new Dictionary<GameObject, int>();
    private Vector2 scrollPosition;
    private int selectionGridIndex = 0;
    private GUISkin skin;
    private string currentScene;
    EasyMotion[] easyMotions;

    [MenuItem("EasyMotion/Rigidbody Motion Setup",false, 12)]
    static void OpenWindow()
    {
        EasyMotionRigidbodySetupWindow window = (EasyMotionRigidbodySetupWindow)GetWindow(typeof(EasyMotionRigidbodySetupWindow));
        window.Show();
        window.titleContent = new GUIContent("Rigidbody Setup", "Select the rigidbody for which you wish to map motion to");
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 900);
    }

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load(EasyMotionConstants.pluginSkin);
        availableRigidBodies = EasyMotionUtility.FindActiveRigidbodies();
        PopulatePreviewDictionary();
        currentScene = SceneManager.GetActiveScene().name;
        EditorApplication.hierarchyChanged += HierarchyWindowChanged;
        easyMotions = EasyMotionUtility.FindActiveEnabledEasyMotions();
    }    

    private void HierarchyWindowChanged()
    {
        if (!CheckIfSceneChanged())
        {
            CheckIfRigidbodyCountChanged();
        }
    }

    private void CheckIfRigidbodyCountChanged()
    {
        Rigidbody[] currentRigidbodies = EasyMotionUtility.FindActiveRigidbodies();
        if(availableRigidBodies.Length != currentRigidbodies.Length)
        {
            ReOpenWindow();
        }
    }

    private bool CheckIfSceneChanged()
    {
        bool sceneChanged = false;
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            sceneChanged = true;
            Close();
        }
        return sceneChanged;
    }

    private void ReOpenWindow()
    {
        EasyMotionUtility.ReOpen<EasyMotionRigidbodySetupWindow>(
                "Rigidbody Setup",
                "Select the rigidbody for which you wish to map motion to");
    }

    private void PopulatePreviewDictionary()
    {
        foreach (Rigidbody rigidbody in availableRigidBodies)
        {
            InitialiseNullPreviewCountDictionary(rigidbody.gameObject);
            AddObjectPreviewToDictionary(rigidbody.gameObject);
        }        
    }

    private void InitialiseNullPreviewCountDictionary(GameObject rigidbodyObject)
    {
        if (!nullPreviewCount.ContainsKey(rigidbodyObject.gameObject))
        {
            nullPreviewCount.Add(rigidbodyObject.gameObject, 0);
        }
    }

    private void AddObjectPreviewToDictionary(GameObject rigidbodyObject)
    {
        Texture2D preview = AssetPreview.GetAssetPreview(rigidbodyObject);
        if (preview != null)
        {
            rigidbodyGameobjectsAndImageDictionary.Add(rigidbodyObject, preview);
        }
        else
        {
            AddTemporaryNullPreviewToDictionary(rigidbodyObject, preview);        
        }
    }

    private void AddTemporaryNullPreviewToDictionary(GameObject rigidbodyObject, Texture2D preview)
    {
        nullPreviewCount[rigidbodyObject]++;
        if (nullPreviewCount[rigidbodyObject].Equals(20))
        {
            preview = (Texture2D)Resources.Load("brokenImage");
            rigidbodyGameobjectsAndImageDictionary.Add(rigidbodyObject, preview);
        }
        if (!rigidbodyGameobjectsAndImageDictionary.ContainsKey(rigidbodyObject))
        {
            rigidbodyGameobjectsAndImageDictionary.Add(rigidbodyObject, null);
        }
    }

    private void Update()
    {
        if (Application.IsPlaying(this))
        {
            Close();
        }
        if (availableRigidBodies.Length != rigidbodyGameobjectsAndImageDictionary.Count)
        {
            PopulatePreviewDictionary();
        }
    }
    
    private void OnGUI()
    {
        DrawLogoBox();
        DrawScroll();
        DrawButtons();
        Repaint();
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
        if (easyMotions.Length < 1 && availableRigidBodies.Length > 0)
        {
            int rowCapacity = Mathf.FloorToInt(position.width / 80f);
            GUILayout.Label("Select the Rigidbody to map motion to:", GetAvailableLabelStyle());
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            selectionGridIndex = GUILayout.SelectionGrid(
                selectionGridIndex,
                GetAvailableRigidbodyPreviews(),
                rowCapacity,
                GetSelectionGridStyle());
            GUILayout.EndScrollView();
        }
        else
        {
            if (availableRigidBodies.Length < 1)
            {
                GUILayout.Label("No Rigidbodies were found on this scene.", GetAvailableLabelStyle());
                GUILayout.FlexibleSpace();
                DrawCloseButton();
            }
        }
    }

    private GUIStyle GetAvailableLabelStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label);
        guiStyle.margin = new RectOffset(8, 10, 10, 8);
        guiStyle.fontStyle = FontStyle.Bold;
        return guiStyle;
    }

    private GUIStyle GetSelectionGridStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.alignment = TextAnchor.LowerCenter;
        guiStyle.imagePosition = ImagePosition.ImageAbove;
        guiStyle.fixedWidth = 80;
        guiStyle.fixedHeight = 80;
        guiStyle.margin = new RectOffset(10, 10, 10, 10);
        return guiStyle;
    }

    private void DrawButtons()
    {

        if (easyMotions.Length < 1 && availableRigidBodies.Length > 0)
        {
            GUILayout.FlexibleSpace();
            DrawApplyButton();
            DrawCloseButton();
        }
        else
        {
            if (availableRigidBodies.Length > 0)
            {
                EditorGUILayout.HelpBox("Rigidbody assigned. (Only 1 Rigidbody per scene currently supported.)", MessageType.Info);
                DrawLocateButton();
                GUILayout.FlexibleSpace();
                DrawResetButton();
                DrawCloseButton();
            }
        }
    }

    private void DrawLocateButton()
    {
        if (GUILayout.Button("Locate assigned Rigidbody", skin.button))
        {
            EditorGUIUtility.PingObject(easyMotions[0].gameObject);
            Selection.activeGameObject = easyMotions[0].gameObject;
        }
    }

    private void DrawApplyButton()
    {
        try
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", skin.button))
            {
                availableRigidBodies[selectionGridIndex].gameObject.AddComponent<EasyMotion>();
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                EditorGUIUtility.PingObject(availableRigidBodies[selectionGridIndex].gameObject);
                Selection.activeGameObject = availableRigidBodies[selectionGridIndex].gameObject;
                this.Close();
            }
        }
        catch (IndexOutOfRangeException)
        {
            EditorUtility.DisplayDialog("EasyMotion - Rigidbody Selection", "Please select a Rigidbody.", "Ok");
        }
    }

    private void DrawResetButton()
    {
        if (GUILayout.Button("Reset", skin.button))
        {
            if (EditorUtility.DisplayDialog("EasyMotion - Rigidbody Reset", "This will remove the EasyMotion component and any of its saved force motion multipliers.\n\nDo you wish to Reset?", "Ok", "Cancel"))
            {
                RemoveAssignedEasyMotion();
                ReOpenWindow();
            }
        }
    }

    private void RemoveAssignedEasyMotion()
    {
        DestroyAllEasyMotionInstances();
        DestroyAllVisualAids();
    }

    private void DrawCloseButton()
    {
        if (GUILayout.Button("Close", skin.button))
        {
            this.Close();
        }
    }

    private void DestroyAllEasyMotionInstances()
    {
        foreach (EasyMotion script in easyMotions)
        {
            DestroyImmediate(script);
        }
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        easyMotions = EasyMotionUtility.FindActiveEnabledEasyMotions();
    }

    private void DestroyAllVisualAids()
    {
        while (GameObject.Find(EasyMotionConstants.visualAidPrefab))
        {
            DestroyImmediate(GameObject.Find(EasyMotionConstants.visualAidPrefab));
        }
    }

    private GUIContent[] GetAvailableRigidbodyPreviews()
    {        
        List<GUIContent> guiContentList = new List<GUIContent>();
        try
        {
            if (availableRigidBodies.Length.Equals(rigidbodyGameobjectsAndImageDictionary.Count))
            {
                foreach (KeyValuePair<GameObject, Texture2D> entry in rigidbodyGameobjectsAndImageDictionary)
                {
                    GUIContent guiContent = new GUIContent
                    {
                        text = entry.Key.name,
                        image = entry.Value
                    };
                    guiContentList.Add(guiContent);
                }
            }
        }
        catch (Exception)
        {
            EasyMotionUtility.ReOpen<EasyMotionRigidbodySetupWindow>("Rigidbody Setup", "Select the rigidbody for which you wish to map motion to");
        }
        
        return guiContentList.ToArray();
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= HierarchyWindowChanged;
    }
}
