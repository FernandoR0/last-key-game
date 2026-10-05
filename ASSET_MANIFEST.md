# LAST KEY — assets visuales y de audio activos

Costo adicional: S/0. Las piezas existentes se adaptan, importan y recolorean; no se modeló ropa nueva ni se generaron assets con IA en esta fase.

| Recurso activo | Fuente original | Licencia | Uso real |
|---|---|---|---|
| Rain v3 | https://studio.blender.org/characters/rain/v3/ | CC BY | CIPHER: cuerpo, vestuario moderno, pelo y expresiones del rig original |
| Snow v4 | https://studio.blender.org/characters/snow/v4/ | CC BY | UPLINK, RELAY, VECTOR: ropa civil, anatomías adaptadas y expresiones |
| Cabello y barba Quaternius | https://quaternius.com/packs/universalbasecharacters.html | CC0 | Peinados y barba diferenciados; no prendas medievales |
| Universal Animation Library Standard | https://quaternius.com/packs/universalanimationlibrary.html | CC0 | Sentado, conversación/gesto corporal, reacción de impacto; versión Standard gratuita |
| CMU 79/85, typing on a laptop | https://mocap.cs.cmu.edu/search.php?subjectnumber=79 | Uso/modificación/redistribución permitidos: https://mocap.cs.cmu.edu/faqs.php | Base corporal de tecleo, retarget Humanoid. La pierna derecha de la captura carece de movimiento: los contactos y pies se resuelven con IK de la silla. No es mocap individual de cada dedo |
| amc2bvh | https://github.com/thcopeland/amc2bvh | MIT | Conversión ASF/AMC a BVH con fuente compilada localmente; herramienta de trabajo |
| Omie's Office Set | https://omies-assets.itch.io/omies-assets-office-set | CC0 | Escritorios y sillas importados. Ningún monitor ni PC se usa en las estaciones |
| Kenney Space Station Kit | https://kenney.nl/assets/space-station-kit | CC0 | Paredes, puerta y piso modulares; materiales adaptados |
| Ultimate Modular Sci-Fi Pack | https://quaternius.com/packs/ultimatemodularscifi.html | CC0 | Tuberías, columnas y estantes |
| Xeno Tracking Collar | https://3dassets.dev/assets/xeno-species-and-alien-creatures-xeno-tracking-collar-215adb28 | CC0 declarada por proveedor | Collar y reutilización de su banda para sujeciones. El proveedor identifica su asset como creado con herramientas de IA; aquí no se generó ni se pagó ninguno |
| URP Toon Lit Shader Example | https://github.com/ColinLeung-NiloCat/UnityURPToonLitShaderExample | MIT | Personajes estilo historieta; se conserva LICENSE. No se utiliza NiloToon de pago |
| Interface Sounds | https://kenney.nl/assets/interface-sounds | CC0 | Tecleo, error, combo, confirmaciones y selección |
| Sci-Fi Sounds | https://kenney.nl/assets/sci-fi-sounds | CC0 | NULL y descarga del collar |
| aargh3, JohnsonBrandEditing | https://freesound.org/people/JohnsonBrandEditing/sounds/173944/ | CC0 | Grito de eliminación. Solo aargh3: no se distribuyen los otros archivos con licencia CC BY del archivo original |
| Mainframe / Iron Dominion / Dead Current | Archivos WAV proporcionados por Fernando | Autorización del usuario para LAST KEY | Música original entregada, sin modificar sus archivos de origen |
| TextMeshPro / URP | Unity | Licencias de los paquetes | Renderizado y texto físico de la pantalla táctil |

Atribución requerida de Blender Studio: **Rain Rig (CC) Blender Foundation | studio.blender.org**; **Snow Rig (CC) Blender Foundation | studio.blender.org**. Adaptaciones: skeleton Humanoid ligero, conversión FBX, seis expresiones horneadas desde los controles originales, retirada de superficies interiores de prendas, máscaras de piel cubierta y recolores/variantes. Se conserva la licencia y crédito en Assets/LastKey/Visual/Characters/BlenderStudio/LICENSE.md.

Los assets antiguos Poly Haven, modelos privados y candidatos no seleccionados son referencias históricas; no representan los personajes ni las estaciones activos. Los archivos privados de Renderpeople no deben incluirse en un handoff público.

La pantalla táctil y las teclas se componen mediante elementos de renderizado de Unity: es la superficie física del mundo. La interfaz flotante, los menús, controles de interacción y multijugador quedan en la parte asignada a Fernando.

## Metallic collar animation pilot (revision 18)

# LAST KEY — adapted Space Collar

Original model: **the space collar** by **Infernalight**.
Source: https://sketchfab.com/3d-models/the-space-collar-daa389bf97f545309518a3c519a66b5d
License: Creative Commons Attribution 4.0 International (CC BY 4.0).
https://creativecommons.org/licenses/by/4.0/

Downloaded through Sketchfab's official download interface, without payment.
Adaptations: GLB-to-FBX conversion, graphite/steel materials, emissive Danger display, human-neck scale and bone attachment, removal of projecting rear tracker geometry for jacket clearance. Original geometry is reused; no new collar was modeled. The original author described it as a space dog collar; LAST KEY adapts the existing technological housing as a human competition restraint. This does not imply endorsement by the original author.

Current integration: one-character animation pilot. The previous playable build still uses the previous assets. No claims of final four-character integration.

Original GLB SHA256: 02f939ed883f06209a7c2cd9ae52549b6a1bd9f8b2d6e57dba77d8e4e36e8761

## Imported character and motions used in the integrated match
ActorCore Party-M-0001, official free character downloaded through Fernando’s account. Four temporary copies of the approved pilot; no four distinct final identities claimed. Source: https://actorcore.reallusion.com/3d-character/free?asset=party-m-0001
Mixamo: SittingIdleBreathing, WakingSeated, BeingStrangled, Typing, BeingElectrocuted, SittingVictory. Downloaded official free motions; private source files, embedded gameplay use. https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html
Character cinematic revision 23 additionally uses SittingLookAround (head channel), Looking Down (downloaded again from the official Mixamo library on 2026-10-05 at 60 fps), Sit To Type, and Type To Sit. No paid motion pack was obtained. Looking Down source is stored privately as `Assets/PrivateMixamo/LookingDown.fbx`; raw Adobe and ActorCore assets are not published in the repository.
No Fernando voice recording or rejected synthetic scream included in game builds. Cost: S/0.
