using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BotCore.Types;
using System.Drawing;
using BotCore.BotForms;
using BotCore.Actions;
using System.Runtime.InteropServices;
using BotCore.Shared.Memory;

namespace BotCore
{
    public partial class BotInterface : Form
    {
        public Client client { get; set; }
        public MiniInterace MiniWindow { get; set; }
        private StateManager stateManager { get; set; }

        public BotInterface(Client client)
        {
            InitializeComponent();
            this.client = client;
            MiniWindow = new MiniInterace(client);
            MiniWindow.MdiParent = Collections.ParentForm;
            MiniWindow.Dock = DockStyle.Bottom;

            // Initialize debug logger
            Types.DebugLogger.Initialize(this);
            Types.DebugLogger.LogInfo("Bot Interface initialized");

            Task.Run(() => { UpdateClient(); });
            VisibleChanged += BotInterface_VisibleChanged;
        }

        public Color GetBlendedColor(int percentage)
        {
            return Color.FromArgb(percentage, Color.Green);
        }

        private void BotInterface_VisibleChanged(object sender, EventArgs e)
        {
            if (client.ClientReady && client.IsInGame() && !IsDisposed)
                Text = client.Attributes.PlayerName;
        }

        public void UpdateClient()
        {
            while (true)
            {

                if (!Disposing && !IsDisposed
                    && InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate ()
                    {
                        if (client.Paused)
                        {
                            if (!button4.Enabled)
                                button4.Enabled = true;
                            if (button3.Enabled)
                                button3.Enabled = false;
                        }
                        else
                        {
                            if (button4.Enabled)
                                button4.Enabled = false;
                            if (!button3.Enabled)
                                button3.Enabled = true;
                        }

                        try
                        {
                            if (!client.ClientReady || !client.IsInGame())
                            {
                                client.ClientReady = client.IsInGame();
                                if (Visible)
                                    Hide();
                            }
                            else
                            {
                                if (!Visible && !MiniWindow.Visible && !MiniWindow.Disposing
                                && !MiniWindow.IsDisposed)
                                    Show();

                                MiniWindow.UpdateStatistics();
                            }
                        }
                        catch
                        {

                        }
                    });
                }
                Thread.Sleep(1500);
            }
        }

        private void BotInterface_Load(object sender, EventArgs e)
        {
            Invalidated += BotInterface_Invalidated;
            client.ObjectSearcher.OnTargetUpdated += ObjectUpdated;
            comboBox1.DataSource = client.StateMachine.States.OrderBy(i => i.Priority).Select(i => i.GetType().Name).ToList();
            button4.Enabled = false;

            // Initialize StateManager
            InitializeStateManager();

            //example target condition for sprite 467 (noam plain white bird)
            Collections.TargetConditions[467] = new TargetCondition()
            {
                predicate = (target) => true,
                Priority = 3
            };

            //packet editor initialize
            richTextBox1.BackColor = Color.White;
            richTextBox1.ReadOnly = true;
        }

        void BotInterface_Invalidated(object sender, InvalidateEventArgs e)
        {
            SetStateNodes();
            
            // Try to initialize StateManager if it wasn't ready before
            if (stateManager != null)
            {
                stateManager.TryInitializeIfReady();
            }
        }

        private void SetStateNodes()
        {
            panel1.Controls.Clear();
            foreach (var s in client.StateMachine.States.Where(i => i.Enabled).OrderBy(i => i.Priority))
            {
                var spanel = new statetoggle();
                spanel.state = s;
                spanel.client = client;
                spanel.botinterface = this;
                spanel.Dock = DockStyle.Top;
                spanel.SetInfo();
                panel1.Controls.Add(spanel);
            }
            panel1.Invalidate();
        }

        private void ObjectUpdated(object sender, MapObject[] e)
        {
            var nearest = client.ObjectSearcher?.NearestTarget;

            if (nearest != null)
            {

            }
        }

        private void BotInterface_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (Visible)
            {
                Hide();
                MiniWindow.Show();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MakeActiveState(comboBox1.Text);
        }

        public void MakeActiveState(string name)
        {
            statepanel.Controls.Clear();
            statepanel.Invalidate();

            var state = client.StateMachine.States.FirstOrDefault(i => i.GetType().Name == name);
            if (state != null)
            {
                state.InitState();
                statepanel.Controls.Add(state.SettingsInterface);
            }

            SetStateNodes();
        }

