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
parser = argparse.ArgumentParser(description="Script to analyze a XR ProfilingToolkit session")

# main
parser.add_argument('-s', '--session', type=str, help="XRProfilingToolkit session result directory", required=True)
parser.add_argument('-f','--features', type=str, help="XRProfilingToolkit session features")
args = parser.parse_args()

sessionDir = args.session

if args.features:
    input_features = args.features.split('|')
    samplefeatures = [feature for feature in input_features if feature in samplefeatures]


if not os.path.isdir(sessionDir):
    print("Please pass in a valid session")
    exit()
    
automationId, metricsDatas,start_time,finish_time = get_metrics_data(sessionDir)

##### genertate report #####
source_file_path = os.path.join(sessionDir, "report_template")
destination_file_path = os.path.join(sessionDir, "analyze_report")
shutil.copytree(source_file_path, destination_file_path, dirs_exist_ok=True)
source_file_path = os.path.join(sessionDir+"/screencap", os.path.basename(sessionDir))
destination_file_path = os.path.join(sessionDir+"/analyze_report/resource", "captures")
shutil.copytree(source_file_path, destination_file_path, dirs_exist_ok=True)
report_path = os.path.join(sessionDir+"/analyze_report", "index.html")
# initialize report data
config_json = {}
config_json['type'] = "XR Profiling Session Analysis Report"
config_json['name'] = []
config_json['name'].append(os.path.basename(sessionDir))
config_json['Automation Id'] = automationId
config_json['Device Spec'] = []
config_json['DataSet'] = []
config_json['Captures'] = {}
# add device spec to report
DevicesessionData = {}
fDeviceSpec = open(os.path.join(sessionDir, "device_spec.log"), 'r')
deviceSpecs = fDeviceSpec.readlines()
for specLine in deviceSpecs:
    if 'OS version' in specLine:
        specLinesplit = specLine.split(':', 1)
        DevicesessionData['OS version'] = specLinesplit[1].replace('\n', '')
    if 'Device name' in specLine:
        specLinesplit = specLine.split(',', 1)
        subspecLinesplit1 = specLinesplit[0].split(':', 1)
        DevicesessionData['Device name'] = subspecLinesplit1[1].replace('\n', '')
        subspecLinesplit2 = specLinesplit[1].split(':', 1)
        DevicesessionData['Model'] = subspecLinesplit2[1].replace('\n', '')
    if 'Default eye buffer size' in specLine:
        specLinesplit = specLine.split(':', 1)
        DevicesessionData['Default eye buffer size'] = specLinesplit[1].replace('\n', '')
DevicesessionData['Start time(first frame)'] = start_time.strftime('%Y-%m-%d %H:%M:%S.%M')
DevicesessionData['End time'] = finish_time.strftime('%Y-%m-%d %H:%M:%S.%M')
config_json['Device Spec'].append(DevicesessionData)
for name,data in metricsDatas.items():
    if hasattr(data, "timestamps"):
        metricsData = {}
        metricsData['name'] = name
        metricsData['desc'] = data.description
        metricsData['data'] = []
        avg_value = 0
        for i in range(0, len(data.timestamps)):
            subdata = {}
            subdata['x'] = data.timestamps[i]
            subdata['y'] = data.val[i]
            subdata['name'] = os.path.basename(sessionDir)
            avg_value += data.val[i]
            metricsData['data'].append(subdata)
        metricsData['value'] = "avg: {:.2f}".format(avg_value/len(data.timestamps))
        config_json['DataSet'].append(metricsData)

config_json['Captures']['type'] = "normal"
config_json['Captures']['data'] = []

