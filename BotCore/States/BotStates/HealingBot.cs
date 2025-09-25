using BotCore.States.BotStates;
using BotCore.Shared;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BotCore.States
{
    /// <summary>
    /// Comprehensive healing bot state that can heal self, party members, and specific named players
    /// Supports multiple healing spells with automatic spell selection based on health percentage
    /// </summary>
    [StateAttribute(Author: "ETDA", Desc: "Advanced healing bot for self, party members, and named players", DefaultPriority: 90)]
    public class HealingBot : GameState
    {
        #region Private Fields
        private bool m_enabled = true;
        private bool m_healSelf = true;
        private bool m_healParty = true;
        private string m_targetPlayers = "";
        private int m_castRange = 12;
        private double m_healThreshold = 0.8; // Heal when below 80% health
        private double m_criticalThreshold = 0.5; // Use stronger heals when below 50%
        private string m_primaryHealSpell = "ioc";
        private string m_criticalHealSpell = "mor ioc";
        private string m_emergencyHealSpell = "ard ioc";
        private int m_healCooldown = 3000; // 3 seconds between heals on same target
        private bool m_prioritizeCritical = true;
        private bool m_useSmartSpellSelection = true;

        // Tracking dictionaries for cooldowns and health monitoring
        private Dictionary<uint, DateTime> m_lastHealTime = new Dictionary<uint, DateTime>();
        private Dictionary<uint, double> m_lastKnownHealth = new Dictionary<uint, double>();
        #endregion

        #region Properties
        [Description("Enable/disable healing bot"), Category("General")]
        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        [Description("Heal self when health drops below threshold"), Category("Self Healing")]
        public bool HealSelf
        {
            get { return m_healSelf; }
            set { m_healSelf = value; }
        }

        [Description("Heal nearby party members"), Category("Party Healing")]
        public bool HealParty
        {
            get { return m_healParty; }
            set { m_healParty = value; }
        }

        [Description("Comma-separated list of player names to heal (non-party members)"), Category("Target Healing")]
        public string TargetPlayers
        {
            get { return m_targetPlayers; }
            set { m_targetPlayers = value; }
        }

        [Description("Maximum range to cast healing spells"), Category("General")]
        public int CastRange
        {
            get { return m_castRange; }
            set { m_castRange = value; }
        }

        [Description("Health percentage threshold to start healing (0.0 - 1.0)"), Category("Healing Logic")]
        public double HealThreshold
        {
            get { return m_healThreshold; }
            set { m_healThreshold = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        [Description("Health percentage for critical healing with stronger spells (0.0 - 1.0)"), Category("Healing Logic")]
        public double CriticalThreshold
        {
            get { return m_criticalThreshold; }
            set { m_criticalThreshold = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        [Description("Primary healing spell for normal healing"), Category("Spells")]
        public string PrimaryHealSpell
        {
            get { return m_primaryHealSpell; }
            set { m_primaryHealSpell = value; }
        }

        [Description("Healing spell for critical health situations"), Category("Spells")]
        public string CriticalHealSpell
        {
            get { return m_criticalHealSpell; }
            set { m_criticalHealSpell = value; }
        }

        [Description("Emergency healing spell for very low health"), Category("Spells")]
        public string EmergencyHealSpell
        {
            get { return m_emergencyHealSpell; }
            set { m_emergencyHealSpell = value; }
        }

        [Description("Cooldown between heals on same target (milliseconds)"), Category("Healing Logic")]
        public int HealCooldown
        {
            get { return m_healCooldown; }
            set { m_healCooldown = Math.Max(1000, value); }
        }

        [Description("Prioritize critical health targets over others"), Category("Healing Logic")]
        public bool PrioritizeCritical
        {
            get { return m_prioritizeCritical; }
            set { m_prioritizeCritical = value; }
        }

        [Description("Automatically select best healing spell based on health and mana"), Category("Spells")]
        public bool UseSmartSpellSelection
        {
            get { return m_useSmartSpellSelection; }
            set { m_useSmartSpellSelection = value; }
        }
        #endregion

        #region GameState Overrides
        public override bool NeedToRun
        {
            get
            {
                if (!m_enabled || Client?.Attributes == null)
                    return false;

                // Check if we need to heal self
                if (m_healSelf && NeedsSelfHealing())
                {
                    return true;
                }

                // Check if party members or named targets need healing
                if ((m_healParty || !string.IsNullOrEmpty(m_targetPlayers)) && HasTargetsNeedingHealing())
                {
                    return true;
                }

                return false;
            }
            set { }
        }

        public override int Priority { get; set; } = 90;

        public override void Run(TimeSpan elapsed)
        {
            if (!m_enabled || InTransition || Client?.Attributes == null)
                return;

            try
            {
                InTransition = true;

                // Clean up old tracking data (remove entries older than 5 minutes)
                CleanupTrackingData();

                // Handle self healing first if critical
                if (m_healSelf && NeedsSelfHealing())
                {
                    var selfHealthPercent = GetHealthPercent(Client.Attributes.CurrentHP(), Client.Attributes.MaximumHP());
                    if (selfHealthPercent <= m_criticalThreshold)
                    {
                        PerformSelfHealing();
                        Client.TransitionTo(this, elapsed);
                        return;
                    }
                }

                // Get all healing targets and prioritize them
                var healingTargets = GetHealingTargets();
                if (healingTargets.Count > 0)
                {
                    // Sort by priority: critical health first, then by health percentage
                    var prioritizedTargets = healingTargets
                        .Where(t => CanHealTarget(t.Key))
                        .OrderBy(t => t.Value) // Lower health percentage = higher priority
                        .ToList();

                    if (prioritizedTargets.Count > 0)
                    {
                        var target = prioritizedTargets.First();
                        PerformHealingOnTarget(target.Key, target.Value);
                    }
                }

                // Handle self healing if not critical but still needed
                if (m_healSelf && NeedsSelfHealing())
                {
                    PerformSelfHealing();
                }

                Client.TransitionTo(this, elapsed);
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"HealingBot error: {ex.Message}");
                Client.TransitionTo(this, elapsed);
            }
            finally
            {
                InTransition = false;
            }
        }
        #endregion

        #region Helper Methods
        private bool NeedsSelfHealing()
        {
            if (Client?.Attributes == null) return false;

            var currentHp = Client.Attributes.CurrentHP();
            var maxHp = Client.Attributes.MaximumHP();
            
            if (maxHp <= 0) return false;

            var healthPercent = GetHealthPercent(currentHp, maxHp);
            return healthPercent < m_healThreshold;
        }

        private bool HasTargetsNeedingHealing()
        {
            var targets = GetHealingTargets();
            return targets.Count > 0;
        }

        private Dictionary<MapObject, double> GetHealingTargets()
        {
            var targets = new Dictionary<MapObject, double>();

            try
            {
                // Method 1: Get nearby bot clients if party healing is enabled
                if (m_healParty && Client.OtherClients?.Count > 0)
                {
                    foreach (var botClient in Client.OtherClients)
                    {
                        if (botClient?.Attributes?.ServerPosition == null) continue;

                        var distance = Client.Attributes.ServerPosition.DistanceFrom(botClient.Attributes.ServerPosition);
                        if (distance <= m_castRange)
                        {
                            var healthPercent = GetHealthPercent(botClient.Attributes.CurrentHP(), botClient.Attributes.MaximumHP());
                            if (healthPercent < m_healThreshold && CanHealTarget((uint)botClient.Attributes.Serial))
                            {
                                // Create a virtual Aisling object for the bot client
                                var botAsTarget = new Aisling(botClient.Attributes.PlayerName)
                                {
                                    Serial = botClient.Attributes.Serial,
                                    ServerPosition = botClient.Attributes.ServerPosition,
                                    Type = MapObjectType.Aisling
                                };
                                targets[botAsTarget] = healthPercent;
                            }
                        }
                    }
                }

                // Method 2: Get visual players by name or all nearby if party healing enabled
                if (!string.IsNullOrEmpty(m_targetPlayers) || m_healParty)
                {
                    var allPlayers = Client.ObjectSearcher?.RetreivePlayerTargets(obj => true) ?? new List<MapObject>();

                    foreach (var player in allPlayers)
                    {
                        if (player?.ServerPosition == null) continue;

                        var distance = Client.Attributes.ServerPosition.DistanceFrom(player.ServerPosition);
                        if (distance > m_castRange) continue;

                        var aisling = player as Aisling;
                        if (aisling == null) continue;

                        bool shouldHeal = false;

                        // Check if this is a named target
                        if (!string.IsNullOrEmpty(m_targetPlayers))
                        {
                            var targetNames = m_targetPlayers.Split(',').Select(name => name.Trim().ToLower());
                            if (targetNames.Contains(aisling.Name.ToLower()))
                            {
                                shouldHeal = true;
                            }
                        }

                        // Check if party healing is enabled (heal all nearby players)
                        if (m_healParty && !shouldHeal)
                        {
                            shouldHeal = true;
                        }

                        if (shouldHeal)
                        {
                            // For visual players, we can't directly access their health
                            // So we estimate based on visual cues or use a default threshold
                            double estimatedHealth = EstimatePlayerHealth(player);
                            
                            if (estimatedHealth < m_healThreshold && CanHealTarget((uint)player.Serial))
                            {
                                targets[player] = estimatedHealth;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"HealingBot GetHealingTargets error: {ex.Message}");
            }

            return targets;
        }

        private double EstimatePlayerHealth(MapObject player)
        {
            // Since we can't directly read other players' health from memory,
            // we use visual cues and tracking to estimate health
            
            if (m_lastKnownHealth.ContainsKey((uint)player.Serial))
            {
                return m_lastKnownHealth[(uint)player.Serial];
            }

            // Default assumption: if we can see them and they're not moving erratically,
            // assume they might need healing (conservative approach)
            return m_healThreshold - 0.1; // Slightly below threshold to trigger healing
        }

        private bool CanHealTarget(uint serial)
        {
            if (!m_lastHealTime.ContainsKey(serial))
                return true;

            var timeSinceLastHeal = DateTime.Now - m_lastHealTime[serial];
            return timeSinceLastHeal.TotalMilliseconds >= m_healCooldown;
        }

        private bool CanHealTarget(MapObject target)
        {
            return CanHealTarget((uint)target.Serial);
        }

        private void PerformSelfHealing()
        {
            if (Client?.Attributes == null) return;

            var healthPercent = GetHealthPercent(Client.Attributes.CurrentHP(), Client.Attributes.MaximumHP());
            var healSpell = SelectOptimalHealSpell(healthPercent);

            DebugLogger.Log($"HealingBot: Self-healing with {healSpell}");
            
            Client.Utilities.CastSpell(healSpell, Client as Client);
            m_lastHealTime[(uint)Client.Attributes.Serial] = DateTime.Now;
        }

        private void PerformHealingOnTarget(MapObject target, double healthPercent)
        {
            if (target == null) return;

            var healSpell = SelectOptimalHealSpell(healthPercent);
            string targetName = (target is Aisling aisling) ? aisling.Name : $"Serial_{target.Serial}";

            DebugLogger.Log($"HealingBot: Healing {targetName} with {healSpell}");
            
            Client.Utilities.CastSpell(healSpell, target);
            m_lastHealTime[(uint)target.Serial] = DateTime.Now;
            m_lastKnownHealth[(uint)target.Serial] = healthPercent;
        }

        private string SelectOptimalHealSpell(double healthPercent)
        {
            if (!m_useSmartSpellSelection)
                return m_primaryHealSpell;

            // Emergency healing for very low health
            if (healthPercent <= 0.25)
            {
                if (HasSpell(m_emergencyHealSpell) && HasSufficientMana(m_emergencyHealSpell))
                    return m_emergencyHealSpell;
            }

            // Critical healing for low health
            if (healthPercent <= m_criticalThreshold)
            {
                if (HasSpell(m_criticalHealSpell) && HasSufficientMana(m_criticalHealSpell))
                    return m_criticalHealSpell;
            }

            // Default to primary healing spell
            if (HasSpell(m_primaryHealSpell) && HasSufficientMana(m_primaryHealSpell))
                return m_primaryHealSpell;

            // Fallback to any available healing spell
            var availableHeals = new[] { "ard ioc", "mor ioc", "ioc", "beag ioc" };
            foreach (var spell in availableHeals)
            {
                if (HasSpell(spell) && HasSufficientMana(spell))
                    return spell;
            }

            return m_primaryHealSpell; // Last resort
        }

        private bool HasSpell(string spellName)
        {
            return Client?.GameMagic?.Spells?.Any(s => 
                s != null && 
                Client.Utilities.CleanSpellName(s.Name).Equals(spellName, StringComparison.OrdinalIgnoreCase)) == true;
        }

        private bool HasSufficientMana(string spellName)
        {
            if (Client?.Attributes == null || !Collections.BaseSpells.ContainsKey(spellName.ToLower()))
                return false;

            var spellInfo = Collections.BaseSpells[spellName.ToLower()];
            var currentMp = Client.Attributes.CurrentMP();
            
            return currentMp >= spellInfo.Mana;
        }

        private double GetHealthPercent(int currentHp, int maxHp)
        {
            if (maxHp <= 0) return 1.0;
            return (double)currentHp / maxHp;
        }

        private void CleanupTrackingData()
        {
            var cutoff = DateTime.Now.AddMinutes(-5);
            var keysToRemove = m_lastHealTime.Where(kvp => kvp.Value < cutoff).Select(kvp => kvp.Key).ToList();
            
            foreach (var key in keysToRemove)
            {
                m_lastHealTime.Remove(key);
                m_lastKnownHealth.Remove(key);
            }
        }

        public override string ToString()
        {
            var status = m_enabled ? "Active" : "Inactive";
            var selfHeal = m_healSelf ? "Self" : "";
            var partyHeal = m_healParty ? "Party" : "";
            var namedCount = !string.IsNullOrEmpty(m_targetPlayers) ? m_targetPlayers.Split(',').Length : 0;
            var namedHeal = namedCount > 0 ? $"Named({namedCount})" : "";

            var targets = new[] { selfHeal, partyHeal, namedHeal }.Where(s => !string.IsNullOrEmpty(s));
            var targetInfo = targets.Any() ? string.Join(", ", targets) : "None";

            return $"HealingBot - {status} | Targets: {targetInfo} | Threshold: {m_healThreshold:P0}";
        }
        #endregion
    }
}