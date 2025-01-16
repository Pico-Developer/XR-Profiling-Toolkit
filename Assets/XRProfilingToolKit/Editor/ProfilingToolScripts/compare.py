#################################################################################################################
## Copyright (c) 2024 PICO Developer
## SPDX-License-Identifier: MIT
## Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and#or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
## The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
## THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
################################################################################################################
# Author: Xutong Zhou (xutong.zhou@bytedance.com)

import argparse
import os
import webbrowser
import json
import shutil

from datetime import datetime
from parseutil import *


features = []

class ScreenCaptureGroup:
    def __init__(self):
        # base screen capture path used to compare with the other screen captures, usually this one has all rendering feature off
        self.basePath = ""
        self.baseFeatueFlag = 1024
        self.otherPaths = []
    
    def addEntry(self, path):
        name = os.path.basename(path)
        if name.split('_')[2] != "None":
            # reverse the feature flag string. For base Path, we want to advanced features like adaptive resolution and app SW to be off
            feature_flag_str = name.split('_')[-2][::-1]
            # feature flag string is binary
            feature_flag_number = int(feature_flag_str, 2)

            if (feature_flag_number < self.baseFeatueFlag ):
                self.baseFeatueFlag = feature_flag_number
                if self.basePath:
                    self.otherPaths.append(self.basePath)
                self.basePath = path
            else:
                self.otherPaths.append(path)
        else:
            self.basePath = path
    
    def getBasePath(self):
        return self.basePath

    def getOtherPaths(self):
        return self.otherPaths

class ScreenCaptureCollection:
    def __init__(self, screenCapturePaths):
        self.groups = {}
        for path in screenCapturePaths:
            name = os.path.basename(path)
            groupId = '_'.join(name.split('_', 2)[:2])
            if groupId not in self.groups:
                self.groups[groupId] = ScreenCaptureGroup()
            self.groups[groupId].addEntry(path)
    
    def getGroups(self):
        return [self.groups[groupId] for groupId in self.groups]

# global variables
samplefeatures = ["Foveation(High)", "MSAA(4x)", "Adaptive Resolution"]
parser = argparse.ArgumentParser(description="Script to compare two XRProfilingToolkit sessions")


# main
parser.add_argument('-s', '--session', type=str, help="XRProfilingToolkit session result directories", required=True, nargs='+')
parser.add_argument('-f','--features', type=str, help="XRProfilingToolkit session features")
args = parser.parse_args()
    
# Check if exactly two directories are provided
if len(args.session) != 2 or not all(os.path.isdir(directory) for directory in args.session):
    print('Please pass in two valid session directories')
    exit()
   
if args.features:
    input_features = args.features.split('|')
    samplefeatures = [feature for feature in input_features if feature in samplefeatures]

session1_dir = args.session[0]
session2_dir = args.session[1]

automationId1, metricsData1,start_time1,finish_time1 = get_metrics_data(session1_dir)
automationId2, metricsData2,start_time2,finish_time2 = get_metrics_data(session2_dir)

##### genertate report #####
reportpathname = "comparison_report"+f"_{os.path.basename(session1_dir)}_{os.path.basename(session2_dir)}"
source_file_path = os.path.join(session1_dir, "report_template")
destination_file_path = os.path.join(session1_dir, reportpathname)
shutil.copytree(source_file_path, destination_file_path, dirs_exist_ok=True)
source_file_path = os.path.join(session1_dir+"/screencap", os.path.basename(session1_dir))
destination_file_path = os.path.join(session1_dir+"/"+reportpathname+"/resource/captures", os.path.basename(session1_dir))
shutil.copytree(source_file_path, destination_file_path, dirs_exist_ok=True)
source_file_path = os.path.join(session2_dir+"/screencap", os.path.basename(session2_dir))
destination_file_path = os.path.join(session1_dir+"/"+reportpathname+"/resource/captures", os.path.basename(session2_dir))
shutil.copytree(source_file_path, destination_file_path, dirs_exist_ok=True)
report_path = os.path.join(session1_dir+"/"+reportpathname, "index.html")

