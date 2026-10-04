# QA LAST KEY V3 — revisión, no handoff final

Fecha: 2026-10-03, America/Lima. Unity 6000.6.0f1. Costo adicional S/0. Se continuó desde el proyecto existente. Checkpoint previo da077c5 conserva el experimento modular anterior.

## Gates

| Gate | Resultado | Evidencia y alcance |
|---|---|---|
| CHARACTER QUALITY | PASS | Cuatro Renderpeople Humanoid válidos, cuerpo completo/lateral, doce poses de sentado/despertar/victoria, typing en gameplay y cuatro retratos revisados. Anatomías/ropa distintas; manos fuera del torso, dedos visibles sobre teclado, muñecas de victoria alineadas y zapatos fuera del reposapiés. |
| GAMEPLAY CAMERA | PASS | Capturas reales del Editor y standalone muestran personaje humano, ambas manos, teclado, monitor, texto HUD, tres rivales y arena. Vista amplia sobre hombro; F3 permite alternar. |
| MUSIC | PENDIENTE | Tres WAV íntegros, sistema técnico y clips funcionan. Esta sesión no dispone de una herramienta para escuchar audio: no se certifican escucha completa, continuidad audible, adecuación de victoria ni balance perceptivo de SFX. Se requiere revisión auditiva, disponible en la página de revisión. |

El ZIP de handoff V3 final no se crea ni libera mientras MUSIC GATE no esté aprobado. Los archivos presentes son evidencia de revisión. El handoff V2 es histórico y no representa el arte/música actuales.

## Pruebas funcionales

- CoreQa: 22 checks; PolishQa: 53; MusicRuntimeQa: 10. Todas aprobadas. Music QA comprueba dos fuentes persistentes, pista suministrada en menú, guardado PlayerPrefs, apertura suave, skip hacia 16 s, pausa del concurso mientras sigue la música, reanudación sin reinicio, ducking NULL, recuperación y loop anticipado al segmento con las mismas dos fuentes.
- Replay real en Unity Editor de cinco rondas, mediante Competition.Input/Tick. No se asignan métricas ni puntuación para fabricar imágenes. CIPHER 6173, VECTOR 3806, RELAY 3134, UPLINK 1544: ganador por score total. CIPHER eliminado en ronda 1 con Danger 100% y 415 puntos, regresa en ronda 2 conservando el total.
- Danger 50/80 cambia segmentos con histéresis 45/75. El clip 04 incluye tensión; clip 06 incluye crítico 80/96 y descarga/eliminación 100. NULL y ECHO se ven activos en ronda 2. Los resultados muestran score de ronda y acumulado.
- Windows x64: build final SUCCESS, 219540673 bytes según BuildReport. El ejecutable arrancó sin intervención de seguridad y completó smoke de intro y 12 s de gameplay, con nueve PNG capturados por el propio runtime. No se afirma haber completado cinco rondas en el standalone.
- WebGL: build final SUCCESS, 77102625 bytes según BuildReport. En localhost se verificaron carga, modelos/texturas, selección, entrada de texto real, cambio de prompt/score, pausa Escape y control de volumen. Tras recarga, volumen conservado y barra corregida sin desbordarse. Sin error bloqueante; un aviso de deprecación de sincronización persistente Unity no impide guardar.
- No se publicó Unity Play ni GitHub; GitHub CLI sigue sin sesión. No se modificaron My Flow, GDD, FigJam, Trello, Canva/PDF ni video académico final.

## Capturas y clips revisados

Nueve PNG de cuerpo completo/lateral, doce diagnósticos de pose y veinte PNG de partida/retratos del Editor. Diagnósticos aislados identificados como tales: no se presentan como gameplay normal. Modelo realista y movimientos procedurales de MVP; no se afirma animación facial individual, mocap ni dedos AAA.

Ocho MP4 1920×1080, 30 fps, H.264 + AAC estéreo. Música/SFX grabados en vivo desde la salida MASTER de Unity AudioRenderer, sin insertar WAV en posproducción. Los ocho clips se volvieron a capturar desde Unity con el aumento de música de aproximadamente 11.2 dB solicitado por Fernando. El contador de tiempo de captura es fijo: los clips no son benchmarks de FPS.

El decodificador Unity Editor llegó al timeout del primer archivo. Se empleó el navegador como alternativa: los ocho MP4 se reprodujeron hasta el final, sin error del elemento video. Se guardaron cinco fotogramas decodificados de cada clip (40 en total), revisados visualmente. Los contadores del reproductor no equivalen al número de samples H.264; los samples/duración/FPS exactos provienen del contenedor MP4.

Se comprobó PCM no vacío en los ocho segmentos, picos por debajo de 0.272 y ausencia de saturación. Duración AAC difiere menos de 22 ms del video por padding de codec. Esta verificación de señal no certifica balance audible ni ausencia de cortes musicales.

La captura `06_DANGER_WARNING.png` procede del fotograma del clip 04 a 3.93 s, Danger 56%, sin editar métricas. La captura original de aviso temprano 40% se conserva en la evidencia fuente y no se etiqueta como estado 50–79. Los alias de screenshots duplican archivos reales, no representan nuevas partidas.

## Límites y trazabilidad

Renderpeople tiene licencia gratuita propietaria, no CC0. Fuentes/texturas/prefabs derivados privados, excluidos de Git y paquetes fuente; se admiten renders y builds según su licencia. Script Tools/setup_renderpeople.py permite una descarga individual oficial y adaptación local, después de revisar términos. Ropa incluida con los modelos, recoloreada mediante sus máscaras, sin creación desde cero ni IA. Las proporciones y contacto fueron inspeccionados en las poses muestreadas, no se promete ausencia de clipping en cualquier pose futura.

Se corrigió el cierre del helper: Stop es idempotente, el coroutine sale antes de consultar AudioRenderer después de Stop, y el smoke standalone llama Application.Quit. Una captura nueva de menú Windows terminó con exit 0 sin el aviso anterior de AudioRenderer; los segmentos grabados previamente siguen siendo válidos.

ACCIÓN MANUAL PENDIENTE: autenticar GitHub CLI con gh auth login.

## Revisión de volumen solicitada por Fernando

MUSIC pasa de −14 a −6 dB y la ganancia base de 0.45 a 0.65: +11.2 dB. NULL/ECHO, eliminación, Danger, errores y typing reducen temporalmente la música para mantener señales competitivas. Ocho clips nuevos se reprodujeron completos y sus 40 fotogramas se revisaron nuevamente. PCM de salida MASTER: RMS de menú 0.02690 frente a 0.00752 anterior, sin saturación en ningún clip. Los WAV maestros mantienen SHA256 original. La aprobación auditiva del nuevo balance sigue pendiente.

## Validación adicional de audio WebGL

Antes del clic: contexto suspended y señal cero. Después de COMENZAR CON SONIDO: contexto running, RMS 0.016470520564355592, pico 0.03265218809247017, 48000 Hz. La fuente musical Mainframe aparece activa con buffer de 8001409 muestras, aparte del ambiente de 176400 muestras; los logs confirman repeticiones del segmento del menú. Esta prueba confirma reproducción musical, no sustituye la escucha del balance ni aprobación musical.
