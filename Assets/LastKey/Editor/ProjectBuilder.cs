using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace LastKey.Editor
{
    public static class ProjectBuilder
    {
        [MenuItem("LAST KEY/Prepare Project")]
        public static void Prepare()
        {
            CoreQa.Run();
            if(!File.Exists("Assets/TextMesh Pro/Resources/TMP Settings.asset"))
            {
                var essential=Path.Combine(EditorApplication.applicationContentsPath,"Resources/PackageManager/BuiltInPackages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage");
                AssetDatabase.ImportPackage(essential,false);
            }
            Directory.CreateDirectory("Assets/LastKey/Scenes");Directory.CreateDirectory("Assets/LastKey/Rendering");
            string rendererPath="Assets/LastKey/Rendering/LastKeyRenderer.asset",pipelinePath="Assets/LastKey/Rendering/LastKeyURP.asset";
            var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(rendererPath);
            if(renderer==null){renderer=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(renderer,rendererPath);}
            var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
            if(pipeline==null){pipeline=UniversalRenderPipelineAsset.Create(renderer);AssetDatabase.CreateAsset(pipeline,pipelinePath);}
            GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
            PlayerSettings.companyName="LAST KEY";PlayerSettings.productName="LAST KEY";PlayerSettings.defaultScreenWidth=1920;PlayerSettings.defaultScreenHeight=1080;
            PlayerSettings.fullScreenMode=FullScreenMode.Windowed;PlayerSettings.runInBackground=true;
            PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
            // Legacy input is explicitly enabled for physical typing and uGUI buttons.
            var serialized=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var input=serialized.FindProperty("activeInputHandler");if(input!=null){input.intValue=0;serialized.ApplyModifiedPropertiesWithoutUndo();}
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            var root=new GameObject("Systems");root.AddComponent<LastKeyGame>();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(),"Assets/LastKey/Scenes/LastKey.unity");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/LastKey/Scenes/LastKey.unity",true)};
            AssetDatabase.SaveAssets();Debug.Log("LAST_KEY_PREPARED "+Application.unityVersion);
        }
        [MenuItem("LAST KEY/Build Windows")]
        public static void BuildWindows()
        {
            Prepare();Directory.CreateDirectory("Builds/Windows");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/LastKey/Scenes/LastKey.unity"},locationPathName="Builds/Windows/LAST_KEY.exe",target=BuildTarget.StandaloneWindows64,options=BuildOptions.Development});
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Windows build failed "+report.summary.result);
            Debug.Log("LAST_KEY_WINDOWS_BUILD_SUCCESS "+report.summary.totalSize);
        }
        [MenuItem("LAST KEY/Build WebGL")]
        public static void BuildWebGL()
        {
            Prepare();PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;
            Directory.CreateDirectory("Builds/WebGL");
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/LastKey/Scenes/LastKey.unity"},locationPathName="Builds/WebGL",target=BuildTarget.WebGL,options=BuildOptions.None});
            if(report.summary.result!=BuildResult.Succeeded)throw new Exception("WebGL build failed "+report.summary.result);
            File.AppendAllText("Builds/WebGL/TemplateData/style.css", "\n/* Fit the full 16:9 HUD in desktop and compact browser views. */\n#unity-fullscreen-container { width:min(100vw,calc((100vh - 38px) * 16 / 9)) !important; height:auto !important; aspect-ratio:16/9; }\n#unity-container.unity-desktop { width:min(100vw,calc((100vh - 38px) * 16 / 9)); }\n");
            Debug.Log("LAST_KEY_WEBGL_BUILD_SUCCESS "+report.summary.totalSize);
        }
        public static void PlayEvidence()
        {
            CoreQa.Run();
            EditorSceneManager.OpenScene("Assets/LastKey/Scenes/LastKey.unity");
            // Set a fixed Game view size so runtime PNGs are exactly 1920 x 1080.
            var assembly=typeof(UnityEditor.Editor).Assembly;
            var gameViewType=assembly.GetType("UnityEditor.GameView");
            var view=EditorWindow.GetWindow(gameViewType);
            var sizesType=assembly.GetType("UnityEditor.GameViewSizes");
            var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var sizes=singleton.GetProperty("instance").GetValue(null);
            var getGroup=sizesType.GetMethod("GetGroup");
            var group=getGroup.Invoke(sizes,new object[]{Enum.Parse(getGroup.GetParameters()[0].ParameterType,"Standalone")});
            var sizeType=assembly.GetType("UnityEditor.GameViewSize");
            var kind=assembly.GetType("UnityEditor.GameViewSizeType");
            var size=Activator.CreateInstance(sizeType,new object[]{Enum.Parse(kind,"FixedResolution"),1920,1080,"LAST KEY Evidence 1080p"});
            int count=(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null);
            group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});
            gameViewType.GetProperty("selectedSizeIndex",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic).SetValue(view,count);
            view.Show();view.Focus();EditorApplication.isPlaying=true;
        }
    }
}
