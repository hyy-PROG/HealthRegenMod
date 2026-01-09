using System;
using System.Reflection;

namespace HealthRegenMod
{
    /// <summary>
    /// 提供给其他模组调用的API接口
    /// </summary>
    public static class HealthRegenAPI
    {
        /// <summary>
        /// 设置最大生命值（允许其他模组动态调整）
        /// </summary>
        /// <param name="maxHealth">最大生命值 (1-10000000)</param>
        /// <returns>是否设置成功</returns>
        public static bool SetMaxHealth(int maxHealth)
        {
            try
            {
                if (maxHealth < 1 || maxHealth > 10000000)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogError($"SetMaxHealth: Value {maxHealth} out of range (1-10000000)");
                    }
                    return false;
                }

                PluginConfig.MaxHealth.Value = maxHealth;
                
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"API: MaxHealth set to {maxHealth}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"API: SetMaxHealth error: {ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 获取当前设置的最大生命值
        /// </summary>
        /// <returns>当前最大生命值</returns>
        public static int GetMaxHealth()
        {
            return PluginConfig.MaxHealth.Value;
        }

        /// <summary>
        /// 启用或禁用上帝模式
        /// </summary>
        /// <param name="enabled">是否启用上帝模式</param>
        /// <returns>是否设置成功</returns>
        public static bool SetGodMode(bool enabled)
        {
            try
            {
                PluginConfig.EnableGodMode.Value = enabled;
                
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"API: GodMode set to {enabled}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"API: SetGodMode error: {ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 获取上帝模式状态
        /// </summary>
        /// <returns>上帝模式是否启用</returns>
        public static bool GetGodMode()
        {
            return PluginConfig.EnableGodMode.Value;
        }

        /// <summary>
        /// 设置每帧生命恢复量
        /// </summary>
        /// <param name="regenAmount">每帧恢复量 (0-1000)</param>
        /// <returns>是否设置成功</returns>
        public static bool SetHealthRegenRate(int regenAmount)
        {
            try
            {
                if (regenAmount < 0 || regenAmount > 1000)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogError($"SetHealthRegenRate: Value {regenAmount} out of range (0-1000)");
                    }
                    return false;
                }

                PluginConfig.HealthRegenPerFrame.Value = regenAmount;
                
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"API: HealthRegenRate set to {regenAmount}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"API: SetHealthRegenRate error: {ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 获取当前每帧生命恢复量
        /// </summary>
        /// <returns>当前每帧恢复量</returns>
        public static int GetHealthRegenRate()
        {
            return PluginConfig.HealthRegenPerFrame.Value;
        }

        /// <summary>
        /// 直接设置玩家当前生命值（需要找到PlayerHealth实例）
        /// </summary>
        /// <param name="health">生命值</param>
        /// <returns>是否设置成功</returns>
        public static bool SetPlayerHealth(int health)
        {
            try
            {
                // 尝试通过反射找到PlayerHealth实例
                var playerHealthType = Type.GetType("PlayerHealth, Assembly-CSharp");
                if (playerHealthType == null)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogError("API: PlayerHealth type not found");
                    }
                    return false;
                }

                // 查找当前活动的PlayerHealth实例
                var playerInstance = UnityEngine.Object.FindObjectOfType(playerHealthType);
                if (playerInstance == null)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogError("API: PlayerHealth instance not found");
                    }
                    return false;
                }

                // 获取health字段
                var healthField = playerHealthType.GetField("health",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (healthField == null)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogError("API: health field not found");
                    }
                    return false;
                }

                // 设置生命值
                healthField.SetValue(playerInstance, health);
                
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"API: Player health set to {health}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"API: SetPlayerHealth error: {ex.Message}");
                }
                return false;
            }
        }

        /// <summary>
        /// 获取当前玩家生命值
        /// </summary>
        /// <returns>当前生命值，-1表示获取失败</returns>
        public static int GetPlayerHealth()
        {
            try
            {
                var playerHealthType = Type.GetType("PlayerHealth, Assembly-CSharp");
                if (playerHealthType == null) return -1;

                var playerInstance = UnityEngine.Object.FindObjectOfType(playerHealthType);
                if (playerInstance == null) return -1;

                var healthField = playerHealthType.GetField("health",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (healthField == null) return -1;

                return (int)healthField.GetValue(playerInstance);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 保存当前配置到文件
        /// </summary>
        /// <returns>是否保存成功</returns>
        public static bool SaveConfig()
        {
            try
            {
                // BepInEx Config会自动保存，但我们可以在需要时触发保存
                // 这里可以添加额外的保存逻辑
                
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo("API: Configuration saved");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"API: SaveConfig error: {ex.Message}");
                }
                return false;
            }
        }
    }
}