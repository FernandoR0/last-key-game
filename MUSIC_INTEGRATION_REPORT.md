# LAST KEY — integración musical V3 en revisión

Estado: integración técnica implementada; revisión auditiva NO VERIFICADA. No se declara MUSIC GATE PASS ni handoff final.

## Masters

Los WAV de Fernando fueron copiados sin modificar. Las sumas SHA-256 coinciden con los originales.

| Pista | Duración | PCM original | SHA-256 |
|---|---|---|---|
| Iron Dominion | 66.368 s | 48000 Hz / 2 canales / 16 bit | `879e1f41b46d561cceada84a447b12b2ee4f398cb5de145f0795d55c5b438bba` |
| Dead Current | 52.203 s | 48000 Hz / 2 canales / 16 bit | `38d519587b8915c02c30504a633b6ea695d3a0cbdbd78f18fecc63eb99228856` |
| Mainframe | 166.688 s | 48000 Hz / 2 canales / 16 bit | `5f4853f22b95d6a11e421a081408854c28722ccf3c58559cf7dfb104b0a0e03c` |

## Asignación provisional por segmentos

Esta asignación se basa en la envolvente de amplitud medida en los archivos completos. No sustituye la escucha íntegra solicitada. Las estimaciones automáticas de BPM tienen ambigüedad de mitad/doble tempo y no se presentan como BPM certificados.

| Pista | Uso | Offset | LoopStart | LoopEnd |
|---|---|---:|---:|---:|
| Mainframe | Menú, selección, preparación y resultados de ronda | 0 s | 0 s | 16 s |
| Iron Dominion | Despertar, cuenta regresiva | 0 s | 16 s | 60 s |
| Iron Dominion | Gameplay normal o entrada directa | 16 s | 16 s | 60 s |
| Dead Current | Danger 50–79 | 16 s | 16 s | 48 s |
| Dead Current | Danger 80–100 | 28 s | 28 s | 48 s |
| Iron Dominion | THE LAST KEY / victoria | 40 s | 40 s | 60 s |

Iron Dominion presenta una caída de energía en 14–15 s seguida de un aumento fuerte en 16 s. La introducción dura 13 s y el countdown 3 s, para aprovechar ese cambio; SPACE permite saltar la introducción y entrar con offset de 16 s y fade de 0.5 s. El original mantiene su apertura completa.

## Sistema implementado

MusicManager controla dos fuentes A/B, seek por timeSamples, crossfade de potencia constante y anticipación del loop. Transiciones de 1–1.2 s, excepción entrada directa de 0.5 s. Los segmentos evitan volver del final del archivo al principio, pero su adecuación musical y la ausencia de cortes audibles requieren escucha.

Mixer: MASTER con MUSIC −6 dB, SFX 0 dB, UI −3 dB y AMBIENCE −14 dB. Ganancia base musical 0.65 × Music Volume (por defecto 0.7), SFX 0.32. Music Volume existe en menú/pausa y persiste en PlayerPrefs. Pausa mantiene el punto de reproducción y reduce la música al 45%.

NULL/ECHO: duck musical de 6 dB durante 1.1 s; eliminación: 10 dB durante 1.5 s; alerta Danger: 5 dB durante 0.8 s. Typing reduce música 2 dB durante 0.1 s y error 4 dB durante 0.25 s. SFX independientes. Se aumentó el nivel musical aproximadamente 11.2 dB a pedido de Fernando, sin modificar los WAV. Danger sube a tensión en 50 y crítico en 80, con retorno en menos de 45/75 para evitar cambios repetidos.

AudioImporter conserva Vorbis quality 0.7, estéreo y carga en segundo plano. Default/Editor/WebGL: DecompressOnLoad y preload activado; Standalone: override Streaming y preload desactivado. Unity confirmó ambos ajustes; rechazaba los overrides de loadType específicos para WebGL. La página exige COMENZAR CON SONIDO por gesto del usuario y espera tanto AudioClip Loaded con samples > 0 como el contexto Web Audio running antes de iniciar o buscar una pista. Esto evita el silencio causado por intentar reproducir AAC durante la carga asíncrona. timeSamples se establece antes de Play. Un tap de AnalyserNode permite verificar señal del Web Audio sin añadir sonido ni modificar los WAV. Fuente técnica: https://docs.unity.com/en-us/engine/6000.0/manual/platform-specific/webgl/develop/audio .

## Evidencia y límites

Los clips de revisión se capturan del gameplay ejecutado en Unity con inputs de QA enviados al núcleo real. La música y SFX se registran mediante AudioRenderer desde la salida MASTER, sin insertar música ni recrear la mezcla después. Captura sincronizada a 30 fps; no son una medición de rendimiento en tiempo real. Verificación: diez checks de MusicRuntimeQa aprobados; ocho segmentos PCM no vacíos y sin saturación; ocho MP4 H.264/AAC 1080p30 reproducidos completamente en navegador con 40 muestras decodificadas revisadas. Windows y WebGL recompilados; smoke real Windows y teclado/UI WebGL aprobados. La primera revisión fue validada visualmente; tras el reporte de volumen bajo se regeneran los ocho clips con la nueva mezcla y se verifican de nuevo. Detalle en QA_REPORT_V3.md.

MUSIC GATE permanece pendiente hasta verificar por escucha la adecuación de los segmentos, loops y balance de SFX. CHARACTER QUALITY GATE y GAMEPLAY CAMERA GATE: PASS tras revisar los cuatro modelos, doce poses, gameplay Editor/standalone y retratos. La escucha sigue siendo una comprobación separada y pendiente. No se crea el handoff V3 antes de pasar los gates.
