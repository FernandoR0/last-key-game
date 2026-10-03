# LAST KEY

**ESCRIBE. SOBREVIVE. SÉ EL ÚLTIMO.**

Competencia de mecanografía para PC: un humano (CIPHER) y tres bots. Cinco rondas de 60 segundos; vence la mayor puntuación acumulada. Danger 100% elimina solamente de la ronda actual. Los empates por puntos se declaran explícitamente.

## Proyecto y dependencias

- Unity **6000.6.0f1**, URP **17.6.0**, uGUI/TextMeshPro **2.6.0**.
- Windows Mono y WebGL. .NET 10 se utiliza únicamente para la comprobación independiente del núcleo; no es requisito del jugador ni del código Unity.
- Sin multijugador online, servicios pagos ni dependencias de cuentas durante el juego.
- `Assets/LastKey/Core/Competition.cs`: reglas, validación, puntuación, bots, rondas, NULL y ECHO, sin dependencia de Unity.
- `Assets/LastKey/Runtime/`: arena, interfaz y replay de QA.
- `Assets/Resources/last_key_rules.json`: reglas y balance editables.
- `Assets/Resources/last_key_content.json`: banco de textos.
- `Assets/LastKey/Editor/ProjectBuilder.cs`: preparación, builds y captura automatizada.
- `handoff/chatgpt/`: estado verificable, manifiestos y evidencia real.

## Abrir y jugar

1. Abrir esta carpeta desde Unity Hub con la versión indicada.
2. Abrir `Assets/LastKey/Scenes/LastKey.unity`. Si todavía no existe, usar **LAST KEY → Prepare Project**.
3. Pulsar Play; elegir Classic y **INICIAR COMPETENCIA**.
4. Escribir exactamente el carácter subrayado, incluidos espacios y signos. El cursor solo avanza al acertar; un error aumenta Danger y rompe el combo. No se necesita borrar el error.
5. ECHO puede aceptarse o ignorarse con los botones visibles; su riesgo y recompensa se muestran antes de aceptar. Al aceptarlo, el texto de escritura cambia temporalmente al desafío.
6. **SIGUIENTE RONDA** conserva el score acumulado y restablece Danger, combo y métricas de ronda.
7. `Enter` inicia o avanza desde los resultados. `F2` cambia la cámara. `ESC` pausa/reanuda.

Classic funciona sin saber programación. Code es una variante opcional de copia literal, sin ejecutar comandos ni evaluar conocimientos. Numbers usa secuencias. Memory revela una frase al inicio y luego oculta el objetivo. Sudden Death multiplica por cuatro la penalización de error. Chaos no está implementado.

## Balance inicial, no definitivo

Carácter correcto +1 × multiplicador de racha; palabra terminada +8. La racha aumenta el multiplicador cada 20 caracteres, hasta ×4. Error +8 Danger; carácter correcto −0.35 Danger. NULL emite una directiva común por ronda, rotando Precision Protocol, Speed Check y Clean Streak. ECHO: 12 segundos, éxito +150 puntos y −15 Danger, fallo +15 Danger.

WPM = caracteres correctos / 5 / minutos activos de ronda. Accuracy = correctos / (correctos + errores). Al eliminarse, las métricas dejan de aumentar hasta la siguiente ronda. La simulación de los bots envía caracteres por la misma validación que el humano.

## Construir y comprobar

Menú **LAST KEY → Build Windows** o **Build WebGL**. Salidas: `Builds/Windows/LAST_KEY.exe` y `Builds/WebGL/`.

Por línea de comandos del Editor: `-batchmode -nographics -quit -projectPath <carpeta> -executeMethod LastKey.Editor.ProjectBuilder.BuildWindows` (o `BuildWebGL`).

`CoreQa.Run` verifica las reglas dentro de Unity. La comprobación independiente se ejecuta con `dotnet run --project Tests/LastKey.Core.Tests.csproj -c Release`; Smart App Control puede impedir cargar los ensamblados locales.

`ClipEncoder.Encode --evidence-path <carpeta>` produce los seis MP4 a partir de frames reales. `ClipValidator.Validate` reproduce y decodifica los clips con el VideoPlayer de Unity.

## Evidencia

`ProjectBuilder.PlayEvidence` con argumentos `--evidence --evidence-path <carpeta>` ejecuta un replay de QA dentro de Unity. Envía entradas por la API normal y provoca errores reales para mostrar Danger. No modifica cifras ni resultados manualmente. Los frames se codifican con `UnityEditor.Media.MediaEncoder`; FFmpeg instalado fue bloqueado por Control de aplicaciones. ninguna imagen de gameplay procede de IA. Consultar el manifiesto de handoff para distinguir lo verificado de lo pendiente.

## Arte y licencias

Escritorios y textura de piso: Poly Haven, CC0. Los cuatro participantes usan Quaternius Universal Base Characters Standard, CC0: dos bases Humanoid con variaciones de proporción, tono de piel, peinado, barba y vestuario. CIPHER (Valeria Rojas), UPLINK (Ethan Cole), RELAY (Akira Mori) y VECTOR (Noah Bennett) comparten una misma dirección artística. La animación original sobre sus huesos incluye sentado, idle seated, typing, error, eliminación y victoria sentada. No depende de Mixamo. Arena y periféricos son geometría procedural propia. Ver `ASSET_MANIFEST.md`. Costo adicional: **S/0**.

La historia profunda de AEGIS, NULL y ECHO queda para Story Mode futuro. La presentación académica, FigJam y Trello están fuera de este repositorio.
