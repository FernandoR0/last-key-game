# LAST KEY

Competencia de mecanografía por rondas: cuatro participantes, una persona y tres bots. Gana la mayor puntuación acumulada tras cinco rondas de 60 segundos; Danger 100% elimina únicamente de la ronda actual.

## Abrir

Unity 6000.6.0f1 / URP 17.6.0. Escena Assets/LastKey/Scenes/LastKey.unity. Builds: Builds/Windows/LAST_KEY.exe y Builds/WebGL/.

Para WebGL: servir Builds/WebGL por HTTP en 127.0.0.1:8765 y abrir http://127.0.0.1:8765/?visual=1. Pulsar COMENZAR CON SONIDO. Esta vista inicia directamente la revisión física: intro de 26 segundos, cuenta atrás y primera persona. Resultados conservan UI provisional para avanzar. El enlace es local a esta PC.

## Revisión visual V4

Mesas/sillas importadas, pantalla táctil integrada, teclado español extendido completo, arena modular iluminada, personajes 3D de historieta con ropa existente y expresiones. Animaciones sentadas retargeteadas, typing CMU + adaptación IK, despertar/collar/sujeción/rivales/NULL solo primera ronda, reacciones y victoria suavizadas. Sin monitores de sobremesa. Las teclas muestran entradas físicas; no son botones táctiles funcionales. Fuentes/licencias/atribuciones: ASSET_MANIFEST.md. Separación de funciones: VISUAL_HANDOFF_FOR_UI.md.

Música suministrada con una sola fuente activa, mute persistente y cierre del audio propio; efectos gratuitos. Se verificó señal MASTER sin clipping en ocho clips. La escucha perceptual no se declara completada.

## Reglas

Carácter correcto +1 multiplicado por racha; palabra +8. Cada 20 de combo aumenta multiplicador hasta x4. Error +8 Danger; correcto −0.35. NULL rota Precision Protocol, Speed Check y Clean Streak. ECHO opcional dura 12 segundos, éxito +150 puntos/−15 Danger, fallo +15 Danger. WPM = correctos /5/minutos activos. Accuracy = correctos/(correctos+errores). Bots pasan por la misma validación.

Classic principal; Code/Numbers/Memory/Sudden Death BETA. Code copia texto sin ejecutarlo ni evaluar programación. Memory revela cuatro segundos. Chaos y Story Mode no implementados. Configuración en Assets/Resources/last_key_rules.json y last_key_content.json.

## Build y pruebas

Menú LAST KEY → Prepare Project / Build Windows / Build WebGL. CLI Unity: -batchmode -nographics -quit -projectPath <proyecto> -executeMethod LastKey.Editor.ProjectBuilder.BuildWindows (o BuildWebGL). Prepare ejecuta CoreQa 22 y PolishQa 53. Runtime de audio: MusicRuntimeQa 13.

PlayEvidence --music-evidence --evidence-path <carpeta> registra frames reales y MASTER por AudioRenderer; ClipEncoder.Encode genera MP4 1920×1080/30 fps. --pose-audit y --wardrobe-review son pruebas aisladas identificadas, no gameplay. No se desactiva la seguridad de Windows para generar evidencia.

## Alcance

Aprobación artística pendiente. Fernando controla Canvas/HUD/menús/TAB/ENTER/volumen de interfaz y networking de salas privadas; no se implementa multijugador aquí. No GDD, Trello, Figma, Canva/PDF ni video académico final. GitHub y Unity Play no publicados. Revisión técnica actual en QA_REPORT_VISUAL_V4.md.
