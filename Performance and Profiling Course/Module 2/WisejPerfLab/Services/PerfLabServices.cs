using System;
using Microsoft.Extensions.DependencyInjection;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// The service provider built in <c>Startup.cs</c>, kept here so the tab pages
    /// (<c>UserControl</c>s created by <c>MainPage</c>) can ask for the same singletons.
    /// </summary>
    /// <remarks>
    /// Wisej.NET injects <c>[Inject]</c> properties on <c>Page</c> and <c>Form</c> through the provider
    /// registered with <c>Application.Services.AddService&lt;IServiceProvider&gt;(app.Services)</c>;
    /// <c>MainPage</c> uses that. A <c>UserControl</c> is not a top-level container, so the tab pages take
    /// what they need from here instead. Everything resolved is a singleton or a transient: Wisej.NET asks
    /// the <b>root</b> provider, and a scoped registration would throw at construction.
    /// </remarks>
    public static class PerfLabServices
    {
        public static IServiceProvider Provider { get; set; }

        public static T Get<T>() => Provider is null ? default : Provider.GetService<T>();
    }
}
