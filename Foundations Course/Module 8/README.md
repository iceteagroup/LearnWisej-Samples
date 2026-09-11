# WisejTrainingApp · Wisej.NET Foundations · Module 8

The JavaScript status widget from **Module 8 · JavaScript Integration with Widget**, as the walkthrough video
builds it: a `Wisej.Web.Widget` (`widStatus`) drawn by `Widgets/statusGauge.js` + `statusGauge.css`, fed only
safe display values from a C# `StatusService`; a native card with the same values; Refresh Server Data and
Set Healthy / Warning / Critical buttons; and an event log.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 8/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5088
```

Then open <http://localhost:5088>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

## What to try

| Action | What you should see |
|---|---|
| **Refresh Server Data** | the gauge and the native labels update together; log *Widget refreshed with server data* |
| **Set Healthy / Warning / Critical** | the gauge recolours (green / amber / red) — C# decides the status, the widget only draws it |
| Click the gauge | the JS fires `gaugeClick`; the log shows *Gauge clicked at N%* |

## Where things live

| File | What it's for |
|---|---|
| `StatusPage.cs` | `StatusPage_Load` (Packages, InitScript, Options, WidgetEvent), the buttons, `UpdateWidget` |
| `Widgets/statusGauge.js` | The widget itself: `init`, `update`, `pulse`, `render` |
| `Widgets/statusGauge.css` | The gauge styles |
| `Services/StatusService.cs` | The data and the status rule — stays in C# |
| `Models/StatusInfo.cs` | The values the page shows |
