const events = [];
const log = x => { events.push(x); console.log(x); };

function save() {
  log('SAVE');
  return Promise.resolve();
}

function followUp() {
  log('FOLLOW_UP');
  return Promise.reject(new Error('follow-up failed'));
}

function runOperation() {
  return save()
    .then(() => {
      followUp().catch(() => log('LATE_ERROR'));
    })
    .then(() => log('SUCCESS'))
    .catch(() => log('ERROR'));
}

runOperation().then(() => {
  setTimeout(() => {
    console.log('EVENTS=' + events.join(','));
    process.exit(events.includes('SUCCESS') ? 2 : 0);
  }, 10);
});
