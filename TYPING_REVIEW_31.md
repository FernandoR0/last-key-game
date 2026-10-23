# LAST KEY — corrección de tecleo, revisión 31

Se conserva Typing.fbx descargado de Mixamo (16,4667 s), el personaje aprobado y la animación original del collar.

## Causa
La revisión 30 repetía únicamente 1,05–2,35 s. En ese tramo ambos pulgares estaban inmóviles y tres dedos izquierdos apenas variaban 1°. Además, ContactHand aplanaba la muñeca y fijaba la mano a la altura del cristal.

## Cambio
Se reproduce el tramo 2,6–7,8 s, con inversión suave en los extremos. Se conservan las curvas importadas de los diez dedos y de las muñecas. La adaptación al mueble solo eleva una mano si penetra la superficie; no baja las manos que se levantan al escribir. Se conserva el límite longitudinal del área del teclado.

## Prueba de fuente y retarget
Una vuelta completa de 10,4 s sobre el rig real, comparando el clip con y sin adaptación al escritorio:

- Diez dedos activos: amplitudes proximales de 8,12° a 87,24°.
- Mismas amplitudes digitales antes y después del contacto: las correcciones no sobrescriben dedos.
- Mano izquierda: 5,75 cm lateral y 1,37 cm vertical.
- Mano derecha: 15,48 cm lateral y 6,40 cm vertical.
- El contacto conserva la amplitud vertical original; antes la reducía al fijar la altura.

Registro: work/game/typing-fixed-audit.log, TYPING_COMPARISON_COMPLETE audit.

La nueva captura de partida usa el flujo real de menú, introducción y cuenta atrás antes de grabar. No asigna poses ni puntuaciones. La validación final de builds y captura se agrega al terminar.

## Captura en el juego
Se recorrieron menú, introducción de 46 s y cuenta atrás en la build Windows. Se capturaron 305 fotogramas reales; los primeros 300 forman TECLEO_CORREGIDO_1080P.mp4: 1920×1080, 30 fps, 10 s, sin audio. El archivo se decodificó completo sin errores y se verificó su reproducción en el navegador.

El chequeo automático de 360 fotogramas se interrumpió cuando se pausó la partida; no se declara PASS para ese chequeo. La prueba de amplitud de los diez dedos en el rig sí pasó. La sesión oculta inicial no permitió capturar pantalla; se repitió con el reproductor visible. Se revisaron los fotogramas de la captura visible.

Windows: LAST_KEY_WINDOWS_BUILD_SUCCESS 297226316. PolishQa: 53 PASS.
WebGL: LAST_KEY_WEBGL_BUILD_SUCCESS 114755980. PolishQa: 53 PASS. Plantilla 31typing para invalidar la caché de los cuatro recursos de la build. Vista previa: http://127.0.0.1:8765/?revision=31 y clip en /typing-review/.
