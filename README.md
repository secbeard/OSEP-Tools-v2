# Tools

Powerinject.py - Python3 script to generate .PS1 payloads that perform process injection.

Powerhollow.py - Python3 script to generate .PS1 payloads that perform process hollowing with PPID spoofing.

Formatshellcode.py - Python3 script to format C# shellcode output by msfvenom into proper format for use with Builder.exe.

Port_ipeggs.py - Python3 script to format C# shellcode output by msfvenom into proper format for user with Builder.exe. FOR USE WITH CLI PAYLOADS!

Builder - C# project that compiles to Builder.exe which will craft different .exe/.dll payloads from Template.cs files in other projects.

Hollow - C# project that compiles to Hollow.exe which performs process hollowing with PPID spoofing.

Runnerinject - C# project that compiel to Runnerinject.exe which performs process injection.

x64_met_staged_reversetcp_inject.exe - Command line args: IP PORT PROCESS_TO_INJECT(explorer)

x64_met_staged_reversetcp_hollow.exe - Command line args: IP PORT PROCESS_TO_HOLLOW(c:\\windows\\system32\\svchost.exe) PPID_SPOOF(explorer) 

sql.exe - x64 application for exploitation of linked SQL servers

pscradle.docm - Word doc with caeser cipher encoding that calls powershell download cradle.  Use with vbobfuscate.ps1 to generate and replace obfuscated text in pscradle.docm.

vbobfuscate.ps1 - ps1 to generate caeser cipher code for pscradle.  Make sure offsets match for encrypt/decrypt. First output is download cradle, last is app name for app name check before running. 

# NOTES

With Powerinject/Powerhollow make sure you think about whether you will be calling PS download cradle from powershell or cmd.exe and use the appropriate mode when constructing payloads.  When you call powershell.exe <cradle> from cmd.exe or even from another powershell window, you are creating a child process and while the embedded AMSI bypass may work for the child process the parent process will detect the child performing malicious actions and flag it.
  
Do NOT use msfvenom encoders with any Hollowing tool. Causes problems.
  
Injection tools:
    Your target for injection must be of the same integrity or lower than the method by which you have code execution.  I.e. if you are running in medium integrity you cannot inject into spoolsv, inject into explorer.
  
Hollowing tools:
    Your target parent process for PPID spoofing must be of the same integrity or lower than the method by which you have code execution. I.e. if you are running in medium integrity you cannot specify spoolsv as the parent process.  Hollowed process will inherity integrity of parent process.
  
  On Word Macros:
  
  WordMacroRunner - This is a baseline runner that will return a shell from WINWORD.exe.
  
  WordMacroInject - This macro performs process injection.  Currently specified for explorer.exe. NOTE: This runner is really only good for 64-bit word.  Seeing as we have no idea what version of word an organization will be running, the use case for this is limited.  The issue stems from the fact that 32 bit processes cannot easily inject into 64 bit ones; The presumed typical target environment will be running 32 bit word on a 64 bit OS, which renders the injection into explorer impossibly.  There are advanced techniques out there that might be able to facilitate this (Heaven's gate) but no idea if they could be implemented in VBA. Additionally there is no telling what/if any other 32 bit processes suitable for injetion might be running on a target machine.  In theory code could be written to enumerate running 32 bit processes and then just try to inject into an arbitrary one, but there are obvious issues concerning stability, and longevity of the process to maintain a reverse shell.  In reality just using a non-injecting runner and then setting up a C2 to automigrate is probably best practice as they are equipped to do so.
  
  To DOs: Detect word version (2019 vs earlier) in order to go ahead and patch amsi or not. Detect word architecture/include shellcode for both x64 and x86 word so we can get a good shell no matter how we land.
  
  Setup/formatting information:
  1. Write "legitimate" contents of the word doc, select all, then navigate to Insert > Quick Parts > AutoTexts and Save Selection to AutoText Gallery
  2. Give it a name, make sure it's saved to that particular document and not a template. Hit ok. Then delete the content from the body of the word doc.
  3. Copy in/write your pretexting content to the body of the word doc.  This is the piece that include "enable macros, hit this key combo to execute" etc.
  4. 
