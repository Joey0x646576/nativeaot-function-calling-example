### Inject your Native AOT assembly and perform internal function calling within the injected context
This project leverages the .NET ahead-of-time compiler to inject a natively compiled assembly into a Windows process and execute functions from within the injected context. The context in this case is **notepad.exe**.

# Description
DLL injection is usually associated with C++, where you create and inject DLLs to interact with process internals. This project demonstrates that .NET in combination with Native AOT compilation, is just as capable! Since there aren't many examples on this subject I decided to go more in depth about it on my [blog post](https://joeysenna.com/posts/nativeaot-internal-function-calling), feel free to read it!

### How to

1) Publish the project
`dotnet publish -r win-x64 -c Release`
3) Inject the dll in **notepad.exe** using LoadLibrary or tools such as Process Hacker which support this.

The example contains some simple interactions with the process:
* Search
* JumpToLine
* Reading variables

## Remarks
> [!WARNING]  
> This is done on notepad.exe(**10.0.19041.3996**) on Windows 10 22h2.

# Preview
![2024-04-2719-48-42](https://github.com/Joey0x646576/nativeaot-function-calling-example/assets/9116413/aec849bc-3d5a-458e-90a0-3f060cb053c4)
