# XR Profiling Toolkit

The XR Profiling Toolkit Unity package is an automated and customizable graphics profiling tool for evaluating the performance of XR applications running on headset devices. The core framework of the toolkit involves automated test scripting, graphics feature toggling, profiling data export, and report generation for in-depth performance analysis and comparison. You can easily integrate this tool into your existing XR projects via the Unity Package Manager. In addition, we provide a project with a high-quality sample scene to demonstrate the toolkit's usages and capabilities.

The video clips below showcase the Cyber Alley test scene and the graphics settings in the provided open-source project.

## The Cyber Alley Sample Test Scene
https://github.com/user-attachments/assets/25ac988b-add7-4832-b43c-1cac2542c248

## Particle Settings
https://github.com/user-attachments/assets/423b15f9-36a6-453e-9375-45da6cb97ebc

## XR Graphics Settings
https://github.com/user-attachments/assets/eeebc885-5dc0-44f5-9ac3-2e6100760c74


## Use cases
- **Cross-Vendor Evaluation**: The XR Profiling Toolkit enables developers to assess the hardware capabilities of various XR headsets and easily draw comparisons between two testing results, providing a clear understanding of the performance budget across different hardware platforms.
- **Content Optimization**: The XR Profiling Toolkit helps developers make informed decisions about the art content, rendering features, and visual quality achievable on different headsets, assisting content creation optimization and resource planning for XR devices.
- **Art Scene Sample and Testing**: The XR Profiling Toolkit provides a high-quality art scene as a reference for developers. This sample also serves as a testbed to help reproduce performance or functional issues, making it easy to communicate with headset vendors when troubleshooting.


## 1. Setup
### 1.1 Supported Devices
- PICO 4 series, with PICO 4 Ultra support coming in the first half of 2025
- PICO Neo 3 series
- Meta Quest 3
- Meta Quest 2

### 1.2 Development Environment
- PICO SDK version: 3.0.4 
  - included in the project and imported from the downloaded file
  - Note that there may be conflicting guids with other providers when you import the PICO SDK from the git url. This is a known issue and will be fixed in a future update.
- Meta XR All-in-One SDK version: 69.0.1
  - Included in the project and installed via the Unity Package Manager
- Unity version: 2022.3.23f1 LTS
- Graphics API: Vulkan
- Target architecture: ARM64
- Windows or Mac host
  - Android Debug Bridge installed
  - Python version: 3.7.17 (pip version: 23.0.1)

### 1.3 Cross-Vendor Build
If you want to build the Unity project yourself, you will need to set the build target to Android and configure the XR Plug-in provider.
The project is developed to support cross-vendor builds and testing. When targeting a specific device such as PICO and Quest, select the corresponding Plug-in Providers in **Project Settings -> XR Plug-in Management**. We recommend using the settings provided in the project for optimized performance and functionalities.
Notice that you will need to uncheck the previous platform when switching and deploying to other platforms.
If you are interested in cross-vendor deployment in your own project and extending the platforms currently provided in the sample project, check out the PlatformSwitcher.cs script and the XR Origin setup in the scene.

### 1.4 XR Feature Settings
The XR Feature Settings object can be found in **Packages/XR Profiling Toolkit/Settings/XRFeatureSettings.** Once set, the settings will be applied to all scenes.

