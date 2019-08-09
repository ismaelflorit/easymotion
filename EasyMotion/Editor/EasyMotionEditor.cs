/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Custom Editor window for EasyMotion.
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(EasyMotion))]
[CanEditMultipleObjects]
public class EasyMotionEditor : Editor
{
    private GUISkin skin;
    private SerializedProperty dropdownForSerialPorts;
    private GameObject visualAidGameObject;
    private bool lookingForVisualAidPrefab;
    EasyMotion easyMotion;
    private bool objectActivated;
    private bool easyMotionDeleted = false;
    ToggleStates[] toggleStatesScripts;
    ToggleStates toggleStates;

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load(EasyMotionConstants.pluginSkin);
        easyMotion = (EasyMotion)serializedObject.targetObject;
        toggleStatesScripts = Resources.FindObjectsOfTypeAll<ToggleStates>();
        if (toggleStatesScripts.Length > 0)
        {
            toggleStates = toggleStatesScripts[0];
        }
        if (easyMotion.gameObject.activeInHierarchy)
        {
            InitialiseVisualAidPrefab();
            lookingForVisualAidPrefab = true;
            FindVisualAidPrefab();
            GetGroupHeaderStates();
            GetVisualAidStates();
            GetJitterEffectState();
            objectActivated = true;
        }
    }

    private void FindVisualAidPrefab()
    {
        while (lookingForVisualAidPrefab)
        {
            visualAidGameObject = GameObject.Find(EasyMotionConstants.visualAidPrefab);
            if (GameObject.Find(EasyMotionConstants.visualAidPrefab))
            {
                lookingForVisualAidPrefab = false;
            }
        }
    }

    private void InitialiseVisualAidPrefab()
    {
        if (!GameObject.Find(EasyMotionConstants.visualAidPrefab))
        {
            GameObject visualAidPrefab = (GameObject)Instantiate(Resources.Load(EasyMotionConstants.visualAidPrefab));
            visualAidPrefab.name = EasyMotionConstants.visualAidPrefab;
            //visualAidPrefab.hideFlags = HideFlags.HideInHierarchy;
            DefaultAllTogglesToTrue();
        }
    }

    private void DefaultAllTogglesToTrue()
    {
        toggleStates.forceSettingsHeaderGroupToggled = true;
        toggleStates.visualAidHeaderGroupToggled = true;
        toggleStates.platformSimulationToggled = true;
        toggleStates.gForceSimulationToggled = true;
        toggleStates.jitterEffectToggled = true;
        PlayerPrefs.SetInt(EasyMotionConstants.forceSettingsHeaderGroup, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.visualAidHeaderGroup, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.platformSimulationButton, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.gForceSimulationButton, 1);
        PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectToggled, 1);
    }

    private void GetGroupHeaderStates()
    {
        GetForceHeaderGroupState();
        GetVisualAidHeaderGroupState();
        GetJitterEffectHeaderGroupState();
    }

    private void GetForceHeaderGroupState()
    {
        if (PlayerPrefs.GetInt(EasyMotionConstants.forceSettingsHeaderGroup) == 1)
        {
            toggleStates.forceSettingsHeaderGroupToggled = true;
        }
        else
        {
            toggleStates.forceSettingsHeaderGroupToggled = false;
        }
    }

    private void GetVisualAidHeaderGroupState()
    {
        if (PlayerPrefs.GetInt(EasyMotionConstants.visualAidHeaderGroup) == 1)
        {
            toggleStates.visualAidHeaderGroupToggled = true;
        }
        else
        {
            toggleStates.visualAidHeaderGroupToggled = false;
        }
    }

    private void GetJitterEffectHeaderGroupState()
    {
        if (PlayerPrefs.GetInt(EasyMotionConstants.jitterEffectHeaderGroup) == 1)
        {
            toggleStates.jitterEffectHeaderGroupToggled = true;
        }
        else
        {
            toggleStates.jitterEffectHeaderGroupToggled = false;
        }
    }

    private void GetVisualAidStates()
    {
        GetPlatformSimulationPressedState();
        GetGForceUIPressedState();
    }

    private void GetPlatformSimulationPressedState()
    {
        if (PlayerPrefs.GetInt(EasyMotionConstants.platformSimulationButton) == 1)
        {
            toggleStates.platformSimulationToggled = true;
            ActivateMotionPlatformVisualAid();
        }
        else
        {
            toggleStates.platformSimulationToggled = false;
            DeactivateMotionPlatformVisualAid();
        }
    }

    private void GetGForceUIPressedState()
    {   
        if (PlayerPrefs.GetInt(EasyMotionConstants.gForceSimulationButton) == 1)
        {
            toggleStates.gForceSimulationToggled = true;
            ActivateGForceVisualAid();
        }
        else
        {
            toggleStates.gForceSimulationToggled = false;
            DeactivateGForceVisualAid();
        }
    }

    private void GetJitterEffectState()
    {
        if (PlayerPrefs.GetInt(EasyMotionConstants.jitterEffectToggled) == 1)
        {
            toggleStates.jitterEffectToggled = true;
            ActivateJitterEffect();
        }
        else
        {
            toggleStates.jitterEffectToggled = false;
            DeactivateJitterEffect();
        }
    }

    public override void OnInspectorGUI()
    {
        if (!objectActivated && easyMotion.gameObject.activeInHierarchy)
        {
            InitialiseVisualAidPrefab();
            lookingForVisualAidPrefab = true;
            FindVisualAidPrefab();
            GetGroupHeaderStates();
            GetVisualAidStates();
            GetJitterEffectState();
            objectActivated = true;
        }
        CheckForDuplicateEasyMotions();
        if(serializedObject.targetObject != null && easyMotion.gameObject.activeInHierarchy)
        {
            serializedObject.Update();
            DrawLogoBox();
            DrawHeaderGroups();
            serializedObject.ApplyModifiedProperties();
        }
        if (!easyMotionDeleted)
        {
            CheckIfObjectIsDisabled();
        }
    }

    private void CheckForDuplicateEasyMotions()
    {
        EasyMotion[] easyMotions = GetActiveEnabledEasyMotions();
        GameObject gameObject = Selection.activeGameObject;
        if (easyMotions.Length > 1)
        {
            Debug.LogWarning("<b>EasyMotion</b> can only track 1 Rigidbody per scene. Removing the component from <i>" + gameObject.name + "</i>.");
            DestroyImmediate(gameObject.GetComponent<EasyMotion>());
            easyMotionDeleted = true;
        }
    }

    private EasyMotion[] GetActiveEnabledEasyMotions()
    {
        EasyMotion[] easyMotions = Resources.FindObjectsOfTypeAll<EasyMotion>();
        List<EasyMotion> easyMotionList = new List<EasyMotion>();
        List<EasyMotion> easyMotionsEnabled = new List<EasyMotion>();
        foreach (EasyMotion easyMotion in easyMotions)
        {
            easyMotionList.Add(easyMotion);
        }
        foreach (EasyMotion easyMotion in easyMotionList)
        {
            if (easyMotion.isActiveAndEnabled)
            {
                easyMotionsEnabled.Add(easyMotion);
            }
        }
        return easyMotionsEnabled.ToArray();
    }

    private void CheckIfObjectIsDisabled()
    {
        if (!easyMotion.gameObject.activeInHierarchy)
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("\n <color=blue><i> Activate object to modify EasyMotion parameters.</i></color>", skin.GetStyle("WarningLabel"));
            Repaint();
        }
    }

    void DrawLogoBox()
    {
        EditorGUILayout.Space();
        GUILayout.Box("", skin.GetStyle("Logo"));
        EditorGUILayout.Space();
    }

    void DrawHeaderGroups()
    {
        EditorStyles.foldoutHeader.margin = new RectOffset(30, 10, 5, 0);
        EditorStyles.foldoutHeader.padding = new RectOffset(18, 0, 0, 0);
        EditorStyles.foldoutHeader.fontSize = 12;
        DrawForceSettingsHeaderGroup();
        GUILayout.Space(1f);
        DrawVisualAidHeaderGroup();
        GUILayout.Space(1f);
        DrawJitterEffectHeaderGroup();
        GUILayout.Space(1f);
        EditorGUILayout.Space();
    }

    private void DrawForceSettingsHeaderGroup()
    {
        GUIContent label = new GUIContent("Force Settings", "Adjust the G Forces produced by EasyMotion");
        toggleStates.forceSettingsHeaderGroupToggled = EditorGUILayout.BeginFoldoutHeaderGroup(toggleStates.forceSettingsHeaderGroupToggled, label, EditorStyles.foldoutHeader);
        if (toggleStates.forceSettingsHeaderGroupToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.forceSettingsHeaderGroup, 1);
            DrawForceSettingsContent();
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.forceSettingsHeaderGroup, 0);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawForceSettingsContent()
    {
        GUIContent longitudinalLabel = new GUIContent("Longitudinal G Force", "Amount of longitudinal G Force produced (1-100).\n\nThis will not affect the vehicle's incline pitch mapping. It is recommended to keep this value lower to the lateral amount.");
        GUIContent lateralLabel = new GUIContent("Lateral G Force", "Amount of lateral G Force produced (1-100).\n\nThis will not affect the vehicle's incline roll mapping. Recommended value is between 45-80.");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(longitudinalLabel, skin.label);
        serializedObject.FindProperty("developerLongitudonalMotionMultiplier").floatValue = GUILayout.HorizontalSlider(serializedObject.FindProperty("developerLongitudonalMotionMultiplier").floatValue, 1, 100, skin.horizontalSlider, skin.horizontalSliderThumb);
        EditorGUILayout.IntField(Mathf.RoundToInt(serializedObject.FindProperty("developerLongitudonalMotionMultiplier").floatValue), GetIntFieldStyle(), GUILayout.Width(Screen.width / 10));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(lateralLabel, skin.label);
        serializedObject.FindProperty("developerLateralMotionMultiplier").floatValue = GUILayout.HorizontalSlider(serializedObject.FindProperty("developerLateralMotionMultiplier").floatValue, 1, 100, skin.horizontalSlider, skin.horizontalSliderThumb);
        EditorGUILayout.IntField(Mathf.RoundToInt(serializedObject.FindProperty("developerLateralMotionMultiplier").floatValue), GetIntFieldStyle(), GUILayout.Width(Screen.width / 10));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    private void DrawVisualAidHeaderGroup()
    {
        GUIContent label = new GUIContent("Visual Aids", "Display platform and force simulations.");
        toggleStates.visualAidHeaderGroupToggled = EditorGUILayout.BeginFoldoutHeaderGroup(toggleStates.visualAidHeaderGroupToggled, label, EditorStyles.foldoutHeader);
        if (toggleStates.visualAidHeaderGroupToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.visualAidHeaderGroup, 1);
            DrawVisualAidContent();
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.visualAidHeaderGroup, 0);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawVisualAidContent()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        DrawPlatformSimulationButton();
        DrawGForceSimulationButton();
        EditorGUILayout.EndVertical();
    }

    private void DrawPlatformSimulationButton()
    {
        string label = "Platform Simulation - OFF";
        if (toggleStates.platformSimulationToggled)
        {
            label = "Platform Simulation - ON";
        }
        toggleStates.platformSimulationToggled = (GUILayout.Toggle(toggleStates.platformSimulationToggled, new GUIContent(label, "Displays a simulation of a 2DOF motion platform. "), skin.button));
        if (toggleStates.platformSimulationToggled)
        {
            ActivateMotionPlatformVisualAid();
            PlayerPrefs.SetInt(EasyMotionConstants.platformSimulationButton, 1);

            if (!toggleStates.gForceSimulationToggled)
            {
                visualAidGameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-402.31f, -213.7f, 0);
            }
            else
            {
                visualAidGameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-663.1f, -213.7f, 0);
            }
        }
        else
        {
            DeactivateMotionPlatformVisualAid();
            PlayerPrefs.SetInt(EasyMotionConstants.platformSimulationButton, 0);
        }
    }

    private void ActivateMotionPlatformVisualAid()
    {
        visualAidGameObject.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void DeactivateMotionPlatformVisualAid()
    {
        visualAidGameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void DrawGForceSimulationButton()
    {
        string label = "GForce Simulation - OFF";
        if (toggleStates.gForceSimulationToggled)
        {
            label = "GForce Simulation - ON";
        }

        toggleStates.gForceSimulationToggled = (GUILayout.Toggle(toggleStates.gForceSimulationToggled, new GUIContent(label, " Displays the GForces produced by EasyMotion. "), skin.button));
        if (toggleStates.gForceSimulationToggled)
        {
            ActivateGForceVisualAid();
            PlayerPrefs.SetInt(EasyMotionConstants.gForceSimulationButton, 1);
            if (toggleStates.platformSimulationToggled)
            {
                visualAidGameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-663.1f, -213.7f, 0);
            }
        }
        else
        {
            DeactivateGForceVisualAid();
            PlayerPrefs.SetInt(EasyMotionConstants.gForceSimulationButton, 0);
            if (toggleStates.platformSimulationToggled)
            {
                visualAidGameObject.transform.GetChild(1).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-402.31f, -213.7f, 0);
            }
        }
    }

    private void ActivateGForceVisualAid()
    {
        visualAidGameObject.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DeactivateGForceVisualAid()
    {
        visualAidGameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void DrawJitterEffectHeaderGroup()
    {
        GUIContent label = new GUIContent("Jitter Effect Demonstration", "Demonstrates the StartJitterEffect() method.");
        toggleStates.jitterEffectHeaderGroupToggled = EditorGUILayout.BeginFoldoutHeaderGroup(toggleStates.jitterEffectHeaderGroupToggled, label, EditorStyles.foldoutHeader);
        if (toggleStates.jitterEffectHeaderGroupToggled)
        {
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectHeaderGroup, 1);
            DrawJitterEffectContent();
        }
        else
        {
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectHeaderGroup, 0);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }    

    private void DrawJitterEffectContent()
    {
        GUIContent label = new GUIContent("Jitter Amount", "Control the amount of jitter used in this effect demonstration. \n\nNOTE: It is recommended that StartJitterEffect() and StopJitterEffect() be used to use this effect dynamically.\n\nRefer to the User Guide for more information.");
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        DrawJitterEffectButton();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();        
        EditorGUILayout.PrefixLabel(label, skin.label);
        serializedObject.FindProperty("jitterAmount").floatValue = GUILayout.HorizontalSlider(serializedObject.FindProperty("jitterAmount").floatValue, 1, 10, skin.horizontalSlider, skin.horizontalSliderThumb);
        EditorGUILayout.IntField(Mathf.RoundToInt(serializedObject.FindProperty("jitterAmount").floatValue), GetIntFieldStyle(), GUILayout.Width(Screen.width/10));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawJitterEffectButton()
    {
        string label = "Jitter Effect - OFF";
        if (toggleStates.jitterEffectToggled)
        {
            label = "Jitter Effect - ON";
        }
        toggleStates.jitterEffectToggled = (GUILayout.Toggle(toggleStates.jitterEffectToggled, new GUIContent(label, " Demonstrates the effect of StartJitterEffect(). "), skin.button));
        if (toggleStates.jitterEffectToggled)
        {
            ActivateJitterEffect();
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectToggled, 1);
        }
        else
        {
            DeactivateJitterEffect();
            PlayerPrefs.SetInt(EasyMotionConstants.jitterEffectToggled, 0);
        }
    }

    private GUIStyle GetIntFieldStyle()
    {
        GUIStyle intFieldStyle = new GUIStyle() {
            margin = new RectOffset(10, 0, 0, 0),
            padding = new RectOffset(0,0,0,0),
            border = new RectOffset(0,0,0,0),
            stretchHeight = true
        };
        return intFieldStyle;
    }

    private void ActivateJitterEffect()
    {
        float jitterAmount = easyMotion.jitterAmount;
        easyMotion.StartJitterEffect(jitterAmount);
    }

    private void DeactivateJitterEffect()
    {
        easyMotion.StopJitterEffect();
    }

    private void OnDestroy()
    {
        EasyMotion[] easyMotions = GetActiveEnabledEasyMotions();

        if (easyMotions.Length < 1)
        {
            DestroyImmediate(GameObject.Find(EasyMotionConstants.visualAidPrefab));
        }
    }
}
