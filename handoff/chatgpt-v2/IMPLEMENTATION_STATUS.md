# Estado de implementación — LAST KEY V2

Costo adicional: **S/0**. Proyecto existente pulido, sin reconstruir el núcleo. No se modificaron GDD académico, Trello, Figma ni Canva/PDF. No se creó el video final.

## Implementado

- Flujo: Boot → Loading → menú → modo → personaje → configuración → intro de seis segundos → 3/2/1 → gameplay → resultados por ronda → THE LAST KEY. Siguiente ronda, revancha, volver al menú y pausa.
- Cuatro personajes seleccionables con las mismas estadísticas: CIPHER/Valeria Rojas, UPLINK/Ethan Cole, RELAY/Akira Mori, VECTOR/Noah Bennett. El elegido es humano; tres bots con reglas comunes.
- Cinco rondas de 60 segundos; clasificación y puntuación acumuladas; empates declarados. Danger 100% elimina solo en esa ronda, conserva puntos y permite volver en la siguiente.
- Mecanografía física, WPM, Accuracy, Errors, Score, Combo y Danger. Prompts individuales con shuffle bag, dificultad, historial reciente y sin duplicados dentro de una ronda.
- NULL automático: Precision Protocol, Speed Check y Clean Streak. ECHO: F1 acepta, F2 ignora, oferta de ocho segundos y reto de doce; premio/penalización visibles.
- Classic principal; Code, Numbers, Memory y Sudden Death BETA. Memory revela cada texto cuatro segundos. Chaos solo aparece como PRÓXIMAMENTE.
- Arena industrial compacta; cuatro escritorios/teclados/monitores/estaciones; texto y progreso en monitores reales; marcador de sala. Cámara de gameplay con humano y tres rivales visibles; seis roles de cámara; F3 alterna vista amplia.
- Quaternius Universal Base Characters Standard CC0 con dos bases Humanoid y cuatro variantes de proporción, piel, pelo, barba y ropa. Animaciones originales seated_idle, type_normal, type_fast, mistake_reaction, danger_reaction, eliminated, round_win, final_win; intro de despertar. No modelos de IA, placeholders como personajes finales ni clips Mixamo descargados.
- Correcciones de postura: asiento más corto y separación del respaldo, piernas flexionadas con reposapiés, mano de despertar delante del torso y orientación de muñecas en victoria. Doce diagnósticos de los cuatro personajes revisados antes de recapturar la partida.
- Sonidos sintetizados originales y VFX de collar/descarga estilizada. HUD y menús escalables; ropa con shader original y material eléctrico referenciado en Resources para evitar stripping de WebGL.

## Evidencia de la partida V2

Replay de QA en Unity Editor usando Competition.Input/Tick; no se asignaron puntuaciones ni Danger para fabricar resultados. Cinco rondas completas. CIPHER: 6173; VECTOR: 3806; RELAY: 3134; UPLINK: 1544. Gana CIPHER por mayor puntuación total. Ronda 1: CIPHER eliminado con Danger 100% y 415 puntos; vuelve en ronda 2 conservando esos 415 puntos.

Veinte PNG de Editor: dieciséis estados y cuatro personajes; nueve clips 1920×1080/30 fps. La captura WebGL y pruebas de resoluciones se documentan en QA_REPORT.md. El audio de clips mezcla los mismos sonidos del juego según eventos, sin narración ni grabación de micrófono. Los diagnósticos aislados en technical/pose-review sirven para revisar el rig y están identificados como pruebas, no como partida normal.

## Validación y límites

CoreQa: 22 comprobaciones; PolishQa: 53. Total 75 aprobadas en Unity. Builds Windows x64 y WebGL; resultado y ejecución efectiva descritos en QA_REPORT.md. Smart App Control impide lanzar el standalone en este equipo; compilación Windows no equivale a validación de su ejecución. Unity Editor y WebGL son las vías de runtime utilizadas. No se desactiva seguridad.

No implementado: Chaos, Story Mode, historia profunda de AEGIS/NULL/ECHO, multijugador online, matchmaking, progresión/campaña ni narración/video académico final. BETA no significa balance terminado. Arte y movimientos son estilizados de MVP, con posible pulido posterior.

Git local preparado. Publicación GitHub depende de autenticación; no se afirma push ni URL pública. Unity Play no publicado.

**ACCIÓN MANUAL PENDIENTE: autenticar GitHub CLI con gh auth login**

**STOP — WAIT_FOR_CHATGPT_PDF**, después de entregar evidencia V2 revisada.
