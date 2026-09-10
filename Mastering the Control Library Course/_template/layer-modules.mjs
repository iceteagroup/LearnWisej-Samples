// Layer the Mastering the Control Library samples cumulatively:
//   Module N = Module 1 + own changes of Module 2 .. Module N (each module's own changes win over earlier ones).
// Usage: node layer-modules.mjs [--dry]
import fs from "node:fs";
import path from "node:path";

const ROOT = "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course";
const DRY = process.argv.includes("--dry");
const SKIP_DIRS = new Set(["bin", "obj", ".vs"]);
const PER_MODULE = new Set(["README.md", "OperationsConsole/Properties/launchSettings.json"]); // never layered

function walk(dir, base = dir, out = []) {
  for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
    if (SKIP_DIRS.has(e.name)) continue;
    const p = path.join(dir, e.name);
    if (e.isDirectory()) walk(p, base, out);
    else out.push(path.relative(base, p).replace(/\\/g, "/"));
  }
  return out;
}
const same = (a, b) => fs.existsSync(a) && fs.existsSync(b) && fs.readFileSync(a).equals(fs.readFileSync(b));

const mod = (n) => path.join(ROOT, `Module ${n}`);
const base = mod(1);

// 1. snapshot each module's OWN changes relative to Module 1 (before touching anything)
const own = {};
for (let n = 2; n <= 7; n++) {
  const files = walk(mod(n));
  own[n] = files.filter((f) => !PER_MODULE.has(f) && !same(path.join(mod(n), f), path.join(base, f)));
  console.log(`Module ${n}: ${own[n].length} own files`);
}

// 2. layer
const conflicts = [];
for (let n = 3; n <= 7; n++) {
  const ownSet = new Set(own[n]);
  for (let k = 2; k < n; k++) {
    for (const f of own[k]) {
      if (ownSet.has(f)) { conflicts.push(`Module ${n}: ${f} changed by both Module ${k} and Module ${n} (kept Module ${n}'s)`); continue; }
      // a later module (k2 in (k, n)) may also own it: the loop order means the highest k wins, report it
      const later = [];
      for (let k2 = k + 1; k2 < n; k2++) if (own[k2].includes(f)) later.push(k2);
      if (later.length) conflicts.push(`Module ${n}: ${f} changed by Module ${k} and Module ${later.join("/")} (Module ${later[later.length - 1]}'s wins)`);
      const src = path.join(mod(k), f), dst = path.join(mod(n), f);
      if (DRY) { console.log(`  [dry] ${k} -> ${n}: ${f}`); continue; }
      fs.mkdirSync(path.dirname(dst), { recursive: true });
      fs.copyFileSync(src, dst);
    }
  }
}
console.log("\nConflicts / notes:");
for (const c of conflicts) console.log("  " + c);
if (!conflicts.length) console.log("  none");
