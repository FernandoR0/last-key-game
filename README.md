# LAST KEY

**ESCRIBE. SOBREVIVE. SÉ EL ÚLTIMO.**

Competencia de mecanografía para PC. Elige uno de cuatro participantes; los otros tres son bots. Cinco rondas de 60 segundos. Gana la mayor puntuación acumulada; Danger 100% elimina únicamente de esa ronda y se restablece en la siguiente. Los empates se declaran explícitamente.

## Abrir y jugar

Unity 6000.6.0f1, URP 17.6.0, uGUI/TextMeshPro 2.6.0. Abrir `Assets/LastKey/Scenes/LastKey.unity` y pulsar Play. El flujo incluye carga, menú, modo, personaje, configuración, intro de seis segundos, cuenta atrás de tres segundos, gameplay, resultados de cada ronda y ganador final. Los resultados ofrecen siguiente ronda, revancha y menú.

Seleccionables: CIPHER — Valeria Rojas; UPLINK — Ethan Cole; RELAY — Akira Mori; VECTOR — Noah Bennett. La selección cambia identidad y estación del humano, sin ventajas estadísticas.

Escribe el carácter subrayado, incluidos espacios y signos. Solo acertar avanza el cursor. No hace falta borrar un error. F1 acepta ECHO; F2 lo ignora; la oferta caduca automáticamente a los ocho segundos. F3 alterna cámara de gameplay y arena; Escape pausa/reanuda. NULL emite directivas automáticamente.

Classic es la experiencia principal. Code, Numbers, Memory y Sudden están disponibles como BETA. Code es copia literal, sin ejecutar código ni evaluar conocimientos. Memory revela cada nuevo texto cuatro segundos antes de ocultarlo. Chaos aparece como PRÓXIMAMENTE.

## Reglas y contenido

Correcto +1 por multiplicador de racha; palabra terminada +8. Cada 20 caracteres de combo aumenta el multiplicador hasta ×4. Error +8 Danger; correcto −0.35 Danger. NULL rota Precision Protocol, Speed Check y Clean Streak. ECHO dura 12 segundos al aceptarse: éxito +150 puntos y −15 Danger; fallo +15 Danger. WPM = correctos / 5 / minutos activos. Accuracy = correctos / (correctos + errores).

Los bots usan la misma validación del humano. Cada participante tiene prompts con shuffle bag, dificultad gradual y memoria reciente: sin repetición consecutiva ni duplicados dentro de una ronda. Configuración editable en `Assets/Resources/last_key_rules.json` y `last_key_content.json`.

## Builds y QA

Menú LAST KEY → Prepare Project / Build Windows / Build WebGL. Salidas `Builds/Windows/LAST_KEY.exe` y `Builds/WebGL/`. WebGL requiere servidor HTTP, por ejemplo `python -m http.server 8765 --bind 127.0.0.1 --directory Builds/WebGL`; abrir localhost:8765.

CLI Unity: `-batchmode -nographics -quit -projectPath <proyecto> -executeMethod LastKey.Editor.ProjectBuilder.BuildWebGL` (o BuildWindows). Prepare ejecuta las pruebas CoreQa y PolishQa. El núcleo no depende de Unity; existe también `Tests/LastKey.Core.Tests.csproj`.

Replay real de QA: `-executeMethod LastKey.Editor.ProjectBuilder.PlayEvidence --evidence-v2 --evidence-path <carpeta>`. Envía entradas por Competition.Input/Tick, sin asignar métricas o scores. `--pose-audit` genera diagnósticos aislados de postura; esos diagnósticos no son una partida normal.

ClipEncoder.Encode codifica frames reales a MP4 1080p/30 fps. El audio usa los mismos clips sintetizados del juego, registrados por evento y mezclados en sus posiciones temporales; no es grabación del micrófono. ClipValidator.Validate decodifica todos los frames de cada clip y guarda muestras para revisión visual.

Smart App Control de este equipo impide ejecutar el standalone y algunas herramientas locales; validar el runtime en Unity Editor y WebGL. No se desactiva la protección. Consultar el informe de QA V2 para el estado verificado, sin inferir que un build compilado ha sido jugado.

## Arte y alcance

Quaternius Universal Base Characters Standard CC0: dos bases Humanoid y cuatro variantes de piel, proporción, pelo, barba y ropa. Animaciones originales sobre el rig: seated_idle, type_normal, type_fast, mistake_reaction, danger_reaction, eliminated, round_win y final_win; intro de despertar. No modelos de IA ni Mixamo descargado. Se corrigieron asiento/piernas, mano de intro y muñecas de victoria con revisión cercana de los cuatro personajes.

Poly Haven CC0: escritorios y piso. Arena y periféricos propios. Ropa mediante colores de vértice y shader original; sonido sintetizado original. Licencias en ASSET_MANIFEST.md. Costo adicional S/0.

AEGIS/NULL/ECHO tienen historia reservada para Story Mode futuro. No multijugador online. GDD académico, Figma, Trello, Canva/PDF y video final quedan fuera de esta fase. La evidencia V1 permanece separada de V2.
