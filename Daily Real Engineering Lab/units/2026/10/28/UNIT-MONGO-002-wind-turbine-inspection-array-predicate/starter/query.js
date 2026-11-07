// Learner-editable production query: return sorted turbine IDs requiring brake repair.
globalThis.findAffectedTurbines = function (collection) {
  return collection.find(
    {
      "inspections.type": "brake",
      "inspections.status": "FAILED"
    },
    { projection: { _id: 0, turbineId: 1 } }
  ).sort({ turbineId: 1 }).toArray().map(row => row.turbineId);
};
