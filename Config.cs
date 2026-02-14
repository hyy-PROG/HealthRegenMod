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
        
        // 是否启用秒杀伤害免疫
        public static ConfigEntry<bool> EnableOneShotProtection { get; private set; }
        
        // 秒杀保护的最小保留生命值
        public static ConfigEntry<int> OneShotMinHealth { get; private set; }
        
        // 秒杀保护的最大单次伤害限制（超过此值的伤害会被削减）
        public static ConfigEntry<int> MaxSingleDamage { get; private set; }
        
        // 配置初始化
        public static void InitConfig(ConfigFile config)
        {
            try
            {
                EnableLogging = config.Bind(
                    "General",
                    "EnableLogging",
                    false,
                    "启用调试日志到控制台"
                );

                EnableGodMode = config.Bind(
                    "Gameplay",
                    "EnableGodMode",
                    true,
                    "启用上帝模式 - 生命值永远不会减少"
                );

                MaxHealth = config.Bind(
                    "Gameplay",
                    "MaxHealth",
                    10000,
                    new ConfigDescription(
                        "玩家的最大生命值",
                        new AcceptableValueRange<int>(1, 10000000)
                    )
                );

                HealthRegenPerFrame = config.Bind(
                    "Gameplay", 
                    "HealthRegenPerFrame",
                    0,
                    new ConfigDescription(
                        "每帧回复的生命值。设置为0时，上帝模式下会立即完全治愈",
                        new AcceptableValueRange<int>(0, 1000)
                    )
                );

                ShowDebugInfo = config.Bind(
                    "UI",
                    "ShowDebugInfo",
                    false,
                    "在屏幕上显示调试信息（如果游戏支持）"
                );
                
                EnableOneShotProtection = config.Bind(
                    "Protection",
                    "EnableOneShotProtection",
                    true,
                    "防止被一击秒杀，限制最大伤害"
                );
                
                OneShotMinHealth = config.Bind(
                    "Protection",
                    "OneShotMinHealth",
                    1,
                    new ConfigDescription(
                        "受到巨大伤害后保留的最小生命值（仅当秒杀保护启用时有效）",
                        new AcceptableValueRange<int>(1, 1000)
                    )
                );
                
                MaxSingleDamage = config.Bind(
                    "Protection",
                    "MaxSingleDamage",
                    500,
                    new ConfigDescription(
                        "单次受到的最大伤害。超过此值的伤害会被削减（0 = 无限制）",
                        new AcceptableValueRange<int>(0, 10000)
                    )
                );

                if (EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo("配置初始化成功");
                }
            }
            catch (Exception ex)
            {
                Plugin.ModLog.LogError($"配置初始化失败: {ex.Message}");
                throw;
            }
        }
    }
}