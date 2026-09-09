using System.Collections.Specialized;

namespace IntegrationLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// </summary>
        static void Main(NameValueCollection args)
        {
            new Window1().Show();
        }
    }
}
