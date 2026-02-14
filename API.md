# HealthRegenMod API Documentation

## Overview

The HealthRegenMod API provides a comprehensive interface for other mods to interact with and control health-related settings dynamically. All API methods are static and can be called without instantiating any classes.

## Namespace


HealthRegenMod.HealthRegenAPI


## API Reference

### Configuration Methods

#### SetMaxHealth

public static bool SetMaxHealth(int maxHealth)


Sets the maximum health value for the player.

**Parameters:**
- `maxHealth` (int): Maximum health value (1-10,000,000)

**Returns:**
- `bool`: `true` if successful, `false` if failed

**Example:**

// Set maximum health to 50,000
bool success = HealthRegenMod.HealthRegenAPI.SetMaxHealth(50000);
if (success)
{
    Debug.Log("Maximum health set to 50,000");
}


#### GetMaxHealth

public static int GetMaxHealth()


Gets the current maximum health setting.

**Returns:**
- `int`: Current maximum health value

**Example:**

int currentMaxHealth = HealthRegenMod.HealthRegenAPI.GetMaxHealth();
Debug.Log($"Current maximum health: {currentMaxHealth}");


#### SetGodMode

public static bool SetGodMode(bool enabled)


Enables or disables god mode (invincibility).

**Parameters:**
- `enabled` (bool): `true` to enable god mode, `false` to disable

**Returns:**
- `bool`: `true` if successful, `false` if failed

**Example:**

// Enable god mode
HealthRegenMod.HealthRegenAPI.SetGodMode(true);

// Disable god mode later
HealthRegenMod.HealthRegenAPI.SetGodMode(false);


#### GetGodMode

public static bool GetGodMode()


Gets the current god mode status.

**Returns:**
- `bool`: `true` if god mode is enabled, `false` if disabled

**Example:**

bool isGodModeEnabled = HealthRegenMod.HealthRegenAPI.GetGodMode();
if (isGodModeEnabled)
{
    Debug.Log("God mode is currently active");
}


#### SetHealthRegenRate

public static bool SetHealthRegenRate(int regenAmount)


Sets the health regeneration rate per frame.

**Parameters:**
- `regenAmount` (int): Health regenerated per frame (0-1000)

**Returns:**
- `bool`: `true` if successful, `false` if failed

**Example:**

// Set slow regeneration (10 health per frame)
HealthRegenMod.HealthRegenAPI.SetHealthRegenRate(10);

// Set fast regeneration (100 health per frame)
HealthRegenMod.HealthRegenAPI.SetHealthRegenRate(100);

// Disable regeneration
HealthRegenMod.HealthRegenAPI.SetHealthRegenRate(0);


#### GetHealthRegenRate

public static int GetHealthRegenRate()


Gets the current health regeneration rate.

**Returns:**
- `int`: Current health regeneration rate per frame

**Example:**

int regenRate = HealthRegenMod.HealthRegenAPI.GetHealthRegenRate();
Debug.Log($"Current regeneration rate: {regenRate} health per frame");


### Player Status Methods

#### SetPlayerHealth

public static bool SetPlayerHealth(int health)


Directly sets the player's current health value. This method bypasses normal health modification rules.

**Parameters:**
- `health` (int): Desired health value

**Returns:**
- `bool`: `true` if successful, `false` if failed (e.g., PlayerHealth instance not found)

**Example:**

// Set player health to 500
bool success = HealthRegenMod.HealthRegenAPI.SetPlayerHealth(500);
if (success)
{
    Debug.Log("Player health set to 500");
}


**Note:** This method requires an active PlayerHealth instance in the game scene.

#### GetPlayerHealth

public static int GetPlayerHealth()


Gets the player's current health value.

**Returns:**
- `int`: Current health value, or `-1` if failed to retrieve

**Example:**

int currentHealth = HealthRegenMod.HealthRegenAPI.GetPlayerHealth();
if (currentHealth >= 0)
{
    Debug.Log($"Player health: {currentHealth}");
}
else
{
    Debug.Log("Failed to get player health");
}


### System Methods

#### SaveConfig

public static bool SaveConfig()


Saves the current configuration to file.

**Returns:**
- `bool`: `true` if successful, `false` if failed

**Example:**

// After making multiple API changes, save the configuration
HealthRegenMod.HealthRegenAPI.SetMaxHealth(75000);
HealthRegenMod.HealthRegenAPI.SetGodMode(true);
HealthRegenMod.HealthRegenAPI.SaveConfig();


