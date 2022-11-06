using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RDP_Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new Mutex(true, "RDP-Helper");

            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("程序已经在运行!", "提示");
                this.Shutdown();
            }
        }


        //private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        //{
        //    var executingAssembly = Assembly.GetExecutingAssembly();
        //    var assemblyName = new AssemblyName(args.Name);

        //    var path = assemblyName.Name + ".dll";
        //    if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
        //        path = $@"{assemblyName.CultureInfo}\{path}";

        //    using (Stream stream = executingAssembly.GetManifestResourceStream(path))
        //    {
        //        if (stream == null) return null;

        //        var assemblyRawBytes = new byte[stream.Length];
        //        stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
        //        return Assembly.Load(assemblyRawBytes);
        //    }
        //}
    }
}
