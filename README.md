# LAST KEY

Competencia de mecanografía por rondas: cuatro participantes, una persona y tres bots. Gana la mayor puntuación acumulada tras cinco rondas de 60 segundos; Danger 100% elimina únicamente de la ronda actual.

## Abrir

Unity 6000.6.0f1 / URP 17.6.0. Escena Assets/LastKey/Scenes/LastKey.unity. Builds: Builds/Windows/LAST_KEY.exe y Builds/WebGL/.

Para WebGL: servir Builds/WebGL por HTTP en 127.0.0.1:8765 y abrir http://127.0.0.1:8765/?design=19. El enlace es local a esta PC. Iniciar desde el menú para conservar el HUD completo. La opción histórica visual=1 oculta parte del HUD y no sirve para revisar esta integración.

## Diseño integrado — revisión 19

Cuatro copias temporales del ActorCore Party-M-0001 aprobado en el piloto, con las identidades y estaciones CIPHER/UPLINK/RELAY/VECTOR. Se sustituyen los personajes anteriores en la partida. Se reutilizan seis motions Mixamo: despertar sentado, gesto de collar, tecleo, idle, descarga y victoria. Collar metálico importado ajustado al cuello y contacto de manos. Pantalla integrada en la mesa, teclado español extendido y primera persona. Las teclas muestran entradas físicas; no son botones táctiles funcionales. Fuentes/licencias/atribuciones: ASSET_MANIFEST.md. Detalles actuales: VISUAL_INTEGRATION.md; la documentación V4 es histórica.

HUD integrado: los cuatro participantes en clasificación en vivo, texto de mecanografía inferior, notificaciones destacadas NULL/ECHO/combo. TAB cambia aceptar/ignorar ECHO; ENTER confirma. Las voces quedan para el final, sin integrar la voz sintética rechazada ni el grito genérico.

Música suministrada con una sola fuente activa, mute persistente y cierre del audio propio; efectos gratuitos. Se verificó señal MASTER sin clipping en ocho clips. La escucha perceptual no se declara completada.

## Reglas

Carácter correcto +1 multiplicado por racha; palabra +8. Cada 20 de combo aumenta multiplicador hasta x4. Error +8 Danger; correcto −0.35. NULL rota Precision Protocol, Speed Check y Clean Streak. ECHO opcional dura 12 segundos, éxito +150 puntos/−15 Danger, fallo +15 Danger. WPM = correctos /5/minutos activos. Accuracy = correctos/(correctos+errores). Bots pasan por la misma validación.

Classic principal; Code/Numbers/Memory/Sudden Death BETA. Code copia texto sin ejecutarlo ni evaluar programación. Memory revela cuatro segundos. Chaos y Story Mode no implementados. Configuración en Assets/Resources/last_key_rules.json y last_key_content.json.

## Build y pruebas

CLI Unity: -batchmode -quit -force-d3d11 -projectPath <proyecto> -executeMethod LastKey.Editor.ProductionVisualSetup.BuildWindows (o BuildWebGL). Esto prepara el prefab con los modelos y motions locales antes de compilar. Las fuentes ActorCore/Mixamo son privadas y no se incluyen en Git: reconstruir requiere obtenerlas bajo sus licencias. Prepare ejecuta CoreQa 22 y PolishQa 53. Runtime de audio: MusicRuntimeQa 13.

PlayEvidence --music-evidence --evidence-path <carpeta> registra frames reales y MASTER por AudioRenderer; ClipEncoder.Encode genera MP4 1920×1080/30 fps. --pose-audit y --wardrobe-review son pruebas aisladas identificadas, no gameplay. No se desactiva la seguridad de Windows para generar evidencia.

## Alcance

Esta integración permite revisar el diseño dentro del juego; no declara terminado todo el videojuego. La responsabilidad del HUD fue delegada nuevamente a Codex. No se implementa multijugador aquí: sigue siendo una persona y tres bots. No GDD, Trello, Figma, Canva/PDF ni video académico final. GitHub y Unity Play no publicados. Voces y personalización quedan pendientes.
