# IconDesk.Icons - how an icon enters the pack, and how an application overrides one

This file travels with the pack, not with the application, because it is the pack's contract.

## What the pack is

A plain class library. Twelve SVGs under `Resources/`, all `EmbeddedResource`, plus `AppIcons.cs`:
a static catalog of `resource.wx` strings. No Wisej dependency, so it can be referenced from any
application and its catalog can be unit-tested without a session.

```
IconDesk.Icons.dll
  IconDesk.Icons.Resources.add.svg        → resource.wx/IconDesk.Icons/add.svg
  IconDesk.Icons.Resources.delete.svg     → resource.wx/IconDesk.Icons/delete.svg
  ...
```

The URL is the **assembly** name, then the file. The build strips `<RootNamespace>.Resources.` off
the manifest name and nothing else.

## SVG cleanup: what an icon has to look like before it goes in

Two rules, and the second one is the one that costs an afternoon if you get it wrong.

**1. No `width` and `height` on the root element, only `viewBox`.**
An SVG with `width="24" height="24"` renders at 24 pixels and ignores the control it is in. Strip
them and keep `viewBox="0 0 24 24"`, and the same file is crisp at 16 and at 48.

**2. Paint with `currentColor`, never with a literal.**
Wisej.NET recolours an SVG by rewriting the root element:

```xml
<!-- what the browser actually receives, with ?color=highlight -->
<svg viewBox="0 0 24 24" fill="#298AE5" style="color: rgb(41,138,229); fill: rgb(41,138,229);">
  <path fill="none" stroke="currentColor" stroke-width="1.8" ... />
</svg>
```

It sets `fill` and `color` on the root. It does **not** rewrite `stroke` attributes further down -
verified in Module 3, where a white stroke survived untouched while the fill beside it was
replaced. So:

- A **filled** icon works if its shapes have no `fill` of their own, or a single one.
- A **line** icon - which most of these twelve are - only recolours if it strokes with
  `stroke="currentColor"`. A literal `stroke="#5A6B7D"` renders correctly, accepts a `?color=`
  suffix without complaint, and simply never changes colour. Nothing warns you.

That was the one real cleanup this pack needed: the twelve icons were drawn with a literal stroke
and had to be switched to `currentColor` before the suffix did anything. Press **Recolour three
catalog icons** in the lab to see it working, and note that the other nine do not move.

Also worth doing on the way in: drop the XML prolog and any editor metadata, drop `<title>` unless
it is meaningful, and keep the stroke width consistent across the set (1.8 here) so icons do not
look like they came from different families.

## Naming

- Lower-case file name, one concept, no size or colour in the name: `delete.svg`, not
  `delete-red-24.svg`. Colour is a suffix at the call site and size is the control's business.
- Name the **concept**, not the picture. `delete.svg`, not `rubbish-bin.svg` - the artwork may be
  redrawn, the meaning will not.
- The catalog constant is the file name in PascalCase: `delete.svg` → `AppIcons.Delete`. That
  mapping is mechanical on purpose, so the drift check can compare the two lists by name.

## Adding an icon

1. Clean the SVG against the two rules above and drop it in `Resources/`.
2. Add a constant to `AppIcons` and an entry to `AppIcons.All`.
3. Run the lab's **Check the catalog against the assembly** - it compares the embedded resource
   names with the catalog and names anything on one side only. In a real project this is the unit
   test that guards the pack.

Never add an icon to an application by copying the file. If it belongs to the product, it belongs
in the pack.

## Versioning

- **Patch** - artwork redrawn, same concept, same name. Callers do nothing.
- **Minor** - icons added. Callers do nothing.
- **Major** - an icon removed or renamed. That is a breaking change: a removed constant is a
  compile error in every consuming application, which is exactly the behaviour you want and the
  reason callers use the catalog rather than typing paths.

Never silently repoint a name at different artwork across a major version. Add the new name,
mark the old constant `[Obsolete]` with the replacement in the message, and remove it a version
later.

## How an application overrides one icon

It cannot, and that is deliberate. Every catalog constant is **qualified** with the assembly name,
so `resource.wx/IconDesk.Icons/delete.svg` always resolves to the shipped artwork and no file on a
deployment's disk can replace it (Module 5's note has the evidence).

An application that genuinely needs its own version of one icon has two honest routes:

- **Assign a different image source at the call site.** The pack is a default, not a mandate;
  nothing stops a screen from pointing at its own asset.
- **Add a deliberately unqualified alias** to the application, not to the pack:
  `resource.wx/delete.svg` embedded in the application assembly, which a file beside the
  executable can then override. Do this only for assets a deployment is *meant* to replace - a
  customer logo, a site banner - and write down which they are.

What not to do is take the qualification off a pack constant to make one customer's override work.
That makes every application's copy of that icon silently replaceable, and the next person to
debug a wrong icon in production will have no idea why.
