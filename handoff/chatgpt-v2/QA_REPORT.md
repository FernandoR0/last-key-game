# QA V2 — LAST KEY

## Resultado y evidencia

| Puerta | Resultado | Evidencia y alcance |
|---|---|---|
| CORE | PASS | CoreQa 22 + PolishQa 53 = 75 comprobaciones aprobadas en Unity; selección humano/bots, input, scoring, Danger, rondas, memoria de prompts y ECHO. |
| FLOW | PASS | Replay de cinco rondas reales en el motor; carga, menú, selección, configuración, intro, cuenta atrás, gameplay, resultados y THE LAST KEY. WebGL también completó cinco rondas mediante los botones. |
| UX | PASS | Menús reales, cuatro personajes seleccionables, Classic principal y variantes BETA, controles visibles, resultados alineados y explicación de eliminación solo por ronda. |
| VISUAL | PASS | Veinte PNG de Editor revisados individualmente; doce diagnósticos de postura de los cuatro modelos; mano de intro fuera del torso, asiento sin atravesar muslos, muñecas de victoria alineadas. |
| GAMEPLAY CAMERA | PASS | 07_GAMEPLAY_COMPOSITION_PROOF: humano/manos/teclado/monitor y tres rivales/estaciones en el mismo encuadre. Seis cámaras nombradas en CameraDirector. |
| CONTENT | PASS | Prompts con shuffle bag, dificultad y memoria reciente; tests de ausencia de duplicados. Classic, Code, Numbers, Memory y Sudden disponibles; Chaos/Story Mode no implementados. |
| WEBGL | PASS con alcance de smoke test | Build correcto; carga, menús, selección, input correcto/error, métricas, bots, HUD y resultados final verificados en navegador local. Sin errores nuevos de consola asociados al build 127.0.0.1. |
| CAPTURES | PASS | 21 PNG principales, cuatro retratos incluidos en esa cuenta, nueve MP4 1920×1080/30 fps con pista de audio y frames decodificados completos; muestras decodificadas revisadas. |

## Partida de evidencia de Unity Editor

Entradas reproducidas a través de Competition.Input/Tick, sin asignar métricas o resultados. CIPHER gana con 6173, VECTOR 3806, RELAY 3134, UPLINK 1544. CIPHER pierde la primera ronda con Danger 100% y 415 puntos; regresa a la segunda con 415 acumulados y Danger 0%. Los screenshots 10/11/12 muestran 48/80/100% y eliminación. 14 muestra el reto ECHO aceptado y 16 la victoria final.

La captura de carga y su clip se volvieron a crear después de corregir el progreso de la barra. Los restantes estados pertenecen a la misma versión funcional; únicamente cambió la representación de esa barra. capture_states.json registra la procedencia por estado/cámara/métricas. Los retratos son cámaras del juego tras los resultados, con HUD oculto, no renders de IA.

## Revisión de posturas

Se ajustaron asiento, respaldo, posición de caderas y objetivos IK de tobillos; se añadió reposapiés. Se adelantó la mano de despertar y se redujo alcance de brazos en victoria con orientación de manos independiente. Se revisaron los cuatro personajes sentado, despertando y ganando. technical/pose-review contiene pruebas aisladas identificadas como diagnóstico, no prueba de resultados normales. La eliminación incluye collar rojo y descarga estilizada; el monitor oculta parte del cuerpo en su cámara, pero se ven cabeza, collar y manos. El arte sigue siendo de MVP estilizado.

## WebGL

Interfaz revisada a 1920×1080, 1600×900 y 1366×768: texto legible, sin recorte de HUD ni bandas blancas. Canvas 16:9 con márgenes oscuros cuando corresponde. El ajuste de tamaño necesita un frame para estabilizarse; las capturas se guardaron después del ajuste.

Se cambiaron los cuatro personajes con botones y se confirmó VECTOR como humano. La entrada «Respira. » avanzó nueve caracteres; «x» produjo Errors 1, Accuracy 90%, Score 17 y Danger 8%. En otra ronda «Escribe » dio Combo 8. El bot CIPHER quedó eliminado en ronda 4 y regresó en ronda 5. Resultado final de esa sesión: RELAY 3823, UPLINK 3012, CIPHER 1570, VECTOR 184; gana RELAY.

ECHO se verificó aceptado/resuelto en el replay de Editor y su caducidad automática en WebGL. Los intentos físicos F1 en el navegador llegaron después de vencer la oferta; no se presenta esa sesión como confirmación independiente de aceptación con F1/F2. No se hizo ensayo de carga, compatibilidad de navegadores múltiples ni balance completo de variantes BETA.

El fallo anterior de shader nulo se corrigió incluyendo LastKeyElectric.mat en Resources. El error de consola del build viejo localhost está separado del nuevo build probado por 127.0.0.1. No se desactivó Smart App Control ni se sorteó una pantalla de seguridad.

## Clips y sonido

Nueve MP4. ClipValidator/VideoPlayer de Unity decodificó todos los frames; clip_decode_report.json registra conteos esperados/obtenidos. Hubo timeouts intermitentes del decodificador de Windows; repetir en el Editor gráfico y solicitar su player loop permitió completar los nueve. No se ocultan esos reintentos como fallos del juego.

Se revisaron muestras al inicio, cuarto, mitad, tres cuartos y final de cada clip, además de capturas cercanas de posturas. MP4 contiene una pista de audio; audio_source_report.json comprueba muestras PCM no nulas y amplitud. El audio se mezcló con los mismos clips sintetizados que dispara el juego, según sus eventos temporales; no es narración ni audio capturado del micrófono. No se afirma auditoría auditiva completa.

## Windows, publicación y entrega

Windows x64 compilado correctamente (Development/Mono); WebGL compilado correctamente. Smart App Control impide ejecutar el standalone en este equipo. La validación de runtime se hizo en Editor y WebGL; Windows solo tiene validación de compilación. technical/22_UNITY_EDITOR.png muestra Play real.

GitHub CLI sin sesión. El conector identifica la cuenta, pero no dispone de creación de repositorios y no hay last-key-game existente accesible. No se escribió en My Flow ni en otro repositorio. El proyecto, commits y bundle están preparados localmente. Unity Play no publicado; no se declara link público inexistente.

ACCIÓN MANUAL PENDIENTE: autenticar GitHub CLI con gh auth login

STOP — WAIT_FOR_CHATGPT_PDF
