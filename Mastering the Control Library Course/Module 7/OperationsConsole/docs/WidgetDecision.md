# Widget decision note

**Module 7 · Custom Widgets, Extensions, Theming, and Capstone — lab task 3**
*"Write a one-paragraph decision note: which native controls you considered for the rating or
indicator, why none fits, and which extension type you are building."*

## The decision

The Operations Console needs a **customer satisfaction rating** on the Widgets screen: five stars,
click or arrow-key to pick a value from 1 to 5, a visible "saved" state once the server has stored
it. I went through the native library first, as the module's decision rules require. `TrackBar`
gives a 1–5 integer with keyboard support but reads as a volume slider, not a judgement, and has no
place to show "this is the value we stored". A `NumericUpDown` or a `ComboBox`/`RadioButton` group
records the same number correctly but turns a one-click gesture into a form field — the control
would be technically fine and the interaction wrong. A `Rating`-style control does not exist in the
library. `ProgressBar` displays a level but is read-only, so it is an indicator, not an input.
A `UserControl` holding five `Button`s with star images would work and would stay entirely native —
that is the honest runner-up — but it costs five images per theme, hover and keyboard handling
written by hand in C#, a server round trip for every hover preview, and it still would not give the
hover-preview animation that makes a star rating feel like a star rating. The gesture (hover
preview, click, keyboard, a purely visual "saved" flourish) is **browser-only work**, which is
exactly what the module reading reserves for JavaScript. So the extension type is a **`Widget`**:
the general-purpose client container for a small third-party-style JavaScript library
(`wwwroot/rating.js`) that lives inside a Wisej.NET container. Not a **Control** — the integration
is one screen in one application, not a first-class component to reuse across products; not a
**Component** — it has a visual surface; not an **Extender Provider** — it does not add a property
to existing controls the way `ToolTip` and `ErrorProvider` do; not an **Icon Pack** — nothing is
being packaged for the designer. And crucially: `Widget` keeps the parts that matter on the server.
The rule "a rating is a whole number from 1 to 5" lives in `Services/RatingService.cs`, the payload
is validated on arrival, the value is stored server-side, and the colours come from a packaged
stylesheet so `Application.LoadTheme` still reaches the widget.

## Where the alternatives are recorded

| Considered | Why it was not used |
|---|---|
| `TrackBar` | Correct value type and keyboard support, but the affordance is "adjust a level", not "rate this customer"; no saved-state affordance. |
| `NumericUpDown` | Turns a one-click gesture into typing. Right data, wrong interaction. |
| `ComboBox` / `RadioButton` group | Same objection, plus five labels of chrome for one value. |
| `ProgressBar` | Read-only — an indicator, not an input. |
| `UserControl` with five star `Button`s | The closest native answer and a legitimate choice. Rejected for the hover-preview round trips and five themed images per theme; noted here so the next developer knows it was weighed, not overlooked. |
| **`Widget` + `rating.js` + `rating.css`** | **Chosen.** Browser-only rendering and gestures in JavaScript, every rule and all state on the server, theming through packaged CSS, the contract documented in `WidgetContract.md`. |

## Evidence (what the running app shows)

- The Widgets section shows a live five-star control; hovering previews, clicking commits — none of
  which produces a server round trip until the click.
- The Event log records exactly one message per click in each direction:
  `← JS→.NET ratingChanged {"value":4}` then `→ .NET→JS setSaved 4`.
- **Send malformed payload** and **Send out-of-range payload** prove the rule is not in the
  JavaScript: the client can be made to send `"seven"` or `9`, and the server still refuses.
- **Theme → Material-3** restyles the console *and* the stars, which is the test of "no colours
  hard-coded in JavaScript".
