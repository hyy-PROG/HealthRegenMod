using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace HealthRegenMod
{
    [BepInPlugin("hyy.HealthRegenMod", "HealthRegenMod", "1.0.0")]
    [BepInProcess("REPO.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource ModLog;
        private static Harmony _harmony;
        
        private void Awake()
        {
            ModLog = Logger;
            
            try
            {
                // 初始化配置
                PluginConfig.InitConfig(Config);
                
                // 应用Harmony补丁
                _harmony = new Harmony("hyy.HealthRegenMod");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());
                
                if (PluginConfig.EnableLogging.Value)
                {
                    ModLog.LogInfo("HealthRegenMod loaded successfully!");
                    ModLog.LogInfo($"Configuration loaded - MaxHealth: {PluginConfig.MaxHealth.Value}, GodMode: {PluginConfig.EnableGodMode.Value}, RegenPerFrame: {PluginConfig.HealthRegenPerFrame.Value}");
                    ModLog.LogInfo("author: lingzhu");
                    ModLog.LogInfo("version: 1.0.0");
                    ModLog.LogInfo("==========================================");
                }
            }
            catch (Exception ex)
            {
                ModLog.LogError($"Load error: {ex.Message}");
            }
        }
    }

    #region 健康修改部分

    [HarmonyPatch(typeof(PlayerHealth), "Start")]
    public class SetMaxHealthPatch
    {
        private static FieldInfo maxHealthField;

        [HarmonyPostfix]
        static void Postfix(PlayerHealth __instance)
        {
            try
            {
                if (!PluginConfig.EnableGodMode.Value) return;
                
                if (maxHealthField == null)
                {
                    maxHealthField = typeof(PlayerHealth).GetField("maxHealth",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                if (__instance != null && maxHealthField != null)
                {
                    int targetMaxHealth = PluginConfig.MaxHealth.Value;
                    int currentMaxHealth = (int)maxHealthField.GetValue(__instance);
                    
                    if (currentMaxHealth != targetMaxHealth)
                    {
                        maxHealthField.SetValue(__instance, targetMaxHealth);
                        
                        if (PluginConfig.EnableLogging.Value)
                        {
                            Plugin.ModLog.LogInfo($"Set max health to {targetMaxHealth}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"Error in SetMaxHealthPatch: {ex.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "Update")]
    public class KeepHealthAtMaxPatch
    {
        private static FieldInfo healthField;
        private static FieldInfo maxHealthField;

        [HarmonyPrefix]
        static void Prefix(PlayerHealth __instance)
        {
            try
            {
                if (healthField == null)
                {
                    healthField = typeof(PlayerHealth).GetField("health",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                if (maxHealthField == null)
                {
                    maxHealthField = typeof(PlayerHealth).GetField("maxHealth",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                if (__instance != null && healthField != null && maxHealthField != null)
                {
                    int currentHealth = (int)healthField.GetValue(__instance);
                    int currentMaxHealth = (int)maxHealthField.GetValue(__instance);
                    int targetMaxHealth = PluginConfig.MaxHealth.Value;
                    
                    // 确保最大生命值正确
                    if (currentMaxHealth != targetMaxHealth)
                    {
                        maxHealthField.SetValue(__instance, targetMaxHealth);
                        currentMaxHealth = targetMaxHealth;
                    }
                    
                    if (PluginConfig.EnableGodMode.Value)
                    {
                        // 上帝模式：立即恢复生命值
                        if (currentHealth < currentMaxHealth)
                        {
                            healthField.SetValue(__instance, currentMaxHealth);
                            
                            if (PluginConfig.EnableLogging.Value)
                            {
                                Plugin.ModLog.LogDebug($"God mode active: Health restored to {currentMaxHealth}");
                            }
                        }
                    }
                    else if (PluginConfig.HealthRegenPerFrame.Value > 0)
                    {
                        // 非上帝模式：按配置回复生命值
                        int regenAmount = PluginConfig.HealthRegenPerFrame.Value;
                        int newHealth = Mathf.Min(currentHealth + regenAmount, currentMaxHealth);
                        
                        if (newHealth != currentHealth)
                        {
                            healthField.SetValue(__instance, newHealth);
                            
                            if (PluginConfig.EnableLogging.Value)
                            {
                                Plugin.ModLog.LogDebug($"Health regen: {currentHealth} -> {newHealth} (+{regenAmount})");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"Error in KeepHealthAtMaxPatch: {ex.Message}");
                }
            }
        }
    }

    #endregion
}