if automationId1 != automationId2:
    print("Automation ids are different, the comparison may not be valid!")
    exit()

# initialize report data
config_json = {}
config_json['type'] = "XR Profiling Session Comparison Report"
config_json['name'] = []
config_json['name'].append(os.path.basename(session1_dir))
config_json['name'].append(os.path.basename(session2_dir))
config_json['Automation Id'] = automationId1
config_json['Device Spec'] = []
config_json['DataSet'] = []
config_json['Captures'] = {}
# add device spec to report
DevicesessionData1 = {}
fDeviceSpec = open(os.path.join(session1_dir, "device_spec.log"), 'r')
deviceSpecs = fDeviceSpec.readlines()
for specLine in deviceSpecs:
    if 'OS version' in specLine:
        specLinesplit = specLine.split(':', 1)
        DevicesessionData1['OS version'] = specLinesplit[1].replace('\n', '')
    if 'Device name' in specLine:
        specLinesplit = specLine.split(',', 1)
        subspecLinesplit1 = specLinesplit[0].split(':', 1)
        DevicesessionData1['Device name'] = subspecLinesplit1[1].replace('\n', '')
        subspecLinesplit2 = specLinesplit[1].split(':', 1)
        DevicesessionData1['Model'] = subspecLinesplit2[1].replace('\n', '')
    if 'Default eye buffer size' in specLine:
        specLinesplit = specLine.split(':', 1)
        DevicesessionData1['Default eye buffer size'] = specLinesplit[1].replace('\n', '')
DevicesessionData1['Start time(first frame)'] = start_time1.strftime('%Y-%m-%d %H:%M:%S.%MS')
DevicesessionData1['End time'] = finish_time1.strftime('%Y-%m-%d %H:%M:%S.%MS')
config_json['Device Spec'].append(DevicesessionData1)
DevicesessionData2 = {}
fDeviceSpec = open(os.path.join(session1_dir, "device_spec.log"), 'r')
deviceSpecs = fDeviceSpec.readlines()
for specLine in deviceSpecs:
    if 'OS version' in specLine:
        specLinesplit = specLine.split(':', 1)
        DevicesessionData2['OS version'] = specLinesplit[1].replace('\n', '')
    if 'Device name' in specLine:
        specLinesplit = specLine.split(',', 1)
        subspecLinesplit1 = specLinesplit[0].split(':', 1)
        DevicesessionData2['Device name'] = subspecLinesplit1[1].replace('\n', '')
        subspecLinesplit2 = specLinesplit[1].split(':', 1)
        DevicesessionData2['Model'] = subspecLinesplit2[1].replace('\n', '')
    if 'Default eye buffer size' in specLine:
        specLinesplit = specLine.split(':', 1)
        DevicesessionData2['Default eye buffer size'] = specLinesplit[1].replace('\n', '')
DevicesessionData2['Start time(first frame)'] = start_time2.strftime('%Y-%m-%d %H:%M:%S.%MS')
DevicesessionData2['End time'] = finish_time2.strftime('%Y-%m-%d %H:%M:%S.%MS')
config_json['Device Spec'].append(DevicesessionData2)

for name,data1 in metricsData1.items():
    data2 = metricsData2[name]
    if hasattr(data1, "timestamps") and hasattr(data2, "timestamps"):
        metricsData = {}
        metricsData['name'] = name
        metricsData['desc'] = data1.description
        metricsData['data'] = []
        avg_value1 = 0
        avg_value2 = 0
        for i in range(0, len(data1.timestamps)):
            subdata1 = {}
            subdata1['x'] = data1.timestamps[i]
            subdata1['y'] = data1.val[i]
            subdata1['name'] = os.path.basename(session1_dir)
            avg_value1 += data1.val[i]
            metricsData['data'].append(subdata1)
        for i in range(0, len(data2.timestamps)):
            subdata2 = {}
            subdata2['x'] = data2.timestamps[i]
            subdata2['y'] = data2.val[i]
            subdata2['name'] = os.path.basename(session2_dir)
            avg_value2 += data2.val[i]
            metricsData['data'].append(subdata2)
        metricsData['value'] = "avg1: {:.2f}".format(avg_value1/len(data1.timestamps))+", "+"avg2: {:.2f}".format(avg_value2/len(data2.timestamps))
        config_json['DataSet'].append(metricsData)
