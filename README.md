# Visual Cupboard

Shows a visual sphere of building privilege radius on nearby tool cupboards.

This is a rebuilt and maintained version of [Visual Cupboard Radius](https://umod.org/plugins/visual-cupboard-radius) by ColonBlow. Credit to the original author. This rebuild attaches spheres to building entities, skips fully overlapping spheres.

## Features

- Draw a visual sphere around building privilege for cupboards you own
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

- `visualcupboard.allowed` â€” use `/showsphere` and `/showsphereall`
- `visualcupboard.admin` â€” use `/showsphereadmin` and `/killsphere`, and also use the player commands

*Optional tool -  https://codefling.com/plugins/permissions-manager

## Commands

Chat commands use a `/` prefix. Console commands use the same names without `/`.

- `/showsphere` â€” show building privilege spheres on your owned cupboards within range. Only you can see them
- `/showsphereall` â€” same as `/showsphere`, but other players can also see the spheres
- `/showsphereadmin` â€” admin: show spheres on all nearby cupboards, visible to everyone, and print cupboard owner names
- `/killsphere` â€” admin: destroy all visual spheres from this plugin

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
- Privilege in current Rust follows the building, not a perfect circle around the cupboard. This rebuild draws spheres on building entities instead of only the cupboard itself, which is closer to real coverage than the original static cupboard sphere.
- Original plugin: [Visual Cupboard Radius by ColonBlow](https://umod.org/plugins/visual-cupboard-radius)

## License

MIT. See [LICENSE.md](LICENSE.md).