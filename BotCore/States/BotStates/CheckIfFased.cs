using BotCore.Actions;
using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BotCore.States
{
    public class FasSpellConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[]
            {
                "beag fas nadur",
                "fas nadur", 
                "mor fas nadur",
                "ard fas nadur"
            });
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    [StateAttribute(Author: "ETDA", Desc: "Manages Fas spells for self and party members", DefaultPriority: 94)]
    public class CheckIfFased : GameState
    {
        private DateTime m_lastCheck = DateTime.MinValue;
        private DateTime m_lastCast = DateTime.MinValue;

        #region Configuration Properties

        private int m_checkInterval = 30000;
        [Description("Milliseconds between fas checks"), Category("Timing")]
        public int CheckInterval
        {
            get { return m_checkInterval; }
            set { m_checkInterval = value; }
        }

        private int m_castInterval = 5000;
        [Description("Minimum milliseconds between fas casts"), Category("Timing")]
        public int CastInterval
        {
            get { return m_castInterval; }
            set { m_castInterval = value; }
        }

        private bool m_castOnSelf = true;
        [Description("Cast fas on self when not active"), Category("Self Cast")]
        public bool CastOnSelf
        {
            get { return m_castOnSelf; }
            set { m_castOnSelf = value; }
        }

        private bool m_castOnParty = true;
        [Description("Cast fas on nearby party members"), Category("Party Cast")]
        public bool CastOnParty
        {
            get { return m_castOnParty; }
            set { m_castOnParty = value; }
        }

        private string m_targetPlayers = "";
        [Description("Comma-separated list of player names to cast fas on"), Category("Party Cast")]
        public string TargetPlayers
        {
            get { return m_targetPlayers; }
            set { m_targetPlayers = value; }
        }

        private int m_castRange = 10;
        [Description("Maximum range to cast fas on other players"), Category("Party Cast")]
        public int CastRange
        {
            get { return m_castRange; }
            set { m_castRange = value; }
        }

        private string m_fasSpell = "mor fas nadur";
        [Description("Fas spell to cast"), Category("Spell Selection")]
        [TypeConverter(typeof(FasSpellConverter))]
        public string FasSpell
        {
            get { return m_fasSpell; }
            set { m_fasSpell = value; }
        }

        #endregion

        #region Tracking Data

        // Track when we last cast fas on players for time-based detection
        private Dictionary<int, DateTime> m_lastFasTime = new Dictionary<int, DateTime>();

        #endregion

        #region GameState Implementation

        public override bool NeedToRun
        {
            get
            {
                if (!Enabled) return false;

                var now = DateTime.Now;
                var timeSinceLastCheck = now - m_lastCheck;

                // Check at configured interval
                if (timeSinceLastCheck.TotalMilliseconds < m_checkInterval)
                    return false;

                // Ensure minimum time between casts
                var timeSinceLastCast = now - m_lastCast;
                if (timeSinceLastCast.TotalMilliseconds < m_castInterval)
                    return false;

                // Check if self needs fas
                if (m_castOnSelf && !HasFas())
                {
                    return true;
                }

                // Check if party members need fas
                if (m_castOnParty || !string.IsNullOrEmpty(m_targetPlayers))
                {
                    var targetPlayers = GetTargetPlayers();
                    foreach (var player in targetPlayers)
                    {
                        if (!PlayerHasFas(player))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            set
            {

            }
        }

        public override int Priority { get; set; } = 94;

        public override void Run(TimeSpan Elapsed)
        {
            if (!Enabled || InTransition) return;

            InTransition = true;
            m_lastCheck = DateTime.Now;

            try
            {
                System.Diagnostics.Debug.WriteLine($"CheckIfFased: Running fas check cycle");

                // Cast on self first if needed
                if (m_castOnSelf && !HasFas())
                {
                    System.Diagnostics.Debug.WriteLine($"Self needs fas - casting {m_fasSpell}");
                    CastFasOnSelf();
                    Client.TransitionTo(this, Elapsed);
                    return;
                }

                // Cast on party members if needed
                if (m_castOnParty || !string.IsNullOrEmpty(m_targetPlayers))
                {
                    var targetPlayers = GetTargetPlayers();
                    System.Diagnostics.Debug.WriteLine($"Checking {targetPlayers.Count} potential fas targets");

                    foreach (var target in targetPlayers)
                    {
                        if (!PlayerHasFas(target))
                        {
                            string targetName = (target is Aisling aisling) ? aisling.Name : $"Serial_{target.Serial}";
                            System.Diagnostics.Debug.WriteLine($"Target {targetName} needs fas - casting");
                            CastFasOnTarget(target);
                            Client.TransitionTo(this, Elapsed);
                            return;
                        }
                    }
                }

                // Clean up old fas tracking entries (older than 10 minutes)
                CleanupFasTracking();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckIfFased error: {ex.Message}");
            }
            finally
            {
                Client.TransitionTo(this, Elapsed);
            }
        }

        #endregion

        #region Helper Methods

        private bool HasFas()
        {
            // Fas spell icon is 119
            bool hasFas = Client.SpellBar?.Contains(119) == true;
            System.Diagnostics.Debug.WriteLine($"Self fas status: {(hasFas ? "Active" : "Inactive")}");
            return hasFas;
        }

        private List<MapObject> GetTargetPlayers()
        {
            var targets = new List<MapObject>();

            try
            {
                // Method 1: Get nearby bot clients if party casting is enabled
                if (m_castOnParty && Client.OtherClients?.Count > 0)
                {
                    var nearbyBotClients = Client.OtherClients.Where(botClient =>
                        botClient.Attributes?.ServerPosition != null &&
                        Client.Attributes?.ServerPosition != null &&
                        Client.Attributes.ServerPosition.DistanceFrom(botClient.Attributes.ServerPosition) <= m_castRange
                    ).ToList();

                    foreach (var botClient in nearbyBotClients)
                    {
                        // Create MapObject representation of bot client
                        var mapObj = new MapObject()
                        {
                            Serial = botClient.Attributes.Serial,
                            ServerPosition = botClient.Attributes.ServerPosition,
                            Sprite = 1 // Ensure valid sprite for casting
                        };
                        targets.Add(mapObj);
                    }

                    System.Diagnostics.Debug.WriteLine($"Found {nearbyBotClients.Count} nearby bot clients for fas");
                }

                // Method 2: Get visual players by name or all nearby if party casting enabled
                if (!string.IsNullOrEmpty(m_targetPlayers) || m_castOnParty)
                {
                    var allPlayers = Client.ObjectSearcher?.RetreivePlayerTargets(obj => true) ?? new List<MapObject>();
                    
                    foreach (var player in allPlayers)
                    {
                        var aisling = player as Aisling;
                        if (aisling == null) continue;

                        // Skip self
                        if (player.Serial == Client.Attributes.Serial) continue;

                        // Check distance
                        if (Client.Attributes?.ServerPosition != null && 
                            player.ServerPosition != null &&
                            Client.Attributes.ServerPosition.DistanceFrom(player.ServerPosition) > m_castRange)
                            continue;

                        bool shouldTarget = false;

                        // Check if specifically named
                        if (!string.IsNullOrEmpty(m_targetPlayers))
                        {
                            var targetNames = m_targetPlayers.Split(',').Select(name => name.Trim().ToLower());
                            if (targetNames.Contains(aisling.Name.ToLower()))
                            {
                                shouldTarget = true;
                            }
                        }

                        // Check if party casting is enabled (cast on all nearby players)
                        if (m_castOnParty)
                        {
                            shouldTarget = true;
                        }

                        if (shouldTarget)
                        {
                            // Ensure valid sprite for casting
                            if (player.Sprite == 0 || player.Sprite == ushort.MaxValue)
                            {
                                player.Sprite = 1;
                            }
                            targets.Add(player);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting fas target players: {ex.Message}");
            }

            return targets;
        }

        private bool PlayerHasFas(MapObject player)
        {
            if (player == null) return true; // Assume they have it to be safe

            // Method 1: For bot clients, check their spell bar directly
            var botClient = Client.OtherClients?.FirstOrDefault(c => c.Attributes.Serial == player.Serial);
            if (botClient != null)
            {
                bool hasFas = botClient.SpellBar?.Contains(119) == true;
                System.Diagnostics.Debug.WriteLine($"Bot client {botClient.Attributes.PlayerName}: Fas = {hasFas}");
                return hasFas;
            }

            // Method 2: For non-bot players, use time-based tracking
            // We can't read their spell bars, so we track when we last cast fas
            if (m_lastFasTime.ContainsKey(player.Serial))
            {
                var timeSinceLastCast = DateTime.Now - m_lastFasTime[player.Serial];
                bool shouldBeActive = GetFasDuration(m_fasSpell, timeSinceLastCast);
                
                if (player is Aisling aisling)
                {
                    System.Diagnostics.Debug.WriteLine($"Player {aisling.Name}: Last fas {timeSinceLastCast.TotalSeconds:F0}s ago, Should be active: {shouldBeActive}");
                }
                
                return shouldBeActive;
            }

            // Never cast fas on this player, so they don't have it
            if (player is Aisling aisling2)
            {
                System.Diagnostics.Debug.WriteLine($"Player {aisling2.Name}: Never cast fas, needs buff");
            }
            return false;
        }

        private bool GetFasDuration(string spellName, TimeSpan timeSinceLastCast)
        {
            // Fas durations based on spell level
            double durationMinutes;
            switch (spellName.ToLower())
            {
                case "beag fas nadur":
                case "fas nadur":
                case "mor fas nadur":
                    durationMinutes = 7.5; // 7 minutes 30 seconds
                    break;
                case "ard fas nadur":
                    durationMinutes = 3.75; // 3 minutes 45 seconds
                    break;
                default:
                    durationMinutes = 7.5; // Default to longer duration
                    break;
            }

            return timeSinceLastCast.TotalMinutes < durationMinutes;
        }

        private void CleanupFasTracking()
        {
            var cutoffTime = DateTime.Now.AddMinutes(-10); // Remove entries older than 10 minutes
            var expiredKeys = m_lastFasTime.Where(kvp => kvp.Value < cutoffTime).Select(kvp => kvp.Key).ToList();
            
            foreach (var key in expiredKeys)
            {
                m_lastFasTime.Remove(key);
            }

            if (expiredKeys.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"Cleaned up {expiredKeys.Count} old fas tracking entries");
            }
        }

        private void CastFasOnSelf()
        {
            try
            {
                Client.Utilities.CastSpell(m_fasSpell, Client as Client);
                m_lastCast = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Cast {m_fasSpell} on self");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error casting fas on self: {ex.Message}");
            }
        }

        private void CastFasOnTarget(MapObject target)
        {
            try
            {
                string targetName = (target is Aisling aisling) ? aisling.Name : $"Serial_{target.Serial}";
                System.Diagnostics.Debug.WriteLine($"Attempting to cast {m_fasSpell} on {targetName} (Serial: {target.Serial}, Sprite: {target.Sprite})");
                
                // Ensure target has valid sprite for casting (fix for sprite check in GameUtilities.Cast)
                if (target.Sprite == 0 || target.Sprite == ushort.MaxValue)
                {
                    System.Diagnostics.Debug.WriteLine($"Target {targetName} has invalid sprite ({target.Sprite}), setting to default player sprite");
                    target.Sprite = 1; // Default player sprite
                }
                
                Client.Utilities.CastSpell(m_fasSpell, target);
                m_lastCast = DateTime.Now;

                // Track when we cast fas on this player for time-based detection
                m_lastFasTime[target.Serial] = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Successfully cast {m_fasSpell} on {targetName} (Serial: {target.Serial})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error casting fas on target: {ex.Message}");
            }
        }

        #endregion

        public override string ToString()
        {
            bool hasFas = HasFas();
            var targetCount = GetTargetPlayers().Count;
            return $"CheckIfFased - Self: {(hasFas ? "Active" : "Inactive")}, Targets: {targetCount}";
        }
    }
}