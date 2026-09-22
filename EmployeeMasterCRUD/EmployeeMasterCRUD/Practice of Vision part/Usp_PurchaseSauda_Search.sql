/*
========================================================================================================================================================
Author                  : Yalung Kiranti
Created On              : 01/06/2025
Purpose                 : Stored Procedure for searching and selecting data 
                          related to PurchaseReturn.

Last Modified By        : [Name of Modifier]
Last Modified On        : [Date of Modification]
Modification Details    : [Description of the changes made]

========================================================================================================================================================
*/


CREATE PROC [Purchase].[Usp_PurchaseSauda_Search]
(
--************************************************ Begin  Parameter ******************************************************************************************
     @ID INT = 0
	,@IsSelect BIT = 0 
	,@FYID tinyint
	,@UnitID smallint
	,@NumIDs NVARCHAR(MAX)=''
    ,@PageID smallint = 2211 
--************************************************ Navigation & Order By Parameter ***************************************************************************
	,@NavigationType CHAR(1) =''
	,@NavigationValue NVARCHAR(256) =NULL
--************************************************ End  Parameter ********************************************************************************************
) 
WITH ENCRYPTION
AS
IF @IsSelect  = 1
BEGIN
	IF(@NavigationType <>'')
	BEGIN
	IF		    @NavigationType ='F' SELECT @ID = ID  FROM [Purchase].[TBL_PurchaseReturnMaster] WHERE VNo = ( SELECT  TOP 1 VNo FROM [Purchase].[TBL_PurchaseReturnMaster] [M] WHERE (ISNULL([M].[NumID],0) IN(SELECT value  FROM STRING_SPLIT(@NumIDs,',')) OR @NumIDs='')  ORDER BY VNo )
		ELSE IF @NavigationType ='N' SELECT @ID = ID  FROM [Purchase].[TBL_PurchaseReturnMaster] WHERE VNo = ( SELECT  TOP 1 VNo FROM [Purchase].[TBL_PurchaseReturnMaster] [M] WHERE [M].[VNo] > @NavigationValue AND (ISNULL([M].[NumID],0) IN(SELECT value  FROM STRING_SPLIT(@NumIDs,',')) OR @NumIDs='')  ORDER BY VNo )
		ELSE IF @NavigationType ='P' SELECT @ID = ID  FROM [Purchase].[TBL_PurchaseReturnMaster] WHERE VNo = ( SELECT  TOP 1 VNo FROM [Purchase].[TBL_PurchaseReturnMaster] [M] WHERE [M].[VNo] < @NavigationValue AND (ISNULL([M].[NumID],0) IN(SELECT value  FROM STRING_SPLIT(@NumIDs,',')) OR @NumIDs='')  ORDER BY VNo DESC)
		ELSE IF @NavigationType ='L' SELECT @ID = ID  FROM [Purchase].[TBL_PurchaseReturnMaster] WHERE VNo = ( SELECT  TOP 1 VNo FROM [Purchase].[TBL_PurchaseReturnMaster] [M] WHERE (ISNULL([M].[NumID],0) IN(SELECT value  FROM STRING_SPLIT(@NumIDs,',')) OR @NumIDs='')  ORDER BY VNo DESC )
	END

    --Master
	SELECT 
          [ID],[VNo],[VDate],[EffDate],[PartyVno],[PartyDate],[RefVno],[RefDate],[DueDays],[DueDate],[BillID],[IsCash],
          [BillingCPerson],[BillingName],[BillingAdress],[BillingAdress2],[BillingCountryID],[BillingPhone],[BillingMobile],[BillingMobile2],[BillingEmail],[BillingVatPanNo],
          [ShippingCPerson],[ShippingAdress],[ShippingAdress2],[ShippingCountryID],[ShippingPhone],[ShippingMobile],[ShippingMobile2],[ShippingEmail],
          [GodownID],[LCmasterID],[GLID],[AccountGLID],[PartyBranchID],[SLID],[AGID],[ARID],
          [CLS1ID],[CLS2ID],[CLS3ID],[CLS4ID],[GoodsGLID],[GoodsSLID],[IsNotLocal],[CurrencyID],[CurRate],[DcurRate],
          [BasicAmt],[TAmt],[NetAmt], [INotes],[Remarks],[TrackingNo],[PrintNo],[NumID]       
	FROM [Purchase].[TBL_PurchaseReturnMaster] AS M
	WHERE ID = @ID 

    --TBL_PurchaseReturnOtherInfo
    IF EXISTS (SELECT 1 FROM [Purchase].[TBL_PurchaseReturnOtherInfo] WHERE ID = @ID)
    BEGIN
   SELECT
         [ID],[GateEntryNo],[GateEntryDate],[NetWeight],[TareQty],[GrossWeight]
   FROM [Purchase].[TBL_PurchaseReturnOtherInfo] [P]
   WHERE [P].[ID] = @ID
   END
      ELSE
      BEGIN
            SELECT 
                @ID AS [ID], NULL AS [GateEntryNo], NULL AS [GateEntryDate], 
                NULL AS [NetWeight], NULL AS [TareQty], NULL AS [GrossWeight]
        END

	--- TBL_PurchaseReturnDetails
	SELECT
        [ID],[MasterID],[Sno],[PID],[BrandID],[GodownID],[AltStockQty],[AltQty],[AltUOMID],[StockQty],[Qty],[UOMID],[FreeAltStockQty],[FreeAltQty],[FreeAltUOMID],[FreeStockQty],[FreeQty],
        [FreeUOMID],[ExtraFreeStockQty],[ExtraFreeQty],[Rate],[RateBasisID],[BasicAmt],[TAmt],[NetAmt],[GrossQty],[TareQty],[BillID],
        [PCSOnly],[FreePCSOnly],[GoodsGLID],[GoodsSLID],
        [Weight],[WastagePercentage],[Wastage],[MetalAmt],[StoneQty],[StoneAmt],[Descriptions],[TrackingNo]
	FROM [Purchase].[TBL_PurchaseReturnDetails] [P]
	WHERE [P].[MasterID] = @ID 


    --TBL_PurchaseReturnStoneDetails
    SELECT 
         [PBSD].[ID],[PBSD].[Sno] AS [DetailSno],[PBSD].[Sno],[PBSD].[PID],[PBSD].[GodownID],
         [PBSD].[Qty],[PBSD].[UOMID],[PBSD].[PCS],[PBSD].[Rate],[PBSD].[Amount],[PBSD].[Descriptions],[PBSD].[TrackingNo]
    FROM [Purchase].[TBL_PurchaseReturnStoneDetails] [PBSD]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnDetails] [D] ON [PBSD].[DetailID]=[D].[ID]
    WHERE [PBSD].[MasterID] = @ID

    --TBL_PurchaseReturnFreeKitDetails
    SELECT
         [PBFKD].[ID],[PBFKD].[DetailSno],[PBFKD].[TypeID],[PBFKD].[Sno],[PBFKD].[PID],[PBFKD].[AltStockQty],[PBFKD].[AltQty],[PBFKD].[AltUOMID],
         [PBFKD].[StockQty],[PBFKD].[Qty],[PBFKD].[UOMID],[PBFKD].[Rate],[PBFKD].[RateBasisID],[PBFKD].[BasicAmt],[PBFKD].[GrossQty],[PBFKD].[TareQty],
         [PBFKD].[PCSOnly],[PBFKD].[Descriptions],[PBFKD].[TrackingNo]
    FROM [Purchase].[TBL_PurchaseReturnFreeKitDetails] [PBFKD]
    WHERE [PBFKD].[MasterID]= @ID
           --TBL_PurchaseReturnTerms
   SELECT 
         [PRT].[ID],[D].[Sno] AS [DetailSno],[PRT].[BTID],[PRT].[TypeID],[BT].[PBSign]  AS [Sign],
         [PRT].[Formulation],[PRT].[CalculableAmount],[PRT].[Rate],
		 (CASE WHEN [BT].[TaxTypeID] = 862 THEN [PRT].[Amt] else   ABS([PRT].[Amt])  END) AS [Amt],
		 [PRT].[IsFixed]
    FROM [Purchase].[TBL_PurchaseReturnTerms] [PRT]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnDetails] [D] ON [PRT].[DetailID]=[D].[ID]
	INNER JOIN GM.Tbl_BillingTerm AS [BT] ON [PRT].[BTID] =[BT].[ID]
    WHERE [PRT].[MasterID] = @ID AND [PRT].[TypeID]<>101

    --TBL_PurchaseReturnBatch
   SELECT
         [PBB].[ID],[PBB].[DetailID],[D].[Sno] AS [DetailSno],[PBB].[FreeKitDetailID],[PBFKD].[Sno] AS [FreeKitDetailSno],[PBFKD].[TypeID] AS [TypeID],
         [PBB].[BatchID],[BM].[Batch],[BM].[Barcode],[BM].[MFGDate],[BM].[EXPDate],
	     [BMA].[MRRate],[BMA].[B1],[BMA].[B2],[BMA].[B3],[BMA].[B4],[BMA].[B5],[BMA].[B6],[BMA].[B7],[BMA].[B8],
	     [BMA].[SheetQty],
	     [BMA].[VendorBatchNumber],[BMA].[VendorGrossQty],[BMA].[VendorTareQty],[BMA].[VendorNetQty], 

         [PBB].[Sno],[PBB].[PID],
         [PBB].[AltStockQty],[PBB].[AltQty],[PBB].[AltUOMID],[PBB].[StockQty],[PBB].[Qty],[PBB].[UOMID],[PBB].[FreeAltStockQty],
         [PBB].[FreeAltQty],[PBB].[FreeAltUOMID],[PBB].[FreeStockQty],[PBB].[FreeQty],[PBB].[FreeUOMID],
         [PBB].[ExtraFreeStockQty],[PBB].[ExtraFreeQty],[PBB].[Rate],[PBB].[BasicAmt],
         [PBB].[GrossQty],[PBB].[TareQty],[PBB].[PCSOnly],[PBB].[FreePCSOnly],
         [PBB].[Descriptions],[PBB].[TrackingNo]
    FROM [Purchase].[TBL_PurchaseReturnBatch] [PBB]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnDetails] [D] ON [PBB].[DetailID]=[D].[ID]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnFreeKitDetails] [PBFKD] ON [PBB].[FreeKitDetailID]=[PBFKD].[ID]
    LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BM] ON [PBB].[BatchID]=[BM].[ID]
    LEFT OUTER JOIN [GM].[TBL_BatchMasterAdd] [BMA] ON [PBB].[BatchID]=[BMA].[ID]
    WHERE [PBB].[MasterID] = @ID

   

      --TBL_PurchaseReturnSerial
    SELECT
          [PBS].[ID],[D].[Sno] AS [DetailSno],
          [PBS].[BatchDetailID],[PBB].[Sno] AS [BatchDetailSno],
          [PBS].[FreeKitDetailID],[PBFKD].[Sno] AS [FreeKitDetailSno],
          [PBS].[TypeID],[PBS].[Sno],[PBS].[PID],[PBS].[SerialID],

		  [SM].[Serial],[SM].[MFGDate],[SM].[EXPDate],[SM].[InstallationDate],
		  [SM].[S1],[SM].[S2],[SM].[S3],[SM].[S4],[SM].[S5],[SM].[S6],[SM].[S7],[SM].[S8],[SM].[S9],[SM].[S10],
		  [SM].[SheetQty],

          [PBS].[AltStockQty],[PBS].[AltQty],[PBS].[AltUOMID],[PBS].[StockQty],[PBS].[Qty],[PBS].[UOMID],[PBS].[Rate],
          [PBS].[BasicAmt],[PBS].[GrossQty],[PBS].[TareQty],[PBS].[PCSOnly],[PBS].[Descriptions],[PBS].[TrackingNo]
    FROM [Purchase].[TBL_PurchaseReturnSerial] [PBS]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnDetails] [D] ON [PBS].[DetailID]=[D].[ID]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnBatch] [PBB] ON [PBS].[BatchDetailID]=[PBB].[ID]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnFreeKitDetails] [PBFKD] ON [PBS].[FreeKitDetailID]=[PBFKD].[ID]
	LEFT OUTER JOIN [GM].[TBL_SerialMaster] [SM] ON [PBS].[SerialID]=[SM].[ID]
    WHERE [PBS].[MasterID]= @ID

   --TBL_PurchaseReturnUOMDetails
    SELECT
         [PBUD].[ID],[D].[Sno] AS [DetailSno],
         [PBB].[Sno] AS [BatchDetailSno],
         [PBUD].[CQty],[PBUD].[CUOMID],[PBUD].[UOMQty],[PBUD].[UOMID]
    FROM [Purchase].[TBL_PurchaseReturnUOMDetails] [PBUD]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnDetails] [D] ON [PBUD].[DetailID]=[D].[ID]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnBatch] [PBB] ON [PBUD].[BatchDetailID]=[PBB].[ID]
    WHERE [PBUD].[MasterID] = @ID

   --TBL_PurchaseReturnCommission
    SELECT
          [PBC].[ID],[D].[Sno] AS [DetailSno],[PBC].[TypeID],[PBC].[CommissionBasisID],[PBC].[GLID],[PBC].[SLID] ,[PBC].[Rate],[PBC].[Amount],
          [PBC].[INotes],[PBC].[Descriptions],[PBC].[TrackingNo]
    FROM [Purchase].[TBL_PurchaseReturnCommission] [PBC]
    LEFT OUTER JOIN [Purchase].[TBL_PurchaseReturnDetails] [D] ON [PBC].[DetailID]=[D].[ID]
    WHERE [PBC].[MasterID] = @ID

   --TBL_PurchaseReturnDocs
    SELECT
         [Docs].[ID],[Docs].[MasterID],[Docs].[Sno],[Docs].[DocumentTypeID],[Docs].[DocumentName],[Docs].[DocumentLocation],[Docs].[Descriptions],[Docs].[Tag],[Docs].[DocumentContent]
    FROM [Purchase].[TBL_PurchaseReturnDocs] [Docs] 
    WHERE [Docs].[MasterID]= @ID
   
	
