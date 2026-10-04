# QA visual V4 â€” revisiÃ³n tÃ©cnica

No es aprobaciÃ³n artÃ­stica ni entrega de PDF/video acadÃ©mico.

- CoreQa: 22 PASS. PolishQa: 53 PASS. MusicRuntimeQa: 13 comprobaciones de audio.
- Humanoid Rain/Snow vÃ¡lido. Cabeza reexportada preservando pesos de cuello/pecho, prefabs regenerados con SetupModels. Retirado el recorte extra de piel que producÃ­a agujeros cerca de prendas. Los pesos no deformantes del rig fuente no se exportan.
- Collar ajustado por superficie de cuello con lÃ­mites individuales; sin antena. PoseAudit revisa sentado, despertar, tecleo y victoria en los cuatro personajes. Los diagnÃ³sticos son pruebas aisladas.
- Teclado fÃ­sico completo de 61 teclas visuales: Ã‘, tildes, Ãœ, nÃºmeros, puntuaciÃ³n, espacio y Code. En WebGL, la frase Â«No desperdicies una tecla. El tiempo sigue corriendo.Â» avanzÃ³ y produjo 240 puntos con Accuracy 100%. El primer test detectÃ³ contraste pobre durante combo; se corrigiÃ³ con texto oscuro sobre el destello.
- La captura bajo D3D12 perdiÃ³ el dispositivo al codificar MP4. D3D11 codificÃ³ los ocho clips anteriores; se utiliza D3D11 para la nueva evidencia. No se desactivÃ³ seguridad.
- Windows V4 anterior completÃ³ smoke hasta 12 segundos de gameplay y salida normal. Ese smoke precede al ajuste final de cuello; los nuevos builds se identifican por logs/manifest del handoff.

La UI heredada queda provisional y corresponde a Fernando. El modo visual oculta Canvas durante intro/gameplay. Por ello no presentar estados sin panel de evento como diseÃ±o definitivo del HUD.

No se afirma escucha perceptual, multijugador, publicaciÃ³n GitHub ni Unity Play. La revisiÃ³n final y los hashes se incluyen al completar los nuevos builds/MP4.

## Revisión después de las correcciones

PoseAudit completó las 16 poses de los cuatro personajes; nueve vistas de cuerpo/lateral regeneradas. Replay de cinco rondas finalizado sin asignar score. Ocho masters reales sin muestras recortadas. El build Windows nuevo completó smoke de menú, intro, cuenta atrás y 12 s de gameplay; cerró con exit 0. Los ocho MP4 H.264/AAC son 1920×1080 a 30 fps. VideoPlayer decodificó todos sus frames: 241, 962, 180, 120, 840, 180, 181 y 182. Se revisaron imágenes de inicio/intermedias/final de cada clip; no se afirma escucha perceptual.


La primera prueba del nuevo WebGL encontró meshes no legibles en el player al medir el collar. Se habilitó ModelImporter.isReadable en ambos personajes y se añadió una comprobación preventiva. Esta corrección de importación no cambia la geometría ni las animaciones capturadas.

Windows final con Read/Write habilitado completó smoke (menú → intro → cuenta atrás → 12 s gameplay → cierre normal, exit 0). Los cuatro collares registran measured=True, sin errores de lectura de vertices. La evidencia está en native-review y el log de validación.

Builds finales: LAST_KEY_WINDOWS_BUILD_SUCCESS 240482738 y LAST_KEY_WEBGL_BUILD_SUCCESS 86959024. Core/Polish volvieron a pasar durante ambos builds. Los clips son del mismo contenido visual; la última corrección afecta la disponibilidad de datos para medir el collar en el player.

WebGL revision=4.2 cargó, completó intro y aceptó la frase física «No desperdicies una tecla. El tiempo sigue corriendo.», produjo 240 puntos y avanzó al siguiente texto. Se guardó WebGL_keyboard_proof.png. No se registraron errores de consola asociados al framework 42neck; los mensajes 41neck previos pertenecen al build descartado.