        private void InitializeStateManager()
        {
            try
            {
                stateManager = new StateManager();
                stateManager.Dock = DockStyle.Fill;
                tabPageStateManager.Controls.Add(stateManager);
                
                // Initialize after the form is fully loaded
                if (client != null && client.StateMachine != null && client.StateMachine.States != null)
                {
                    stateManager.Initialize(client, this);
                }
                else
                {
                    DebugLogger.Log("StateManager initialization deferred - client not fully ready");
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Log($"Error initializing StateManager: {ex.Message}");
                DebugLogger.Log($"StateManager stack trace: {ex.StackTrace}");
            }
        }


        #region Packet Editor Stuff
        bool EnablePacketEditor = false;
        bool IsReceivingPackets = false;
        bool IsSendingPackets = false;


        public void AppendText(RichTextBox box, string text, Color color, bool AddNewLine = false)
        {
            if (box.Lines.Length > 256)
                box.Clear();

            if (pausePackets)
            {
                return;
            }

            if (AddNewLine)
            {
                text += Environment.NewLine;
            }

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;

            box.SelectionStart = box.Text.Length;
            box.ScrollToCaret();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            EnablePacketEditor = !EnablePacketEditor;

            if (EnablePacketEditor)
            {
                client.OnPacketRecevied += EditorReceivedPacket;
                client.OnPacketSent += EditorSentPacket;
                richTextBox1.Enabled = true;
            }
            else
            {
                client.OnPacketRecevied -= EditorReceivedPacket;
                client.OnPacketSent -= EditorSentPacket;
                richTextBox1.Enabled = false;
            }
        }

        private void EditorSentPacket(object sender, Packet e)
        {
            if (!IsSendingPackets)
                return;

            AppendText(richTextBox1, "s: " + e.ToString(), Color.Blue, true);
        }

        private void EditorReceivedPacket(object sender, Packet e)
        {
            if (!IsReceivingPackets)
                return;

            AppendText(richTextBox1, "r: " + e.ToString(), Color.Red, true);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            IsReceivingPackets = !IsReceivingPackets;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            IsSendingPackets = !IsSendingPackets;
        }

        public static byte[] StringToByteArray(string hex)
        {
            try
            {
                return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Invalid packet");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var line in richTextBox2.Lines)
            {
                try
                {
                    byte[] buffer = null;
                    var newline = line;

                    if (comboBox2.Text == "Auto")
                    {
                        bool outgoing = false;

                        if (line.StartsWith("r:"))
                            newline = line.Replace("r:", string.Empty);
                        else if (line.StartsWith("s:"))
                        {
                            newline = line.Replace("s:", string.Empty);
                            outgoing = true;
                        }

                        newline = newline.Trim();
                        buffer = StringToByteArray(newline.Replace(" ", string.Empty).Trim());
                        if (buffer != null && buffer.Length <= 0)
                            continue;

                        if (outgoing)
                            GameClient.InjectPacket<ServerPacket>(client, new Packet(buffer), true);
                        else
                            GameClient.InjectPacket<ClientPacket>(client, new Packet(buffer), true);

                        continue;
                    }

                    if (line.StartsWith("r:"))
                        newline = line.Replace("r:", string.Empty);
                    else if (line.StartsWith("s:"))
                        newline = line.Replace("s:", string.Empty);

                    buffer = StringToByteArray(newline.Replace(" ", string.Empty).Trim());
                    if (buffer != null && buffer.Length <= 0)
                        continue;



                    if (comboBox2.Text == "Client")
                        GameClient.InjectPacket<ClientPacket>(client, new Packet(buffer), true);
                    if (comboBox2.Text == "Server")
                        GameClient.InjectPacket<ServerPacket>(client, new Packet(buffer), true);
                }
                catch (InvalidOperationException)
                {
                    continue;
                }
            }
        }

        bool pausePackets = false;

        private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pausePackets = true;
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && pausePackets)
                pausePackets = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            client.Paused = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            client.Paused = false;
        }


        public static int OptionTable = (int)DAStaticPointers.OptionTable;

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x08] = (byte)((checkBox4.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x10] = (byte)((checkBox5.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x02] = (byte)((checkBox8.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x06] = (byte)((checkBox7.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);

        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            client[OptionTable + 0x04] = (byte)((checkBox6.Checked == true) ? 1 : 0);
            GameActions.Refresh(client, true);

        }

        private void button5_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                client.Utilities.CastSpell("Dark Seal", client);

            }).Start();

        }

        private void buttonDebugClear_Click(object sender, EventArgs e)
        {
            if (richTextBoxDebug.InvokeRequired)
            {
                richTextBoxDebug.Invoke(new System.Action(() => richTextBoxDebug.Clear()));
            }
            else
            {
                richTextBoxDebug.Clear();
            }
        }

        private void buttonDebugSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Text files (*.txt)|*.txt|Log files (*.log)|*.log|All files (*.*)|*.*";
                    saveDialog.DefaultExt = "txt";
                    saveDialog.FileName = $"DebugLog_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(saveDialog.FileName, richTextBoxDebug.Text);
                        MessageBox.Show($"Debug log saved to: {saveDialog.FileName}", "Save Complete", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving debug log: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDebugCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(richTextBoxDebug.Text))
                {
                    Clipboard.SetText(richTextBoxDebug.Text);
                    MessageBox.Show("Debug log copied to clipboard!", "Copy Complete", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Debug log is empty!", "Copy Warning", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", "Copy Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void AppendDebugLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] {message}\r\n";

            if (richTextBoxDebug.InvokeRequired)
            {
                richTextBoxDebug.Invoke(new System.Action(() =>
                {
                    richTextBoxDebug.AppendText(logEntry);
                    richTextBoxDebug.ScrollToCaret();
                }));
            }
            else
            {
                richTextBoxDebug.AppendText(logEntry);
                richTextBoxDebug.ScrollToCaret();
            }
        }

        public void SwitchToBottingStatesTab()
        {
            try
            {
                // Switch to the Botting States tab (tabPage1)
                if (tabControl1 != null && tabPage1 != null)
                {
                    tabControl1.SelectedTab = tabPage1;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine($"Error switching to Botting States tab: {ex.Message}");
            }
        }
    }
}
