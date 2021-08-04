using System;
using System.Runtime.InteropServices;

namespace DeckSurf.SDK.Models
{
    /// <summary>
    /// Gets an HBITMAP that represents an <see href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nn-shobjidl_core-ishellitem">IShellItem</see>.
    /// Refer to <see href="https://docs.microsoft.com/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellitemimagefactory-getimage">Microsoft documentation</see> on the enum.
    /// </summary>
    [Flags]
    public enum SIIGBF
    {
        /// <summary>
        /// Shrink the bitmap as necessary to fit, preserving its aspect ratio.
        /// </summary>
        SIIGBF_RESIZETOFIT = 0x00,

        /// <summary>
        /// Passed by callers if they want to stretch the returned image themselves.
        /// </summary>
        SIIGBF_BIGGERSIZEOK = 0x01,

        /// <summary>
        /// Return the item only if it is already in memory.
        /// </summary>
        SIIGBF_MEMORYONLY = 0x02,

        /// <summary>
        /// Return only the icon, never the thumbnail.
        /// </summary>
        SIIGBF_ICONONLY = 0x04,

        /// <summary>
        /// Return only the thumbnail, never the icon.
        /// </summary>
        SIIGBF_THUMBNAILONLY = 0x08,

        /// <summary>
        /// Allows access to the disk, but only to retrieve a cached item.
        /// </summary>
        SIIGBF_INCACHEONLY = 0x10,

        /// <summary>
        /// Introduced in Windows 8. If necessary, crop the bitmap to a square.
        /// </summary>
        SIIGBF_CROPTOSQUARE = 0x20,

        /// <summary>
        /// Introduced in Windows 8. Stretch and crop the bitmap to a 0.7 aspect ratio.
        /// </summary>
        SIIGBF_WIDETHUMBNAILS = 0x40,

        /// <summary>
        /// Introduced in Windows 8. If returning an icon, paint a background using the associated app's registered background color.
        /// </summary>
        SIIGBF_ICONBACKGROUND = 0x80,

        /// <summary>
        /// Introduced in Windows 8. If necessary, stretch the bitmap so that the height and width fit the given size.
        /// </summary>
        SIIGBF_SCALEUP = 0x100,
    }

    /// <summary>
    /// Requests the form of an item's display name to retrieve through <see href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nf-shobjidl_core-ishellitem-getdisplayname">IShellItem::GetDisplayName</see> and <see href="https://docs.microsoft.com/windows/desktop/api/shobjidl_core/nf-shobjidl_core-shgetnamefromidlist">SHGetNameFromIDList</see>.
    /// Refer to <see href="https://docs.microsoft.com/windows/win32/api/shobjidl_core/ne-shobjidl_core-sigdn">Microsoft documentation</see> on the enum.
    /// </summary>
    internal enum SIGDN : uint
    {
        SIGDN_NORMALDISPLAY = 0x00000000,
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
        SIGDN_FILESYSPATH = 0x80058000,
        SIGDN_URL = 0x80068000,
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
        SIGDN_PARENTRELATIVE = 0x80080001,
        SIGDN_PARENTRELATIVEFORUI = 0x80094001,
    }

    /// <summary>
    /// An integer value that indicates the result or status of an operation.
    /// Refer to <see href="https://docs.microsoft.com/openspecs/windows_protocols/ms-erref/6b46e050-0761-44b1-858b-9b37a74ca32e#gt_799103ab-b3cb-4eab-8c55-322821b2b235">Microsoft documentation</see> on the enum.
    /// </summary>
    internal enum HResult
    {
        S_OK = 0x0000,
        S_FALSE = 0x0001,
        E_INVALIDARG = unchecked((int)0x80070057),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_FAIL = unchecked((int)0x80004005),
        E_NOT_FOUND = unchecked((int)0x80070490),
        E_ELEMENTNOTFOUND = unchecked((int)0x8002802B),
        E_NOOBJECT = unchecked((int)0x800401E5),
        E_CANCELLED = unchecked((int)0x800704C7),
        E_BUSY = unchecked((int)0x800700AA),
        E_ACCESSDENIED = unchecked((int)0x80030005),
    }

    /// <summary>
    /// Exposes methods that retrieve information about a Shell item.
    /// Refer to <see href="https://pinvoke.net/default.aspx/Interfaces/IShellItem.html">PInvoke</see> and the <see href="https://docs.microsoft.com/windows/win32/api/shobjidl_core/nn-shobjidl_core-ishellitem">Microsoft documentation</see> for details on this interface.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Interface does not need to match file name because the file contains more native structures than one.")]
    internal interface IShellItem
    {
        void BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

        void GetParent(out IShellItem ppsi);

        void GetDisplayName(SIGDN sigdnName, out IntPtr ppszName);

        void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);

        void Compare(IShellItem psi, uint hint, out int piOrder);
    }

    /// <summary>
    /// Exposes a method to return either icons or thumbnails for Shell items. If no thumbnail or icon is available for the requested item, a per-class icon may be provided from the Shell.
    /// Refer to <see href="https://docs.microsoft.com/windows/win32/api/shobjidl_core/nn-shobjidl_core-ishellitemimagefactory">Microsoft documentation</see> on the interface.
    /// </summary>
    [ComImport]
    [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItemImageFactory
    {
        [PreserveSig]
        HResult GetImage([In, MarshalAs(UnmanagedType.Struct)] SIZE size, [In] SIIGBF flags, [Out] out IntPtr phbm);
    }

    /// <summary>
    /// The SIZE structure specifies the width and height of a rectangle.
    /// Refer to <see href="https://docs.microsoft.com/previous-versions/dd145106(v=vs.85)">Microsoft documentation</see> on the struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        private int cx;
        private int cy;

        public int Width { set { this.cx = value; } }

        public int Height { set { this.cy = value; } }
    }

    /// <summary>
    /// The RGBQUAD structure describes a color consisting of relative intensities of red, green, and blue.
    /// Refer to <see href="https://docs.microsoft.com/windows/win32/api/wingdi/ns-wingdi-rgbquad">Microsoft documentation</see> on the struct.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Matches Windows API setup.")]
    internal struct RGBQUAD
    {
        /// <summary>
        /// The intensity of blue in the color.
        /// </summary>
        public byte rgbBlue;

        /// <summary>
        /// The intensity of green in the color.
        /// </summary>
        public byte rgbGreen;

        /// <summary>
        /// The intensity of red in the color.
        /// </summary>
        public byte rgbRed;

        /// <summary>
        /// This member is reserved and must be zero.
        /// </summary>
        public byte rgbReserved;
    }
}
