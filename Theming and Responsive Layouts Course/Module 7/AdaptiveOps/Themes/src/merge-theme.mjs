// Regenerates ../AdaptiveOps.theme = Bootstrap-4 (the embedded base theme, dumped from Wisej.Framework.dll)
// deep-merged with AdaptiveOps.overrides.json, so the shipped theme is complete even when the runtime does
// not merge a top-level "inherit" itself. Objects merge key by key (our value wins), arrays and scalars are
// replaced. The "inherit" key is kept for runtimes that merge base themes.
//
//   node merge-theme.mjs <path-to-Bootstrap-4.theme>
import { readFileSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const here = dirname(fileURLToPath(import.meta.url));
const basePath = process.argv[2];
if (!basePath) {
  console.error('usage: node merge-theme.mjs <Bootstrap-4.theme>');
  process.exit(1);
}

const base = JSON.parse(readFileSync(basePath, 'utf8'));
const over = JSON.parse(readFileSync(join(here, 'AdaptiveOps.overrides.json'), 'utf8'));

const isObj = (v) => v && typeof v === 'object' && !Array.isArray(v);
const merge = (a, b) => {
  if (!isObj(a) || !isObj(b)) return b;
  const out = { ...a };
  for (const k of Object.keys(b)) out[k] = k in a ? merge(a[k], b[k]) : b[k];
  return out;
};

const merged = merge(base, over);
merged.name = over.name;
merged.inherit = over.inherit;
writeFileSync(join(here, '..', 'AdaptiveOps.theme'), JSON.stringify(merged, null, 1) + '\n');
console.log(
  'wrote AdaptiveOps.theme:',
  Object.keys(merged.appearances).length, 'appearances,',
  Object.keys(merged.colors).length, 'colors,',
  Object.keys(merged.fonts).length, 'fonts'
);
