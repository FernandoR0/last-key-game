# Reexportar los modelos de Blender Studio

Los FBX y prefabs listos para Unity están incluidos en Assets/LastKey/Visual/Characters/BlenderStudio y Assets/Resources/VisualCharacters.

Para reexportar, descargar las fuentes oficiales gratuitas Rain v3 / Snow v4 indicadas en ASSET_MANIFEST.md. Usar Blender 4.2 LTS:

blender --background --disable-autoexec <archivo.blend> --python Tools/export_studio_characters.py -- rain <carpeta-salida>

Usar snow para la otra fuente. La carpeta de salida del proyecto es Assets/LastKey/Visual/Characters/BlenderStudio.

Después ejecutar LastKey.Editor.VisualAssetSetup.SetupModels en Unity para regenerar los prefabs y arrays de huesos. No basta con sobrescribir el FBX cuando cambian sus pesos/bindposes.

Se preservan los pesos de cuello/pecho de las mallas de cabeza y solo se incluyen huesos deformantes. Se conserva la máscara original de piel; no se recortan caras visibles por proximidad a ropa. No se modela ropa nueva. Los morphs se hornean de controles de expresión originales, con scripts internos del blend desactivados.
