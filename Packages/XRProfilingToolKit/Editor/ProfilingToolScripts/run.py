#################################################################################################################
## Copyright (c) 2024 PICO Developer
## SPDX-License-Identifier: MIT
## Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and#or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
## The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
## THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
################################################################################################################
# Author: Xutong Zhou (xutong.zhou@bytedance.com)

import argparse
import datetime
import json
import os
import subprocess
import time
from enum import Enum
import re
import shutil

class Platform(Enum):
    Unknown = 0
    Pico = 1
    Quest = 2
# global variables
app_id = "com.bytedance.PICODeveloperTech.XRProfilingToolkit"
app_main_activity = f"{app_id}/com.unity3d.player.UnityPlayerActivity"
session_id = ""
session_dir = ""
screencap_dir = ""
pxr_screencap_dir = ""

scene_name = "UnKnown"
feature_status = "None"
screen_cap_cnt = 0

screen_cap_w = 1920
screen_cap_h = 1920

hFov = 0.0
vFov = 0.0

pMetrics = None
pXRProfilingToolkitLog = None

pGprobe = None

gprobe_realtime_mode = True
gprobe_dir = ""

current_platform = Platform.Unknown

parser = argparse.ArgumentParser(description="Script to run the XR profilingtoolkit project on XR HMD")

gpu_profiler = ""

fDeviceSpec = None

json_persistent_path = f"/storage/emulated/0/Android/data/{app_id}/files/CommandQueue.json"
#functions
def check_devices():
    out = subprocess.check_output(['adb', 'devices'], text = True).splitlines()
    devices = []
    for line in out[1:]:
        if not line.strip():
            continue
        if 'offline' in line:
            continue
        
        return True
    return False

def check_platform():
    out = subprocess.check_output(["adb", "shell", "getprop", "ro.product.system.brand"], text = True)
    if ('oculus' in out):
        return Platform.Quest
    elif ("Pico" in out):
        return Platform.Pico
    else:
        return Platform.Unknown
    
def create_dir():
    global pxr_screencap_dir, session_dir, screencap_dir, gprobe_dir, gprobe_realtime_mode
    
    if args.outputPath:
        session_dir = os.path.join(args.outputPath, session_id)
    else:
        session_dir = os.path.join(os.getcwd(), session_id)
    os.makedirs(session_dir)

    scripts_dir = os.path.dirname(os.path.abspath(__file__))
    source_file_path = os.path.join(scripts_dir, "pxr_metrics.schema")
    destination_file_path = os.path.join(session_dir, "pxr_metrics.schema")
    shutil.copy(source_file_path, destination_file_path)
    source_file_path = os.path.join(scripts_dir, "ovr_metrics.schema")
    destination_file_path = os.path.join(session_dir, "ovr_metrics.schema")
    shutil.copy(source_file_path, destination_file_path)
    source_file_path = os.path.join(scripts_dir, "pil_output.schema")
    destination_file_path = os.path.join(session_dir, "pil_output.schema")
    shutil.copy(source_file_path, destination_file_path)
    source_file_path = os.path.join(scripts_dir, "report_template")
    destination_file_path = os.path.join(session_dir, "report_template")
    shutil.copytree(source_file_path, destination_file_path, dirs_exist_ok=True)

    screencap_dir = os.path.join(session_dir, "screencap")
    os.makedirs(screencap_dir)

    if current_platform is Platform.Pico:
        pxr_screencap_parent_dir = "/sdcard/Pictures/Screenshots"
        pxr_screencap_dir = f"/sdcard/Pictures/Screenshots/{session_id}"

        try:
            output = subprocess.check_output(["adb", "shell", "ls", pxr_screencap_parent_dir], text = True)
        except subprocess.CalledProcessError as exec:
            # create the screenshots directory if not exists. The directory will be created at first screen capture
            subprocess.call(["adb", "shell", "mkdir", pxr_screencap_parent_dir])
        subprocess.call(["adb","shell", "mkdir", pxr_screencap_dir])
    else:
        pxr_screencap_dir = f"/sdcard/Pictures/{session_id}"
        subprocess.call(["adb","shell", "mkdir", pxr_screencap_dir])
    
    # create subdir for gprobe rendering stage and drawcall capture
    if not gprobe_realtime_mode:
        gprobe_dir = os.path.join(session_dir, "gpu_profiler")
        os.makedirs(gprobe_dir)

