# Character cinematic review 25 — collar skin attachment

Revision 24's separate rigid transform was rejected because the collar appeared static while the neck bent. Replaced that attachment with actual character skinning.

- Convert the existing downloaded metallic collar meshes to `SkinnedMeshRenderer` components sharing the ActorCore character's skeleton and inverse bind transforms.
- Transfer weights from the four nearest neck-surface vertices, blended by inverse distance and normalised to four influences. The character supplies 464 neck-band surface samples and 75 available skeleton bones. The band/liner use spatially varying neck weights. Small hardware modules use one shared weight set per module to preserve their shape.
- The collar centre and danger light anchor use the same skeletal deformation; neither is positioned by the previous station-up angle clamp. The old per-frame `SamplePose` override is now empty because skinning owns the attachment.
- Preserve the downloaded struggle, typing passage and revision 24 neck recovery. Per the user's latest request, disable the relaxed-finger overlay: the struggle now displays the original downloaded finger motion with no collar wrist/arm IK. Original retargeted motion is presented for review before further finger adaptation.
- Reuse the original metallic asset, with slightly larger 19 × 5.2 × 19 cm fitting bounds. Remove rear-facing triangles from the runtime mesh copy behind the neck to avoid protruding through the jacket; retain front and side hardware. The downloaded source geometry remains unchanged. No additional asset cost.
- Apply the user's latest direction: lower the bind centre to 8% along the neck-to-head segment instead of 46%. The cuff sits at the base of the neck below the chin, with the rear naturally occluded by the jacket. Rear coverage by clothing is intentional; it must not be reported as a defect merely because the back is hidden.

The attempted jacket-vertex adjustment was rejected after visual inspection because it deformed the chin; that code is removed. The final implementation does not edit the character's mesh. Do not deliver candidates 25A or 25C: 25A failed component conversion at startup, and 25C includes the rejected character deformation.

Verified final review capture: `LAST_KEY_CINEMATIC_25G`, 46 seconds, 1380 real Unity player frames, 1920×1080 / 30 fps, silent. Runtime log records successful neck-skin binding for all four characters and no blocking exceptions. Windows build succeeded (297219071 bytes), competition checks passed (53), and Unity MediaEncoder exported the movie (13168142 bytes). Inspected detailed crops at 18, 20, 22, 24, 26–34, 36, 39 and 43 seconds and full-resolution contact frames. Visual checks concern this cinematic and its camera views, not a physics collision guarantee for every possible future motion.

Published review is `/character-cinematic/?revision=25`. The browser playable WebGL remains revision 21; this page is the actual native Unity character review movie. User review of this correction remains pending.

Integración posterior (2026-10-22): el personaje y esta secuencia ya están incluidos en las builds principales Windows/WebGL de la revisión 30. La indicación histórica de WebGL revisión 21 anterior queda sustituida por UI_INTEGRATION_30.md.
