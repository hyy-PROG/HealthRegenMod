using System;
using System.Reflection;
using UnityEngine;

namespace HealthRegenMod
{
    /// <summary>
    /// 健康管理器接口 - 供其他模组使用
    /// </summary>
    public interface IHealthManager
    {
        // 配置属性
        int MaxHealth { get; set; }
        bool GodMode { get; set; }
        int HealthRegenRate { get; set; }
        bool OneShotProtection { get; set; }
        int OneShotMinHealth { get; set; }
        int MaxSingleDamage { get; set; }

        // 玩家状态
        int CurrentHealth { get; }
        bool IsGodModeActive { get; }
        bool IsOneShotProtectionActive { get; }

        // 操作方法
        bool SetPlayerHealth(int health);
        bool HealPlayer(int amount);
        bool DamagePlayer(int damage);
        void ApplyInvincibility(float duration);

        // 事件
        event Action<int> OnHealthChanged;
        event Action<bool> OnGodModeToggled;
        event Action<int> OnDamageTaken;
        event Action<int> OnHealed;
    }

    /// <summary>
    /// 健康管理器实现类
    /// </summary>
    public class HealthManager : IHealthManager
    {
        private static HealthManager _instance;
        private PlayerHealth _cachedPlayerHealth;
        private FieldInfo _healthField;
        private FieldInfo _maxHealthField;
        private FieldInfo _photonViewField;
        private MethodInfo _healMethod;
        private MethodInfo _hurtMethod;
        private MethodInfo _invincibleSetMethod;

        public static HealthManager Instance => _instance ??= new HealthManager();

        // 事件
        public event Action<int> OnHealthChanged;
        public event Action<bool> OnGodModeToggled;
        public event Action<int> OnDamageTaken;
        public event Action<int> OnHealed;

        private HealthManager()
        {
            // 使用反射初始化字段信息
            _healthField = typeof(PlayerHealth).GetField("health",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _maxHealthField = typeof(PlayerHealth).GetField("maxHealth",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            _photonViewField = typeof(PlayerHealth).GetField("photonView",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // 获取方法信息
            _healMethod = typeof(PlayerHealth).GetMethod("Heal",
                BindingFlags.Instance | BindingFlags.Public);
            _hurtMethod = typeof(PlayerHealth).GetMethod("Hurt",
                BindingFlags.Instance | BindingFlags.Public);
            _invincibleSetMethod = typeof(PlayerHealth).GetMethod("InvincibleSet",
                BindingFlags.Instance | BindingFlags.Public);
        }

        // 配置属性
        public int MaxHealth
        {
            get => PluginConfig.MaxHealth.Value;
            set
            {
                if (value >= 1 && value <= 10000000)
                {
                    PluginConfig.MaxHealth.Value = value;
                    ApplyToPlayer();
                }
            }
        }

        public bool GodMode
        {
            get => PluginConfig.EnableGodMode.Value;
            set
            {
                PluginConfig.EnableGodMode.Value = value;
                OnGodModeToggled?.Invoke(value);
                ApplyToPlayer();
            }
        }

        public int HealthRegenRate
        {
            get => PluginConfig.HealthRegenPerFrame.Value;
            set
            {
                if (value >= 0 && value <= 1000)
                {
                    PluginConfig.HealthRegenPerFrame.Value = value;
                }
            }
        }

        public bool OneShotProtection
        {
            get => PluginConfig.EnableOneShotProtection.Value;
            set => PluginConfig.EnableOneShotProtection.Value = value;
        }

        public int OneShotMinHealth
        {
            get => PluginConfig.OneShotMinHealth.Value;
            set
            {
                if (value >= 1 && value <= 1000)
                {
                    PluginConfig.OneShotMinHealth.Value = value;
                }
            }
        }

        public int MaxSingleDamage
        {
            get => PluginConfig.MaxSingleDamage.Value;
            set
            {
                if (value >= 0 && value <= 10000)
                {
                    PluginConfig.MaxSingleDamage.Value = value;
                }
            }
        }

        // 只读属性
        public int CurrentHealth
        {
            get
            {
                var playerHealth = GetPlayerHealthInstance();
                if (playerHealth != null && _healthField != null)
                {
                    return (int)_healthField.GetValue(playerHealth);
                }
                return -1;
            }
        }

        public bool IsGodModeActive => GodMode && PluginConfig.EnableGodMode.Value;
        public bool IsOneShotProtectionActive => OneShotProtection && PluginConfig.EnableOneShotProtection.Value;

        private PlayerHealth GetPlayerHealthInstance()
        {
            if (_cachedPlayerHealth == null || !_cachedPlayerHealth.gameObject.activeInHierarchy)
            {
                _cachedPlayerHealth = UnityEngine.Object.FindObjectOfType<PlayerHealth>();
            }
            return _cachedPlayerHealth;
        }

        private void ApplyToPlayer()
        {
            var playerHealth = GetPlayerHealthInstance();
            if (playerHealth != null && _maxHealthField != null)
            {
                // 应用最大生命值
                int currentMaxHealth = (int)_maxHealthField.GetValue(playerHealth);
                if (currentMaxHealth != MaxHealth)
                {
                    _maxHealthField.SetValue(playerHealth, MaxHealth);
                }

                // 如果上帝模式启用，确保生命值为最大值
                if (GodMode && _healthField != null)
                {
                    int currentHealth = (int)_healthField.GetValue(playerHealth);
                    if (currentHealth < MaxHealth)
                    {
                        _healthField.SetValue(playerHealth, MaxHealth);
                        OnHealthChanged?.Invoke(MaxHealth);
                    }
                }
            }
        }

        public bool SetPlayerHealth(int health)
        {
            try
            {
                var playerHealth = GetPlayerHealthInstance();
                if (playerHealth == null || _healthField == null) return false;

                int newHealth = Math.Max(1, Math.Min(health, MaxHealth));
                int oldHealth = (int)_healthField.GetValue(playerHealth);

                _healthField.SetValue(playerHealth, newHealth);

                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"HealthManager: Health set from {oldHealth} to {newHealth}");
                }

                OnHealthChanged?.Invoke(newHealth);
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"HealthManager: SetPlayerHealth error: {ex.Message}");
                }
                return false;
            }
        }

