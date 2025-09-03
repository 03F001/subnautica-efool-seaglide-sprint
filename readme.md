# efool's Seaglide Sprint Mod for Subnautica

Use sprint button while using the seaglide for a speed boost.

- Does not consume extra charge
- Speed boost configurable

# Requirements

- Subnautica Jun-2025 82304
- Tobey's BepInEx Pack v5
	- [Nexus Mods](https://www.nexusmods.com/subnautica/mods/1108)
	- [Github](https://github.com/toebeann/BepInEx.Subnautica)
- Nautilus v1.0.0-pre.42 or later
	- [Nexus Mods](https://www.nexusmods.com/subnautica/mods/1262)
	- [Github](https://github.com/SubnauticaModding/Nautilus)

# Installation

- Install Subnautica Jun-2025 82304
- Install Tobey's BepInEx Pack v5
- Install Nautilus v1.0.0-pre.42 or later
- Extract `efool-seaglide-sprint_#.#.#.zip` to `BepInEx/plugins`
	- `[game]/BepInEx/plugins/efool-custom-inventory/efool-custom-inventory.dll`
	- `[game]/BepInEx/plugins/efool-custom-inventory/presets.json`

Note: `[game]` is the directory containing `Subnautica.exe`

## Older Versions

- Subnautica Mar-2023 71288
	- Use efool-seaglide-sprint v0.0.2

# Console Commands

| Description                            | Command                    |
| -------------------------------------- | -------------------------- |
| Seaglide sprint boost flat addition    | seaglide_sprint_addition   |
| Seaglide sprint boost final multiplier | seaglide_sprint_multiplier |

# Known Issues

- Speed boost is uniform in all directions, unlike all other speeds
- Speed boost does not put additional drain on battery