
### Available API Methods

- `SetMaxHealth(int maxHealth)` - Set maximum health (1-10,000,000)
- `GetMaxHealth()` - Get current maximum health
- `SetGodMode(bool enabled)` - Enable/disable god mode
- `GetGodMode()` - Get god mode status
- `SetHealthRegenRate(int regenAmount)` - Set health regeneration rate (0-1000)
- `GetHealthRegenRate()` - Get current regeneration rate
- `SetPlayerHealth(int health)` - Directly set player's current health
- `GetPlayerHealth()` - Get player's current health
- `SaveConfig()` - Save configuration to file

For complete API documentation, see [API.md](API.md)

## Compatibility

- **Game Version**: Tested with the latest version of the game
- **BepInEx Version**: Requires BepInEx 5.4.x or higher
- **REPOConfig**: Highly recommended for best experience (version 1.2.3+)
- **Other Mods**: Should be compatible with most other mods

## Version History

### v1.1.2 (Current)
- **Added comprehensive API** for other mods to dynamically control health settings
- Expanded maximum health range from 100-100,000 to 1-10,000,000
- Added GitHub URL to manifest.json
- Improved mod compatibility with API integration
- Fixed minor bugs in health regeneration calculation

### v1.1.0
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
- **API**: Fully documented public API for mod interoperability

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

5. **API not working**:
   - Ensure you're referencing the correct assembly
   - Check that HealthRegenMod is loaded before your mod
   - Enable logging to see API error messages

### Getting Help
1. Check the BepInEx console for error messages
2. Ensure you have the latest version of the mod
3. Verify all prerequisites are met
4. Check the GitHub repository for known issues

## Performance Notes
- The mod is very lightweight with minimal performance impact
- Logging may slightly impact performance when enabled
- High regen rates (1000+) may cause minor frame drops on lower-end systems
- API calls are optimized for performance

## Disclaimer

This mod is provided as-is. Use at your own risk. The author is not responsible for any issues that may arise from using this mod, including but not limited to game crashes, save file corruption, or online play restrictions.

## Credits

- **Author**: hyy-prog (lingzhu)
- **Special Thanks**: 
  - BepInEx team
  - Harmony library contributors
  - REPOConfig developers for configuration system

---


*Enjoy the mod! If you have any questions or feedback, please refer to the game's modding community for support.*
