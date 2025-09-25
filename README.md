# ETDA - Enhanced Darkages Bot Framework

> **Status:** Working for USDA Darkages Clients (www.darkages.com) *7.41*  
> [DarkAges741single.exe](https://s3.amazonaws.com/kru-downloads/da/DarkAges741single.exe)

![ETDA Screenshot](http://s32.postimg.org/ok7drfpqd/etda.png)

## 🚀 Recent Major Updates

**StateManager Interface** - Revolutionary bot management system with professional UI design, real-time filtering, and comprehensive configuration management.

**Advanced Healing Bot** - Multi-target healing system supporting self, party members, and named players with intelligent spell parsing.

**Dynamic Priority System** - Logical hierarchy system ensuring proper bot state execution order and conflict resolution.

---

## 📋 Table of Contents
- [Overview](#overview)
- [Build Instructions](#build-instructions)
- [System Requirements](#system-requirements)
- [Framework Dependencies](#framework-dependencies)
- [Project Structure](#project-structure)
- [Features](#features)
- [Development](#development)
- [Credits & Attribution](#credits--attribution)

---

## 🎯 Overview

ETDA is a memory-based Darkages hunting bot/application that operates without network-based backends. This approach provides significant advantages in response time and performance by directly interfacing with the game client's memory rather than intercepting and processing network packets.

### Why Memory-Based?
Unlike traditional network bots that require socket layers, packet parsing, encryption/decryption, and buffering - ETDA bypasses all this overhead by calling the client's functions directly. This results in:
- **Faster response times** - No network latency or packet processing delays
- **Better reliability** - Reduced disconnection issues during long botting sessions  
- **Lower overhead** - Direct memory access vs. complex packet manipulation
- **Inline execution** - Let the client do the work it's already designed to do

---

## 🔧 Build Instructions

### Prerequisites
1. **Visual Studio 2019 or later** with C++ and C# workloads
2. **Windows SDK 10.0** or later
3. **.NET Framework 4.7.2** or higher
4. **Git** for version control

### Step-by-Step Build Process

#### 1. Clone the Repository
```bash
git clone https://github.com/tech005/ETDA.git
cd ETDA
```

#### 2. Restore NuGet Packages
```bash
# In Visual Studio Package Manager Console or command line with NuGet CLI
nuget restore BotCore.sln
```

#### 3. Build Order
**Important:** Build projects in this specific order due to dependencies:

1. **EtDA (C++ Project)**
   ```bash
   # Build the native DLL first
   msbuild EtDA\EtDA.vcxproj /p:Configuration=Release /p:Platform=x86
   ```

2. **BotCore (C# Library)**
   ```bash
   # Build the core bot framework
   msbuild BotCore\BotCore.csproj /p:Configuration=Release /p:Platform=x86
   ```

3. **Bot (C# Application)**
   ```bash
   # Build the main application
   msbuild Bot\Bot.csproj /p:Configuration=Release /p:Platform=x86
   ```

#### 4. Complete Solution Build
```bash
# Or build everything at once (after NuGet restore)
msbuild BotCore.sln /p:Configuration=Release /p:Platform=x86
```

### Visual Studio Build
1. Open `BotCore.sln` in Visual Studio
2. Set solution configuration to **Release** and platform to **x86**
3. Right-click solution → **Restore NuGet Packages**
4. Build → **Build Solution** (F6)

---

## 💻 System Requirements

### Operating System
- **Windows 10** or later (x86/x64)
- **Windows 8.1** (limited testing)

### Hardware Requirements
- **CPU:** Intel/AMD x86 compatible processor
- **RAM:** 4GB minimum, 8GB recommended
- **Storage:** 100MB free space
- **Graphics:** DirectX 9.0c compatible

### Game Client Requirements
- **Dark Ages Client 7.41** ([Download](https://s3.amazonaws.com/kru-downloads/da/DarkAges741single.exe))
- Must run in **x86 mode** (32-bit compatibility)

---

## 📦 Framework Dependencies

### .NET Framework Components
```xml
<TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
```

### NuGet Packages
- **log4net 2.0.5** - Logging framework
- **PostSharp 4.2.21** - Aspect-oriented programming
- **PostSharp.Patterns.Common 4.2.21** - Common patterns
- **PostSharp.Patterns.Diagnostics 4.2.21** - Diagnostic patterns
- **PostSharp.Patterns.Diagnostics.Log4Net 4.2.21** - Log4Net integration
- **PostSharp.Patterns.Model 4.2.21** - Model patterns
- **PostSharp.Patterns.Threading 4.2.21** - Threading patterns

### External Libraries
- **MemorySharp.dll** - Memory manipulation library by ZenLulz
- **Fasm.NET.dll** - Flat Assembler .NET wrapper
- **OpenGL libraries** - Graphics rendering support

### C++ Runtime Requirements
- **Microsoft Visual C++ Redistributable** for Visual Studio 2019 (x86)
- **Windows Universal CRT**

---

## 📁 Project Structure

```
ETDA/
├── EtDA/                           # Native C++ DLL Project
│   ├── Darkages.cpp               # Core Dark Ages integration
│   ├── main.cpp                   # DLL entry point
│   ├── GameFunctions.h            # Game function definitions
│   └── ...
├── BotCore/                       # C# Bot Framework
│   ├── Actions/                   # Game action implementations
│   ├── BotForms/                  # UI Forms and Dialogs
│   │   ├── StateManager.cs       # NEW: Revolutionary state management
│   │   ├── BotInterface.cs       # Main bot interface
│   │   └── ...
│   ├── States/BotStates/          # Bot behavior states
│   │   ├── HealingBot.cs         # ENHANCED: Multi-target healing
│   │   ├── CheckIfFased.cs       # Fas spell management
│   │   └── ...
│   ├── Components/                # Modular bot components
│   ├── Client/                    # Game client integration
│   └── ...
├── Bot/                          # Main Application
│   ├── MainForm.cs               # Application entry point
│   └── ...
├── packages/                     # NuGet packages
└── Documentation/
    ├── CHANGELOG.md              # Detailed change history
    └── PULL_REQUEST.md          # Feature summary
```

---

## ✨ Features

### 🎮 ETDA Core Features
- **Direct Memory Access** - No packet encryption/decryption overhead
- **Packet Injection** - Client/Server packet injection without network layer
- **Packet Interception** - Filter and modify packets in real-time
- **Chat Integration** - Advanced chat handling and automation
- **Freeze-Free Debugging** - Debug without client disconnection
- **Memory Pattern Scanning** - Dynamic address resolution

### 🎯 Game Interaction
- **Click-to-Move** - Precise movement control via Point invocation
- **Cursor Management** - SetCursor(Point) for UI interaction
- **Pathfinding** - WalkTo(Point A, Point B) with obstacle avoidance
- **Spell Casting** - Direct spell invocation with target validation
- **Equipment Management** - Automated gear and inventory handling

### 🎨 Visual Overlays
- **GDI/GDI++ Support** - Hardware-accelerated overlay rendering
- **HUD System** - Customizable heads-up display with name/arc display
- **Bitmap Rendering** - Support for custom graphics and textures
- **Text Overlays** - Real-time information display
- **Debug Visualization** - Visual debugging tools and state indicators

### 🤖 BotCore Framework
- **StateManager Interface** 🆕 - Revolutionary UI with split-panel design (600px/296px)
  - Real-time filtering and priority-based grouping
  - Visual state indicators and comprehensive configuration
  - XML-based per-player settings persistence
  
- **Advanced Healing Bot** 🆕 - Multi-target healing system
  - Self, party, and named player support
  - Intelligent spell parsing with regex matching
  - Priority-based targeting with range validation
  
- **Dynamic Priority System** 🆕 - Logical state hierarchy
  - Emergency (95+) → Healing (85-94) → Combat (70-84) → Buffs (50-69) → Movement (10-49)
  - StateAttribute integration with DefaultPriority
  - Conflict resolution and execution order management

### 🔧 Architecture Features
- **State Machine Based** - Fast response times with efficient state transitions
- **Component Based** - Modular design for easy customization and extension
- **Plugin System** - Support for custom states, plugins, and components
- **No Scripting Required** - Visual configuration through advanced UI
- **Multi-Class Support** - Every Dark Ages class including subpaths
- **Dynamic Pathfinding** - Intelligent navigation and route optimization

---

## 🛠️ Development

### Getting Started
1. Follow the [Build Instructions](#build-instructions) above
2. Launch Dark Ages client (7.41)
3. Run Bot.exe as Administrator
4. Use the StateManager interface to configure bot behavior
5. Start automation and monitor through the comprehensive UI

### Configuration
- **Per-Player Settings** - XML-based configuration automatically saved
- **State Management** - Use the StateManager for visual configuration
- **Priority System** - Leverage the dynamic priority hierarchy for optimal performance
- **Debugging** - Enable debug output for detailed execution logging

### Contributing
This project welcomes contributions! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes with proper documentation
4. Submit a pull request with detailed description

### Troubleshooting
- **Build Errors** - Ensure all NuGet packages are restored and build order is followed
- **Runtime Issues** - Run as Administrator and check Dark Ages client version
- **Memory Access** - Verify Windows Defender/Antivirus exclusions for memory access

---

## 🏆 Credits & Attribution

### Original Development
- **Shynd** - Taught the fundamentals of memory-based botting. The foundation of this approach is derived from his teachings. "DA Procuration!"
- **ZenLulz** - Creator of the fantastic MemorySharp library that powers our memory operations
- **Acht/Kyle** - Provided the elegant RegexMessageHandler code found in public bot implementations
- **Huy/Jimmy** - Contributed to preliminary state implementations and core framework development

### Recent Enhancement Development
A significant portion of the recent enhancements and modernization efforts were developed with the assistance of:

- **GitHub Copilot** - AI pair programming assistant that accelerated development
- **Claude 3.5 Sonnet (Anthropic)** - Advanced language model that provided architectural guidance, code review, and comprehensive documentation support

The StateManager interface, advanced healing system, dynamic priority management, and extensive documentation were collaboratively developed using these AI tools, demonstrating the power of human-AI collaboration in software development.

### Libraries and Dependencies
- **Microsoft .NET Framework** - Application runtime and base class libraries
- **PostSharp** - Aspect-oriented programming framework for cross-cutting concerns
- **log4net** - Flexible logging framework for debugging and monitoring
- **Fasm.NET** - Flat Assembler integration for low-level operations
- **OpenGL** - Graphics rendering pipeline for overlay systems

---

## ⚖️ Legal Notice

This program is developed for **educational purposes only**. All code is released as public domain, however:

- Please provide proper attribution when using or modifying this code
- Respect the intellectual property rights of game developers
- Use responsibly and in accordance with applicable terms of service
- The developers assume no responsibility for misuse of this software

---

## 🔗 Links

- **Game Client**: [DarkAges741single.exe](https://s3.amazonaws.com/kru-downloads/da/DarkAges741single.exe)
- **Official Game**: [www.darkages.com](http://www.darkages.com)
- **Documentation**: See `CHANGELOG.md` for detailed development history
- **Issues**: Report bugs and feature requests through GitHub Issues

---

*Last Updated: September 25, 2025 - Major Enhancement Release*