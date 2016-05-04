using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Web.UI.Design.WebControls;
using System.Windows.Forms;
using NppPlugin.DllExport;
using NppPluginNET;

namespace NppXmlTreeviewPlugin
{
    class UnmanagedExports
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static bool isUnicode()
        {
            return true;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static void setInfo(NppData notepadPlusData)
        {
            PluginBase.nppData = notepadPlusData;
            Main.CommandMenuInit();
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static IntPtr getFuncsArray(ref int nbF)
        {
            nbF = PluginBase._funcItems.Items.Count;
            return PluginBase._funcItems.NativePointer;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static uint messageProc(uint Message, IntPtr wParam, IntPtr lParam)
        {
            return 1;
        }

        static IntPtr _ptrPluginName = IntPtr.Zero;
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static IntPtr getName()
        {
            if (_ptrPluginName == IntPtr.Zero)
                _ptrPluginName = Marshal.StringToHGlobalUni(Main.PluginName);
            return _ptrPluginName;
        }

        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        static void beNotified(IntPtr notifyCode)
        {
            SCNotification nc = (SCNotification)Marshal.PtrToStructure(notifyCode, typeof(SCNotification));
            switch (nc.nmhdr.code)
            {
                case (uint)NppMsg.NPPN_TBMODIFICATION:
                    PluginBase._funcItems.RefreshItems();
                    Main.SetToolBarIcon();
                    break;
                case (uint)NppMsg.NPPN_SHUTDOWN:
                    Main.PluginCleanUp();
                    Marshal.FreeHGlobal(_ptrPluginName);
                    break;
                case (uint)NppMsg.NPPN_FILESAVED:
                case (uint)NppMsg.NPPN_FILEOPENED:
                case 4294967294:
                    if (null == Main.frmMyDlg)
                    {
                        return;
                    }
                    Main.frmMyDlg.UpdateUserInterface();
                    break;
                case (uint)SciMsg.SCN_MODIFIED:
                    if (null == Main.frmMyDlg || (nc.modificationType != 1048576 && nc.modificationType != 2064))
                    {
                        return;
                    }

                    Main.frmMyDlg.UpdateUserInterface();
                    break;
            }
        }
    }
}
