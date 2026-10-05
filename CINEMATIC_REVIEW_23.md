# Character cinematic review 23

Revision 22 was rejected for abrupt arm transitions, changed collar struggle and collar/neck intersection. This review addresses those specific defects; the character cinematic remains subject to visual review.

- Restored original downloaded `BeingStrangled` upper-body motion. Removed the held 7.8-second arm override and collar wrist/arm IK. Finger relaxation is the only pose overlay on that struggle, using the existing downloaded seated idle finger pose. Source seconds 5.8–10.92 are played with slower timing around the original 7.2–7.6 s hand lift; no body/arm poses are replaced. An FBX audit confirmed a source hand displacement of 0.153 m in one 30 fps step at 7.4 s. Slower source timing preserves its trajectory while avoiding an abrupt-looking lift.
- Blend the final skeletal pose after furniture adaptation, including skinned helper-bone rotations and positions. Also blend when switching clips within the same stage. Previously post-blend hand adaptation caused an instantaneous change at stage boundaries.
- Character presentation runs in `LateUpdate`, after Animator evaluation. Transition snapshots use the cached final displayed pose. The bone cache contains all 102 source skeleton transforms; the 75 skinned joints alone omit important unweighted arm controllers.
- Use downloaded `SitToType` and `TypeToSit` transitions, followed by the previously approved centered typing passage.
- Downloaded `Looking Down` at 60 fps from the official authenticated Mixamo library on 2026-10-05. The head channel is used for discovery and downward typing gaze. Existing `SittingLookAround` supplies the seated inspection head motion. No generated head-turn keyframes.
- Refit the existing metallic collar to the current neck-to-head axis after the final pose instead of keeping its initial rigid neck-bone offset.
- No new HUD, room, voice or multiplayer changes.

Source/license verification: [Adobe Mixamo FAQ](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html), [Looking Down library result](https://www.mixamo.com/#/?page=1&query=looking%20down), [ActorCore free library reviewed](https://actorcore.reallusion.com/3d-motion/free). No paid motion packs used. Raw motion/character source files remain private.

Validation: Windows Unity player capture at 1920×1080, 30 fps, 46 seconds. Review focuses on 22–25 seconds, 31–34 seconds and 35–39 seconds. Per-frame wrist displacement around those boundaries is logged by the capture tool. The review movie is distinct from the older published WebGL build.

Verified candidate: `LAST_KEY_CINEMATIC_23F`. Runtime capture completed with 1380 frames and no blocking exception. Recorded maximum wrist step within those specific windows: entry 0.003785 m, release 0.034024 m, typing 0.014730 m. These measurements describe the transition windows, not all fingers or the entire movie. Consecutive rendered frames of the original struggle were also inspected. Encoded by Unity MediaEncoder, 13,163,770 bytes. Windows build and competition checks passed. Review 23 is published at `/character-cinematic/?revision=23`; the WebGL playable remains revision 21. Character/user approval remains pending.

The local review server now supports HTTP byte ranges. The old standard Python server exposed a video seekable range of 0–0, causing section buttons to restart playback. Verified replacement returns HTTP 206 for `bytes=0-15`, and the browser now reports 0–46 s as seekable and reaches 35 s via the typing button.
