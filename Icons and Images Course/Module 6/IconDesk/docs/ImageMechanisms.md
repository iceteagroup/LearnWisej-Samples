# Module 1 lab note - which mechanism carries which icon

Written from the running application, not from the documentation. Every behaviour below was read
back through `IImage` on the IconDesk command panel.

## The four buttons

| Control | Mechanism | What crosses the network |
|---|---|---|
| `btnOpen` | `Image` | `Images/open.png` is read into a `System.Drawing.Image` on the server. The browser never sees that file: Wisej.NET sends the decoded image as PNG data under a `component.wx` URL. |
| `btnSave` | `ImageSource` | The string `"icon-save"`. Nothing is decoded on the server; the client resolves the name against the theme it has already downloaded, and the picture arrives as an inline `data:image/svg+xml` URL. |
| `btnPrint` | `ImageList` + `ImageKey` | The shared collection holds the picture; the button holds the key `"print"`. |
| `btnDelete` | `ImageList` + `ImageKey` | Same collection, key `"delete"`. |

`lblDiagnostics` prints one line per button, and each line is produced by reading that control
back - not by remembering what was assigned to it.

## What happened when the key was replaced

The second press of **Delete** assigns a different `System.Drawing.Image` to the entry stored
under `"delete"`:

```csharp
var entry = this.imagesCommands.Images["delete"];
entry.Image = LoadServerImage("delete-alt.png");
```

No image property on any control is assigned, and the button follows, because it holds the key and
not the picture. That indirection is the whole reason to prefer `ImageKey` over a direct `Image`
assignment when more than one control shows the same concept: one edit, every user of the key.

The string indexer is read-only - `Images["delete"] = image` does not compile. It hands back the
live `ImageListEntry`, and assigning that entry's `Image` is how a key is repointed.

`ImageKey` was chosen over `ImageIndex` for the usual reason: `"delete"` still means delete after
somebody inserts an image above it in the collection, and `3` does not.

### The part the documentation does not mention

Repointing the entry changes the picture **on the server only**. Wisej.NET serves a control's
picture from a `component.wx` URL stamped with that control's own version, and the version moves
when one of the control's image properties is written - not when the list behind it changes. A
browser holding the old URL therefore keeps showing the old picture. Neither
`((IWisejComponent)list).UpdateWidget()` nor `control.Update()` moves it; `ImageList.UpdateControls`
is not public.

`RepaintControlsOn` handles it honestly: it walks the page, finds every `IImage` whose `ImageList`
is that list and whose `ImageKey` is that key, and writes the **same key** back. Nothing about
what the control points at changes; only its version does. The search is also the point of the
exercise - the code does not know which controls use the key, it asks.

## What happened when a second image property was set on one button

The third press of **Delete** assigns `ImageSource` to a button that already had `ImageList` and
`ImageKey`, and then reads the button back:

```csharp
var keyBefore = this.btnDelete.ImageKey;   // "delete"
this.btnDelete.ImageSource = "icon-close";
// this.btnDelete.ImageKey is now empty
```

Wisej.NET does not keep both and does not merge them. Assigning the image source **cleared** the
key. They feed one visual slot, so the later assignment does not win an argument - it ends one,
and the earlier property is gone by the time anything reads it back.

That is worth knowing because of how the bug usually presents. Nobody writes both lines together;
one is in the designer and the other in a helper method that runs later, and the reported symptom
is "the icon disappeared after we added the theme support". `lblDiagnostics` finds it in one
click, because it reports the properties rather than guessing from the picture.

## One more thing the diagnostics had to learn

A control that takes its picture from an `ImageList` also returns that picture from `Image`. A
naive report that tests `Image` first therefore calls every keyed control an image-object control.
`DescribeIcon` asks about `ImageSource` first (because it displaces everything else) and about the
list before `Image`, and says so in a comment - the test order is part of the answer, not an
implementation detail.

## Names that are not what the lesson writes

- The lesson's conflict example writes `btnDelete.ImageSource = "icon-delete"`. Bootstrap-4 has no
  theme image of that name, so the button would simply go blank and the browser would log a
  failed request. The sample assigns `"icon-close"`, which the theme really defines. The
  mechanism, and everything the diagnostics card says about it, is unchanged.
- `"icon-save"` does exist in Bootstrap-4 and is used as the lesson writes it. The theme draws it
  in the control's `ForeColor`, so it is the same slate grey as the button caption rather than a
  colour this application chose - which is the point of a theme image.
