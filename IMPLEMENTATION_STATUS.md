# LAST KEY — interfaz integrada, revisión 31

Se continúa sobre el proyecto existente. Costo adicional S/0. El juego principal usa cuatro copias temporales del ActorCore Party-M-0001 aprobado y la secuencia cinematográfica 25, con CIPHER/UPLINK/RELAY/VECTOR.

## Integración actual

- Revisión 31: tecleo ampliado al tramo 2,6–7,8 s de Typing.fbx. Se conservan diez dedos activos y las muñecas originales; el contacto solo corrige penetración, sin fijar la mano al cristal. Detalle y pruebas en TYPING_REVIEW_31.md.

- Interfaz nativa Unity del boceto 26: frase flotante sin recuadro, carácter actual subrayado, puntuación de ronda, WPM, precisión, errores, racha, Danger y clasificación acumulada en vivo.
- Menú claro con «Modos de juego». Sin botones de crear/unirse a salas. Classic principal; Code, Numbers, Memory y Sudden Death beta; Chaos no seleccionable, próximo.
- Personaje actual, once clips importados, collar frontal bajo ligado a los pesos del cuello y dedos originales del forcejeo. Esta integración no modifica cara ni ropa.
- Introducción completa solo en la primera ronda; transición al tecleo sin reiniciar el gesto de sentarse a escribir. En las siguientes rondas comienza la cuenta atrás.
- Pantalla horizontal y teclado español integrados en la mesa; entrada mediante teclado físico. La UI no presenta el teclado 3D como un teclado táctil por clic.
- NULL/ECHO con reglas y tiempo visibles. ECHO puede aceptarse/ignorarse con botones o Tab/Enter, sin invadir el área de mecanografía.
- Volumen general y música conectados al audio real, porcentaje y tiradores circulares. Esc pausa; el temporizador no avanza mientras está en pausa.
- Resultados de ronda y THE LAST KEY usan los puntos reales del núcleo. Danger 100% elimina solo de la ronda y el jugador vuelve en la siguiente.

## Builds y evidencia

Windows: Builds/Windows/LAST_KEY.exe. WebGL: Builds/WebGL, servido por HTTP en http://127.0.0.1:8765/?revision=31. La plantilla usa el identificador de assets 30minimal para evitar revisiones en caché.

Pruebas del núcleo: CoreQa y PolishQa (53 comprobaciones adicionales). Replay nativo completo de cinco rondas durante la integración inicial: LAST_KEY_UI_27_REVIEW. Smoke tests de callbacks de interfaz, audio, pausa, entrada y eventos: LAST_KEY_UI_30_QA. Los registros concretos y la comprobación del navegador están en UI_INTEGRATION_30.md.

La compilación de producción WebGL mantiene RuntimeSpeed; BuildWebGLPreview usa BuildTimes para la vista previa local con los mismos modelos, materiales, iluminación, reglas y UI.

## Límites

Una persona y tres bots; no multijugador de salas, campaña, Chaos ni personalización. Las voces quedan para el final: no se integra la muestra personal ni el grito sintético rechazado. No se afirma aprobación auditiva de la música.

No se modifican GDD, Trello, Figma, Canva/PDF ni se produce el video académico final. Repositorio de colaboración: https://github.com/FernandoR0/last-key-game. Unity Play sigue pendiente. Las fuentes ActorCore/Mixamo siguen privadas y la reconstrucción requiere sus archivos y licencias.
