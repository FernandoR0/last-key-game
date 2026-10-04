# Estado de implementación — LAST KEY V3 en revisión

Costo adicional: **S/0**. Proyecto existente pulido, sin reconstruir el núcleo. No se modificaron GDD académico, Trello, Figma ni Canva/PDF. No se creó el video final.

## Implementado

- Flujo: Boot → Loading → menú → modo → personaje → configuración → intro de trece segundos → 3/2/1 → gameplay → resultados por ronda → THE LAST KEY. Siguiente ronda, revancha, volver al menú y pausa.
- Cuatro personajes seleccionables con las mismas estadísticas: CIPHER/Valeria Rojas, UPLINK/Ethan Cole, RELAY/Akira Mori, VECTOR/Noah Bennett. El elegido es humano; tres bots con reglas comunes.
- Cinco rondas de 60 segundos; clasificación y puntuación acumuladas; empates declarados. Danger 100% elimina solo en esa ronda, conserva puntos y permite volver en la siguiente.
- Mecanografía física, WPM, Accuracy, Errors, Score, Combo y Danger. Prompts individuales con shuffle bag, dificultad, historial reciente y sin duplicados dentro de una ronda.
- NULL automático: Precision Protocol, Speed Check y Clean Streak. ECHO: F1 acepta, F2 ignora, oferta de ocho segundos y reto de doce; premio/penalización visibles.
- Classic principal; Code, Numbers, Memory y Sudden Death BETA. Memory revela cada texto cuatro segundos. Chaos solo aparece como PRÓXIMAMENTE.
- Arena industrial compacta; cuatro escritorios/teclados/monitores/estaciones; texto y progreso en monitores reales; marcador de sala. Cámara de gameplay con humano y tres rivales visibles; seis roles de cámara; F3 alterna vista amplia.
- Renderpeople: Carla/CIPHER, Eric/UPLINK, Manuel/RELAY y Nathan/VECTOR; cuatro rigs Humanoid válidos y anatomías distintas, con ropa civil importada y recoloreada por máscara de prenda. Animaciones originales seated_idle, type_normal, type_fast, mistake_reaction, danger_reaction, eliminated, round_win, final_win; intro de despertar. No modelos de IA, placeholders como personajes finales ni clips Mixamo descargados.
- Correcciones de postura: asiento más corto y separación del respaldo, piernas flexionadas con reposapiés, mano de despertar delante del torso y orientación de muñecas en victoria. Doce diagnósticos de los cuatro personajes revisados antes de recapturar la partida.
- Sonidos sintetizados originales y VFX de collar/descarga estilizada. HUD y menús escalables; prendas importadas con materiales URP y máscaras de recolor y material eléctrico referenciado en Resources para evitar stripping de WebGL.

## Evidencia de la partida V2

Replay de QA en Unity Editor usando Competition.Input/Tick; no se asignaron puntuaciones ni Danger para fabricar resultados. Cinco rondas completas. CIPHER: 6173; VECTOR: 3806; RELAY: 3134; UPLINK: 1544. Gana CIPHER por mayor puntuación total. Ronda 1: CIPHER eliminado con Danger 100% y 415 puntos; vuelve en ronda 2 conservando esos 415 puntos.

Veinte PNG de Editor: dieciséis estados y cuatro personajes; nueve clips 1920×1080/30 fps. La captura WebGL y pruebas de resoluciones se documentan en QA_REPORT.md. El audio de clips mezcla los mismos sonidos del juego según eventos, sin narración ni grabación de micrófono. Los diagnósticos aislados en technical/pose-review sirven para revisar el rig y están identificados como pruebas, no como partida normal.

## Validación y límites

CoreQa: 22 comprobaciones; PolishQa: 53. Total 75 aprobadas en Unity. Builds Windows x64 y WebGL; resultado y ejecución efectiva descritos en QA_REPORT.md. El standalone Windows V3 arrancó y completó smoke de menú, intro y 12 s de gameplay, con nueve PNG propios. Editor completó cinco rondas; WebGL fue probado con teclado/pausa y persistencia de volumen. No se desactiva seguridad; consultar QA_REPORT_V3.md.

No implementado: Chaos, Story Mode, historia profunda de AEGIS/NULL/ECHO, multijugador online, matchmaking, progresión/campaña ni narración/video académico final. BETA no significa balance terminado. Modelos realistas de muestra; movimientos procedurales de MVP, sin animación facial individual ni mocap de mecanografía. Se revisaron doce poses nuevas y nueve vistas de cuerpo completo/lateral.

Git local preparado. Publicación GitHub depende de autenticación; no se afirma push ni URL pública. Unity Play no publicado.

**ACCIÓN MANUAL PENDIENTE: autenticar GitHub CLI con gh auth login**

## Cambio V3 y gates

Checkpoint previo da077c5. Dos builds nuevos (Windows/WebGL) y revisión V3 se documentan en QA_REPORT_V3.md. Tres WAV íntegros, mixer y MusicManager A/B integrados. QA runtime aprobó fuentes persistentes, volumen, intro/skip, pausa/reanudación, ducking NULL y loop por segmento. Ocho clips nuevos con salida MASTER real, sin música añadida después. Los números del replay anterior coinciden con el nuevo replay de cinco rondas; no se fabricaron scores.

No hay handoff V3 final mientras MUSIC GATE siga pendiente de escucha. El handoff V2 es histórico. No se publicó GitHub ni Unity Play. Se preparan archivos de revisión directamente en esta conversación; no se exige traslado manual entre Codex y ChatGPT.

### Corrección de audio WebGL y volumen

Inicio musical diferido hasta AudioClip Loaded con samples positivos y contexto Web Audio running; seek antes de Play. Import default compatible DecompressOnLoad/preload, Windows conserva Streaming. Música +11.2 dB a solicitud de Fernando; ocho clips regrabados y revisados, masters intactos. MUSIC GATE continúa pendiente de aprobación auditiva.
