# Character cinematic review 24

Addresses the user's revision 23 feedback: collar intersection / insufficient size and a neck that stays tilted after the struggle.

- Reused the same downloaded metallic SpaceCollar model. The padding bounding dimensions now fit 0.18 × 0.052 × 0.18 m, previously 0.15 × 0.042 × 0.15 m. These are mesh fitting bounds, not a measurement of the interior opening.
- Its centre is a cached socket 46% along the bind neck-to-head segment, carried by the animated neck. Hardware no longer slides up and down with the changing head-to-neck vector. The rigid ring follows the socket while limiting tilt to 10° from station up, preventing the retargeted neck joint from lifting the front towards the mouth.
- Preserve the original downloaded struggle and its complete arm trajectory, slowed source timing and relaxed finger overlay. No collar arm IK or frozen arm pose was added.
- Following the struggle, return both neck twist joints to their rest rotations through the existing 1.8-second final-pose transition. Waiting looks forward. Typing looks down the keyboard centre with a balanced head orientation; the previous looking-down mocap's sideways lean no longer drives that pose.

Validation: candidate `LAST_KEY_CINEMATIC_24C`, actual Windows Unity player, 1380 captured frames / 46 s / 1920×1080 / 30 fps. Inspected a full-sequence sheet at one-second intervals and detailed crops at 18, 20, 22, 24, 26–34, 36, 39 and 43 seconds, including full-resolution contact and typing frames. This is visual inspection of those views, not a universal mesh collision guarantee. Two initial fitting candidates were rejected before publication because they lifted the cuff too far behind the ear or under the chin.

Windows build completed: `LAST_KEY_WINDOWS_BUILD_SUCCESS 297213335`. Core competition checks passed (`LAST_KEY_POLISH_CORE_PASS 53`). Runtime capture completed without blocking exceptions. Transition-window maximum hand steps remain entry 0.003785 m, release 0.034024 m and typing 0.014730 m. Unity MediaEncoder exported `cinematic_character.mp4`, 13,153,859 bytes.

Review published at `/character-cinematic/?revision=24`. This silent character review replaces the review movie only; the published playable WebGL is still the older revision 21. Voice, HUD, room, competition rules and the final academic video are outside this correction. User approval of the new fit remains pending.
