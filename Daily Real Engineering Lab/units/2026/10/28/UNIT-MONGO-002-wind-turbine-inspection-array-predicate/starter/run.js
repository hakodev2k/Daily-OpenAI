load("/lab/starter/fixture.js");
load("/lab/starter/query.js");
const collection = db.getSiblingDB("wind_turbine_inspection_lab").turbines;
const actual = globalThis.findAffectedTurbines(collection);
print("DISPATCH_CANDIDATES=" + JSON.stringify(actual));
print("OPERATOR_CONFIRMED_COUNT=3");
for (const row of collection.find({}, { projection: { _id: 0, turbineId: 1, inspections: 1 } }).sort({ turbineId: 1 }).toArray()) {
  print("INSPECTION=" + JSON.stringify(row));
}