screencaps = glob.glob(os.path.join(sessionDir, 'screencap',os.path.splitext(os.path.basename(sessionDir))[0], '*.png'))
scenename = os.path.basename(screencaps[0]).split('_', 1)[0]
config_json['Captures']['SceneName'] = scenename
collection = ScreenCaptureCollection(screencaps)
group:ScreenCaptureGroup
for group in collection.getGroups():
    if len(group.getOtherPaths()) == 0:
        Capturedata = {}
        Capturedata['Capture Content'] = os.path.basename(group.basePath).split('_', 2)[1]
        Capturedata['info'] = []
        Capturedata['same'] = {}
        subCapture = []
        subCapturebase = {}
        sp_base_path = group.basePath
        sp_base_name = os.path.basename(sp_base_path)
        cap_time_str = sp_base_name.split('_', -1)[-1].split('.', 1)[0]
        cap_time = datetime.strptime(cap_time_str[:17], '%Y%m%d%H%M%S%f')
        subCapturebase['src'] = "./resource/captures/"+sp_base_name
        subCapturebase['Capture Time'] = cap_time_str[:4]+"-"+cap_time_str[4:6]+"-"+cap_time_str[6:8]+" "+cap_time_str[8:10]+":"+cap_time_str[10:12]+":"+cap_time_str[12:14]+"."+cap_time_str[14:17]
        subCapturebase['Frame Index'] = str((cap_time-start_time).seconds)
        feature_flags = sp_base_name.rsplit('_',2)[-2]
        if feature_flags != "None":
            for i in range(len(samplefeatures)):
                if feature_flags[i] == '1':
                    Capturedata['same'][samplefeatures[i]] = "On"
                elif feature_flags[i] == '0':
                    Capturedata['same'][samplefeatures[i]] = "Off"
                else:
                    Capturedata['same'][samplefeatures[i]] = "Unknown"
        subCapture.append(subCapturebase)
        Capturedata['info'].append(subCapture)
        config_json['Captures']['data'].append(Capturedata)
    else:
        for path in group.getOtherPaths():
            Capturedata = {}
            Capturedata['Capture Content'] = os.path.basename(group.basePath).split('_', 2)[1]
            Capturedata['info'] = []
            Capturedata['same'] = {}
            subCapture = []
            subCapturebase1 = {}
            subCapturebase2 = {}
            sp_base_path = group.basePath
            sp_path = path
            sp_base_name = os.path.basename(sp_base_path)
            sp_name = os.path.basename(sp_path)
            cap_time_str = sp_base_name.split('_', -1)[-1].split('.', 1)[0]
            cap_time = datetime.strptime(cap_time_str[:17], '%Y%m%d%H%M%S%f')
            subCapturebase1['src'] = "./resource/captures/"+sp_base_name
            subCapturebase1['Capture Time'] = cap_time_str[:4]+"-"+cap_time_str[4:6]+"-"+cap_time_str[6:8]+" "+cap_time_str[8:10]+":"+cap_time_str[10:12]+":"+cap_time_str[12:14]+"."+cap_time_str[14:17]
            subCapturebase1['Frame Index'] = str((cap_time-start_time).seconds)
            cap_time_str = sp_name.split('_', -1)[-1].split('.', 1)[0]
            cap_time = datetime.strptime(cap_time_str[:17], '%Y%m%d%H%M%S%f')
            subCapturebase2['src'] = "./resource/captures/"+sp_name
            subCapturebase2['Capture Time'] = cap_time_str[:4]+"-"+cap_time_str[4:6]+"-"+cap_time_str[6:8]+" "+cap_time_str[8:10]+":"+cap_time_str[10:12]+":"+cap_time_str[12:14]+"."+cap_time_str[14:17]
            subCapturebase2['Frame Index'] = str((cap_time-start_time).seconds)
            feature_flags = sp_name.rsplit('_',2)[-2]
            if feature_flags != "None":
                subCapturebase1['diff'] = {}
                subCapturebase2['diff'] = {}
                for i in range(len(samplefeatures)):
                    if feature_flags[i] == '1':
                        subCapturebase1['diff'][samplefeatures[i]] = "Off"
                        subCapturebase2['diff'][samplefeatures[i]] = "On"
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
config_path = os.path.join(sessionDir+"/analyze_report/resource", "config.js")
fconfig = open(config_path, "w")
fconfig.write('window.configJSON = ')
fconfig.write(config_json_str)
# open the report in web browser
webbrowser.open_new(report_path)
print(f"Analysis report generated to {report_path}")


