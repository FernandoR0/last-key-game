# LAST KEY

**ESCRIBE. SOBREVIVE. SÉ EL ÚLTIMO.**

Competencia de mecanografía para PC. Elige uno de cuatro participantes; los otros tres son bots. Cinco rondas de 60 segundos. Gana la mayor puntuación acumulada; Danger 100% elimina únicamente de esa ronda y se restablece en la siguiente. Los empates se declaran explícitamente.

## Abrir y jugar

Unity 6000.6.0f1, URP 17.6.0, uGUI/TextMeshPro 2.6.0. Abrir `Assets/LastKey/Scenes/LastKey.unity` y pulsar Play. El flujo incluye carga, menú, modo, personaje, configuración, intro de trece segundos, cuenta atrás de tres segundos, gameplay, resultados de cada ronda y ganador final. Los resultados ofrecen siguiente ronda, revancha y menú.

Seleccionables: CIPHER — Valeria Rojas; UPLINK — Ethan Cole; RELAY — Akira Mori; VECTOR — Noah Bennett. La selección cambia identidad y estación del humano, sin ventajas estadísticas.

Escribe el carácter subrayado, incluidos espacios y signos. Solo acertar avanza el cursor. No hace falta borrar un error. F1 acepta ECHO; F2 lo ignora; la oferta caduca automáticamente a los ocho segundos. F3 alterna cámara de gameplay y arena; Escape pausa/reanuda. NULL emite directivas automáticamente.

Classic es la experiencia principal. Code, Numbers, Memory y Sudden están disponibles como BETA. Code es copia literal, sin ejecutar código ni evaluar conocimientos. Memory revela cada nuevo texto cuatro segundos antes de ocultarlo. Chaos aparece como PRÓXIMAMENTE.

## Reglas y contenido

Correcto +1 por multiplicador de racha; palabra terminada +8. Cada 20 caracteres de combo aumenta el multiplicador hasta ×4. Error +8 Danger; correcto −0.35 Danger. NULL rota Precision Protocol, Speed Check y Clean Streak. ECHO dura 12 segundos al aceptarse: éxito +150 puntos y −15 Danger; fallo +15 Danger. WPM = correctos / 5 / minutos activos. Accuracy = correctos / (correctos + errores).

Los bots usan la misma validación del humano. Cada participante tiene prompts con shuffle bag, dificultad gradual y memoria reciente: sin repetición consecutiva ni duplicados dentro de una ronda. Configuración editable en `Assets/Resources/last_key_rules.json` y `last_key_content.json`.

## Builds y QA

Menú LAST KEY → Prepare Project / Build Windows / Build WebGL. Salidas `Builds/Windows/LAST_KEY.exe` y `Builds/WebGL/`. WebGL requiere servidor HTTP, por ejemplo `python -m http.server 8765 --bind 127.0.0.1 --directory Builds/WebGL`; abrir localhost:8765 y pulsar COMENZAR CON SONIDO para habilitar audio mediante el gesto del navegador.

CLI Unity: `-batchmode -nographics -quit -projectPath <proyecto> -executeMethod LastKey.Editor.ProjectBuilder.BuildWebGL` (o BuildWindows). Prepare ejecuta las pruebas CoreQa y PolishQa. El núcleo no depende de Unity; existe también `Tests/LastKey.Core.Tests.csproj`.

Replay real de QA: `-executeMethod LastKey.Editor.ProjectBuilder.PlayEvidence --evidence-v2 --evidence-path <carpeta>`. Envía entradas por Competition.Input/Tick, sin asignar métricas o scores. `--pose-audit` genera diagnósticos aislados de postura; esos diagnósticos no son una partida normal.

ClipEncoder.Encode codifica frames reales a MP4 1080p/30 fps. En V3, `--music-evidence` registra la salida MASTER real mediante Unity AudioRenderer: música suministrada y SFX, sincronizados con los frames. No es grabación del micrófono ni mezcla posterior de canciones. El replay V2 permanece como evidencia histórica. ClipValidator.Validate decodifica todos los frames de cada clip y guarda muestras para revisión visual.

La restricción histórica del standalone ya no se reproduce en la revisión V3: el ejecutable Windows arrancó y completó un smoke con capturas reales de menú, intro y 12 s de gameplay. Otras herramientas multimedia siguen bloqueadas; se utiliza Unity AudioRenderer/MediaEncoder y reproducción del navegador. No se desactiva seguridad. Consultar QA_REPORT_V3.md para límites de las pruebas.

## Arte y alcance

Cuatro modelos Renderpeople gratuitos con rigs Humanoid válidos: Carla/CIPHER, Eric/UPLINK, Manuel/RELAY y Nathan/VECTOR. Se reutiliza su ropa civil y se recolorean únicamente máscaras de prendas; piel y cabello conservados. Alturas 1.73/1.81/1.76/1.85 m. Animación procedural propia adaptada a los rigs: sentado, typing normal/rápido, error, Danger, eliminación, victoria de ronda/final y despertar. No se utiliza mocap Mixamo. El despertar mueve cabeza y mano; no existe animación facial individual de ojos.

Renderpeople permite su uso en videojuegos, pero prohíbe redistribuir los assets fuente o derivados como recursos individuales. `Assets/PrivateRenderpeople/` está excluido de Git. Para reproducir el proyecto en otra máquina, revisar la licencia enlazada en ASSET_MANIFEST.md, instalar Python con numpy/Pillow y ejecutar `python Tools/setup_renderpeople.py`; después ejecutar en Unity `LastKey.Editor.RealisticImport.Inspect`. El script descarga los dos paquetes FBX oficiales y crea localmente las variantes. El fallback histórico Quaternius se conserva fuera de Resources en Assets/CharacterFallback y en el checkpoint da077c5; no se usa como personaje final.

Poly Haven CC0: escritorios y piso. Arena y periféricos propios. VFX y SFX originales. Música suministrada: Iron Dominion, Dead Current y Mainframe, conservada byte a byte en Assets/Audio/Music/Masters. MusicManager A/B, mixer MASTER/MUSIC/SFX/UI/AMBIENCE, crossfade, segmentos con timeSamples, ducking NULL/ECHO y Music Volume persistente. Preparar con `LastKey.Editor.MusicSetup.Prepare` si faltan mixer/library. WebGL usa carga DecompressOnLoad con preload y un inicio por clic; Windows conserva Streaming. La entrada fuerte se sitúa en 16 s, después de 13 s de intro y 3 s de countdown. La selección musical/loops sigue pendiente de escucha; no se declara MUSIC GATE PASS. Licencias y asignación en ASSET_MANIFEST.md y MUSIC_INTEGRATION_REPORT.md. Costo adicional S/0.

AEGIS/NULL/ECHO tienen historia reservada para Story Mode futuro. No multijugador online. GDD académico, Figma, Trello, Canva/PDF y video final quedan fuera de esta fase. La evidencia V1 permanece separada de V2.
