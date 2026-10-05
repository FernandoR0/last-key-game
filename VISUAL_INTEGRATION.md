# LAST KEY — integración del diseño aprobado

Se conecta el piloto aprobado de la revisión 18 a la competencia existente. Por autorización de Fernando se utiliza provisionalmente el mismo modelo ActorCore Party-M-0001 en los cuatro puestos, conservando CIPHER, UPLINK, RELAY y VECTOR y sus colores de estación. No se ha creado otro modelo ni ropa.

Reutilizados: seis motions importados de Mixamo, collar metálico de Infernalight, assets de arena existentes. Despertar/gesto de collar con contacto en primera ronda; tecleo con alcance a pantalla integrada; reacciones de Danger 40/70/100; victoria del ganador final. Las reglas y la puntuación siguen en Competition.

HUD: clasificación de los cuatro competidores en vivo, texto en panel inferior, avisos destacados NULL/ECHO/combo, ECHO mediante TAB/ENTER. Las voces quedan expresamente pendientes. Los gritos sintetizados rechazados no se integran; se retiró también el grito genérico de eliminación del runtime.

Validación nativa completada: capturas de ejecución, intro, contacto, eventos, rondas y ganador. Los resultados y límites de esta revisión se registran abajo.

Las fuentes ActorCore/Mixamo permanecen privadas e ignoradas por Git. El prefab público referencia esas fuentes locales; para reconstruir el proyecto es necesario obtenerlas bajo sus licencias. Los builds incluyen sus recursos compilados para uso dentro del juego.

## Evidencia de revisión 19

Windows: work/game/integrated-build-windows-composition.log, build correcto (296 MB). CoreQa 22 y PolishQa 53. Runtime completo: work/game/integrated-runtime-release.log, LAST_KEY_V2_EVIDENCE_COMPLETE sin excepciones ni assertions. Evidencia: outputs/LAST_KEY_DESIGN_RELEASE_QA/capture_states.json y PNGs 1920×1080. Replay automático de teclas al núcleo, no gameplay de una persona. Revisadas las vistas de collar, rivales, primera persona, NULL, ECHO, eliminación, retorno y resultados finales.

WebGL: work/game/integrated-build-webgl-release.log, LAST_KEY_WEBGL_BUILD_SUCCESS 113904993 bytes. Carga, menús e introducción comprobados en el navegador local. Enlace: http://127.0.0.1:8765/?design=19. Galería: http://127.0.0.1:8765/design-review/.

No hay voces ni lip sync nuevos. Multijugador y personalización pendientes. Publicación remota GitHub pendiente de autenticar GitHub CLI; repo local se conserva. No se declara terminado todo el videojuego.

WebGL en navegador: se verificó también entrada de teclado con cambio de progreso/métricas y feedback de error; sin mensajes de error en consola durante la prueba. Los PNG de la galería siguen siendo capturas nativas Windows, no capturas de esa prueba del navegador.
