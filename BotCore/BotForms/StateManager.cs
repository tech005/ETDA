using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using BotCore.States;
using BotCore.States.BotStates;
using BotCore.Types;

namespace BotCore.BotForms
{
    public partial class StateManager : UserControl
    {
        public Client client { get; set; }
        public BotInterface botInterface { get; set; }
        
        private Dictionary<GameState, StateControls> stateControlMap = new Dictionary<GameState, StateControls>();
        private StateControls selectedStateControl = null;
        private string configDirectory;
        private string playerConfigFile;
        private System.Windows.Forms.Timer statusTimer;

        public StateManager()
        {
            InitializeComponent();
            configDirectory = Path.Combine(Application.StartupPath, "Configs");
            if (!Directory.Exists(configDirectory))
                Directory.CreateDirectory(configDirectory);

            // Initialize UI
            comboBoxGroupBy.SelectedIndex = 0; // None
            
            // Setup status timer
            statusTimer = new System.Windows.Forms.Timer();
            statusTimer.Interval = 1000; // Update every second
            statusTimer.Tick += StatusTimer_Tick;
            statusTimer.Start();
        }

        public void Initialize(Client client, BotInterface botInterface)
        {
            try
            {
                this.client = client;
                this.botInterface = botInterface;
                
                if (client?.Attributes?.PlayerName != null)
                {
                    playerConfigFile = Path.Combine(configDirectory, $"{client.Attributes.PlayerName}.xml");
                }
                
                if (client?.StateMachine?.States != null)
                {
                    ApplyDefaultPriorities();
                    SetupStateControls();
                    LoadConfiguration();
                }
                else
                {
                    DebugLogger.Log("StateManager: Client StateMachine not ready, deferring setup");
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"StateManager Initialize error: {ex.Message}");
                DebugLogger.Log($"StateManager Initialize stack trace: {ex.StackTrace}");
            }
        }

        private void SetupStateControls()
        {
            try
            {
                if (client?.StateMachine?.States == null)
                {
                    DebugLogger.Log("SetupStateControls: No states available");
                    return;
                }

                stateControlMap.Clear();

                // Create all state controls
                foreach (var state in client.StateMachine.States)
                {
                    try
                    {
                        var stateControl = new StateControls();
                        stateControl.Initialize(state, this);
                        stateControl.Margin = new Padding(2);
                        
                        stateControlMap[state] = stateControl;
                    }
                    catch (Exception ex)
                    {
                        DebugLogger.Log($"Error creating control for state {state?.GetType()?.Name}: {ex.Message}");
                    }
                }

                // Apply current filter and grouping
                FilterAndGroupStates();
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"SetupStateControls error: {ex.Message}");
            }
        }

        private void ApplyDefaultPriorities()
        {
            try
            {
                if (client?.StateMachine?.States == null) return;

                foreach (var state in client.StateMachine.States)
                {
                    // Check if the state already has a priority set (non-zero)
                    if (state.Priority != 0) continue;

                    // Get the StateAttribute from the state's type
                    var stateAttribute = state.GetType().GetCustomAttributes(typeof(StateAttribute), false)
                        .FirstOrDefault() as StateAttribute;

                    if (stateAttribute != null)
                    {
                        // Apply the default priority from the attribute
                        state.Priority = stateAttribute.DefaultPriority;
                        DebugLogger.Log($"Applied default priority {stateAttribute.DefaultPriority} to {state.GetType().Name}");
                    }
                    else
                    {
                        // Fallback to a middle-ground priority if no attribute is found
                        state.Priority = 50;
                        DebugLogger.Log($"Applied fallback priority 50 to {state.GetType().Name}");
                    }
                }

                // Sort states by priority after applying defaults
                if (client.StateMachine.States is List<GameState> stateList)
                {
                    stateList.Sort();
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"ApplyDefaultPriorities error: {ex.Message}");
            }
        }

        public void RefreshStateDisplay()
        {
            foreach (var kvp in stateControlMap)
            {
                kvp.Value.RefreshDisplay();
            }
        }

