using CandySugar.Com.Options.ComponentObject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using XExten.Advance.LogFramework;

namespace CandySugar.Com.Library.DLLoader
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        private string _Path;
        public static ConcurrentQueue<Type> Single = new ConcurrentQueue<Type>();
        public static List<DLLInformations> Dll = new List<DLLInformations>();
        public AssemblyLoader(string Path)
        {
            _Path = Path;
            this.Resolving += DllResolving;
        }

        #region Event
        private Assembly DllResolving(AssemblyLoadContext context, AssemblyName name)
        {
            string[] Filter = { $"{Path.GetFileNameWithoutExtension(context.Name)}.resources" };
            if (Filter.Contains(name.Name)) return null;
            string expectedPath = Path.Combine(_Path, name.Name + ".dll");
            if (File.Exists(expectedPath)) return context.LoadFromAssemblyPath(expectedPath);
            else return null;
        }
        #endregion

        public void Loads(string dllFileName, string typeName, string ioc = "Module", string description = "")
        {
            try
            {
                string path = Path.Combine(_Path, dllFileName);
                var assembly = this.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                Type InstanceType = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Equals(typeName.ToLower()));
                Type ViewModel = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Contains($"{typeName}Model".ToLower()));
                Type IocModule = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Equals(ioc.ToLower()));
                Dll.Add(new DLLInformations
                {
                    InstanceViewModel = ViewModel,
                    InstanceType = InstanceType,
                    IocModule = IocModule,
                    Description = description,
                    IsEnable = true,
                });
            }
            catch (Exception ex)
            {
                XLog.Fatal(ex, $"当前资源不存在：{ex.Message}");
            }
        }

        public void Loads(ComponentObjectModel objectModel)
        {
            try
            {
                string path = Path.Combine(_Path, objectModel.Plugin);
                var assembly = this.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                Type InstanceType = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Equals(objectModel.Bootstrapper.ToLower()));
                Type ViewModel = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Contains($"{objectModel.Bootstrapper}Model".ToLower()));
                Type IocModule = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Equals(objectModel.Ioc.ToLower()));
                Dll.Add(new DLLInformations
                {
                    InstanceViewModel = ViewModel,
                    InstanceType = InstanceType,
                    IocModule = IocModule,
                    Description = objectModel.Description,
                    IsEnable = true,
                    Handle = objectModel.Code
                });
            }
            catch (Exception ex)
            {
                XLog.Fatal(ex, $"当前资源不存在：{ex.Message}");
            }
        }


        public void Load(string dllFileName, string typeName)
        {
            try
            {
                string path = Path.Combine(_Path, dllFileName);
                var assembly = this.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
                Type InstanceType = assembly.GetTypes().FirstOrDefault(t => t.Name.ToLower().Equals(typeName.ToLower()));
                Single.Enqueue(InstanceType);
            }
            catch (Exception ex)
            {
                XLog.Fatal(ex, $"当前资源不存在：{ex.Message}");
            }
        }
    }
}
