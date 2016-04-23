using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using NppPluginNET;
using NppXmlTreeviewPlugin.Forms;
using NppXmlTreeviewPlugin.Properties;

namespace NppXmlTreeviewPlugin
{
    class Main
    {
        #region " Fields "
        internal const string PluginName = "Npp Xml Treview";
        static string iniFilePath = null;
        static bool someSetting = false;
        public static FormTreeView frmMyDlg = null;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Resources.star;
        static Bitmap tbBmp_tbTab = Resources.star_bmp;
        static Icon tbIcon = null;
        #endregion

        #region " StartUp/CleanUp "
        static Main()
        {
            // Following code let's you reference any additional .net assembly
            // from a subfolder which has the same name as your plugin. Example:
            // ...\plugins\NppXmlTreeviewPlugin.dll
            // ...\plugins\NppXmlTreeviewPlugin\AdditionalAssembly1.dll
            // ...\plugins\NppXmlTreeviewPlugin\AdditionalAssembly2.dll
            AppDomain.CurrentDomain.AssemblyResolve += LoadFromPluginSubFolder;
        }

        static Assembly LoadFromPluginSubFolder(object sender, ResolveEventArgs args)
        {
            string pluginPath = typeof(Main).Assembly.Location;
            string pluginName = Path.GetFileNameWithoutExtension(pluginPath);
            string pluginSubFolder = Path.Combine(Path.GetDirectoryName(pluginPath), pluginName);
            string assemblyPath = Path.Combine(pluginSubFolder, new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            else
            {
                // HACK: allow loading of session files created with older assembly versions
                Assembly asm = typeof(Main).Assembly;
                if (args.Name.Contains("NppXmlTreeviewPlugin, Version=") && (args.Name != asm.FullName))
                    return typeof(Main).Assembly;
            }
            return null;
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            PluginBase.SetCommand(0, "Npp XML TreeView", myDockableDialog); idMyDlg = 1;
            PluginBase.SetCommand(1, "About NppXMLTreeView", myMenuFunction, new ShortcutKey(false, false, false, Keys.None));

        }

        internal static void SetToolBarIcon()
        {
            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
        }
        #endregion

        #region " Menu functions "

        internal static void myMenuFunction()
        {
            MessageBox.Show("NppTreeView, License under Apache 2.0 (http://www.apache.org/licenses/LICENSE-2.0)\r\n" +
                            "Created by João Rosa\r\n\r\n" +
                            "Source code hosted in https://github.com/joaoasrosa/nppxmltreeview \r\n" +
                            "Please feel free to contribute.");
        }

        internal static void myDockableDialog()
        {
            if (frmMyDlg == null)
            {
                frmMyDlg = new FormTreeView();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap
                    {
                        OldColor = Color.Blue,
                        NewColor = Color.FromKnownColor(KnownColor.ButtonFace)
                    };
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmMyDlg.Handle;
                _nppTbData.pszName = "XML Treeview";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, NppMsg.NPPM_DMMSHOW, 0, frmMyDlg.Handle);
            }
        }

        #endregion

        #region " Logging "
        internal static void ErrorOut(Exception ex)
        {
            TRACE(ex.Message);
            try
            {
                using (TextWriter w = new StreamWriter("c:\\temp\\log.text", true))
                {
                    w.WriteLine(string.Format(
                        "\n{0}-{1}-{2} {3}-{4}-{5}:\n" +
                        "====================",
                        DateTime.Now.Year,
                        DateTime.Now.Month.ToString("00"),
                        DateTime.Now.Day.ToString("00"),
                        DateTime.Now.Hour.ToString("00"),
                        DateTime.Now.Minute.ToString("00"),
                        DateTime.Now.Second.ToString("00")));
                    w.WriteLine(ex.Message);
                    w.WriteLine(ex.StackTrace);
                }
                MessageBox.Show("Owing to unfortunate circumstances an error with the following message occured:\n\n"
                                + "\"" + ex.Message + "\"\n\n"
                                + "Hence a logfile has been written to the SourceCookifier folder.\n"
                                + "Please post its content in the forum, if you think it's worth being fixed.\n"
                                + "Sorry for the inconvenience.",
                                "SourceCookifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while attempting to write error logfile:\n" + e.Message,
                                "SourceCookifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [Conditional("TRACE_ALL")]
        internal static void TRACE(string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(TRACEPATH))
                {
                    TRACEPATH = Environment.ExpandEnvironmentVariables("%SYSTEMDRIVE%\\NppXmlTreeview.TRACE.txt");
                    TRACEPID = Process.GetCurrentProcess().Id.ToString("X04");
                }
                using (TextWriter w = new StreamWriter(TRACEPATH, true))
                {
                    StackTrace stackTrace = new StackTrace();
                    MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();
                    w.WriteLine(string.Format("{0}:{1:000} <{2}> {3}[{4}] {5}",
                        DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, TRACEPID,
                        new String(' ', (stackTrace.FrameCount - 1) * 3), methodBase.Name, msg));
                }
            }
            catch (Exception ex)
            {
                if (!TRACEERRORSHOWN)
                {
                    TRACEERRORSHOWN = true;
                    MessageBox.Show("Error while attempting to write trace file:\n" + ex.Message,
                                    "SourceCookifier", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        static string TRACEPATH, TRACEPID;
        static bool TRACEERRORSHOWN;
        #endregion
    }
}