using System.Runtime.InteropServices;

namespace UnityEngine.Purchasing
{
	internal delegate void UnityNativePurchasingCallback([In][MarshalAs(UnmanagedType.LPStr)] string subject, [In][MarshalAs(UnmanagedType.LPStr)] string payload, [In][MarshalAs(UnmanagedType.LPStr)] string receipt, [In][MarshalAs(UnmanagedType.LPStr)] string transactionId);
}
