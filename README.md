# Design Space Exploration - Package & Project Inventory

## Project Overview

**Repository**: Design Space Exploration (DSE) Toolkit  
**Primary Language**: C# 8.0  
**Target Framework**: .NET Framework 4.8  
**IDE**: Visual Studio 2017+ (Format Version 12.00)  
**Main Solution**: `AllProjects.sln`  
**Total Projects**: 23 projects in main solution  

### Purpose
A comprehensive design space exploration toolkit with multi-objective optimization capabilities, primarily built as Grasshopper plugins for Rhino 3D, featuring advanced mathematical computing and windows UI frameworks.

---

## Main Projects & Assembly Versions

### Core Libraries
| Project | Description | Version | Type |
|---------|-------------|---------|------|
| **DSECommon** | Design Space Exploration Common Library | - | Library |
| **JMetalCSharp** | Metaheuristic algorithms library | - | Library |
| **JMetalRunners** | JMetal algorithm runners | - | Library |
| **StructureEngineCS_StormCloud** | Structural analysis engine | 1.0.0.0 | Library |
| **NetDxf** | DXF file handling library | - | Library |

### Optimization & Analysis
| Project | Description | Version | Type |
|---------|-------------|---------|------|
| **MOO** | Multi-Objective Optimization | 1.0.0.0 | Grasshopper Component |
| **DSOpt** | Design Space Optimization | 1.0.0.0 | Grasshopper Component |
| **Radical** | Advanced optimization interface | - | Grasshopper Component |
| **Gradient_MOO** | Gradient-based multi-objective optimization | - | Grasshopper Component |

### Grasshopper Components
| Project | Description | Version | Type |
|---------|-------------|---------|------|
| **Cluster** | Clustering algorithms | - | .gha |
| **Capture** | Data capture component | - | .gha |
| **Reader** | Data reading component | - | .gha |
| **Writer** | Data writing component | - | .gha |
| **Sampler** | Sampling algorithms | - | .gha |
| **Sift** | Data filtering component | - | .gha |
| **Effects** | Visual effects component | - | .gha |
| **Diversity** | Diversity analysis component | - | .gha |
| **Contort** | Geometric manipulation component | - | .gha |
| **DesignLogger** | Design logging component | - | .gha |
| **Tilde** | Additional functionality component | - | .gha |
| **Stepper** (StepperAux) | Step-by-step design exploration | - | .gha |

---

## NuGet Packages & Versions

### Primary Framework Dependencies
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **RhinoCommon** | `8.0.23304.9001` | 18+ projects |
| **Grasshopper** | `8.0.23304.9001` | 15+ projects |

### Mathematical & Scientific Computing
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **MathNet.Numerics** | `5.0.0` | DSOpt, Radical, Sampler, StructureEngineCS_StormCloud |

### Optimization Libraries
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **NLoptNet** | `1.4.3` | DSOpt, Radical, StepperAux |

### Logging
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **log4net** | `3.0.2` | MOO, JMetalCSharp |

### Reactive Programming
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **System.Reactive** | `6.0.1` | DSOpt, Radical, StepperAux |
| **System.Reactive.Core** | `6.0.1` | DSOpt, Radical, StepperAux |
| **System.Reactive.Interfaces** | `6.0.1` | DSOpt, Radical, StepperAux |
| **System.Reactive.Linq** | `6.0.1` | DSOpt, Radical, StepperAux |
| **System.Reactive.PlatformServices** | `6.0.1` | DSOpt, Radical, StepperAux |
| **System.Reactive.Windows.Threading** | `6.0.1` | DSOpt, Radical, StepperAux |

### User Interface Libraries
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **LiveCharts** | `0.9.7` | DSOpt, Radical, StepperAux |
| **LiveCharts.Wpf** | `0.9.7` | DSOpt, Radical, StepperAux |
| **MaterialDesignThemes** | `2.4.0.1044` | DSOpt, Radical, StepperAux |
| **MaterialDesignColors** | `1.1.3` | DSOpt, Radical, StepperAux |
| **InteractiveDataDisplay.WPF** | `1.0.0` | DSOpt, Radical, StepperAux |
| **Eto.Forms** | `2.9.0` | Radical, StepperAux |
| **Eto.Platform.Wpf** | `2.9.0` | Radical, StepperAux |
| **Extended.Wpf.Toolkit** | `3.6.0` | Radical, StepperAux |

