using System.Reflection;

// Tells Wisej.NET that this assembly exports client resources: every embedded /Platform/*.js file
// (here Platform/ticketops.interop.js) is bundled into the client and loaded with the framework, so
// window.ticketOps exists before any widget renders. The client code is versioned and deployed with the
// app — not a loose file a teammate can forget to copy.
[assembly: Wisej.Core.WisejResources]

[assembly: AssemblyTitle("TicketOps")]
[assembly: AssemblyDescription("Production Architecture Deep Dive — Module 9: JavaScript Integration, Widget Interop & Client-Side Enhancements")]
[assembly: AssemblyProduct("TicketOps Console")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
