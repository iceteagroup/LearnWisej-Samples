using Wisej.Web;

namespace FieldOps
{
    /// <summary>Typed access to the few values this session keeps.</summary>
    internal static class Session
    {
        public static Model.Technician Technician
        {
            get => (Model.Technician)Application.Session["Technician"];
            set => Application.Session["Technician"] = value;
        }

        public static WorkOrdersPage ListPage
        {
            get => (WorkOrdersPage)Application.Session["ListPage"];
            set => Application.Session["ListPage"] = value;
        }
    }

    /// <summary>
    /// Which build of FieldOps is running. The FieldOps web project leaves it false; the offline
    /// project (FieldOps.Offline, the Local app inside the Hybrid client) sets it in its
    /// Program.Main. The screens are the same source files in both.
    /// </summary>
    public static class AppMode
    {
        public static bool IsLocal { get; set; }
    }
}
