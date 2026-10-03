using UnityEditor;
namespace LastKey.Editor {public sealed class ModularRigImporter:AssetPostprocessor {void OnPreprocessModel(){if(!assetPath.Contains("/ModularCharacters/")||!(assetPath.Contains("Men_")||assetPath.Contains("Women_")))return;var i=(ModelImporter)assetImporter;i.animationType=ModelImporterAnimationType.Generic;i.importAnimation=false;}}}
