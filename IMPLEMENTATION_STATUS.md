# LAST KEY — diseño integrado, revisión 19

Se continúa sobre el proyecto existente. Costo adicional S/0. Cuatro copias temporales del ActorCore Party-M-0001 aprobado, manteniendo CIPHER/UPLINK/RELAY/VECTOR. Las revisiones V4 y el piloto aislado son antecedentes, no el estado actual.

## Integración

- Mocap Mixamo descargado: idle sentado, despertar, contacto con collar, tecleo, descarga y victoria. Se reutilizan las adaptaciones de alcance del piloto aprobado.
- Collar metálico importado ligado al cuello, contacto de manos y efectos de Danger. Mesh readable habilitado también en build.
- Despertar/collar/rivales/NULL antes de la primera ronda; esa introducción no se repite en las siguientes.
- Descargas débiles/medias a Danger 40/70 y eliminación a 100. El eliminado deja de competir solo en esa ronda y vuelve después. El ganador final reproduce el motion de victoria.
- Pantalla horizontal integrada y teclado español con signos y tildes. Corregida la asociación A/B/T, antes absorbida por el rótulo TAB.
- HUD delegado a Codex: clasificación de los cuatro, texto inferior y avisos NULL/ECHO/combo. TAB selecciona respuesta ECHO y ENTER confirma. Tarjetas ocultas cuando el evento no está activo.
- Menús conservados para probar. Las teclas físicas generan la entrada; no hay teclado táctil por clic.

## Núcleo y límites

Una persona y tres bots. Cinco rondas de 60 segundos; score acumulado, WPM/Accuracy/Errors/Score/Combo/Danger. NULL y ECHO conservados. Classic; Code/Numbers/Memory/Sudden Death BETA. Chaos, campaña, personalización y multijugador de salas no implementados.

Las voces quedan para el final por instrucción de Fernando. No se integra la muestra personal ni el grito sintético rechazado. Se retiró el grito genérico de eliminación. Música y efectos permanecen; no se afirma aprobación auditiva.

No se modifican GDD, Trello, Figma, Canva/PDF ni se produce video académico final. GitHub y Unity Play no se declaran publicados. ActorCore/Mixamo permanecen privados; reconstruir requiere obtener las fuentes bajo sus licencias.

## Validación

Build Windows correcto. CoreQa 22 y PolishQa 53 aprobadas. Replay nativo completo de cinco rondas sin excepciones/assertions: NULL/ECHO, Danger, eliminación, retorno y ganador. Entradas pasan por Competition.Input/Tick, sin asignar scores ficticios. Última composición y build WebGL registrados en VISUAL_INTEGRATION.md.
