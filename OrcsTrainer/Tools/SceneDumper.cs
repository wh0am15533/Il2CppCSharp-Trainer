using System;
using System.IO;
using System.Text;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using Il2CppSystem.Reflection;
using Il2CppSystem.Diagnostics;

namespace Trainer.Tools
{
    public static class SceneDumper
    {
        public static void DumpObjects(params GameObject[] objects)
        {
            var fname = AppDomain.CurrentDomain.BaseDirectory + "\\SceneDump.txt"; //Path.GetTempFileName()

            BepInExLoader.log.LogMessage($"Dumping {objects.Length} GameObjects to {fname}");

            using (var f = File.OpenWrite(fname))
            using (var sw = new StreamWriter(f, Encoding.UTF8))
            {
                foreach (var obj in objects)
                    PrintRecursive(sw, obj);
            }

            BepInExLoader.log.LogMessage("Complete!");
            BepInExLoader.log.LogMessage($"Opening {fname}");

            Trainer.Helpers.StartExternalProcess.Start("notepad.exe " + fname, AppDomain.CurrentDomain.BaseDirectory);            
            // Il2CppSystem way doesn't work - SUCCESS Exception. WTF??!
            //var pi = new ProcessStartInfo(fname) { UseShellExecute = true };
            //Process.Start(pi);
        }

        private static void PrintRecursive(TextWriter sw, GameObject obj, int d = 0)
        {
            if (obj == null) return;

            var pad1 = new string(' ', 3 * d);
            var pad2 = new string(' ', 3 * (d + 1));
            var pad3 = new string(' ', 3 * (d + 2));
            sw.WriteLine(pad1 + obj.name + "--" + obj.GetIl2CppType().FullName);

            foreach (var c in obj.GetComponents<Component>())
            {
                sw.WriteLine(pad2 + "::" + c.GetIl2CppType().Name);

                var ct = c.GetIl2CppType();

                var props = ct.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
                foreach (var p in props)
                {
                    try
                    {
                        var v = p.GetValue(c);
                        sw.WriteLine(pad3 + "@" + p.Name + "<" + p.PropertyType.Name + "> = " + v.ToString());
                    }
                    catch (Exception e)
                    {
                        sw.WriteLine(pad3 + "@" + p.Name + "<" + p.PropertyType.Name + "> = null");
                        //BepInExLoader.log.LogMessage(p.Name + "<" + p.PropertyType.Name + "> = " + "Couldn't Resolve Value!"); //"Null Property or No Get() Method Exists!"
                    }
                }
            }

            foreach (var child in obj.transform)
            {
                var t = child.Cast<Transform>();
                PrintRecursive(sw, t.gameObject, d + 1);
            }
        }
    }
}