        public bool HealPlayer(int amount)
        {
            try
            {
                if (amount <= 0) return false;

                var playerHealth = GetPlayerHealthInstance();
                if (playerHealth == null || _healMethod == null) return false;

                // 检查是否是本地玩家
                if (_photonViewField != null)
                {
                    var photonView = _photonViewField.GetValue(playerHealth);
                    if (photonView != null)
                    {
                        var isMineProperty = photonView.GetType().GetProperty("IsMine");
                        if (isMineProperty != null)
                        {
                            bool isMine = (bool)isMineProperty.GetValue(photonView);
                            if (!isMine) return false;
                        }
                    }
                }

                // 使用反射调用Heal方法
                _healMethod.Invoke(playerHealth, new object[] { amount, true });

                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"HealthManager: Healed {amount} health");
                }

                OnHealed?.Invoke(amount);
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"HealthManager: HealPlayer error: {ex.Message}");
                }
                return false;
            }
        }

        public bool DamagePlayer(int damage)
        {
            try
            {
                if (damage <= 0) return false;

                var playerHealth = GetPlayerHealthInstance();
                if (playerHealth == null || _hurtMethod == null) return false;

                // 检查是否是本地玩家
                if (_photonViewField != null)
                {
                    var photonView = _photonViewField.GetValue(playerHealth);
                    if (photonView != null)
                    {
                        var isMineProperty = photonView.GetType().GetProperty("IsMine");
                        if (isMineProperty != null)
                        {
                            bool isMine = (bool)isMineProperty.GetValue(photonView);
                            if (!isMine) return false;
                        }
                    }
                }

                if (GodMode)
                {
                    if (PluginConfig.EnableLogging.Value)
                    {
                        Plugin.ModLog.LogInfo($"HealthManager: God mode active, ignoring {damage} damage");
                    }
                    return true; // 上帝模式下伤害被忽略
                }

                // 应用秒杀保护
                int actualDamage = ApplyOneShotProtection(damage);

                // 使用反射调用Hurt方法
                _hurtMethod.Invoke(playerHealth, new object[] { actualDamage, false, -1 });

                OnDamageTaken?.Invoke(actualDamage);
                return true;
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"HealthManager: DamagePlayer error: {ex.Message}");
                }
                return false;
            }
        }

        public void ApplyInvincibility(float duration)
        {
            try
            {
                var playerHealth = GetPlayerHealthInstance();
                if (playerHealth == null || _invincibleSetMethod == null) return;

                // 使用反射调用InvincibleSet方法
                _invincibleSetMethod.Invoke(playerHealth, new object[] { duration });

                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogInfo($"HealthManager: Applied invincibility for {duration}s");
                }
            }
            catch (Exception ex)
            {
                if (PluginConfig.EnableLogging.Value)
                {
                    Plugin.ModLog.LogError($"HealthManager: ApplyInvincibility error: {ex.Message}");
                }
            }
        }

        private int ApplyOneShotProtection(int incomingDamage)
        {
            if (!OneShotProtection) return incomingDamage;

            int currentHealth = CurrentHealth;
            int maxDamage = MaxSingleDamage;
            int minHealth = OneShotMinHealth;

            // 应用最大单次伤害限制
            if (maxDamage > 0 && incomingDamage > maxDamage)
            {
                incomingDamage = maxDamage;
            }

            // 计算受到伤害后的生命值
            int healthAfterDamage = currentHealth - incomingDamage;

            // 如果会导致生命值低于最小值
            if (healthAfterDamage < minHealth && healthAfterDamage > 0)
            {
                incomingDamage = currentHealth - minHealth;
            }
            // 如果会导致死亡，完全阻止
            else if (healthAfterDamage <= 0)
            {
                incomingDamage = 0;
            }

            return incomingDamage;
        }
    }

    /// <summary>
    /// 提供给其他模组调用的API接口
    /// </summary>
    public static class HealthRegenAPI
    {
        /// <summary>
        /// 获取健康管理器实例
        /// </summary>
        public static IHealthManager GetHealthManager()
        {
            return HealthManager.Instance;
        }

        /// <summary>
        /// 检查模组是否已加载并准备好
        /// </summary>
        public static bool IsModReady()
        {
            return HealthManager.Instance != null;
        }

        /// <summary>
        /// 旧API兼容性方法
        /// </summary>
        [Obsolete("Use GetHealthManager().MaxHealth instead")]
        public static bool SetMaxHealth(int maxHealth)
        {
            var manager = GetHealthManager();
            if (manager == null) return false;

            manager.MaxHealth = maxHealth;
            return true;
        }

        [Obsolete("Use GetHealthManager().GodMode instead")]
        public static bool SetGodMode(bool enabled)
        {
            var manager = GetHealthManager();
            if (manager == null) return false;

            manager.GodMode = enabled;
            return true;
        }

        [Obsolete("Use GetHealthManager().HealthRegenRate instead")]
        public static bool SetHealthRegenRate(int regenAmount)
        {
            var manager = GetHealthManager();
            if (manager == null) return false;

            manager.HealthRegenRate = regenAmount;
            return true;
        }
    }
}