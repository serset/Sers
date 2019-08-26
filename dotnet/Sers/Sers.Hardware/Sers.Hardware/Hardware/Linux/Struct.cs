using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TcpTestClient.linux
{


    public class SettingsGroup
    {
        /// <summary>
        /// Gets the name of the group.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the settings found in the group.
        /// </summary>
        public Dictionary<string, Setting> Settings { get; private set; }

        internal SettingsGroup(string name)
        {
            Name = name;
            Settings = new Dictionary<string, Setting>();
        }

        internal SettingsGroup(string name, List<Setting> settings)
        {
            Name = name;
            Settings = new Dictionary<string, Setting>();

            foreach (Setting setting in settings)
                Settings.Add(setting.Name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, int value)
        {
            Setting setting = new Setting(name);
            setting.SetValue(value);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, float value)
        {
            Setting setting = new Setting(name);
            setting.SetValue(value);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, bool value)
        {
            Setting setting = new Setting(name);
            setting.SetValue(value);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, string value)
        {
            Setting setting = new Setting(name);
            setting.SetValue(value);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The values of the setting.</param>
        public void AddSetting(string name, params int[] values)
        {
            Setting setting = new Setting(name);
            setting.SetValue(values);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The values of the setting.</param>
        public void AddSetting(string name, params float[] values)
        {
            Setting setting = new Setting(name);
            setting.SetValue(values);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The values of the setting.</param>
        public void AddSetting(string name, params bool[] values)
        {
            Setting setting = new Setting(name);
            setting.SetValue(values);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The values of the setting.</param>
        public void AddSetting(string name, params string[] values)
        {
            Setting setting = new Setting(name);
            setting.SetValue(values);
            Settings.Add(name, setting);
        }

        /// <summary>
        /// Deletes a setting from the group.
        /// </summary>
        /// <param name="name">The name of the setting to delete.</param>
        public void DeleteSetting(string name)
        {
            Settings.Remove(name);
        }
    }



    public class Setting
    {
        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the raw value of the setting.
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// Gets whether or not the setting is an array.
        /// </summary>
        public bool IsArray { get; private set; }

        internal Setting(string name)
        {
            Name = name;
            RawValue = string.Empty;
            IsArray = false;
        }

        internal Setting(string name, string value, bool isArray)
        {
            Name = name;
            RawValue = value;
            IsArray = isArray;
        }

        /// <summary>
        /// Attempts to return the setting's value as an integer.
        /// </summary>
        /// <returns>An integer representation of the value</returns>
        public int GetValueAsInt()
        {
            return int.Parse(RawValue, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Attempts to return the setting's value as a float.
        /// </summary>
        /// <returns>A float representation of the value</returns>
        public float GetValueAsFloat()
        {
            return float.Parse(RawValue, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Attempts to return the setting's value as a bool.
        /// </summary>
        /// <returns>A bool representation of the value</returns>
        public bool GetValueAsBool()
        {
            return bool.Parse(RawValue);
        }

        /// <summary>
        /// Attempts to return the setting's value as a string.
        /// </summary>
        /// <returns>A string representation of the value</returns>
        public string GetValueAsString()
        {
            ;

            return RawValue;
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of integers.
        /// </summary>
        /// <returns>An integer array representation of the value</returns>
        public int[] GetValueAsIntArray()
        {
            string[] parts = RawValue.Split(',');

            int[] valueParts = new int[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                valueParts[i] = int.Parse(parts[i], CultureInfo.InvariantCulture.NumberFormat);

            return valueParts;
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of floats.
        /// </summary>
        /// <returns>An float array representation of the value</returns>
        public float[] GetValueAsFloatArray()
        {
            string[] parts = RawValue.Split(',');

            float[] valueParts = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                valueParts[i] = float.Parse(parts[i], CultureInfo.InvariantCulture.NumberFormat);

            return valueParts;
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of bools.
        /// </summary>
        /// <returns>An bool array representation of the value</returns>
        public bool[] GetValueAsBoolArray()
        {
            string[] parts = RawValue.Split(',');

            bool[] valueParts = new bool[parts.Length];

            for (int i = 0; i < parts.Length; i++)
                valueParts[i] = bool.Parse(parts[i]);

            return valueParts;
        }

        /// <summary>
        /// Attempts to return the setting's value as an array of strings.
        /// </summary>
        /// <returns>An string array representation of the value</returns>
        public string[] GetValueAsStringArray()
        {
            Match match = Regex.Match(RawValue, "[\\\"][^\\\"]*[\\\"][,]*");

            List<string> values = new List<string>();

            while (match.Success)
            {
                string value = match.Value;
                if (value.EndsWith(","))
                    value = value.Substring(0, value.Length - 1);

                value = value.Substring(1, value.Length - 2);
                values.Add(value);
                match = match.NextMatch();
            }

            return values.ToArray();
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new value to store.</param>
        public void SetValue(int value)
        {
            RawValue = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new value to store.</param>
        public void SetValue(float value)
        {
            RawValue = value.ToString(CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new value to store.</param>
        public void SetValue(bool value)
        {
            RawValue = value.ToString();
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new value to store.</param>
        public void SetValue(string value)
        {
            RawValue = assertStringQuotes(value);
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new values to store.</param>
        public void SetValue(params int[] values)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(values[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
                if (i < values.Length - 1)
                    builder.Append(",");
            }

            RawValue = builder.ToString();
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new values to store.</param>
        public void SetValue(params float[] values)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(values[i].ToString(CultureInfo.InvariantCulture.NumberFormat));
                if (i < values.Length - 1)
                    builder.Append(",");
            }

            RawValue = builder.ToString();
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new values to store.</param>
        public void SetValue(params bool[] values)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(values[i]);
                if (i < values.Length - 1)
                    builder.Append(",");
            }

            RawValue = builder.ToString();
        }

        /// <summary>
        /// Sets the value of the setting.
        /// </summary>
        /// <param name="value">The new values to store.</param>
        public void SetValue(params string[] values)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(assertStringQuotes(values[i]));
                if (i < values.Length - 1)
                    builder.Append(",");
            }

            RawValue = builder.ToString();
        }

        private static string assertStringQuotes(string value)
        {
            return value;
        }

        public string GetValue()
        {
            return RawValue;
        }
    }

    public class ConfigFile
    {

        /// <summary>
        /// Gets the groups found in the configuration file.
        /// </summary>
        public Dictionary<string, SettingsGroup> SettingGroups { get; private set; }

        /// <summary>
        /// Creates a blank configuration file.
        /// </summary>
        public ConfigFile()
        {
            SettingGroups = new Dictionary<string, SettingsGroup>();
        }

        /// <summary>
        /// Loads a configuration file.
        /// </summary>
        /// <param name="file">The filename where the configuration file can be found.</param>
        public ConfigFile(string file)
        {
            Load(file);
        }

        /// <summary>
        /// Loads a configuration file.
        /// </summary>
        /// <param name="file">The stream from which to load the configuration file.</param>
        public ConfigFile(Stream stream)
        {
            Load(stream);
        }

        /// <summary>
        /// Adds a new settings group to the configuration file.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>The newly created SettingsGroup.</returns>
        public SettingsGroup AddSettingsGroup(string groupName)
        {
            if (SettingGroups.ContainsKey(groupName))
                throw new Exception("Group already exists with name '" + groupName + "'");

            SettingsGroup group = new SettingsGroup(groupName);
            SettingGroups.Add(groupName, group);

            return group;
        }

        /// <summary>
        /// Deletes a settings group from the configuration file.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        public void DeleteSettingsGroup(string groupName)
        {
            SettingGroups.Remove(groupName);
        }

        /// <summary>
        /// Loads the configuration from a file.
        /// </summary>
        /// <param name="file">The file from which to load the configuration.</param>
        public void Load(string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                Load(stream);
            }
        }

        /// <summary>
        /// Loads the configuration from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the configuration.</param>
        public void Load(Stream stream)
        {
            //track line numbers for exceptions
            int lineNumber = 0;

            //groups found
            List<SettingsGroup> groups = new List<SettingsGroup>();

            //current group information
            string currentGroupName = null;
            List<Setting> settings = null;

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lineNumber++;

                    //strip out comments
                    if (line.Contains("#"))
                    {
                        if (line.IndexOf("#") == 0)
                            continue;

                        line = line.Substring(0, line.IndexOf("#"));
                    }

                    //trim off any extra whitespace
                    line = line.Trim();

                    //try to match a group name
                    Match match = Regex.Match(line, "\\[[a-zA-Z\\d\\s]+\\]");

                    //found group name
                    if (match.Success)
                    {
                        //if we have a current group we're on, we save it
                        if (settings != null && currentGroupName != null)
                            groups.Add(new SettingsGroup(currentGroupName, settings));

                        //make sure the name exists
                        if (match.Value.Length == 2)
                            throw new Exception(string.Format("Group must have name (line {0})", lineNumber));

                        //set our current group information
                        currentGroupName = match.Value.Substring(1, match.Length - 2);
                        settings = new List<Setting>();
                    }

                    //no group name, check for setting with equals sign
                    else if (line.Contains("="))
                    {
                        //split the line
                        string[] parts = line.Split(new[] { '=' }, 2);

                        //if we have any more than 2 parts, we have a problem
                        if (parts.Length != 2)
                            throw new Exception(string.Format("Settings must be in the format 'name = value' (line {0})", lineNumber));

                        //trim off whitespace
                        parts[0] = parts[0].Trim();
                        parts[1] = parts[1].Trim();

                        //figure out if we have an array or not
                        bool isArray = false;
                        bool inString = false;

                        //go through the characters
                        foreach (char c in parts[1])
                        {
                            //any comma not in a string makes us creating an array
                            if (c == ',' && !inString)
                                isArray = true;

                            //flip the inString value each time we hit a quote
                            else if (c == '"')
                                inString = !inString;
                        }

                        //if we have an array, we have to trim off whitespace for each item and
                        //do some checking for boolean values.
                        if (isArray)
                        {
                            //split our value array
                            string[] pieces = parts[1].Split(',');

                            //need to build a new string
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < pieces.Length; i++)
                            {
                                //trim off whitespace
                                string s = pieces[i].Trim();

                                //convert to lower case
                                string t = s.ToLower();

                                //check for any of the true values
                                if (t == "on" || t == "yes" || t == "true")
                                    s = "true";

                                //check for any of the false values
                                else if (t == "off" || t == "no" || t == "false")
                                    s = "false";

                                //append the value
                                builder.Append(s);

                                //if we are not on the last value, add a comma
                                if (i < pieces.Length - 1)
                                    builder.Append(",");
                            }

                            //save the built string as the value
                            parts[1] = builder.ToString();
                        }

                        //if not an array
                        else
                        {
                            //make sure we are not working with a string value
                            if (!parts[1].StartsWith("\""))
                            {
                                //convert to lower
                                string t = parts[1].ToLower();

                                //check for any of the true values
                                if (t == "on" || t == "yes" || t == "true")
                                    parts[1] = "true";

                                //check for any of the false values
                                else if (t == "off" || t == "no" || t == "false")
                                    parts[1] = "false";
                            }
                        }

                        //add the setting to our list making sure, once again, we have stripped
                        //off the whitespace
                        settings.Add(new Setting(parts[0].Trim(), parts[1].Trim(), isArray));
                    }
                }
            }

            //make sure we save off the last group
            if (settings != null && currentGroupName != null)
                groups.Add(new SettingsGroup(currentGroupName, settings));

            //create our new group dictionary
            SettingGroups = new Dictionary<string, SettingsGroup>();

            //add each group to the dictionary
            foreach (SettingsGroup group in groups)
            {
                SettingGroups.Add(group.Name, group);
            }
        }

        /// <summary>
        /// Saves the configuration to a file
        /// </summary>
        /// <param name="filename">The filename for the saved configuration file.</param>
        public void Save(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Saves the configuration to a stream.
        /// </summary>
        /// <param name="stream">The stream to which the configuration will be saved.</param>
        public void Save(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                foreach (KeyValuePair<string, SettingsGroup> groupValue in SettingGroups)
                {
                    writer.WriteLine("[{0}]", groupValue.Key);
                    foreach (KeyValuePair<string, Setting> settingValue in groupValue.Value.Settings)
                    {
                        writer.WriteLine("{0}={1}", settingValue.Key, settingValue.Value.RawValue);
                    }
                    writer.WriteLine();
                }
            }
        }
    }




    public class ServerInfo
    {
        /// <summary>
        /// 内存
        /// </summary>
        public MemInfo MemInfo { get; set; }

        /// <summary>
        ///硬盘信息
        /// </summary>
        public HDDInfo HDDInfo { get; set; }

        /// <summary>
        /// 网络信息
        /// </summary>
        public NetworkInfo NetworkInfo { get; set; }

        /// <summary>
        /// CPU序列号
        /// </summary>
        public string CpuSerialNumber { get; set; }

        /// <summary>
        /// CPU温度
        /// </summary>
        public float CpuTemperature { get; set; }

        /// <summary>
        /// CPU使用率
        /// </summary>
        public float CpuUsage { get; set; }

        /// <summary>
        /// 接包数据
        /// </summary>
        public long PacketCount { get; set; }

        /// <summary>
        /// 当前会话连接数
        /// </summary>
        public int SessionCount { get; set; }
        /// <summary>
        /// 写入时间
        /// </summary>
        public DateTime create_time { get; set; }
    }




    public class RunTimeInfo
    {
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string LastLogon { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// CPU序列号
        /// </summary>
        public string CpuSerialNo { get; set; } = string.Empty;

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime Startup { get; set; } = DateTime.Now;

        /// <summary>
        /// 魔方Token
        /// </summary>
        public string MagicToken { get; set; } = string.Empty;

        /// <summary>
        /// MAC地址
        /// </summary>
        public string MacAddress { get; set; } = string.Empty;

        /// <summary>
        /// 数据包大小
        /// </summary>
        public long PacketCount { get; set; } = 0;

        /// <summary>
        /// TCP连接数据
        /// </summary>
        public int TcpSessionCount { get; set; } = 0;
    }



    public class NetworkInfo
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 子掩码
        /// </summary>
        public string SubnetMark { get; set; }

        /// <summary>
        /// 网关
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// DNS信息
        /// </summary>
        public string DNS { get; set; }

        /// <summary>
        /// MAC地址
        /// </summary>
        public string MacAddress { get; set; }
    }





    public class MemInfo
    {
        /// <summary>
        /// 总计内存大小
        /// </summary>
        public string Total { get; set; }
        /// <summary>
        /// 可用内存大小
        /// </summary>
        public string Available { get; set; }
    }




    public class HDDInfo
    {
        /// <summary>
        /// 硬盘大小
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 已使用大小
        /// </summary>
        public string Used { get; set; }

        /// <summary>
        /// 可用大小
        /// </summary>
        public string Avail { get; set; }

        /// <summary>
        /// 使用率
        /// </summary>
        public string Usage { get; set; }
    }

}
