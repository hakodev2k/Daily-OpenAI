var orderCount = 1;
var linesPerOrder = 40;
var adjustmentsPerOrder = 25;

var logicalEntities = orderCount + linesPerOrder + adjustmentsPerOrder;
var joinedRows = orderCount * linesPerOrder * adjustmentsPerOrder;

Console.WriteLine($"Orders={orderCount}");
Console.WriteLine($"Lines={linesPerOrder}");
Console.WriteLine($"Adjustments={adjustmentsPerOrder}");
Console.WriteLine($"LogicalEntities={logicalEntities}");
Console.WriteLine($"RowsProcessed={joinedRows}");
Console.WriteLine($"BusinessResult=Lines:{linesPerOrder};Adjustments:{adjustmentsPerOrder}");
