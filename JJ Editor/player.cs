﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JJ_Editor
{
    public partial class Player : Form
    {
        string playerName = null;
        int health;
        byte[] healthBytes = new byte[4];
        byte[] male = { 0x0, 0x2, 0x4, 0x6, 0x8 };
        byte[] female = { 0x1, 0x3, 0x5, 0x7, 0x9 };
        string[] hairStyle = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        string[] hairColor = { "Grey", "Black", "Brown", "Dark Brown", "Light Brown", "Blond", "Light Blond", "Light", "Orange", "Velvet", "Lavender", "Blue", "Turquoise", "Green", "Yellow", "White" };
        string[] skinTone = { "Very Light", "Light", "Tan", "Dark", "Very Dark" };
        byte colorValue;
        byte originalGender;
        byte genderValue;
        byte styleValue;
        byte skinID;

        byte[] bytes;

        string pattern = "";
        string[] keyWords = { "FF" };

        Dictionary<string, int?> hcolor = new Dictionary<string, int?>();
        Dictionary<string, int?> hstyle = new Dictionary<string, int?>();
        Dictionary<string, int?> stone = new Dictionary<string, int?>();
        Dictionary<string, byte[]> dgender = new Dictionary<string, byte[]>();

        // String values
        string helmName = null;
        string chestName = null;
        string legsName = null;
        string feetName = null;

        string[] hotNames = { null, null, null, null, null, null, null, null, null, null };
        string[] slotNames = { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };
        
        // Offsets
        int helm = 0x38C;
        int chest = 0x398;
        int legs = 0x3A4;
        int feet = 0x3B0;

        int fHelm = 0x3C8;
        int fChest = 0x3D4;
        int fLegs = 0x3E0;
        int fFeet = 0x3EC;

        // 10 Hot slots
        public static int[] hots = { 0x80, 0x8C, 0x98, 0xA4, 0xB0, 0xBC, 0xC8, 0xD4, 0xE0, 0xEC };
        public static int[] hotsEnds = { 0x81, 0x8D, 0x99, 0xA5, 0xB1, 0xBD, 0xC9, 0xD5, 0xE1, 0xED };

        public static int[] hotsAmount = { 0x82, 0x8E, 0x9A, 0xA6, 0xB2, 0xBE, 0xCA, 0xD6, 0xE2, 0xEE };
        public static int[] hotsAmountEnds = { 0x83, 0x8F, 0x9B, 0xA7, 0xB3, 0xBF, 0xCB, 0xD7, 0xE3, 0xEF };

        // 36 Inventory slots
        public static int[] slots = { 0x1DC, 0x224, 0x26C, 0x2B4, 0x2FC, 0x344, 0x1E8, 0x230, 0x278, 0x2C0, 0x308, 0x350, 0x1F4, 0x23C, 0x284, 0x2CC, 0x314, 0x35C, 0x200, 0x248, 0x290, 0x2D8, 0x320, 0x368, 0x20C, 0x254, 0x29C, 0x2E4, 0x32C, 0x374, 0x218, 0x260, 0x2A8, 0x2F0, 0x338, 0x380 };
        public static int[] slotsEnds = { 0x1DD, 0x225, 0x26D, 0x2B5, 0x2FD, 0x345, 0x1E9, 0x231, 0x279, 0x2C1, 0x309, 0x351, 0x1F5, 0x23D, 0x285, 0x2CD, 0x315, 0x35D, 0x201, 0x249, 0x291, 0x2D9, 0x321, 0x369, 0x20D, 0x255, 0x29D, 0x2E5, 0x32D, 0x375, 0x219, 0x261, 0x2A9, 0x2F1, 0x339, 0x381 };

        public static int[] slotsAmount = { 0x1DE, 0x226, 0x26E, 0x2B6, 0x2FE, 0x346, 0x1EA, 0x232, 0x27A, 0x2C2, 0x30A, 0x352, 0x1F6, 0x23E, 0x286, 0x2CE, 0x316, 0x35E, 0x202, 0x24A, 0x292, 0x2DA, 0x322, 0x36A, 0x20E, 0x256, 0x29E, 0x2E6, 0x32E, 0x376, 0x21A, 0x262, 0x2AA, 0x2F2, 0x33A, 0x382 };
        public static int[] slotsAmountEnds = { 0x1DF, 0x227, 0x26F, 0x2B7, 0x2FF, 0x347, 0x1EB, 0x233, 0x27B, 0x2C3, 0x30B, 0x353, 0x1F7, 0x23F, 0x287, 0x2CF, 0x317, 0x35F, 0x203, 0x24B, 0x293, 0x2DB, 0x323, 0x36B, 0x20F, 0x257, 0x29F, 0x2E7, 0x32F, 0x377, 0x21B, 0x263, 0x2AB, 0x2F3, 0x33B, 0x383 };

        // Starting and ending offsets for 2 byte values
        int start;
        int end;

        // Stores all selected node tags
        ArrayList tags = new ArrayList();
        ArrayList englishIDs = new ArrayList();
        ArrayList englishNames = new ArrayList();

        // Item ID of converted hex value
        int itemID;
        int selectedAmount;
        int readItemAmount;

        string dir;
        string file;

        string englishJson = File.ReadAllText("english.json");
        int englishID;
        string englishName;

        bool autoSave = false;
        bool orderOption = false;

        string hexViewText;

        // For listview datasource
        //List<string> itemNames = new List<string>();

        class english
        {
            public List<englishItem> treasures { get; set; }
        }

        class englishItem
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        // Reads offsets for items
        void readOffset(int start, int end, string offset, TextBox textbox = null, TreeNode treeView = null, NumericUpDown itemAmount = null, BinaryReader br = null)
        {
            char[] characters = null;
            for (int i = start; i <= end; i++)
            {
                br.BaseStream.Position = i;
                offset += br.ReadByte().ToString("X2");
            }

            characters = offset.ToCharArray();
            HexToID.hexToID(characters);
            string hexID = new string(characters);

            bool isEmpty = false;
            if (hexID == "FFFF")
            {
                isEmpty = true;
            }
            else if (hexID.EndsWith("0"))
            {
                hexID = hexID.Substring(0, hexID.Length - 1);
            }

            int itemID = Convert.ToInt32(hexID, 16);

            english englishItems = JsonConvert.DeserializeObject<english>(englishJson);
            foreach (var item in englishItems.treasures)
            {
                englishIDs.Add(item.id);
                englishNames.Add(item.name);
                //itemNames.Add(item.name);
            }

            if (textbox != null && isEmpty)
            {
                textbox.Text = null;
            }

            treeView1.BeginUpdate();
            if (treeView != null && isEmpty)
            {
                treeView.Nodes.Add("[Empty]");
            }

            for (int i = 0; i < englishItems.treasures.Count; i++)
            {
                englishIDs.Add(englishItems.treasures[i].id);
                englishNames.Add(englishItems.treasures[i].name);

                if ((textbox != null))
                {

                    int eID = Convert.ToInt32(englishIDs[i]);
                    if (itemID == eID)
                    {
                        textbox.Text = englishNames[i].ToString();
                    }
                }

                if ((treeView != null))
                {
                    int eID = Convert.ToInt32(englishIDs[i]);
                    if (itemID == eID)
                    {
                        treeView.Nodes.Add(englishNames[i].ToString());
                    }
                }
                treeView1.EndUpdate();
            }
            if (itemAmount != null)
            {
                //itemAmount.Tag = itemID;
                tags.Add(itemID);
            }
        }

        private string playerFile;
        public Player(string dir, string file, bool autoSaveCheck, bool orderCheck)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Add nodes
            treeView1.Nodes.Add("Hot Slots");
            treeView1.Nodes.Add("Inventory");
            //treeView1.Nodes.Add("Potions");
            //treeView1.Nodes.Add("Armor");
            //treeView1.Nodes.Add("Potions");

            playerFile = dir + "/" + file;
            bytes = File.ReadAllBytes(playerFile);

            if (autoSaveCheck == true)
            {
                timer.Enabled = true;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = 20000; // 20 seconds
                timer.Start();
            }
            else if (autoSaveCheck == false)
            {
                timer.Enabled = false;
            }
            if (orderCheck == true)
            {
                orderOption = true;
            }
            else if (orderCheck = false)
            {
                orderOption = false;
            }

            readPlayerFile();
        }      

        private void readPlayerFile()
        {
            using (var br = new BinaryReader(File.OpenRead(playerFile)))
            {
                // Read player name
                for (int i = 0x58; i <= 0x66; i++)
                {
                    br.BaseStream.Position = i;
                    playerName += br.ReadChar().ToString();
                }
                nameText.Text = playerName;
                nameText.MaxLength = 15;

                // Read health
                int a = 0;
                for (int i = 0x538; i <= 0x53B; i++)
                {
                    br.BaseStream.Position = i;
                    //health += br.ReadByte().ToString("X2");
                    //health += br.ReadUInt16().ToString();

                    healthBytes[a] = br.ReadByte();
                    a++;
                }
                float hFloat = BitConverter.ToSingle(healthBytes, 0);
                health = (int)hFloat;
                if (health > 0xFFFF || health < 0)
                {
                    health = 0xFFFF;
                }
                healthBox.Value = health;

                if (healthBox.Value == 0xFFFF)
                {
                    NaN.Checked = true;
                }

                // Read characteristics
                // Hair color
                int b = 0x1;
                foreach(string color in hairColor)
                {
                    hcolor.Add(color, b);
                    b++;
                }
                colorBox.DataSource = new BindingSource(hcolor, null);
                colorBox.DisplayMember = "Key";
                colorBox.ValueMember = "Value";

                br.BaseStream.Position = 0x74;
                byte cValue = br.ReadByte();
                foreach (var value in hcolor)
                {
                    if ((Convert.ToByte(value.Value) << 4) == cValue)
                    {
                        colorBox.SelectedValue = value.Value;
                        colorBox.SelectedText = value.Key;
                    }
                }

                // Skin tone
                int d = 0x1;
                foreach (string tone in skinTone)
                {
                    stone.Add(tone, d);
                    d++;
                }
                toneBox.DataSource = new BindingSource(stone, null);
                toneBox.DisplayMember = "Key";
                toneBox.ValueMember = "Value";

                br.BaseStream.Position = 0x75;

                // Gender
                dgender.Add("Male", male);
                dgender.Add("Female", female);
                
                genderBox.ValueMember = "Value";
                genderBox.DisplayMember = "Key";
                genderBox.DataSource = new BindingSource(dgender, null);

                br.BaseStream.Position = 0x75;
                byte gValue = br.ReadByte();

                foreach (var value in dgender)
                {
                    foreach(byte v in value.Value)
                    {                      
                        if ((Convert.ToByte(v)) == (gValue >> 4))
                        {
                            genderBox.SelectedValue = value.Value;
                            genderBox.SelectedText = value.Key;
                            originalGender = v;
                        }

                        // Skin tone
                        switch(gValue >> 4)
                        {
                            case 0: // Male
                                toneBox.SelectedValue = 1;
                                skinID = 0x0;
                                break;
                            case 1: // Female
                                toneBox.SelectedValue = 1;
                                skinID = 0x1;
                                break;
                            case 2: // Male
                                toneBox.SelectedValue = 2;
                                skinID = 0x2;
                                break;
                            case 3: // Female
                                toneBox.SelectedValue = 2;
                                skinID = 0x3;
                                break;
                            case 4: // Male
                                toneBox.SelectedValue = 3;
                                skinID = 0x4;
                                break;
                            case 5: // Female
                                toneBox.SelectedValue = 3;
                                skinID = 0x5;
                                break;
                            case 6: // Male
                                toneBox.SelectedValue = 4;
                                skinID = 0x6;
                                break;
                            case 7: // Female
                                toneBox.SelectedValue = 4;
                                skinID = 0x7;
                                break;
                            case 8: // Male
                                toneBox.SelectedValue = 5;
                                skinID = 0x8;
                                break;
                            case 9: // Female
                                toneBox.SelectedValue = 5;
                                skinID = 0x9;
                                break;
                        }
                    }
                }

                // Hair style
                int c = 0x1;
                foreach (string style in hairStyle)
                {
                    hstyle.Add(style, c);
                    c++;
                }
                styleBox.DataSource = new BindingSource(hstyle, null);
                styleBox.DisplayMember = "Key";
                styleBox.ValueMember = "Value";

                br.BaseStream.Position = 0x75;
                byte sValue = br.ReadByte();
                foreach (var value in hstyle)
                {
                    //MessageBox.Show((Convert.ToByte(value.Value) & 0x0F).ToString("X2") + " and the value you want: " + (sValue & 0x0F).ToString("X2"));
                    if ((Convert.ToByte(value.Value) & 0x0F) == (sValue & 0x0F))
                    {
                        styleBox.SelectedValue = value.Value;
                        styleBox.SelectedText = value.Key;
                    }
                }
            
                // Helmet
                readOffset(helm, 0x38D, helmName, helmText, null, null, br);
                // Chest
                readOffset(chest, 0x399, chestName, chestText, null, null, br);
                // Legs
                readOffset(legs, 0x3A5, legsName, legsText, null, null, br);
                // Feet
                readOffset(feet, 0x3B1, feetName, feetText, null, null, br);

                // Add items to treeview
                var treeHots = treeView1.Nodes[0];
                for (var i = 0; i < hots.Length; i++)
                {
                    readOffset(hots[i], hotsEnds[i], hotNames[i], null, treeHots, null, br);
                    readOffset(hotsAmount[i], hotsAmountEnds[i], hotNames[i], null, null, numericUpDown1, br);
                    //treeHots.Tag = hots[i];

                    if (numericUpDown1.Value > numericUpDown1.Maximum)
                    {
                        numericUpDown1.Value = 0xFFFF;
                    }
                    //int[] tag = { hots[i], hotsAmount[i] };
                    treeView1.Nodes[0].Nodes[i].Tag = tags[i];

                }
                tags.Clear();

                var treeSlots = treeView1.Nodes[1];
                for (var i = 0; i < slots.Length; i++)
                {
                    readOffset(slots[i], slotsEnds[i], slotNames[i], null, treeSlots, null, br);
                    readOffset(slotsAmount[i], slotsAmountEnds[i], null, null, null, numericUpDown1, br);
                    //treeSlots.Tag = slots[i];

                    if (numericUpDown1.Value > numericUpDown1.Maximum)
                    {
                        numericUpDown1.Value = 0xFFFF;
                    }
                    //int[] tag = { slots[i], slotsAmount[i] };
                    treeView1.Nodes[1].Nodes[i].Tag = tags[i];
                }
                tags.Clear();
            }

            readHex();
        }

        private void readHex()
        {
            int bytesPerLine = 16;
            string dump = bytes.Select((c, i) => new { Char = c, Chunk = i / bytesPerLine })
                .GroupBy(c => c.Chunk)
                .Select(g => g.Select(c => String.Format("{0:X2} ", c.Char))
                .Aggregate((s, i) => s + i))
                .Select((s, i) => String.Format("{0:d6}: {1}", i * bytesPerLine, s))
                .Aggregate("", (s, i) => s + i + Environment.NewLine);
            hexBox.Text = dump;

            // Alternating row colors
            for (int i = 0; i < hexBox.Lines.Length; i++)
            {
                int first = hexBox.GetFirstCharIndexFromLine(i);
                hexBox.Select(first, hexBox.Lines[i].Length);
                hexBox.SelectionBackColor = (i % 2 == 0) ? System.Drawing.ColorTranslator.FromHtml("#efefef") : Color.White;
            }

            // Syntax highlighting
            foreach (var a in keyWords)
            {
                pattern += a + "|";
            }

            Regex R = new Regex(pattern);
            int index = hexBox.SelectionStart;

            foreach (Match m in R.Matches(hexBox.Text))
            {
                hexBox.Select(m.Index, m.Value.Length);
                hexBox.SelectionColor = Color.Green;
                hexBox.SelectionStart = index;
            }
            hexBox.SelectionColor = Color.Black;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            writeToPlayer();
        }

        private void itemBox_TextChanged_1(object sender, EventArgs e)
        {
            english englishItems = JsonConvert.DeserializeObject<english>(englishJson);

            itemList.BeginUpdate();
            itemList.Items.Clear();
            if (itemBox.Text != null)
            {
                for (int i = 1; i < englishItems.treasures.Count; i++)
                {
                    if (englishNames[i].ToString().Contains(itemBox.Text))
                    {
                        itemList.Items.Add(englishNames[i].ToString());
                        if (orderOption == true)
                        {
                            itemList.Sorted = true;
                        }
                    }
                }
                itemList.EndUpdate();
            }
            else
            {
                itemList.Items.Clear();
            }
        }

        private void Player_Load(object sender, EventArgs e)
        {
            
        }

        private void NaN_CheckedChanged(object sender, EventArgs e)
        {
            if (NaN.CheckState == CheckState.Checked)
            {
                healthBox.Value = 0xFFFF;
                healthBox.Enabled = false;
            }
            else
            {
                healthBox.Value = health;
                healthBox.Enabled = true;
            }
        }

        private void checkBoxFancy_CheckedChanged(object sender, EventArgs e)
        {
            using (var br = new BinaryReader(File.OpenRead(playerFile)))
            {
                if (checkBoxFancy.Checked == true)
                {
                    helm = fHelm;
                    chest = fChest;
                    legs = fLegs;
                    feet = fFeet;

                    // Helmet
                    readOffset(helm, helm + 1, helmName, helmText, null, null, br);
                    // Chest
                    readOffset(chest, chest + 1, chestName, chestText, null, null, br);
                    // Legs
                    readOffset(legs, legs + 1, legsName, legsText, null, null, br);
                    // Feet
                    readOffset(feet, feet + 1, feetName, feetText, null, null, br);
                }
                else
                {
                    helm = 0x38C;
                    chest = 0x398;
                    legs = 0x3A4;
                    feet = 0x3B0;

                    // Helmet
                    readOffset(helm, 0x38D, helmName, helmText, null, null, br);
                    // Chest
                    readOffset(chest, 0x399, chestName, chestText, null, null, br);
                    // Legs
                    readOffset(legs, 0x3A5, legsName, legsText, null, null, br);
                    // Feet
                    readOffset(feet, 0x3B1, feetName, feetText, null, null, br);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                numericUpDown1.Value = 0xFFFF;
                numericUpDown1.Enabled = false;
            }
            else
            {
                numericUpDown1.Value = Convert.ToInt32(treeView1.SelectedNode.Tag);              
                numericUpDown1.Enabled = true;
            }
        }

        System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();

        private void timer2_Tick(object sender, EventArgs e)
        {
            saveToolStripMenuItem2.Enabled = true;
            timer2.Stop();
        }

        void writeToPlayer()
        {
            timer2.Interval = 2000; 
            timer2.Tick += timer2_Tick;
            timer2.Start();
            saveToolStripMenuItem2.Enabled = false;

            main m = (main)this.MdiParent;
            var s = m.statusLabelPlayer;
            s.Text = "Player file saved!";

            // Write all changed values to file
            using (var bw = new BinaryWriter(File.OpenWrite(playerFile)))
            {
                playerName = nameText.Text;

                // Clear all name characters
                for (int i = 0x57; i <= 0x66; i++)
                {
                    bw.BaseStream.Position = i;
                    bw.Write(0x00);
                }
                // Write player name
                bw.BaseStream.Position = 0x57;
                bw.Write(playerName);

                // Write health
                bw.BaseStream.Position = 0x538;
                if (healthBox.Value == 0xFFFF)
                {
                    byte[] invincible = { 0x00, 0x00, 0xFF, 0xFF };
                    bw.Write(invincible);
                }
                else
                {
                    health = (int)healthBox.Value;
                    float hFloat = Convert.ToSingle(health);
                    byte[] healthBytes = BitConverter.GetBytes(hFloat);
                    bw.Write(healthBytes);
                }

                // Write characteristics
                // Hair color
                byte shiftValue = 4;
                bw.BaseStream.Position = 0x74;
                colorValue = Convert.ToByte((colorBox.SelectedItem as dynamic).Value);
                bw.Write((byte)(colorValue << shiftValue));

                // Gender 
                // 0 2 4 6 8 male
                // 1 3 5 6 7 female
                bw.BaseStream.Position = 0x75;

                switch(toneBox.SelectedValue)
                {
                    case 1: // Very Light
                        if(genderBox.SelectedValue == male)
                        {
                            skinID = 0;
                        }
                        else
                        {
                            skinID = 1;
                        }
                        break;
                    case 2: // Light
                        if (genderBox.SelectedValue == male)
                        {
                            skinID = 2;
                        }
                        else
                        {
                            skinID = 3;
                        }
                        break;
                    case 3: // Tan
                        if (genderBox.SelectedValue == male)
                        {
                            skinID = 4;
                        }
                        else
                        {
                            skinID = 5;
                        }
                        break;
                    case 4: // Dark
                        if (genderBox.SelectedValue == male)
                        {
                            skinID = 6;
                        }
                        else
                        {
                            skinID = 7;
                        }
                        break;
                    case 5: // very Dark
                        if (genderBox.SelectedValue == male)
                        {
                            skinID = 8;
                        }
                        else
                        {
                            skinID = 9;
                        }
                        break;
                }

                //bw.Write((byte)(genderValue << shiftValue));

                // Hair style
                bw.BaseStream.Position = 0x75;
                styleValue = Convert.ToByte((styleBox.SelectedItem as dynamic).Value);
                bw.Write((byte)((skinID << shiftValue)) | (styleValue & 0x0F));

                readHex();

                // Write armor values
                void writeOffset(int start, TextBox textbox = null, NumericUpDown itemAmount = null)
                {
                    int id = 65535;
                    bw.BaseStream.Position = start;

                    english englishItems = JsonConvert.DeserializeObject<english>(englishJson);

                    if (textbox != null)
                    {
                        if (textbox.Text == "")
                        {
                            id = 65535;
                        }
                        else
                        {
                            foreach (var item in englishItems.treasures)
                            {
                                englishID = item.id;
                                englishName = item.name;

                                if (textbox.Text == englishName)
                                {
                                    id = englishID;
                                    //id = int.Parse(textbox.Text);
                                }
                            }
                        }
                        IDToHex.idToHex(id);
                        bw.Write(id);
                    }
                    else if (textbox == null)
                    {
                        id = int.Parse(numericUpDown1.Value.ToString());
                        IDToHex.idToHex(id);
                        bw.Write(id);
                    }
                    bw.BaseStream.Position = start + 6; // For toggle render
                    bw.Write(0x00);
                }

                writeOffset(helm, helmText);
                writeOffset(chest, chestText);
                writeOffset(legs, legsText);
                writeOffset(feet, feetText);

                // Write treeview values
                if (treeView1.SelectedNode == null)
                {
                    return;
                }
                else
                {
                    if (itemBox.Text == "")
                    {
                        treeView1.SelectedNode.Text = "[Empty]";
                        numericUpDown1.Value = 0;
                    }
                    else if (itemBox.Text == "0")
                    {
                        treeView1.SelectedNode.Text = "[Empty]";
                        numericUpDown1.Value = 0;
                    }
                    treeView1.SelectedNode.Text = itemBox.Text;

                    // Invenory items
                    for (var i = 0; i < hots.Length; i++)
                    {
                        int[] tag = { hots[i], hotsAmount[i] };
                        treeView1.Nodes[0].Nodes[i].Tag = tag[0]; //hots[i];
                    }
                    for (var i = 0; i < slots.Length; i++)
                    {
                        int[] tag = { slots[i], slotsAmount[i] };
                        treeView1.Nodes[1].Nodes[i].Tag = tag[0];
                    }

                    int selectedItem = Convert.ToInt32(treeView1.SelectedNode.Tag);
                    writeOffset(selectedItem, itemBox);

                    // Item amounts
                    for (var i = 0; i < hots.Length; i++)
                    {
                        int[] tag = { hots[i], hotsAmount[i] };
                        treeView1.Nodes[0].Nodes[i].Tag = tag[1]; //hots[i];
                    }
                    for (var i = 0; i < slots.Length; i++)
                    {
                        int[] tag = { slots[i], slotsAmount[i] };
                        treeView1.Nodes[1].Nodes[i].Tag = tag[1];
                    }
                    selectedAmount = Convert.ToInt32(treeView1.SelectedNode.Tag);
                    writeOffset(selectedAmount, null, numericUpDown1);
                }
            }

            // Read to itemAmount again
            using (var br = new BinaryReader(File.OpenRead(playerFile)))
            {
                var treeHots = treeView1.Nodes[0];
                for (var i = 0; i < hots.Length; i++)
                {
                    //readOffset(hots[i], hotsEnds[i], hotNames[i], null, treeHots, null, br);
                    readOffset(hotsAmount[i], hotsAmountEnds[i], hotNames[i], null, null, numericUpDown1, br);
                    //treeHots.Tag = hots[i];

                    if (numericUpDown1.Value > numericUpDown1.Maximum)
                    {
                        numericUpDown1.Value = 0xFFFF;
                    }
                    //int[] tag = { hots[i], hotsAmount[i] };
                    treeView1.Nodes[0].Nodes[i].Tag = tags[i];
                }
                tags.Clear();
                var treeSlots = treeView1.Nodes[1];
                for (var i = 0; i < slots.Length; i++)
                {
                    //readOffset(slots[i], slotsEnds[i], slotNames[i], null, treeSlots, null, br);
                    readOffset(slotsAmount[i], slotsAmountEnds[i], null, null, null, numericUpDown1, br);
                    //treeSlots.Tag = slots[i];

                    if (numericUpDown1.Value > numericUpDown1.Maximum)
                    {
                        numericUpDown1.Value = 0xFFFF;
                    }
                    //int[] tag = { slots[i], slotsAmount[i] };
                    treeView1.Nodes[1].Nodes[i].Tag = tags[i];
                }
                tags.Clear();
            }
        }

        // Save menu item
        void saveToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            writeToPlayer();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            itemBox.Enabled = true;
            numericUpDown1.Enabled = true;
            checkBox1.Enabled = true;
            itemList.Enabled = true;
            saveButton.Enabled = true;

            numericUpDown1.Value = Convert.ToInt32(treeView1.SelectedNode.Tag);

            itemBox.Text = treeView1.SelectedNode.Text;
            if (treeView1.SelectedNode.Text == "[Empty]")
            {
                itemBox.Text = "";
            }

            if (numericUpDown1.Value == 0xFFFF)
            {
                checkBox1.Checked = true;
                numericUpDown1.Enabled = false;
            }
            else
            {
                checkBox1.Checked = false; ;
                numericUpDown1.Enabled = true;
            }
            if (treeView1.SelectedNode.Text == "Hot Slots" ||
                treeView1.SelectedNode.Text == "Inventory")
            {
                itemBox.Text = "";
                itemBox.Enabled = false;
                numericUpDown1.Enabled = false;
                checkBox1.Enabled = false;
                itemList.Enabled = false;
                itemList.Items.Clear();
                saveButton.Enabled = false;
            }
        }

        private void itemList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemList.SelectedItem != null)
            {
                itemBox.Text = itemList.SelectedItem.ToString();
            }
            treeView1.SelectedNode.Text = itemBox.Text;
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            main m = (main)this.MdiParent;
            var s = m.statusLabelPlayer;
            s.Text = Path.GetFileName(playerFile) + " closed";
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            writeToPlayer();
        }

        private void Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            main m = (main)this.MdiParent;
            var s = m.statusLabelPlayer;
            s.Text = Path.GetFileName(playerFile) + " closed";
        }

        /**private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer2.Interval = 2000;
            timer2.Tick += timer2_Tick;
            timer2.Start();

            // Clear all fields
            nameText.Clear();
            healthBox.Value = 0;
            helmText.Clear();
            chestText.Clear();
            legsText.Clear();
            feetText.Clear();
            itemBox.Clear();
            itemList.ClearSelected();
            numericUpDown1.Value = 0;
            treeView1.Nodes[0].Nodes.Clear();
            treeView1.Nodes[1].Nodes.Clear();

            readPlayerFile();
        }**/
    }
}
