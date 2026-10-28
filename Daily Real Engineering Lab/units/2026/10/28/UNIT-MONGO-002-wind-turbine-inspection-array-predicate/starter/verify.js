load("/lab/starter/fixture.js");
load("/lab/starter/query.js");
const collection = db.getSiblingDB("wind_turbine_inspection_lab").turbines;
function assertIds(label, expected) {
  const actual = globalThis.findAffectedTurbines(collection);
  if (JSON.stringify(actual) !== JSON.stringify(expected)) {
    print("VERIFY_FAILED " + label + " expected=" + JSON.stringify(expected) + " actual=" + JSON.stringify(actual));
    quit(1);
  }
  print("PASS " + label + " " + JSON.stringify(actual));
}
assertIds("baseline", ["WT-042", "WT-044", "WT-045"]);
collection.insertMany([
  { turbineId: "WT-047", site: "EAST", inspections: [
    { type: "brake", status: "PASS", ticket: "I-4701" },
    { type: "blade", status: "FAILED", ticket: "I-4702" }
  ] },
  { turbineId: "WT-048", site: "EAST", inspections: [
    { type: "brake", status: "FAILED", ticket: "I-4801" },
    { type: "blade", status: "PASS", ticket: "I-4802" }
  ] }
]);
assertIds("regression", ["WT-042", "WT-044", "WT-045", "WT-048"]);
print("VERIFY_PASSED learner_query");
quit(0);
