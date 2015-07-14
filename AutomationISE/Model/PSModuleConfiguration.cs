﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;

using System.Diagnostics;

namespace AutomationISE.Model
{
    public class PSModuleConfiguration
    {
        /* Updates the AzureAutomationAuthoringToolkit PowerShell Module to point at the specified workspace directory */
        public static void UpdateModuleConfiguration(string workspace)
        {
            string modulePath = findModulePath();
            string configFilePath = System.IO.Path.Combine(modulePath, ModuleData.ConfigFileName);
            if (!File.Exists(configFilePath))
            {
                Debug.WriteLine("Warning: a config file wasn't found in the module, so a new one will be created");
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<PathConfiguration> config = jss.Deserialize<List<PathConfiguration>>((File.ReadAllText(configFilePath)));
            foreach (PathConfiguration pc in config)
            {
                if (pc.Name.Equals(ModuleData.LocalAssetsPath_FieldName))
                {
                    pc.Value = System.IO.Path.Combine(workspace, ModuleData.LocalAssetsFileName);
                }
                else if (pc.Name.Equals(ModuleData.SecureLocalAssetsPath_FieldName))
                {
                    pc.Value = System.IO.Path.Combine(workspace, ModuleData.SecureLocalAssetsFileName);
                }
                else
                {
                    Debug.WriteLine("Unknown configuration found: " + pc.Name);
                }
            }
            File.WriteAllText(configFilePath, jss.Serialize(config));
        }

        private static string findModulePath()
        {
            String[] moduleLocations = Environment.GetEnvironmentVariable(ModuleData.EnvPSModulePath).Split(';');
            foreach (String moduleLocation in moduleLocations)
            {
                String possibleModulePath = System.IO.Path.Combine(moduleLocation, ModuleData.ModuleName);
                if (Directory.Exists(possibleModulePath))
                {
                    return possibleModulePath;
                }
            }
            return null;
        }
        
        public class PathConfiguration
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static class ModuleData
        {
            public const string ModuleName = "AzureAutomationAuthoringToolkit";
            public const string ConfigFileName = "Config.json";
            public const string LocalAssetsPath_FieldName = "LocalAssetsPath";
            public const string SecureLocalAssetsPath_FieldName = "SecureLocalAssetsPath";
            public const string LocalAssetsFileName = "LocalAssets.json";
            public const string SecureLocalAssetsFileName = "SecureLocalAssets.json";
            public const string EnvPSModulePath = "PSModulePath";
        }
    }
}