def start_profilingtoolkit():
    global pMetrics, pGprobe, gprobe_realtime_mode
    pMetrics = start_pxr_metrics()
    lock_cpu_level()
    lock_gpu_level()
    if gprobe_realtime_mode:
        # TODO(xutong): make sampling frequency configurable
        pGprobe = start_gprobe_realtime(1)

def stop_profilingtoolkit():
    pXRProfilingToolkitLog.terminate()
    time.sleep(2)
    pull_screencaps()
    if pGprobe is not None:
        pGprobe.terminate()
    pMetrics.terminate()

# pico screencap service require root access
def cap_screen(context):
    cur_time = subprocess.check_output(["adb", "shell", "date", "+%Y%m%d%H%M%S%N"], text=True).strip()
    screen_cap_path = f"{pxr_screencap_dir}/{scene_name}_{context}_{feature_status}_{cur_time}.png"
    if False:
        subprocess.call(["adb", "shell", "am", "startservice", "-p", "com.bytedance.pico.screencapture", "-a", "pvr.intent.action.SCREEN_SHOT", "--es", "from", "test", "--es", "file_path", screen_cap_path, "--eia", "resolution", f"{screen_cap_w},{screen_cap_h}"])
    else:
        subprocess.call(["adb", "shell", "screencap", "-p", screen_cap_path])

# pico screencap service require root access
def start_screen_record():
    if (current_platform is Platform.Pico):
        pxr_time = subprocess.check_output(["adb", "shell", "date", "+%Y%m%d%H%M%S%N"], text=True).strip()
        screen_rc_path = f"{pxr_screencap_dir}/{scene_name}_{pxr_time}.mp4"
        subprocess.call(['adb', 'shell', 'am', 'startservice', '-p', 'com.bytedance.pico.screencapture', '-a', 'pvr.intent.action.SCREEN_RECORD', '--es', 'pvr.intent.action.SCREEN_RECORD', 'COMMAND_START', '--es', 'from', 'xxx', '--es', 'file_path', screen_rc_path])
    else:
        print(f"Screen capture not supported on {current_platform}")
    
# pico screencap service require root access
def stop_screen_record():
    if (current_platform is Platform.Pico):
        subprocess.call(['adb', 'shell', 'am', 'startservice', '-p', 'com.bytedance.pico.screencapture', '-a', 'pvr.intent.action.SCREEN_RECORD', '--es', 'pvr.intent.action.SCREEN_RECORD', 'COMMAND_RECORD_STOP', '--es', 'from', 'xxx'])
    else:
        print(f"Screen capture not supported on {current_platform}")

def pull_screencaps():
    print(f"adb pull {pxr_screencap_dir} {screencap_dir}")
    subprocess.check_output(["adb", "pull", pxr_screencap_dir, screencap_dir])
    # clean up the screencap dir on device
    subprocess.call(["adb", "shell", "rm", "-r", pxr_screencap_dir])

def update_feature_status(line):
    global feature_status
    feature_status = ""
    features = line.split(",")
    for f in features:
        if "True" in f:
            feature_status += "1"
        elif "False" in f:
            feature_status += "0"
    
def update_scene_name(line):
    global scene_name
    scene_name = line.split(":")[-1].strip()
    scene_name.replace("", "_")

def process_line(line):
    global hFov, vFov
    if "CaptureScreen" in line:
        cap_screen(line.split(':')[-1].strip())
    if "XRProfilingToolkit starting" in line:
        start_profilingtoolkit()
    if "XRProfilingToolkit finished" in line or "XRProfilingToolkit paused" in line:
        stop_profilingtoolkit()
    if "StartScreenRecord" in line:
        start_screen_record()
    if "EndScreenRecord" in line:
        stop_screen_record()
    if "Feature status" in line:
        update_feature_status(line)
    if "Scene loaded" in line:
        update_scene_name(line)
    if "CaptureDrawCall" in line:
        gprobe_drawcall(line.split(':')[-1].strip())
    if "CaptureRenderingStage" in line:
        gprobe_stage(line.split(':')[-1].strip())
    if "$DeviceSpec" in line:
        deviceSpecStr = line.split('$DeviceSpec')[-1].strip()
        # write device spec to file
        fDeviceSpec.write(deviceSpecStr + '\n')
        # record device fov to calculate PPD later
        if "FOV" in deviceSpecStr:
            fovs = re.findall(r"[-+]?(?:\d*\.*\d+)", deviceSpecStr)
            hFov = float(fovs[0]) + float(fovs[1])
            vFov = float(fovs[2]) + float(fovs[3])

