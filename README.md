# HealthRegenMod - Health Regeneration and One-Shot Protection Mod

## Overview

HealthRegenMod is a BepInEx mod for the REPO game that provides powerful health management features, including unlimited health, health regeneration, and one-shot kill protection.

## Features

### 🛡️ Core Features
- **God Mode**: Complete immunity to all damage, health never decreases
- **Max Health Adjustment**: Customizable maximum health (1-10,000,000)
- **Health Regeneration**: Automatic health regeneration per frame with customizable speed

### 🚫 One-Shot Protection
- **One-Shot Kill Immunity**: Prevent instant death from single attacks
- **Damage Limiting**: Cap maximum damage from single hits
- **Minimum Health Protection**: Maintain at least specified health when taking fatal damage

### ⚙️ Configuration Options
- Full in-game configuration menu
- Adjustable parameters for all features
- Support for hot-reloading configurations

### 🔌 API Interface
- Complete API for other mods
- Support for dynamic adjustment of all settings
- Thread-safe API calls

## Installation

### Prerequisites
1. BepInEx 5.4.x or higher installed
2. Game version: REPO

### Installation Steps
1. Download the latest HealthRegenMod.dll
2. Copy HealthRegenMod.dll to `REPO/BepInEx/plugins/` directory
3. Launch the game

## Configuration

### Configuration File Location
`REPO/BepInEx/config/hyy.HealthRegenMod.cfg`

### Configuration Details

#### General Settings
- **EnableLogging**: Enable debug logging (Default: false)
- **ShowDebugInfo**: Show debug information on screen (Default: false)

#### Gameplay
- **EnableGodMode**: Enable God Mode (Default: true)
- **MaxHealth**: Maximum health value (Range: 1-10,000,000, Default: 10000)
- **HealthRegenPerFrame**: Health regenerated per frame (Range: 0-1000, Default: 0)

#### Damage Protection
- **EnableOneShotProtection**: Enable one-shot kill protection (Default: true)
- **OneShotMinHealth**: Minimum health to remain after massive damage (Range: 1-1000, Default: 1)
- **MaxSingleDamage**: Maximum damage that can be taken in a single hit (Range: 0-10000, 0=unlimited, Default: 500)

## API Documentation

### Basic Usage
```csharp
using HealthRegenMod;

// Set maximum health
HealthRegenAPI.SetMaxHealth(50000);

// Enable God Mode
HealthRegenAPI.SetGodMode(true);

// Set health regeneration rate
HealthRegenAPI.SetHealthRegenRate(10);
```

### Advanced Features
```csharp
// Enable one-shot protection
HealthRegenAPI.SetOneShotProtection(true);

// Set minimum health to remain
HealthRegenAPI.SetOneShotMinHealth(100);

// Set maximum single damage
HealthRegenAPI.SetMaxSingleDamage(1000);

// Get current player health
int health = HealthRegenAPI.GetPlayerHealth();

// Directly set player health
HealthRegenAPI.SetPlayerHealth(1000);
```

### Error Handling
```csharp
try
{
    if (!HealthRegenAPI.SetMaxHealth(50000))
    {
        // Handle failure
    }
}
catch (Exception ex)
{
    // Handle exception
}
```

## In-Game Usage

### Enabling/Disabling Features
1. Launch the game
2. Press F1 to open BepInEx configuration menu
3. Find "HealthRegenMod" configuration
4. Adjust desired settings
5. Settings are saved automatically

### Recommended Configurations

#### Beginner Friendly
```
EnableGodMode = true
HealthRegenPerFrame = 0
EnableOneShotProtection = true
MaxSingleDamage = 1000
```

#### Challenge Mode
```
EnableGodMode = false
HealthRegenPerFrame = 5
EnableOneShotProtection = true
MaxSingleDamage = 500
OneShotMinHealth = 10
```

#### Hardcore Mode
```
EnableGodMode = false
HealthRegenPerFrame = 0
EnableOneShotProtection = false
```

## Frequently Asked Questions

### Q: The mod is not working, what should I do?
A: Check the following:
1. BepInEx is correctly installed
2. HealthRegenMod.dll is in the correct directory
3. Game version matches
4. Check BepInEx console logs

### Q: How to confirm the mod is loaded?
A: Check the BepInEx console when launching the game, you should see similar messages:
```
[Info] HealthRegenMod loaded successfully!
```

### Q: Do I need to restart the game after changing configurations?
A: No, configurations take effect immediately. However, some settings may require reloading the game scene.

### Q: Is it compatible with other mods?
A: This mod uses Harmony for patching and is theoretically compatible with most mods. If there are conflicts, try adjusting the load order.

## Troubleshooting

### Log Analysis
Enable `EnableLogging` to view detailed logs for troubleshooting:

1. Enable logging
2. Check BepInEx console
3. Fix issues based on error messages

### Common Errors
- **"PlayerHealth type not found"**: Game version mismatch
- **"Configuration failed to load"**: Corrupted configuration file
- **"Harmony patches failed"**: Conflict with other mods

## Developer Information

### Technical Details
- **Development Framework**: BepInEx 5
- **Patching System**: Harmony 2.x
- **Target Framework**: .NET Framework 4.8
- **Compilation Requirements**: Visual Studio 2019+

### Project Structure
```
HealthRegenMod/
├── Plugin.cs          # Main plugin logic
├── Config.cs          # Configuration management
├── API.cs             # API interface
├── AssemblyInfo.cs    # Assembly information
└── HealthRegenMod.csproj  # Project file
```

### Contribution Guidelines
1. Fork the project
2. Create a feature branch
3. Commit changes
4. Open a Pull Request

## Version History

### v1.1.4 (Current)
- Added: One-shot kill protection functionality
- Added: Damage limiting system
- Improved: Configuration menu structure
- Optimized: API interface

### v1.1.3
- Added: Complete API interface
- Improved: Configuration validation
- Fixed: Health regeneration logic

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) file for details.

## Support & Feedback

### Issue Reporting
1. Check [Known Issues](#frequently-asked-questions)
2. Report on GitHub Issues
3. Provide detailed error logs

### Feature Requests
Feature requests and suggestions are welcome.

### Contact
- GitHub: [hyy-PROG/HealthRegenMod](https://github.com/hyy-PROG/HealthRegenMod/)
- Author: lingzhu

## Disclaimer

This mod is for learning and entertainment purposes only. Using this mod may affect game balance. Please use it in single-player games only. Using in multiplayer games may violate the game's terms of service, use with caution.

---

**Note**: This mod is still under development, features may change. Check for updates regularly for the latest features and fixes.