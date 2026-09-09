using System.Reflection;

// Tells Wisej.NET that this assembly exports client resources: every embedded
// /Platform/*.js (and *.css) file is bundled into the client and loaded with the
// framework, so the qx class integrationlab.controls.SimpleGaugeControl and the
// VendorGauge library it uses are available before any page renders.
[assembly: Wisej.Core.WisejResources]

[assembly: AssemblyTitle("IntegrationLab")]
[assembly: AssemblyDescription("Application Integration Course — Module 5: Custom Controls & Theming")]
[assembly: AssemblyProduct("IntegrationLab")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
