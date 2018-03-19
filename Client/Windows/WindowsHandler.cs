﻿using LunaClient.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Profiling;

// ReSharper disable ForCanBeConvertedToForeach

namespace LunaClient.Windows
{
    public static class WindowsHandler
    {
        private static IWindow[] _windows = new IWindow[0];

        /// <summary>
        /// Here we pick all the classes that inherit from ISystem and we put them in the systems array
        /// </summary>
        public static void FillUpWindowsList()
        {
            var windowsList = new List<IWindow>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var systems = assembly.GetTypes().Where(t => t.IsClass && typeof(IWindow).IsAssignableFrom(t) && !t.IsAbstract).ToArray();
                    foreach (var sys in systems)
                    {
                        if (sys.GetProperty("Singleton", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null, null) is IWindow windowImplementation)
                            windowsList.Add(windowImplementation);
                    }
                }
                catch (Exception ex)
                {
                    LunaLog.LogError(string.Format("Exception loading types from assembly {0}: {1}", assembly.FullName, ex.Message));
                }
            }

            _windows = windowsList.ToArray();
        }

        public static void Update()
        {
            for (var i = 0; i < _windows.Length; i++)
            {
                try
                {
                    Profiler.BeginSample(_windows[i].WindowName);
                    _windows[i].Update();
                    Profiler.EndSample();
                }
                catch (Exception e)
                {
                    MainSystem.Singleton.HandleException(e, "WindowsHandler-Update");
                }
            }
        }

        public static void OnGui()
        {
            for (var i = 0; i < _windows.Length; i++)
            {
                try
                {
                    Profiler.BeginSample(_windows[i].WindowName);
                    _windows[i].OnGui();
                    Profiler.EndSample();
                }
                catch (Exception e)
                {
                    MainSystem.Singleton.HandleException(e, "WindowsHandler-OnGui");
                }
            }
        }
    }
}
