# Module 4 lab note - the switch, the fallback, and the static field

## The shape of the switch

The combo box handler does one thing:

```csharp
Application.CurrentCulture = CultureInfo.GetCultureInfo(name);
```

and stops. Everything that has to happen next hangs off `Application.CultureChanged`:

```csharp
private void Application_CultureChanged(object sender, EventArgs e)
{
    ApplyTextResources();      // shared resources: captions follow immediately
    UpdateCulturePreview();    // culture-sensitive values: reformatted
    CreateEditor();            // designer resources: only a new instance reads them
    SyncCultureCombo();        // the combo itself, if something else made the change
}
```

The indirection is the point. A handler that switched the culture and then refreshed everything
inline would work from exactly one button. With the event, **every** route into a culture change
gets the same refresh: the `?lang=` URL parameter, the browser's own `Accept-Language`, a saved
user preference, a support tool. This lab proves it with the URL parameter, and the combo box ends
up in step without the combo box handler ever running.

`SyncCultureCombo` guards against re-entry with a flag, because assigning `SelectedItem` raises
`SelectedIndexChanged` again. It also handles a culture the application does not list - a French
browser arriving on `fr-FR` - by adding it rather than leaving the combo blank. That is not an
error condition: the text falls back to the neutral file and the values are still formatted
correctly.

The page unsubscribes in `Dispose`. `Application.CultureChanged` outlives any one page, and a page
that forgets is kept alive by the event for the rest of the session.

## `culture: auto`, and when a fixed culture is right

```jsonc
"culture": "auto"
```

Each session starts from the browser's `Accept-Language`, so a German browser lands on German
without anyone choosing anything, and `?lang=` overrides it for that session.

**A fixed culture is the better choice** when the application serves one market and its values
must be unambiguous for everyone who reads them - an internal back office where every user is in
the same country, or a screen whose numbers are compared across users and have to be formatted
identically. `1.850,75` and `1,850.75` are the same number until two people read them differently
in the same meeting. `auto` is the right default for anything customer-facing.

## Fallback, made visible

`de-AT` is in the picker on purpose, and there is no `Strings.de-AT.resx`. Open
`http://localhost:6104/?lang=de-AT` and the result is worth looking at closely:

| | de-DE | de-AT |
|---|---|---|
| Text | Willkommen bei GlobalDesk | Willkommen bei GlobalDesk |
| Date | Sonntag, 15. März 2026 | Sonntag, 15. März 2026 |
| Number | `12.500` | `12 500` |
| Currency | `1.850,75 €` | `€ 1.850,75` |

Same words, different numbers. The text came from `Strings.de.resx` because .NET tried
`Strings.de-AT` first, found nothing, and fell back to the language; the **formatting** stayed
Austrian, because formatting is `de-AT`'s own business. Austria groups thousands with a space and
puts the euro symbol in front.

That is the whole "language versus formatting" distinction in one screenshot, and it is why the
preview panel shows the parent culture: `de-AT - Deutsch (Österreich) (Greift zurück auf de)`.
Fallback that is visible is fallback you can reason about; fallback that is silent is how a
half-finished translation ships.

## Why the culture must not live in a static field

`Application.CurrentCulture` is **per session**. It belongs to the user in front of one browser
tab, not to the process.

Put it in a `static` field instead:

```csharp
// Wrong, and it will pass every test you write on your own machine.
public static CultureInfo Culture = CultureInfo.GetCultureInfo("en-US");
```

and here is what two users get. Anna in Berlin opens GlobalDesk and picks German; the static field
becomes `de-DE`. Ben in London is already signed in, reading English, and does nothing at all -
and his next click renders in German. He has no idea why. He cannot switch back except by picking
English, which then does the same thing to Anna.

It is worse than it sounds, for three reasons:

1. **It never reproduces in development.** One developer, one browser, one session - the static
   field and the session agree perfectly. It only appears with two concurrent users, which is to
   say in production.
2. **The symptom does not point at the cause.** The report is "the application randomly changes
   language", and nobody connects it to a colleague's click.
3. **It is not only language.** The same field decides how every number and date is formatted, so
   an amount can be misread as a thousand times larger or smaller depending on who last switched.

The same argument applies to anything else that is per user: the selected customer, the theme, an
upload in progress. In a server-side web framework, a `static` mutable field is shared by everyone
who is signed in.

The test that catches it: open the application in two browsers - not two tabs, two browsers or one
private window - and change the language in one. If anything moves in the other, something is
static that should not be.
