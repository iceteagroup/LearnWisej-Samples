# Widget triage table · Module 1

Three candidate widgets scored against the four decision drivers from the lesson, plus the
"cost of getting the depth wrong". The last column is the integration type chosen in
[ADR-001](IntegrationDecisionRecord.md).

| Candidate | What it does | Screens now / in 2 years | Direction of data | Theme + Designer needed | Vendor maintenance | Cost if depth is wrong | **Integration type** |
|---|---|---|---|---|---|---|---|
| **Small gauge** | Displays one reading against warn/alarm bands | 1 / 3 | Server → widget (numbers); one event back | No (dashboard colors are fine) | Low: tiny library, no upgrade pressure | Low: re-wrap in a day | **One-off → Reusable** |
| **Rich text editor** | Edits formatted text entered by users | 3 / 6 | Both ways; **HTML user content returns to the server** | Yes: sits next to built-in inputs | Medium: active vendor, breaking upgrades | Medium: theming and sanitizer must be redone per screen if copied | **Reusable + Themeable** |
| **Data grid** | Data-bound list surface with sorting, paging, selection | 12 / 30+ | Both ways: data pages out, selection/edits back | Yes: the standard list surface of the app | High: large vendor API, yearly breaking changes | **High:** dozens of screens depend on it | **Native `DataGridView` first; native-feeling custom control only if required** |

## How each candidate maps to the vocabulary

| Word | Small gauge | Rich text editor | Data grid |
|---|---|---|---|
| **Component** | `TemperatureGauge` (server object) | `RichEditor` wrapper class | `DataGridView` (built-in) |
| **Control** | yes, has a surface | yes | yes |
| **Widget** | `temperature-gauge.js` adapter around `VendorGauge` | adapter around the editor library | Wisej.NET client grid |
| **State (server)** | `Value`, `Minimum`, `Maximum`, `WarnAt`, `Threshold` | document HTML, read-only flag, toolbar set | rows, columns, selection, sort, page |
| **Local browser state** | needle animation, hover | caret, undo stack, toolbar popups | scroll position, column resize drag |
| **Rendering** | `{"value":104}` | `{"html":"…","readOnly":false}` | data page JSON |
| **Event** | `thresholdExceeded {value}`, `rangeChanged {range,value}` | `changed {html}` (sanitized on the server) | selection / cell edit |

## Container rule check

| Candidate | Vendor wants its own element? | Hosted inside a child of the Wisej.NET container? | Notes |
|---|---|---|---|
| Small gauge | yes, it appends an `<svg>` | yes (`div.temperature-gauge-host`) | resize → `gauge.resize()`, dispose → `gauge.destroy()` then clear host |
| Rich text editor | yes, replaces the target element with its own frame | yes | never hand it the framework root: it would rebuild it |
| Data grid | not applicable for native; yes for a vendor grid | yes if vendor | heavy DOM, virtualized rows: disposal must be exact |

## What was learned from the triage

- Two of the three candidates need no new custom control at all. The triage stops the team
  from "coding blindly" a vendor grid that the framework already provides natively.
- The editor is the only candidate with a **security boundary** (HTML in). That, not its UI,
  drives its integration depth.
- The gauge is the cheapest place to practice the full pipeline, which is why Module 1 builds it.
