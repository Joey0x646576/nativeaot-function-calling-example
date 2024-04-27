using System.Runtime.InteropServices;

namespace NativeAOT.FunctionCalling
{
    internal partial class EntrypointHandler
    {
        private const uint DllProcessDetach = 0,
            DllProcessAttach = 1,
            DllThreadAttach = 2,
            DllThreadDetach = 3;

        [UnmanagedCallersOnly(EntryPoint = "DllMain")]
        public static bool DllMain(nint module, uint reason, nint reserved)
        {
            switch (reason)
            {
                case DllProcessDetach: break;
                case DllProcessAttach:
                    Task.Run(Execute);
                    break;
                case DllThreadAttach:
                case DllThreadDetach: break;
            }
            return true;
        }

        public static async Task Execute()
        {
            AllocConsole();

            var notepadFunctions = new NotepadFunctions(GetModuleHandle(default!));

            notepadFunctions.JumpToLine(3);
            notepadFunctions.PrintCursorPosition();

            await Task.Delay(1000);

            notepadFunctions.Search("Some Text", SearchDirection.Forward);
            notepadFunctions.PrintCursorPosition();
        }

        [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleA")]
        public static partial nint GetModuleHandle([MarshalAs(UnmanagedType.LPStr)] string moduleName);

        [LibraryImport("kernel32.dll", EntryPoint = "AllocConsole", StringMarshalling = StringMarshalling.Utf16)]
        public static partial nint AllocConsole();
    }
}
