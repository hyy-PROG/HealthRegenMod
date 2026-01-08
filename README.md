# HealthRegenMod
A fully configurable health regeneration mod with god mode. Customize max health, regen rate, and toggle features via in-game menu.
# HealthRegenMod

A fully configurable BepInEx mod that provides health regeneration and god mode functionality with extensive customization options.

## Features

- **Configurable Maximum Health**: Adjust maximum health from 100 to 100000 via in-game menu
- **God Mode**: Toggle invincibility on/off
- **Health Regeneration**: Set custom health regeneration rate per frame
- **Integrated Configuration**: All settings accessible through REPOConfig in-game menu
- **Debug Options**: Enable/disable logging and debug info display
- **Hot-Reload**: Settings can be changed while game is running

## Installation

### Prerequisites
1. Make sure you have BepInEx 5.4.x or higher installed
2. Optional but recommended: Install [REPOConfig](https://thunderstore.io/c/REPO/p/nickklmao/REPOConfig/) for in-game configuration menu

### Installation Steps
1. Download the latest version of HealthRegenMod
2. Extract the `HealthRegenMod.dll` file from the downloaded package
3. Copy the DLL file to your game's `BepInEx/plugins` folder
4. Launch the game

## Configuration

### Using REPOConfig (Recommended)
1. In-game, press ESC to open the menu
2. Click on "Mods" button
3. Find and select "HealthRegenMod"
4. Adjust settings in real-time
5. Click "Save Changes" to apply

### Available Settings

#### General
- **EnableLogging**: Toggle debug logging to console
- **ShowDebugInfo**: Display debug information on screen

#### Gameplay
- **EnableGodMode**: Enable/disable invincibility (health never decreases)
- **MaxHealth**: Maximum health value (100-100000)
- **HealthRegenPerFrame**: Health regenerated per frame (0-1000). Set to 0 for instant full heal when god mode is enabled.

### Manual Configuration (Alternative)
If REPOConfig is not installed, you can edit the configuration file manually:
1. Navigate to `BepInEx/config` folder
2. Open `hyy.HealthRegenMod.cfg`
3. Modify the values
4. Restart the game

## Usage Modes

### God Mode (Recommended)
1. Set `EnableGodMode = true`
2. Health will automatically stay at maximum value
3. Perfect for invincibility gameplay

### Regeneration Mode
1. Set `EnableGodMode = false`
2. Set `HealthRegenPerFrame` to desired value (e.g., 10 for slow regen, 100 for fast regen)
3. Health will regenerate at the specified rate

### Custom Health Pool
1. Adjust `MaxHealth` to desired value
2. Higher values make the game easier, lower values increase challenge

## Compatibility

- **Game Version**: Tested with the latest version of the game
- **BepInEx Version**: Requires BepInEx 5.4.x or higher
- **REPOConfig**: Highly recommended for best experience (version 1.2.3+)
- **Other Mods**: Should be compatible with most other mods

## Version History

### v1.1.0 (Current)
- Added full configuration support
- Integrated with REPOConfig in-game menu
- Added health regeneration rate control
- Added debug logging toggle
- Added god mode toggle
- Added configurable max health

### v1.0.0
- Initial release
- Basic health regeneration functionality
- Fixed maximum health of 10000
- Basic god mode

## Technical Details

- **Mod Loader**: BepInEx 5
- **Configuration**: BepInEx Config system with REPOConfig integration
- **Harmony Patches**: Uses Harmony for runtime patching
- **Hot-Reload**: Configuration changes apply immediately
- **Logging**: Configurable logging to BepInEx console

## Troubleshooting

### Common Issues

1. **Mod not loading**:
   - Ensure BepInEx is properly installed
   - Check that the DLL is in `BepInEx/plugins` folder
   - Check BepInEx console for error messages

2. **Configuration not appearing in menu**:
   - Install REPOConfig mod
   - Ensure REPOConfig is up to date

3. **Settings not taking effect**:
   - Click "Save Changes" in REPOConfig menu
   - For manual configuration, restart the game

4. **Conflict with other health mods**:
   - Disable other health-related mods
   - Check load order in BepInEx

### Getting Help
1. Check the BepInEx console for error messages
2. Ensure you have the latest version of the mod
3. Verify all prerequisites are met

## Performance Notes
- The mod is very lightweight with minimal performance impact
- Logging may slightly impact performance when enabled
- High regen rates (1000+) may cause minor frame drops on lower-end systems

## Disclaimer

This mod is provided as-is. Use at your own risk. The author is not responsible for any issues that may arise from using this mod, including but not limited to game crashes, save file corruption, or online play restrictions.

## Credits

- **Author**: hyy-prog (lingzhu)
- **Special Thanks**: 
  - BepInEx team
  - Harmony library contributors
  - REPOConfig developers for configuration system