def start_pxr_metrics():
    global current_platform
    fMetrics = open(os.path.join(session_dir, "ovr_metrics.log" if current_platform is Platform.Quest else "pxr_metrics.log"), "w")
    metric_command = "VrApi" if (current_platform == Platform.Quest) else "PxrMetric"
    p = subprocess.Popen(["adb", "logcat", f"{metric_command}:V", "*:S"], stdout=fMetrics, universal_newlines=True, text = True)
    return p

def gprobe_stage(context):
    global gprobe_dir, gprobe_realtime_mode
    if gprobe_realtime_mode:
        print("GPU Profiler in realtime mode, unable to capture rendering stage!")
        return
    
    pxr_time = subprocess.check_output(["adb", "shell", "date", "+%Y%m%d%H%M%S%N"], text=True).strip()
    outputPath = f"{gprobe_dir}/stage_{scene_name}_{context}_{feature_status}_{pxr_time}.log"
    fDrawcall = open(outputPath, "w")
    subprocess.call(["adb", "shell", gpu_profiler, "-t"], stdout=fDrawcall, universal_newlines=True, text = True)

def gprobe_drawcall(context):
    global gprobe_dir, gprobe_realtime_mode
    if gprobe_realtime_mode:
        print("GPU Profiler in realtime mode, unable to capture drawcall!")
        return
    
    pxr_time = subprocess.check_output(["adb", "shell", "date", "+%Y%m%d%H%M%S%N"], text=True).strip()
    outputPath = f"{gprobe_dir}/drawcall_{scene_name}_{context}_{feature_status}_{pxr_time}.log"
    fStage = open(outputPath, "w")
    subprocess.call(["adb", "shell", gpu_profiler, "-x", "--time", "0.2"], stdout=fStage, universal_newlines=True, text = True)
    
def start_gprobe_realtime(freq):
    adb_time = subprocess.check_output(["adb", "shell", "date", "+%Y%m%d%H%M%S%N"], text=True).strip()
    fGpuInfo = open(os.path.join(session_dir, "pil_output.log"), "w")
    fGpuInfo.write(f"{adb_time}\n")
    command = ["adb", "shell", gpu_profiler, "-r"]
    if current_platform is Platform.Pico:
        command.append("--time")
        command.append(str(freq))
    p = subprocess.Popen(command, stdout=fGpuInfo, universal_newlines=True, text = True)
    return p

def log_screen_spec():
    fDeviceSpec.write("Screen resolution(both eyes total) ")
    screen_panel_res = subprocess.check_output(["adb", "shell", "wm", "size"], text=True)
    fDeviceSpec.write(screen_panel_res)
    # calculate pixel density (PPD), pixel distribution is even across view angle
    if hFov != 0.0 and vFov != 0.0:
        res = re.findall(r"[-+]?(?:\d*\.*\d+)", screen_panel_res)
        screenW = float(res[0])
        screenH = float(res[1])
        # physical screen width should always be approximately 2x the height as it combines width of both eyes
        # Pico Neo 3 swaps screen width and height, need to swap it back here
        if (screenW / screenH < 1.0):         
            screenW, screenH = screenH, screenW
        screenW /= 2
        fDeviceSpec.write(f"Pixel per degree: WxH: {screenW / hFov :.2f}x{screenH / vFov :.2f} \n")

def log_os_version():
    fDeviceSpec.write("OS version: ")
    if current_platform == Platform.Pico:
        os_version = subprocess.check_output(["adb", "shell", "getprop", "ro.pvr.internal.version"], text=True)
        fDeviceSpec.write(os_version)
    elif current_platform == Platform.Quest:
        os_version = subprocess.check_output(["adb", "shell", "getprop", "ro.system.build.fingerprint"], text=True).rstrip("\n")
        runtime_version = subprocess.check_output(["adb", "shell", "getprop", "ro.vros.build.version"], text=True)
        fDeviceSpec.write(f"{os_version} Runtime version: {runtime_version}")
    else:
        fDeviceSpec.write("\n")

