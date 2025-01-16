#################################################################################################################
## Copyright (c) 2024 PICO Developer
## SPDX-License-Identifier: MIT
## Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and#or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
## The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
## THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
################################################################################################################
# Author: Xutong Zhou (xutong.zhou@bytedance.com)

from parse import *
import dateutil.parser 
import os
from datetime import datetime, timedelta
import glob
import json

@with_pattern(r'([A-Za-z]{1}[A-Za-z\d_]*\.)+[A-Za-z][A-Za-z\d_]*')
def parse_package_name(text):
    return text

def parse_metrics(metricsSchema, metricsDesc, dataEntries, key_val_separator):
    res = {}
    desc = {}
    for entry in dataEntries:
        keyVal = entry.split(key_val_separator)
        keyVal[0] = keyVal[0].strip()
        keyVal[1] = keyVal[1].strip()
        if keyVal[0] in metricsSchema.keys():
            matched = search(metricsSchema[keyVal[0]], keyVal[1], 0, None, {"package_name" : parse_package_name}).named
            # add metrics description
            if keyVal[0] in metricsDesc:
                for pattern in matched:
                    # convert matched pattern to value name, exclude the matched type (like value, unit)
                    metricsId = '.'.join(pattern.split('.')[:-1])
                    desc[metricsId] = metricsDesc[keyVal[0]]               
            res.update(matched)
    return res, desc

# generate metrics data for the given session
def get_metrics_data(sessionDir):
    adb_timestamp_format = "%m-%d %H:%M:%S.%f"
    current_year = dateutil.parser.parse(sessionDir, fuzzy=True).year

    ##### read start time, finish time and id from the xrprofilingtoolkit log #####
    fXRProfilingToolkitLog = open(os.path.join(sessionDir, "xr_profilingtoolkit.log"), "r")
    logLines = fXRProfilingToolkitLog.readlines()

    automationId = ""

    for line in logLines:
        if not line:
            continue
        time_str = line[0:18]
        time = datetime.strptime(f"{current_year} {time_str}", f"%Y {adb_timestamp_format}")
        if "starting" in line:
            start_time = time
            automationId = line.split(":")[-1].strip()
        if "finished" in line:
            finish_time = time

    ##### handle adb performance metrics #####
    metricsFileName = glob.glob(os.path.join(sessionDir, '*_metrics.log'))[0]
    metricsSchemaPath = glob.glob(os.path.join(sessionDir, '*_metrics.schema'))[0]
    # load metrics data schema
    metricsSchema, metricsDesc, metric_splitter, key_val_separator, metricsLines = load_metrics_with_schema(metricsFileName, metricsSchemaPath)

    timeStamps = []
    metricsData = {}

    for line in metricsLines:  
        if not line or line.startswith("---------"):
            continue
        time_str = line[0:18]
        time = datetime.strptime(f"{current_year} {time_str}", f"%Y {adb_timestamp_format}")
        if time > finish_time:
            break
        if time < start_time:
            continue

        duration = time - start_time
        
        timeStamps.append(duration)
        dataStr = line[18:].split(":")[1].strip()
        dataEntries = dataStr.split(metric_splitter)
        for i in range(len(dataEntries)):
            # merge the entry with previous one since it is not a separate data entry
            # previous entry contains separator in the middle
            if '=' not in dataEntries[i]:
                dataEntries[i - 1] = metric_splitter.join([dataEntries[i - 1], dataEntries[i]])
                # set the entry to empty to be removed later
                dataEntries[i] = ''
                
        # remove empty entry
        dataEntries = [i for i in dataEntries if i]
        parsedMetrics, parsedDesc = parse_metrics(metricsSchema, metricsDesc, dataEntries, key_val_separator)
        lineData = {}
        for metric in parsedMetrics:
            if "ignore" in metric:
                continue
            tags = metric.split(".", 1)
            name = tags[0] 
            type = tags[1]
            if lineData.get(name) == None:
                lineData[name] = []
            lineData[name].append((metric, parsedMetrics[metric]))
        
        for mKey in lineData:
            if mKey not in metricsData.keys():
                metricsData[mKey] = create_metrics(lineData[mKey])
            
            metricsData[mKey].append(lineData[mKey])

        # add description to metrics data
        # TODO(xutong.zhou): do it only once, not every metrics line
        for pd in parsedDesc:
            mName = pd.split('.')[0]
            if mName in metricsData:
                metricsData[mName].add_description(pd, parsedDesc[pd]) 

    # this is for adb metrics only
    x_seconds = [time.total_seconds() for time in timeStamps]

    for md in metricsData:
        metricsData[md].add_timestamps(x_seconds)

    ##### handle pil tools output #####
    metricsFileName = os.path.join(sessionDir, 'pil_output.log')
    #metricsSchemaPath = os.path.splitext(os.path.basename(metricsFileName))[0] + ".schema"
    metricsSchemaPath = os.path.join(sessionDir, 'pil_output.schema')

    # load metrics data schema
    metricsSchema, metricsDesc, metric_splitter, key_val_separator, metricsLines = load_metrics_with_schema(metricsFileName, metricsSchemaPath)
    # convert start time to datetime, exclide last three characters to convert nano seconds to micro seconds
    pil_start_time = datetime.strptime(metricsLines[0].split('\n', 1)[0][:-3], "%Y%m%d%H%M%S%f")
    metricsLines[0] = metricsLines[0].split('\n', 1)[1]
    time = pil_start_time
    timeStamps = []
    pil_metrics = []
    for line in metricsLines:
        if not line:
            continue
        lineData = {}
        time = time + timedelta(seconds=1)
        if time > finish_time:
            break
        if time < start_time:
            continue
        
        duration = time - start_time
        timeStamps.append(duration)
        dataEntries = line.split(metric_splitter)
        
        parsedMetrics, parsedDesc = parse_metrics(metricsSchema, metricsDesc, dataEntries, key_val_separator)
        for metric in parsedMetrics:
            tags = metric.split(".", 1)
            name = tags[0] 
            type = tags[1]
            if lineData.get(name) == None:
                lineData[name] = []
            lineData[name].append((metric, parsedMetrics[metric]))
        
        for mKey in lineData:
            if mKey not in metricsData.keys():
                metricsData[mKey] = create_metrics(lineData[mKey])
                pil_metrics.append(mKey)
            
            metricsData[mKey].append(lineData[mKey])

        # add description to metrics data
        # TODO(xutong.zhou): do it only once, not every metrics line
        for pd in parsedDesc:
            mName = pd.split('.')[0]
            if mName in metricsData:
                metricsData[mName].add_description(pd, parsedDesc[pd])

    pil_time_seconds = [time.total_seconds() for time in timeStamps]

    for mKey in pil_metrics:
        metricsData[mKey].add_timestamps(pil_time_seconds)

    return automationId, metricsData,start_time,finish_time

