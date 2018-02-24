# Silphy Bot

This is a discord bot written in C# using the Discord .NET framework.

## Syntax Info

* Module classes must be located inside the `Modules` folder.
* The namespace must be `<namespace>.Modules` (see source for example)
* Module classes must be explicitly defined as public or they will not be included.
* Module classes must inherit from `ModuleBase<>`, prefferably from `ModuleBase<SocketCommandContext>`.