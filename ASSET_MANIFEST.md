# Asset manifest — LAST KEY



Costo adicional total: **S/0**. No se han contratado suscripciones, créditos ni assets pagos.



| Recurso | Autor/fuente | URL | Licencia | Archivo utilizado | Uso | Atribución |

|---|---|---|---|---|---|---|

| Metal Office Desk | Ulan Cabanilla / Poly Haven | https://polyhaven.com/a/metal_office_desk | CC0 | `Assets/Resources/PolyHaven/metal_office_desk.fbx`, `metal_office_desk_diff_1k.jpg` | Cuatro estaciones de Arena | No obligatoria; crédito incluido |

| Metal Plate | Rob Tuytel / Poly Haven | https://polyhaven.com/a/metal_plate | CC0 | `Assets/Resources/PolyHaven/metal_plate_diff_1k.jpg` | Piso de Arena | No obligatoria; crédito incluido |

| Arena, paredes, tuberías, racks, PC, teclados, monitores, sillas, collares | Autoría procedural de este proyecto | `Assets/LastKey/Runtime/ArenaFactory.cs` | Código y geometría originales del proyecto | Generados en runtime | Arena | Sin requisito de tercero |

| Universal Base Characters Standard | Quaternius | https://quaternius.com/packs/universalbasecharacters.html | CC0 1.0; License_Standard.txt incluido | Assets/Resources/Quaternius: bases FullBody male/female, cabello, barba, ojos y texturas | Cuatro variantes Humanoid CIPHER/UPLINK/RELAY/VECTOR; vestuario y animación procedural propios | Crédito incluido |

| TextMeshPro Essential Resources y shaders | Unity Technologies | https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TMP.html | Licencias incluidas en el paquete Unity | `Assets/TextMesh Pro/` | HUD y scoreboard | Conservar avisos incluidos |

| Liberation Sans | Google (2010) / Red Hat (2012); distribuido en TMP | Fuente y licencia dentro de `Assets/TextMesh Pro/Fonts/` | SIL Open Font License 1.1 según archivo incluido | `LiberationSans.ttf`, `LiberationSans SDF.asset` y fallback | Texto | Conservar licencia de la fuente |



Licencia Poly Haven verificada: https://polyhaven.com/license . API pública usada para obtener los enlaces directos de los archivos, sin descargar renders promocionales como gameplay.



## Recursos preferidos revisados, todavía no integrados



- MakeHuman: https://www.makehumancommunity.org/ . No se ha usado ningún modelo ni asset adicional; no se atribuyen estos avatares a MakeHuman.

- Mixamo: https://www.mixamo.com/ . Ningún rig/animación descargado; no dependencia de login para el MVP.

- Omie's Office Set: https://omies-assets.itch.io/omies-assets-office-set . Referencia de mobiliario, pack anunciado CC0/name-your-own-price; no archivo del pack incorporado.

- ambientCG: https://ambientcg.com/ . Sin textura incorporada.

- Poly Haven Modular Industrial Pipes y Utility Box: referencias; la tubería y los racks actuales son primitivas propias, no estos modelos.



Los placeholders primitivos se sustituyeron por dos bases Quaternius y cuatro variantes de piel, proporciones, peinado y vestuario. Animación original sobre huesos Humanoid: sentado, idle seated, typing, error, eliminación y victoria. Mixamo no fue necesario. Las capturas muestran estos modelos reales en Unity; ninguna es generada por IA.


## Incorporaciones V2

- Ropa: shader original `Assets/Resources/LastKeyClothing.shader` y colores de vértice sobre las bases Quaternius CC0; no nueva geometría generada por IA.
- Animaciones originales: IK Humanoid, ocho estados, intro de despertar. Sin clips Mixamo descargados.
- Sonido original sintetizado en `GameAudio.cs`: ambiente, typing, error, combo, warning, NULL, ECHO, eliminación, ronda y victoria. Sin música de terceros ni costo.
- Material eléctrico URP referenciado en Resources para que el shader forme parte del build.
- Sillas con asiento ajustado y apoyo de pies: geometría original del proyecto.


## Realistic character evaluation — 2026-10-03 (not final assets)

Renderpeople free rigged/animated samples: https://renderpeople.com/free-3d-people/
License checked before download: https://renderpeople.com/general-terms-and-conditions/ (version 2024-12-13), sections 2.5, 4.1–4.3. Free license permits games and rendered images, but prohibits redistribution of individual source assets. Evaluation files remain outside Assets and Git. No Renderpeople source files may be added to public GitHub or the handoff ZIP. Compatibility and availability of four suitable distinct characters remain unverified. No character quality PASS is declared.

## Supplied music masters — preserved 2026-10-03

Iron Dominion.wav, Dead Current.wav, Mainframe.wav: supplied by Fernando for this integration. Copies in Assets/Audio/Music/Masters are byte-identical (SHA-256 in work/game/music-analysis.json). No music generation or paid service. No third-party music license inferred from file names. Full auditory verification remains pending.

## Reemplazo realista en evaluación local

| Identidad | Asset original Renderpeople | Ropa existente | Adaptación |
|---|---|---|---|
| CIPHER / Valeria Rojas | rp_carla_rigged_001_u3d | Chaqueta, pantalón y zapatos del modelo | Acento teal oscuro; altura 1.73 m |
| UPLINK / Ethan Cole | rp_eric_rigged_001_u3d | Camisa, chaleco, corbata y pantalón | Acento azul; altura 1.81 m |
| RELAY / Akira Mori | rp_manuel_animated_001_dancing | Camiseta, jeans y zapatillas | Acento verde apagado; altura 1.76 m; masculino |
| VECTOR / Noah Bennett | rp_nathan_animated_003_walking | Camiseta, jeans y zapatillas | Acento rojo oscuro; altura 1.85 m |

Fuente: https://renderpeople.com/free-3d-people/ . Descarga directa oficial FBX de paquetes rigged y animated. Licencia gratuita propietaria Renderpeople, https://renderpeople.com/general-terms-and-conditions/ . Los cuatro Humanoid importan como isValid/isHuman; aproximadamente 11.4k–13.1k vértices por personaje. Ropa incluida, no modelada desde cero. Se conservan piel y cabello; mask01 recolorea la prenda superior y mask02 pantalones. Texturas runtime máximo 2048.

Rig y skinning originales. El juego utiliza su animación procedural existente adaptada a estos rigs: sentado, typing normal/rápido, error, Danger, eliminación, victoria de ronda/final y despertar. No se afirma haber utilizado Mixamo ni mocap descargado para mecanografía. Los modelos animated se usan con sus rigs, sin reproducir baile/caminata en gameplay. No redistribuir fuentes, texturas, prefabs o derivados Renderpeople en GitHub público, bundle fuente ni ZIP de handoff. Assets/PrivateRenderpeople está excluido de Git; el checkpoint anterior permite restaurar el fallback Quaternius.