## Usage Examples

### Basic Integration

// In your mod's initialization
public void Initialize()
{
    // Check if HealthRegenMod is available
    try
    {
        // Set up health system for your mod
        HealthRegenMod.HealthRegenAPI.SetMaxHealth(20000);
        HealthRegenMod.HealthRegenAPI.SetHealthRegenRate(25);
        
        Debug.Log("HealthRegenMod integration successful");
    }
    catch (Exception ex)
    {
        Debug.LogWarning("HealthRegenMod not available: " + ex.Message);
    }
}


### Dynamic Health Adjustment

// Adjust health based on game events
public void OnDifficultyIncreased(int difficultyLevel)
{
    int baseHealth = 10000;
    int adjustedHealth = baseHealth / difficultyLevel;
    
    HealthRegenMod.HealthRegenAPI.SetMaxHealth(adjustedHealth);
    Debug.Log($"Adjusted max health to {adjustedHealth} for difficulty {difficultyLevel}");
}


### Conditional God Mode

// Enable god mode only in specific situations
public void OnBossFightStart()
{
    // Save current god mode state
    bool wasGodModeEnabled = HealthRegenMod.HealthRegenAPI.GetGodMode();
    
    // Disable god mode for boss fight
    HealthRegenMod.HealthRegenAPI.SetGodMode(false);
    
    // Restore after boss fight
    StartCoroutine(RestoreGodModeAfterBoss(wasGodModeEnabled));
}

private IEnumerator RestoreGodModeAfterBoss(bool originalState)
{
    yield return new WaitForSeconds(60); // Wait 1 minute
    
    HealthRegenMod.HealthRegenAPI.SetGodMode(originalState);
}


## Error Handling

All API methods include built-in error handling. Failed operations return `false` (for boolean methods) or special values (like `-1` for `GetPlayerHealth()`).

### Recommended Error Handling Pattern

public void SafeAPIUsage()
{
    try
    {
        if (!HealthRegenMod.HealthRegenAPI.SetMaxHealth(50000))
        {
            Debug.LogWarning("Failed to set max health");
        }
        
        int health = HealthRegenMod.HealthRegenAPI.GetPlayerHealth();
        if (health == -1)
        {
            Debug.LogWarning("Could not retrieve player health");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"API Error: {ex.Message}");
    }
}


## Thread Safety

The API methods are designed to be thread-safe and can be called from different threads. However, it's recommended to call them from the main Unity thread when interacting with game objects.

## Dependencies

The API has no external dependencies beyond the HealthRegenMod assembly itself. However, some methods (like `SetPlayerHealth` and `GetPlayerHealth`) require the game to be in a state where PlayerHealth instances exist.

## Version Compatibility

The API is stable and backward compatible. Future versions will maintain compatibility with existing method signatures.

| API Version | HealthRegenMod Version | Notes |
|-------------|----------------------|-------|
| v1.0        | 1.1.3+              | Initial API release |
| v1.0        | 1.1.0-1.1.1        | No API available |

## Best Practices

1. **Check Availability**: Wrap API calls in try-catch blocks
2. **Validate Inputs**: Validate parameters before passing to API
3. **Error Handling**: Always check return values
4. **Performance**: Avoid calling API methods every frame unless necessary
5. **Threading**: Call API from main Unity thread when possible

## Troubleshooting

### Common Issues

1. **API methods returning false**:
   - Check if HealthRegenMod is properly loaded
   - Verify parameter ranges (e.g., maxHealth 1-10,000,000)
   - Enable logging in HealthRegenMod configuration

2. **GetPlayerHealth returning -1**:
   - Game may not have initialized PlayerHealth yet
   - Wait until game is fully loaded
   - Check if player character exists in scene

3. **Methods throwing exceptions**:
   - HealthRegenMod.dll may not be referenced correctly
   - Version mismatch between mods
   - Check BepInEx console for detailed errors

### Debug Tips

Enable logging in HealthRegenMod configuration to see API call details:
1. Open REPOConfig menu in-game
2. Navigate to HealthRegenMod settings
3. Enable "EnableLogging"
4. Check BepInEx console for API call logs

## Support

For API-related issues or questions:
1. Check the [GitHub repository](https://github.com/hyy-PROG/HealthRegenMod/)
2. Review the README.md file
3. Contact the mod author through appropriate channels

---

*Last Updated: Version 1.1.3*
