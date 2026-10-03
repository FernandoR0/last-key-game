# Estado de implementación — LAST KEY



Costo adicional: **S/0**. Fase actual: MVP Unity y evidencia real para ChatGPT. No se ha generado GDD, PDF académico ni video final.



## Implementado



- Competencia local: CIPHER humano y tres bots, cinco rondas de 60 segundos.

- Validación de caracteres, WPM, Accuracy, Errors, Score, Combo y Danger.

- Puntuación acumulada, clasificación y ganador THE LAST KEY; empates explícitos.

- Danger 100% elimina solamente en la ronda actual. El jugador conserva puntos y retorna al comenzar la siguiente.

- NULL: Precision Protocol, Speed Check y Clean Streak; bonificación y reglas comunes.

- ECHO opcional: aceptar/ignorar, tiempo limitado, recompensa o penalización declaradas.

- Classic, Code, Numbers, Memory y Sudden Death. Son variantes jugables del mismo motor; no modos de campaña separados.

- Arena industrial compacta con cuatro escritorios Poly Haven, estaciones, monitores, teclados, sillas y racks.

- Participantes Quaternius Universal Base Characters Standard, CC0: CIPHER/Valeria Rojas, UPLINK/Ethan Cole, RELAY/Akira Mori, VECTOR/Noah Bennett.

- Vestuario por regiones del modelo, accesorios, variación de proporciones/tono de piel/peinado/color. Dos bases gratuitas; no se ha adquirido Source.

- Animación procedural original sobre rigs Humanoid: sentado, idle seated, typing, reacción a error, eliminación y victoria sentada. No se afirma uso de clips Mixamo.

- HUD, cambio de cámara, pausa, botones para ECHO, resultados y selección de variantes.



## Validación de gameplay



22 comprobaciones del núcleo aprobadas dentro de Unity. Un replay de QA envió entradas a través de la misma API de validación; no alteró scores, Danger ni resultados. No representa una partida humana ni multijugador online.



La competencia de evidencia completó cinco rondas: CIPHER 5417, VECTOR 3726, RELAY 3078, UPLINK 1520. Ganó CIPHER por puntuación acumulada. Se capturaron Danger 48%, 80%, 100%, eliminación y retorno en ronda 2 conservando 355 puntos.



Se revisaron individualmente las capturas de arena, HUD, Danger, NULL, ECHO, resultados, ganador, cuatro retratos y cinco variantes. Las capturas se renderizaron en Unity a 1920×1080; la captura técnica muestra el Editor real.



## No implementado



Chaos; Story Mode; historia profunda de AEGIS/NULL/ECHO; multijugador online; matchmaking; progresión/campaña; voz/narración; video académico final. Balance, vestuario y animaciones son del MVP y pueden pulirse posteriormente.



## Verificación de builds y publicación



Builds finales Windows x64 y WebGL compilados correctamente con personajes Quaternius. Los seis MP4 fueron decodificados completos y revisados visualmente. Ver handoff/chatgpt/QA_REPORT.md y WEBGL_RUNTIME_STATUS.md para resultados y límites.



Windows Smart App Control bloqueó el lanzamiento del ejecutable standalone y FFmpeg. Se validó el juego mediante Unity Editor sin desactivar seguridad. GitHub no bloquea el handoff local.

