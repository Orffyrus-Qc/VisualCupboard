# Visual Cupboard

<img src="Images/icon.png" alt="Visual Cupboard icon" width="128" height="128">

![Visual Cupboard banner](Images/banner.jpg)

![Individual foundation claim](Images/foundation-claim.jpg)

Shows a visual claim sphere on **each connected building block**, not one circle around the tool cupboard.

In modern Rust, building privilege is projected from the building: every connected block extends the claim. This plugin draws a sphere on each of those blocks so the shape of the visual follows the base.

This is a rebuilt and maintained version of [Visual Cupboard Radius](https://umod.org/plugins/visual-cupboard-radius) by ColonBlow. Credit to the original author. The original only drew one sphere on the cupboard. This rebuild:

- Looks up the cupboard's building via `BuildingManager`
- Spawns a claim sphere on each connected decay/building entity
- Skips spheres whose volume is fully covered by neighbors (`IsRedundant`)

## Features

- Per-block visual claim spheres for the connected-building privilege system
- Draw spheres on cupboards you own
- Optionally show spheres to everyone nearby
- Admin command to show all nearby cupboards plus owner names
- Admin command to remove all visual spheres
- Spheres auto-remove after a configurable duration
- Permission-based access

## Permissions

This plugin uses the Oxide permission system.

```
oxide.grant user <name or steamid> visualcupboard.allowed
oxide.grant user <name or steamid> visualcupboard.admin
oxide.grant group default visualcupboard.allowed
```

- `visualcupboard.allowed` — use `/showsphere` and `/showsphereall`
- `visualcupboard.admin` — use `/showsphereadmin` and `/killsphere`, and also use the player commands

## Commands

Chat commands use a `/` prefix. Console commands use the same names without `/`.

- `/showsphere` — show building privilege spheres on your owned cupboards within range. Only you can see them
- `/showsphereall` — same as `/showsphere`, but other players can also see the spheres
- `/showsphereadmin` — admin: show spheres on all nearby cupboards, visible to everyone, and print cupboard owner names
- `/killsphere` — admin: destroy all visual spheres from this plugin

## Configuration

Default config (`oxide/config/VisualCupboard.json`):

```json
{
  "My Privilege Radius per Block is (16 is default)": 34.0,
  "Show Visuals On Cupboards Withing Range Of": 50.0,
  "Show Visuals For This Long": 60.0,
  "How Dark to make Visual Cupboard": 1
}
```

| Option | Default | Description |
| --- | --- | --- |
| My Privilege Radius per Block is (16 is default) | 34.0 | Sphere radius in meters |
| Show Visuals On Cupboards Withing Range Of | 50.0 | How far from the player to search for cupboards |
| Show Visuals For This Long | 60.0 | How long spheres stay visible, in seconds |
| How Dark to make Visual Cupboard | 1 | How many overlapping spheres to spawn (higher = darker / easier to see) |

## Installation

1. Copy `VisualCupboard.cs` into `oxide/plugins`
2. Reload or wait for Oxide to compile it
3. Grant `visualcupboard.allowed` (and `visualcupboard.admin` if needed)
4. Use `/showsphere`

Do not install this alongside the original Visual Cupboard Radius plugin. Both use the same class name (`VisualCupboard`) and will conflict.

## Notes

- Spheres are a visual aid. They do not change actual building privilege.
- Privilege follows the building, not a circle around the cupboard. This rebuild draws spheres on building entities and skips spheres fully covered by neighbors.
- Original plugin: [Visual Cupboard Radius by ColonBlow](https://umod.org/plugins/visual-cupboard-radius)

## License

MIT. See [LICENSE.md](LICENSE.md).
