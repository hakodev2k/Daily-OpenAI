// Isolated lab database; each run restores a deterministic dataset.
const lab = db.getSiblingDB("wind_turbine_inspection_lab");
lab.turbines.deleteMany({});
lab.turbines.insertMany([
  { turbineId: "WT-041", site: "NORTH", inspections: [
    { type: "brake", status: "PASS", ticket: "I-4101" },
    { type: "blade", status: "FAILED", ticket: "I-4102" }
  ] },
  { turbineId: "WT-042", site: "NORTH", inspections: [
    { type: "brake", status: "FAILED", ticket: "I-4201" },
    { type: "blade", status: "PASS", ticket: "I-4202" }
  ] },
  { turbineId: "WT-043", site: "SOUTH", inspections: [
    { type: "brake", status: "PASS", ticket: "I-4301" }
  ] },
  { turbineId: "WT-044", site: "SOUTH", inspections: [
    { type: "brake", status: "FAILED", ticket: "I-4401" }
  ] },
  { turbineId: "WT-045", site: "WEST", inspections: [
    { type: "brake", status: "PASS", ticket: "I-4501" },
    { type: "brake", status: "FAILED", ticket: "I-4502" }
  ] },
  { turbineId: "WT-046", site: "WEST", inspections: [
    { type: "brake", status: "PASS", ticket: "I-4601" },
    { type: "electrical", status: "FAILED", ticket: "I-4602" }
  ] }
]);
print("FIXTURE_READY count=" + lab.turbines.countDocuments({}));
