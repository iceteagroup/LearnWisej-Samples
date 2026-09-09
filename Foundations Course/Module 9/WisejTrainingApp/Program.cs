using System.Collections.Specialized;

namespace WisejTrainingApp
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// </summary>
        static void Main(NameValueCollection args)
        {
            new ReleaseReviewWindow().Show();
        }
    }
}