        public void SaveConfiguration()
        {
            try
            {
                if (string.IsNullOrEmpty(playerConfigFile))
                {
                    MessageBox.Show("Cannot save configuration: Player name not available.", "Save Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var doc = new XmlDocument();
                var root = doc.CreateElement("BotConfiguration");
                doc.AppendChild(root);

                var statesNode = doc.CreateElement("States");
                root.AppendChild(statesNode);

                foreach (var state in client.StateMachine.States)
                {
                    var stateNode = doc.CreateElement("State");
                    stateNode.SetAttribute("Type", state.GetType().FullName);
                    stateNode.SetAttribute("Enabled", state.Enabled.ToString());
                    stateNode.SetAttribute("Priority", state.Priority.ToString());
                    
                    // Save state-specific properties
                    var propertiesNode = doc.CreateElement("Properties");
                    var properties = state.GetType().GetProperties()
                        .Where(p => p.CanRead && p.CanWrite && 
                               p.GetCustomAttributes(typeof(System.ComponentModel.CategoryAttribute), false).Length > 0);
                    
                    foreach (var property in properties)
                    {
                        try
                        {
                            var value = property.GetValue(state);
                            if (value != null)
                            {
                                var propNode = doc.CreateElement("Property");
                                propNode.SetAttribute("Name", property.Name);
                                propNode.SetAttribute("Value", value.ToString());
                                propNode.SetAttribute("Type", property.PropertyType.FullName);
                                propertiesNode.AppendChild(propNode);
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.Log($"Error saving property {property.Name}: {ex.Message}");
                        }
                    }
                    
                    stateNode.AppendChild(propertiesNode);
                    statesNode.AppendChild(stateNode);
                }

                doc.Save(playerConfigFile);
                MessageBox.Show($"Configuration saved successfully to {Path.GetFileName(playerConfigFile)}", 
                    "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                DebugLogger.Log($"Configuration saved to {playerConfigFile}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugLogger.Log($"Error saving configuration: {ex.Message}");
            }
        }

        public void LoadConfiguration()
        {
            try
            {
                if (string.IsNullOrEmpty(playerConfigFile) || !File.Exists(playerConfigFile))
                {
                    DebugLogger.Log("No saved configuration found, using defaults");
                    return;
                }

                var doc = new XmlDocument();
                doc.Load(playerConfigFile);

                var statesNode = doc.SelectSingleNode("//States");
                if (statesNode == null) return;

                foreach (XmlNode stateNode in statesNode.ChildNodes)
                {
                    var stateTypeName = stateNode.Attributes["Type"]?.Value;
                    if (string.IsNullOrEmpty(stateTypeName)) continue;

                    var state = client.StateMachine.States.FirstOrDefault(s => s.GetType().FullName == stateTypeName);
                    if (state == null) continue;

                    // Load basic state properties
                    if (bool.TryParse(stateNode.Attributes["Enabled"]?.Value, out bool enabled))
                        state.Enabled = enabled;
                    
                    if (int.TryParse(stateNode.Attributes["Priority"]?.Value, out int priority))
                        state.Priority = priority;

                    // Load state-specific properties
                    var propertiesNode = stateNode.SelectSingleNode("Properties");
                    if (propertiesNode != null)
                    {
                        foreach (XmlNode propNode in propertiesNode.ChildNodes)
                        {
                            var propName = propNode.Attributes["Name"]?.Value;
                            var propValue = propNode.Attributes["Value"]?.Value;
                            var propType = propNode.Attributes["Type"]?.Value;

                            if (string.IsNullOrEmpty(propName) || string.IsNullOrEmpty(propValue)) continue;

                            try
                            {
                                var property = state.GetType().GetProperty(propName);
                                if (property != null && property.CanWrite)
                                {
                                    object convertedValue = Convert.ChangeType(propValue, property.PropertyType);
                                    property.SetValue(state, convertedValue);
                                }
                            }
                            catch (Exception ex)
                            {
                                DebugLogger.Log($"Error loading property {propName}: {ex.Message}");
                            }
                        }
                    }
                }

                RefreshStateDisplay();
                DebugLogger.Log($"Configuration loaded from {playerConfigFile}");
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"Error loading configuration: {ex.Message}");
            }
        }

        public void RestoreDefaults()
        {
            try
            {
                var result = MessageBox.Show("This will reset all bot states to their default settings. Continue?", 
                    "Restore Defaults", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                
                if (result != DialogResult.Yes) return;

                foreach (var state in client.StateMachine.States)
                {
                    // Reset to default values
                    state.Enabled = false;
                    
                    // Apply default priority from StateAttribute
                    var stateAttribute = state.GetType().GetCustomAttributes(typeof(StateAttribute), false)
                        .FirstOrDefault() as StateAttribute;
                    
                    state.Priority = stateAttribute?.DefaultPriority ?? 50; // Use attribute or fallback to 50
                    
                    // Reset state-specific properties to their defaults
                    var properties = state.GetType().GetProperties()
                        .Where(p => p.CanWrite && 
                               p.GetCustomAttributes(typeof(System.ComponentModel.CategoryAttribute), false).Length > 0);
                    
                    foreach (var property in properties)
                    {
                        try
                        {
                            // Try to get default value from DefaultValueAttribute
                            var defaultValueAttr = property.GetCustomAttributes(typeof(System.ComponentModel.DefaultValueAttribute), false)
                                .Cast<System.ComponentModel.DefaultValueAttribute>().FirstOrDefault();
                            
                            if (defaultValueAttr != null)
                            {
                                property.SetValue(state, defaultValueAttr.Value);
                            }
                            else if (property.PropertyType.IsValueType)
                            {
                                // Set to default value for value types
                                property.SetValue(state, Activator.CreateInstance(property.PropertyType));
                            }
                            else if (property.PropertyType == typeof(string))
                            {
                                property.SetValue(state, string.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugLogger.Log($"Error resetting property {property.Name}: {ex.Message}");
                        }
                    }
                }

                RefreshStateDisplay();
                MessageBox.Show("All bot states have been reset to default values.", "Restore Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                DebugLogger.Log("Bot states restored to defaults");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error restoring defaults: {ex.Message}", "Restore Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DebugLogger.Log($"Error restoring defaults: {ex.Message}");
            }
        }

        public void OnStateSettingsClicked(GameState state)
        {
            if (botInterface != null)
            {
                botInterface.MakeActiveState(state.GetType().Name);
                // Switch to the Botting States tab
                botInterface.SwitchToBottingStatesTab();
            }
        }

        public void TryInitializeIfReady()
        {
            if (client?.StateMachine?.States != null && stateControlMap.Count == 0)
            {
                DebugLogger.Log("StateManager: Client now ready, initializing states");
                SetupStateControls();
                LoadConfiguration();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveConfiguration();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void buttonRestoreDefaults_Click(object sender, EventArgs e)
        {
            RestoreDefaults();
        }

        #region New Event Handlers

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (client?.Paused == true)
                {
                    labelBotStatus.Text = "Bot Status: Paused";
                    labelBotStatus.ForeColor = Color.Orange;
                    buttonPause.Enabled = false;
                    buttonResume.Enabled = true;
                }
                else if (client != null)
                {
                    labelBotStatus.Text = "Bot Status: Running";
                    labelBotStatus.ForeColor = Color.Green;
                    buttonPause.Enabled = true;
                    buttonResume.Enabled = false;
                }
                else
                {
                    labelBotStatus.Text = "Bot Status: Not Connected";
                    labelBotStatus.ForeColor = Color.Red;
                    buttonPause.Enabled = false;
                    buttonResume.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"StatusTimer error: {ex.Message}");
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Paused = true;
                DebugLogger.Log("Bot paused from State Manager");
            }
        }

        private void buttonResume_Click(object sender, EventArgs e)
        {
            if (client != null)
            {
                client.Paused = false;
                DebugLogger.Log("Bot resumed from State Manager");
            }
        }

        private void buttonDisableAll_Click(object sender, EventArgs e)
        {
            foreach (var state in client.StateMachine.States)
            {
                state.Enabled = false;
            }
            RefreshStateDisplay();
            DebugLogger.Log("All bot states disabled");
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            if (selectedStateControl?.state != null)
            {
                var state = selectedStateControl.state;
                var currentPriority = state.Priority;
                
                // Find next higher priority (lower number)
                var higherPriorityStates = client.StateMachine.States
                    .Where(s => s.Priority < currentPriority)
                    .OrderByDescending(s => s.Priority);
                
                if (higherPriorityStates.Any())
                {
                    var targetState = higherPriorityStates.First();
                    var targetPriority = targetState.Priority;
                    
                    // Swap priorities
                    state.Priority = targetPriority;
                    targetState.Priority = currentPriority;
                    
                    RefreshStateDisplay();
                    SetupStateControls(); // Recreate to reorder
                    DebugLogger.Log($"Moved {state.GetType().Name} up in priority");
                }
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            if (selectedStateControl?.state != null)
            {
                var state = selectedStateControl.state;
                var currentPriority = state.Priority;
                
                // Find next lower priority (higher number)
                var lowerPriorityStates = client.StateMachine.States
                    .Where(s => s.Priority > currentPriority)
                    .OrderBy(s => s.Priority);
                
                if (lowerPriorityStates.Any())
                {
                    var targetState = lowerPriorityStates.First();
                    var targetPriority = targetState.Priority;
                    
                    // Swap priorities
                    state.Priority = targetPriority;
                    targetState.Priority = currentPriority;
                    
                    RefreshStateDisplay();
                    SetupStateControls(); // Recreate to reorder
                    DebugLogger.Log($"Moved {state.GetType().Name} down in priority");
                }
            }
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            FilterAndGroupStates();
        }

        private void comboBoxGroupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterAndGroupStates();
        }

        private void FilterAndGroupStates()
        {
            try
            {
                if (client?.StateMachine?.States == null)
                {
                    return;
                }

                var filterText = textBoxFilter?.Text?.ToLower() ?? "";
                var groupBy = comboBoxGroupBy?.SelectedItem?.ToString() ?? "None";

                panelStates.Controls.Clear();

                var filteredStates = client.StateMachine.States.AsEnumerable();

            // Apply text filter
            if (!string.IsNullOrEmpty(filterText))
            {
                filteredStates = filteredStates.Where(s => 
                    s.GetType().Name.ToLower().Contains(filterText) ||
                    GetStateDescription(s).ToLower().Contains(filterText));
            }

            // Group and display states
            switch (groupBy)
            {
                case "Priority Range":
                    GroupStatesByPriorityRange(filteredStates);
                    break;
                case "Enabled Status":
                    GroupStatesByEnabledStatus(filteredStates);
                    break;
                case "State Type":
                    GroupStatesByType(filteredStates);
                    break;
                default:
                    DisplayStatesUngrouped(filteredStates.OrderBy(s => s.Priority));
                    break;
            }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"FilterAndGroupStates error: {ex.Message}");
            }
        }

        private void GroupStatesByPriorityRange(IEnumerable<GameState> states)
        {
            var highPriority = states.Where(s => s.Priority <= 25).OrderBy(s => s.Priority);
            var mediumPriority = states.Where(s => s.Priority > 25 && s.Priority <= 75).OrderBy(s => s.Priority);
            var lowPriority = states.Where(s => s.Priority > 75).OrderBy(s => s.Priority);

            if (highPriority.Any())
            {
                AddGroupHeader("High Priority (1-25)");
                foreach (var state in highPriority)
                {
                    AddStateControl(state);
                }
            }

            if (mediumPriority.Any())
            {
                AddGroupHeader("Medium Priority (26-75)");
                foreach (var state in mediumPriority)
                {
                    AddStateControl(state);
                }
            }

            if (lowPriority.Any())
            {
                AddGroupHeader("Low Priority (76+)");
                foreach (var state in lowPriority)
                {
                    AddStateControl(state);
                }
            }
        }

        private void GroupStatesByEnabledStatus(IEnumerable<GameState> states)
        {
            var enabledStates = states.Where(s => s.Enabled).OrderBy(s => s.Priority);
            var disabledStates = states.Where(s => !s.Enabled).OrderBy(s => s.Priority);

            if (enabledStates.Any())
            {
                AddGroupHeader("Enabled States");
                foreach (var state in enabledStates)
                {
                    AddStateControl(state);
                }
            }

            if (disabledStates.Any())
            {
                AddGroupHeader("Disabled States");
                foreach (var state in disabledStates)
                {
                    AddStateControl(state);
                }
            }
        }

        private void GroupStatesByType(IEnumerable<GameState> states)
        {
            var groups = states.GroupBy(s => GetStateCategory(s))
                             .OrderBy(g => g.Key);

            foreach (var group in groups)
            {
                AddGroupHeader(group.Key);
                foreach (var state in group.OrderBy(s => s.Priority))
                {
                    AddStateControl(state);
                }
            }
        }

        private void DisplayStatesUngrouped(IEnumerable<GameState> states)
        {
            foreach (var state in states)
            {
                AddStateControl(state);
            }
        }

        private void AddGroupHeader(string groupName)
        {
            var headerLabel = new Label
            {
                Text = groupName,
                Font = new Font("Arial", 9F, FontStyle.Bold),
                BackColor = Color.DarkSlateBlue,
                ForeColor = Color.White,
                Height = 25,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            panelStates.Controls.Add(headerLabel);
            headerLabel.BringToFront();
        }

        private void AddStateControl(GameState state)
        {
            if (stateControlMap.ContainsKey(state))
            {
                var control = stateControlMap[state];
                control.RefreshDisplay();
                control.Dock = DockStyle.Top;
                panelStates.Controls.Add(control);
                control.BringToFront();
            }
        }

        private string GetStateCategory(GameState state)
        {
            var name = state.GetType().Name;
            
            if (name.Contains("Attack") || name.Contains("Melee") || name.Contains("Combat"))
                return "Combat States";
            if (name.Contains("Heal") || name.Contains("Buff") || name.Contains("Aite") || name.Contains("BC"))
                return "Support States";
            if (name.Contains("Walk") || name.Contains("Follow") || name.Contains("Move"))
                return "Movement States";
            if (name.Contains("Check") || name.Contains("Monitor"))
                return "Monitoring States";
            if (name.Contains("Fas") || name.Contains("Curse"))
                return "Spell States";
            
            return "Other States";
        }

        private string GetStateDescription(GameState state)
        {
            var descAttr = state.GetType().GetCustomAttributes(typeof(StateAttribute), false)
                .Cast<StateAttribute>().FirstOrDefault();
            return descAttr?.Desc ?? "No description available";
        }

        public void OnStateControlSelected(StateControls control)
        {
            // Deselect previous
            if (selectedStateControl != null)
            {
                selectedStateControl.BackColor = selectedStateControl.state.Enabled ? 
                    Color.LightGreen : Color.LightGray;
            }

            // Select new
            selectedStateControl = control;
            if (selectedStateControl != null)
            {
                selectedStateControl.BackColor = Color.LightBlue;
            }

            // Update move button states
            buttonMoveUp.Enabled = selectedStateControl != null;
            buttonMoveDown.Enabled = selectedStateControl != null;
        }

        #endregion
    }

    // Individual state control for the state manager
    public partial class StateControls : UserControl
    {
        public GameState state { get; private set; }
        private StateManager manager;
        
        public StateControls()
        {
            InitializeComponent();
            
            // Make entire control clickable for selection
            this.Click += OnControlClicked;
            panelMain.Click += OnControlClicked;
            panelCenter.Click += OnControlClicked;
            labelStateName.Click += OnControlClicked;
            labelDescription.Click += OnControlClicked;
        }

        public void Initialize(GameState state, StateManager manager)
        {
            this.state = state;
            this.manager = manager;
            
            RefreshDisplay();
            
            // Wire up events
            checkBoxEnabled.CheckedChanged += OnEnabledChanged;
            numericUpDownPriority.ValueChanged += OnPriorityChanged;
            buttonSettings.Click += OnSettingsClicked;
        }

        private void OnControlClicked(object sender, EventArgs e)
        {
            manager?.OnStateControlSelected(this);
        }

        public void RefreshDisplay()
        {
            if (state == null) return;

            labelStateName.Text = state.GetType().Name;
            numericUpDownPriority.Value = Math.Max(numericUpDownPriority.Minimum, 
                Math.Min(numericUpDownPriority.Maximum, state.Priority));
            
            // Update enabled checkbox appearance
            checkBoxEnabled.Checked = state.Enabled;
            if (state.Enabled)
            {
                checkBoxEnabled.Text = "ON";
                checkBoxEnabled.BackColor = Color.LightGreen;
                this.BackColor = Color.Honeydew;
            }
            else
            {
                checkBoxEnabled.Text = "OFF";
                checkBoxEnabled.BackColor = Color.LightGray;
                this.BackColor = Color.WhiteSmoke;
            }
            
            // Show description
            var descAttr = state.GetType().GetCustomAttributes(typeof(StateAttribute), false)
                .Cast<StateAttribute>().FirstOrDefault();
            
            string description = descAttr?.Desc ?? "No description available";
            labelDescription.Text = description.Length > 60 ? 
                description.Substring(0, 57) + "..." : description;
            
            // Set tooltips
            toolTip.SetToolTip(labelStateName, $"State: {state.GetType().Name}\nPriority: {state.Priority}\nEnabled: {state.Enabled}");
            toolTip.SetToolTip(labelDescription, description);
            toolTip.SetToolTip(this, description);
        }

        private void OnEnabledChanged(object sender, EventArgs e)
        {
            if (state != null)
            {
                state.Enabled = checkBoxEnabled.Checked;
                RefreshDisplay();
                manager?.RefreshStateDisplay();
            }
        }

        private void OnPriorityChanged(object sender, EventArgs e)
        {
            if (state != null)
            {
                state.Priority = (int)numericUpDownPriority.Value;
            }
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            manager?.OnStateSettingsClicked(state);
        }
    }
}