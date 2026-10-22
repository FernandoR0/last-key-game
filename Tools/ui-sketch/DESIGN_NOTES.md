# LAST KEY — Boceto de interfaz 26

Solicitud: integrar la revisión 25 en el juego principal y rediseñar la interfaz con una estética de web minimalista y elegante. Primero se entrega este boceto; la aplicación del nuevo HUD a Unity queda para el siguiente paso.

## Dirección

El texto que se escribe es el protagonista: dos líneas grandes sin recuadro. El mundo y las manos permanecen visibles. El marcador ocupa una columna pequeña a la derecha. Velocidad, precisión, errores y Danger se leen en el borde inferior. NULL/ECHO usan una zona lateral con título grande, explicación corta, tiempo y acciones cuando corresponda.

Paleta: papel frío `#eaf0f3`, tinta azul `#253b49`, petróleo `#167e88`, hielo `#bce8e5`, oro de combo `#e5ae63`, coral de error `#ed8790`. Blanco para el texto principal sobre el escenario. El menú emplea una superficie clara para evitar otra propuesta enteramente oscura. La partida conserva el contraste sobre la arena actual.

Tipografía: Aptos con Segoe UI de respaldo, disponibles localmente; sin fuentes remotas ni instalaciones. Escala contenida: frase 2,65 cqw, marcador 1,02 cqw, métricas 1,45 cqw, reloj 2,2 cqw, con tamaños mínimos para el panel de vista previa. Iconos SVG sencillos para volumen, pausa y marca.

Composición: marca y ronda a la izquierda; tiempo centrado; controles y clasificación a la derecha. Menú con texto a la izquierda y personaje a la derecha. Resultados con tabla y prioridad en puntuación acumulada. Se descartaron paneles idénticos y recuadros grandes porque competían con el espacio de mecanografía.

## Contenido del boceto

- Partida, menú y resultados navegables.
- Estados de combo, error, NULL, ECHO y Danger activables manualmente.
- ECHO se elige con Tab y Enter; al aceptar muestra `NODE-07-RECOVERY`.
- Volumen con icono alineado y control que actualiza su porcentaje. No reproduce audio: es un control del boceto, no una corrección del mezclador del juego.
- Opción de reducir movimiento y respeto de `prefers-reduced-motion`.
- Las cifras y los tiempos son ejemplos, no una partida en ejecución. El menú se actualizó a «Modos de juego», sin controles de salas.

## Evidencia y alcance

Los fondos se renderizaron de nuevo en Unity con la arena y los cuatro personajes actuales mediante `UiMockupCapture`, en una escena temporal sin guardar cambios en la escena de producción. Se usan la malla y las animaciones actuales; no se generaron imágenes con IA ni se modificaron la cara, el collar o el forcejeo. La pantalla está integrada en la mesa.

Este HTML es el boceto del nuevo HUD. No es una build nueva del juego ni confirma la integración del HUD en Unity. La revisión cinematográfica 25 continúa separada para su revisión. Siguiente trabajo: llevar la composición elegida al Canvas principal y comprobar la intro 25, estados de partida, intervenciones, controles de sonido y resultados en la build.

Herramientas: Unity ya instalado, HTML/CSS/JS y navegador local. Dependencias externas nuevas: ninguna. Costo adicional: S/0.
