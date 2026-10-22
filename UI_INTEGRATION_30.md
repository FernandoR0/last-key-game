# LAST KEY — interfaz y personaje integrados

La revisión 30 lleva el boceto de interfaz al Canvas del juego principal y compila el personaje de la revisión cinematográfica 25 en las builds Windows y WebGL. Se continúa sobre el proyecto existente.

## Cambios

- Menú claro con «Modos de juego», selección de prueba, identidad, preparación, pausa y resultados. Sin crear/unirse a salas. Classic principal; Code, Numbers, Memory y Sudden Death beta; Chaos sigue sin implementarse.
- Texto de mecanografía flotante, carácter siguiente subrayado y error coral. WPM, precisión, errores, puntos, racha y Danger se obtienen de Competition, sin cifras de maqueta.
- Clasificación acumulada en vivo para cuatro participantes, con nombres, puntos y líneas de progreso. Resultados y ganador siguen la puntuación total del núcleo existente.
- NULL y ECHO muestran reglas, tiempo y recompensa/penalización. ECHO tiene botones reales y selección Tab/Enter. Sus opciones están fuera de la región de mecanografía, incluso con una frase larga. Las notificaciones transitorias usan la misma paleta del boceto.
- Volumen general conectado a AudioListener y música conectada a MusicManager, con porcentajes, persistencia y tiradores circulares. Iconos de sonido/pausa alineados y con contraste apropiado al fondo. Pausa deja fijo el tiempo de la ronda.
- La introducción, collar bajo unido a los pesos del cuello y forcejeo original de la revisión 25 están en el juego principal. El mismo modelo aprobado sigue en las cuatro identidades temporales. No se cambian cara, ropa ni dedos originales en esta integración.
- Continuidad del tecleo al salir de la intro: la cuenta atrás ya no reinicia el fragmento de sentarse a escribir después de que el personaje empezara a teclear.
- Se eliminan textos duplicados en la pantalla del jugador y el tablero cuando se usa el HUD flotante. Permanecen pantalla y teclado integrados físicamente en la mesa.

## Código y builds

`LastKeyGame.UI.cs` concentra composición, menús, indicadores, actualización y controles de sonido. `LastKeyGame.cs` conserva el flujo, la entrada y la coordinación del juego. Las reglas y fórmulas de Competition no se modifican.

Windows: `Builds/Windows/LAST_KEY.exe`.
WebGL local: `http://127.0.0.1:8765/?revision=30`, servido desde `Builds/WebGL`.

El método de build de producción WebGL conserva RuntimeSpeed. La vista previa local se exporta con BuildWebGLPreview/BuildTimes; cambia la optimización del compilador, no los personajes, materiales, iluminación ni el contenido. La plantilla utiliza URLs de assets con `30minimal` para evitar cargar el código de revisiones anteriores.

## Validación

El núcleo se verifica con CoreQa y PolishQa durante cada build. El replay nativo de cinco rondas de la integración inicial completó NULL/ECHO, Danger, eliminación, retorno y ganador sin excepciones bloqueantes (`LAST_KEY_UI_27_REVIEW`).

`MinimalInterfaceQa`, activado únicamente mediante `--minimal-ui-qa`, ejecuta callbacks de botones reales, modifica y restaura los sliders, valida pausa/reanudación y usa Competition.Input para generar puntos y errores reales. Incluye una comprobación geométrica para que las dos opciones de ECHO no invadan el rectángulo de la frase. No asigna puntos ni mueve temporizadores artificialmente. Las capturas finales están en `LAST_KEY_UI_30_QA`.

Resultados finales: CoreQa 22 PASS; PolishQa 53 PASS; `LAST_KEY_MINIMAL_INTERFACE_QA_PASS 34`. Windows build correcto, 297222861 bytes reportados por Unity; WebGL correcto, 114734585 bytes reportados. No se registraron excepciones bloqueantes ni advertencias de texto cortado en el smoke test final. Capturas seleccionadas: `Docs/ui30/main-menu.png`, `Docs/ui30/gameplay.png`, `Docs/ui30/echo.png`.

WebGL cargó la revisión `30minimal` desde el servidor local sin errores de navegador. Se verificaron el botón de inicio y los pasos menú → modos → identidad → preparación → introducción usando Enter y Tab reales sobre el Canvas. El personaje y la introducción se renderizan en la build principal, no en el reproductor del piloto. Tras la introducción se verificó entrada física de teclado y reacción de puntos, errores y Danger en la UI WebGL. Se guardaron 13_WEBGL_MAIN_MENU.png y 14_WEBGL_GAMEPLAY.png. El registro de errores del navegador permaneció vacío. La pausa y la retención del reloj se comprobaron en el smoke test nativo.

Se descartaron durante la revisión visual un título cortado, un tirador de slider estirado y el cruce entre opciones ECHO/frase. La composición corregida se revisa a 1920×1080.

## Alcance

Una persona y tres bots; no multijugador de salas. Voces y personalización siguen pendientes. No se toca GDD, Trello, Figma, Canva/PDF ni se genera el video académico final. S/0 de costo adicional. No se declara publicación en GitHub o Unity Play.