### Mapping & Visualization
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **Microsoft.Maps.MapControl.WPF** | `1.0.0.3` | DSOpt, Radical, StepperAux |

### Windows API Integration
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **Microsoft-WindowsAPICodePack-Core** | `1.1.4` | Radical, StepperAux |
| **Microsoft-WindowsAPICodePack-Shell** | `1.1.4` | Radical, StepperAux |

### System Utilities
| Package | Version | Used In Projects |
|---------|---------|------------------|
| **System.Runtime.CompilerServices.Unsafe** | `4.5.3` / `6.1.0` | DSOpt, Radical, StepperAux |
| **System.Threading.Tasks.Extensions** | `4.5.4` / `4.6.0` | DSOpt, Radical, StepperAux |
| **System.ComponentModel.Annotations** | `5.0.0` | Radical, StepperAux |
| **Microsoft.CSharp** | `4.7.0` | StructureEngineCS_StormCloud |
| **System.Core** | `3.5.21022.801` | StructureEngineCS_StormCloud |

---

## Project Dependencies

### Common Dependency Pattern
Most Grasshopper components follow this pattern:
```
Project
├── DSECommon (internal shared library)
├── RhinoCommon (8.0.23304.9001)
└── Grasshopper (8.0.23304.9001)
```

### Optimization Projects Dependencies
(DSOpt, Radical, MOO) have additional dependencies:
```
Optimization Project
├── Base Dependencies (DSECommon, RhinoCommon, Grasshopper)
├── JMetalCSharp (internal optimization library)
├── MathNet.Numerics (mathematical computations)
├── NLoptNet (optimization algorithms)
├── UI Libraries (LiveCharts, MaterialDesign, etc.)
└── Reactive Extensions (System.Reactive.*)
```

---

## Build Configurations

### Supported Configurations
- **Debug** / **Release**
- **Debug32** / **Debug64** (architecture-specific variants)
- **Platform**: AnyCPU (majority of projects)

### Build Outputs
- **Grasshopper Components**: Build to `.gha` files
- **Libraries**: Standard `.dll` assemblies
- **Post-build Actions**: Automatic copying to `Output/` directories

### Development Environment
- **Visual Studio**: 2017 or later
- **Rhino**: Version 8 compatibility
- **Grasshopper**: Version 8 compatibility
- **Windows**: Required for Windows API components

---

## Solution Structure

```
gh-design-space-exploration/
├── AllProjects.sln
├── Directory.Build.props
├── DSE Build 1.5.zip
├── FixProject.vsix
├── _TestFiles/
├── Capture/
├── Cluster/
├── Contort/
├── DesignLogger/
├── Diversity/
├── Documentation/
├── DSECommon/
├── DSOpt/
├── Effects/
├── Gradient_MOO/
├── JMetalCSharp.V05/
├── MOO/
├── netDxf/
├── Output/
├── packages/
├── Radical/
├── Reader/
├── Sampler/
├── Sift/
├── Stepper/
├── StepperAux/
├── StormCloud/
├── StormCloudAnalysis/
├── StructureEngineCS/
├── StructureEngineCS_StormCloud/
├── structurefit-master/
├── temp/
├── Tilde/
└── Writer/
```

---

## Getting Started

### Prerequisites
1. **Visual Studio 2017+** with C# support
2. **Rhino 3D 8.0+** installed
3. **Grasshopper** (included with Rhino)
4. **.NET Framework 4.8** Developer Pack

### Build Instructions
1. Clone the repository
2. Open `AllProjects.sln` in Visual Studio
3. Restore NuGet packages
4. Build solution (all projects should build successfully)
5. Built `.gha` files will be copied to `Output/` directory

### Installation
Copy the built `.gha` files from the `Output/` directory to your Grasshopper Components folder.

---