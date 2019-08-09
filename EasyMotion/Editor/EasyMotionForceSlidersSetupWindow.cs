/**
 * EasyMotion Plugin
 * Author: Ismael Florit
 * Student Number: 40009944 * 
 * 
 * Configuration window for slider controller setup.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Timers;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EasyMotionForceSlidersSetupWindow : EditorWindow
{
    private static Slider[] availableSliders;
    private Dictionary<GameObject, Texture2D> sliderGameobjectsAndImagesDictionary = new Dictionary<GameObject, Texture2D>();
    private Vector2 scrollPositionRollSlider;
    private int selectionGridIndexRollSlider = -1;
    private Vector2 scrollPositionPitchSlider;
    private int selectionGridIndexPitchSlider = -1;
    private GUISkin skin;
    private Texture2D sliderIcon;
    private Timer timer;
    private bool pingSecondObject = false;
    private bool closeWindow = true;
    private string currentScene;

    [MenuItem("EasyMotion/In-Game Menu References/Force Sliders Setup", false, 15)]
    static void Init()
    {
        EasyMotionForceSlidersSetupWindow window = (EasyMotionForceSlidersSetupWindow)EditorWindow.GetWindow(typeof(EasyMotionForceSlidersSetupWindow));
        window.Show();
        window.titleContent = new GUIContent("Force Sliders Setup", "Select the sliders the player will use to control the amount of Pitch/Roll forces produced.");
        window.minSize = new Vector2(300, 400);
        window.maxSize = new Vector2(300, 900);
    }

    private void OnEnable()
    {
        skin = (GUISkin)Resources.Load(EasyMotionConstants.pluginSkin);
        sliderIcon = (Texture2D)Resources.Load("slider");
        availableSliders = FindActiveSliders();
        GeneratePreviews();
        currentScene = SceneManager.GetActiveScene().name;
    }

    private void OnHierarchyChange()
    {
        CloseIfSceneChanged();
        ReopenIfSliderCountChanged();
    }

    private void CloseIfSceneChanged()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            Close();
        }
    }

    private void ReopenIfSliderCountChanged()
    {
        Slider[] currentSliders = FindActiveSliders();
        if (availableSliders.Length != currentSliders.Length)
        {
            EasyMotionUtility.ReOpen<EasyMotionForceSlidersSetupWindow>(
                "Force Sliders Setup",
                "Select the sliders the player will use to control the amount of Pitch/Roll forces produced.");
        }
    }

    private static Slider[] FindActiveSliders()
    {
        Slider[] availableSliders = Resources.FindObjectsOfTypeAll<Slider>();
        List<Slider> sliderList = new List<Slider>();
        foreach (Slider slider in availableSliders)
        {
            if (slider.gameObject.activeInHierarchy)
            {
                sliderList.Add(slider);
            }
        }
        return sliderList.ToArray();
    }

    private void GeneratePreviews()
    {
        foreach (Slider slider in availableSliders)
        {
            if (!sliderGameobjectsAndImagesDictionary.ContainsKey(slider.gameObject))
            {
                sliderGameobjectsAndImagesDictionary.Add(slider.gameObject, sliderIcon);
            }
        }
    }

    private void Update()
    {
        if (availableSliders.Length != sliderGameobjectsAndImagesDictionary.Count)
        {
            GeneratePreviews();
        }

        if (pingSecondObject)
        {
            EditorGUIUtility.PingObject(SliderExists(EasyMotionConstants.rollSliderSavePath).sliderScript.gameObject);
            Selection.activeGameObject = SliderExists(EasyMotionConstants.rollSliderSavePath).sliderScript.gameObject;
            pingSecondObject = false;
            if (closeWindow)
            {
                Close();

            }
        }
    }

    private void OnGUI()
    {
        DrawLogoBox();
        DrawScrolls();
        DrawApplyOrResetButton();
        DrawCloseButton();
    }

    private void DrawLogoBox()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Box("", skin.GetStyle("Logo"));
        EditorGUILayout.Space();
    }

    private void DrawScrolls()
    {
        if (availableSliders.Length < 2)
        {
            GUILayout.Label("You need two sliders to use this feature.", GetAvailableLabelStyle());
            return;
        }

        if (availableSliders.Length > 0)
        {
            DrawScrollForPitchSliders();
            DrawScrollForRollSliders();
        }
        else
        {
            GUILayout.Label("No sliders were found on this scene.", GetAvailableLabelStyle());
        }
    }

    private void DrawScrollForPitchSliders()
    {
        if (availableSliders.Length > 0 && !SliderExists(EasyMotionConstants.pitchSliderSavePath).boolean)
        {
            int rowCapacity = Mathf.FloorToInt(position.width / (110f));
            GUILayout.Label("Select Pitch Slider: ", GetAvailableLabelStyle());
            scrollPositionPitchSlider = GUILayout.BeginScrollView(scrollPositionPitchSlider);
            selectionGridIndexPitchSlider = GUILayout.SelectionGrid(
                selectionGridIndexPitchSlider,
                GetAvailableSliderPreviews(sliderGameobjectsAndImagesDictionary),
                rowCapacity,
                GetSelectionGridStyle());
            GUILayout.EndScrollView();
        }
    }

    private void DrawScrollForRollSliders()
    {
        if (availableSliders.Length > 0 && !SliderExists(EasyMotionConstants.rollSliderSavePath).boolean)
        {
            int rowCapacity = Mathf.FloorToInt(position.width / (110f));
            GUILayout.Label("Select Roll Slider: ", GetAvailableLabelStyle());
            scrollPositionRollSlider = GUILayout.BeginScrollView(scrollPositionRollSlider);
            selectionGridIndexRollSlider = GUILayout.SelectionGrid(
                selectionGridIndexRollSlider,
                GetAvailableSliderPreviews(sliderGameobjectsAndImagesDictionary),
                rowCapacity,
                GetSelectionGridStyle());
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
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
        guiStyle.wordWrap = true;
        return guiStyle;
    }

    private GUIStyle GetSelectionGridStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);
        guiStyle.alignment = TextAnchor.LowerCenter;
        guiStyle.imagePosition = ImagePosition.ImageAbove;
        guiStyle.fixedWidth = 75;
        guiStyle.fixedHeight = 40;
        guiStyle.margin = new RectOffset(24, 10, 10, 10);
        guiStyle.padding = new RectOffset(5, 5, 5, 5);
        return guiStyle;
    }

    private GUIContent[] GetAvailableSliderPreviews(Dictionary<GameObject, Texture2D> dictionary)
    {
        List<GUIContent> images = new List<GUIContent>();
        if (availableSliders.Length == sliderGameobjectsAndImagesDictionary.Count)
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

    private void DrawApplyOrResetButton()
    {

        if (availableSliders.Length >= 2 && !SliderExists(EasyMotionConstants.rollSliderSavePath).boolean && !SliderExists(EasyMotionConstants.pitchSliderSavePath).boolean)
        {
            if (GUILayout.Button("Apply", skin.button))
            {
                if (AllSlidersAreSelected() && selectionGridIndexRollSlider != selectionGridIndexPitchSlider)
                {
                    closeWindow = true;
                    EasyMotionSliderController rollSlider = availableSliders[selectionGridIndexRollSlider].gameObject.AddComponent<EasyMotionSliderController>();
                    rollSlider.SetSavePath(EasyMotionConstants.rollSliderSavePath);
                    EasyMotionSliderController pitchSlider = availableSliders[selectionGridIndexPitchSlider].gameObject.AddComponent<EasyMotionSliderController>();
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    pitchSlider.SetSavePath(EasyMotionConstants.pitchSliderSavePath);
                    PingPitchSlider();
                    SetTimer();
                }
                else
                {
                    DrawUnselectedSliderMessages();
                }
            }
        } else
        {
            if(SliderExists(EasyMotionConstants.rollSliderSavePath).boolean && SliderExists(EasyMotionConstants.pitchSliderSavePath).boolean)
            {
                EditorGUILayout.HelpBox("Sliders are already assigned.", MessageType.Info);
                DrawLocateButton();
                GUILayout.FlexibleSpace();
                DrawResetButton();
            }
        }
    }

    private (bool boolean, EasyMotionSliderController sliderScript) SliderExists(string savePath)
    {
        bool sliderExists = false;
        EasyMotionSliderController[] sliders = EasyMotionUtility.FindActiveEnabledSliderControllers();
        foreach(EasyMotionSliderController slider in sliders)
        {          
            if (slider.GetSavePath().Equals(Application.persistentDataPath + savePath))
            {
                sliderExists = true;
                return (sliderExists, slider);
            }
        }
        return (false, null);
    }

    private void DrawLocateButton()
    {
        if (GUILayout.Button("Locate assigned sliders", skin.button))
        {
            closeWindow = false;
            PingPitchSlider();
            SetTimer();
        }
    }

    private void PingPitchSlider()
    {
        EditorGUIUtility.PingObject(SliderExists(EasyMotionConstants.pitchSliderSavePath).sliderScript.gameObject);
        Selection.activeGameObject = SliderExists(EasyMotionConstants.pitchSliderSavePath).sliderScript.gameObject;
    }

    private void SetTimer()
    {
        timer = new Timer();
        timer.Interval = 700;
        timer.Enabled = true;
        timer.Elapsed += new ElapsedEventHandler(PingRollSlider);
    }

    private void PingRollSlider(object source, ElapsedEventArgs e)
    {
        timer.Stop();
        pingSecondObject = true;
    }

    private void DrawUnselectedSliderMessages()
    {
        if (selectionGridIndexPitchSlider.Equals(selectionGridIndexRollSlider))
        {
            EditorUtility.DisplayDialog("EasyMotion - Slider Selection", "Same sliders were selected.\n\nPlease select separate sliders for Roll and Pitch control.", "Ok");
            return;
        }

        if (selectionGridIndexRollSlider.Equals(-1) && selectionGridIndexPitchSlider.Equals(-1))
        {
            EditorUtility.DisplayDialog("EasyMotion - Slider Selection", "No sliders were selected.\n\nPlease select sliders for Roll and Pitch control.", "Ok");
            return;
        }
        if (selectionGridIndexRollSlider.Equals(-1))
        {
            EditorUtility.DisplayDialog("EasyMotion - Slider Selection", "No Force slider selected.\n\nPlease select a slider for Roll control.", "Ok");
        }
        if (selectionGridIndexPitchSlider.Equals(-1))
        {
            EditorUtility.DisplayDialog("EasyMotion - Slider Selection", "No Pitch slider selected.\n\nPlease select a slider for Pitch control.", "Ok");
        }
    }

    private bool AllSlidersAreSelected()
    {
        if(selectionGridIndexRollSlider != -1 && selectionGridIndexPitchSlider != -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DrawCloseButton()
    {
        if (GUILayout.Button("Close", skin.button))
        {
            Close();
        }
    }

    private void DrawResetButton()
    {
        if (GUILayout.Button("Reset", skin.button))
        {
            if (EditorUtility.DisplayDialog("EasyMotion - Force Reset", "This will remove the assigned slider controllers.\n\nDo you wish to Reset?", "Ok", "Cancel"))
            {
                ResetPlugin();
            }
        }
    }

    private void ResetPlugin()
    {
        DeleteAllSliderControllerScripts();
    }

    private void DeleteAllSliderControllerScripts()
    {
        EasyMotionSliderController[] sliderScripts = EasyMotionUtility.FindActiveEnabledSliderControllers();
        foreach(EasyMotionSliderController script in sliderScripts)
        {
            DestroyImmediate(script);
        }
        availableSliders = FindActiveSliders();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
}