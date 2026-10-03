# WebGL — comprobación de runtime

Build final Unity 6000.6.0f1 completado. Servido por HTTP local y cargado en el navegador integrado de Codex, sin cambiar protecciones de Windows.

Se comprobó visualmente:

- Arena con cuatro estaciones y participantes Quaternius.
- HUD completo en una ventana compacta: contenedor adaptado a 16:9.
- Botón INICIAR COMPETENCIA inicia ronda y temporizador.
- Teclado envía L, a, espacio y z: tres aciertos y un error. Accuracy 75%, Errors 1 y Danger 8%; texto avanza y score cambia.
- Bots acumulan puntos y la clasificación cambia durante la ronda.
- No errores ni advertencias nuevos en la consola del build corregido durante esta comprobación.

Se preservaron colliders utilizados indirectamente por CreatePrimitive para evitar que IL2CPP los elimine. La carga anterior mostraba avisos de CapsuleCollider; corregidos en el build entregado.

technical/23_WEBGL_KEYBOARD_UI.png es evidencia real del navegador. Las capturas principales 1080p proceden de Unity Editor y del replay de QA.

Esta prueba cubre carga, teclado y UI de inicio/métricas; la competencia completa de cinco rondas y variantes fue validada mediante el runtime de Unity Editor. No afirmar que se probó una partida humana completa en WebGL. Unity Play no publicado; no se inventa enlace público.