def load_metrics_with_schema(metricsFileName, metricsSchemaPath):
    fMetrics = open(metricsFileName, "r")
    fMetricsSchema = open(metricsSchemaPath, "r")
    
    metricsSchemaJson = json.loads(fMetricsSchema.read())

    metric_splitter = metricsSchemaJson['metric_splitter']
    key_val_separator = metricsSchemaJson['key_val_separator']
    metric_line_splitter = metricsSchemaJson["line_splitter"]
    metricsSchema={}
    metricsDesc={}

    for metric in metricsSchemaJson['metrics']:
        if metric['enabled'] == 0:
            continue
        metricsSchema[metric['name']] = metric['template']
        if 'description' in metric:
            metricsDesc[metric['name']] = metric['description']
    
    metricLines = fMetrics.read().split(metric_line_splitter)
    return metricsSchema, metricsDesc, metric_splitter, key_val_separator, metricLines

def create_metrics(metricsData):
    metrics = None

    for entry in metricsData:
        tags = entry[0].split(".")
        if len(tags) > 2:
            metrics = MetricsValueSet(metricsData)
            break
        elif tags[-1] == "status":
            metrics = MetricsStatus()
            break
        elif tags[-1] == "stringValue":
            metrics = MetricsStringValue()
            break
        elif tags[-1] == "value":
            metrics = MetricsValue()
            break
    
    return metrics
    
class MetricsValue:
    def __init__(self, unit = None, maxValue = None, maxValueUnit = None):
        self.val = []
        self.unit = unit
        self.maxValue = maxValue
        self.maxValueUnit = maxValueUnit
        self.description = ""
        self.timestamps = []
    
    def append(self, data):
        for entry in data:
            tag = entry[0].split(".")[-1]
            if tag == "value":
                self.val.append(entry[1])
            elif tag == "unit":
                self.unit = entry[1]
            elif tag == "maxValue":
                self.maxValue = entry[1]
            elif tag == "maxValueUnit":
                self.maxValueUnit = entry[1]
    
    def add_description(self, name, description):
        self.description = description
    
    def average(self):
        assert(len(self.val) == len(self.timestamps))
        total = 0
        for i in range(0, len(self.val)):
            duration = self.timestamps[i] if i == 0 else self.timestamps[i] - self.timestamps[i - 1]
            total += duration * self.val[i]
        return total / self.timestamps[-1]
    
    def add_timestamps(self, timestamps):
        self.timestamps = timestamps           

class MetricsStatus:
    def __init__(self):
        self.val = []
        self.description = ""
        self.timestamps = []
        
    def add_description(self, name, description):
        self.description = description
    
    def append(self, data):
        for entry in data:
            tag = entry[0].split(".")[-1]
            if tag == "status":
                self.val.append(entry[1])
    
    def add_timestamps(self, timestamps):
        self.timestamps = timestamps
      
class MetricsStringValue:
    def __init__(self):
        self.val = []
        self.description = ""
        self.timestamps = []
    
    def add_description(self, name, description):
        self.description = description

    def append(self, data):
        for entry in data:
            tag = entry[0].split(".")[-1]
            if tag == "stringValue":
                self.status.append(entry[1])
                
    def add_timestamps(self, timestamps):
        self.timestamps = timestamps   

class MetricsValueSet:
    def __init__(self, metricsData):
        self.valset = {}
        for entry in metricsData:
            tags = entry[0].split(".")
            if tags[1] not in self.valset.keys():
                self.valset[tags[1]] = MetricsValue()
    
    def append(self, data):
        dataset = {}
        for entry in data:
            tags = entry[0].split(".")
            if tags[1] not in dataset.keys():
                dataset[tags[1]] = []
            dataset[tags[1]].append((tags[-1], entry[1]))
        for mKey in dataset:
            self.valset[mKey].append(dataset[mKey])
    
    # name to look up for sub metrics value
    def add_description(self, name, description):
        subvalName = name.split('.')[-1]
        if subvalName in self.valset:
            self.valset[subvalName].add_description(subvalName, description)
        self.description = description

    def average(self):
        avgs = {}
        for mKey in self.valset:
            avgs[mKey] = self.valset[mKey].average()
        return avgs

    def add_timestamps(self, timestamps):
        for mKey in self.valset:
            self.valset[mKey].add_timestamps(timestamps)  