# LAST KEY — piloto de animación dentro de la arena



Estado: prueba técnica de un personaje candidato. No es el juego actualizado ni la introducción final. No se sustituyeron todavía los cuatro personajes del juego publicado.

Captura nativa de Unity, 1920Ã—1080, 30 fps, 34 segundos, 1020 cuadros decodificados y verificados. Audio de prueba con efectos gratuitos ya descargados; sin narración NULL ni música. Mezcla de cues sobre la captura, no grabación de una partida.



## Qué se hizo

- Teclado integrado reducido a aproximadamente 52 cm de ancho, con cinco filas completas de letras, números, puntuación, tildes, Ã‘ y espacio. HUD del jugador sin rediseño.

- Texto de la mesa ampliado y con mayor superficie disponible.

- Mesa de esta prueba acercada y elevada para coincidir con la silla y este cuerpo; ese ajuste de mobiliario es específico del candidato, aún no global.

- Humanoid ActorCore Party-M-0001; materiales originales y collar gratuito existente, colocado siguiendo cuello/cabeza y conservando una orientación más estable.

- Motion clips oficiales de Mixamo: Waking (Waking Up While Sitting), Being Strangled (Getting Snuck Up On And Strangled), Typing, Being Electrocuted (Being Electrocuted Into Unconciousness), Sitting Victory, Sitting Idle (breathing).

- El despertar y gesto del cuello se aplican solo al torso sobre una pose sentada, evitando que las piernas atraviesen la mesa. El gesto del collar es una adaptación del clip de cuello con contacto IK: no un clip original de arrancar un collar.

- Dedos y muñecas conservan articulación de la animación descargada. IK adapta el alcance de las manos a la mesa, sin sustituirlo por una animación de dedos inventada.

- Descargas débiles/medias mezcladas sobre la animación de tecleo; la fuerte usa una mezcla completa del torso y grito. 40%, 70% y eliminación son eventos programados en esta prueba, no activados aquí por una partida.

- Párpados para despertar/parpadeo y mandíbula neutral corregida. Actuación facial completa y voz todavía pendientes.



## Secuencia de revisión

0–5 s despertar; 5–10 s collar; 10–14 s tecleo exterior; 14–18 s primera persona; 18–21 s descarga débil; 21–24 s media; 24–30 s fuerte; 30–34 s victoria.

Las fases están separadas para evaluar movimientos. Esta no es una cinemática terminada y sus transiciones/cámara son de prueba.



## Pendiente para la implementación completa

Cuatro variantes de personajes y ropa definitiva; introducción compartida con luces y otros participantes; pantalla/voz y reglas de NULL; diálogo del personaje; integración de eventos con el núcleo competitivo; actuación facial y control final de intersecciones. No se afirma que todo el primer prompt ya esté implementado.

El link del juego anterior continúa mostrando el build anterior. Este link nuevo muestra solamente la prueba de animaciones.



## Fuentes

https://www.mixamo.com/#/?page=1&query=waking

https://www.mixamo.com/#/?page=1&query=electrocuted

https://www.mixamo.com/#/?page=1&query=strangled

https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html

https://actorcore.reallusion.com/3d-character/free?asset=party-m-0001

https://www.reallusion.com/Content/EULA/EULA.htm

Sonidos Kenney CC0: https://kenney.nl/assets/interface-sounds y https://kenney.nl/assets/sci-fi-sounds

Grito CC0 aargh3: https://freesound.org/people/JohnsonBrandEditing/sounds/173944/

Fuentes FBX bajo sus respectivas licencias conservadas privadas. Paquete con renders, clips y documentación únicamente. Costo adicional: S/0.
