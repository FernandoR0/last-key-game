# LAST KEY — plan de reparto visual / UI

Estado: PROPUESTA. No se ha refactorizado el juego ni implementado multijugador en esta revisión.

## Dirección recomendada

3D estilizado de historieta: siluetas distintas, ropa moderna, sombras claras, contornos discretos y animación fluida. Conservar CIPHER, UPLINK, RELAY y VECTOR. Probar una estación y un personaje antes de sustituir los cuatro. El modelo definitivo debe demostrar parpadeo, mirada y expresiones en Unity: Humanoid corporal no demuestra capacidad facial.

## Responsabilidades

| Área | Fernando | Codex |
|---|---|---|
| UI del jugador | Menús, HUD, ventanas, tipografía, métricas, resultados, ajustes y accesibilidad | Puntos de conexión; no rediseñar UI |
| Entrada | Mecanografía, foco, navegación TAB/ENTER, aceptación/rechazo ECHO, selección y pausa | Animar únicamente en respuesta a eventos |
| Competencia | Reglas, score, combos, Danger, rondas, equipos | Representación visual sin modificar reglas |
| Salas privadas | Crear/unirse, ready, invitaciones, 1v1/2v2/4 libres, host y sincronización | Puestos, avatares y reacciones sincronizadas a eventos recibidos |
| Personajes | Selección desde UI | Assets, ropa, rigs, expresiones, clips existentes y corrección de cruces |
| Arena | Preferencias de UI de pantalla | Escritorios, sillas, sujeciones, collar, pantalla táctil, luz y materiales |
| Cámaras/cinemática | Acción de omitir y estado de partida | Primera persona, prólogo solo ronda 1 y transición al juego |
| Sonido audiovisual | Slider y preferencias | Reproducción exclusiva, ciclo de vida, música, tecleo, combos, descarga y grito |

La red se propone a cargo de Fernando para respetar que Codex asuma la presentación audiovisual. No se presupone que esté ya implementada.

## Separación del código actual

LastKeyGame.cs mezcla actualmente UI, inputs y flujo. Se propone separar GameFlowController, PlayerInputController y PlayerUIController (Fernando). Competition.cs y PromptManager.cs siguen como núcleo. CharacterFactory, CharacterMotion, CameraDirector, ArenaFactory, CompetitionEffects, GameAudio y MusicManager quedan a cargo de Codex; la lógica de efectos no decide puntuaciones ni eliminación.

Contrato propuesto, aún no implementado: snapshots de participante (identidad, equipo, estación, WPM, Accuracy, Errors, Score, Combo, Danger, Eliminated) y eventos MatchPrepared, IntroStarted, TypingAccepted, TypingMistake, ComboReached, DangerChanged, DirectiveChanged, EchoChanged, RoundEliminated, RoundEnded, MatchEnded y SessionClosed. El núcleo emite; UI y visuales escuchan. Los visuales no interceptan TAB/ENTER ni escriben scores. Music Volume se envía desde UI al servicio de audio.

## Primera ronda

Secuencia: despertar con respiración/parpadeo; tocar e intentar retirar collar; mirar sujeciones de cintura/piernas sin bloquear manos para escribir; observar otros tres participantes despertando con desfases; encendido de pantalla integrada al escritorio, logo NULL y reglas; cuenta regresiva; primera persona con manos y rivales visibles. En rondas posteriores: preparación breve y countdown, sin repetir prólogo. El reloj competitivo empieza después del prólogo/countdown. Omitir llega al mismo estado funcional.

Las reglas de NULL explican competencia por rondas, ganador por puntos acumulados y Danger 100% fuera solo de la ronda. La narrativa no cambia esas reglas.

## Gameplay audiovisual

Pantalla física: error rojo breve, combo acento de color y sonido, estado normal recuperado sin tapar texto. Collares legibles con carcasa, cierre y señal luminosa. Animación de tecleo adaptada al plano de la pantalla táctil, con manos sin atravesar mesa/torso. Ropa/silla revisadas en despertar, collar, tecleo, descarga y victoria. Eliminación: descarga y reacción/grito espacial de impacto, manteniendo vista del jugador y reloj de rivales; no corte automático de cámara ni pausa global.

## Audio pendiente

Hay dos fuentes A/B con solapamiento de transiciones y loop del menú 0–16 s. Se propone reproducción exclusiva sin repetir la apertura por cada cambio de menú, salida de menú antes de intro, y volumen probado en build mediante UI real. El audio que continúa tras cerrar exige revisar instancias de build/WebGL/reproductores y el cierre de sesión; no atribuir causa sin reproducir. SessionClosed detiene fuentes y libera reproductores. No iniciar audio en ventanas de QA cerradas por Fernando.

## Reutilización antes de creación

Fuentes verificadas para selección: Quaternius Universal Base Characters (CC0, FBX/Humanoid), Universal Animation Library (edición gratuita CC0; incluye sentado), Mixamo oficial (gratis con Adobe ID, animaciones corporales; no prometer facial), CMU mocap (libre para todos los usos), Blender Studio Rain/Snow (estilizados; comprobar licencia concreta y exportación facial a Unity). Rain v1 declara CC-BY. Shader de historieta: evaluar el ejemplo público SimpleURPToonLit de NiloCat bajo MIT, distinto del producto comercial.

Collar y grito: candidatos externos por confirmar; ningún asset se considera integrado ni autorizado por estar simplemente publicado en GitHub. Verificar autor, licencia, edición realmente gratuita, geometría, rig y uso/redistribución antes de descargar e integrar. No prometer que existe gratuitamente un clip exacto de arrancarse un collar. Adaptar clips existentes mediante retargeting, capas y ajustes de contacto mínimos, evitando fabricar secuencias enteras por código.

## Salas y costo

Sin matchmaking público. Propuesta inicial: host autoritativo y salas privadas con 1v1, 2v2 y cuatro libres. 2v2 necesita definir victoria por suma de puntos del equipo; el host valida puntuaciones y sincroniza reloj, Danger y eventos. LAN y salas por Internet son rutas diferentes. WebGL no admite UDP directo; requiere transporte compatible. Relay tiene cuotas gratuitas y facturación fuera de cuota; no habilitar cobros para prometer S/0. Elegir la ruta de sala antes de integrarla.

## Orden

1. Acordar reparto y contrato, luego separar UI/inputs/flujo.
2. Probar una estación estilizada y un personaje con animación corporal/facial existente.
3. Validar cuello, manos, ropa, silla, primera persona y pantalla táctil.
4. Replicar los cuatro, crear prólogo solo ronda 1 y corregir audio/lifecycle.
5. Conectar la UI de Fernando y los eventos; integrar salas y modos de equipo.
6. Probar dos clientes, rondas posteriores, error/combo, eliminación, cierre de audio y builds Windows/WebGL antes de capturas finales.

Fuentes:
- https://quaternius.com/packs/universalbasecharacters.html
- https://quaternius.com/packs/universalanimationlibrary.html
- https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html
- https://mocap.cs.cmu.edu/
- https://studio.blender.org/characters/rain/v1/
- https://studio.blender.org/blog/snow-rain-updated/
- https://github.com/ColinLeung-NiloCat/UnityURPToonLitShaderExample
- https://docs.unity.com/en-us/mps-sdk/relay-limitations
- https://docs.unity.com/en-us/services/pricing-and-billing