END
ELSE IF @IsSelect = 0 
BEGIN
    SELECT 
      [M].[ID]
     ,[M].[VNo] AS [VoucherNo] 
     ,[M].[VDate] AS [Date] 
     ,DM.Miti AS [Miti]
   ,CASE WHEN [M].[CancelByID] IS NULL THEN NULL ELSE 'Cancelled' END AS [CancelStatus]

     ,[Vendor].[Code] AS [VendorCode] 
     ,[Vendor].[Name] AS [VendorName]
     ,[CUR].[Code] AS [Currency]
     ,[M].[CurRate] AS [Currency Rate]
     ,[M].[NetAmt] AS [NetAmount]
     ,COALESCE(NULLIF(TRIM([EnumColl].[Caption]), ''), [EnumColl].[Name], 'All') AS [Type]

    FROM [PURCHASE].[TBL_PurchaseReturnMaster] AS M       
     LEFT OUTER JOIN [GM].[Tbl_Ledger] AS [Vendor] ON [M].[GLID] = [Vendor].[ID]
     LEFT OUTER JOIN [USYS].[TBL_Currency] AS [CUR] ON [M].[CurrencyID] = [CUR].[ID]
     LEFT OUTER JOIN [GM].[Tbl_DocNumbering] AS DN ON M.[NumId] = DN.[ID]
     LEFT OUTER JOIN [USYS].[TBL_EnumColl] AS [EnumColl] ON [DN].[TypeID] = [EnumColl].[ID]
     LEFT OUTER JOIN [Usys].[Tbl_DateMiti] AS DM ON [M].[VDate] = [DM].[ID]

    WHERE 
     ([M].[ID] = @ID OR @ID = 0)
      AND ([M].[FYID] = @FYID)
      AND ([M].[UnitID] = @UnitID) 
      AND (ISNULL([M].[NumID], 0) IN (SELECT value FROM STRING_SPLIT(@NumIDs, ',')) OR @NumIDs = '')
END
