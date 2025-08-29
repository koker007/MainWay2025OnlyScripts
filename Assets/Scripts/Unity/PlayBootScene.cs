using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public static class BootSceneButton
{
    private const string PATH_BOOT_SCENE = "Assets/Scenes/BootScene.unity";

    static BootSceneButton()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("▶ Launch", "Play from BootScene"), GUILayout.Width(70)))
        {
            PlayBootScene();
        }
    }

    private static void PlayBootScene()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Already in play mode");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(PATH_BOOT_SCENE);
            EditorApplication.isPlaying = true;
        }
    }
}