config_json['Captures']['type'] = "comparison"
config_json['Captures']['data'] = []

session1_dir+"/"+reportpathname+"/resource/captures", os.path.basename(session1_dir)
screencaps1 = glob.glob(os.path.join(session1_dir+"/"+reportpathname+"/resource/captures", os.path.basename(session1_dir), '*.png'))
screencaps2 = glob.glob(os.path.join(session1_dir+"/"+reportpathname+"/resource/captures", os.path.basename(session2_dir), '*.png'))

scenename = os.path.basename(screencaps1[0]).split('_', 1)[0]
config_json['Captures']['SceneName'] = scenename

screen_cap_lookups = {}

for path in screencaps1:
    key = os.path.basename(path).rsplit("_", 1)[0]
    screen_cap_lookups[key] = [path, ""]

for path in screencaps2:
    key = os.path.basename(path).rsplit("_", 1)[0]
    if key in screen_cap_lookups:
        screen_cap_lookups[key][1] = path
    else:
        screen_cap_lookups[key] = ["", path]

for name,data in screen_cap_lookups.items():
    Capturedata = {}
    Capturedata['Capture Content'] = os.path.basename(name).split('_', 2)[1]
    Capturedata['info'] = []
    Capturedata['same'] = {}
    subCapture = []
    subCapturebase1 = {}
    subCapturebase2 = {}
    sp_base_path = data[0]
    sp_path = data[1]
    sp_base_name = os.path.basename(sp_base_path)
    sp_name = os.path.basename(sp_path)
    cap_time_str = sp_base_name.split('_', -1)[-1].split('.', 1)[0]
    cap_time = datetime.strptime(cap_time_str[:17], '%Y%m%d%H%M%S%f')
    subCapturebase1['src'] = "./resource/captures/"+os.path.basename(session1_dir)+"/"+sp_base_name
    subCapturebase1['Capture Time'] = cap_time_str[:4]+"-"+cap_time_str[4:6]+"-"+cap_time_str[6:8]+" "+cap_time_str[8:10]+":"+cap_time_str[10:12]+":"+cap_time_str[12:14]+"."+cap_time_str[14:17]
    subCapturebase1['Frame Index'] = str((cap_time-start_time1).seconds)
    cap_time_str = sp_name.split('_', -1)[-1].split('.', 1)[0]
    cap_time = datetime.strptime(cap_time_str[:17], '%Y%m%d%H%M%S%f')
    subCapturebase2['src'] = "./resource/captures/"+os.path.basename(session2_dir)+"/"+sp_name
    subCapturebase2['Capture Time'] = cap_time_str[:4]+"-"+cap_time_str[4:6]+"-"+cap_time_str[6:8]+" "+cap_time_str[8:10]+":"+cap_time_str[10:12]+":"+cap_time_str[12:14]+"."+cap_time_str[14:17]
    subCapturebase2['Frame Index'] = str((cap_time-start_time2).seconds)
    feature_flags = sp_name.rsplit('_',2)[-2]
    if feature_flags != "None":
        for i in range(len(samplefeatures)):
            if feature_flags[i] == '1':
                Capturedata['same'][samplefeatures[i]] = "On"
            elif feature_flags[i] == '0':
                Capturedata['same'][samplefeatures[i]] = "Off"
            else:
                Capturedata['same'][samplefeatures[i]] = "Unknown"
    subCapture.append(subCapturebase1)
    subCapture.append(subCapturebase2)
    Capturedata['info'].append(subCapture)
    config_json['Captures']['data'].append(Capturedata)
# save config.js file
config_json_str = json.dumps(config_json, indent=4)
config_path = os.path.join(session1_dir+"/"+reportpathname+"/resource", "config.js")
fconfig = open(config_path, "w")
fconfig.write('window.configJSON = ')
fconfig.write(config_json_str)
print(f"Comparison report generated to {report_path}")

# open the report in web browser
webbrowser.open_new(report_path)

