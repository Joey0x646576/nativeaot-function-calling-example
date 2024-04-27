using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace NativeAOT.FunctionCalling
{
    /// <summary>
    /// This example is based on notepad.exe(10.0.19041.3996) on Windows 10 22h2.
    /// </summary>
    internal unsafe class NotepadFunctions(nint baseAddress)
    {
        /// <summary>
        /// Jumps to the given lineNumber, if available. (ctrl + g functionality)
        /// </summary>
        /// <param name="lineNumber">The lineNumber to focus on.</param>
        /// <remarks>
        /// void __fastcall sub_140007E58(int a1)
        /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/function-pointers#function-pointers-1
        /// </remarks>
        public void JumpToLine(int lineNumber)
        {
            var jumpToLineAddress = baseAddress + 0x7E98;
            var jumpToLine = (delegate* unmanaged<int, void>)jumpToLineAddress; // You can specify the calling convention.

            jumpToLine(lineNumber);
        }

        public void PrintCursorPosition()
        {
            ForceStatusBarUpdate();

            var cursorPosition = (CursorPosition*)(baseAddress);
            Console.WriteLine($"LineNumber: {cursorPosition->LineNumber} - ColumnNumber: {cursorPosition->ColumnNumber}");
        }

        // If the UI is not in focus or active the status bar won't update.
        private void ForceStatusBarUpdate()
        {
            var forceStatusBarUpdateAddress = baseAddress + 0xB324;
            var forceStatusBarUpdate = (delegate* unmanaged<bool, void>)forceStatusBarUpdateAddress;

            // if ( a1 || v10 != dword_1400320E0 || v8 != dword_1400320E4 )
            forceStatusBarUpdate(true); // Directly assigning a1, but should function fine without.
        }

        // Notepad search function doesn't actually take in a search string.
        // As the search string is stored in regedit(interesting!) and then read from. 

        // So to make searching work, we have several options;
        // 1) Internally write to the static variable.
        // 2) Externally overwrite from memory.
        // 3) Write to the registery key & call loading of global variables func.

        // Will show using option 1.

        /// <summary>
        /// Tries to find given input inside the text view. (ctrl + f functionality)
        /// </summary>
        /// <param name="query">The text to search on.</param>
        /// <param name="searchDirection"><see cref="SearchDirection"/></param>
        /// <remarks>char __fastcall sub_14001C834(__int64 a1, char a2)</remarks>
        public void Search(string query, SearchDirection searchDirection)
        {
            ModifySearchTerm(query);

            var searchAddress = baseAddress + 0x1C834;
            var search = (delegate* unmanaged<nint, SearchDirection, char>)searchAddress;

            var result = search(nint.Zero, searchDirection) == 1;

            Console.WriteLine($"hasResult: {result}");
        }

        // wchar_t StringValue[128]
        private void ModifySearchTerm(string query)
        {
            var bufferSize = 128 * 2; // 128 characters, 256 bytes for 16-bit.
            var querySize = Encoding.Unicode.GetByteCount(query);

            if (querySize > bufferSize)
            {
                return;
            }

            var queryBuffer = stackalloc char[bufferSize];

            _ = Encoding.Unicode.GetBytes(query, new Span<byte>(queryBuffer, bufferSize));

            var searchQueryAddress = (char*)(baseAddress + 0x32470);
            Unsafe.CopyBlock(searchQueryAddress, queryBuffer, (uint)bufferSize);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct CursorPosition
    {
        [FieldOffset(0x320E0)]
        public int ColumnNumber;

        [FieldOffset(0x320E4)]
        public int LineNumber;
    }

    internal enum SearchDirection
    {
        Forward,
        Reverse
    }
}
