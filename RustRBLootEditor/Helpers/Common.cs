using RustRBLootEditor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace RustRBLootEditor.Helpers
{
    public static class Common
    {
        public static bool SaveJson<T>(T theobject, string filePath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        //sw.WriteLine("#" + Common.GetVersion());

                        using (MemoryStream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer
                            (typeof(T));
                            serializer.WriteObject(ms, theobject);
                            Encoding enc = Encoding.UTF8;
                            sw.Write(enc.GetString(ms.ToArray()));
                        }
                    }

                    fs.Flush();
                    fs.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static T LoadJson<T>(string filePath)
        {
            T result;
            if (!System.IO.File.Exists(filePath))
            {
                return default(T);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string filetext = sr.ReadToEnd();

                    if (filetext.StartsWith("#"))
                    {
                        int firstlineindex = filetext.IndexOf(System.Environment.NewLine);
                        filetext = filetext.Substring(firstlineindex + System.Environment.NewLine.Length);
                    }

                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(filetext)))
                    {
                        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                        var deserializer = new DataContractJsonSerializer(typeof(T), settings);
                        result = (T)deserializer.ReadObject(ms);
                    }
                }

                fs.Close();
            }

            return result;
        }

        public static T LoadJsonResource<T>(string resourceName)
        {
            T result;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string filetext = sr.ReadToEnd();

                    if (filetext.StartsWith("#"))
                    {
                        int firstlineindex = filetext.IndexOf(System.Environment.NewLine);
                        filetext = filetext.Substring(firstlineindex + System.Environment.NewLine.Length);
                    }

                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(filetext)))
                    {
                        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                        var deserializer = new DataContractJsonSerializer(typeof(T), settings);
                        result = (T)deserializer.ReadObject(ms);
                    }
                }

                stream.Close();
            }

            return result;
        }

        public static T GetParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject dependencyObject = VisualTreeHelper.GetParent(child);

            if (dependencyObject != null)
            {
                T parent = dependencyObject as T;

                if (parent != null)
                {
                    return parent;
                }
                else
                {
                    return GetParent<T>(dependencyObject);
                }
            }
            else
            {
                return null;
            }
        }

        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }

        public static void DownloadImages(List<RustItem> items, string saveLoc)
        {
            Directory.CreateDirectory(saveLoc);
            foreach (var item in items)
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileAsync(item.image, saveLoc + "\\" + item.shortName + ".png");
                }
            }
        }


        public static bool ResourceExists(string resourcePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            return ResourceExists(assembly, resourcePath);
        }

        public static bool ResourceExists(Assembly assembly, string resourcePath)
        {
            var a = new List<object>(GetResourcePaths(assembly));
            return a.Contains(resourcePath.ToLowerInvariant());
        }

        public static IEnumerable<object> GetResourcePaths(Assembly assembly)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var resourceName = assembly.GetName().Name + ".g";
            var resourceManager = new ResourceManager(resourceName, assembly);

            try
            {
                var resourceSet = resourceManager.GetResourceSet(culture, true, true);

                foreach (System.Collections.DictionaryEntry resource in resourceSet)
                {
                    yield return resource.Key;
                }
            }
            finally
            {
                resourceManager.ReleaseAllResources();
            }
        }
    }
}
