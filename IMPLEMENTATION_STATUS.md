# LAST KEY — revisión visual V4

Actualización sobre el proyecto existente. Costo adicional S/0. La aprobación artística de Fernando sigue pendiente; esta entrega permite revisar cambios reales, no declara terminado el videojuego.

## Cambios visuales

- Dirección 3D de historieta: cuatro variantes Humanoid de modelos gratuitos Rain/Snow, prendas existentes recoloreadas y peinados/accesorios reutilizados. CIPHER turquesa, UPLINK azul, RELAY violeta y VECTOR ámbar. Fuente y atribución en ASSET_MANIFEST.md.
- Mesas/sillas Omie CC0, pantalla horizontal integrada sin monitor ni teclado externo. 61 teclas visuales: A–Z, Ñ, vocales acentuadas, Ü, números, signos, espacio y símbolos de Code. La tecla se ilumina al escribir con el teclado físico. No se implementó teclado por clic/touch.
- Arena compacta con paneles y suelo Kenney, elementos Quaternius, materiales iluminados, sombras y acentos por estación. Cámara de primera persona estabilizada que permite ver mesa, manos y tres rivales.
- Intro de 26 segundos solo antes de la primera ronda: despertar, collar, comprobar sujeción, rivales despertando, encendido NULL y reglas. Cuenta atrás posterior; tiempo competitivo separado de la intro.
- Animaciones sentadas gratuitas retargeteadas y mocap corporal CMU 79/85 para typing. IK para silla, piernas/manos y contactos; el registro CMU no aporta dedos individuales ni una pierna derecha válida. Expresiones y parpadeo con seis morphs de rig fuente. Victoria suavizada; eliminación sin cambiar la cámara de juego.
- Error rojo / combo con color de participante en la pantalla física; collar y descarga estilizada. SFX gratuitos de teclas, combo, NULL/ECHO, victoria y grito CC0.
- Una sola fuente musical activa, gain revisado, mute persistente y apagado de fuentes propias al salir. Los WAV originales permanecen sin modificaciones. No se afirma revisión auditiva humana.

## Núcleo conservado

Una persona y tres bots; cinco rondas, puntos acumulados, WPM/Accuracy/Errors/Score/Combo/Danger. Danger 100% elimina solo de la ronda, permite volver en la siguiente. NULL y ECHO. Classic; Code, Numbers, Memory y Sudden Death BETA. Chaos/Story Mode no implementados.

## Verificaciones

CoreQa 22 + PolishQa 53 aprobadas. MusicRuntimeQa 13 comprobaciones de fuentes, volumen/mute, pausa, seek, ducking y cierre. Replay de cinco rondas con entradas reales al núcleo, sin asignar puntuaciones para fabricar resultados. Nueve vistas de cuerpo completo/laterales y 16 diagnósticos de poses revisados; capturas primera persona y estados reales. Los diagnósticos están separados de las capturas de partida.

Ocho pistas MASTER capturadas por Unity AudioRenderer durante el replay: ningún sample digital recortado. Esta comprobación de señal no sustituye la escucha de Fernando.

## Límites y reparto

Canvas/HUD/menús, navegación TAB/ENTER, controles de volumen visibles y salas privadas/multijugador corresponden a Fernando. La interfaz anterior queda provisional. La revisión /?visual=1 oculta Canvas en intro/gameplay, por lo que ciertas capturas muestran el entorno de un evento sin panel HUD; no presentarlas como diseño final de NULL/ECHO o métricas completas.

No se modifican GDD/Trello/Figma/Canva/PDF ni se produce video académico final. GitHub/Unity Play no se declaran publicados. Las fuentes privadas Renderpeople históricas no forman parte de los personajes activos ni se redistribuyen en el handoff.
