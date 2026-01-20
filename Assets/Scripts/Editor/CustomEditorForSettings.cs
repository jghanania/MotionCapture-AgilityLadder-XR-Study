using UnityEditor;
using UnityEngine;
using static SettingsManager;

[CustomEditor(typeof(SettingsManager))]
public class CustomButtonForSettings : Editor
{

    private bool showDefaultInspector = false;
    private readonly int _toggleHeight = 25;

    public override void OnInspectorGUI()
    {
        // Get a reference to the target script
        SettingsManager settings = (SettingsManager)target;

        // Styling for of the buttons for the in Game Action (e.g. next field)
        GUIStyle inGameActionStyle = new(GUI.skin.button)
        {
            fixedHeight = 35
        };

        // Foldout toggle
        showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "Controller and Scripts");

        if (showDefaultInspector)
        {
            // Draw the default inspector inside the foldout
            EditorGUI.indentLevel++;
            base.OnInspectorGUI();
            EditorGUILayout.Space(20);
            EditorGUI.indentLevel--;
        }


        // Subject Information
        EditorGUILayout.LabelField("Subject Information", EditorStyles.boldLabel);
        DrawIntFieldWithButtons("Subject ID", ref settings.SubjectID, settings.SetBalancedLatinSquare);
        DrawMultipleChoiceButton("Gender", ref settings.SubjectGender, settings.SetSubjectGender);
        DrawIntFieldWithButtons("Shoe Size", ref settings.SubjectShoeSize);


        // Spacing between settings groups
        EditorGUILayout.Space(20);

        // OptiTrack settings, avatar scale and camera controll
        EditorGUILayout.LabelField("Optitrack Settings", EditorStyles.boldLabel);
        DrawMultipleChoiceButton("Show Marker", ref settings.OptitrackController.ShowMarker, settings.OptitrackController.ToggleOptiTrackMarker);// Field for Avatar Scaling
        DrawPreciseFloatWithButtons("Avatar Scaling", ref settings.OptitrackController.AvatarScale, 0.001f, 0.01f, settings.OptitrackController.ScaleAvatar);


        if (GUILayout.Button("Align Camera", inGameActionStyle))
        {
            // Call the AlignCam method in the target script
            settings.StartCoroutine(settings.OptitrackController.AlignCam());
        }

        EditorGUILayout.Space();




        // Settings for the current condition and path
        EditorGUILayout.LabelField("Conditions Settings", EditorStyles.boldLabel);

        DrawConditionCounter(ref settings.ConditionNumber, settings.latinSquare.GetLength(0), settings.SetBalancedLatinSquare);
        DrawMultipleChoiceButton("Environment", ref settings.EnvironmentSetting, settings.SetEnvironment);
        DrawMultipleChoiceButton("Visualization", ref settings.VisualizationSetting, settings.SetVisualization);
        DrawMultipleChoiceButton("Current Path", ref settings.PathSetting, settings.AgilityLadderController.ResetPath);


        EditorGUILayout.Space();




        // Button to log the step and show next field
        if (GUILayout.Button("Log Step & Show Next Field", inGameActionStyle))
        {

            settings.AgilityLadderController.NextField();
        }


        // ---------- EXAMPLES -----------

        //if (GUILayout.Button("NAME_OF_YOUR_BUTTON", bigButtonStyle))
        //{
        //    settings.NAME_OF_FUNCTION_FROM_TARGET_SCRIPT();
        //}

        // Too create a Label
        // EditorGUILayout.LabelField("YOUR_LABEL_HERE", EditorStyles.boldLabel);

        // Generate fields with increment/decrement buttons
        // DrawIntFieldWithButtons("NAME_OF_YOUR_FIELD", ref settings.VARIABLE_NAME_IN_TARGET_SCRIPT, (optional) settings.FUNCTION_TO_TRIGGER_UPON_PRESSING);

        // Draw multiple choice button group for Gender selection
        // DrawMultipleChoiceButton("NAME_OF_YOUR_FIELD", ref settings.ENUM_INSTANCE_NAME_IN_TARGET_SCRIPT, (optional) settings.FUNCTION_TO_TRIGGER_UPON_PRESSING);

        // Draw float field with fine and coarse increment/decrement buttons for Avatar Scaling
        // DrawPreciseFloatWithButtons("Avatar Scaling", ref settings.AvatarScale, 0.001f, 0.01f, (optional) settings.FUNCTION_TO_TRIGGER_UPON_PRESSING);

        // Draw numbered button to represent all of the conditions for one subject
        // DrawConditionCounter(ref settings.INT_FIELD_FOR_CURRENT_CONDITION, settings.NUMBER_OF_CONDITIONS, (optional) settings.FUNCTION_TO_TRIGGER_UPON_PRESSING)

        // Draw a sim
        // DrawToggleButton("NAME_OF_YOUR_FIELD", ref settings.BOOLEAN_CONTROLLING_STATE, "TEXT_IN_BUTTON_WHEN_ON", "TEXT_IN_BUTTON_WHEN_OFF", (optional) settings.FUNCTION_TO_TRIGGER_UPON_PRESSING)

