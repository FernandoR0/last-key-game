# Character review 22

Revision 21 was rejected for eye alignment, wrist deformation and a held collar pose. This revision keeps the same licensed character, downloaded metallic collar and imported motion clips.

Changes:
- Parallel eye directions from a shared target, with limited yaw and pitch. Own-neck targets no longer cause independent eye convergence.
- Collar contact no longer caches a wrist rotation from the preceding lap pose.
- Wrists follow an upright palm direction at contact; elbows use a stable lower hint rather than following the source's raised-elbow passage.
- The collar motion samples source seconds 6.6–8.3 repeatedly, with smooth direction changes. It does not hold a terminal pose for the rest of the action.
- A separate Humanoid arm layer uses the clean 7.8 s source pose while the head/body continue the forceful motion. This avoids the source's raised-arm postures while retaining visible torso movement.
- Looking toward the own collar uses parallel downward eye aim rather than an unreachable near-neck convergence target.
- The review camera stays closer so face and hand defects can be inspected.

The Windows player is the evidence source. The review film remains 46 seconds, 1920×1080, 30 fps, silent. No voice, new UI, environment or multiplayer work is included. The WebGL game remains revision 21 until the character review is accepted and the new build is published; the revision 22 review page must not link to that older game as if it were revision 22.

Approval pending. Capture checkpoints and runtime logs accompany this review; they do not establish pixel-perfect clearance of all fingers in every frame.