![image](https://github.com/user-attachments/assets/fb007693-037f-4ec2-ab69-ee156a826349)

- Incompatible Features: features that are not compatible with each other. 
  - Incompatible features can't be enabled simultaneously, such as Foveated Fixed Rendering (FFR) and Adaptive Resolution. By default, none are in the list.
- Foveation Level: level of foveated rendering.
  - Recommend medium level for a balance of quality and performance
- MSAA Samples: number of samples for MSAA.
  - 4x is always recommended.
- Feature Status In Manual Mode: Sets which features to turn on when in Manual Mode. In Manual Mode, users can explore the scene and interact with objects.
  - It is recommended that both MSAA and FFR should be turned on.
- Target Resolution: the eye buffer resolution at which the scene will be rendered.
  - On different platforms, the width or the height of the eye buffer may not match exactly, but the total number of pixels should be close. 
  - The given value is for mainstream devices. Value can be set higher for advanced devices.

## 2. Sample Scene
You can import the sample scene from **Windows->Package Manager->XR Profiling Toolkit->Samples->Cyber Alley VR Sample Scene**. In this VR sample scene, users can interact with interactable objects, teleport to viewpoints, and explore various rendering optimization features.

### 2.1. Main Menu
Use the controller to select a sample scene in the main menu interface. Only the first test scene is available now. We plan to release other samples to cover broader application scenarios.

![image (1)](https://github.com/user-attachments/assets/a012fcf9-6c73-47b0-8ee4-73a324549f68)

### 2.2. Cyber Alley VR Sample Scene
The Cyber Alley scene is a showcase scene that evaluates the performance of a well-made VR scene. It features various graphics enhancement features, a dynamic particle system, and performance evaluation and debugging tools. On PICO 4, it runs consistently at 72 frames per second (FPS), which is the default maximum FPS. In the project settings, we lock it to be 72 FPS for performance reasons, such as protecting overburn, and recommend doing so. If you don't want to lock the FPS, you can change the setting in **Assets -> XR -> Settings -> PXR_Settings.**

### Controls

| Controller  | Actions |
| ------------- | ------------- |
| ![Controls](https://github.com/user-attachments/assets/bdf7d2c9-656c-4232-b2b6-731547a5ffd5)  | ![Control Scheme](https://github.com/user-attachments/assets/a83c8f8a-156f-4dfd-a9b9-2aefdef9a95d) |
| PICO 4 controller input mapping. Other controllers share similar controls. Control tooltips are also displayed on the controller model when running the scene. | <ul><li>Toggle UI Menu: A/X or Menu Button 1</li><li>Interact with Object or UI: Trigger</li> <li>Teleport: Thumbstick forward to activate, release to select target and teleport</li> <li>Snap Turn: Thumbstick left or right</li> </ul> |


### Teleportation
There are 5 teleportation points in the scene, and they will be visible when teleportation is activated by the user. When a teleportation point is selected by hovering, its visual effect will change. Teleporation will take place when releasing the thumbstick with a target selected.
Note that the player may not keep their original orientation after teleportation. This is designed to orient users to look at specific areas in the scene.

### UI Menu
There are two tabs in the UI menu: Settings and User Guide.

#### Graphics
It is expected to see noticeable visual changes when each of the settings is toggled.

**Debug View:**
The Tile Visualizer is used to visualize the Fragment Density Map (FDM) pattern when FFR is turned on. The green area indicates the full resolution. When FFR is turned on, there should be some yellow (1/2 resolution) and orange (1/4 resolution) tiles in the peripheral region of the view.
Tile patterns vary across platforms and also change when MSAA is turned on and off.

**Misc:**
Mute - mute scene audio. Useful to mute the sound when testing to not get disturbed.

#### User Guide
Instructions on how to interact with the scene. 

### Dynamic Particles
The particle emitter is highlighted in the scene with an outlining effect.
When hovering, the highlighting effect will be off, and a pop-up will prompt users to change particle mode by interacting with the emitter.
Modes
- Off: no particle
- Particle system: orange smoke
- VFX graph: purple smoke
  - Notice that the default VFX graph renderer block is not compatible with Multiview. We overcame this by using a custom shader graph for particle rendering. To adjust the effect yourself, you will need to enable shader graph for VFX graph.
GPU load will change when switching between modes, and it will likely cause some frame drops. It should be running at full frame rate once stabilized.

## 3. Automated Test
The scene can be run automatically with a pre-configured command sequence (see Command Queue below), and a performance report showing the test results will be generated. 
This report is designed to help developers evaluate scene complexities, identify performance hotspots, and verify performance optimization and regression.

### 3.1 Command Queue
Command Queue is used to store scene automation sequences. Whenever running a profiling session, a serialized Command Queue needs to be specified to define how the session should be run. For the convenience of saving and debugging by yourself, we recommend clicking **XR Profiling Toolkit -> Shortcut -> Copy Scriptfolder To Assets**, which moves the demo script from the Package folder to the asset folder, then you can find the demo script in **Assets\XRProfilingToolkit\Editor\ProfilingToolScripts.**

### 3.1.1  Create and Edit a Command Queue
To create a Command Queue in Unity Editor, right-click on the Project window and select **Create-> XRProfilingToolkit -> Command Queue.**
The following section contains instructions for command queue configurations. 
Once configured, click the Save to File button to save the current Command Queue to a JSON file on disk for later use.

![image](https://github.com/user-attachments/assets/9cb0d394-0b10-4f20-b887-890427ebcde4)
Command Queue Example

You can find three sample Command Queue assets by default in **Package\XR Profiling Toolkit\ Editor\ProfilingToolScripts\SampleCommandQueues**
If you click the shortcut menu **XR Profiling Toolkit -> Shortcut -> Copy ProfilingToolScripts** folder To Assets these sample Command Queue assets will be copied to the Assets folder in **Assets\XRProfilingToolkit\Editor\ ProfilingToolScripts\SampleCommandQueues** for ease of use.

![image](https://github.com/user-attachments/assets/957e68e1-18f2-40d8-8e94-c120982de8cc)
Sample Command Queues

Likewise, you can find three Command Queue samples JSON files saved in **Package\XR Profiling Toolkit\Editor\ProfilingToolScripts\AutomationScripts** or **Assets\XRProfilingToolkit\Editor\ ProfilingToolScripts\AutomationScripts.**

![image](https://github.com/user-attachments/assets/3b83e4f5-f9a8-48b4-a757-5af256756d68)
Profiling Tool Scripts. We recommend copy the files from the Packages folder to the Assets folder for ease of use.


### 3.1.2 Configurations
1. Id: used to identify the command queue. Only profiling sessions running with the same command queue should be compared to ensure the scene has been run in the same way.
  1. Version: id should end with semantic version X.Y.Z. Whenever the command queue is saved, patch version (Z) will be bumped automatically. Major (X) and minor (Y) versions will need to be bumped manually.
2. Stop Time: after the specified number of seconds, the whole command queue will be stopped, and any pending commands will not run.
3. Commands: list of scene automation commands. Scene Automation Command can be added to the list and command type can be specified with the dropdown list.

The following basic commands are supported by default for any test scene:
  1. CommandMove: move to the target position at the specified speed.
  2. CommandPause: pause the whole command queue when this command is reached. Any subsequent commands will not run.
  3. CommandWait: wait for a certain period before the next command.
    1. DurationInSec (float): seconds to wait
  4. CommandScreenCapture: capture screenshot, screen record, rendering stage, or draw call.
This works with the Python script. The command prints out an adb log with the Python script monitoring logcat. When the screen capture log is printed, the Python script calls the corresponding service to capture it.
    1. Type (enum): type of capture
      1. Including CaptureScreen, StartScreenRecord, EndScreenRecord, CaptureRenderingStage, CaptureDrawCall. The last two options capture detailed GPU metrics and are only recommended if you know how to read them.
    2. Context (string): context of the capture
      1. This can be any metadata associated with the capture. Captures with the same context will be grouped together in the report.
  The following basic commands are supported in conjunction with components in any scene:
  1. CommandLoadLevel: load a specific level with the SceneLoader.prefab, you can find it at **XR Profiling ToolKit->Shortcuts**
    1. Level Index (int): which level to load, starting from 0. 0 indicates the 1st profiling scene.
  2. CommandToggleFeature: toggle the scene features with the FeatureManager.prefab, you can find it at **XR Profiling ToolKit->Shortcuts**
    1. Feature (enum):
      1. includes the following rendering features: FFR (fixed foveated rendering, Med Level), MSAA (4x), Dynamic Resolution (Meta) or Adaptive Resolution (PICO).
    2. Enabled (bool): turn on or off the feature
  The following examples of expandable commands are supported in the Cyber Alley scene according to their specific logic. You can try to expand your own commands on your own project:
  1. CommandSetDynamicMode: set the dynamic mode of the scene. Scene 1 has a dynamic particle system
    1. Mode (int): mode of dynamic system. 
      1. In Scene 1 (Cyber Alley), 0: no effect, 1: using particle system, 2 - using vfx graph
  2. CommandTeleport: teleport to a target teleportation anchor location
    1. TargetId (int): index of the teleportation anchor, ranging from 0 to 4 available targets in the scene. Check TeleportAnchors_Scene1 in CyberAlley for teleportation locations and their corresponding index.

## 3.2 Device Profiling Tool Window
![image](https://github.com/user-attachments/assets/243f9bdc-8f08-4540-87f6-098d830bc430)
The Device Profiling Tool Window for running tests

### 3.2.1 Profiling Sessions
#### 3.2.1.1 Requirements
- Host machine, Windows or Mac, with adb installed
- Headset connected to the host machine, listed in adb devices
- Python version: 3.7 or later
  
#### 3.2.1.2 Running a Session
1. Choose a CommandQueue JSON path by clicking Browse to select a JSON file we saved in 3.1.1, or copying the full path to the JSON object we saved in 3.1.1 and paste it into the command text.
2. Choose a Profiling Data Output Path by clicking Browse to select a directory to output file, or copying the directory path and paste it into the command text.
3. Click Run Automation button and then you will see logs printed out in the terminal. Once completed, you will find a new folder containing metrics and screenshots captured during the session at Output Path.

![image](https://github.com/user-attachments/assets/59bdc9f9-11f8-4bdd-9231-bcba8c0d325e)

4. If the Run Automation button doesn't work, you can also click the Copy Run Automation Command button and try it on a command line terminal.

## 3.3 Result Analysis
Once we have the session data, we can then generate reports to visualize the performance analysis of the profiling sessions.

### 3.3.1 Requirements
- Python version: 3.7 or later
- Python dependencies installed
  - Click the Set Up Analysis Tool Environment button and wait for completion.
  - If the Set Up Analysis Tool Environment button doesn't work, you can try to open a command line terminal, locate the Package's Profilting ToolScripts folder in Package\XR Profiling Toolkit\Editor\ProfilingToolScripts\, and run pip install -r requirements.txt command to manually run the command.

### 3.3.2 Metrics Configuration
There are three .schema files in JSON format under the ProfilingToolScripts folder defining which metrics should be displayed in the profiling report
- pxr_metrics.schema: basic performance metrics for PICO
- ovr_metrics.schema: counterpart of basic performance metrics on Quest
- pil_output.schema: advanced cross-platform GPU metrics
Each schema file contains a list of available metrics along with their descriptions (either a website link or in the file itself). Following is an example of a metric:
      {
        "enabled": 1, // whether the metric should be displayed in the report, 0: disabled, 1: enabled
        "name": "% Time Shading Fragments", // name of the metric shown in the report
        "description": "Percentage of time spent shading fragments. Formula: Time spent on shading fragments / Total time spent on shading.", // description of the metric shown in the report
        "template": "{time_shading_fragments_percentage.value:f}" // data template, do not modify!
      },

### 3.3.3 Performing Analysis
There are two ways to analyze the performance of profiling sessions

#### 3.3.3.1 Generate Analysis Report
 The result of a single session will be displayed on a local webpage.
1. Choose a Profiling Data Directory by clicking Browse to selecting a folder generated from running a session in 3.2.1.2, or copying the full path to the folder we saved in 3.2.1.2 and paste it into the command text.
2. Click Generate Analysis Report button and then there will be an analyze_report folder generated in the Profiling Data Directory.The local webpage will show out.You can also open it in the analyze_report folder by clicking index.html later.
3. If the Generate Analysis Report  button doesn't work, you can also click the Copy Generate Analysis Command button and try it on a command line terminal

#### 3.3.3.2 Generate Comparison Report
 The results of two sessions will be compared and displayed on the same local webpage.
1. Choose a Comparison Profiling Data Directory by clicking Browse to selecting a folder generated from running a session in 3.2.1.2, or copying the full path to the folder we saved in 3.2.1.2 and paste it into the command text.It will be compared with the Profiling Data Path we chose in 3.3.3.1 .
2. Click Generate Comparison Report button and then there will be a comparison_report_{benchmark_session_directory_1}_{benchmark_session_directory_2}  folder generated in the Profiling Data Directory.The local webpage will show out.You can also open it in the comparison_report_{benchmark_session_directory_1}_{benchmark_session_directory_2} folder by clicking index.html later.
3. If the Generate Comparison Report  button doesn't work, you can also click the Copy Generate Comparison Command button and try it on a command line terminal

## 3.4 Reading the Report
Header and Device Specification
Showing the session name, automation command queue id along with the hardware spec, rendering configurations of the device.

![image](https://github.com/user-attachments/assets/4960bde0-efed-455b-931a-45675c48e8bb)

Metrics
Displaying metric data plotted on graphs
Tabs on the left switch among available metrics configured in 3.3.2. Metrics name and session average are displayed on the tab.

![image](https://github.com/user-attachments/assets/98326eb3-aa6a-43c3-9da9-496a1a244f44)

Screen Captures
Displays captured screenshots. Captures with the same context will be grouped together. As shown below, two images are displayed side by side for comparison. The left one is the baseline, with the least number of rendering features turned on, while the right one with some additional features turned on.

![image](https://github.com/user-attachments/assets/96d292ed-d4bd-438e-a1df-5e1c01033404)

For the comparison report, the report differs in the following ways.
- Since two sessions may run on different devices, device specs of individual sessions are displayed side by side.
- If the same metric data is available from both sessions, they will be plotted on the same graph. The session average of both sessions will be displayed on the tab.
- Screen captures will be displayed side by side only if they share the same context and rendering feature status.
4. Porting XR Profiling Toolkit to Another Project
To port the XR Profiling Toolkit to other Unity projects, follow these minimal steps:
1. Import the XRProfilingToolKit package by Window->Package Manager-> Add package from disk.
2. Go to the Unity editor menu, click and run XR Profiling ToolKit->Shortcuts->Validate Provider Plugin (Meta\PICO):
  - Note that running this menu command will automatically check other XR SDKs, specifically, from Meta and PICO, in the existing project, and feature-flag the code needed to support respective platforms.
3. Add the CommandRunner.prefab to the Scene by clicking Add CommandRunner To Scene button:
  - In the scene where you want to run the profiling, create an empty GameObject and attach the CommandRunner.cs script.
4. Set up Command Queue:
  - Follow the Automated Test section to create a command queue in your Unity project. Note that only a limited set of commands is supported, as some commands have dependencies on other systems. However, you can create new command types that suit your project’s needs by using the existing commands as a reference. See more details in 3.1.2.
5. Build and deploy the project:
  - Build and deploy your Unity project to install the apk file on your device. Then, you can follow the Automated Test section to run a profiling session and generate performance reports.

## 5. Editor Menu Description
Below is a table detailing the Unity Editor Menu of the toolkit and the description for reference.

| Editor Menu  | Description |
| ------------- | ------------- |
| Device Profiling Tool Window | The main Unity Editor Window interface for device profiling |
| ShortCuts->Add CommandRunner To Scene | The core prefab for running automatically with a pre-configured command sequence
| ShortCuts->Validate Provider Plugin (Meta\PICO) | Switching and deploying to other platforms |
| ShortCuts->Add SceneLoader To Scene | Add the core prefab for using the CommandLoadLevel command |
| ShortCuts->Add FeatureManager To Scene | Add the core prefab for using the CommandToggleFeature command
| ShortCuts->Copy ProfilingToolScripts folder To Assets | Useful shortcut to copy the profiling tool scripts and files from the Package folder to the Assets folder for ease of use

## 6. Credits

We would like to thank the following members and their contributions.

Product Design and Management: Ke Jing, Jane Tian
Graphics and Software Engineering: Xutong Zhou
Unity Engineering: Xiangrui Meng
Data Visualization: Fukang Hong
QA and Testing: Lang Li
Art: PICO Design Center
