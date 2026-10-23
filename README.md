# LAST KEY

Actualización 31: tecleo Mixamo 2,6–7,8 s, diez dedos y muñecas originales; contacto con la mesa sin fijar la altura. Evidencia: TYPING_REVIEW_31.md.

Competencia de mecanografía por rondas: cuatro participantes, una persona y tres bots. Gana la mayor puntuación acumulada tras cinco rondas de 60 segundos; Danger 100% elimina únicamente de la ronda actual.

## Abrir

Unity 6000.6.0f1 / URP 17.6.0. Escena Assets/LastKey/Scenes/LastKey.unity. Builds: Builds/Windows/LAST_KEY.exe y Builds/WebGL/.

Para WebGL: servir Builds/WebGL por HTTP en 127.0.0.1:8765 y abrir http://127.0.0.1:8765/?revision=31. El enlace es local a esta PC. Iniciar desde el menú para conservar el HUD completo. La opción histórica visual=1 oculta parte del HUD y no sirve para revisar esta integración.

## Diseño integrado — revisión 30

Cuatro copias temporales del ActorCore Party-M-0001 aprobado, con las identidades CIPHER/UPLINK/RELAY/VECTOR. Se integra el personaje y la secuencia de la revisión cinematográfica 25 en la partida principal. Se preparan once clips Mixamo existentes para despertar, inspección, mirar abajo, forcejeo, tecleo, transiciones sentadas, descarga y victoria. El forcejeo conserva los dedos originales solicitados. El collar frontal se deforma con el cuello; la fuente del personaje no se modifica.

Interfaz nativa Unity basada en el boceto 26: texto flotante sin recuadro, clasificación acumulada en vivo, WPM/precisión/errores/combo/Danger y avisos NULL/ECHO. Menú claro con «Modos de juego», sin controles de crear o unirse a salas. Classic principal; Code, Numbers, Memory y Sudden Death beta; Chaos indicado como próximo. Volumen general y música tienen controles reales y porcentajes, con navegación mediante Tab y confirmación con Enter. Esc pausa la partida. La introducción completa se reproduce solo en la primera ronda.

Detalles y evidencia actuales: UI_INTEGRATION_30.md e IMPLEMENTATION_STATUS.md. Las revisiones anteriores son históricas. La música, las voces pendientes y las licencias siguen sus condiciones previas.

Música suministrada con una sola fuente activa, mute persistente y cierre del audio propio; efectos gratuitos. Se verificó señal MASTER sin clipping en ocho clips. La escucha perceptual no se declara completada.

## Reglas

Carácter correcto +1 multiplicado por racha; palabra +8. Cada 20 de combo aumenta multiplicador hasta x4. Error +8 Danger; correcto −0.35. NULL rota Precision Protocol, Speed Check y Clean Streak. ECHO opcional dura 12 segundos, éxito +150 puntos/−15 Danger, fallo +15 Danger. WPM = correctos /5/minutos activos. Accuracy = correctos/(correctos+errores). Bots pasan por la misma validación.

Classic principal; Code/Numbers/Memory/Sudden Death BETA. Code copia texto sin ejecutarlo ni evaluar programación. Memory revela cuatro segundos. Chaos y Story Mode no implementados. Configuración en Assets/Resources/last_key_rules.json y last_key_content.json.

## Build y pruebas

CLI Unity: -batchmode -quit -force-d3d11 -projectPath <proyecto> -executeMethod LastKey.Editor.ProductionVisualSetup.BuildWindows (o BuildWebGL). BuildWebGL usa RuntimeSpeed. BuildWebGLPreview usa BuildTimes para la vista previa local, con el mismo contenido y dirección visual. Esto prepara el prefab con los modelos y motions locales antes de compilar. Las fuentes ActorCore/Mixamo son privadas y no se incluyen en Git: reconstruir requiere obtenerlas bajo sus licencias. Prepare ejecuta CoreQa 22 y PolishQa 53. Runtime de audio: MusicRuntimeQa 13. La comprobación --minimal-ui-qa valida los botones reales, volumen, pausa, entrada, NULL y ECHO y captura PNG nativos; ver UI_INTEGRATION_30.md.

PlayEvidence --music-evidence --evidence-path <carpeta> registra frames reales y MASTER por AudioRenderer; ClipEncoder.Encode genera MP4 1920×1080/30 fps. --pose-audit y --wardrobe-review son pruebas aisladas identificadas, no gameplay. No se desactiva la seguridad de Windows para generar evidencia.

## Alcance

Esta integración permite revisar el diseño dentro del juego; no declara terminado todo el videojuego. La responsabilidad del HUD fue delegada nuevamente a Codex. No se implementa multijugador aquí: sigue siendo una persona y tres bots. No GDD, Trello, Figma, Canva/PDF ni video académico final. GitHub y Unity Play no publicados. Voces y personalización quedan pendientes.
