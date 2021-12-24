﻿using HFrame.ENT;
using HFrame.EX;
using HFrame.OS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Pal98Timer
{
    public class 古剑2官方 : TimerCore
    {

        private GameObjectGuJian2 GameObj = new GameObjectGuJian2();
        private bool IsAllRun = true;
        private string GMD5 = "none";
        public IntPtr PalHandle;
        public IntPtr GameWindowHandle = IntPtr.Zero;
        private int PID = -1;
        private Process PalProcess;
        private int CheckInterval = 70;
        private bool _HasGameStart = false;
        private bool _IsFirstStarted = false;
        private int ExeBaseAddr = -1;
        private PTimer MT = new PTimer();
        private PTimer ST = new PTimer();
        private PTimer LT = new PTimer();
        private DateTime InBattleTime;
        private DateTime OutBattleTime;
        public TimeSpan BattleLong = new TimeSpan(0);
        private int HandPauseCount = 0;
        private bool IsPause = false;
        private bool IsUIPause = false;

        private bool IsInBattle = false;
        private bool IsDoMoreEndBattle = true;

        private string SelectSword = "";
        private string EndName = "";

        private string cryerror = "";

        private TimeSpan WillClear;
        private TimeSpan BestClear;
        //private long PALBaseAddr;
        public 古剑2官方() : base()
        {
            try
            {
                BestFile = "bestGuJian2.txt";
                this.OnCurrentStepChangedInner = delegate (int cstep)
                {
                    try
                    {
                        if (cstep > 0)
                        {
                            //CheckPoints[cstep].Current = MT.CurrentTSOnly;
                            long cha = CheckPoints[cstep - 1].GetCHA() * 1000 * 10000;
                            if ((BestClear.Ticks + cha) <= 0)
                            {
                                WillClear = BestClear.Add(new TimeSpan(0));
                            }
                            else
                            {
                                WillClear = BestClear.Add(new TimeSpan(cha));
                            }
                        }
                    }
                    catch
                    { }
                };
            }
            catch
            { }
        }
        private string WillAppendNamedBattle = "";
        public override string GetAAction()
        {
            if (WillAppendNamedBattle == "")
            {
                return "";
            }
            else
            {
                string res = WillAppendNamedBattle;
                WillAppendNamedBattle = "";
                return res;
            }
        }
        public override bool IsShowC()
        {
            return false;
        }

        public override string GetCriticalError()
        {
            if (cryerror == "")
            {
                return "";
            }
            else
            {
                string tmp = cryerror;
                cryerror = "";
                return tmp;
            }
        }

        public override string GetGameVersion()
        {
            if (PID != -1)
            {
                return "古剑奇谭2官方版";
            }
            else
            {
                return "等待游戏运行";
            }
        }

        public override string GetMainWatch()
        {
            return MT.ToString();
        }

        public override string GetMoreInfo()
        {
            int m = GameObj.Money;
            int tong = m % 100;
            m = m / 100;
            int yin = m % 100;
            m = m / 100;

            return m + "金 " + yin.ToString().PadLeft(2, '0') + "银 " + tong.ToString().PadLeft(2, '0') + "铜";
        }

        public override string GetPointEnd()
        {
            return "预计通关  " + TimeSpanToStringLite(WillClear);
        }

        public override string GetSecondWatch()
        {
            if (ST.CurrentTSOnly.Ticks == 0)
            {
                return "";
            }
            return ST.ToString();
        }

        public override string GetSmallWatch()
        {
            return BattleLong.TotalSeconds.ToString("F2") + "s";
        }

        public override void InitCheckPoints()
        {
            LoadBest();
            _CurrentStep = -1;
            Data = new HObj();
            CheckPoints = new List<CheckPoint>();
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("太乙神兵", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.太乙神兵 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("慧明", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.慧明 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("金砖", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.金砖 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("灵虚", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.灵虚 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("偃甲将军", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.偃甲将军 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("雩风", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.雩风 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("尸兽厌火", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.尸兽厌火 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("安尼瓦尔", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.安尼瓦尔 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("流月守卫头领", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.流月城守卫头领 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("远古石兽", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.远古石兽 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("火龙伏英", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.火龙伏英 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP < 3000))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("怒霜天君", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.怒霜天君 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("温留", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.温留 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("蜃精玉怜", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.蜃精玉怜 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("初七", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.初七 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("程廷钧", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.程廷钧 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("魔化华月", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.魔化华月 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("沈夜变身", new TimeSpan(0, 6, 59)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.沈夜变身 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });
            CheckPoints.Add(new CheckPoint(CheckPoints.Count, GetBest("通关", new TimeSpan(2, 0, 38)))
            {
                Check = delegate ()
                {
                    if (GameObj.BattleID == GameObjectGuJian2.EBattle.砺罂 && (GameObj.EnemyCount <= 0 || GameObj.TotalEnemyHP <= 0))
                    {
                        WillAppendNamedBattle = GameObj.BattleID.ToString() + ":" + BattleLong.TotalSeconds.ToString("F2");
                        return true;
                    }
                    return false;
                }
            });

            WillClear = GetBest("通关", new TimeSpan(2, 0, 38)).BestTS;
            BestClear = GetBest("通关", new TimeSpan(2, 0, 38)).BestTS;
        }

        private Button btnPause;
        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (!IsUIPause)
            {
                HandPauseCount++;
            }
            SetUIPause(!IsUIPause);
            if (HandPauseCount > 0)
            {
                btnPause.Text = "暂停 " + HandPauseCount;
            }
        }
        private void SetUIPause(bool isp)
        {
            IsUIPause = isp;
            if (IsUIPause)
            {
                btnPause.ForeColor = Color.Red;
            }
            else
            {
                btnPause.ForeColor = Color.White;
            }
        }
        public string GetRStr()
        {
            HObj exdata = new HObj();
            exdata["Current"] = MT.ToString();
            exdata["Idle"] = ST.ToString();
            exdata["Lite"] = LT.ToString();
            exdata["Step"] = CurrentStep;
            exdata["OSTime"] = DateTime.Now.Ticks.ToString();
            exdata["GMD5"] = GMD5;
            HObj cps = new HObj();
            foreach (CheckPoint c in CheckPoints)
            {
                HObj cur = new HObj();
                cur["name"] = c.Name;
                cur["des"] = c.NickName;
                cur["time"] = TItem.TimeSpanToString(c.Current);
                cps.Add(cur);
            }
            exdata["CheckPoints"] = cps;

            return exdata.ToJson();
        }
        public override void InitUI(NewForm form)
        {
            btnPause = form.NewMenuButton(0);
            btnPause.Text = "暂停";
            btnPause.Click += BtnPause_Click;

            var btnExportCurrent = form.NewMenuItem();
            btnExportCurrent.Text = "导出本次成绩";
            btnExportCurrent.Click += delegate (object sender, EventArgs e) {
                DateTime now = DateTime.Now;
                string filename = now.ToString("GuJian2_yyyyMMddHHmmss");
                string ext = GetRStr();

                try
                {
                    using (FileStream fileStream = new FileStream(filename + ".txt", FileMode.Create))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default))
                        {
                            streamWriter.Write(ext);
                            //streamWriter.Flush();
                        }
                    }
                    form.Success("已将此次成绩保存至" + filename + ".txt");
                }
                catch (Exception ex)
                {
                    form.Error("保存失败：" + ex.Message);
                }
            };

            var btnSetCurrentToBest = form.NewMenuItem();
            btnSetCurrentToBest.Text = "设置本次成绩为最佳";
            btnSetCurrentToBest.Click += delegate (object sender, EventArgs e) {
                DateTime now = DateTime.Now;
                string filename = now.ToString("GuJian2_yyyyMMddHHmmss");
                string ext = GetRStr();
                try
                {
                    if (File.Exists("bestGuJian2.txt"))
                    {
                        File.Move("bestGuJian2.txt", "best" + filename + ".txt");
                    }
                    using (FileStream fileStream = new FileStream("bestGuJian2.txt", FileMode.Create))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default))
                        {
                            streamWriter.Write(ext);
                            //streamWriter.Flush();
                        }
                    }
                    if (form.Confirm("保存成功，确定要重置计时器么？"))
                    {
                        form._ResetAll();
                    }
                }
                catch (Exception ex)
                {
                    form.Error("保存失败：" + ex.Message);
                }
            };
        }

        public override void OnFunctionKey(int FunNo, NewForm form)
        {
            switch (FunNo)
            {
                case 9:
                    BtnPause_Click(null, null);
                    break;
                case 12:
                    DebugForm df = new DebugForm();
                    df.ShowData(GameObj);
                    df.Show();
                    break;
            }
        }

        public override void Reset()
        {
            HandPauseCount = 0;
            HasAlertMutiPal = false;
            ST.Stop();
            _IsFirstStarted = false;
            BattleLong = new TimeSpan(0);
            InitCheckPoints();
            MT.Reset();
            ST.Reset();
            btnPause.Text = "暂停";
        }

        public override void SetTS(TimeSpan ts)
        {
            MT.SetTS(ts);
            CheckPoints[CurrentStep].Current = ts;
        }

        public override void Unload()
        {
            IsAllRun = false;
        }

        private string GetGameFilePath(string fn)
        {
            string palpath = PalProcess.MainModule.FileName;
            string[] spli = palpath.Split('\\');
            spli[spli.Length - 1] = fn;
            palpath = "";
            foreach (string s in spli)
            {
                palpath += s + "\\";
            }
            if (palpath != "")
            {
                palpath = palpath.Substring(0, palpath.Length - 1);
            }
            return palpath;
        }
        private void CalcPalMD5()
        {
            try
            {
                string dllmd5 = GetFileMD5(GetGameFilePath("GuJian2.exe"));
                GMD5 = dllmd5;
            }
            catch
            {
                GMD5 = "none";
            }
        }
        private bool HasAlertMutiPal = false;
        private void JudgePause()
        {
            if (GameObj.IsLoading)
            {
                IsPause = true;
                return;
            }
            if (IsUIPause)
            {
                IsPause = true;
                return;
            }
            IsPause = !this.IsGameWindowFocus();
        }
        private bool IsGameWindowFocus()
        {
            IntPtr hWnd = User32.GetForegroundWindow();    //获取活动窗口句柄  
            int calcID = 0;    //进程ID  
            int calcTD = 0;    //线程ID  
            calcTD = User32.GetWindowThreadProcessId(hWnd, out calcID);
            if (calcID == PID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool GetPalHandle()
        {
            Process[] res = Process.GetProcessesByName("GuJian2");
            if (res.Length > 1)
            {
                if (!HasAlertMutiPal)
                {
                    cryerror = "检测到多个GuJian2.exe进程，请关闭其他的，只保留一个！";
                    HasAlertMutiPal = true;
                }
                return false;
            }

            HasAlertMutiPal = false;
            if (res.Length > 0)
            {
                PalProcess = res[0];
                GameWindowHandle = res[0].MainWindowHandle;
                if (PID != PalProcess.Id || ExeBaseAddr<=0)
                {
                    foreach (ProcessModule pm in PalProcess.Modules)
                    {
                        if (pm.ModuleName == "TRGame.vPlugin")
                        {
                            ExeBaseAddr = pm.BaseAddress.ToInt32();
                            break;
                        }
                    }
                }
                PID = PalProcess.Id;
                PalHandle = new IntPtr(Kernel32.OpenProcess(0x1F0FFF, false, PID));
                
                CalcPalMD5();

                return true;
            }
            else
            {
                PalHandle = IntPtr.Zero;
                GameWindowHandle = IntPtr.Zero;
                PalProcess = null;
                PID = -1;
                GMD5 = "none";
                //PALBaseAddr = -1;
                return false;
            }
        }
        private bool HasStartGame()
        {
            if (!_HasGameStart)
            {
                if (GameObj.CanControl)
                {
                    _HasGameStart = true;
                    if (IsPause)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (IsPause)
                {
                    return false;
                }
                return true;
            }
        }
        private void Checking()
        {
            if (CurrentStep < 0 && CheckPoints.Count > 0)
            {
                CheckPoints[0].IsBegin = true;
                CurrentStep = 0;
            }

            if (CurrentStep < CheckPoints.Count)
            {
                CheckPoints[CurrentStep].Current = MT.CurrentTSOnly;
                if (CheckPoints[CurrentStep].Check())
                {
                    CheckPoints[CurrentStep].Current = new TimeSpan(MT.CurrentTSOnly.Ticks);
                    CheckPoints[CurrentStep].IsEnd = true;
                    //CurrentStep++;
                    int nextstep = CurrentStep + 1;
                    if (nextstep >= CheckPoints.Count)
                    {
                        OnLastCheckPointEnd();
                    }
                    else
                    {
                        CheckPoints[nextstep].IsBegin = true;
                    }
                    CurrentStep = nextstep;
                }
            }
            else
            {
                OnLastCheckPointEnd();
            }
        }
        private void OnLastCheckPointEnd()
        {
            MT.Stop();
        }
        public override void Start()
        {
            FormEx.Run(delegate () {
                while (IsAllRun)
                {
                    try
                    {
                        if (GetPalHandle())
                        {

                            JudgePause();
                            try
                            {
                                FlushGameObject();
                            }
                            catch (Exception ex)
                            {
                            }


                            try
                            {
                                if (GameObj.IsInBattle)
                                {
                                    if (!IsInBattle)
                                    {
                                        BattleBegin();
                                    }
                                    IsInBattle = true;
                                    IsDoMoreEndBattle = true;
                                    Battling();
                                }
                                else
                                {
                                    if (!IsDoMoreEndBattle)
                                    {
                                        BattleEndMore();
                                        IsDoMoreEndBattle = true;
                                    }
                                    if (IsInBattle)
                                    {
                                        BattleEnd();
                                        IsDoMoreEndBattle = false;
                                    }
                                    IsInBattle = false;
                                }
                            }
                            catch { }

                            if (HasStartGame())
                            {
                                ST.Stop();
                                if (!_IsFirstStarted)
                                {
                                    _IsFirstStarted = true;
                                }
                                MT.Start();
                                Checking();
                            }
                            else
                            {
                                MT.Stop();
                            }
                        }
                        else
                        {
                            _HasGameStart = false;
                            MT.Stop();

                            if (_IsFirstStarted)
                            {
                                ST.Start();
                            }
                        }
                    }
                    catch
                    { }
                    Thread.Sleep(CheckInterval);
                }
            });
        }
        private void BattleBegin()
        {
            BattleLong = new TimeSpan(0);
            InBattleTime = DateTime.Now;
        }
        private void Battling()
        {
            BattleLong = DateTime.Now - InBattleTime;
        }
        private void BattleEnd()
        {
            OutBattleTime = DateTime.Now;
            BattleLong = OutBattleTime - InBattleTime;
        }
        private void BattleEndMore()
        {
        }
        private void FlushGameObject()
        {
            GameObj.Flush(PalHandle, PID, ExeBaseAddr);
        }
        public override bool NeedBlockCtrlEnter()
        {
            return false;
        }
    }

    public class GameObjectGuJian2
    {
        private static readonly int[] MoneyOffset = new int[] { 0x7080A8, 0x20, 0x4 };
        private const int CanControlOffset = 0x7080A8 + 0x544;
        private static readonly int[] GameTimeOffset = new int[] { 0x7080A8, 0x20, 0xE8 };
        private static readonly int[] InBattleOffset = new int[] { 0x700870, 0xAE8 };
        private static readonly int[] EnemyCountBeginOffset = new int[] { 0x700870, 0xC90 };
        private static readonly int[] EnemyCountCurrentOffset = new int[] { 0x700870, 0x16C };
        private static readonly int[] UITypeOffset = new int[] { 0x707C5C, 0x10 };
        private const int EnemyCountCurrentOffset2 = 0x910;
        private static readonly int[][] EnemyIDOffset = new int[][] {
            new int[]{ 0x700870, 0x118, 0x0, 0x560},
            new int[]{ 0x700870, 0x118, 0x4, 0x560},
            new int[]{ 0x700870, 0x118, 0x8, 0x560},
            new int[]{ 0x700870, 0x118, 0xC, 0x560},
            new int[]{ 0x700870, 0x118, 0x10, 0x560},
        };
        private static readonly int[][] EnemyHPOffset = new int[][] {
            new int[]{ 0x700870, 0x118, 0x0, 0x6C4},
            new int[]{ 0x700870, 0x118, 0x4, 0x6C4},
            new int[]{ 0x700870, 0x118, 0x8, 0x6C4},
            new int[]{ 0x700870, 0x118, 0xC, 0x6C4},
            new int[]{ 0x700870, 0x118, 0x10, 0x6C4},
        };

        public bool CanControl = false;
        public bool IsInBattle = false;
        public bool IsLoading = false;
        public int UIType = 0;//5战斗 6剧情 3读条 2主界面 4正常
        public int Money = 0;
        public float GameTime = 0.0F;
        private int EnemyCountBegin = 0;
        public int EnemyCount = 0;
        public List<GuJian2EnemyObject> Enemies = new List<GuJian2EnemyObject>();
        public enum EBattle
        {
            none,
            normal,
            太乙神兵,
            慧明,
            金砖,
            灵虚,
            偃甲将军,
            雩风,
            尸兽厌火,
            安尼瓦尔,
            流月城守卫头领,
            远古石兽,
            火龙伏英,
            怒霜天君,
            温留,
            蜃精玉怜,
            初七,
            程廷钧,
            魔化华月,
            沈夜变身,
            砺罂
        }
        public EBattle BattleID = EBattle.none;
        public int TotalEnemyHP = 0;

        public GameObjectGuJian2()
        {
        }
        public override string ToString()
        {
            string ret = "钱：" + Money + "\r\n"; ;
            ret += "读条中：" + (IsLoading ? "是" : "否") + "\r\n\r\n";
            ret += "可操作：" + (CanControl ? "可" : "不可") + "\r\n";
            ret += "战斗中：" + (IsInBattle ? "是" : "否") + " (" + BattleID.ToString() + ")[" + TotalEnemyHP + "]" + "\r\n\r\n";
            ret += "敌人数量：" + EnemyCount + "\r\n";
            foreach (var eo in Enemies)
            {
                ret += "[" + eo.ID + "]" + eo.HP + "\r\n";
            }

            return ret;
        }
        public void Flush(IntPtr handle, int PID, int ExeBaseAddr)
        {
            Money = Readm<int>(handle, ExeBaseAddr, MoneyOffset);
            GameTime = Readm<float>(handle, ExeBaseAddr, GameTimeOffset);
            IsInBattle = (Readm<int>(handle, ExeBaseAddr, InBattleOffset) != 0);
            CanControl = (Readm<int>(handle, ExeBaseAddr + CanControlOffset) == 1);
            UIType = Readm<int>(handle, ExeBaseAddr, UITypeOffset);
            IsLoading = (UIType == 3);
            EnemyCountBegin = Readm<int>(handle, ExeBaseAddr, EnemyCountBeginOffset);
            int tmpec = 0;
            if (IsInBattle)
            {
                int ecaddr = Readm<int>(handle, ExeBaseAddr, EnemyCountCurrentOffset);
                if (ecaddr > 0)
                {
                    tmpec = Readm<int>(handle, ecaddr + EnemyCountCurrentOffset2);
                }
                else
                {
                    tmpec = EnemyCountBegin;
                }
            }
            else
            {
                tmpec = 0;
                BattleID = EBattle.none;
            }
            if (tmpec > 50) tmpec = 0;
            EnemyCount = tmpec;
            List<GuJian2EnemyObject> el = new List<GuJian2EnemyObject>();
            int tmphp = 0;
            for (int i = 0; i < EnemyIDOffset.Length && i < EnemyCount; ++i)
            {
                GuJian2EnemyObject eo = new GuJian2EnemyObject();
                eo.ID = Readm<int>(handle, ExeBaseAddr, EnemyIDOffset[i]);
                eo.HP = Readm<int>(handle, ExeBaseAddr, EnemyHPOffset[i]);
                if (eo.ID > 0 && eo.ID < 2000)
                {
                    el.Add(eo);
                    if (eo.HP > 0 && eo.HP < 1000000)
                    {
                        tmphp += eo.HP;
                    }
                }
            }
            Enemies = el;
            TotalEnemyHP = tmphp;
            if (IsInBattle)
            {
                _anabattle();
            }
        }

        private void _anabattle() {
            if (Enemies.Count <= 0) return;
            foreach (var eo in Enemies)
            {
                switch (eo.ID)
                {
                    case 702:
                        BattleID = EBattle.太乙神兵;
                        return;
                    case 703:
                        BattleID = EBattle.慧明;
                        return;
                    case 704:
                    case 705:
                        BattleID = EBattle.金砖;
                        return;
                    case 706:
                    case 707:
                        BattleID = EBattle.灵虚;
                        return;
                    case 708:
                        BattleID = EBattle.偃甲将军;
                        return;
                    case 709:
                        BattleID = EBattle.雩风;
                        return;
                    case 711:
                        BattleID = EBattle.尸兽厌火;
                        return;
                    case 713:
                        BattleID = EBattle.安尼瓦尔;
                        return;
                    case 717:
                        BattleID = EBattle.流月城守卫头领;
                        return;
                    case 719:
                        BattleID = EBattle.远古石兽;
                        return;
                    case 721:
                        BattleID = EBattle.火龙伏英;
                        return;
                    case 722:
                        BattleID = EBattle.怒霜天君;
                        return;
                    case 723:
                        BattleID = EBattle.温留;
                        return;
                    case 724:
                        BattleID = EBattle.蜃精玉怜;
                        return;
                    case 726:
                        BattleID = EBattle.初七;
                        return;
                    case 727:
                        BattleID = EBattle.程廷钧;
                        return;
                    case 729:
                        BattleID = EBattle.魔化华月;
                        return;
                    case 731:
                        BattleID = EBattle.沈夜变身;
                        return;
                    case 732:
                        BattleID = EBattle.砺罂;
                        return;
                }
            }
            BattleID = EBattle.normal;
        }

        public class GuJian2EnemyObject
        {
            public int ID;
            public int HP;
        }
        public static T Readm<T>(IntPtr handle, int baseaddr, int[] offset)
        {
            int addr = baseaddr;
            for (var i = 0; i < offset.Length - 1; ++i)
            {
                addr = Readm<int>(handle, addr + offset[i]);
            }
            return Readm<T>(handle, addr + offset[offset.Length - 1]);
        }
        public static T Readm<T>(IntPtr handle, int addr)
        {
            T res = default(T);
            Type t = typeof(T);
            int size = 0;
            if (t.Name == "String")
            {
                size = 1024;
            }
            else
            {
                size = System.Runtime.InteropServices.Marshal.SizeOf(t);
            }
            byte[] buffer = new byte[size];
            int sizeofRead;

            if (Kernel32.ReadProcessMemory(handle, new IntPtr(addr), buffer, size, out sizeofRead))
            {
                if (t == typeof(short))
                {
                    short tmp = BitConverter.ToInt16(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(ushort))
                {
                    ushort tmp = BitConverter.ToUInt16(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(byte))
                {
                    byte tmp = buffer[0];
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(int))
                {
                    int tmp = BitConverter.ToInt32(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(long))
                {
                    long tmp = BitConverter.ToInt64(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(bool))
                {
                    bool tmp = BitConverter.ToBoolean(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(double))
                {
                    double tmp = BitConverter.ToDouble(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else if (t == typeof(float))
                {
                    float tmp = BitConverter.ToSingle(buffer, 0);
                    res = (T)Convert.ChangeType(tmp, t);
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < sizeofRead; ++i)
                    {
                        //byte b = buffer[i];
                        char c = (char)buffer[i];
                        if (c == '\0')
                        {
                            res = (T)Convert.ChangeType(sb.ToString(), t);
                            break;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }
                }
            }

            return res;
        }
    }
}
