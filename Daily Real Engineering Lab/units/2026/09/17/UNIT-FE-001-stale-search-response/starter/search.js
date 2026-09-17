const delays = { a: 80, ab: 20 };

function fakeSearch(query) {
  console.log(`REQUEST_STARTED ${query}`);
  return new Promise(resolve => {
    setTimeout(() => {
      console.log(`REQUEST_COMPLETED ${query}`);
      resolve([`${query}-result`]);
    }, delays[query]);
  });
}

const state = { query: '', results: [] };

async function onSearchChanged(query) {
  state.query = query;
  const results = await fakeSearch(query);
  state.results = results;
  console.log(`STATE_COMMITTED ${query} ${results[0]}`);
}

await Promise.all([onSearchChanged('a'), onSearchChanged('ab')]);
console.log(`FINAL_QUERY ${state.query}`);
console.log(`FINAL_RESULT ${state.results[0]}`);