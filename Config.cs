using BepInEx;
using BepInEx.Configuration;
using System;

namespace HealthRegenMod
{
    public static class PluginConfig
    {
        // 是否启用调试日志
        public static ConfigEntry<bool> EnableLogging { get; private set; }
        
        // 是否启用上帝模式（锁血）
        public static ConfigEntry<bool> EnableGodMode { get; private set; }
        
        // 最大生命值
        public static ConfigEntry<int> MaxHealth { get; private set; }
        
        // 每帧回复的生命值
        public static ConfigEntry<int> HealthRegenPerFrame { get; private set; }
        
        // 是否显示调试信息
        public static ConfigEntry<bool> ShowDebugInfo { get; private set; }
        
        // 配置初始化
        public static void InitConfig(ConfigFile config)
        {
            try
            {
                EnableLogging = config.Bind(
                    "General",
                    "EnableLogging",
                    false,
                    "Enable logging to the console for debugging purposes"
                );

                EnableGodMode = config.Bind(
                    "Gameplay",
                    "EnableGodMode",
                    true,
                    "Enable god mode - health never decreases below max value"
                );

                MaxHealth = config.Bind(
                    "Gameplay",
                    "MaxHealth",
                    10000,
                    new ConfigDescription(
                        "Maximum health value for the player",
                        new AcceptableValueRange<int>(1, 10000000)
                    )
                );

                HealthRegenPerFrame = config.Bind(
                    "Gameplay", 
                    "HealthRegenPerFrame",
                    0,
                    new ConfigDescription(
                        "Amount of health regenerated per frame. Set to 0 for instant full heal when god mode is enabled",
                        new AcceptableValueRange<int>(0, 1000)
                    )
                );

                ShowDebugInfo = config.Bind(
                    "UI",
                    "ShowDebugInfo",
                    false,
                    "Show debug information on screen (if supported by the game)"
                );

                if (EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo("Configuration initialized successfully");
                }
            }
            catch (Exception ex)
            {
                Plugin.ModLog.LogError($"Failed to initialize configuration: {ex.Message}");
                throw;
            }
        }
    }
}