namespace BotCore.BotForms
{
    partial class StateManager
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelMain = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBoxStates = new System.Windows.Forms.GroupBox();
            this.panelStatesContainer = new System.Windows.Forms.Panel();
            this.panelStates = new System.Windows.Forms.Panel();
            this.panelStateFilter = new System.Windows.Forms.Panel();
            this.comboBoxGroupBy = new System.Windows.Forms.ComboBox();
            this.labelGroupBy = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.labelFilter = new System.Windows.Forms.Label();
            this.groupBoxControls = new System.Windows.Forms.GroupBox();
            this.panelPriorityControls = new System.Windows.Forms.Panel();
            this.buttonMoveDown = new System.Windows.Forms.Button();
            this.buttonMoveUp = new System.Windows.Forms.Button();
            this.labelPriorityHelp = new System.Windows.Forms.Label();
            this.panelBotControls = new System.Windows.Forms.Panel();
            this.buttonResume = new System.Windows.Forms.Button();
            this.buttonPause = new System.Windows.Forms.Button();
            this.labelBotStatus = new System.Windows.Forms.Label();
            this.panelConfigControls = new System.Windows.Forms.Panel();
            this.buttonRestoreDefaults = new System.Windows.Forms.Button();
            this.buttonLoadConfig = new System.Windows.Forms.Button();
            this.buttonSaveConfig = new System.Windows.Forms.Button();
            this.labelConfigHelp = new System.Windows.Forms.Label();
            this.panelQuickActions = new System.Windows.Forms.Panel();
            this.buttonDisableAll = new System.Windows.Forms.Button();
            this.labelQuickActions = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBoxStates.SuspendLayout();
            this.panelStatesContainer.SuspendLayout();
            this.panelStateFilter.SuspendLayout();
            this.groupBoxControls.SuspendLayout();
            this.panelPriorityControls.SuspendLayout();
            this.panelBotControls.SuspendLayout();
            this.panelConfigControls.SuspendLayout();
            this.panelQuickActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.splitContainer1);
            this.panelMain.Controls.Add(this.labelTitle);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(900, 600);
            this.panelMain.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 40);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBoxStates);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBoxControls);
            this.splitContainer1.Size = new System.Drawing.Size(900, 560);
            this.splitContainer1.SplitterDistance = 600;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBoxStates
            // 
            this.groupBoxStates.Controls.Add(this.panelStatesContainer);
            this.groupBoxStates.Controls.Add(this.panelStateFilter);
            this.groupBoxStates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxStates.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxStates.Location = new System.Drawing.Point(0, 0);
            this.groupBoxStates.Name = "groupBoxStates";
            this.groupBoxStates.Size = new System.Drawing.Size(600, 560);
            this.groupBoxStates.TabIndex = 0;
            this.groupBoxStates.TabStop = false;
            this.groupBoxStates.Text = "Bot States";
            // 
            // panelStatesContainer
            // 
            this.panelStatesContainer.Controls.Add(this.panelStates);
            this.panelStatesContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStatesContainer.Location = new System.Drawing.Point(3, 80);
            this.panelStatesContainer.Name = "panelStatesContainer";
            this.panelStatesContainer.Size = new System.Drawing.Size(594, 477);
            this.panelStatesContainer.TabIndex = 1;
            // 
            // panelStates
            // 
            this.panelStates.AutoScroll = true;
            this.panelStates.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStates.Location = new System.Drawing.Point(0, 0);
            this.panelStates.Name = "panelStates";
            this.panelStates.Padding = new System.Windows.Forms.Padding(5);
            this.panelStates.Size = new System.Drawing.Size(594, 477);
            this.panelStates.TabIndex = 0;
            // 
            // panelStateFilter
            // 
            this.panelStateFilter.Controls.Add(this.comboBoxGroupBy);
            this.panelStateFilter.Controls.Add(this.labelGroupBy);
            this.panelStateFilter.Controls.Add(this.textBoxFilter);
            this.panelStateFilter.Controls.Add(this.labelFilter);
            this.panelStateFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelStateFilter.Location = new System.Drawing.Point(3, 22);
            this.panelStateFilter.Name = "panelStateFilter";
            this.panelStateFilter.Size = new System.Drawing.Size(594, 58);
            this.panelStateFilter.TabIndex = 0;
            // 
            // comboBoxGroupBy
            // 
            this.comboBoxGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGroupBy.Font = new System.Drawing.Font("Arial", 9F);
            this.comboBoxGroupBy.FormattingEnabled = true;
            this.comboBoxGroupBy.Items.AddRange(new object[] {
            "None",
            "Priority Range",
            "Enabled Status",
            "State Type"});
            this.comboBoxGroupBy.Location = new System.Drawing.Point(380, 28);
            this.comboBoxGroupBy.Name = "comboBoxGroupBy";
            this.comboBoxGroupBy.Size = new System.Drawing.Size(150, 25);
            this.comboBoxGroupBy.TabIndex = 3;
            this.comboBoxGroupBy.SelectedIndexChanged += new System.EventHandler(this.comboBoxGroupBy_SelectedIndexChanged);
            // 
            // labelGroupBy
            // 
            this.labelGroupBy.AutoSize = true;
            this.labelGroupBy.Font = new System.Drawing.Font("Arial", 9F);
            this.labelGroupBy.Location = new System.Drawing.Point(380, 8);
            this.labelGroupBy.Name = "labelGroupBy";
            this.labelGroupBy.Size = new System.Drawing.Size(73, 17);
            this.labelGroupBy.TabIndex = 2;
            this.labelGroupBy.Text = "Group By:";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Font = new System.Drawing.Font("Arial", 9F);
            this.textBoxFilter.Location = new System.Drawing.Point(10, 28);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(200, 25);
            this.textBoxFilter.TabIndex = 1;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // labelFilter
            // 
            this.labelFilter.AutoSize = true;
            this.labelFilter.Font = new System.Drawing.Font("Arial", 9F);
            this.labelFilter.Location = new System.Drawing.Point(10, 8);
            this.labelFilter.Name = "labelFilter";
            this.labelFilter.Size = new System.Drawing.Size(94, 17);
            this.labelFilter.TabIndex = 0;
            this.labelFilter.Text = "Filter States:";
            // 
            // groupBoxControls
            // 
            this.groupBoxControls.Controls.Add(this.panelPriorityControls);
            this.groupBoxControls.Controls.Add(this.panelBotControls);
            this.groupBoxControls.Controls.Add(this.panelConfigControls);
            this.groupBoxControls.Controls.Add(this.panelQuickActions);
            this.groupBoxControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxControls.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxControls.Location = new System.Drawing.Point(0, 0);
            this.groupBoxControls.Name = "groupBoxControls";
            this.groupBoxControls.Size = new System.Drawing.Size(296, 560);
            this.groupBoxControls.TabIndex = 0;
            this.groupBoxControls.TabStop = false;
            this.groupBoxControls.Text = "Control Panel";
            // 
            // panelPriorityControls
            // 
            this.panelPriorityControls.Controls.Add(this.buttonMoveDown);
            this.panelPriorityControls.Controls.Add(this.buttonMoveUp);
            this.panelPriorityControls.Controls.Add(this.labelPriorityHelp);
            this.panelPriorityControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPriorityControls.Location = new System.Drawing.Point(3, 322);
            this.panelPriorityControls.Name = "panelPriorityControls";
            this.panelPriorityControls.Size = new System.Drawing.Size(290, 100);
            this.panelPriorityControls.TabIndex = 3;
            // 
            // buttonMoveDown
            // 
            this.buttonMoveDown.BackColor = System.Drawing.Color.LightSalmon;
            this.buttonMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMoveDown.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonMoveDown.Location = new System.Drawing.Point(150, 60);
            this.buttonMoveDown.Name = "buttonMoveDown";
            this.buttonMoveDown.Size = new System.Drawing.Size(120, 30);
            this.buttonMoveDown.TabIndex = 2;
            this.buttonMoveDown.Text = "↓ Move Down";
            this.buttonMoveDown.UseVisualStyleBackColor = false;
            this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
            // 
            // buttonMoveUp
            // 
            this.buttonMoveUp.BackColor = System.Drawing.Color.LightBlue;
            this.buttonMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMoveUp.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonMoveUp.Location = new System.Drawing.Point(20, 60);
            this.buttonMoveUp.Name = "buttonMoveUp";
            this.buttonMoveUp.Size = new System.Drawing.Size(120, 30);
            this.buttonMoveUp.TabIndex = 1;
            this.buttonMoveUp.Text = "↑ Move Up";
            this.buttonMoveUp.UseVisualStyleBackColor = false;
            this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
            // 
            // labelPriorityHelp
            // 
            this.labelPriorityHelp.Font = new System.Drawing.Font("Arial", 8F);
            this.labelPriorityHelp.Location = new System.Drawing.Point(10, 10);
            this.labelPriorityHelp.Name = "labelPriorityHelp";
            this.labelPriorityHelp.Size = new System.Drawing.Size(270, 45);
            this.labelPriorityHelp.TabIndex = 0;
            this.labelPriorityHelp.Text = "Priority Management:\r\nSelect a state and use buttons to adjust execution order. L" +
    "ower numbers = higher priority.";
            // 
            // panelBotControls
            // 
            this.panelBotControls.Controls.Add(this.buttonResume);
            this.panelBotControls.Controls.Add(this.buttonPause);
            this.panelBotControls.Controls.Add(this.labelBotStatus);
            this.panelBotControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelBotControls.Location = new System.Drawing.Point(3, 222);
            this.panelBotControls.Name = "panelBotControls";
            this.panelBotControls.Size = new System.Drawing.Size(290, 100);
            this.panelBotControls.TabIndex = 2;
            // 
            // buttonResume
            // 
            this.buttonResume.BackColor = System.Drawing.Color.LightGreen;
            this.buttonResume.Enabled = false;
            this.buttonResume.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonResume.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.buttonResume.Location = new System.Drawing.Point(150, 50);
            this.buttonResume.Name = "buttonResume";
            this.buttonResume.Size = new System.Drawing.Size(120, 35);
            this.buttonResume.TabIndex = 2;
            this.buttonResume.Text = "▶ Resume";
            this.buttonResume.UseVisualStyleBackColor = false;
            this.buttonResume.Click += new System.EventHandler(this.buttonResume_Click);
            // 
            // buttonPause
            // 
            this.buttonPause.BackColor = System.Drawing.Color.Gold;
            this.buttonPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPause.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold);
            this.buttonPause.Location = new System.Drawing.Point(20, 50);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(120, 35);
            this.buttonPause.TabIndex = 1;
            this.buttonPause.Text = "⏸ Pause";
            this.buttonPause.UseVisualStyleBackColor = false;
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // labelBotStatus
            // 
            this.labelBotStatus.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.labelBotStatus.ForeColor = System.Drawing.Color.Green;
            this.labelBotStatus.Location = new System.Drawing.Point(10, 10);
            this.labelBotStatus.Name = "labelBotStatus";
            this.labelBotStatus.Size = new System.Drawing.Size(270, 30);
            this.labelBotStatus.TabIndex = 0;
            this.labelBotStatus.Text = "Bot Status: Running";
            this.labelBotStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelConfigControls
            // 
            this.panelConfigControls.Controls.Add(this.buttonRestoreDefaults);
            this.panelConfigControls.Controls.Add(this.buttonLoadConfig);
            this.panelConfigControls.Controls.Add(this.buttonSaveConfig);
            this.panelConfigControls.Controls.Add(this.labelConfigHelp);
            this.panelConfigControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelConfigControls.Location = new System.Drawing.Point(3, 122);
            this.panelConfigControls.Name = "panelConfigControls";
            this.panelConfigControls.Size = new System.Drawing.Size(290, 100);
            this.panelConfigControls.TabIndex = 1;
            // 
            // buttonRestoreDefaults
            // 
            this.buttonRestoreDefaults.BackColor = System.Drawing.Color.LightCoral;
            this.buttonRestoreDefaults.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRestoreDefaults.Font = new System.Drawing.Font("Arial", 8F);
            this.buttonRestoreDefaults.Location = new System.Drawing.Point(195, 65);
            this.buttonRestoreDefaults.Name = "buttonRestoreDefaults";
            this.buttonRestoreDefaults.Size = new System.Drawing.Size(80, 25);
            this.buttonRestoreDefaults.TabIndex = 3;
            this.buttonRestoreDefaults.Text = "Reset All";
            this.buttonRestoreDefaults.UseVisualStyleBackColor = false;
            this.buttonRestoreDefaults.Click += new System.EventHandler(this.buttonRestoreDefaults_Click);
            // 
            // buttonLoadConfig
            // 
            this.buttonLoadConfig.BackColor = System.Drawing.Color.LightBlue;
            this.buttonLoadConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoadConfig.Font = new System.Drawing.Font("Arial", 8F);
            this.buttonLoadConfig.Location = new System.Drawing.Point(105, 65);
            this.buttonLoadConfig.Name = "buttonLoadConfig";
            this.buttonLoadConfig.Size = new System.Drawing.Size(80, 25);
            this.buttonLoadConfig.TabIndex = 2;
            this.buttonLoadConfig.Text = "Load";
            this.buttonLoadConfig.UseVisualStyleBackColor = false;
            this.buttonLoadConfig.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonSaveConfig
            // 
            this.buttonSaveConfig.BackColor = System.Drawing.Color.LightGreen;
            this.buttonSaveConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSaveConfig.Font = new System.Drawing.Font("Arial", 8F);
            this.buttonSaveConfig.Location = new System.Drawing.Point(15, 65);
            this.buttonSaveConfig.Name = "buttonSaveConfig";
            this.buttonSaveConfig.Size = new System.Drawing.Size(80, 25);
            this.buttonSaveConfig.TabIndex = 1;
            this.buttonSaveConfig.Text = "Save";
            this.buttonSaveConfig.UseVisualStyleBackColor = false;
            this.buttonSaveConfig.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // labelConfigHelp
            // 
            this.labelConfigHelp.Font = new System.Drawing.Font("Arial", 8F);
            this.labelConfigHelp.Location = new System.Drawing.Point(10, 10);
            this.labelConfigHelp.Name = "labelConfigHelp";
            this.labelConfigHelp.Size = new System.Drawing.Size(270, 45);
            this.labelConfigHelp.TabIndex = 0;
            this.labelConfigHelp.Text = "Configuration Management:\r\nSave your current setup or load a previous configurat" +
    "ion for this character.";
            // 
            // panelQuickActions
            // 
            this.panelQuickActions.Controls.Add(this.buttonDisableAll);
            this.panelQuickActions.Controls.Add(this.labelQuickActions);
            this.panelQuickActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelQuickActions.Location = new System.Drawing.Point(3, 22);
            this.panelQuickActions.Name = "panelQuickActions";
            this.panelQuickActions.Size = new System.Drawing.Size(290, 80);
            this.panelQuickActions.TabIndex = 0;
            // 
            // buttonDisableAll
            // 
            this.buttonDisableAll.BackColor = System.Drawing.Color.LightPink;
            this.buttonDisableAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDisableAll.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonDisableAll.Location = new System.Drawing.Point(85, 40);
            this.buttonDisableAll.Name = "buttonDisableAll";
            this.buttonDisableAll.Size = new System.Drawing.Size(120, 30);
            this.buttonDisableAll.TabIndex = 1;
            this.buttonDisableAll.Text = "Disable All";
            this.buttonDisableAll.UseVisualStyleBackColor = false;
            this.buttonDisableAll.Click += new System.EventHandler(this.buttonDisableAll_Click);
            // 
            // labelQuickActions
            // 
            this.labelQuickActions.Font = new System.Drawing.Font("Arial", 8F);
            this.labelQuickActions.Location = new System.Drawing.Point(10, 10);
            this.labelQuickActions.Name = "labelQuickActions";
            this.labelQuickActions.Size = new System.Drawing.Size(270, 25);
            this.labelQuickActions.TabIndex = 0;
            this.labelQuickActions.Text = "Quick Actions: Disable all states at once.";
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelTitle.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.White;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(900, 40);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "🤖 Bot State Manager - Control Center";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StateManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Arial", 8F);
            this.Name = "StateManager";
            this.Size = new System.Drawing.Size(920, 620);
            this.panelMain.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBoxStates.ResumeLayout(false);
            this.panelStatesContainer.ResumeLayout(false);
            this.panelStateFilter.ResumeLayout(false);
            this.panelStateFilter.PerformLayout();
            this.groupBoxControls.ResumeLayout(false);
            this.panelPriorityControls.ResumeLayout(false);
            this.panelBotControls.ResumeLayout(false);
            this.panelConfigControls.ResumeLayout(false);
            this.panelQuickActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBoxStates;
        private System.Windows.Forms.Panel panelStatesContainer;
        private System.Windows.Forms.Panel panelStates;
        private System.Windows.Forms.Panel panelStateFilter;
        private System.Windows.Forms.ComboBox comboBoxGroupBy;
        private System.Windows.Forms.Label labelGroupBy;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label labelFilter;
        private System.Windows.Forms.GroupBox groupBoxControls;
        private System.Windows.Forms.Panel panelPriorityControls;
        private System.Windows.Forms.Button buttonMoveDown;
        private System.Windows.Forms.Button buttonMoveUp;
        private System.Windows.Forms.Label labelPriorityHelp;
        private System.Windows.Forms.Panel panelBotControls;
        private System.Windows.Forms.Button buttonResume;
        private System.Windows.Forms.Button buttonPause;
        private System.Windows.Forms.Label labelBotStatus;
        private System.Windows.Forms.Panel panelConfigControls;
        private System.Windows.Forms.Button buttonRestoreDefaults;
        private System.Windows.Forms.Button buttonLoadConfig;
        private System.Windows.Forms.Button buttonSaveConfig;
        private System.Windows.Forms.Label labelConfigHelp;
        private System.Windows.Forms.Panel panelQuickActions;
        private System.Windows.Forms.Button buttonDisableAll;
        private System.Windows.Forms.Label labelQuickActions;
    }

    partial class StateControls
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.numericUpDownPriority = new System.Windows.Forms.NumericUpDown();
            this.labelPriority = new System.Windows.Forms.Label();
            this.panelCenter = new System.Windows.Forms.Panel();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelStateName = new System.Windows.Forms.Label();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPriority)).BeginInit();
            this.panelCenter.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panelCenter);
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Controls.Add(this.panelLeft);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(570, 55);
            this.panelMain.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.buttonSettings);
            this.panelRight.Controls.Add(this.numericUpDownPriority);
            this.panelRight.Controls.Add(this.labelPriority);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(370, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(200, 55);
            this.panelRight.TabIndex = 2;
            // 
            // buttonSettings
            // 
            this.buttonSettings.BackColor = System.Drawing.Color.AliceBlue;
            this.buttonSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSettings.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.buttonSettings.Location = new System.Drawing.Point(125, 15);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(65, 25);
            this.buttonSettings.TabIndex = 2;
            this.buttonSettings.Text = "⚙ Config";
            this.buttonSettings.UseVisualStyleBackColor = false;
            // 
            // numericUpDownPriority
            // 
            this.numericUpDownPriority.Font = new System.Drawing.Font("Arial", 9F);
            this.numericUpDownPriority.Location = new System.Drawing.Point(60, 17);
            this.numericUpDownPriority.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numericUpDownPriority.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownPriority.Name = "numericUpDownPriority";
            this.numericUpDownPriority.Size = new System.Drawing.Size(60, 25);
            this.numericUpDownPriority.TabIndex = 1;
            this.numericUpDownPriority.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // labelPriority
            // 
            this.labelPriority.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.labelPriority.Location = new System.Drawing.Point(10, 20);
            this.labelPriority.Name = "labelPriority";
            this.labelPriority.Size = new System.Drawing.Size(50, 20);
            this.labelPriority.TabIndex = 0;
            this.labelPriority.Text = "Priority:";
            this.labelPriority.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.labelDescription);
            this.panelCenter.Controls.Add(this.labelStateName);
            this.panelCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCenter.Location = new System.Drawing.Point(90, 0);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new System.Drawing.Size(280, 55);
            this.panelCenter.TabIndex = 1;
            // 
            // labelDescription
            // 
            this.labelDescription.Font = new System.Drawing.Font("Arial", 7F, System.Drawing.FontStyle.Italic);
            this.labelDescription.ForeColor = System.Drawing.Color.Gray;
            this.labelDescription.Location = new System.Drawing.Point(5, 32);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(270, 20);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "State description";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelStateName
            // 
            this.labelStateName.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.labelStateName.Location = new System.Drawing.Point(5, 8);
            this.labelStateName.Name = "labelStateName";
            this.labelStateName.Size = new System.Drawing.Size(270, 25);
            this.labelStateName.TabIndex = 0;
            this.labelStateName.Text = "StateName";
            this.labelStateName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.checkBoxEnabled);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(90, 55);
            this.panelLeft.TabIndex = 0;
            // 
            // checkBoxEnabled
            // 
            this.checkBoxEnabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxEnabled.BackColor = System.Drawing.Color.LightGray;
            this.checkBoxEnabled.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightGreen;
            this.checkBoxEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxEnabled.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Bold);
            this.checkBoxEnabled.Location = new System.Drawing.Point(10, 10);
            this.checkBoxEnabled.Name = "checkBoxEnabled";
            this.checkBoxEnabled.Size = new System.Drawing.Size(70, 35);
            this.checkBoxEnabled.TabIndex = 0;
            this.checkBoxEnabled.Text = "OFF";
            this.checkBoxEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxEnabled.UseVisualStyleBackColor = false;
            // 
            // StateControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Font = new System.Drawing.Font("Arial", 8F);
            this.Name = "StateControls";
            this.Size = new System.Drawing.Size(570, 55);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPriority)).EndInit();
            this.panelCenter.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.NumericUpDown numericUpDownPriority;
        private System.Windows.Forms.Label labelPriority;
        private System.Windows.Forms.Panel panelCenter;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelStateName;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.CheckBox checkBoxEnabled;
        private System.Windows.Forms.ToolTip toolTip;
    }
}