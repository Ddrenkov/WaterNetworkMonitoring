namespace WaterNetworkMonitoring
{
    internal static class Program
    {
        internal static void ConfigureTraceOutput()
        {
            // The Visual Studio designer's listener displays ordinary traces as dialogs.
            for (int i = System.Diagnostics.Trace.Listeners.Count - 1; i >= 0; i--)
            {
                var listenerType = System.Diagnostics.Trace.Listeners[i].GetType();
                if (listenerType.FullName == "Microsoft.DotNet.DesignTools.Utilities.ServerTraceListener" ||
                    listenerType.Name == "WindowsFormsTraceListener")
                    System.Diagnostics.Trace.Listeners.RemoveAt(i);
            }

            if (!System.Diagnostics.Trace.Listeners.OfType<System.Diagnostics.DefaultTraceListener>().Any())
            {
                System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.DefaultTraceListener
                {
                    AssertUiEnabled = false
                });
            }
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ConfigureTraceOutput();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new LoginForm());
        }
    }
}
