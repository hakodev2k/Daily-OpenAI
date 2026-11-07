load("/lab/starter/fixture.js");
load("/lab/starter/query.js");
const collection = db.getSiblingDB("wind_turbine_inspection_lab").turbines;
const actual = globalThis.findAffectedTurbines(collection);
const originalSymptom = ["WT-041", "WT-042", "WT-044", "WT-045", "WT-046"];
if (JSON.stringify(actual) !== JSON.stringify(originalSymptom)) {
  print("REPRODUCTION_NOT_CONFIRMED actual=" + JSON.stringify(actual));
  quit(1);
}
print("REPRODUCTION_CONFIRMED actual=" + JSON.stringify(actual) + " operatorConfirmed=3");
quit(0);
