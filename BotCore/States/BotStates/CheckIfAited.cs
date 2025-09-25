using BotCore.Actions;
using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BotCore.States
{
    public class AiteSpellConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[]
            {
                "beag naomh aite",
                "naomh aite", 
                "mor naomh aite",
                "ard naomh aite"
            });
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }
    [StateAttribute(Author: "ETDA", Desc: "Manages Aite spells for self and party members", DefaultPriority: 95)]
    public class CheckIfAited : GameState
    {
        private DateTime m_lastCheck = DateTime.MinValue;
        private DateTime m_lastCast = DateTime.MinValue;

        #region Configuration Properties

        private int m_checkInterval = 30000;
        [Description("Milliseconds between aite checks"), Category("Timing")]
        public int CheckInterval
        {
            get { return m_checkInterval; }
            set { m_checkInterval = value; }
        }

        private int m_castInterval = 5000;
        [Description("Minimum milliseconds between aite casts"), Category("Timing")]
        public int CastInterval
        {
            get { return m_castInterval; }
            set { m_castInterval = value; }
        }



        private bool m_castOnSelf = true;
        [Description("Cast aite on self when not active"), Category("Self Cast")]
        public bool CastOnSelf
        {
            get { return m_castOnSelf; }
            set { m_castOnSelf = value; }
        }

        private bool m_castOnParty = true;
        [Description("Cast aite on nearby party members"), Category("Party Cast")]
        public bool CastOnParty
        {
            get { return m_castOnParty; }
            set { m_castOnParty = value; }
        }

        private string m_targetPlayers = "";
        [Description("Comma-separated list of player names to cast aite on"), Category("Party Cast")]
        public string TargetPlayers
        {
            get { return m_targetPlayers; }
            set { m_targetPlayers = value; }
        }



        private int m_castRange = 10;
        [Description("Maximum range to cast aite on other players"), Category("Party Cast")]
        public int CastRange
        {
            get { return m_castRange; }
            set { m_castRange = value; }
        }

        private string m_aiteSpell = "ard naomh aite";
        [TypeConverter(typeof(AiteSpellConverter))]
        [Description("Aite spell to cast"), Category("Spell Selection")]
        public string AiteSpell
        {
            get { return m_aiteSpell; }
            set { m_aiteSpell = value; }
        }

        private bool m_requireMana = true;
        [Description("Only cast if we have enough mana"), Category("Safety")]
        public bool RequireMana
        {
            get { return m_requireMana; }
            set { m_requireMana = value; }
        }

        private int m_minimumMana = 20;
        [Description("Minimum mana required before casting"), Category("Safety")]
        public int MinimumMana
        {
            get { return m_minimumMana; }
            set { m_minimumMana = value; }
        }

        #endregion

        public override bool NeedToRun
        {
            get
            {
                // Rate limit checks
                if ((DateTime.Now - m_lastCheck).TotalMilliseconds < m_checkInterval)
                    return false;

                // Check if we need to do anything aite-related
                if (m_castOnSelf && !HasAite())
                {
                    return true;
                }

                if (m_castOnParty && HasTargetsNeedingAite())
                {
                    return true;
                }
                return false;
            }
            set { }
        }

        public override int Priority { get; set; } = 95;

        public override void Run(TimeSpan Elapsed)
        {
            if (!Enabled || InTransition)
                return;

            InTransition = true;

            try
            {
                m_lastCheck = DateTime.Now;

                // Clean up old tracking entries (older than 5 minutes)
                CleanupAiteTracking();

                // Check if we can cast (mana requirements)
                if (m_requireMana && Client.Attributes.CurrentMP() < m_minimumMana)
                {

                    return;
                }

                // Rate limit casting
                if ((DateTime.Now - m_lastCast).TotalMilliseconds < m_castInterval)
                    return;

                // Cast on self if needed
                if (m_castOnSelf && !HasAite())
                {
                    CastAiteOnSelf();
                }
                // Cast on party members if enabled (don't return after self cast)
                else if (m_castOnParty)
                {
                    var target = GetNextTargetNeedingAite();
                    if (target != null)
                    {
                        CastAiteOnTarget(target);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CheckIfAited error: {ex.Message}");
            }
            finally
            {
                Client.TransitionTo(this, Elapsed);
            }
        }

        #region Helper Methods

        private bool HasAite()
        {
            // Check if we have any aite spell active (spell ID 11)
            return Client?.SpellBar?.Contains(11) == true;
        }

        private void CleanupAiteTracking()
        {
            // Remove entries older than 5 minutes to prevent memory leaks
            var cutoffTime = DateTime.Now.AddMinutes(-5);
            var keysToRemove = m_lastAiteTime.Where(kvp => kvp.Value < cutoffTime).Select(kvp => kvp.Key).ToList();
            
            foreach (var key in keysToRemove)
            {
                m_lastAiteTime.Remove(key);
            }

            if (keysToRemove.Any())
            {

            }
        }

        private bool HasTargetsNeedingAite()
        {
            var targets = GetTargetPlayers();

            foreach (var target in targets)
            {
                var distance = Client.Attributes.ServerPosition.DistanceFrom(target.ServerPosition);
                var hasAite = PlayerHasAite(target);
                var inRange = distance <= m_castRange;

                if (inRange && !hasAite)
                {
                    return true;
                }
            }
            return false;
        }

        private MapObject GetNextTargetNeedingAite()
        {
            var targets = GetTargetPlayers();
            return targets.FirstOrDefault(target => 
                Client.Attributes.ServerPosition.DistanceFrom(target.ServerPosition) <= m_castRange &&
                !PlayerHasAite(target));
        }

        // Track when we last cast aite on players (since we can't read their buff status)
        private Dictionary<int, DateTime> m_lastAiteTime = new Dictionary<int, DateTime>();

        private List<MapObject> GetTargetPlayers()
        {
            var targets = new List<MapObject>();

            // Method 1: Get bot clients (from OtherClients - these are other connected bots)
            if (Client?.OtherClients != null)
            {

                foreach (var botClient in Client.OtherClients)
                {
                    if (botClient?.IsInGame() == true && 
                        botClient?.Attributes?.ServerPosition != null &&
                        botClient.Attributes.ServerPosition.DistanceFrom(Client.Attributes.ServerPosition) <= m_castRange)
                    {
                        // Create a MapObject-like representation for the bot client
                        var botAsTarget = new Aisling(botClient.Attributes.PlayerName)
                        {
                            Serial = botClient.Attributes.Serial,
                            ServerPosition = botClient.Attributes.ServerPosition,
                            Type = MapObjectType.Aisling
                        };

                        targets.Add(botAsTarget);

                        bool hasAite = botClient.SpellBar?.Contains(11) == true;

                    }
                }
            }

            // Method 2: Get visible players by name (if specified)
            if (!string.IsNullOrWhiteSpace(m_targetPlayers))
            {
                var playerNames = m_targetPlayers.Split(',')
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                if (playerNames.Any())
                {
                    var allPlayers = Client.ObjectSearcher?.RetreivePlayerTargets(obj => true) ?? new List<MapObject>();
                    

                    System.Diagnostics.Debug.WriteLine($"Total visible players: {allPlayers.Count}");

                    foreach (var targetName in playerNames)
                    {
                        var foundPlayer = allPlayers.FirstOrDefault(obj => 
                            obj is Aisling aisling && 
                            !string.IsNullOrEmpty(aisling.Name) &&
                            string.Equals(aisling.Name, targetName, StringComparison.OrdinalIgnoreCase) &&
                            obj.ServerPosition.DistanceFrom(Client.Attributes.ServerPosition) <= m_castRange);

                        if (foundPlayer != null && !targets.Any(t => t.Serial == foundPlayer.Serial))
                        {
                            targets.Add(foundPlayer);
                            System.Diagnostics.Debug.WriteLine($"Found named player: {targetName} (Serial: {foundPlayer.Serial})");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Could not find player: {targetName}");
                        }
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Final target count: {targets.Count}");

            return targets;
        }

        private bool PlayerHasAite(MapObject player)
        {
            // Method 1: Check if this is a bot client we can read spell bar from
            var botClient = Client?.OtherClients?.FirstOrDefault(c => c.Attributes.Serial == player.Serial);
            if (botClient != null)
            {
                bool hasAite = botClient.SpellBar?.Contains(11) == true;
                System.Diagnostics.Debug.WriteLine($"Bot client {botClient.Attributes.PlayerName}: Aite = {hasAite}");
                return hasAite;
            }

            // Method 2: For non-bot players, use time-based tracking
            // We can't read their spell bars, so we track when we last cast aite
            if (m_lastAiteTime.ContainsKey(player.Serial))
            {
                var timeSinceLastCast = DateTime.Now - m_lastAiteTime[player.Serial];
                bool shouldBeActive = timeSinceLastCast.TotalMinutes < 4; // Aite lasts ~4 minutes
                
                if (player is Aisling aisling)
                {
                    System.Diagnostics.Debug.WriteLine($"Player {aisling.Name}: Last aite {timeSinceLastCast.TotalSeconds:F0}s ago, Should be active: {shouldBeActive}");
                }
                
                return shouldBeActive;
            }

            // Never cast aite on this player, so they don't have it
            if (player is Aisling aisling2)
            {
                System.Diagnostics.Debug.WriteLine($"Player {aisling2.Name}: Never cast aite, needs buff");
            }
            return false;
        }

        private void CastAiteOnSelf()
        {
            try
            {
                Client.Utilities.CastSpell(m_aiteSpell, Client as Client);
                m_lastCast = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Cast {m_aiteSpell} on self");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error casting aite on self: {ex.Message}");
            }
        }

        private void CastAiteOnTarget(MapObject target)
        {
            try
            {
                string targetName = (target is Aisling aisling) ? aisling.Name : $"Serial_{target.Serial}";
                System.Diagnostics.Debug.WriteLine($"Attempting to cast {m_aiteSpell} on {targetName} (Serial: {target.Serial}, Sprite: {target.Sprite})");
                
                // Ensure target has valid sprite for casting (fix for sprite check in GameUtilities.Cast)
                if (target.Sprite == 0 || target.Sprite == ushort.MaxValue)
                {
                    System.Diagnostics.Debug.WriteLine($"Target {targetName} has invalid sprite ({target.Sprite}), setting to default player sprite");
                    target.Sprite = 1; // Default player sprite
                }
                
                Client.Utilities.CastSpell(m_aiteSpell, target);
                m_lastCast = DateTime.Now;

                // Track when we cast aite on this player for time-based detection
                m_lastAiteTime[target.Serial] = DateTime.Now;

                System.Diagnostics.Debug.WriteLine($"Successfully cast {m_aiteSpell} on {targetName} (Serial: {target.Serial})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error casting aite on target: {ex.Message}");
            }
        }

        #endregion

        public override string ToString()
        {
            bool hasAite = HasAite();
            var targetCount = GetTargetPlayers().Count;
            return $"CheckIfAited - Self: {(hasAite ? "Active" : "Inactive")}, Targets: {targetCount}";
        }
    }
}