        // DrawToggleButtonWithFloatField("NAME_OF_YOUR_FIELD", ref settings.BOOLEAN_CONTROLLING_STATE, "TEXT_IN_BUTTON_WHEN_ON", "TEXT_IN_BUTTON_WHEN_OFF", ref settings.FLOAT_TO_TRACK, "UNIT_OF_FLOAT", (optional) settings.FUNCTION_TO_TRIGGER_UPON_PRESSING)

        // Spacing between fields or buttons
        // EditorGUILayout.Space()
    }


    private void DrawIntFieldWithButtons(string label, ref int field, System.Action onChange = null)
    {
        EditorGUILayout.BeginHorizontal();

        // Reserve rect for the draggable field
        Rect fieldRect = EditorGUILayout.GetControlRect();
        field = EditorGUI.IntField(fieldRect, new GUIContent(label), field); // Drag-enabled

        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
            field--;  // Decrease button
            onChange?.Invoke();
        }
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            field++;  // Increase button
            onChange?.Invoke();
        }

        EditorGUILayout.EndHorizontal();
    }


    private void DrawPreciseFloatWithButtons(string label, ref float field, float smallStep, float largeStep, System.Action onChange = null)
    {
        EditorGUILayout.BeginHorizontal();

        Rect fieldRect = EditorGUILayout.GetControlRect();
        field = EditorGUI.FloatField(fieldRect, new GUIContent(label), field); // Drag-enabled

        if (GUILayout.Button("--", GUILayout.Width(30)))
        {
            field = Mathf.Round((field - largeStep) * 1000f) / 1000f;
            onChange?.Invoke();
        }
        if (GUILayout.Button("-", GUILayout.Width(30)))
        {
            field = Mathf.Round((field - smallStep) * 1000f) / 1000f;
            onChange?.Invoke();
        }
        if (GUILayout.Button("+", GUILayout.Width(30)))
        {
            field = Mathf.Round((field + smallStep) * 1000f) / 1000f;
            onChange?.Invoke();
        }
        if (GUILayout.Button("++", GUILayout.Width(30)))
        {
            field = Mathf.Round((field + largeStep) * 1000f) / 1000f;
            onChange?.Invoke();
        }

        EditorGUILayout.EndHorizontal();
    }


    private void DrawMultipleChoiceButton<T>(string label, ref T enumField, System.Action onChange = null, bool enumIsBig = false) where T : System.Enum
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label(label, GUILayout.Width(EditorGUIUtility.labelWidth));

        float totalWidth = 0f;
        float labelWidth = enumIsBig ? 0 : EditorGUIUtility.labelWidth;
        float maxWidth = EditorGUIUtility.currentViewWidth - labelWidth - 40f; // adjust for padding
        if (enumIsBig)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
        }

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();

        foreach (string name in System.Enum.GetNames(typeof(T)))
        {
            var option = (T)System.Enum.Parse(typeof(T), name);
            GUIContent content = new(name);
            Vector2 size = GUI.skin.button.CalcSize(content);
            float buttonWidth = size.x + 10f; // padding

            // If adding this button would exceed the max width, wrap to new line
            if (totalWidth + buttonWidth + 10 > maxWidth)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                totalWidth = 0f;
            }

            bool isSelected = enumField.Equals(option);
            bool clicked = GUILayout.Toggle(isSelected, name, "Button", GUILayout.ExpandWidth(true), GUILayout.Height(_toggleHeight));

            if (clicked && !isSelected)
            {
                enumField = option;
                onChange?.Invoke();
            }

            totalWidth += buttonWidth;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

    }






    private void DrawConditionCounter(ref int value, int buttonCount, System.Action onChange = null)
    {
        float maxWidth = EditorGUIUtility.currentViewWidth; // Subtract padding for scrollbars, margins, etc.
        int minButtonWidth = 30;
        int maxButtonsPerRow = Mathf.Max(1, Mathf.FloorToInt((maxWidth - (buttonCount - 1) * 5f) / minButtonWidth));

        int optimalRowCount = Mathf.CeilToInt((float)buttonCount / maxButtonsPerRow);

        int buttonsRemaining = buttonCount;
        int buttonIndex = 0;

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        int buttonValue = buttonIndex;
        bool isSelected = (value == buttonValue);

        if (GUILayout.Toggle(isSelected, buttonValue.ToString(), "Button", GUILayout.Height(_toggleHeight)))
        {
            if (!isSelected)
            {
                value = buttonValue;
                onChange?.Invoke();
            }
        }
        EditorGUILayout.EndHorizontal();

        for (int row = 0; row < optimalRowCount; row++)
        {
            int rowsLeft = optimalRowCount - row;
            int buttonsInThisRow = Mathf.CeilToInt((float)buttonsRemaining / rowsLeft);
            float buttonWidth = (maxWidth - (buttonsInThisRow - 1) * 5f - 10f) / buttonsInThisRow;

            EditorGUILayout.BeginHorizontal();

            for (int i = 0; i < buttonsInThisRow; i++)
            {
                buttonValue = buttonIndex + 1;
                isSelected = (value == buttonValue);

                if (GUILayout.Toggle(isSelected, buttonValue.ToString(), "Button", GUILayout.Width(buttonWidth), GUILayout.Height(_toggleHeight)))
                {
                    if (!isSelected)
                    {
                        value = buttonValue;
                        onChange?.Invoke();
                    }
                }

                buttonIndex++;
                buttonsRemaining--;

                if (buttonIndex >= buttonCount)
                    break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

}

