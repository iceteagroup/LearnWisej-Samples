# Architecture — one FieldOps codebase and its clients

```mermaid
flowchart LR
  subgraph Server["FieldOps server (ASP.NET Core + app.UseWisej())"]
    APP["FieldOps (Wisej.NET app)<br/>WorkOrdersPage · WorkOrderDetail<br/>WorkOrderService<br/>package: Wisej-4 + <b>Wisej-4-Hybrid</b><br/>Device.Valid gate"]
  end

  subgraph Clients["Clients — no screen copied per client"]
    TAB["Browser tab<br/>Device.Valid = false"]
    PWA["Installed PWA<br/>manifest + sw.js<br/>Device.Valid = false"]
    HC["FieldOps.Client (.NET MAUI)<br/>package: <b>Wisej-4-Hybrid-Native</b><br/>Startup.cs: UseWisejHybrid(config =><br/>config.StartupUrl = server<br/>config.OfflineTimeout = 5000)<br/>WebView2 · WebView · WKWebView"]
  end

  subgraph Device["Inside FieldOps.Client, on the device"]
    OFF["FieldOps.Offline (Local app)<br/>OfflineStartup: EmbedIO WebServer + WithWisej()<br/>registered with UseWisejOffline&lt;OfflineStartup&gt;()"]
  end

  TAB -- "HTTP + WebSocket" --> APP
  PWA -- "HTTP + WebSocket" --> APP
  HC -- "StartupUrl (Remote)" --> APP
  HC -. "after OfflineTimeout without internet" .-> OFF
  APP -- "Wisej.Hybrid.Device calls<br/>(built-in JavaScript bridge)" --> HC
```

| Project | Package | Arrives in | Role |
|---|---|---|---|
| `FieldOps` (web) | `Wisej-4` 4.1.7, `Wisej-4-Hybrid` 4.1.7 | Module 1 (this folder) | All screens, all rules, the `Device` calls. |
| `FieldOps.Client` (MAUI) | `Wisej-4-Hybrid-Native` 4.1.7 | Module 3 | Hosts the app in the platform WebView; implements the device side. |
| `FieldOps.Offline` (Local) | `Wisej-4`, `Wisej-4-Hybrid`, `EmbedIO` | Module 3 | The fallback app on the embedded server, chosen because the decision record picked Remote + Local fallback. |

The joint between them is `Device.Valid`: the web project never branches on which client opened it,
only on whether a Hybrid client is present and what that client can do.
