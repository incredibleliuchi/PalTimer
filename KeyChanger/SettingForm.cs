﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace KeyChanger
{
    public partial class SettingForm : Form
    {
        private KC kc;
        private MainForm np = null;
        public SettingForm(MainForm p)
        {
            this.np = p;
            InitializeComponent();
            kc = new KC("");
            InitFromFile();
            cbEnable.Checked = kc.IsEnable;
            Showi();
        }

        public int CurrentKeyCode
        {
            get
            {
                if (np != null)
                {
                    return np.CurrentKeyCode;
                }
                return -1;
            }
        }

        private void InitFromFile()
        {
            if (File.Exists("keychange.txt"))
            {
                string keychangestr = "";
                using (FileStream fileStream = new FileStream("keychange.txt", FileMode.Open))
                {
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        keychangestr = streamReader.ReadToEnd();
                    }
                }
                kc = new KC(keychangestr);
            }
        }

        private void Showi()
        {
            lstAll.Items.Clear();
            foreach (KeyValuePair<int, int> kv in kc.KeyMap)
            {
                lstAll.Items.Add(new klstitem(kv.Key, kv.Value));
            }
        }

        private void Save()
        {
            kc.IsEnable = cbEnable.Checked;
            try
            {
                if (File.Exists("keychange.txt"))
                {
                    File.Delete("keychange.txt");
                }
                using (FileStream fileStream = new FileStream("keychange.txt", FileMode.OpenOrCreate))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default))
                    {
                        streamWriter.Write(kc.ToString());
                        streamWriter.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "保存失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool IsSet = false;
        public int ori;
        public int n;

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstAll.SelectedIndex >= 0)
            {
                klstitem li = (klstitem)(lstAll.Items[lstAll.SelectedIndex]);
                kc.Remove(li.ori);
                Showi();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            KeyAddForm ka = new KeyAddForm(this);
            IsSet = false;
            ka.ShowDialog(this);
            if (IsSet)
            {
                kc.Add(ori, n);
                Showi();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Save();
            this.Close();
        }
    }
    public class klstitem
    {
        public int ori;
        public int n;

        public klstitem(int ori, int n)
        {
            this.ori = ori;
            this.n = n;
        }

        public override string ToString()
        {
            return KC.KeyCode2Name(ori) + " -> " + KC.KeyCode2Name(n);
        }
    }
}
