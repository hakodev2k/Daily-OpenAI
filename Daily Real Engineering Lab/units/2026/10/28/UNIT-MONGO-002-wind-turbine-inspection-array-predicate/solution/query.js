// Reference only: verify.ps1 intentionally reads starter/query.js instead.
globalThis.findAffectedTurbines = function (collection) {
  return collection.find(
    {
      inspections: {
        $elemMatch: { type: "brake", status: "FAILED" }
      }
    },
    { projection: { _id: 0, turbineId: 1 } }
  ).sort({ turbineId: 1 }).toArray().map(row => row.turbineId);
};
