const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));

const latency = new Map([
  ['ann', 80],
  ['anna', 20],
]);

let currentQuery = '';
let rendered = null;

async function fakeSearch(query) {
  await sleep(latency.get(query) ?? 10);
  return { query, items: [`${query}-01`, `${query}-02`] };
}

async function onQueryChanged(query) {
  currentQuery = query;
  console.log(`intent:${query}`);

  const response = await fakeSearch(query);
  console.log(`complete:${response.query}; current:${currentQuery}`);

  // Investigation note: a completed request is not necessarily the current user intent.
  rendered = response;
  console.log(`render:${response.query}`);
}

async function scenario() {
  const first = onQueryChanged('ann');
  await sleep(5);
  const second = onQueryChanged('anna');
  await Promise.all([first, second]);

  console.log(`final-input:${currentQuery}`);
  console.log(`final-render:${rendered?.query}`);
  return { currentQuery, renderedQuery: rendered?.query };
}

const result = await scenario();
const mode = process.argv[2];

if (mode === 'reproduce') {
  process.exit(result.currentQuery === 'anna' && result.renderedQuery === 'ann' ? 0 : 2);
}

if (mode === 'verify') {
  process.exit(result.currentQuery === 'anna' && result.renderedQuery === 'anna' ? 0 : 3);
}