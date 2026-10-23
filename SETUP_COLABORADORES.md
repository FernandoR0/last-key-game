# Preparar LAST KEY para colaborar

## Entorno

- Unity Hub y Unity Editor **6000.6.0f1**, con licencia personal activa.
- Git; Git LFS si se consultan versiones históricas con archivos LFS.
- Windows Build Support / WebGL Build Support para compilar esos destinos.
- .NET SDK **10** solo para ejecutar `Tests/LastKey.Core.Tests.csproj` fuera de Unity.

Unity restaura las dependencias de `Packages/manifest.json`: URP 17.6.0 y uGUI 2.6.0, entre otras. Conserva los `.meta` al mover o modificar assets.

## Personaje actual: fuente separada

El repositorio público no distribuye el modelo fuente ActorCore ni los FBX Mixamo. Cada integrante debe obtener los recursos desde su cuenta y bajo las condiciones de esos proveedores. El código y las capturas del juego sí están incluidos.

Personaje: [ActorCore Party-M-0001](https://actorcore.reallusion.com/3d-character/free?asset=party-m-0001).

Guarda el FBX y sus texturas con esta estructura local:

```text
Assets/PrivateActorCore/Male/Actor/party-m-0001/
  party-m-0001.fbx
  party-m-0001.fbm/
    Character_Pbr_Diffuse.png
    Character_Pbr_Normal.jpg
```

Si la descarga oficial usa otros nombres, conserva el original fuera de Git y prepara una copia con esos nombres. La importación debe conservar esqueleto humanoide, malla y blendshapes.

## Animaciones

Obtén los clips desde [Mixamo](https://www.mixamo.com/). Exporta FBX para Unity con esqueleto humanoide y, cuando esté disponible, 60 fps. Los nombres del archivo local deben ser exactamente estos:

| Archivo en `Assets/PrivateMixamo/` | Clip de origen |
|---|---|
| SittingIdleBreathing.fbx | Sitting Idle / respiración sentado |
| WakingSeated.fbx | Waking Seated |
| BeingStrangled.fbx | Being Strangled |
| Typing.fbx | Typing |
| BeingElectrocuted.fbx | Being Electrocuted |
| SittingVictory.fbx | Sitting Victory |
| SittingLookAround.fbx | Sitting Look Around |
| SittingDisbeliefForward.fbx | Sitting Disbelief / Forward |
| SitToType.fbx | Sit To Type |
| TypeToSit.fbx | Type To Sit |
| LookingDown.fbx | Looking Down |

No es necesario comprar un paquete de animaciones. La revisión de tecleo usa el tramo 2,6–7,8 s del Typing original de 16,47 s: una variante diferente del clip puede requerir revisar ese intervalo.

## Comprobar y abrir

```bash
python Tools/check_setup.py
```

El chequeo es de presencia de archivos; la validez del rig y de los clips se comprueba al importarlos en Unity. Si faltan fuentes, el script indica cuáles; no las sustituye ni las descarga desde sitios alternativos.

Después de importar, ejecuta el método `LastKey.Editor.ProductionVisualSetup.Prepare` con el Editor cerrado si usas la línea de comandos:

```text
Unity.exe -batchmode -quit -projectPath <ruta-del-clon> -executeMethod LastKey.Editor.ProductionVisualSetup.Prepare
```

Ese método configura Humanoid, materiales, once clips y vuelve a generar el prefab aprobado con los GUID de tu importación local. Luego abre `Assets/LastKey/Scenes/LastKey.unity` y pulsa Play.

No envíes cambios del prefab/material generado solo por regenerar GUID en otro equipo; conserva esos cambios localmente y separa cualquier cambio visual intencional en su propia rama.

## Probar WebGL local

Después de compilar WebGL:

```bash
python Tools/serve_review.py --directory Builds/WebGL --port 8765
```

Abre `http://127.0.0.1:8765/`. Ese enlace solo funciona mientras el servidor está activo en tu equipo. Para que otros prueben una build sin instalar Unity, hace falta publicarla por separado; el repositorio no contiene todavía una build alojada.
