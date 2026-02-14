using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace HealthRegenMod
{
    [BepInPlugin("hyy.HealthRegenMod", "HealthRegenMod", "1.2.0")]
    [BepInProcess("REPO.exe")]
    [BepInDependency("nickklmao-REPOConfig")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource ModLog;
        private static Harmony _harmony;
        private static GameObject _managerObject;

        private void Awake()
        {
            ModLog = Logger;
            _managerObject = new GameObject("HealthRegenMod_Manager");
            UnityEngine.Object.DontDestroyOnLoad(_managerObject);

            try
            {
                // 初始化配置
                PluginConfig.InitConfig(Config);

                // 应用Harmony补丁
                _harmony = new Harmony("hyy.HealthRegenMod");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());

                // 添加更新组件
                _managerObject.AddComponent<HealthRegenUpdater>();

                if (PluginConfig.EnableLogging.Value)
                {
                    ModLog.LogInfo("HealthRegenMod 1.2.0 loaded successfully!");
                    ModLog.LogInfo($"Game Version: 0.3.2 Compatibility");
                    ModLog.LogInfo($"Configuration loaded - MaxHealth: {PluginConfig.MaxHealth.Value}, GodMode: {PluginConfig.EnableGodMode.Value}");
                    ModLog.LogInfo("author: lingzhu");
                    ModLog.LogInfo("==========================================");
                }
            }
            catch (Exception ex)
            {
                ModLog.LogError($"Load error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void OnDestroy()
        {
            if (_harmony != null)
            {
                _harmony.UnpatchSelf();
            }

            if (_managerObject != null)
            {
                UnityEngine.Object.Destroy(_managerObject);
            }
        }
    }

    /// <summary>
    /// 更新器组件，负责每帧更新
    /// </summary>
    internal class HealthRegenUpdater : MonoBehaviour
    {
        private PlayerHealth _cachedPlayerHealth;
        private FieldInfo _photonViewField;
        private FieldInfo _healthField;
        private FieldInfo _maxHealthField;
        private FieldInfo _isMenuAvatarField;

        private void Start()
        {
            // 使用反射获取字段信息
            _photonViewField = typeof(PlayerHealth).GetField("photonView",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _healthField = typeof(PlayerHealth).GetField("health",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _maxHealthField = typeof(PlayerHealth).GetField("maxHealth",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _isMenuAvatarField = typeof(PlayerHealth).GetField("isMenuAvatar",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }

        private void Update()
        {
            try
            {
                if (_cachedPlayerHealth == null || !_cachedPlayerHealth.gameObject.activeInHierarchy)
                {
                    _cachedPlayerHealth = UnityEngine.Object.FindObjectOfType<PlayerHealth>();
                    if (_cachedPlayerHealth == null) return;

                    // 检查是否是菜单角色
                    bool isMenuAvatar = false;
                    if (_isMenuAvatarField != null)
                    {
                        isMenuAvatar = (bool)_isMenuAvatarField.GetValue(_cachedPlayerHealth);
                    }

                    if (isMenuAvatar)
                    {
                        _cachedPlayerHealth = null;
                        return;
                    }

                    // 检查是否是本地玩家（多人游戏）
                    if (_photonViewField != null)
                    {
                        var photonView = _photonViewField.GetValue(_cachedPlayerHealth);
                        // 使用反射检查 IsMine 属性
                        if (photonView != null)
                        {
                            var isMineProperty = photonView.GetType().GetProperty("IsMine");
                            if (isMineProperty != null)
                            {
                                bool isMine = (bool)isMineProperty.GetValue(photonView);
                                if (!isMine)
                                {
                                    _cachedPlayerHealth = null;
                                    return;
                                }
                            }
                        }
                    }
                }

                ApplyModEffects(_cachedPlayerHealth);
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"Updater error: {ex.Message}");
                }
            }
        }

        private void ApplyModEffects(PlayerHealth playerHealth)
        {
            if (playerHealth == null) return;

            try
            {
                // 获取当前生命值和最大生命值
                int currentHealth = 100;
                int currentMaxHealth = 100;

                if (_healthField != null)
                {
                    currentHealth = (int)_healthField.GetValue(playerHealth);
                }

                if (_maxHealthField != null)
                {
                    currentMaxHealth = (int)_maxHealthField.GetValue(playerHealth);
                }

                int targetMaxHealth = PluginConfig.MaxHealth.Value;

                // 应用最大生命值
                if (currentMaxHealth != targetMaxHealth)
                {
                    _maxHealthField?.SetValue(playerHealth, targetMaxHealth);
                    currentMaxHealth = targetMaxHealth;

                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogDebug($"Set max health to {targetMaxHealth}");
                    }
                }

                // 上帝模式
                if (PluginConfig.EnableGodMode.Value)
                {
                    if (currentHealth < currentMaxHealth)
                    {
                        _healthField?.SetValue(playerHealth, currentMaxHealth);

                        if (PluginConfig.EnableLogging.Value)
                        {
                            Plugin.ModLog.LogDebug($"God mode: Health restored to {currentMaxHealth}");
                        }
                    }
                }
                // 生命恢复
                else if (PluginConfig.HealthRegenPerFrame.Value > 0)
                {
                    int regenAmount = PluginConfig.HealthRegenPerFrame.Value;
                    int newHealth = Mathf.Min(currentHealth + regenAmount, currentMaxHealth);

                    if (newHealth != currentHealth)
                    {
                        _healthField?.SetValue(playerHealth, newHealth);

                        if (PluginConfig.EnableLogging.Value)
                        {
                            Plugin.ModLog.LogDebug($"Health regen: {currentHealth} -> {newHealth} (+{regenAmount})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"ApplyModEffects error: {ex.Message}");
                }
            }
        }
    }

    #region Harmony补丁

    [HarmonyPatch(typeof(PlayerHealth), "Hurt")]
    public class PlayerHealth_Hurt_Patch
    {
        private static FieldInfo _photonViewField;
        private static FieldInfo _healthField;

        [HarmonyPrefix]
        static bool Prefix(PlayerHealth __instance, ref int damage, bool savingGrace, int enemyIndex = -1)
        {
            try
            {
                // 初始化字段信息（如果未初始化）
                if (_photonViewField == null)
                {
                    _photonViewField = typeof(PlayerHealth).GetField("photonView",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                if (_healthField == null)
                {
                    _healthField = typeof(PlayerHealth).GetField("health",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                // 检查是否是本地玩家
                if (_photonViewField != null)
                {
                    var photonView = _photonViewField.GetValue(__instance);
                    if (photonView != null)
                    {
                        var isMineProperty = photonView.GetType().GetProperty("IsMine");
                        if (isMineProperty != null)
                        {
                            bool isMine = (bool)isMineProperty.GetValue(photonView);
                            if (!isMine) return true;
                        }
                    }
                }

                // 上帝模式
                if (PluginConfig.EnableGodMode.Value)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogInfo($"God mode: Blocked {damage} damage");
                    }
                    return false; // 跳过原方法
                }

                // 秒杀保护
                if (PluginConfig.EnableOneShotProtection.Value)
                {
                    int currentHealth = 100;
                    if (_healthField != null)
                    {
                        currentHealth = (int)_healthField.GetValue(__instance);
                    }

                    int originalDamage = damage;
                    int maxDamage = PluginConfig.MaxSingleDamage.Value;
                    int minHealth = PluginConfig.OneShotMinHealth.Value;

                    // 应用最大伤害限制
                    if (maxDamage > 0 && damage > maxDamage)
                    {
                        damage = maxDamage;

                        if (PluginConfig.EnableLogging.Value)
                        {
                            Plugin.ModLog.LogInfo($"One-shot protection: Damage reduced from {originalDamage} to {maxDamage}");
                        }
                    }

                    // 计算受到伤害后的生命值
                    int healthAfterDamage = currentHealth - damage;

                    // 防止生命值低于最小值
                    if (healthAfterDamage < minHealth && healthAfterDamage > 0)
                    {
                        damage = currentHealth - minHealth;

                        if (PluginConfig.EnableLogging.Value)
                        {
                            Plugin.ModLog.LogInfo($"One-shot protection: Adjusted damage to keep minimum {minHealth} health");
                        }
                    }
                    // 防止死亡
                    else if (healthAfterDamage <= 0)
                    {
                        damage = 0;

                        if (PluginConfig.EnableLogging.Value)
                        {
                            Plugin.ModLog.LogInfo($"One-shot protection: Prevented fatal damage");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"Error in Hurt patch: {ex.Message}");
                }
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(PlayerHealth), "Heal")]
    public class PlayerHealth_Heal_Patch
    {
        private static FieldInfo _photonViewField;

        [HarmonyPrefix]
        static void Prefix(PlayerHealth __instance, int healAmount, bool effect = true)
        {
            try
            {
                // 初始化字段信息
                if (_photonViewField == null)
                {
                    _photonViewField = typeof(PlayerHealth).GetField("photonView",
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                }

                // 检查是否是本地玩家
                if (_photonViewField != null)
                {
                    var photonView = _photonViewField.GetValue(__instance);
                    if (photonView != null)
                    {
                        var isMineProperty = photonView.GetType().GetProperty("IsMine");
                        if (isMineProperty != null)
                        {
                            bool isMine = (bool)isMineProperty.GetValue(photonView);
                            if (!isMine) return;
                        }
                    }
                }

                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogDebug($"Healing: {healAmount} health");
                }
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"Error in Heal patch: {ex.Message}");
                }
            }
        }
    }

    #endregion
}