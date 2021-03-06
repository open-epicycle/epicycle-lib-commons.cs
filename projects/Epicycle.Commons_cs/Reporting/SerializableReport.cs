﻿// [[[[INFO>
// Copyright 2015 Epicycle (http://epicycle.org, https://github.com/open-epicycle)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// For more information check https://github.com/open-epicycle/Epicycle.Commons-cs
// ]]]]

using System;
using System.Collections.Generic;
using System.Text;

namespace Epicycle.Commons.Reporting
{
    public sealed class SerializableReport : IReport
    {
        private object _lock = new object();

        public static readonly string Indentation = "    ";

        private IList<KeyValuePair<string, object>> _entries;

        public SerializableReport()
        {
            _entries = null;
            Prefix = null;
        }

        public string Prefix { get; set; }

        public IReport SubReport(string name)
        {
            var subReport = new SerializableReport();

            ReportInner(name, subReport);

            return subReport;
        }

        public void Report(string name, int value)
        {
            ReportInner(name, value);
        }

        public void Report(string name, long value)
        {
            ReportInner(name, value);
        }

        public void Report(string name, float value)
        {
            ReportInner(name, value);
        }

        public void Report(string name, double value)
        {
            ReportInner(name, value);
        }

        public void Report(string name, string value)
        {
            ReportInner(name, value);
        }

        public void Report(string name, object value)
        {
            ReportInner(name, value);
        }

        private void ReportInner(string name, object value)
        {
            lock (_lock)
            {
                if (_entries == null)
                {
                    _entries = new List<KeyValuePair<string, object>>();
                }

                _entries.Add(new KeyValuePair<string, object>(name, value));
            }
        }

        public string Serialize()
        {
            return Serialize(level: 0);
        }

        private string Serialize(int level)
        {
            lock (_lock)
            {
                return InnerSerialize(level);
            }
        }

        private string InnerSerialize(int level)
        {
            var result = new StringBuilder();

            var correctedPrefix = Prefix.EnsureNewLineIfNotEmpty();
            if(correctedPrefix != null)
            {
                result.Append(correctedPrefix);
            }

            WriteEntries(result, level);

            return result.ToString();
        }

        private void WriteEntries(StringBuilder result, int level)
        {
            if (_entries == null || _entries.Count == 0)
            {
                return;
            }

            var linePrefix = Indentation.Repeat(level);

            foreach (var entry in _entries)
            {
                result.Append(linePrefix);

                var name = entry.Key;
                var value = entry.Value;

                if (value is SerializableReport)
                {
                    var subReporter = (SerializableReport)value;

                    result.Append(String.Format("{0}:\n", name));
                    result.Append(subReporter.Serialize(level + 1));
                }
                else
                {
                    WriteValue(result, name, value);
                }
            }
        }

        private void WriteValue(StringBuilder result, string name, object value)
        {
            result.Append(String.Format("{0}: {1}\n", name, value.ToString()));
        }
    }
}
