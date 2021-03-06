﻿/**
 * DeepFreeze Continued...
 * (C) Copyright 2015, Jamie Leighton
 *
 * Kerbal Space Program is Copyright (C) 2013 Squad. See http://kerbalspaceprogram.com/. This
 * project is in no way associated with nor endorsed by Squad.
 *
 *  This file is part of JPLRepo's DeepFreeze (continued...) - a Fork of DeepFreeze. Original Author of DeepFreeze is 'scottpaladin' on the KSP Forums.
 *  This File was not part of the original Deepfreeze but was written by Jamie Leighton.
 *  (C) Copyright 2015, Jamie Leighton
 *
 * Continues to be licensed under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 4.0)
 * creative commons license. See <https://creativecommons.org/licenses/by-nc-sa/4.0/>
 * for full details.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RSTUtils;
using UnityEngine;
using Random = System.Random;

namespace DF
{
    internal class DeepFreezeGUI : MonoBehaviour, Savable
    {
        //GUI Properties
        private bool gamePaused;

        private IButton button1;
        private ApplicationLauncherButton stockToolbarButton; // Stock Toolbar Button
        private float DFWINDOW_WIDTH = 560;
        private float CFWINDOW_WIDTH = 340;
        private float KACWINDOW_WIDTH = 485;
        private float VSWINDOW_WIDTH = 340;
        private float WINDOW_BASE_HEIGHT = 350;
        private Rect DFwindowPos;
        private Rect CFwindowPos;
        private Rect DFKACwindowPos;
        private Rect DFVSwindowPos;
        private Rect DFVSFwindowPos;
        private static int windowID = 199999;
        private static int CFwindowID = 200000;
        private static int KACwindowID = 2000001;
        private static int VSwindowID = 2000002;
        private static int VSFwindowID = 2000003;
        private GUIStyle statusStyle, frozenStyle, comaStyle, sectionTitleStyle, resizeStyle, StatusOKStyle, StatusWarnStyle, StatusRedStyle, StatusGrayStyle, ButtonStyle;
        private Vector2 GUIscrollViewVector, GUIscrollViewVector2, GUIscrollViewVectorKAC, GUIscrollViewVectorKACKerbals = Vector2.zero;
        private bool mouseDownDF;
        private bool mouseDownKAC;
        private float DFtxtWdthName;
        private float DFtxtWdthProf;
        private float DFtxtWdthVslN;

        private float KACtxtWdthName;
        private float KACtxtWdthAtyp;
        private float KACtxtWdthATme;
        private float KACtxtWdthKName;
        private float KACtxtWdthKTyp;
        private float KACtxtWdthKTg1;
        private float KACtxtWdthKTg2;

        private float DFvslWdthName;
        private float DFvslPrtName;
        private float DFvslPrtTmp;
        private float DFvslPrtElec;
        private float DFvslAlarms;
        private float DFvslLstUpd;
        private float DFvslRT;

        private bool showKACGUI;
        private bool showConfigGUI;
        private bool LoadConfig = true;
        private bool ModKACAlarm;
        private KACWrapper.KACAPI.KACAlarm KACalarmMod;
        private List<string> KACAlarm_FrzKbls = new List<string>();
        private List<string> KACAlarm_ThwKbls = new List<string>();

        //settings vars for GUI
        private bool InputVautoRecover;

        private bool InputVdebug;
        private bool InputVECReqd;
        private bool InputAppL = true;
        private string InputScostThawKerbal = "";
        private float InputVcostThawKerbal;
        private string InputSecReqdToFreezeThaw = "";
        private int InputVecReqdToFreezeThaw;
        private string InputSglykerolReqdToFreeze = "";
        private int InputVglykerolReqdToFreeze;
        private bool InputVRegTempReqd;
        private string InputSRegTempFreeze = "";
        private double InputVRegTempFreeze;
        private string InputSRegTempMonitor = "";
        private double InputVRegTempMonitor;
        private string InputSheatamtMonitoringFrznKerbals = "";
        private double InputVheatamtMonitoringFrznKerbals;
        private string InputSheatamtThawFreezeKerbal = "";
        private double InputVheatamtThawFreezeKerbal;
        private bool InputVTempinKelvin = true;
        private bool InputStripLightsOn = true;
        private bool InputfatalOption;
        private string InputScomatoseTime = "";
        private float InputVcomatoseTime;

        //Settings vars
        private bool ECreqdForFreezer;

        private bool debugging;
        private bool AutoRecoverFznKerbals;
        private float KSCcostToThawKerbal;
        private int ECReqdToFreezeThaw;
        private bool fatalOption;
        private float comatoseTime;
        private int GlykerolReqdToFreeze;
        private bool RegTempReqd;
        private double RegTempFreeze;
        private double RegtempMonitor;
        private double heatamtMonitoringFrznKerbals;
        private double heatamtThawFreezeKerbal;
        private bool TempinKelvin;
        private bool StripLightsOn;

        //SwitchVessel vars
        private bool showUnabletoSwitchVessel;

        private bool showSwitchVessel;
        private bool switchVesselManual;
        private string showSwitchVesselStr = string.Empty;
        private Vessel switchVessel;
        private double switchVesselManualTimer;
        private bool chgECHeatsettings;
        private double chgECHeatsettingsTimer;

        //GuiVisibility
        private bool _Visible;

        public Boolean GuiVisible
        {
            get { return _Visible; }
            set
            {
                _Visible = value;      //Set the private variable
                if (_Visible)
                {
                    RenderingManager.AddToPostDrawQueue(3, onDraw);
                }
                else
                {
                    RenderingManager.RemoveFromPostDrawQueue(3, onDraw);
                }
            }
        }

        public bool Useapplauncher;

        #region AppLauncher

        private void OnGUIAppLauncherReady()
        {
            Utilities.Log_Debug("OnGUIAppLauncherReady");
            if (ApplicationLauncher.Ready)
            {
                Utilities.Log_Debug("Adding AppLauncherButton");
                stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
                    onAppLaunchToggle,
                    onAppLaunchToggle,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.FLIGHT |
                                          ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.VAB |
                                          ApplicationLauncher.AppScenes.TRACKSTATION,
                                          GameDatabase.Instance.GetTexture("REPOSoftTech/DeepFreeze/Icons/DeepFreezeOff", false));
            }
        }

        private void DummyVoid()
        {
        }

        private void onAppLaunchToggle()
        {
            GuiVisible = !GuiVisible;
            stockToolbarButton.SetTexture(GameDatabase.Instance.GetTexture(GuiVisible ? "REPOSoftTech/DeepFreeze/Icons/DeepFreezeOn" : "REPOSoftTech/DeepFreeze/Icons/DeepFreezeOff", false));
        }

        #endregion AppLauncher

        internal void OnDestroy()
        {
            if (ToolbarManager.ToolbarAvailable && Useapplauncher == false)
            {
                button1.Destroy();
            }
            else
            {
                // Set up the stock toolbar
                Utilities.Log("DeepFreezeGUI Removing onGUIAppLauncher callbacks");
                GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
                if (stockToolbarButton != null)
                {
                    ApplicationLauncher.Instance.RemoveModApplication(stockToolbarButton);
                    stockToolbarButton = null;
                }
            }
            if (GuiVisible) GuiVisible = !GuiVisible;
            GameEvents.onGamePause.Remove(GamePaused);
            GameEvents.onGameUnpause.Remove(GameUnPaused);
        }

        internal void Start()
        {
            Utilities.Log_Debug("DeepFreezeGUI startup");
            windowID = new Random().Next();
            CFwindowID = windowID + 1;
            KACwindowID = CFwindowID + 1;
            VSwindowID = KACwindowID + 1;
            VSFwindowID = VSFwindowID + 1;

            DFwindowPos = new Rect(40, Screen.height / 2 - 100, DFWINDOW_WIDTH, WINDOW_BASE_HEIGHT);
            CFwindowPos = new Rect(450, Screen.height / 2 - 100, CFWINDOW_WIDTH, 250);
            DFKACwindowPos = new Rect(600, Screen.height / 2 - 100, KACWINDOW_WIDTH, WINDOW_BASE_HEIGHT);
            DFVSwindowPos = new Rect(Screen.width / 2 - VSWINDOW_WIDTH / 2, Screen.height / 2 - 100, VSWINDOW_WIDTH, WINDOW_BASE_HEIGHT);
            DFVSFwindowPos = new Rect(Screen.width / 2 - VSWINDOW_WIDTH / 2, Screen.height / 2 - 100, VSWINDOW_WIDTH, WINDOW_BASE_HEIGHT);
            DFtxtWdthName = Mathf.Round((DFWINDOW_WIDTH - 28f) / 3.5f);
            DFtxtWdthProf = Mathf.Round((DFWINDOW_WIDTH - 28f) / 4.8f);
            DFtxtWdthVslN = Mathf.Round((DFWINDOW_WIDTH - 28f) / 3.5f);

            KACtxtWdthName = Mathf.Round((KACWINDOW_WIDTH - 38f) / 3.5f);
            KACtxtWdthAtyp = Mathf.Round((KACWINDOW_WIDTH - 38f) / 6f);
            KACtxtWdthATme = Mathf.Round((KACWINDOW_WIDTH - 38f) / 5f);
            KACtxtWdthKName = Mathf.Round((KACWINDOW_WIDTH - 48f) / 3f);
            KACtxtWdthKTyp = Mathf.Round((KACWINDOW_WIDTH - 48f) / 5f);
            KACtxtWdthKTg1 = Mathf.Round((KACWINDOW_WIDTH - 48f) / 6f);
            KACtxtWdthKTg2 = Mathf.Round((KACWINDOW_WIDTH - 48f) / 6f);

            DFvslWdthName = Mathf.Round((DFWINDOW_WIDTH - 28f) / 4.2f);
            DFvslPrtName = Mathf.Round((DFWINDOW_WIDTH - 28f) / 6.3f);
            DFvslPrtTmp = Mathf.Round((DFWINDOW_WIDTH - 28f) / 11f);
            DFvslPrtElec = Mathf.Round((DFWINDOW_WIDTH - 28f) / 12.3f);
            DFvslAlarms = Mathf.Round((DFWINDOW_WIDTH - 28f) / 8f);
            DFvslLstUpd = Mathf.Round((DFWINDOW_WIDTH - 28f) / 5.5f);
            DFvslRT = Mathf.Round((DFWINDOW_WIDTH - 28f) / 12.3f);

            Utilities.setScaledScreen();

            // create toolbar button

            if (ToolbarManager.ToolbarAvailable && Useapplauncher == false)
            {
                button1 = ToolbarManager.Instance.add("DeepFreeze", "button1");
                button1.TexturePath = "REPOSoftTech/DeepFreeze/Icons/DFtoolbar";
                button1.ToolTip = "DeepFreeze";
                button1.Visibility = new GameScenesVisibility(GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.SPACECENTER, GameScenes.TRACKSTATION);
                button1.OnClick += e => GuiVisible = !GuiVisible;
            }
            else
            {
                // Set up the stock toolbar
                Utilities.Log_Debug("Adding onGUIAppLauncher callbacks");
                if (ApplicationLauncher.Ready)
                {
                    OnGUIAppLauncherReady();
                }
                else
                    GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
            }
            GameEvents.onGamePause.Add(GamePaused);
            GameEvents.onGameUnpause.Add(GameUnPaused);
            Utilities.Log_Debug("DeepFreezeGUI END startup");
        }

        private void GamePaused()
        {
            gamePaused = true;
        }

        private void GameUnPaused()
        {
            gamePaused = false;
        }

        private void FixedUpdate()
        {
            if (Time.timeSinceLevelLoad < 2f) return; //Wait 2 seconds on level load before executing

            if (chgECHeatsettings)
            {
                double time = Planetarium.GetUniversalTime();
                if (time - chgECHeatsettingsTimer > 2)
                {
                    chgECHeatsettings = false;
                }
            }
        }

        #region GUI

        private void onDraw()
        {
            if (showSwitchVessel)
            {
                if (!Utilities.WindowVisibile(DFVSwindowPos))
                    Utilities.MakeWindowVisible(DFVSwindowPos);
                DFVSwindowPos = GUILayout.Window(VSwindowID, DFVSwindowPos, windowVS, "DeepFreeze Vessel Switch", GUILayout.ExpandWidth(false),
                    GUILayout.ExpandHeight(true), GUILayout.Width(320), GUILayout.MinHeight(100));
            }
            if (showUnabletoSwitchVessel && !switchVesselManual)
            {
                if (!Utilities.WindowVisibile(DFVSFwindowPos))
                    Utilities.MakeWindowVisible(DFVSFwindowPos);
                DFVSFwindowPos = GUILayout.Window(VSFwindowID, DFVSFwindowPos, windowVSF, "DeepFreeze Vessel Switch Failed", GUILayout.ExpandWidth(false),
                    GUILayout.ExpandHeight(true), GUILayout.Width(320), GUILayout.MinHeight(100));
            }
            if (switchVesselManual)
            {
                if (Planetarium.GetUniversalTime() - switchVesselManualTimer > 120)
                {
                    switchVesselManualTimer = 0;
                    switchVesselManual = false;
                }
            }
            if (!GuiVisible || gamePaused || FlightDriver.Pause)
            {
                return;
            }
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER || HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.FLIGHT)
            {
                GUI.skin = HighLogic.Skin;
                if (!Utilities.WindowVisibile(DFwindowPos))
                    Utilities.MakeWindowVisible(DFwindowPos);
                DFwindowPos = GUILayout.Window(windowID, DFwindowPos, windowDF, "DeepFreeze Kerbals", GUILayout.ExpandWidth(true),
                        GUILayout.ExpandHeight(true), GUILayout.MinWidth(200), GUILayout.MinHeight(250));
                if (showConfigGUI)
                {
                    if (LoadConfig)
                    {
                        InputAppL = Useapplauncher;
                        InputVECReqd = ECreqdForFreezer;
                        InputVdebug = debugging;
                        InputVautoRecover = AutoRecoverFznKerbals;
                        InputScostThawKerbal = KSCcostToThawKerbal.ToString();
                        InputSecReqdToFreezeThaw = ECReqdToFreezeThaw.ToString();
                        InputfatalOption = fatalOption;
                        InputScomatoseTime = comatoseTime.ToString();
                        InputVcomatoseTime = comatoseTime;
                        InputSglykerolReqdToFreeze = GlykerolReqdToFreeze.ToString();
                        InputVRegTempReqd = RegTempReqd;
                        InputSRegTempFreeze = RegTempFreeze.ToString();
                        InputSRegTempMonitor = RegtempMonitor.ToString();
                        InputSheatamtMonitoringFrznKerbals = heatamtMonitoringFrznKerbals.ToString();
                        InputSheatamtThawFreezeKerbal = heatamtThawFreezeKerbal.ToString();
                        InputVTempinKelvin = TempinKelvin;
                        InputStripLightsOn = StripLightsOn;
                        LoadConfig = false;
                    }
                    if (!Utilities.WindowVisibile(CFwindowPos))
                        Utilities.MakeWindowVisible(CFwindowPos);
                    CFwindowPos = GUILayout.Window(CFwindowID, CFwindowPos, windowCF, "DeepFreeze Settings", GUILayout.ExpandWidth(false),
                        GUILayout.ExpandHeight(true), GUILayout.Width(320), GUILayout.MinHeight(100));
                }
                if (showKACGUI)
                {
                    if (!Utilities.WindowVisibile(DFKACwindowPos))
                        Utilities.MakeWindowVisible(DFKACwindowPos);
                    DFKACwindowPos = GUILayout.Window(KACwindowID, DFKACwindowPos, windowKAC, "DeepFreeze Alarms", GUILayout.ExpandWidth(true),
                        GUILayout.ExpandHeight(true), GUILayout.MinWidth(360), GUILayout.MinHeight(150));
                }
            }
        }

        private void windowDF(int id)
        {
            //Init styles
            sectionTitleStyle = new GUIStyle(GUI.skin.label);
            sectionTitleStyle.alignment = TextAnchor.MiddleLeft;
            sectionTitleStyle.stretchWidth = true;
            sectionTitleStyle.normal.textColor = Color.blue;
            sectionTitleStyle.fontStyle = FontStyle.Bold;

            statusStyle = new GUIStyle(GUI.skin.label);
            statusStyle.alignment = TextAnchor.MiddleLeft;
            statusStyle.stretchWidth = true;
            statusStyle.normal.textColor = Color.white;

            frozenStyle = new GUIStyle(GUI.skin.label);
            frozenStyle.alignment = TextAnchor.MiddleLeft;
            frozenStyle.stretchWidth = true;
            frozenStyle.normal.textColor = Color.cyan;

            comaStyle = new GUIStyle(GUI.skin.label);
            comaStyle.alignment = TextAnchor.MiddleLeft;
            comaStyle.stretchWidth = true;
            comaStyle.normal.textColor = Color.gray;

            StatusOKStyle = new GUIStyle(GUI.skin.label);
            StatusOKStyle.alignment = TextAnchor.MiddleLeft;
            StatusOKStyle.stretchWidth = true;
            StatusOKStyle.normal.textColor = Color.green;

            StatusWarnStyle = new GUIStyle(GUI.skin.label);
            StatusWarnStyle.alignment = TextAnchor.MiddleLeft;
            StatusWarnStyle.stretchWidth = true;
            StatusWarnStyle.normal.textColor = Color.yellow;

            StatusRedStyle = new GUIStyle(GUI.skin.label);
            StatusRedStyle.alignment = TextAnchor.MiddleLeft;
            StatusRedStyle.stretchWidth = true;
            StatusRedStyle.normal.textColor = Color.red;

            StatusGrayStyle = new GUIStyle(GUI.skin.label);
            StatusGrayStyle.alignment = TextAnchor.MiddleLeft;
            StatusGrayStyle.stretchWidth = true;
            StatusGrayStyle.normal.textColor = Color.gray;

            resizeStyle = new GUIStyle(GUI.skin.button);
            resizeStyle.alignment = TextAnchor.MiddleCenter;
            resizeStyle.padding = new RectOffset(1, 1, 1, 1);

            GUIContent closeContent = new GUIContent("X", "Close Window");
            Rect closeRect = new Rect(DFwindowPos.width - 17, 4, 16, 16);
            if (GUI.Button(closeRect, closeContent))
            {
                onAppLaunchToggle();
                return;
            }

            GUIscrollViewVector2 = GUILayout.BeginScrollView(GUIscrollViewVector2, false, false, GUILayout.MaxHeight(140f));
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vessel", sectionTitleStyle, GUILayout.Width(DFvslWdthName));
            GUILayout.Label("Part", sectionTitleStyle, GUILayout.Width(DFvslPrtName));
            GUILayout.Label("Tmp", sectionTitleStyle, GUILayout.Width(DFvslPrtTmp));
            GUILayout.Label("EC", sectionTitleStyle, GUILayout.Width(DFvslPrtElec));
            if (DFInstalledMods.IsRTInstalled)
                GUILayout.Label("R.T", sectionTitleStyle, GUILayout.Width(DFvslRT));
            GUILayout.Label("Alarms", sectionTitleStyle, GUILayout.Width(DFvslAlarms));
            GUILayout.Label("LastUpd", sectionTitleStyle, GUILayout.Width(DFvslLstUpd));
            if (DeepFreeze.Instance.DFsettings.ECreqdForFreezer)
                GUILayout.Label("TimeRem", sectionTitleStyle, GUILayout.Width(DFvslLstUpd));
            GUILayout.EndHorizontal();
            foreach (KeyValuePair<uint, PartInfo> frzr in DeepFreeze.Instance.DFgameSettings.knownFreezerParts)
            {
                GUILayout.BeginHorizontal();
                VesselInfo vsl = DeepFreeze.Instance.DFgameSettings.knownVessels[frzr.Value.vesselID];
                GUILayout.Label(vsl.vesselName, statusStyle, GUILayout.Width(DFvslWdthName));
                string partname = string.Empty;
                if (frzr.Value.PartName.Substring(8, 1) == "R")
                    partname = frzr.Value.PartName.Substring(0, 9);
                else
                    partname = frzr.Value.PartName.Substring(0, 8);
                GUILayout.Label(partname, statusStyle, GUILayout.Width(DFvslPrtName));
                string TempVar;
                if (DeepFreeze.Instance.DFsettings.TempinKelvin)
                {
                    TempVar = frzr.Value.cabinTemp.ToString("###0") + "K";
                }
                else
                {
                    TempVar = Utilities.KelvintoCelsius(frzr.Value.cabinTemp).ToString("###0") + "C";
                }

                if (DeepFreeze.Instance.DFsettings.RegTempReqd && !chgECHeatsettings)
                {
                    switch (frzr.Value.TmpStatus)
                    {
                        case FrzrTmpStatus.OK:
                            {
                                GUILayout.Label(TempVar, StatusOKStyle, GUILayout.Width(DFvslPrtTmp));
                                break;
                            }
                        case FrzrTmpStatus.WARN:
                            {
                                GUILayout.Label(TempVar, StatusWarnStyle, GUILayout.Width(DFvslPrtTmp));
                                break;
                            }
                        case FrzrTmpStatus.RED:
                            {
                                GUILayout.Label(TempVar, StatusRedStyle, GUILayout.Width(DFvslPrtTmp));
                                switchVessel = FlightGlobals.Vessels.Find(a => a.id == frzr.Value.vesselID);
                                showSwitchVesselStr = "Vessel " + switchVessel.vesselName + " is Over-Heating.";
                                if (HighLogic.LoadedSceneIsFlight)
                                {
                                    if (FlightGlobals.ActiveVessel.id != frzr.Value.vesselID && !switchVesselManual)
                                    {
                                        showSwitchVessel = true;
                                    }
                                }
                                break;
                            }
                    }
                }
                else
                {
                    GUILayout.Label("OFF", StatusGrayStyle, GUILayout.Width(DFvslPrtTmp));
                }

                if (DeepFreeze.Instance.DFsettings.ECreqdForFreezer && !chgECHeatsettings)
                {
                    if (frzr.Value.numFrznCrew == 0)
                    {
                        GUILayout.Label("S/BY", StatusOKStyle, GUILayout.Width(DFvslPrtElec));
                    }
                    else
                    {
                        if (frzr.Value.outofEC)
                        {
                            GUILayout.Label("OUT", StatusRedStyle, GUILayout.Width(DFvslPrtElec));
                            switchVessel = FlightGlobals.Vessels.Find(a => a.id == frzr.Value.vesselID);
                            showSwitchVesselStr = "Vessel " + switchVessel.vesselName + " is out of ElectricCharge.\n Situation Critical.";
                            if (HighLogic.LoadedSceneIsFlight)
                            {
                                if (FlightGlobals.ActiveVessel.id != frzr.Value.vesselID && !switchVesselManual)
                                {
                                    showSwitchVessel = true;
                                }
                            }
                        }
                        else
                        {
                            if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.EClowCriticalTime)
                            {
                                GUILayout.Label("ALRT", StatusRedStyle, GUILayout.Width(DFvslPrtElec));
                                switchVessel = FlightGlobals.Vessels.Find(a => a.id == frzr.Value.vesselID);
                                showSwitchVesselStr = "Vessel " + switchVessel.vesselName + " is almost out of ElectricCharge.";
                                if (HighLogic.LoadedSceneIsFlight)
                                {
                                    if (FlightGlobals.ActiveVessel.id != frzr.Value.vesselID && !switchVesselManual)
                                    {
                                        showSwitchVessel = true;
                                    }
                                }
                            }
                            else
                            {
                                if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.ECLowWarningTime)  // ONE HOUR OF EC WARNING
                                {
                                    // Utilities.Log_Debug("Remaining EC time " + vsl.predictedECOut);
                                    GUILayout.Label("LOW", StatusWarnStyle, GUILayout.Width(DFvslPrtElec));
                                }
                                else
                                {
                                    GUILayout.Label("OK", StatusOKStyle, GUILayout.Width(DFvslPrtElec));
                                }
                            }
                        }
                    }
                }
                else
                {
                    GUILayout.Label("OFF", StatusGrayStyle, GUILayout.Width(DFvslPrtElec));
                }

                if (DFInstalledMods.IsRTInstalled)
                {
                    if (DFInstalledMods.RTVesselConnected(frzr.Value.vesselID))
                    {
                        GUILayout.Label("OK", StatusOKStyle, GUILayout.Width(DFvslRT));
                    }
                    else
                    {
                        GUILayout.Label("NC", StatusRedStyle, GUILayout.Width(DFvslRT));
                    }
                }

                //if (DeepFreeze.Instance.DFgameSettings.knownKACAlarms.Where(e => (e.Value.VesselID == frzr.Value.vesselID) &&
                //((e.Value.FrzKerbals.Count() > 0) || (e.Value.ThwKerbals.Count() > 0))).Count() > 0)
                if (DeepFreeze.Instance.DFgameSettings.knownKACAlarms.Any(e => e.Value.VesselID == frzr.Value.vesselID))
                {
                    //GUILayout.Label("Active", StatusOKStyle, GUILayout.Width(DFvslAlarms));
                    if (GUILayout.Button(new GUIContent("Alarm", "Go to Alarms"), GUILayout.Width(DFvslAlarms)))
                    {
                        showKACGUI = !showKACGUI;
                    }
                }
                else
                {
                    GUILayout.Label("    ", StatusGrayStyle, GUILayout.Width(DFvslAlarms));
                }

                if (HighLogic.LoadedSceneIsFlight)
                {
                    if (frzr.Value.vesselID == FlightGlobals.ActiveVessel.id)
                    {
                        GUILayout.Label("    ", StatusOKStyle, GUILayout.Width(DFvslLstUpd));
                    }
                    else
                    {
                        if (DeepFreeze.Instance.DFsettings.ECreqdForFreezer && !chgECHeatsettings)
                        {
                            if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.EClowCriticalTime)
                            {
                                GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusRedStyle, GUILayout.Width(DFvslLstUpd));
                            }
                            else
                            {
                                if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.ECLowWarningTime)
                                {
                                    GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusWarnStyle, GUILayout.Width(DFvslLstUpd));
                                }
                                else
                                {
                                    GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusOKStyle, GUILayout.Width(DFvslLstUpd));
                                }
                            }
                        }
                        else
                        {
                            GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusGrayStyle, GUILayout.Width(DFvslLstUpd));
                        }
                    }
                }
                else
                {
                    if (DeepFreeze.Instance.DFsettings.ECreqdForFreezer && !chgECHeatsettings)
                    {
                        if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.EClowCriticalTime)
                        {
                            GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusRedStyle, GUILayout.Width(DFvslLstUpd));
                        }
                        else
                        {
                            if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.ECLowWarningTime)
                            {
                                GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusWarnStyle, GUILayout.Width(DFvslLstUpd));
                            }
                            else
                            {
                                GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusOKStyle, GUILayout.Width(DFvslLstUpd));
                            }
                        }
                    }
                    else
                    {
                        GUILayout.Label(Utilities.FormatDateString(Planetarium.GetUniversalTime() - frzr.Value.timeLastElectricity), StatusGrayStyle, GUILayout.Width(DFvslLstUpd));
                    }
                }

                if (DeepFreeze.Instance.DFsettings.ECreqdForFreezer && !chgECHeatsettings)
                {
                    if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.EClowCriticalTime)
                    {
                        GUILayout.Label(Utilities.FormatDateString(vsl.predictedECOut), StatusRedStyle, GUILayout.Width(DFvslLstUpd));
                    }
                    else
                    {
                        if (vsl.predictedECOut < DeepFreeze.Instance.DFsettings.ECLowWarningTime)
                        {
                            GUILayout.Label(Utilities.FormatDateString(vsl.predictedECOut), StatusWarnStyle, GUILayout.Width(DFvslLstUpd));
                        }
                        else
                        {
                            GUILayout.Label(Utilities.FormatDateString(vsl.predictedECOut), StatusOKStyle, GUILayout.Width(DFvslLstUpd));
                        }
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            bool Headers = false;
            GUIscrollViewVector = GUILayout.BeginScrollView(GUIscrollViewVector, false, false);
            GUILayout.BeginVertical();
            if (DeepFreeze.Instance.DFgameSettings.KnownFrozenKerbals.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("There are currently no Frozen Kerbals", frozenStyle);
                GUILayout.EndHorizontal();
            }
            else
            {
                Headers = true;
                GUILayout.BeginHorizontal();
                GUILayout.Label("Kerbal Name", sectionTitleStyle, GUILayout.Width(DFtxtWdthName));
                GUILayout.Label("Profession", sectionTitleStyle, GUILayout.Width(DFtxtWdthProf));
                GUILayout.Label("Vessel Name", sectionTitleStyle, GUILayout.Width(DFtxtWdthVslN));
                GUILayout.EndHorizontal();
                List<KeyValuePair<string, KerbalInfo>> ThawKeysToDelete = new List<KeyValuePair<string, KerbalInfo>>();
                foreach (KeyValuePair<string, KerbalInfo> kerbal in DeepFreeze.Instance.DFgameSettings.KnownFrozenKerbals)
                {
                    GUILayout.BeginHorizontal();
                    GUIStyle dispstyle = kerbal.Value.type != ProtoCrewMember.KerbalType.Tourist ? frozenStyle : comaStyle;
                    GUILayout.Label(kerbal.Key, dispstyle, GUILayout.Width(DFtxtWdthName));
                    GUILayout.Label(kerbal.Value.experienceTraitName, dispstyle, GUILayout.Width(DFtxtWdthProf));
                    GUILayout.Label(kerbal.Value.vesselName, dispstyle, GUILayout.Width(DFtxtWdthVslN));
                    if (kerbal.Value.type != ProtoCrewMember.KerbalType.Tourist)
                    {
                        if (HighLogic.LoadedScene == GameScenes.FLIGHT && DFIntMemory.Instance.ActVslHasDpFrezr)
                        //if in flight and active vessel has a Freezer part check if kerbal is part of this vessel and add a Thaw button to the GUI
                        {
                            //foreach (DeepFreezer frzr in DFIntMemory.Instance.DpFrzrActVsl)
                            //{
                            //if (frzr.DFIStoredCrewList.FirstOrDefault(a => a.CrewName == kerbal.Key) != null)
                            if (kerbal.Value.vesselID == FlightGlobals.ActiveVessel.id && kerbal.Value.type != ProtoCrewMember.KerbalType.Tourist)
                            {
                                if (DFInstalledMods.IsRTInstalled && !DFInstalledMods.RTVesselConnected(DFIntMemory.Instance.ActVslID))
                                {
                                    GUI.enabled = false;
                                }
                                if (GUILayout.Button(new GUIContent("Thaw", "Thaw this Kerbal"), GUILayout.Width(50f)))
                                {
                                    DeepFreezer frzr = DFIntMemory.Instance.DpFrzrActVsl.FirstOrDefault(a => a.part.flightID == kerbal.Value.partID);
                                    if (frzr != null)
                                        frzr.beginThawKerbal(kerbal.Key);
                                }
                                GUI.enabled = true;
                            }
                            //}
                        }
                        if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                        {
                            if (GUILayout.Button(new GUIContent("Thaw", "Thaw this Kerbal"), GUILayout.Width(50f)))
                            {
                                // We need to check kerbal isn't in a vessel still out there somewhere....
                                Vessel vessel = FlightGlobals.Vessels.Find(v => v.id == kerbal.Value.vesselID);
                                if (vessel != null)
                                {
                                    Utilities.Log_Debug("Cannot thaw, vessel still exists " + vessel.situation + " at " + vessel.mainBody.bodyName);
                                    ScreenMessages.PostScreenMessage("Cannot thaw " + kerbal.Key + " from KSC. Vessel still exists " + vessel.situation + " at " + vessel.mainBody.bodyName, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                }
                                else
                                {
                                    ThawKeysToDelete.Add(new KeyValuePair<string, KerbalInfo>(kerbal.Key, kerbal.Value));
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    //}
                }
                foreach (KeyValuePair<string, KerbalInfo> entries in ThawKeysToDelete)
                {
                    DeepFreeze.Instance.ThawFrozenCrew(entries.Key, entries.Value.vesselID);
                }
            }

            if (HighLogic.LoadedScene == GameScenes.FLIGHT && DFIntMemory.Instance.ActVslHasDpFrezr)
            {
                if (!Headers)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Kerbal Name", sectionTitleStyle, GUILayout.Width(DFtxtWdthName));
                    GUILayout.Label("Profession", sectionTitleStyle, GUILayout.Width(DFtxtWdthProf));
                    GUILayout.Label("Vessel Name", sectionTitleStyle, GUILayout.Width(DFtxtWdthVslN));
                    GUILayout.EndHorizontal();
                    Headers = true;
                }
                foreach (DeepFreezer frzr in DFIntMemory.Instance.DpFrzrActVsl)
                {
                    foreach (ProtoCrewMember crewMember in frzr.part.protoModuleCrew.FindAll(a => a.type == ProtoCrewMember.KerbalType.Crew))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(crewMember.name, statusStyle, GUILayout.Width(DFtxtWdthName));
                        GUILayout.Label(crewMember.experienceTrait.Title, statusStyle, GUILayout.Width(DFtxtWdthProf));
                        GUILayout.Label(frzr.part.vessel.vesselName, statusStyle, GUILayout.Width(DFtxtWdthVslN));
                        if (crewMember.type != ProtoCrewMember.KerbalType.Tourist)
                        {
                            if (frzr.DFIcrewXferFROMActive || frzr.DFIcrewXferTOActive || (DFInstalledMods.IsSMInstalled && frzr.IsSMXferRunning())
                                                        || frzr.IsFreezeActive || frzr.IsThawActive || (DFInstalledMods.IsRTInstalled && !DFInstalledMods.RTVesselConnected(DFIntMemory.Instance.ActVslID)))
                            {
                                GUI.enabled = false;
                            }
                            if (GUILayout.Button(new GUIContent("Freeze", "Freeze this Kerbal"), GUILayout.Width(50f)))
                            {
                                frzr.beginFreezeKerbal(crewMember);
                            }
                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(14);
            if (KACWrapper.AssemblyExists && KACWrapper.InstanceExists && KACWrapper.APIReady)
            {
                GUIContent AlarmsContent = new GUIContent("Alarms", "KAC Alarms");
                Rect AlarmsRect = new Rect(DFwindowPos.width - 90, DFwindowPos.height - 17, 70, 16);
                if (GUI.Button(AlarmsRect, AlarmsContent))
                {
                    showKACGUI = !showKACGUI;
                }
            }

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                GUIContent settingsContent = new GUIContent("Settings", "Config Settings");
                Rect settingsRect = new Rect(DFwindowPos.width - 165, DFwindowPos.height - 17, 70, 16);
                if (GUI.Button(settingsRect, settingsContent))
                {
                    showConfigGUI = !showConfigGUI;
                }
            }

            GUIContent resizeContent = new GUIContent("R", "Resize Window");
            Rect resizeRect = new Rect(DFwindowPos.width - 17, DFwindowPos.height - 17, 16, 16);
            GUI.Label(resizeRect, resizeContent, resizeStyle);
            HandleResizeEventsDF(resizeRect);

            GUI.DragWindow();
        }

        private void windowCF(int id)
        {
            //Init styles
            sectionTitleStyle = new GUIStyle(GUI.skin.label);
            sectionTitleStyle.alignment = TextAnchor.MiddleCenter;
            sectionTitleStyle.stretchWidth = true;
            sectionTitleStyle.fontStyle = FontStyle.Bold;

            statusStyle = new GUIStyle(GUI.skin.label);
            statusStyle.alignment = TextAnchor.MiddleLeft;
            statusStyle.stretchWidth = true;
            statusStyle.normal.textColor = Color.white;

            GUIContent closeContent = new GUIContent("X", "Close Window");
            Rect closeRect = new Rect(CFwindowPos.width - 17, 4, 16, 16);
            if (GUI.Button(closeRect, closeContent))
            {
                showConfigGUI = false;
                LoadConfig = true;
                return;
            }

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("ElectricCharge Required to run Freezers", "If on, EC is required to run freezers"), statusStyle, GUILayout.Width(280));
            InputVECReqd = GUILayout.Toggle(InputVECReqd, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            if (!InputVECReqd) GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Fatal EC/Heat Option", "If on Kerbals will die if EC runs out or it gets too hot"), statusStyle, GUILayout.Width(280));
            InputfatalOption = GUILayout.Toggle(InputfatalOption, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            if (InputfatalOption) GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Non Fatal Kerbal Comatose Time(in secs)", "The time in seconds a kerbal is comatose if fatal EC/Heat option is off"), statusStyle, GUILayout.Width(250));
            InputScomatoseTime = Regex.Replace(GUILayout.TextField(InputScomatoseTime, 5, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            if (!float.TryParse(InputScomatoseTime, out InputVcomatoseTime))
            {
                InputVcomatoseTime = comatoseTime;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("AutoRecover Frozen Kerbals at KSC", "If on, will AutoRecover Frozen Kerbals at the KSC and deduct the Cost from your funds"), statusStyle, GUILayout.Width(280));
            InputVautoRecover = GUILayout.Toggle(InputVautoRecover, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Cost to Thaw a Kerbal at KSC", "Amt of currency Reqd to Freeze a Kerbal from the KSC"), statusStyle, GUILayout.Width(250));
            InputScostThawKerbal = Regex.Replace(GUILayout.TextField(InputScostThawKerbal, 5, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();

            if (!float.TryParse(InputScostThawKerbal, out InputVcostThawKerbal))
            {
                InputVcostThawKerbal = KSCcostToThawKerbal;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("ElecCharge Reqd to Freeze/Thaw a Kerbal", "Amt of ElecCharge Reqd to Freeze/Thaw a Kerbal"), statusStyle, GUILayout.Width(250));
            InputSecReqdToFreezeThaw = Regex.Replace(GUILayout.TextField(InputSecReqdToFreezeThaw, 5, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();

            if (!int.TryParse(InputSecReqdToFreezeThaw, out InputVecReqdToFreezeThaw))
            {
                InputVecReqdToFreezeThaw = ECReqdToFreezeThaw;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Glykerol Reqd to Freeze a Kerbal", "Amt of Glykerol used to Freeze a Kerbal, Overrides Part values"), statusStyle, GUILayout.Width(250));
            InputSglykerolReqdToFreeze = Regex.Replace(GUILayout.TextField(InputSglykerolReqdToFreeze, 5, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();

            if (!int.TryParse(InputSglykerolReqdToFreeze, out InputVglykerolReqdToFreeze))
            {
                InputVglykerolReqdToFreeze = GlykerolReqdToFreeze;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Temps are in (K)elvin. (K) = (C)elcius + 273.15. (K) = ((F)arenheit + 459.67) × 5/9", "Get your calculator out"), statusStyle, GUILayout.Width(280));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Regulated Temperatures Required", "If on, Regulated Temps apply to freeze and keep Kerbals Frozen"), statusStyle, GUILayout.Width(280));
            InputVRegTempReqd = GUILayout.Toggle(InputVRegTempReqd, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            if (!InputVRegTempReqd) GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Minimum Part Temp. for Freezer to Freeze (K)", "Minimum part temperature for Freezer to be able to Freeze"), statusStyle, GUILayout.Width(250));
            InputSRegTempFreeze = Regex.Replace(GUILayout.TextField(InputSRegTempFreeze, 3, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();
            if (!double.TryParse(InputSRegTempFreeze, out InputVRegTempFreeze))
            {
                InputVRegTempFreeze = RegTempFreeze;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("maximum Part Temp. to Keep Kerbals Frozen (K)", "Maximum part temperature for Freezer to keep Kerbals frozen"), statusStyle, GUILayout.Width(250));
            InputSRegTempMonitor = Regex.Replace(GUILayout.TextField(InputSRegTempMonitor, 3, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();
            if (!double.TryParse(InputSRegTempMonitor, out InputVRegTempMonitor))
            {
                InputVRegTempMonitor = RegtempMonitor;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Heat generated in thaw/freeze process (kW)", "Amount of thermal heat (kW) generated with each thaw/freeze process"), statusStyle, GUILayout.Width(250));
            InputSheatamtThawFreezeKerbal = Regex.Replace(GUILayout.TextField(InputSheatamtThawFreezeKerbal, 3, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();
            if (!double.TryParse(InputSheatamtThawFreezeKerbal, out InputVheatamtThawFreezeKerbal))
            {
                InputVheatamtThawFreezeKerbal = heatamtThawFreezeKerbal;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Equip. heat generated per frozen kerbal (kW) per minute", "Amount of thermal heat (kW) generated by equip. for each frozen kerbal"), statusStyle, GUILayout.Width(250));
            InputSheatamtMonitoringFrznKerbals = Regex.Replace(GUILayout.TextField(InputSheatamtMonitoringFrznKerbals, 3, GUILayout.MinWidth(30.0F)), "[^.0-9]", "");  //you can play with the width of the text box
            GUILayout.EndHorizontal();
            if (!double.TryParse(InputSheatamtMonitoringFrznKerbals, out InputVheatamtMonitoringFrznKerbals))
            {
                InputVheatamtMonitoringFrznKerbals = heatamtMonitoringFrznKerbals;
            }

            GUI.enabled = true;

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Show Part Temperature in Kelvin", "If on Part right click will show temp in Kelvin, if Off will show in Celcius"), statusStyle, GUILayout.Width(280));
            InputVTempinKelvin = GUILayout.Toggle(InputVTempinKelvin, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Use Stock Launcher Icon (restart required)", "If on uses AppLauncher, If Off Uses Toolbar"), statusStyle, GUILayout.Width(280));
            InputAppL = GUILayout.Toggle(InputAppL, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Freezer Strip Lights On", "Turn off if you do not want the internal freezer strip lights to function"), statusStyle, GUILayout.Width(280));
            InputStripLightsOn = GUILayout.Toggle(InputStripLightsOn, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Box("Debug Mode", statusStyle, GUILayout.Width(280));
            InputVdebug = GUILayout.Toggle(InputVdebug, "", GUILayout.MinWidth(30.0F)); //you can play with the width of the text box
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Save & Exit Settings", "Save & Exit Settings"), GUILayout.Width(155f)))
            {
                Useapplauncher = InputAppL;
                if (ECreqdForFreezer != InputVECReqd)
                {
                    chgECHeatsettings = true;
                    chgECHeatsettingsTimer = Planetarium.GetUniversalTime();
                }

                ECreqdForFreezer = InputVECReqd;
                debugging = InputVdebug;
                
                AutoRecoverFznKerbals = InputVautoRecover;
                KSCcostToThawKerbal = InputVcostThawKerbal;
                ECReqdToFreezeThaw = InputVecReqdToFreezeThaw;
                fatalOption = InputfatalOption;
                GlykerolReqdToFreeze = InputVglykerolReqdToFreeze;
                RegTempReqd = InputVRegTempReqd;
                if (RegTempReqd != InputVRegTempReqd)
                {
                    chgECHeatsettings = true;
                    chgECHeatsettingsTimer = Planetarium.GetUniversalTime();
                }
                RegTempFreeze = InputVRegTempFreeze;
                RegtempMonitor = InputVRegTempMonitor;
                heatamtMonitoringFrznKerbals = InputVheatamtMonitoringFrznKerbals;
                heatamtThawFreezeKerbal = InputVheatamtThawFreezeKerbal;
                TempinKelvin = InputVTempinKelvin;
                StripLightsOn = InputStripLightsOn;
                showConfigGUI = false;
                LoadConfig = true;
                ConfigNode tmpNode = new ConfigNode();
                Save(tmpNode);
            }
            if (GUILayout.Button(new GUIContent("Reset Settings", "Reset Settings"), GUILayout.Width(155f)))
            {
                LoadConfig = true;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void windowKAC(int id)
        {
            //Init styles
            sectionTitleStyle = new GUIStyle(GUI.skin.label);
            sectionTitleStyle.alignment = TextAnchor.MiddleLeft;
            sectionTitleStyle.stretchWidth = true;
            sectionTitleStyle.normal.textColor = Color.blue;
            sectionTitleStyle.fontStyle = FontStyle.Bold;

            statusStyle = new GUIStyle(GUI.skin.label);
            statusStyle.alignment = TextAnchor.MiddleLeft;
            statusStyle.stretchWidth = true;
            statusStyle.normal.textColor = Color.white;

            frozenStyle = new GUIStyle(GUI.skin.label);
            frozenStyle.alignment = TextAnchor.MiddleLeft;
            frozenStyle.stretchWidth = true;
            frozenStyle.normal.textColor = Color.cyan;

            StatusOKStyle = new GUIStyle(GUI.skin.label);
            StatusOKStyle.alignment = TextAnchor.MiddleLeft;
            StatusOKStyle.stretchWidth = true;
            StatusOKStyle.normal.textColor = Color.green;

            StatusWarnStyle = new GUIStyle(GUI.skin.label);
            StatusWarnStyle.alignment = TextAnchor.MiddleLeft;
            StatusWarnStyle.stretchWidth = true;
            StatusWarnStyle.normal.textColor = Color.yellow;

            StatusRedStyle = new GUIStyle(GUI.skin.label);
            StatusRedStyle.alignment = TextAnchor.MiddleLeft;
            StatusRedStyle.stretchWidth = true;
            StatusRedStyle.normal.textColor = Color.red;

            resizeStyle = new GUIStyle(GUI.skin.button);
            resizeStyle.alignment = TextAnchor.MiddleCenter;
            resizeStyle.padding = new RectOffset(1, 1, 1, 1);

            ButtonStyle = new GUIStyle(GUI.skin.toggle);
            ButtonStyle.margin.top = 0;
            ButtonStyle.margin.bottom = 0;
            ButtonStyle.padding.top = 0;
            ButtonStyle.padding.bottom = 0;

            GUIContent closeContent = new GUIContent("X", "Close Window");
            Rect closeRect = new Rect(DFKACwindowPos.width - 17, 4, 16, 16);
            if (GUI.Button(closeRect, closeContent))
            {
                showKACGUI = false;
                return;
            }

            // Utilities.Log_Debug("start WindowKAC ModKacAlarm active=" + ModKACAlarm);

            //Draw the alarms that KAC has that are for the CURRENT Vessel ONLY, so ONLY in FLIGHT mode.
            GUIscrollViewVectorKAC = GUILayout.BeginScrollView(GUIscrollViewVectorKAC, false, false);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", sectionTitleStyle, GUILayout.Width(KACtxtWdthName));
            GUILayout.Label("Alarm Type", sectionTitleStyle, GUILayout.Width(KACtxtWdthAtyp));
            GUILayout.Label("Time Remain.", sectionTitleStyle, GUILayout.Width(KACtxtWdthATme));
            GUILayout.EndHorizontal();
            if (KACWrapper.KAC.Alarms.Count == 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("There are currently no KAC alarms associated to a DeepFreeze vessel", frozenStyle);
                GUILayout.EndHorizontal();
            }
            else
            {
                foreach (KACWrapper.KACAPI.KACAlarm alarm in KACWrapper.KAC.Alarms)
                {
                    //Only show KAC alarms that are in the DeepFreeze known vessels list. (IE: vessels that have a freezer)
                    Guid tmpid = new Guid(alarm.VesselID);
                    if (DeepFreeze.Instance.DFgameSettings.knownVessels.ContainsKey(tmpid))
                    {
                        GUILayout.BeginHorizontal();
                        double TmeRemaining = Math.Round(alarm.AlarmTime - Planetarium.GetUniversalTime(), 0);

                        if (TmeRemaining <= 5)
                        {
                            GUILayout.Label(alarm.Name, StatusWarnStyle, GUILayout.Width(KACtxtWdthName));
                            GUILayout.Label(alarm.AlarmType.ToString(), StatusWarnStyle, GUILayout.Width(KACtxtWdthAtyp));
                            GUILayout.Label(Utilities.FormatDateString(TmeRemaining), StatusWarnStyle, GUILayout.Width(KACtxtWdthATme));
                        }
                        else
                        {
                            GUILayout.Label(alarm.Name, statusStyle, GUILayout.Width(KACtxtWdthName));
                            GUILayout.Label(alarm.AlarmType.ToString(), statusStyle, GUILayout.Width(KACtxtWdthAtyp));
                            GUILayout.Label(Utilities.FormatDateString(TmeRemaining), statusStyle, GUILayout.Width(KACtxtWdthATme));
                        }
                        // Utilities.Log_Debug("Show alarm  from KAC " + alarm.ID + " " + alarm.Name + " " + alarm.VesselID);

                        //Option to delete each alarm
                        if (ModKACAlarm || (DFInstalledMods.IsRTInstalled && !DFInstalledMods.RTVesselConnected(tmpid)))
                        {
                            //If a modify is in progress we turn off the delete button
                            GUI.enabled = false;
                            GUILayout.Button("Delete", GUILayout.Width(50));
                            GUI.enabled = true;
                            // Utilities.Log_Debug("Delete button disabled");
                        }
                        else
                        {
                            if (TmeRemaining <= 0) GUI.enabled = false;
                            if (GUILayout.Button("Delete", GUILayout.Width(50)))
                            {
                                KACWrapper.KAC.DeleteAlarm(alarm.ID);
                            }
                            GUI.enabled = true;
                        }

                        //Option to modify
                        if (ModKACAlarm) // If a Modify is in progress
                        {
                            if (KACalarmMod.ID != alarm.ID) //If it isn't this alarm we disable the button
                            {
                                GUI.enabled = false;
                                GUILayout.Button("Modify", GUILayout.Width(50));
                                GUI.enabled = true;
                                // Utilities.Log_Debug("Modify button disabled");
                            }
                            else //We are modifying an alarm and it's this one. So we draw a SAVE and Cancel button to save/cancel changes.
                            {
                                // Utilities.Log_Debug("mod in progress and it's this one, change to Save/Cancel");
                                if (GUILayout.Button("Save", GUILayout.Width(50)))
                                {
                                    if (DFInstalledMods.IsRTInstalled && !DFInstalledMods.RTVesselConnected(tmpid))
                                    {
                                        ScreenMessages.PostScreenMessage("Cannot Save Alarm. No R/Tech Connection to vessel.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                    }
                                    else
                                    {
                                        if (TmeRemaining > 0)
                                        {
                                            DFIntMemory.Instance.ModifyKACAlarm(KACalarmMod, KACAlarm_FrzKbls, KACAlarm_ThwKbls);
                                            ScreenMessages.PostScreenMessage("DeepFreeze Alarm changes Saved.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                            // Utilities.Log_Debug("DF KAC Modified alarm " + KACalarmMod.ID + " " + KACalarmMod.Name);
                                        }
                                        else
                                        {
                                            ScreenMessages.PostScreenMessage("DeepFreeze Cannot Save alarm changes, Time is up.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                                            Utilities.Log_Debug("DF KAC Couldn't save Modified alarm time is up");
                                        }
                                    }
                                    ModKACAlarm = false;
                                }
                                if (GUILayout.Button("Cancel", GUILayout.Width(50)))
                                {
                                    // Utilities.Log_Debug("User cancelled mod");
                                    ModKACAlarm = false;
                                }
                                GUILayout.EndHorizontal();
                                GUIscrollViewVectorKACKerbals = GUILayout.BeginScrollView(GUIscrollViewVectorKACKerbals, false, false, GUILayout.MaxHeight(100f));
                                GUILayout.BeginVertical();
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Name", sectionTitleStyle, GUILayout.Width(KACtxtWdthKName));
                                GUILayout.Label("Trait", sectionTitleStyle, GUILayout.Width(KACtxtWdthKTyp));
                                GUILayout.Label("Thaw", sectionTitleStyle, GUILayout.Width(KACtxtWdthKTg1));
                                GUILayout.Label("Freeze", sectionTitleStyle, GUILayout.Width(KACtxtWdthKTg2));
                                GUILayout.EndHorizontal();
                                //Build the Crew list for the alarm and allow modifications
                                List<KeyValuePair<uint, PartInfo>> frzrs = DeepFreeze.Instance.DFgameSettings.knownFreezerParts.Where(a => a.Value.vesselID == tmpid).ToList();
                                //foreach (DeepFreezer frzr in DFIntMemory.Instance.DpFrzrActVsl)
                                foreach (KeyValuePair<uint, PartInfo> frzr in frzrs)
                                {
                                    //Thawed Crew List
                                    for (int i = 0; i < frzr.Value.crewMembers.Count; i++)
                                    {
                                        GUILayout.BeginHorizontal();
                                        bool ThawCrew = KACAlarm_ThwKbls.Contains(frzr.Value.crewMembers[i]);
                                        bool FrzCrew = KACAlarm_FrzKbls.Contains(frzr.Value.crewMembers[i]);
                                        GUILayout.Label(frzr.Value.crewMembers[i], statusStyle, GUILayout.Width(KACtxtWdthKName));
                                        GUILayout.Label(frzr.Value.crewMemberTraits[i], statusStyle, GUILayout.Width(KACtxtWdthKTyp));
                                        if (FrzCrew) GUI.enabled = false;
                                        ThawCrew = GUILayout.Toggle(ThawCrew, "", ButtonStyle, GUILayout.Width(KACtxtWdthKTg1));
                                        GUI.enabled = true;
                                        if (ThawCrew)
                                        {
                                            if (!KACAlarm_ThwKbls.Contains(frzr.Value.crewMembers[i]))
                                            {
                                                KACAlarm_ThwKbls.Add(frzr.Value.crewMembers[i]);
                                            }
                                        }
                                        else
                                        {
                                            if (KACAlarm_ThwKbls.Contains(frzr.Value.crewMembers[i]))
                                            {
                                                KACAlarm_ThwKbls.Remove(frzr.Value.crewMembers[i]);
                                            }
                                        }
                                        if (ThawCrew) GUI.enabled = false;
                                        FrzCrew = GUILayout.Toggle(FrzCrew, "", ButtonStyle, GUILayout.Width(KACtxtWdthKTg2));
                                        GUI.enabled = true;

                                        if (FrzCrew)
                                        {
                                            if (!KACAlarm_FrzKbls.Contains(frzr.Value.crewMembers[i]))
                                            {
                                                KACAlarm_FrzKbls.Add(frzr.Value.crewMembers[i]);
                                            }
                                        }
                                        else
                                        {
                                            if (KACAlarm_FrzKbls.Contains(frzr.Value.crewMembers[i]))
                                            {
                                                KACAlarm_FrzKbls.Remove(frzr.Value.crewMembers[i]);
                                            }
                                        }

                                        GUILayout.EndHorizontal();
                                    }
                                    //Frozen Crew List
                                    List<KeyValuePair<string, KerbalInfo>> frzncrew = DeepFreeze.Instance.DFgameSettings.KnownFrozenKerbals.Where(f => f.Value.partID == frzr.Key && f.Value.type != ProtoCrewMember.KerbalType.Tourist).ToList();
                                    foreach (KeyValuePair<string, KerbalInfo> crew in frzncrew)
                                    {
                                        GUILayout.BeginHorizontal();
                                        bool ThawCrew = KACAlarm_ThwKbls.Contains(crew.Key);
                                        bool FrzCrew = KACAlarm_FrzKbls.Contains(crew.Key);
                                        GUILayout.Label(crew.Key, frozenStyle, GUILayout.Width(KACtxtWdthKName));
                                        GUILayout.Label(crew.Value.experienceTraitName, frozenStyle, GUILayout.Width(KACtxtWdthKTyp));
                                        if (FrzCrew) GUI.enabled = false;
                                        ThawCrew = GUILayout.Toggle(ThawCrew, "", ButtonStyle, GUILayout.Width(KACtxtWdthKTg1));
                                        GUI.enabled = true;
                                        if (ThawCrew)
                                        {
                                            if (!KACAlarm_ThwKbls.Contains(crew.Key))
                                            {
                                                KACAlarm_ThwKbls.Add(crew.Key);
                                            }
                                        }
                                        else
                                        {
                                            if (KACAlarm_ThwKbls.Contains(crew.Key))
                                            {
                                                KACAlarm_ThwKbls.Remove(crew.Key);
                                            }
                                        }
                                        if (ThawCrew) GUI.enabled = false;
                                        FrzCrew = GUILayout.Toggle(FrzCrew, "", ButtonStyle, GUILayout.Width(KACtxtWdthKTg2));
                                        GUI.enabled = true;
                                        if (FrzCrew)
                                        {
                                            if (!KACAlarm_FrzKbls.Contains(crew.Key))
                                            {
                                                KACAlarm_FrzKbls.Add(crew.Key);
                                            }
                                        }
                                        else
                                        {
                                            if (KACAlarm_FrzKbls.Contains(crew.Key))
                                            {
                                                KACAlarm_FrzKbls.Remove(crew.Key);
                                            }
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                }
                                GUILayout.EndVertical();
                                GUILayout.EndScrollView();
                                continue;
                            }
                        }
                        else  // no modify is in progress so we draw modify buttons on all alarms
                        {
                            // Utilities.Log_Debug("no modify in progress so just show modify buttons on KAC alarm");
                            if (TmeRemaining <= 0) GUI.enabled = false;
                            if (GUILayout.Button("Modify", GUILayout.Width(50)))
                            {
                                KACalarmMod = alarm;
                                KACAlarm_FrzKbls.Clear();
                                KACAlarm_ThwKbls.Clear();
                                string tmpnotes = DFIntMemory.Instance.ParseKACNotes(alarm.Notes, out KACAlarm_FrzKbls, out KACAlarm_ThwKbls);
                                ModKACAlarm = true;
                                // Utilities.Log_Debug("Modify in progress " + alarm.ID);
                            }
                            GUI.enabled = true;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(14);
            GUIContent resizeContent = new GUIContent("R", "Resize Window");
            Rect resizeRect = new Rect(DFKACwindowPos.width - 17, DFKACwindowPos.height - 17, 16, 16);
            GUI.Label(resizeRect, resizeContent, resizeStyle);
            HandleResizeEventsKAC(resizeRect);
            GUI.DragWindow();
        }

        private void windowVS(int id)
        {
            //Init styles
            sectionTitleStyle = new GUIStyle(GUI.skin.label);
            sectionTitleStyle.alignment = TextAnchor.MiddleCenter;
            sectionTitleStyle.stretchWidth = true;
            sectionTitleStyle.fontStyle = FontStyle.Bold;

            statusStyle = new GUIStyle(GUI.skin.label);
            statusStyle.alignment = TextAnchor.MiddleCenter;
            statusStyle.stretchWidth = true;
            statusStyle.normal.textColor = Color.white;

            //Pause the game
            TimeWarp.SetRate(0, true);
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready && !FlightDriver.Pause)
                FlightDriver.SetPause(true);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            //GUILayout.Box(new GUIContent("ElectricCharge is running out on vessel, you must switch to the vessel now.", "Switch to DeepFreeze vessel required"), statusStyle, GUILayout.Width(280));
            GUILayout.Box(new GUIContent(showSwitchVesselStr, showSwitchVesselStr), statusStyle, GUILayout.Width(320));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Switch to Vessel", "Switch to Vessel"), GUILayout.Width(160)))
            {
                showSwitchVessel = false;
                if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
                    FlightDriver.SetPause(false);
                //Jump to vessel code here.
                int intVesselidx = Utilities.getVesselIdx(switchVessel);
                if (intVesselidx < 0)
                {
                    Utilities.Log("Couldn't find the index for the vessel " + switchVessel.vesselName + "(" + switchVessel.id + ")");
                    showUnabletoSwitchVessel = true;
                }
                else
                {
                    if (HighLogic.LoadedSceneIsFlight)
                    {
                        FlightGlobals.SetActiveVessel(switchVessel);
                    }
                    else
                    {
                        String strret = GamePersistence.SaveGame("DFJumpToShip", HighLogic.SaveFolder, SaveMode.OVERWRITE);
                        Game tmpGame = GamePersistence.LoadGame(strret, HighLogic.SaveFolder, false, false);
                        FlightDriver.StartAndFocusVessel(tmpGame, intVesselidx);
                    }
                }
            }
            if (GUILayout.Button(new GUIContent("Not Now", "Don't switch vessel now"), GUILayout.Width(160)))
            {
                showSwitchVessel = false;
                switchVesselManual = true;
                switchVesselManualTimer = Planetarium.GetUniversalTime();
                if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
                    FlightDriver.SetPause(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void windowVSF(int id)
        {
            //Init styles
            sectionTitleStyle = new GUIStyle(GUI.skin.label);
            sectionTitleStyle.alignment = TextAnchor.MiddleCenter;
            sectionTitleStyle.stretchWidth = true;
            sectionTitleStyle.fontStyle = FontStyle.Bold;

            statusStyle = new GUIStyle(GUI.skin.label);
            statusStyle.alignment = TextAnchor.MiddleCenter;
            statusStyle.stretchWidth = true;
            statusStyle.normal.textColor = Color.white;

            TimeWarp.SetRate(0, true);
            if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
                FlightDriver.SetPause(true);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent("Automatic Switch to vessel failed.\nPlease switch manually to vessel Immediately", "Switch to DeepFreeze vessel required"), statusStyle, GUILayout.Width(280));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("OK", "OK")))
            {
                showSwitchVessel = false;
                showUnabletoSwitchVessel = false;
                switchVesselManual = true;
                switchVesselManualTimer = Planetarium.GetUniversalTime();
                if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ready)
                    FlightDriver.SetPause(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void HandleResizeEventsDF(Rect resizeRect)
        {
            var theEvent = Event.current;
            if (theEvent != null)
            {
                if (!mouseDownDF)
                {
                    if (theEvent.type == EventType.MouseDown && theEvent.button == 0 && resizeRect.Contains(theEvent.mousePosition))
                    {
                        mouseDownDF = true;
                        theEvent.Use();
                    }
                }
                else if (theEvent.type != EventType.Layout)
                {
                    if (Input.GetMouseButton(0))
                    {
                        // Flip the mouse Y so that 0 is at the top
                        float mouseY = Screen.height - Input.mousePosition.y;

                        DFwindowPos.width = Mathf.Clamp(Input.mousePosition.x - DFwindowPos.x + resizeRect.width / 2, 50, Screen.width - DFwindowPos.x);
                        DFwindowPos.height = Mathf.Clamp(mouseY - DFwindowPos.y + resizeRect.height / 2, 50, Screen.height - DFwindowPos.y);
                        DFtxtWdthName = Mathf.Round((DFwindowPos.width - 28f) / 4.2f);
                        DFtxtWdthProf = Mathf.Round((DFwindowPos.width - 28f) / 4.8f);
                        DFtxtWdthVslN = Mathf.Round((DFwindowPos.width - 28f) / 3.5f);
                        DFvslWdthName = Mathf.Round((DFwindowPos.width - 28f) / 4.2f);
                        DFvslPrtName = Mathf.Round((DFwindowPos.width - 28f) / 6.3f);
                        DFvslPrtTmp = Mathf.Round((DFwindowPos.width - 28f) / 11f);
                        DFvslPrtElec = Mathf.Round((DFwindowPos.width - 28f) / 12.3f);
                        DFvslAlarms = Mathf.Round((DFwindowPos.width - 28f) / 8f);
                        DFvslLstUpd = Mathf.Round((DFwindowPos.width - 28f) / 5.5f);
                        DFvslRT = Mathf.Round((DFwindowPos.width - 28f) / 12.3f);
                    }
                    else
                    {
                        mouseDownDF = false;
                    }
                }
            }
        }

        private void HandleResizeEventsKAC(Rect resizeRect)
        {
            var theEvent = Event.current;
            if (theEvent != null)
            {
                if (!mouseDownKAC)
                {
                    if (theEvent.type == EventType.MouseDown && theEvent.button == 0 && resizeRect.Contains(theEvent.mousePosition))
                    {
                        mouseDownKAC = true;
                        theEvent.Use();
                    }
                }
                else if (theEvent.type != EventType.Layout)
                {
                    if (Input.GetMouseButton(0))
                    {
                        // Flip the mouse Y so that 0 is at the top
                        float mouseY = Screen.height - Input.mousePosition.y;

                        DFKACwindowPos.width = Mathf.Clamp(Input.mousePosition.x - DFKACwindowPos.x + resizeRect.width / 2, 50, Screen.width - DFKACwindowPos.x);
                        DFKACwindowPos.height = Mathf.Clamp(mouseY - DFKACwindowPos.y + resizeRect.height / 2, 50, Screen.height - DFKACwindowPos.y);
                        KACtxtWdthName = Mathf.Round((DFKACwindowPos.width - 38f) / 3.5f);
                        KACtxtWdthAtyp = Mathf.Round((DFKACwindowPos.width - 38f) / 6f);
                        KACtxtWdthATme = Mathf.Round((DFKACwindowPos.width - 38f) / 5f);
                        KACtxtWdthKName = Mathf.Round((DFKACwindowPos.width - 48f) / 3f);
                        KACtxtWdthKTyp = Mathf.Round((DFKACwindowPos.width - 48f) / 5f);
                        KACtxtWdthKTg1 = Mathf.Round((DFKACwindowPos.width - 48f) / 6f);
                        KACtxtWdthKTg2 = Mathf.Round((DFKACwindowPos.width - 48f) / 6f);
                    }
                    else
                    {
                        mouseDownKAC = false;
                    }
                }
            }
        }

        #endregion GUI

        #region Savable

        //Class Load and Save of global settings
        public void Load(ConfigNode globalNode)
        {
            Utilities.Log_Debug("DeepFreezeGUI Load");
            DFwindowPos.x = DeepFreeze.Instance.DFsettings.DFwindowPosX;
            DFwindowPos.y = DeepFreeze.Instance.DFsettings.DFwindowPosY;
            CFwindowPos.x = DeepFreeze.Instance.DFsettings.CFwindowPosX;
            CFwindowPos.y = DeepFreeze.Instance.DFsettings.CFwindowPosY;
            DFKACwindowPos.x = DeepFreeze.Instance.DFsettings.DFKACwindowPosX;
            DFKACwindowPos.y = DeepFreeze.Instance.DFsettings.DFKACwindowPosY;
            DFWINDOW_WIDTH = DeepFreeze.Instance.DFsettings.DFWindowWidth;
            CFWINDOW_WIDTH = DeepFreeze.Instance.DFsettings.CFWindowWidth;
            KACWINDOW_WIDTH = DeepFreeze.Instance.DFsettings.KACWindowWidth;
            WINDOW_BASE_HEIGHT = DeepFreeze.Instance.DFsettings.WindowbaseHeight;
            Useapplauncher = DeepFreeze.Instance.DFsettings.UseAppLauncher;
            AutoRecoverFznKerbals = DeepFreeze.Instance.DFsettings.AutoRecoverFznKerbals;
            debugging = DeepFreeze.Instance.DFsettings.debugging;
            ECreqdForFreezer = DeepFreeze.Instance.DFsettings.ECreqdForFreezer;
            fatalOption = DeepFreeze.Instance.DFsettings.fatalOption;
            comatoseTime = DeepFreeze.Instance.DFsettings.comatoseTime;
            KSCcostToThawKerbal = DeepFreeze.Instance.DFsettings.KSCcostToThawKerbal;
            ECReqdToFreezeThaw = DeepFreeze.Instance.DFsettings.ECReqdToFreezeThaw;
            GlykerolReqdToFreeze = DeepFreeze.Instance.DFsettings.GlykerolReqdToFreeze;
            RegTempReqd = DeepFreeze.Instance.DFsettings.RegTempReqd;
            RegTempFreeze = DeepFreeze.Instance.DFsettings.RegTempFreeze;
            RegtempMonitor = DeepFreeze.Instance.DFsettings.RegTempMonitor;
            heatamtMonitoringFrznKerbals = DeepFreeze.Instance.DFsettings.heatamtMonitoringFrznKerbals;
            heatamtThawFreezeKerbal = DeepFreeze.Instance.DFsettings.heatamtThawFreezeKerbal;
            TempinKelvin = DeepFreeze.Instance.DFsettings.TempinKelvin;
            StripLightsOn = DeepFreeze.Instance.DFsettings.StripLightsActive;            
            Utilities.Log_Debug("DeepFreezeGUI Load end");
        }

        public void Save(ConfigNode globalNode)
        {
            Utilities.Log_Debug("DeepFreezeGUI Save");
            DeepFreeze.Instance.DFsettings.DFwindowPosX = DFwindowPos.x;
            DeepFreeze.Instance.DFsettings.DFwindowPosY = DFwindowPos.y;
            DeepFreeze.Instance.DFsettings.CFwindowPosX = CFwindowPos.x;
            DeepFreeze.Instance.DFsettings.CFwindowPosY = CFwindowPos.y;
            DeepFreeze.Instance.DFsettings.DFKACwindowPosX = DFKACwindowPos.x;
            DeepFreeze.Instance.DFsettings.DFKACwindowPosY = DFKACwindowPos.y;
            DeepFreeze.Instance.DFsettings.UseAppLauncher = Useapplauncher;
            DeepFreeze.Instance.DFsettings.AutoRecoverFznKerbals = AutoRecoverFznKerbals;
            DeepFreeze.Instance.DFsettings.debugging = debugging;
            DeepFreeze.Instance.DFsettings.ECreqdForFreezer = ECreqdForFreezer;
            DeepFreeze.Instance.DFsettings.fatalOption = fatalOption;
            DeepFreeze.Instance.DFsettings.comatoseTime = comatoseTime;
            DeepFreeze.Instance.DFsettings.KSCcostToThawKerbal = KSCcostToThawKerbal;
            DeepFreeze.Instance.DFsettings.ECReqdToFreezeThaw = ECReqdToFreezeThaw;
            DeepFreeze.Instance.DFsettings.GlykerolReqdToFreeze = GlykerolReqdToFreeze;
            DeepFreeze.Instance.DFsettings.RegTempReqd = RegTempReqd;
            DeepFreeze.Instance.DFsettings.RegTempFreeze = RegTempFreeze;
            DeepFreeze.Instance.DFsettings.RegTempMonitor = RegtempMonitor;
            DeepFreeze.Instance.DFsettings.heatamtThawFreezeKerbal = heatamtThawFreezeKerbal;
            DeepFreeze.Instance.DFsettings.heatamtMonitoringFrznKerbals = heatamtMonitoringFrznKerbals;
            DeepFreeze.Instance.DFsettings.TempinKelvin = TempinKelvin;
            DeepFreeze.Instance.DFsettings.StripLightsActive = StripLightsOn;
            Utilities.debuggingOn = debugging;
            Utilities.Log_Debug("DeepFreezeGUI Save end");
        }

        #endregion Savable
    }
}