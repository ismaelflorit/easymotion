/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Initial Setup configuration window to guide user through menu dependencies.
 */

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EasyMotionInitialSetup : EditorWindow
{
    private GUISkin skin;
    private bool beginWizard;
    private bool beginToggleStep;
    private bool beginSerialPortStep;
    private bool beginForceSliderStep;
    private bool skippedSliderStep;
    private bool finishWizard;
    private Color green = new Color(0.1086686f, 0.6226415f, 0.1173808f, 1);
    private string currentScene;

    [MenuItem("EasyMotion/Initial Setup", false, 1)]
    static void Init()
    {
        EasyMotionInitialSetup window = GetWindow<EasyMotionInitialSetup>();
        window.Show();
        window.titleContent = new GUIContent("Initial Setup", "Configuration Wizard");
        window.minSize = new Vector2(500, 450);
        window.maxSize = new Vector2(500, 450);        
    }

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load(EasyMotionConstants.pluginSkin);
        beginWizard = true;
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void OnHierarchyChange()
    {
        CloseIfSceneChanged();        
    }

    private void CloseIfSceneChanged()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            Close();
        }
    }

    private void OnGUI()
    {
        MapSetupProgress();
    }

    private void MapSetupProgress()
    {
        if (beginWizard)
        {
            DrawWelcome();
        }
        if (beginToggleStep)
        {
            DrawToggleStep();
        }
        if (FoundToggleController() && beginSerialPortStep)
        {
            beginToggleStep = false;
            DrawSerialPortStep();
        }
        if (FoundDropdownController() && beginForceSliderStep)
        {
            beginSerialPortStep = false;
            DrawForceSlidersStep();
        }
        if ((FoundSliderController() && finishWizard) || (skippedSliderStep && finishWizard))
        {
            beginForceSliderStep = false;
            DrawEndOfWizard();
        }
    }

    private bool FoundToggleController()
    {
        bool found = false;
        EasyMotionToggleController[] easyMotionToggleControllers = Resources.FindObjectsOfTypeAll<EasyMotionToggleController>();
        if(easyMotionToggleControllers.Length > 0)
        {
            found = true;
        }
        return found;
    }

    private bool FoundDropdownController()
    {
        bool found = false;
        EasyMotionSerialPortDropdownController[] easyMotionSerialPortDropdownControllers = Resources.FindObjectsOfTypeAll<EasyMotionSerialPortDropdownController>();
        if (easyMotionSerialPortDropdownControllers.Length > 0)
        {
            found = true;
        }
        return found;
    }

    private bool FoundSliderController()
    {
        bool found = false;
        EasyMotionSliderController[] easyMotionSliderControllers = Resources.FindObjectsOfTypeAll<EasyMotionSliderController>();
        if (easyMotionSliderControllers.Length > 0)
        {
            found = true;
        }
        return found;
    }

    void DrawLogoBox()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Box("", skin.GetStyle("Logo"));
        EditorGUILayout.Space();
    }

    private void DrawWelcome()
    {
        DrawLogoBox();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Welcome to the EasyMotion Configuration Wizard", GetWelcomeStyle());
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("In order for EasyMotion to operate, the following items must be setup:", GetTextAreaStyle());
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1. Platform On/Off Toggle Reference", GetListItemStyle(FontStyle.Bold, Color.black));
        EditorGUILayout.LabelField("2. Serial Port COM Dropdown Reference", GetListItemStyle(FontStyle.Bold, Color.black));
        EditorGUILayout.LabelField("3. Force Slider References (Optional)", GetListItemStyle(FontStyle.Bold, Color.black));
        EditorGUILayout.LabelField("4. Rigidbody Reference (Final)", GetListItemStyle(FontStyle.Bold, Color.black));
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("This wizard will guide you through the necessary steps for setting up EasyMotion.", GetTextAreaStyle());
        if (FindAnyEasyMotionScript())
        {
            EditorGUILayout.HelpBox("An EasyMotion component was found. Running this wizard will reset any assigned scripts.", MessageType.Warning, true);
        }        
        EditorGUILayout.BeginHorizontal(GetButtonStyle());

        if (FindAnyEasyMotionScript())
        {
            if (GUILayout.Button("Next >", skin.button))
            {
                if (EditorUtility.DisplayDialog("EasyMotion - Configuration Wizard", "\nThis wizard will reset any EasyMotion scripts.\n\nDo you wish to continue?", "Ok", "Cancel"))
                {
                    DestroyAllScripts();
                    beginWizard = false;
                    beginToggleStep = true;
                }
            }
        }
        else
        {
            if (GUILayout.Button("Next >", skin.button))
            {
                beginWizard = false;
                beginToggleStep = true;
            }
        }
        if (GUILayout.Button("Cancel", skin.button))
        {
            this.Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawToggleStep()
    {
        DrawLogoBox();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1/3   Platform On-Off Toggle Reference", GetWelcomeStyle());
        EditorGUILayout.LabelField("\nEasyMotion will only collect and generate motion data if a player enables the platform within a settings menu.\n\nSelect the toggle you wish to use to allow the player to do so.", GetTextAreaStyle());
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1. Platform On/Off Toggle Reference", GetListItemStyle(FontStyle.Bold, Color.black));
        EditorGUILayout.LabelField("2. Serial Port COM Dropdown Reference", GetListItemStyle(FontStyle.Normal, Color.grey));
        EditorGUILayout.LabelField("3. Force Slider References (Optional)", GetListItemStyle(FontStyle.Bold, Color.grey));
        EditorGUILayout.LabelField("4. Rigidbody Reference (Final)", GetListItemStyle(FontStyle.Bold, Color.grey));
        GUILayout.FlexibleSpace();
        if (FoundToggleController())
        {
            EditorGUILayout.HelpBox("Toggle was previously assigned. You can reset this through EasyMotion > In-Game References > On-Off Toggle Setup", MessageType.Info, true);
        }
        EditorGUILayout.BeginHorizontal(GetButtonStyle());
        if (FoundToggleController())
        {
            if(GUILayout.Button("Next >", skin.button))
            {
                beginSerialPortStep = true;
            }
        }
        else
        {
            if (GUILayout.Button("Select Toggle", skin.button))
            {
                beginSerialPortStep = true;
                if (!FoundToggleController())
                {
                    EasyMotionToggleSetupWindow window = GetWindow<EasyMotionToggleSetupWindow>(typeof(EasyMotionInitialSetup));
                    window.ShowAuxWindow();
                    window.titleContent = new GUIContent("Toggle Setup", "Select the toggle which will enable the player to enable or disable the motion platform.");
                    window.minSize = new Vector2(300, 400);
                    window.maxSize = new Vector2(300, 900);
                }
            }
        }
        if (GUILayout.Button("Cancel", skin.button))
        {
            if(EditorUtility.DisplayDialog("EasyMotion - Configuration Wizard", "\n\nExit Wizard?", "Ok", "Cancel"))
            {
                this.Close();
            }
        }
        EditorGUILayout.EndHorizontal();
    }


    private void DrawSerialPortStep()
    {
        DrawLogoBox();
        this.titleContent = new GUIContent("2/3 Setup Wizard");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("2/3   Serial Port COM Dropdown Reference", GetWelcomeStyle());
        EditorGUILayout.LabelField("\nEasyMotion communicates directly with the motion platform through its serial port.\n\nSelect the dropdown which will hold all available COM ports.", GetTextAreaStyle());
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1. Platform On/Off Toggle Reference \u221A", GetListItemStyle(FontStyle.Normal, green));
        EditorGUILayout.LabelField("2. Serial Port COM Dropdown Reference", GetListItemStyle(FontStyle.Bold, Color.black));
        EditorGUILayout.LabelField("3. Force Slider References (Optional)", GetListItemStyle(FontStyle.Bold, Color.grey));
        EditorGUILayout.LabelField("4. Rigidbody Reference (Final)", GetListItemStyle(FontStyle.Bold, Color.grey));
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal(GetButtonStyle());
        if (GUILayout.Button("Select Dropdown", skin.button))
        {
            beginForceSliderStep = true;
            if (!FoundDropdownController())
            {
                EasyMotionSerialPortSetupWindow window = GetWindow<EasyMotionSerialPortSetupWindow>(typeof(EasyMotionInitialSetup));
                window.Show();
                window.titleContent = new GUIContent("Serial Port Setup", "Select the dropdown the player will use to select their motion platform's serial port");
                window.minSize = new Vector2(300, 400);
                window.maxSize = new Vector2(300, 900);
            }
        }
        if (GUILayout.Button("Cancel", skin.button))
        {
            if (EditorUtility.DisplayDialog("EasyMotion - Configuration Wizard", "All assigned elements will be reset.\n\nAre you sure you wish to exit the wizard?", "Ok", "Cancel"))
            {
                DestroyAllScripts();
                this.Close();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawForceSlidersStep()
    {
        DrawLogoBox();
        this.titleContent = new GUIContent("3/3 Setup Wizard");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("3/3   Force Slider References", GetWelcomeStyle());
        EditorGUILayout.LabelField("You may wish to give your player some level of control over how much Pitch and Roll forces the plugin will create." +
            "If so, you can assign 2 sliders which will act as overall multipliers.", GetTextAreaStyle());
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1. Platform On/Off Toggle Reference \u221A", GetListItemStyle(FontStyle.Normal, green));
        EditorGUILayout.LabelField("2. Serial Port COM Dropdown Reference \u221A", GetListItemStyle(FontStyle.Normal, green));
        EditorGUILayout.LabelField("3. Force Slider References (Optional)", GetListItemStyle(FontStyle.Bold, Color.black));
        EditorGUILayout.LabelField("4. Rigidbody Reference (Final)", GetListItemStyle(FontStyle.Bold, Color.black));
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal(GetButtonStyle());
        if (GUILayout.Button("Select Sliders", skin.button))
        {
            finishWizard = true;
            if (!FoundSliderController())
            {
                EasyMotionForceSlidersSetupWindow window = GetWindow<EasyMotionForceSlidersSetupWindow>(typeof(EasyMotionInitialSetup));
                window.Show();
                window.titleContent = new GUIContent("Sliders Setup", "Select the sliders the player will use to control produced forces.");
                window.minSize = new Vector2(300, 400);
                window.maxSize = new Vector2(300, 900);
            }
        }
        if (GUILayout.Button("Skip", skin.button))
        {
            finishWizard = true;
            skippedSliderStep = true;
        }
        try
        {
            if (GUILayout.Button("Cancel", skin.button))
            {
                if (EditorUtility.DisplayDialog("EasyMotion - Configuration Wizard", "All assigned elements will be reset.\n\nAre you sure you wish to exit the wizard?", "Ok", "Cancel"))
                {
                    DestroyAllScripts();
                    this.Close();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        catch (Exception) { }
    }

    private void DrawEndOfWizard()
    {
        DrawLogoBox();
        this.titleContent = new GUIContent("Setup Complete");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Initial Setup Complete!", GetWelcomeStyle());
        EditorGUILayout.LabelField("You now have all the menu necessary components setup for use with EasyMotion.\n\n" +
            "\n\nNOTE: You must now assign a Rigidbody for motion tracking. You can do so via the Rigidbody Motion Setup menu item. (EasyMotion > Rigidbody Motion Setup)", GetTextAreaStyle());
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("1. Platform On/Off Toggle Reference \u221A", GetListItemStyle(FontStyle.Normal, green));
        EditorGUILayout.LabelField("2. Serial Port COM Dropdown Reference \u221A", GetListItemStyle(FontStyle.Normal, green));
        if (skippedSliderStep)
        {
            EditorGUILayout.LabelField("3. Force Slider References (Optional)", GetListItemStyle(FontStyle.Italic, Color.grey));
        }
        else
        {
            EditorGUILayout.LabelField("3. Force Slider References \u221A", GetListItemStyle(FontStyle.Normal, green));

        }
        EditorGUILayout.LabelField("4. Rigidbody Reference (Final)", GetListItemStyle(FontStyle.Bold, Color.black));
        GUILayout.FlexibleSpace();
        try
        {
            EditorGUILayout.HelpBox("For more information refer to the Readme found in the EasyMotion menu. \n(EasyMotion > Help)", MessageType.Info, true);
            EditorGUILayout.BeginHorizontal(GetButtonStyle());
            if (GUILayout.Button("Finish", skin.button))
            {
                this.Close();
            }
            EditorGUILayout.EndHorizontal();
        }
        catch (Exception){
        }
     }

    private GUIStyle GetWelcomeStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label)
        {
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(8, 2, 2, 0),
            fontStyle = FontStyle.Bold,
            fontSize = 12
        };
        return guiStyle;
    }

    private GUIStyle GetTextAreaStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label)
        {
            margin = new RectOffset(8, 8, 8, 8),
            padding = new RectOffset(8, 2, 0, 2),
            fontStyle = FontStyle.Normal,
            wordWrap = true
        };
        return guiStyle;
    }

    private GUIStyle GetListItemStyle(FontStyle style, Color color)
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label)
        {
            padding = new RectOffset(50, 1, 1, 1),
            fontStyle = FontStyle.Bold
        };
        guiStyle.normal.textColor = color;
        return guiStyle;
    }

    private GUIStyle GetButtonStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.label)
        {
            padding = new RectOffset(280, 0, 0, 8),
            fontStyle = FontStyle.Normal
        };
        return guiStyle;
    }

    private bool FindAnyEasyMotionScript()
    {
        bool foundScript = false;
        EasyMotionToggleController[] toggleScripts = EasyMotionUtility.FindActiveEnabledToggleControllers();
        EasyMotionSerialPortDropdownController[] portScripts = EasyMotionUtility.FindActiveEnabledDropdownControllers();
        EasyMotion[] easyMotionScripts = EasyMotionUtility.FindActiveEnabledEasyMotions();
        EasyMotionSliderController[] sliderScripts = EasyMotionUtility.FindActiveEnabledSliderControllers();

        if(toggleScripts.Length > 0 || portScripts.Length > 0 || easyMotionScripts.Length > 0 || sliderScripts.Length > 0)
        {
            foundScript = true; 
        }
        return foundScript;
    }

    private void DestroyAllScripts()
    {
        EasyMotionToggleController[] toggleScripts = EasyMotionUtility.FindActiveEnabledToggleControllers();
        EasyMotionSerialPortDropdownController[] portScripts = EasyMotionUtility.FindActiveEnabledDropdownControllers();
        EasyMotionSliderController[] sliderScripts = EasyMotionUtility.FindActiveEnabledSliderControllers();

        foreach(EasyMotionToggleController toggleScript in toggleScripts)
        {
            DestroyImmediate(toggleScript);
        }
        foreach (EasyMotionSerialPortDropdownController portScript in portScripts)
        {
            DestroyImmediate(portScript);
        }
        foreach (EasyMotionSliderController sliderScript in sliderScripts)
        {
            DestroyImmediate(sliderScript);
        }
    }

    private bool NeededComponentsExist()
    {
        Toggle[] toggles = EasyMotionUtility.FindActiveToggles();
        Dropdown[] dropdowns = EasyMotionUtility.FindActiveDropdowns();
        bool neededComponentsExist = true;
        if(toggles.Length < 1 || dropdowns.Length < 1 )
        {
            neededComponentsExist = false;
        }
        return neededComponentsExist;
    }
}