IF OBJECT_ID('SyncBalance', 'P') IS NULL
BEGIN
    EXEC('
        CREATE PROCEDURE SyncBalance
        AS
        BEGIN
            SET NOCOUNT ON;

            DECLARE @ReceiptTable TABLE(ResourceId UNIQUEIDENTIFIER, UnitId UNIQUEIDENTIFIER, Quantity DECIMAL(18,2));
            DECLARE @ShipmentTable TABLE(ResourceId UNIQUEIDENTIFIER, UnitId UNIQUEIDENTIFIER, Quantity DECIMAL(18,2));

            INSERT INTO @ReceiptTable(ResourceId, UnitId, Quantity)
            SELECT 
                ResourceId,
                UnitId,
                SUM(Quantity)
            FROM ReceiptItems RI
            INNER JOIN Receipts R ON R.Id = RI.ReceiptId
            WHERE R.IsDeleted = 0
            GROUP BY ResourceId, UnitId;

            INSERT INTO @ShipmentTable(ResourceId, UnitId, Quantity)
            SELECT 
                ResourceId,
                UnitId,
                SUM(Quantity)
            FROM shipmentItems SI
            INNER JOIN shipment S ON S.Id = SI.shipmentId
            WHERE S.IsDeleted = 0 AND S.IsSigned = 1
            GROUP BY ResourceId, UnitId;

            MERGE balances AS Target
            USING (
                SELECT 
                    COALESCE(R.ResourceId, S.ResourceId) AS ResourceId,
                    COALESCE(R.UnitId, S.UnitId) AS UnitId,
                    ISNULL(R.Quantity, 0) - ISNULL(S.Quantity, 0) AS FinalQuantity
                FROM @ReceiptTable R
                FULL OUTER JOIN @ShipmentTable S
                    ON R.ResourceId = S.ResourceId AND R.UnitId = S.UnitId
            ) AS Source
            ON Target.ResourceId = Source.ResourceId AND Target.UnitOfMeasurementId = Source.UnitId

            WHEN MATCHED THEN
                UPDATE SET Quantity = Source.FinalQuantity

            WHEN NOT MATCHED BY TARGET THEN
                INSERT (Id, ResourceId, UnitOfMeasurementId, Quantity)
                VALUES (NEWID(), Source.ResourceId, Source.UnitId, Source.FinalQuantity);
        END
    ')
END
