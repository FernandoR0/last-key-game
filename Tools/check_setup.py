import argparse
from pathlib import Path

MOTIONS = ["SittingIdleBreathing", "WakingSeated", "BeingStrangled", "Typing", "BeingElectrocuted", "SittingVictory", "SittingLookAround", "SittingDisbeliefForward", "SitToType", "TypeToSit", "LookingDown"]

def required_files():
    base = "Assets/PrivateActorCore/Male/Actor/party-m-0001/"
    return [base + "party-m-0001.fbx", base + "party-m-0001.fbm/Character_Pbr_Diffuse.png", base + "party-m-0001.fbm/Character_Pbr_Normal.jpg"] + ["Assets/PrivateMixamo/" + name + ".fbx" for name in MOTIONS]

def main():
    parser = argparse.ArgumentParser(description="Comprueba las fuentes visuales locales de LAST KEY sin modificarlas.")
    parser.add_argument("--project", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args()
    missing = [name for name in required_files() if not (args.project / name).is_file()]
    if missing:
        print("Faltan fuentes locales (no se distribuyen en el repositorio publico):")
        for name in missing:
            print("  " + name)
        print("Consulta SETUP_COLABORADORES.md. El codigo y las pruebas del nucleo pueden revisarse sin estas fuentes.")
        return 2
    print("OK: modelo, dos texturas y once animaciones presentes. Importa en Unity y ejecuta ProductionVisualSetup.Prepare.")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
