using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LastKey.Editor
{
    [InitializeOnLoad]
    public static class PlayLastKey
    {
        const string ScenePath = "Assets/LastKey/Scenes/LastKey.unity";
        const string RequestPath = "Library/LastKeyPlay.request";

        static PlayLastKey()
        {
            EditorApplication.delayCall += Configure;
            EditorApplication.update += ProcessRequest;
        }

        static void Configure()
        {
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
        }

        [MenuItem("LAST KEY/Play Main Game")]
        public static void Play()
        {
            Configure();
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.isPlaying = true;
            Debug.Log("LAST_KEY_EDITOR_MAIN_GAME_STARTED");
        }

        static void ProcessRequest()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(RequestPath)) return;
            File.Delete(RequestPath);
            Play();
        }
    }
}