def log_serial_number():
    fDeviceSpec.write("Device serial number: ")
    serial_num = subprocess.check_output(["adb", "shell", "getprop", "ro.serialno"], text=True)
    fDeviceSpec.write(serial_num)

# get detailed vulkan support info, currently not used
def get_gpu_driver_info():
    vulkanInfoJson = subprocess.check_output(["adb", "shell", "cmd", "gpu", "vkjson"], text=True)
    vkData = json.loads(vulkanInfoJson)
    print(vkData['devices'][0]['VK_KHR_driver_properties'])
    
def lock_gpu_level():
    # setting performance mode through Quest sdk doesn't lock to a specific GPU level, force the level here
    if current_platform == Platform.Quest:
        subprocess.call(['adb', 'shell', 'setprop', 'debug.oculus.gpuLevel' '3'])

def lock_cpu_level():
    # setting performance mode through Quest sdk doesn't lock to a specific CPU level, force the level here
    if current_platform == Platform.Quest:
        subprocess.call(['adb', 'shell', 'setprop', 'debug.oculus.cpuLevel' '3'])

# main
parser.add_argument('--file', type=str, help="Automation commands file for profilingtoolkiting")
parser.add_argument('--outputPath', type=str, help="Analysis session output path")
parser.add_argument('--appid', type=str, help="app_id")
args = parser.parse_args()
if args.appid:
    app_id = args.appid
    json_persistent_path = f"/storage/emulated/0/Android/data/{app_id}/files/CommandQueue.json"
    app_main_activity = f"{app_id}/com.unity3d.player.UnityPlayerActivity"

if not check_devices():
    print("Device not connected...")
    exit()

current_platform = check_platform()
if (current_platform == Platform.Unknown):
    print("Unsupported platform, exiting!")
    exit()
 
if (current_platform == Platform.Quest):
    gpu_profiler = "ovrgpuprofiler"
elif (current_platform == Platform.Pico):
    gpu_profiler = "gprobe"
    
subprocess.call(["adb","shell", "am", "force-stop", app_id])
subprocess.call(["adb", "shell", gpu_profiler, "-d"])

if args.file:
    subprocess.call(["adb", "push", args.file, json_persistent_path])

# TODO(xutong): make this configurable
gprobe_realtime_mode = True

# if (not gprobe_realtime_mode):
subprocess.call(["adb", "shell", gpu_profiler, "-e"])

subprocess.call(["adb", "logcat", "-c"])
time.sleep(1)
print("Starting xrprofiling app: " + app_main_activity)
p = subprocess.Popen(["adb", "shell", "am", "start", "-n", app_main_activity], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
p.wait()

if (current_platform == Platform.Pico):
    # Pico only, oculus don't need to select process
    subprocess.check_output(["adb", "shell", "gprobe", "-s", app_id], text = True)

(stdout, stderr) = p.communicate()
if stderr:
    print("Failed to start profilingtoolkit app...")
    print(stderr)
    exit()

session_id = "xr_profiling_session_" + datetime.datetime.now().strftime('%Y%m%d%H%M%S')
create_dir()

pXRProfilingToolkitLog = subprocess.Popen(["adb", "logcat", "Unity:V", "*:S"], stdout=subprocess.PIPE)
fDeviceSpec = open(os.path.join(session_dir, "device_spec.log"), "w")
log_os_version()

fXRProfilingToolkitLog = open(os.path.join(session_dir, "xr_profilingtoolkit.log"), "w")
while True:
    if pXRProfilingToolkitLog.poll() is None:
        output = pXRProfilingToolkitLog.stdout.readline().decode('utf-8').strip()
        if "XR_ProfilingToolkit" in output:
            if output:
                fXRProfilingToolkitLog.write(output + "\n")
                print(output)
                process_line(output)
    else:
        print("break")
        break

if args.file:
    # remove the automation file pushed to the device
    subprocess.call(["adb", "shell", "rm", json_persistent_path])
if (current_platform == Platform.Pico):
    log_screen_spec()
print(f"Session saved to {session_dir}")