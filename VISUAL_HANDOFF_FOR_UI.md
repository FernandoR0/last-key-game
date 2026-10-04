# LAST KEY — límites visuales / interfaz

Estado: revisión técnica V4 preparada, aprobación artística pendiente. No GDD ni PDF académico, no video final.

La parte visual usa un cuarto industrial compacto, mesas con pantalla táctil horizontal integrada, caracteres y teclas impresos en la propia pantalla; primera persona durante competencia. CIPHER turquesa #33D6CE, UPLINK azul #4D8CFF, RELAY violeta #B58CFF, VECTOR ámbar #F5B554. Ambiente azul gris, superficies de escritorio claras, pantalla azul casi negro. Error rojo breve; combo adopta el color del participante.

Código del agente visual: ArenaFactory, VisualProps, CharacterFactory, CharacterMotion, CharacterFace, ReusableCharacterMotion, CameraDirector, TouchKeyboardVisual, CompetitionEffects y GameAudio/MusicManager para integración de audio. Asset imports: VisualAssetSetup. Las teclas táctiles son visualización del teclado físico, no un teclado móvil interactivo.

Parte de Fernando: Canvas/HUD/menús y su lenguaje gráfico, TAB/ENTER, controles de volumen en interfaz, interfaces de servicios, salas privadas 1v1/2v2/4 y networking. La UI anterior sigue siendo provisional. No se implementaron salas ni matchmaking en esta fase.

Puntos de conexión: Game.Input -> Arena.TouchKeys[HumanIndex].Press(char) para iluminar una tecla; Arena.Animate toma estado de Competition para prompt/errores/combos/Danger; Music.SetVolume(float 0..1) persiste preferencia; Sound.Cue(nombre) dispara efectos; LastKeyGame.StopSessionAudio() apaga el audio propio de la sesión. No hace falta reconstruir el núcleo.

URL de revisión física del WebGL: /?visual=1&revision=4.2; oculta Canvas durante intro y gameplay y mantiene resultados para avanzar. No se debe identificar un build anterior como V4.
