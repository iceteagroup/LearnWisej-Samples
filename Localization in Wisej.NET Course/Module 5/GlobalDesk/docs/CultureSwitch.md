# Module 4 lab note - where the session's culture lives, and what a static field would do

## Where it comes from

`"culture": "auto"` in `Default.json`. Each session starts from the browser's `Accept-Language`,
so a German browser lands on German and nobody chose anything. `?lang=de-DE` in the URL overrides
it for that session only, which is how a support engineer reproduces a customer's screen without
touching their own browser settings.

A **fixed** culture is the right choice when the application serves one market and the values must
be unambiguous for everyone reading them - an internal back office where every user is in the same
country, or a screen whose numbers are compared across users. `auto` is the right default for
anything customer-facing.

## Where it lives

`Application.CurrentCulture` is **per session**. It is a property of the user in front of this
browser tab, not of the process.

```csharp
Application.CurrentCulture = CultureInfo.GetCultureInfo(name);
```

That one line is the whole handler. Everything else hangs off the event:

```csharp
Application.CultureChanged += this.Application_CultureChanged;   // in the constructor
Application.CultureChanged -= this.Application_CultureChanged;   // in Dispose
```

A handler that switched the culture *and* refreshed everything inline would work exactly once,
from exactly one control. Behind the event, the `?lang=` parameter, a saved user preference and a
support tool all get the same refresh without knowing the refresh exists.

The `-=` in `Dispose` is not tidiness. `Application.CultureChanged` lives as long as the session,
so every page that was ever open keeps handling culture changes - and touching disposed controls -
until the session ends.

## What a static culture field would do

Suppose the culture were cached:

```csharp
private static CultureInfo culture;   // never do this
```

Two people are signed in. One is in Hamburg, one in Chicago. The Hamburg user picks German. The
Chicago user's next click repaints their screen in German, with German date and currency formats,
in the middle of whatever they were doing.

What makes it expensive is not the bug, it is the report. The Chicago user says "the site
randomly switched to German". There is no correlation to a deploy, no stack trace, no entry in any
log. It never reproduces in development, because development has one developer and one browser,
and with one session a static field and a session property behave identically.

The same applies to anything derived from the culture - a cached `NumberFormatInfo`, a formatted
string held in a static, a `ResourceManager` you pinned to a language "for speed".

## The three things a switch has to repaint

| Kind of text | Where it comes from | What the handler does |
|---|---|---|
| Shared captions | `Strings.resx` via `Texts.Get` | `ApplyTextResources()` - re-reads every key |
| Values | `DateTime`, `decimal` | `UpdateCulturePreview()` - reformats with the new culture |
| Designer captions | `CustomerEditor.resx` | `CreateEditor()` - a **new instance**, because `ApplyResources` ran at construction |

The third is the one that catches people. The walkthrough shows six seconds where the dashboard is
German and the editor panel is still English, and then says never to ship it. The fix is not
cleverness, it is doing all three in the same handler so the half-translated state has no frame to
appear in.

The rebuild's cost is real: whatever the user had typed in the editor is gone. A production screen
saves and restores the values around it, or does not offer the switch while an editor is open.

## fr-CA, and what fallback actually covers

`fr-CA` is in the picker and there is no French resource file. Selecting it gives **English text**
with **Canadian French formatting**:

```
mercredi 23 septembre 2026
1 234 567,89
1 850,75 $
```

Two different mechanisms, and only one of them uses resources. Text falls back `fr-CA` → `fr` →
neutral and stops at the first value it finds. Formatting never consults a resource file at all -
it comes from the culture's own data, so an untranslated language still gets its own separators,
its own currency symbol and its own date order.

That is also why a language-region culture is the sharpest fallback test you can run. `de-AT` would
show German text from `Strings.de.resx` with Austrian formatting: `12 500` and `€ 1.850,75` against
Germany's `12.500` and `1.850,75 €`.
