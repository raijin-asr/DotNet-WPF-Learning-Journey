using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeMasterCRUD
{
    internal class Sales_Copy_Master_Backup
    {
        //don't delete or push this region, its for CopyMaster purpose only
        #region CopyMaster part
        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesQuotation(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesQuotation",
                "#TempCopyMasterSalesQuotation2",
                "#TempCopyMasterSalesQuotation3",
                "#TempCopyMasterSalesQuotation4",
                "#TempCopyMasterSalesQuotation5",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesQuotation ",
                " INTO #TempCopyMasterSalesQuotation2 ",
                " INTO #TempCopyMasterSalesQuotation3 ",
                " INTO #TempCopyMasterSalesQuotation4 ",
                " INTO #TempCopyMasterSalesQuotation5 ",
            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesQuotation",
                "@TempCopyMasterSalesQuotation2",
                "@TempCopyMasterSalesQuotation3",
                "@TempCopyMasterSalesQuotation4",
                "@TempCopyMasterSalesQuotation5",
            };

                tempTableNameInsert = new[] { "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesQuotation",
                "UTDT_TempCopyMasterSalesQuotation2",
                "UTDT_TempCopyMasterSalesQuotation3",
                "UTDT_TempCopyMasterSalesQuotation4",
                "UTDT_TempCopyMasterSalesQuotation5"
                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesQuotation;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesQuotation2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesQuotation3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesQuotation4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesQuotation5;

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [M].[IsNotLocal], 
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[Name] AS [ClosedBy],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CL] ON [M].[ClosedByID]=[CL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],	--(put it or not- [M].[{MainFieldName}] AS [MasterCodeName],)
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],[D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesQuotationDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesQuotationOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesQuotationTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesQuotationDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales Quotation ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesQuotationMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Quotation [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesQuotationMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Quotation [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesQuotationMaster] [T]) 


                        SET @smt=@smt + 'Sales Quotation Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesQuotationMaster]
                        (
                            [VNo], [VDate], [VTime], [RefVno],  [RefDate], [DueDays],  [DueDate], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [IsNotLocal], 
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo],  [PrintNo], 
                            [ClosedByID],
                            [ClosedDate], [ClosedRemarks],
                            [CancelByID],
                            [CancelDate], [CancelReason],[IsOpening],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [M].[IsNotLocal], 
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[ID] AS [ClosedByID],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CL] ON [M].[ClosedBy]=[CL].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesQuotationDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesQuotationMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesQuotationOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesQuotationMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesQuotationTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            --[DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesQuotationMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesQuotationDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesQuotationMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE]  
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                        ),[Ledger] AS(
                                SELECT DISTINCT
                                [L].[{MainFieldName}] AS [LedgerCodeName]
                                FROM [LedgerRaw] [LR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                                WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[ClosedByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ClosedByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesQuotationDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AG].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                                    WHERE [M].[AGID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AG].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                                    WHERE [M].[SMID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SP].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                                    WHERE [M].[SPID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                                    WHERE [M].[SLID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesQuotationMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [P].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesQuotationDetails] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [P].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesQuotationOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesQuotationOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesQuotationOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'SalesQuotationDetail' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesQuotationTerms] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [SD] ON [D].[DetailID]=[SD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesQuotationTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesQuotationDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }
        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesSauda(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesSauda",
                "#TempCopyMasterSalesSauda2",
                "#TempCopyMasterSalesSauda3",
                "#TempCopyMasterSalesSauda4",
                "#TempCopyMasterSalesSauda5",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesSauda ",
                " INTO #TempCopyMasterSalesSauda2 ",
                " INTO #TempCopyMasterSalesSauda3 ",
                " INTO #TempCopyMasterSalesSauda4 ",
                " INTO #TempCopyMasterSalesSauda5 ",
            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesSauda",
                "@TempCopyMasterSalesSauda2",
                "@TempCopyMasterSalesSauda3",
                "@TempCopyMasterSalesSauda4",
                "@TempCopyMasterSalesSauda5",
            };

                tempTableNameInsert = new[] { "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesSauda",
                "UTDT_TempCopyMasterSalesSauda2",
                "UTDT_TempCopyMasterSalesSauda3",
                "UTDT_TempCopyMasterSalesSauda4",
                "UTDT_TempCopyMasterSalesSauda5"
                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesSauda;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesSauda2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesSauda3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesSauda4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesSauda5;

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [M].[IsNotLocal], 
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[Name] AS [ClosedBy],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CL] ON [M].[ClosedByID]=[CL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            --[QD].[Sno] AS [QuotationDetailSno],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],[D].[TareQty],[D].[GrossWeight],
                            [D].[WeighBridge_Weight],[D].[ContractDate],[D].[PONo],[D].[PODate],
                            [D].[DocBank],[D].[CoFD],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[TinNo],[D].[TanNo],[D].[ServiceTaxNo],[D].[ExciseNo],
                            [D].[DDANo],[D].[DLNo1],[D].[DLNo2],[D].[DLValidUpto],[D].[GSTNo],
                            [D].[ExportInvNo],[D].[ExportInvDate],[D].[PPNo],[D].[PPDate],
                            [D].[TaxableAmount],[D].[TaxFreeAmount],[D].[Tax]

                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesSaudaOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesSaudaTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesSaudaDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
 
                        """;

            string InsertQuery = $""" 

                        Declare @smt Nvarchar(max) = '*********************** Sales Sauda ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesSaudaMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Sauda [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesSaudaMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Sauda [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesSaudaMaster] [T]) 


                        SET @smt=@smt + 'Sales Sauda Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesSaudaMaster]
                        (
                            [VNo], [VDate], [VTime], [RefVno],  [RefDate], [DueDays],  [DueDate], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [IsNotLocal], 
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo],  [PrintNo], 
                            [ClosedByID],
                            [ClosedDate], [ClosedRemarks],
                            [CancelByID],
                            [CancelDate], [CancelReason],[IsOpening],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [M].[IsNotLocal], 
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[ID] AS [ClosedByID],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CL] ON [M].[ClosedBy]=[CL].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesSaudaDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [QuotationID],
                            [Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[Rate],[D].[RateBasisID],
                            [QD].[ID] AS [QuotationID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesSaudaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[D].[ID] AND  [QD].[Sno]=[D].[QuotationDetailSno]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesSaudaOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],[TareQty],[GrossWeight],
                            [WeighBridge_Weight],[ContractDate],[PONo],[PODate],
                            [DocBank],[CoFD],[DeliveryPort],[CarriageBy],
                            [TermCondition],[TinNo],[TanNo],[ServiceTaxNo],[ExciseNo],
                            [DDANo],[DLNo1],[DLNo2],[DLValidUpto],[GSTNo],
                            [ExportInvNo],[ExportInvDate],[PPNo],[PPDate],
                            [TaxableAmount],[TaxFreeAmount],[Tax]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],[D].[TareQty],[D].[GrossWeight],
                            [D].[WeighBridge_Weight],[D].[ContractDate],[D].[PONo],[D].[PODate],
                            [D].[DocBank],[D].[CoFD],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[TinNo],[D].[TanNo],[D].[ServiceTaxNo],[D].[ExciseNo],
                            [D].[DDANo],[D].[DLNo1],[D].[DLNo2],[D].[DLValidUpto],[D].[GSTNo],
                            [D].[ExportInvNo],[D].[ExportInvDate],[D].[PPNo],[D].[PPDate],
                            [D].[TaxableAmount],[D].[TaxFreeAmount],[D].[Tax]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesSaudaMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesSaudaTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesSaudaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesSaudaDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesSaudaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 


                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE]  
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)


                        ;WITH [LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[ClosedByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ClosedByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL                        
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        
                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AG].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                                    WHERE [M].[AGID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AG].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                                    WHERE [M].[SMID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SP].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                                    WHERE [M].[SPID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                                    WHERE [M].[SLID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesSaudaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [P].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [P].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'QuotationDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[QD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesSaudaDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [QD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesSaudaOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesSaudaOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesSaudaOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                           -- UNION ALL
                            --SELECT DISTINCT
                                --'SalesSaudaDetail' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesSaudaTerms] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[DetailID]=[SD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesSaudaTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                        
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesSaudaDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesOrder(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesOrder",
                "#TempCopyMasterSalesOrder2",
                "#TempCopyMasterSalesOrder3",
                "#TempCopyMasterSalesOrder4",
                "#TempCopyMasterSalesOrder5",
                "#TempCopyMasterSalesOrder6",
                "#TempCopyMasterSalesOrder7",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesOrder ",
                " INTO #TempCopyMasterSalesOrder2 ",
                " INTO #TempCopyMasterSalesOrder3 ",
                " INTO #TempCopyMasterSalesOrder4 ",
                " INTO #TempCopyMasterSalesOrder5 ",
                " INTO #TempCopyMasterSalesOrder6 ",
                " INTO #TempCopyMasterSalesOrder7 ",
            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesOrder",
                "@TempCopyMasterSalesOrder2",
                "@TempCopyMasterSalesOrder3",
                "@TempCopyMasterSalesOrder4",
                "@TempCopyMasterSalesOrder5",
                "@TempCopyMasterSalesOrder6",
                "@TempCopyMasterSalesOrder7",
            };

                tempTableNameInsert = new[] { "", "", "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesOrder",
                "UTDT_TempCopyMasterSalesOrder2",
                "UTDT_TempCopyMasterSalesOrder3",
                "UTDT_TempCopyMasterSalesOrder4",
                "UTDT_TempCopyMasterSalesOrder5",
                "UTDT_TempCopyMasterSalesOrder6",
                "UTDT_TempCopyMasterSalesOrder7"
                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder5;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder6;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesOrder7;

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime],[M].[DeliveryDate], [M].[DeliveryTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate],[M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [M].[IsNotLocal], [M].[TypeID], 
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[Name] AS [ClosedBy],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CL] ON [M].[ClosedByID]=[CL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            --[SD].[Sno] AS [SaudaDetailSno],
                            --[QD].[Sno] AS [QuotationDetailSno],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Weight],[D].[WastagePercentage],
                            [D].[Wastage],[D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesOrderStoneDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesOrderOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesOrderTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Advance---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [MP].[{MainFieldName}] AS [ModeOfPayment],
                            [D].[TypeID],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            [D].[CardNo],[D].[ChequeNo],[D].[ChequeDate],[D].[BankBranchName],
                            [D].[SlipNo],[D].[SlipDate],[D].[ServiceChargeRate],[D].[ServiceChargeAmount],
                            [D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[5]}
                        FROM  {DBName}[SALES].[TBL_SalesOrderAdvance] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [MP] ON [D].[ModeOfPaymentID]=[MP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[6]}
                        FROM  {DBName}[SALES].[TBL_SalesOrderDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                        """;

            string InsertQuery = $""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales Order ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesOrderMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Order [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesOrderMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Order [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesOrderMaster] [T]) 


                        SET @smt=@smt + 'Sales Order Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesOrderMaster]
                        (
                            [VNo], [VDate], [VTime],[DeliveryDate], [DeliveryTime], [RefVno],  [RefDate], [DueDays],  [DueDate],  [IsCash], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [IsNotLocal],[TypeID], 
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo],  [PrintNo], 
                            [ClosedByID],
                            [ClosedDate], [ClosedRemarks],
                            [CancelByID],
                            [CancelDate], [CancelReason],[IsOpening],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate],  [M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [M].[IsNotLocal], [M].[TypeID], 
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[ID] AS [ClosedByID],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CL] ON [M].[ClosedBy]=[CL].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [SaudaID],
                            [QuotationID],
                            [PCSOnly],[FreePCSOnly],[Weight],[WastagePercentage],
                            [Wastage],[MetalAmt],[StoneQty],[StoneAmt],
                            [Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [SD].[ID] AS [SaudaID],
                            [QD].[ID] AS [QuotationID],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Weight],[D].[WastagePercentage],
                            [D].[Wastage],[D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesSaudaDetails] [SD] ON [SD].[MasterID]=[M].[ID] AND  [SD].[Sno]=[D].[SaudaDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[M].[ID] AND  [QD].[Sno]=[D].[QuotationDetailSno]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderStoneDetails]([MasterID],[DetailID],[Sno],[PID],[Qty],[UOMID],[PCS],[Rate],[Amount],[Descriptions],[TrackingNo])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]
                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                       
                        ----------------------------------------------------Advance---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderAdvance]
                        (
                            [MasterID], 
                            [ModeOfPaymentID],
                            [TypeID],
                            [GLID],
                            [SLID],
                            [CardNo],[ChequeNo],[ChequeDate],[BankBranchName],
                            [SlipNo],[SlipDate],[ServiceChargeRate],[ServiceChargeAmount],
                            [Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [MP].[ID] AS [ModeOfPaymentID],
                            [D].[TypeID],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            [D].[CardNo],[D].[ChequeNo],[D].[ChequeDate],[D].[BankBranchName],
                            [D].[SlipNo],[D].[SlipDate],[D].[ServiceChargeRate],[D].[ServiceChargeAmount],
                            [D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[5]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_Product] [MP] ON [D].[ModeOfPayment]=[MP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [D].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[6]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE]  
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesOrderAdvance] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 

                        ),[SubledgerRaw] AS(
                            SELECT DISTINCT
                                [M].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesOrderAdvance] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                        ),[Subledger] AS(
                            SELECT DISTINCT
                            [SL].[{MainFieldName}] AS [SubledgerCodeName]
                            FROM [SubledgerRaw] [SLR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [SLR].[SLID]=[SL].[ID]
                            WHERE [SL].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[ClosedByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ClosedByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                        
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[ProductRaw] AS(
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesOrderStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                        ),[Product] AS(
                                SELECT DISTINCT
                                [P].[{MainFieldName}] AS [ProductCodeName]
                                FROM [ProductRaw] [PR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [PR].[PID]=[P].[ID]
                                WHERE [P].[{MainFieldName}] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesOrderStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ),[DetailRaw] AS(
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesOrderTerms] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesOrderStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                        --),[Detail] AS(
                                --SELECT DISTINCT
                                --[SOD].[Sno] AS [DetailSno]   
                                --FROM [DetailRaw] [DR]
                                --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [SOD] ON [DR].[DetailID]=[SOD].[ID]
                                --WHERE [SOD].[Sno] IS NOT NULL  
                        
                       
                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [SLD].[SubledgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Subledger] [SLD]
                            WHERE [SLD].[SubledgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AG].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                                    WHERE [M].[AGID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AG].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                                    WHERE [M].[SMID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SP].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                                    WHERE [M].[SPID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                                    WHERE [M].[SLID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [PD].[ProductCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Product] [PD]
                            WHERE [PD].[ProductCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'SaudaDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'QuotationDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[QD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesOrderDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [QD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesOrderOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesOrderOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesOrderOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Detail' AS ModuleName,  STRING_AGG(CAST( [DET].[DetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM Detail [DET]
                            --WHERE [DET].[DetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesOrderTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'ModeOfPayment' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [MP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesOrderAdvance] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [MP] ON [D].[ModeOfPaymentID]=[MP].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [MP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesOrderDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesDispatchOrder(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesDispatchOrder",
                "#TempCopyMasterSalesDispatchOrder2",
                "#TempCopyMasterSalesDispatchOrder3",
                "#TempCopyMasterSalesDispatchOrder4",
                "#TempCopyMasterSalesDispatchOrder5",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesDispatchOrder ",
                " INTO #TempCopyMasterSalesDispatchOrder2 ",
                " INTO #TempCopyMasterSalesDispatchOrder3 ",
                " INTO #TempCopyMasterSalesDispatchOrder4 ",
                " INTO #TempCopyMasterSalesDispatchOrder5 ",

            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesDispatchOrder",
                "@TempCopyMasterSalesDispatchOrder2",
                "@TempCopyMasterSalesDispatchOrder3",
                "@TempCopyMasterSalesDispatchOrder4",
                "@TempCopyMasterSalesDispatchOrder5",

            };

                tempTableNameInsert = new[] { "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesDispatchOrder",
                "UTDT_TempCopyMasterSalesDispatchOrder2",
                "UTDT_TempCopyMasterSalesDispatchOrder3",
                "UTDT_TempCopyMasterSalesDispatchOrder4",
                "UTDT_TempCopyMasterSalesDispatchOrder5",

                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesDispatchOrder;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesDispatchOrder2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesDispatchOrder3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesDispatchOrder4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesDispatchOrder5;
                      

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[DeliveryDate],  [M].[DeliveryTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate],[M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[{MainFieldName}] AS [Godown],
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [M].[IsNotLocal],
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[Name] AS [ClosedBy],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [M].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CL] ON [M].[ClosedByID]=[CL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            --[OD].[Sno] AS [OrderDetailSno],
                            --[SD].[Sno] AS [SaudaDetailSno],
                            --[QD].[Sno] AS [QuotationDetailSno],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesDispatchOrderOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesDispatchOrderTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales DispatchOrder ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesDispatchOrderMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales DispatchOrder [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesDispatchOrderMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales DispatchOrder [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesDispatchOrderMaster] [T]) 


                        SET @smt=@smt + 'Sales DispatchOrder Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesDispatchOrderMaster]
                        (
                            [VNo], [VDate], [VTime],[DeliveryDate], [DeliveryTime], [RefVno],  [RefDate], [DueDays],  [DueDate],  [IsCash], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GodownID],
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [IsNotLocal],
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo],  [PrintNo], 
                            [ClosedByID],
                            [ClosedDate], [ClosedRemarks],
                            [CancelByID],
                            [CancelDate], [CancelReason],[IsOpening],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime],[M].[DeliveryDate], [M].[DeliveryTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate],  [M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[ID] AS [GodownID],
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [M].[IsNotLocal], 
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [CL].[ID] AS [ClosedByID],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [M].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CL] ON [M].[ClosedBy]=[CL].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesDispatchOrderDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [OrderID],
                            [SaudaID],
                            [QuotationID],
                            [PCSOnly],[FreePCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [OD].[ID] AS [OrderID],
                            [SD].[ID] AS [SaudaID],
                            [QD].[ID] AS [QuotationID],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [OD] ON [OD].[MasterID]=[D].[ID] AND  [OD].[Sno]=[D].[OrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesSaudaDetails] [SD] ON [SD].[MasterID]=[D].[ID] AND  [SD].[Sno]=[D].[SaudaDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[D].[ID] AND  [QD].[Sno]=[D].[QuotationDetailSno]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesDispatchOrderOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],
                            [TareQty],[GrossWeight],[WeighBridge_Weight],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesDispatchOrderTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesOrderDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE] 
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [GodownRaw] AS(
                            SELECT DISTINCT
                                [M].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GodownID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                        ),[Godown] AS(
                            SELECT DISTINCT
                            [GM].[{MainFieldName}] AS [GodownCodeName]
                            FROM [GodownRaw] [GR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GM] ON [GR].[GodownID]=[GM].[ID]
                            WHERE [GM].[{MainFieldName}] IS NOT NULL 
                        ),[LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[ClosedByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ClosedByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                           
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                       
                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Godown' AS [ModuleName],  STRING_AGG(CAST( [GOD].[GodownCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Godown] [GOD]
                            WHERE [GOD].[GodownCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AG].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                                    WHERE [M].[AGID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AG].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                                    WHERE [M].[SMID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SP].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                                    WHERE [M].[SPID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                                    WHERE [M].[SLID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesDispatchOrderMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                            [P].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [P].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'OrderDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[OD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [OD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'SaudaDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'QuotationDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[QD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [QD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesDispatchOrderOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesDispatchOrderOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesDispatchOrderOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'SalesOrderDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[OD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesDispatchOrderTerms] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[DetailID]=[OD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [OD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesDispatchOrderTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesDispatchOrderDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesChallan(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesChallan",
                "#TempCopyMasterSalesChallan2",
                "#TempCopyMasterSalesChallan3",
                "#TempCopyMasterSalesChallan4",
                "#TempCopyMasterSalesChallan5",
                "#TempCopyMasterSalesChallan6",
                "#TempCopyMasterSalesChallan7",
                "#TempCopyMasterSalesChallan8",
                "#TempCopyMasterSalesChallan9",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesChallan ",
                " INTO #TempCopyMasterSalesChallan2 ",
                " INTO #TempCopyMasterSalesChallan3 ",
                " INTO #TempCopyMasterSalesChallan4 ",
                " INTO #TempCopyMasterSalesChallan5 ",
                " INTO #TempCopyMasterSalesChallan6 ",
                " INTO #TempCopyMasterSalesChallan7 ",
                " INTO #TempCopyMasterSalesChallan8 ",
                " INTO #TempCopyMasterSalesChallan9 ",

            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesChallan",
                "@TempCopyMasterSalesChallan2",
                "@TempCopyMasterSalesChallan3",
                "@TempCopyMasterSalesChallan4",
                "@TempCopyMasterSalesChallan5",
                "@TempCopyMasterSalesChallan6",
                "@TempCopyMasterSalesChallan7",
                "@TempCopyMasterSalesChallan8",
                "@TempCopyMasterSalesChallan9",

            };

                tempTableNameInsert = new[] { "", "", "", "", "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesChallan",
                "UTDT_TempCopyMasterSalesChallan2",
                "UTDT_TempCopyMasterSalesChallan3",
                "UTDT_TempCopyMasterSalesChallan4",
                "UTDT_TempCopyMasterSalesChallan5",
                "UTDT_TempCopyMasterSalesChallan6",
                "UTDT_TempCopyMasterSalesChallan7",
                "UTDT_TempCopyMasterSalesChallan8",
                "UTDT_TempCopyMasterSalesChallan9",

                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan5;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan6;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan7;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan8;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesChallan9;
                      

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate],[M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[{MainFieldName}] AS [Godown],
                            [LCM].[Vno] AS [LCMasterVno],
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [M].[IsNotLocal],
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[Name] AS [AuditLockBy],
                            [M].[AuditLockDate], 
                            [RC].[Name] AS [ReconcileBy],
                            [M].[ReconcileDate], 
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowStockPost],
                            [SPU].[Name] AS [StockPostUser],
                            [M].[StockPostDate],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [M].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [M].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [M].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [AD] ON [M].[AuditLockByID]=[AD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [RC] ON [M].[ReconcileByID]=[RC].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUserID] = [SPU].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            --[DOD].[Sno] AS [DispatchOrderDetailSno],
                            --[OD].[Sno] AS [OrderDetailSno],
                            --[SD].[Sno] AS [SaudaDetailSno],
                            --[QD].[Sno] AS [QuotationDetailSno],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [D].[Weight],[D].[WastagePercentage],[D].[Wastage],
                            [D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [D].[DispatchOrderID]=[DOD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GO].[{MainFieldName}] AS [Godown],
                            [D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanStoneDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GO] ON [D].[GodownID]=[GO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Batch---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[BAT].[PID] AS [Batch],   --here using PID column as there is no code or name field in BatchMaster
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Serial---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[BDT].[Sno] AS [BatchSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[SER].[PID] AS [Serial],   --here using PID column as there is no code or name field in SerialMaster
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SerialMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------FreeKit Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[DetailSno],[D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[5]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------UOM Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[ID] AS [DetailID], --here using ID for now as there is no code, name or sno column
                            --[BDT].[ID] AS [BatchDetailID],  --here using ID for now as there is no code, name or sno column
                            [D].[CQty],
                            [CU].[Code] AS [CUOM],
                            [D].[UOMQty],
                            [UO].[Code] AS [UOM]
                        
                            {tempTableNameInsert[6]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanUOMDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [CU] ON [D].[CUOMID]=[CU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[7]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[8]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[9]}
                        FROM  {DBName}[SALES].[TBL_SalesChallanDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales Challan ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesChallanMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Challan [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesChallanMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Challan [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesChallanMaster] [T]) 


                        SET @smt=@smt + 'Sales Challan Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesChallanMaster]
                        (
                            [VNo], [VDate], [VTime], [RefVno],  [RefDate], [DueDays],  [DueDate],  [IsCash], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GodownID],
                            [LCMasterID],
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [GoodsGLID],
                            [GoodsSLID],
                            [IsNotLocal],
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo], [PrintNo], 
                            [AuditLockByID],
                            [AuditLockDate],
                            [ReconcileByID],
                            [ReconcileDate],
                            [CancelByID],
                            [CancelDate], [CancelReason],[IsOpening],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],
                            [IsAllowStockPost],[StockPostUserID],[StockPostDate],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[RefVno], [M].[RefDate], [M].[DueDays],  [M].[DueDate],  [M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[ID] AS [GodownID],
                            [LCM].[ID] AS [LCMasterID],
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [M].[IsNotLocal], 
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[ID] AS [AuditLockByID],
                            [M].[AuditLockDate], 
                            [RC].[ID] AS [ReconcileByID],
                            [M].[ReconcileDate], 
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowStockPost],
                            [SPU].[ID] AS [StockPostUserID],
                            [M].[StockPostDate],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [M].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [FIN].[TBL_LCMaster] [LCM] ON [LCM].[MasterID]=[M].[ID] AND [M].[LCMasterVno]=[LCM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [M].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [M].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [AD] ON [M].[AuditLockBy]=[AD].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [RC] ON [M].[ReconcileByID]=[RC].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUser] = [SPU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [GrossQty],[TareQty],
                            [DispatchOrderID],
                            [OrderID],
                            [SaudaID],
                            [QuotationID],
                            [PCSOnly],[FreePCSOnly],[GoodsGLID],[GoodsSLID],
                            [Weight],[WastagePercentage],[Wastage],[MetalAmt],
                            [StoneQty],[StoneAmt],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            [DOD].[ID] AS [DispatchOrderID],
                            [OD].[ID] AS [OrderID],
                            [SD].[ID] AS [SaudaID],
                            [QD].[ID] AS [QuotationID],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [D].[Weight],[D].[WastagePercentage],[D].[Wastage],
                            [D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [DOD].[MasterID]=[D].[ID] AND  [DOD].[Sno]=[D].[DispatchOrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [OD] ON [OD].[MasterID]=[D].[ID] AND  [OD].[Sno]=[D].[OrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesSaudaDetails] [SD] ON [SD].[MasterID]=[D].[ID] AND  [SD].[Sno]=[D].[SaudaDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[D].[ID] AND  [QD].[Sno]=[D].[QuotationDetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanStoneDetails]([MasterID],[DetailID],[Sno],[PID],[GodownID],[Qty],[UOMID],[PCS],[Rate],[Amount],[Descriptions],[TrackingNo])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GO].[ID] AS [GodownID],
                            [D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[PCS],[D].[Rate],,[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GO] ON [D].[Godown]=[GO].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Batch---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanBatch]
                        (
                            [MasterID],
                            [DetailID],
                            [FreeKitDetailID],
                            [Sno],
                            [PID],
                            [BatchID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],[Rate],
                            [BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[FreePCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [BAT].[ID] AS [BatchID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],
                            [D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BAT] ON [BAT].[ID]=[D].[ID] AND  [BAT].[Batch]=[D].[Batch]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Serial---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanSerial]
                        (
                            [MasterID],
                            [DetailID], 
                            [BatchDetailID], 
                            [FreeKitDetailID], 
                            [TypeID],[D].[Sno],
                            [PID],
                            [SerialID],
                            [MFGDate],[EXPDate],[InstallationDate],[S1],
                            [S2],[S3],[S4],[S5],[S6],[S7],[S8],
                            [S9],[S10],[AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [SER].[ID] AS [SerialID],
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[Sno]=[D].[BatchSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_SerialMaster] [SER] ON [SER].[MasterID]=[D].[ID] AND  [SER].[Serial]=[D].[Serial]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------FreeKit Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanFreeKitDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [DetailSno],[TypeID],[Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[RateBasisID],[BasicAmt],[GrossQty],
                            [TareQty],[PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[DetailSno],[D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[5]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------UOM Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanUOMDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [BatchDetailID],
                            [CQty],
                            [CUOMID],
                            [UOMQty],
                            [UOMID]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID],
                            [D].[CQty],
                            [CU].[ID] AS [CUOMID],
                            [D].[UOMQty],
                            [UO].[ID] AS [UOMID]
                        FROM {tempTableName[6]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[ID]=[D].[DetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[ID]=[D].[BatchDetailID]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [CU] ON [D].[CUOM]=[CU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID]

                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],
                            [TareQty],[GrossWeight],[WeighBridge_Weight],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[7]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[8]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesChallanDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[9]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesChallanMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE] 
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [GodownRaw] AS(
                            SELECT DISTINCT
                                [M].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GodownID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesChallanStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                        ),[Godown] AS(
                            SELECT DISTINCT
                            [GM].[{MainFieldName}] AS [GodownCodeName]
                            FROM [GodownRaw] [GR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GM] ON [GR].[GodownID]=[GM].[ID]
                            WHERE [GM].[{MainFieldName}] IS NOT NULL 
                        ),[LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[SubledgerRaw] AS(
                            SELECT DISTINCT
                                [M].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsSLID] IS NOT NULL
                        ),[Subledger] AS(
                            SELECT DISTINCT
                            [SL].[{MainFieldName}] AS [SubledgerCodeName]
                            FROM [SubledgerRaw] [SLR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [SLR].[SLID]=[SL].[ID]
                            WHERE [SL].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[AuditLockByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[AuditLockByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ReconcileByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ReconcileByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[StockPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[StockPostUserID] IS NOT NULL
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[ProductRaw] AS(
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesChallanStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                        ),[Product] AS(
                                SELECT DISTINCT
                                [P].[{MainFieldName}] AS [ProductCodeName]
                                FROM [ProductRaw] [PR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [PR].[PID]=[P].[ID]
                                WHERE [P].[{MainFieldName}] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[CUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[CUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesChallanUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ),[DetailRaw] AS(
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesChallanTerms] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                        ),[Detail] AS(
                                SELECT DISTINCT
                                [SOD].[Sno] AS [DetailSno]   
                                FROM [DetailRaw] [DR]
                                LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [SOD] ON [DR].[DetailID]=[SOD].[ID]
                                WHERE [SOD].[Sno] IS NOT NULL  

                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Godown' AS [ModuleName],  STRING_AGG(CAST( [GOD].[GodownCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Godown] [GOD]
                            WHERE [GOD].[GodownCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'LCMaster' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM  (
                                    --SELECT DISTINCT
                                        --[LCM].[Vno]
                                    --FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    --LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                                   -- WHERE [M].[LCMasterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LCM].[Vno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [SLD].[SubledgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Subledger] [SLD]
                            WHERE [SLD].[SubledgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AG].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                                    WHERE [M].[AGID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AG].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                                    WHERE [M].[SMID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SP].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                                    WHERE [M].[SPID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesChallanMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [PD].[ProductCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Product] [PD]
                            WHERE [PD].[ProductCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'DispatchOrderDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[DOD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [D].[DispatchOrderID]=[DOD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DOD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'OrderDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[OD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [OD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'SaudaDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'QuotationDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[QD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesChallanDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [QD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Batch' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[BAT].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesChallanBatch] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BAT].[PID] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Serial' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SER].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesChallanSerial] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SER].[PID] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesChallanOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesChallanOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesChallanOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Detail' AS ModuleName,  STRING_AGG(CAST( [DET].[DetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM Detail [DET]
                            WHERE [DET].[DetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesChallanTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesChallanDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesProforma(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesProforma",
                "#TempCopyMasterSalesProforma2",
                "#TempCopyMasterSalesProforma3",
                "#TempCopyMasterSalesProforma4",
                "#TempCopyMasterSalesProforma5",
                "#TempCopyMasterSalesProforma6",
                "#TempCopyMasterSalesProforma7",
                "#TempCopyMasterSalesProforma8",
                "#TempCopyMasterSalesProforma9",
                "#TempCopyMasterSalesProforma10",
                "#TempCopyMasterSalesProforma11",
                "#TempCopyMasterSalesProforma12",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesProforma ",
                " INTO #TempCopyMasterSalesProforma2 ",
                " INTO #TempCopyMasterSalesProforma3 ",
                " INTO #TempCopyMasterSalesProforma4 ",
                " INTO #TempCopyMasterSalesProforma5 ",
                " INTO #TempCopyMasterSalesProforma6 ",
                " INTO #TempCopyMasterSalesProforma7 ",
                " INTO #TempCopyMasterSalesProforma8 ",
                " INTO #TempCopyMasterSalesProforma9 ",
                " INTO #TempCopyMasterSalesProforma10 ",
                " INTO #TempCopyMasterSalesProforma11 ",
                " INTO #TempCopyMasterSalesProforma12 ",

            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesProforma",
                "@TempCopyMasterSalesProforma2",
                "@TempCopyMasterSalesProforma3",
                "@TempCopyMasterSalesProforma4",
                "@TempCopyMasterSalesProforma5",
                "@TempCopyMasterSalesProforma6",
                "@TempCopyMasterSalesProforma7",
                "@TempCopyMasterSalesProforma8",
                "@TempCopyMasterSalesProforma9",
                "@TempCopyMasterSalesProforma10",
                "@TempCopyMasterSalesProforma11",
                "@TempCopyMasterSalesProforma12",

            };

                tempTableNameInsert = new[] { "", "", "", "", "", "", "", "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesProforma",
                "UTDT_TempCopyMasterSalesProforma2",
                "UTDT_TempCopyMasterSalesProforma3",
                "UTDT_TempCopyMasterSalesProforma4",
                "UTDT_TempCopyMasterSalesProforma5",
                "UTDT_TempCopyMasterSalesProforma6",
                "UTDT_TempCopyMasterSalesProforma7",
                "UTDT_TempCopyMasterSalesProforma8",
                "UTDT_TempCopyMasterSalesProforma9",
                "UTDT_TempCopyMasterSalesProforma10",
                "UTDT_TempCopyMasterSalesProforma11",
                "UTDT_TempCopyMasterSalesProforma12",

                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma5;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma6;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma7;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma8;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma9;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma10;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma11;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesProforma12;
                      

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[EffDate],[M].[RefVno],  [M].[RefDate], [M].[DueDays],  [M].[DueDate],[M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[{MainFieldName}] AS [Godown],
                            [LCM].[Vno] AS [LCMasterVno],
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [M].[IsNotLocal],[M].[IsABT],
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate],[M].[DCurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [GM].[{MainFieldName}] AS [Guest],  
                            [M].[TenderAmount], [M].[ReturnAmount],[M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[Name] AS [AuditLockBy],
                            [M].[AuditLockDate], 
                            [RC].[Name] AS [ReconcileBy],
                            [M].[ReconcileDate], 
                            [CLO].[Name] AS [ClosedBy],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [M].[AccountPostDate],[M].[IsAllowStockPost],
                            [SPU].[Name] AS [StockPostUser],
                            [M].[StockPostDate],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [M].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [M].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [M].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GuestMaster] [GM] ON [M].[GuestID]=[GM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [AD] ON [M].[AuditLockByID]=[AD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [RC] ON [M].[ReconcileByID]=[RC].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CLO] ON [M].[ClosedByID] = [CLO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [APU] ON [M].[AccountPostUserID] = [APU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUserID] = [SPU].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            --[CD].[Sno] AS [ChallanDetailSno],
                            --[DOD].[Sno] AS [DispatchOrderDetailSno],
                            --[OD].[Sno] AS [OrderDetailSno],
                            --[SD].[Sno] AS [SaudaDetailSno],
                            --[QD].[Sno] AS [QuotationDetailSno],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],
                            [D].[Weight],[D].[WastagePercentage],[D].[Wastage],
                            [D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [CD] ON [D].[ChallanID]=[CD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [D].[DispatchOrderID]=[DOD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GO].[{MainFieldName}] AS [Godown],
                            [D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaStoneDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GO] ON [D].[GodownID]=[GO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Batch---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[BAT].[PID] AS [Batch],   --here using PID column as there is no code or name field in BatchMaster
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Serial---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[BDT].[Sno] AS [BatchSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[SER].[PID] AS [Serial],   --here using PID column as there is no code or name field in SerialMaster
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SerialMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------FreeKit Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[DetailSno] AS [SnoDetail],[D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[5]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------UOM Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[ID] AS [DetailID], --no Sno column, so putting other column like ID
                            --[BDT].[ID] AS [BatchDetailID],  --no Sno column, so putting other column like ID
                            [D].[CQty],
                            [CU].[Code] AS [CUOM],
                            [D].[UOMQty],
                            [UO].[Code] AS [UOM]
                        
                            {tempTableNameInsert[6]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaUOMDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [CU] ON [D].[CUOMID]=[CU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Commission---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [D].[TypeID],[D].[CommissionBasisID],
                            [AG].[{MainFieldName}] AS [Agent],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[7]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [D].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [D].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [D].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Settlement---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [MP].[{MainFieldName}] AS [ModeOfPayment],
                            [D].[TypeID],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            --[PBM].[Vno] AS [PurchaseBillVno],
                            --[SRM].[Vno] AS [SalesReturnVno],
                            --[SEM].[Vno] AS [SalesExpiryBreakageVno],
                            [D].[Sign],[D].[CardNo],[D].[ChequeNo],[D].[ChequeDate],[D].[BankBranchName],
                            [D].[SlipNo],[D].[SlipDate],[D].[ServiceChargeRate],[D].[ServiceChargeAmount],
                            [D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[8]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaSettlement] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ModeOfPayment] [MP] ON [D].[ModeOfPaymentID]=[MP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[PURCHASE].[TBL_PurchaseBillMaster] [PBM] ON [D].[PBMID]=[PBM].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [SRM] ON [D].[SRMID]=[SRM].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [SEM] ON [D].[SEMID]=[SEM].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Printing Log---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[DesignID], [D].[DesignName],[D].[PrinterName],
                            [PBY].[Name] AS [Printedby],
                            [D].[PrintedDateTime], [D].[PageID]
                            {tempTableNameInsert[9]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaPrintingLog] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [PBY] ON [D].[PrintedbyID]=[PBY].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[10]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[11]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[12]}
                        FROM  {DBName}[SALES].[TBL_SalesProformaDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $$""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales Proforma ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesProformaMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Proforma [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesProformaMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Proforma [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesProformaMaster] [T]) 


                        SET @smt=@smt + 'Sales Proforma Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesProformaMaster]
                        (
                            [VNo], [VDate], [VTime], [EffDate],[RefVno],  [RefDate], [DueDays],  [DueDate],  [IsCash], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GodownID],
                            [LCMasterID],
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [GoodsGLID],
                            [GoodsSLID],
                            [IsNotLocal],[IsABT],
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate],[DCurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [GuestID], 
                            [TenderAmount], [ReturnAmount],[INotes], [Remarks], [TrackingNo], [PrintNo], 
                            [AuditLockByID],
                            [AuditLockDate],
                            [ReconcileByID],
                            [ReconcileDate],
                            [ClosedByID],
                            [ClosedDate], [ClosedRemarks],
                            [CancelByID],
                            [CancelDate], [CancelReason],[IsOpening],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],
                            [IsAllowAccountPost],[AccountPostUserID],[AccountPostDate],
                            [IsAllowStockPost],[StockPostUserID],[StockPostDate],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[EffDate],[M].[RefVno], [M].[RefDate], [M].[DueDays],  [M].[DueDate],  [M].[IsCash], 
                            [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[ID] AS [GodownID],
                            [LCM].[ID] AS [LCMasterID],
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [M].[IsNotLocal], [M].[IsABT],
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate],[M].[DCurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [GM].[ID] AS [GuestID],  
                            [M].[TenderAmount], [M].[ReturnAmount],[M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[ID] AS [AuditLockByID],
                            [M].[AuditLockDate], 
                            [RC].[ID] AS [ReconcileByID],
                            [M].[ReconcileDate], 
                            [CLO].[ID] AS [ClosedByID],
                            [M].[ClosedDate], [M].[ClosedRemarks],
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],[M].[IsOpening],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [M].[AccountPostDate],[M].[IsAllowStockPost],
                            [SPU].[ID] AS [StockPostUserID],
                            [M].[StockPostDate],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [M].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [FIN].[TBL_LCMaster] [LCM] ON [LCM].[MasterID]=[M].[ID] AND [M].[LCMasterVno]=[LCM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [M].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [M].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GuestMaster] [GM] ON [M].[Guest]=[GM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [AD] ON [M].[AuditLockBy]=[AD].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [RC] ON [M].[ReconcileBy]=[RC].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CLO] ON [M].[ClosedBy] = [CLO].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [APU] ON [M].[AccountPostUser] = [APU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUser] = [SPU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [GrossQty],[TareQty],
                            [ChallanID],
                            [DispatchOrderID],
                            [OrderID],
                            [SaudaID],
                            [QuotationID],
                            [PCSOnly],[FreePCSOnly],
                            [GoodsGLID],[GoodsSLID],
                            [IncludeVatRate],[IncludeVatAmount],
                            [Weight],[WastagePercentage],[Wastage],[MetalAmt],
                            [StoneQty],[StoneAmt],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            [CD].[ID] AS [ChallanID],
                            [DOD].[ID] AS [DispatchOrderID],
                            [OD].[ID] AS [OrderID],
                            [SD].[ID] AS [SaudaID],
                            [QD].[ID] AS [QuotationID],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],
                            [D].[Weight],[D].[WastagePercentage],[D].[Wastage],
                            [D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [CD] ON [CD].[MasterID]=[D].[ID] AND  [CD].[Sno]=[D].[ChallanDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [DOD].[MasterID]=[D].[ID] AND  [DOD].[Sno]=[D].[DispatchOrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [OD] ON [OD].[MasterID]=[D].[ID] AND  [OD].[Sno]=[D].[OrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesSaudaDetails] [SD] ON [SD].[MasterID]=[D].[ID] AND  [SD].[Sno]=[D].[SaudaDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[D].[ID] AND  [QD].[Sno]=[D].[QuotationDetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaStoneDetails]([MasterID],[DetailID],[Sno],[PID],[GodownID],[Qty],[UOMID],[PCS],[Rate],[Amount],[Descriptions],[TrackingNo])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GO].[ID] AS [GodownID],
                            [D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GO] ON [D].[Godown]=[GO].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Batch---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaBatch]
                        (
                            [MasterID],
                            [DetailID],
                            [FreeKitDetailID],
                            [Sno],
                            [PID],
                            [BatchID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],[Rate],
                            [BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[FreePCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [BAT].[ID] AS [BatchID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],
                            [D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BAT] ON [BAT].[ID]=[D].[ID] AND  [BAT].[Batch]=[D].[Batch]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Serial---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaSerial]
                        (
                            [MasterID],
                            [DetailID], 
                            [BatchDetailID], 
                            [FreeKitDetailID], 
                            [TypeID],[Sno],
                            [PID],
                            [SerialID],
                            [MFGDate],[EXPDate],[InstallationDate],[S1],
                            [S3],[S4],[S5],[S6],[S7],[S8],
                            [S9],[S10],[AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [SER].[ID] AS [SerialID],
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[Sno]=[D].[BatchSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_SerialMaster] [SER] ON [SER].[MasterID]=[D].[ID] AND  [SER].[Serial]=[D].[Serial]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------FreeKit Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaFreeKitDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [DetailSno],[TypeID],[Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[RateBasisID],[BasicAmt],[GrossQty],
                            [TareQty],[PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[DetailSno] AS [SnoDetail],[D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[5]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------UOM Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaUOMDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [BatchDetailID],
                            [CQty],
                            [CUOMID],
                            [UOMQty],
                            [UOMID]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID],
                            [D].[CQty],
                            [CU].[ID] AS [CUOMID],
                            [D].[UOMQty],
                            [UO].[ID] AS [UOMID]
                        FROM {tempTableName[6]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[ID]=[D].[DetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[ID]=[D].[BatchDetailID]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [CU] ON [D].[CUOM]=[CU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Commission---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaCommission]
                        (
                            [MasterID],
                            [DetailID],
                            [TypeID],[CommissionBasisID],
                            [AGID],
                            [SPID],
                            [SMID],
                            [GLID],
                            [SLID],
                            [Rate],[Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[TypeID],[D].[CommissionBasisID],
                            [AG].[ID] AS [AGID],
                            [SP].[ID] AS [SPID],
                            [SM].[ID] AS [SMID],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[7]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Rate]=[D].[DetailRate]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [D].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [D].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [D].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [D].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Rate]

                        ----------------------------------------------------Settlement---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaSettlement]
                        (
                            [MasterID], 
                            [ModeOfPaymentID],
                            [TypeID],
                            [GLID],
                            [SLID],
                            [PBMID],
                            [SRMID],
                            [SEMID],
                            [Sign],[CardNo],[ChequeNo],[ChequeDate],[BankBranchName],
                            [SlipNo],[SlipDate],[ServiceChargeRate],[ServiceChargeAmount],
                            [Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [MP].[ID] AS [ModeOfPaymentID],
                            [D].[TypeID],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            --[PBM].[ID] AS [PBMID],
                            --[SRM].[ID] AS [SRMID],
                            --[SEM].[ID] AS [SEMID],
                            [D].[CardNo],[D].[Sign],[D].[ChequeNo],[D].[ChequeDate],[D].[BankBranchName],
                            [D].[SlipNo],[D].[SlipDate],[D].[ServiceChargeRate],[D].[ServiceChargeAmount],
                            [D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[8]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_ModeOfPayment] [MP] ON [D].[ModeOfPayment]=[MP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [D].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [PURCHASE].[TBL_PurchaseBillMaster] [PBM] ON [PBM].[ID]=[D].[MasterID] AND  [PBM].[Vno]=[D].[PurchaseBillVno]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [SRM] ON [SRM].[ID]=[D].[MasterID] AND  [SRM].[Vno]=[D].[SalesReturnVno]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [SEM] ON [SEM].[ID]=[D].[MasterID] AND  [SEM].[Vno]=[D].[SalesExpiryBreakageVno]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  

                        ----------------------------------------------------Printing Log---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaPrintingLog]([MasterID], [DesignID], [DesignName], [PrinterName], [PrintedbyID], [PrintedDateTime], [PageID])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[DesignID], [D].[DesignName],[D].[PrinterName],
                            [PBY].[ID] AS [PrintedbyID],
                            [D].[PrintedDateTime], [D].[PageID]
                        FROM {tempTableName[9]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [PBY] ON [D].[Printedby]=[PBY].[Name]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  

                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],
                            [TareQty],[GrossWeight],[WeighBridge_Weight],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[10]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[11]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesProformaDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[12]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesProformaMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE] 
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [GodownRaw] AS(
                            SELECT DISTINCT
                                [M].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GodownID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesProformaStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                        ),[Godown] AS(
                            SELECT DISTINCT
                            [GM].[{MainFieldName}] AS [GodownCodeName]
                            FROM [GodownRaw] [GR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GM] ON [GR].[GodownID]=[GM].[ID]
                            WHERE [GM].[{MainFieldName}] IS NOT NULL 
                        ),[AgentRaw] AS(
                            SELECT DISTINCT
                                [M].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AGID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AGID] IS NOT NULL  
                             
                        ),[Agent] AS(
                            SELECT DISTINCT
                            [AG].[{MainFieldName}] AS [AgentCodeName]
                            FROM [AgentRaw] [AGR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [AGR].[AGID]=[AG].[ID]
                            WHERE [AG].[{MainFieldName}] IS NOT NULL 
                        ),[SalesManRaw] AS(
                            SELECT DISTINCT
                                [M].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SMID] IS NOT NULL  
                             
                        ),[SalesMan] AS(
                            SELECT DISTINCT
                            [SM].[{MainFieldName}] AS [SalesManCodeName]
                            FROM [SalesManRaw] [SMR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [SMR].[SMID]=[SM].[ID]
                            WHERE [SM].[{MainFieldName}] IS NOT NULL 
                        ),[SalesPartnerRaw] AS(
                            SELECT DISTINCT
                                [M].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SPID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SPID] IS NOT NULL  
                             
                        ),[SalesPartner] AS(
                            SELECT DISTINCT
                            [SP].[{MainFieldName}] AS [SalesPartnerCodeName]
                            FROM [SalesPartnerRaw] [SPR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [SPR].[SPID]=[SP].[ID]
                            WHERE [SP].[{MainFieldName}] IS NOT NULL 
                        ),[LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSettlement] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[SubledgerRaw] AS(
                            SELECT DISTINCT
                                [M].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSettlement] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                        ),[Subledger] AS(
                            SELECT DISTINCT
                            [SL].[{MainFieldName}] AS [SubledgerCodeName]
                            FROM [SubledgerRaw] [SLR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [SLR].[SLID]=[SL].[ID]
                            WHERE [SL].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[AuditLockByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[AuditLockByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ReconcileByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ReconcileByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ClosedByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ClosedByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountPostUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[StockPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[StockPostUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PrintedbyID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesProformaPrintingLog] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PrintedbyID] IS NOT NULL
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[ProductRaw] AS(
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesProformaStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                        ),[Product] AS(
                                SELECT DISTINCT
                                [P].[{MainFieldName}] AS [ProductCodeName]
                                FROM [ProductRaw] [PR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [PR].[PID]=[P].[ID]
                                WHERE [P].[{MainFieldName}] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION 
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[CUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[CUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesProformaUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ),[DetailRaw] AS(
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[BatchDetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesProformaUOMDetails] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesProformaUOMDetails] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesProformaTerms] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                        ),[Detail] AS(
                                SELECT DISTINCT
                                [SOD].[Sno] AS [DetailSno]   
                                FROM [DetailRaw] [DR]
                                LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [SOD] ON [DR].[DetailID]=[SOD].[ID]
                                WHERE [SOD].[Sno] IS NOT NULL  

                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Godown' AS [ModuleName],  STRING_AGG(CAST( [GOD].[GodownCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Godown] [GOD]
                            WHERE [GOD].[GodownCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'LCMaster' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM  (
                                    --SELECT DISTINCT
                                        --[LCM].[Vno]
                                    --FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    --LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                                   -- WHERE [M].[LCMasterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LCM].[Vno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [SLD].[SubledgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Subledger] [SLD]
                            WHERE [SLD].[SubledgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [AGT].[AgentCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Agent] [AGT]
                            WHERE [AGT].[AgentCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [SMN].[SalesManCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesMan] [SMN]
                            WHERE [SMN].[SalesManCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [SPT].[SalesPartnerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesPartner] [SPT]
                            WHERE [SPT].[SalesPartnerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Guest' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [GM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_GuestMaster] [GM] ON [M].[GuestID]=[GM].[ID]
                                    WHERE [M].[GuestID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [GM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesProformaMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [PD].[ProductCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Product] [PD]
                            WHERE [PD].[ProductCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'ChallanDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[CD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [CD] ON [D].[ChallanID]=[CD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'DispatchOrderDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[DOD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [D].[DispatchOrderID]=[DOD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DOD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'OrderDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[OD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [OD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'SaudaDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SD].[Sno] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'QuotationDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[QD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [QD].[Sno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Batch' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[BAT].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaBatch] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BAT].[PID] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Serial' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SER].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesProformaSerial] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SER].[PID] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'PurchaseBill' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [SBM].[Vno]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[PURCHASE].[TBL_PurchaseBillMaster] [SBM] ON [D].[PBMID]=[SBM].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SBM].[Vno] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesReturn' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [SRM].[Vno]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [SRM] ON [D].[SRMID]=[SRM].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SRM].[Vno] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesExpiryBreakage' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [SEM].[Vno]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [SEM] ON [D].[SEMID]=[SEM].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SEM].[Vno] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Detail' AS ModuleName,  STRING_AGG(CAST( [DET].[DetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM Detail [DET]
                            WHERE [DET].[DetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesProformaTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesProformaDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesBill(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesBill",
                "#TempCopyMasterSalesBill2",
                "#TempCopyMasterSalesBill3",
                "#TempCopyMasterSalesBill4",
                "#TempCopyMasterSalesBill5",
                "#TempCopyMasterSalesBill6",
                "#TempCopyMasterSalesBill7",
                "#TempCopyMasterSalesBill8",
                "#TempCopyMasterSalesBill9",
                "#TempCopyMasterSalesBill10",
                "#TempCopyMasterSalesBill11",
                "#TempCopyMasterSalesBill12",
                "#TempCopyMasterSalesBill13",
                "#TempCopyMasterSalesBill14",
                "#TempCopyMasterSalesBill15",
                "#TempCopyMasterSalesBill16",
            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesBill ",
                " INTO #TempCopyMasterSalesBill2 ",
                " INTO #TempCopyMasterSalesBill3 ",
                " INTO #TempCopyMasterSalesBill4 ",
                " INTO #TempCopyMasterSalesBill5 ",
                " INTO #TempCopyMasterSalesBill6 ",
                " INTO #TempCopyMasterSalesBill7 ",
                " INTO #TempCopyMasterSalesBill8 ",
                " INTO #TempCopyMasterSalesBill9 ",
                " INTO #TempCopyMasterSalesBill10 ",
                " INTO #TempCopyMasterSalesBill11 ",
                " INTO #TempCopyMasterSalesBill12 ",
                " INTO #TempCopyMasterSalesBill13 ",
                " INTO #TempCopyMasterSalesBill14 ",
                " INTO #TempCopyMasterSalesBill15 ",
                " INTO #TempCopyMasterSalesBill16 ",

            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesBill",
                "@TempCopyMasterSalesBill2",
                "@TempCopyMasterSalesBill3",
                "@TempCopyMasterSalesBill4",
                "@TempCopyMasterSalesBill5",
                "@TempCopyMasterSalesBill6",
                "@TempCopyMasterSalesBill7",
                "@TempCopyMasterSalesBill8",
                "@TempCopyMasterSalesBill9",
                "@TempCopyMasterSalesBill10",
                "@TempCopyMasterSalesBill11",
                "@TempCopyMasterSalesBill12",
                "@TempCopyMasterSalesBill13",
                "@TempCopyMasterSalesBill14",
                "@TempCopyMasterSalesBill15",
                "@TempCopyMasterSalesBill16",

            };

                tempTableNameInsert = new[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesBill",
                "UTDT_TempCopyMasterSalesBill2",
                "UTDT_TempCopyMasterSalesBill3",
                "UTDT_TempCopyMasterSalesBill4",
                "UTDT_TempCopyMasterSalesBill5",
                "UTDT_TempCopyMasterSalesBill6",
                "UTDT_TempCopyMasterSalesBill7",
                "UTDT_TempCopyMasterSalesBill8",
                "UTDT_TempCopyMasterSalesBill9",
                "UTDT_TempCopyMasterSalesBill10",
                "UTDT_TempCopyMasterSalesBill11",
                "UTDT_TempCopyMasterSalesBill12",
                "UTDT_TempCopyMasterSalesBill13",
                "UTDT_TempCopyMasterSalesBill14",
                "UTDT_TempCopyMasterSalesBill15",
                "UTDT_TempCopyMasterSalesBill16",

                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill5;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill6;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill7;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill8;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill9;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill10;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill11;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill12;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill13;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill14;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill15;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesBill16;
                      

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[IsRetail],[M].[IsPan], 
                            [PT].[{MainFieldName}] AS [PromotionTerm],
                            [DT].[{MainFieldName}] AS [DiscountTerm],
                            [VT].[{MainFieldName}] AS [VatTerm],
                            [RT].[{MainFieldName}] AS [RoundOffTerm],
                            [M].[RefVno],  [M].[RefDate], [M].[TranDate],[M].[IsCash], 
                            [M].[BillingName], [M].[BillingVatPanNo], [M].[BillingAdress], [M].[BillingMobile], [M].[BillingEmail],  
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [M].[IsNotLocal],[M].[IsABT],
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID], [M].[BasicAmt], [M].[TAmt], [M].[BillRoundOffAmount],[M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[PrintNo], 
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [PT] ON [M].[PromotionTermID]=[PT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [DT] ON [M].[DiscountTermID]=[DT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [VT] ON [M].[VatTermID]=[VT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [RT] ON [M].[RoundOffTermID]=[RT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Master Add---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[EffDate], [D].[DueDays],  [D].[DueDate],
                            [D].[BillingCPerson], [D].[BillingAdress2], [D].[BillingCountryID], [D].[BillingPhone], [D].[BillingMobile2], 
                            [D].[ShippingCPerson],[D].[ShippingAdress], [D].[ShippingAdress2],  [D].[ShippingCountryID],
                            [D].[ShippingPhone],[D].[ShippingMobile], [D].[ShippingMobile2], [D].[ShippingEmail], 
                            [GD].[{MainFieldName}] AS [Godown],
                            [LCM].[Vno] AS [LCMasterVno],
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [D].[CurrencyID], [D].[CurRate],[D].[DCurRate],
                            [D].[TrackingNo], 
                            [AD].[Name] AS [AuditLockBy],
                            [D].[AuditLockDate], 
                            [RC].[Name] AS [ReconcileBy],
                            [D].[ReconcileDate], 
                            [D].[ApprovalStatusID], [D].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [D].[ApprovalStatusLastDate],[D].[ApprovalStatusUserRemarks],[D].[ApprovalStatusSystemRemarks],
                            [D].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [D].[AccountPostDate],[D].[IsAllowStockPost],
                            [SPU].[Name] AS [StockPostUser],
                            [D].[StockPostDate]

                            {tempTableNameInsert[1]}
                        FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [D].[LCMasterID]=[LCM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [D].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [D].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [D].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [D].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [AD] ON [D].[AuditLockByID]=[AD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [RC] ON [D].[ReconcileByID]=[RC].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [D].[ApprovalStatusUserID] = [ASU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [APU] ON [D].[AccountPostUserID] = [APU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [SPU] ON [D].[StockPostUserID] = [SPU].[ID]
                        
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            --[PD].[Sno] AS [ProformaDetailSno],
                            --[CD].[Sno] AS [ChallanDetailSno],
                            --[DOD].[Sno] AS [DispatchOrderDetailSno],
                            --[OD].[Sno] AS [OrderDetailSno],
                            --[SD].[Sno] AS [SaudaDetailSno],
                            --[QD].[Sno] AS [QuotationDetailSno],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesBillDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [PD] ON [D].[ProformaID]=[PD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [CD] ON [D].[ChallanID]=[CD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [D].[DispatchOrderID]=[DOD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GO].[{MainFieldName}] AS [Godown],
                            [D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesBillStoneDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GO] ON [D].[GodownID]=[GO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Batch---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[BAT].[PID] AS [Batch],   --here using PID column as there is no code or name field in BatchMaster
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesBillBatch] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Serial---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[BDT].[Sno] AS [BatchSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[SER].[PID] AS [Serial],   --here using PID column as there is no code or name field in SerialMaster
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[5]}
                        FROM  {DBName}[SALES].[TBL_SalesBillSerial] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SerialMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------FreeKit Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[DetailSno],[D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[6]}
                        FROM  {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------UOM Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[ID] AS [DetailID], --no Sno column, so using ID column
                            --[BDT].[ID] AS [BatchDetailID],  --no Sno column, so using ID column
                            [D].[CQty],
                            [CU].[Code] AS [CUOM],
                            [D].[UOMQty],
                            [UO].[Code] AS [UOM]
                        
                            {tempTableNameInsert[7]}
                        FROM  {DBName}[SALES].[TBL_SalesBillUOMDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [CU] ON [D].[CUOMID]=[CU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Retail Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            --[BAT].[PID] AS [Batch],   --here using PID column as there is no code or name field in BatchMaster
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[DiscountRate],[D].[DiscountAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[IncludeVatableAmt],[D].[IncludeVatRate],[D].[IncludeVatAmt],
                            --[CD].[Sno] AS [ChallanDetailSno],
                            --[DOD].[Sno] AS [DispatchOrderDetailSno],
                            --[OD].[Sno] AS [OrderDetailSno],
                            [WT].[{MainFieldName}] AS [Waiter],
                            [D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[8]}
                        FROM  {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [CD] ON [D].[ChallanID]=[CD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [D].[DispatchOrderID]=[DOD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [OD] ON [D].[OrderID]=[OD].[ID]
                        LEFT OUTER JOIN {DBName}[RESTRO].[TBL_Waiter] [WT] ON [D].[WaiterID]=[WT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Retail Detail Add---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[GrossQty],[D].[TareQty],
                            --[PD].[ID] AS [ProformaDetailID],
                            --[SD].[ID] AS [SaudaDetailID],
                            --[QD].[ID] AS [QuotationDetailID],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger]
                            {tempTableNameInsert[9]}
                        FROM  {DBName}[SALES].[TBL_SalesBillRetailDetailsAdd] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[ID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [PD] ON [D].[ProformaID]=[PD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SD] ON [D].[SaudaID]=[SD].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QD] ON [D].[QuotationID]=[QD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Commission---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [D].[TypeID],[D].[CommissionBasisID],
                            [AG].[{MainFieldName}] AS [Agent],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[10]}
                        FROM  {DBName}[SALES].[TBL_SalesBillCommission] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [D].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [D].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [D].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Settlement---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [MP].[{MainFieldName}] AS [ModeOfPayment],
                            [D].[TypeID],[D].[IsAdvance],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            --[PBM].[Vno] AS [PurchaseBillVno],
                            --[SRM].[Vno] AS [SalesReturnVno],
                            --[SEM].[Vno] AS [SalesExpiryBreakageVno],
                            --[SOM].[Vno] AS [SalesOrderVno],
                            [D].[Sign],[D].[CardNo],[D].[ChequeNo],[D].[ChequeDate],[D].[BankBranchName],
                            [D].[SlipNo],[D].[SlipDate],[D].[ServiceChargeRate],[D].[ServiceChargeAmount],
                            [D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[11]}
                        FROM  {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ModeOfPayment] [MP] ON [D].[ModeOfPaymentID]=[MP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[PURCHASE].[TBL_PurchaseBillMaster] [PBM] ON [D].[PBMID]=[PBM].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [SRM] ON [D].[SRMID]=[SRM].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [SEM] ON [D].[SEMID]=[SEM].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderMaster] [SOM] ON [D].[SOMID]=[SOM].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Printing Log---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[PrinterName],[D].[DesignID], [D].[DesignName],
                            [PBY].[Name] AS [Printedby],
                            [D].[PrintedDateTime], [D].[PageID]
                            {tempTableNameInsert[12]}
                        FROM  {DBName}[SALES].[TBL_SalesBillPrintingLog] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [PBY] ON [D].[PrintedbyID]=[PBY].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[13]}
                        FROM  {DBName}[SALES].[TBL_SalesBillOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[14]}
                        FROM  {DBName}[SALES].[TBL_SalesBillTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[15]}
                        FROM  {DBName}[SALES].[TBL_SalesBillDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $$""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales Bill ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesBillMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Bill [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesBillMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Bill [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesBillMaster] [T]) 


                        SET @smt=@smt + 'Sales Bill Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesBillMaster]
                        (
                            [VNo], [VDate], [VTime], [IsRetail],[IsPan], 
                            [PromotionTermID],
                            [DiscountTermID],
                            [VatTermID],
                            [RoundOffTermID],
                            [RefVno],[RefDate], [TranDate],[IsCash], 
                            [BillingName], [BillingVatPanNo], [BillingAdress], [BillingMobile], [BillingEmail],  
                            [GLID],
                            [AccountGLID], 
                            [SLID],
                            [AGID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [IsNotLocal], [IsABT],
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID], [BasicAmt], [TAmt], [BillRoundOffAmount],[NetAmt],
                            [INotes], [Remarks], [PrintNo], 
                            [CancelByID],
                            [CancelDate], [CancelReason],
                            [FYID], 
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[IsRetail],[M].[IsPan], 
                            [PT].[ID] AS [PromotionTermID],
                            [DT].[ID] AS [DiscountTermID],
                            [VT].[ID] AS [VatTermID],
                            [RT].[ID] AS [RoundOffTermID],
                            [M].[RefVno],  [M].[RefDate], [M].[TranDate],[M].[IsCash], 
                            [M].[BillingName], [M].[BillingVatPanNo], [M].[BillingAdress], [M].[BillingMobile], [M].[BillingEmail],  
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [M].[IsNotLocal], [M].[IsABT],
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID], [M].[BasicAmt], [M].[TAmt], [M].[BillRoundOffAmount],[M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[PrintNo], 
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [PT] ON [M].[PromotionTerm]=[PT].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [DT] ON [M].[DiscountTerm]=[DT].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [VT] ON [M].[VatTerm]=[VT].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [RT] ON [M].[RoundOffTerm]=[RT].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 

                        ----------------------------------------------------Master Add---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillMasterAdd]
                        (
                            [EffDate], [DueDays],  [DueDate],
                            [BillingCPerson], [BillingAdress2], [BillingCountryID], [BillingPhone], [BillingMobile2], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GodownID],
                            [LCMasterID],
                            [PartyBranchID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [GoodsGLID],
                            [GoodsSLID],
                            [CurrencyID], [CurRate],[DCurRate],
                            [TrackingNo], 
                            [AuditLockByID],
                            [AuditLockDate], 
                            [ReconcileByID],
                            [ReconcileDate], 
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],
                            [IsAllowAccountPost],
                            [AccountPostUserID],
                            [AccountPostDate],[IsAllowStockPost],
                            [StockPostUserID],
                            [StockPostDate]

                        )

                        SELECT 
                            [D].[EffDate], [D].[DueDays],  [D].[DueDate],
                            [D].[BillingCPerson], [D].[BillingAdress2], [D].[BillingCountryID], [D].[BillingPhone], [D].[BillingMobile2], 
                            [D].[ShippingCPerson],[D].[ShippingAdress], [D].[ShippingAdress2],  [D].[ShippingCountryID],
                            [D].[ShippingPhone],[D].[ShippingMobile], [D].[ShippingMobile2], [D].[ShippingEmail], 
                            [GD].[ID] AS [GodownID],
                            [LCM].[ID] AS [LCMasterID],
                            [PBR].[ID] AS [PartyBranchID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [D].[CurrencyID], [D].[CurRate],[D].[DCurRate],
                            [D].[TrackingNo], 
                            [AD].[ID] AS [AuditLockByID],
                            [D].[AuditLockDate], 
                            [RC].[ID] AS [ReconcileByID],
                            [D].[ReconcileDate], 
                            [D].[ApprovalStatusID], [D].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [D].[ApprovalStatusLastDate],[D].[ApprovalStatusUserRemarks],[D].[ApprovalStatusSystemRemarks],
                            [D].[IsAllowAccountPost],
                            [APU].[ID] AS [AccountPostUserID],
                            [D].[AccountPostDate],[D].[IsAllowStockPost],
                            [SPU].[ID] AS [StockPostUserID],
                            [D].[StockPostDate]

                        FROM {tempTableName[1]} [D]
                        --INNER JOIN @TempDataCodeName [TD] ON [D].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [FIN].[TBL_LCMaster] [LCM] ON [LCM].[MasterID]=[D].[ID] AND [D].[LCMasterVno]=[LCM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [D].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [D].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [D].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [D].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [AD] ON [D].[AuditLockBy]=[AD].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [RC] ON [D].[ReconcileByID]=[RC].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [D].[ApprovalStatusUser] = [ASU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [APU] ON [D].[AccountPostUser] = [APU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [SPU] ON [D].[StockPostUser] = [SPU].[Name]
                        --INNER JOIN @DataList [DL] ON [D].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [GrossQty],[TareQty],
                            [ProformaID],
                            [ChallanID],
                            [DispatchOrderID],
                            [OrderID],
                            [SaudaID],
                            [QuotationID],
                            [PCSOnly],[FreePCSOnly],
                            [GoodsGLID],[GoodsSLID],
                            [IncludeVatRate],[IncludeVatAmount],
                            [Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            [PD].[ID] AS [ProformaID],
                            [CD].[ID] AS [ChallanID],
                            [DOD].[ID] AS [DispatchOrderID],
                            [OD].[ID] AS [OrderID],
                            [SD].[ID] AS [SaudaID],
                            [QD].[ID] AS [QuotationID],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [PD] ON [PD].[MasterID]=[D].[ID] AND  [PD].[Sno]=[D].[ProformaDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [CD] ON [CD].[MasterID]=[D].[ID] AND  [CD].[Sno]=[D].[ChallanDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [DOD].[MasterID]=[D].[ID] AND  [DOD].[Sno]=[D].[DispatchOrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [OD] ON [OD].[MasterID]=[D].[ID] AND  [OD].[Sno]=[D].[OrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesSaudaDetails] [SD] ON [SD].[MasterID]=[D].[ID] AND  [SD].[Sno]=[D].[SaudaDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[D].[ID] AND  [QD].[Sno]=[D].[QuotationDetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillStoneDetails]([MasterID],[DetailID],[Sno],[PID],[GodownID],[Qty],[UOMID],[PCS],[Rate],[Amount],[Descriptions],[TrackingNo])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GO].[ID] AS [GodownID],
                            [D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GO] ON [D].[Godown]=[GO].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Batch---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillBatch]
                        (
                            [MasterID],
                            [DetailID],
                            [FreeKitDetailID],
                            [Sno],
                            [PID],
                            [BatchID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],[Rate],
                            [BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[FreePCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [BAT].[ID] AS [BatchID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],
                            [D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BAT] ON [BAT].[ID]=[D].[ID] AND  [BAT].[Batch]=[D].[Batch]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Serial---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillSerial]
                        (
                            [MasterID],
                            [DetailID], 
                            [BatchDetailID], 
                            [FreeKitDetailID], 
                            [TypeID],[Sno],
                            [PID],
                            [SerialID],
                            [MFGDate],[EXPDate],[InstallationDate],[S1],
                            [S3],[S4],[S5],[S6],[S7],[S8],
                            [S9],[S10],[AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [SER].[ID] AS [SerialID],
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[5]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[Sno]=[D].[BatchSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_SerialMaster] [SER] ON [SER].[ID]=[D].[ID] AND  [SER].[Serial]=[D].[Serial]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------FreeKit Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillFreeKitDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [DetailSno],[TypeID],[Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[RateBasisID],[BasicAmt],[GrossQty],
                            [TareQty],[PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[DetailSno],[D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[6]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------UOM Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillUOMDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [BatchDetailID],
                            [CQty],
                            [CUOMID],
                            [UOMQty],
                            [UOMID]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID],
                            [D].[CQty],
                            [CU].[ID] AS [CUOMID],
                            [D].[UOMQty],
                            [UO].[ID] AS [UOMID]
                        FROM {tempTableName[7]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[ID]=[D].[DetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[ID]=[D].[BatchDetailID]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [CU] ON [D].[CUOM]=[CU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID]

                        ----------------------------------------------------Retail Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillRetailDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [BatchID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [Rate],[RateBasisID],
                            [BasicAmt],[DiscountRate],[DiscountAmt],[TAmt],[NetAmt],
                            [IncludeVatableAmt],[IncludeVatRate],[IncludeVatAmt],
                            [ChallanID],
                            [DispatchOrderID],
                            [OrderID],
                            [WaiterID],
                            [Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [BAT].[ID] AS [BatchID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[DiscountRate],[D].[DiscountAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[IncludeVatableAmt],[D].[IncludeVatRate],[D].[IncludeVatAmt],
                            [CD].[ID] AS [ChallanID],
                            [DOD].[ID] AS [DispatchOrderID],
                            [OD].[ID] AS [OrderID],
                            [WT].[ID] AS [WaiterID],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[8]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BAT] ON [BAT].[ID]=[D].[ID] AND  [BAT].[Batch]=[D].[Batch]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesChallanDetails] [CD] ON [CD].[MasterID]=[D].[ID] AND  [CD].[Sno]=[D].[ChallanDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesDispatchOrderDetails] [DOD] ON [DOD].[MasterID]=[D].[ID] AND  [DOD].[Sno]=[D].[DispatchOrderDetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesOrderDetails] [OD] ON [OD].[MasterID]=[D].[ID] AND  [OD].[Sno]=[D].[OrderDetailSno]
                        LEFT OUTER JOIN [RESTRO].[TBL_Waiter] [WT] ON [D].[Waiter]=[WT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Retail Detail Add---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillRetailDetailsAdd]
                        (
                            [ID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [GrossQty],[TareQty],
                            [ProformaID],
                            [SaudaID],
                            [QuotationID],
                            [PCSOnly],[FreePCSOnly],
                            [GoodsGLID],[GoodsSLID]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[GrossQty],[D].[TareQty],
                            [PD].[ID] AS [ProformaID],
                            [SD].[ID] AS [SaudaID],
                            [QD].[ID] AS [QuotationID],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID]
                        FROM {tempTableName[9]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesProformaDetails] [PD] ON [PD].[MasterID]=[D].[ID] AND  [PD].[ID]=[D].[ProformaDetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesSaudaDetails] [SD] ON [SD].[MasterID]=[D].[ID] AND  [SD].[ID]=[D].[SaudaDetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesQuotationDetails] [QD] ON [QD].[MasterID]=[D].[ID] AND  [QD].[ID]=[D].[QuotationDetailID]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Commission---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillCommission]
                        (
                            [MasterID],
                            [DetailID],
                            [TypeID],[CommissionBasisID],
                            [AGID],
                            [SPID],
                            [SMID],
                            [GLID],
                            [SLID],
                            [Rate],[Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[TypeID],[D].[CommissionBasisID],
                            [AG].[ID] AS [AGID],
                            [SP].[ID] AS [SPID],
                            [SM].[ID] AS [SMID],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[10]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Rate]=[D].[DetailRate]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [D].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [D].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [D].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Rate]

                        ----------------------------------------------------Settlement---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillSettlement]
                        (
                            [MasterID], 
                            [ModeOfPaymentID],
                            [TypeID],[IsAdvance],
                            [GLID],
                            [SLID],
                            [PBMID],
                            [SRMID],
                            [SEMID],
                            [SOMID],
                            [Sign],[CardNo],[ChequeNo],[ChequeDate],[BankBranchName],
                            [SlipNo],[SlipDate],[ServiceChargeRate],[ServiceChargeAmount],
                            [Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [MP].[ID] AS [ModeOfPaymentID],
                            [D].[TypeID],[D].[IsAdvance],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            --[PBM].[ID] AS [PBMID],
                            --[SRM].[ID] AS [SRMID],
                            --[SEM].[ID] AS [SEMID],
                            --[SOM].[ID] AS [SOMID],
                            [D].[CardNo],[D].[Sign],[D].[ChequeNo],[D].[ChequeDate],[D].[BankBranchName],
                            [D].[SlipNo],[D].[SlipDate],[D].[ServiceChargeRate],[D].[ServiceChargeAmount],
                            [D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[11]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_ModeOfPayment] [MP] ON [D].[ModeOfPayment]=[MP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [D].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [PURCHASE].[TBL_PurchaseBillMaster] [PBM] ON [PBM].[ID]=[D].[MasterID] AND  [PBM].[Vno]=[D].[PurchaseBillVno]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [SRM] ON [SRM].[ID]=[D].[MasterID] AND  [SRM].[Vno]=[D].[SalesReturnVno]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [SEM] ON [SEM].[MasterID]=[D].[ID] AND  [SEM].[Vno]=[D].[SalesExpiryBreakageVno]
                        LEFT OUTER JOIN [SALES].[TBL_SalesOrderMaster] [SOM] ON [SOM].[ID]=[D].[MasterID] AND  [SOM].[Vno]=[D].[SalesOrderVno]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  

                        ----------------------------------------------------Printing Log---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillPrintingLog]([MasterID], [PrinterName], [DesignID], [DesignName], [PrintedbyID], [PrintedDateTime], [PageID])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[PrinterName],[D].[DesignID], [D].[DesignName],
                            [PBY].[ID] AS [PrintedbyID],
                            [D].[PrintedDateTime], [D].[PageID]
                        FROM {tempTableName[12]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [PBY] ON [D].[Printedby]=[PBY].[Name]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  

                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],
                            [TareQty],[GrossWeight],[WeighBridge_Weight],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[13]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[14]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesBillDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[15]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE] 
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [TermRaw] AS(
                            SELECT DISTINCT
                                [M].[PromotionTermID] AS [TermID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[PromotionTermID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[DiscountTermID] AS [TermID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[DiscountTermID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[VatTermID] AS [TermID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[VatTermID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[RoundOffTermID] AS [TermID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[RoundOffTermID] IS NOT NULL
                        ),[Term] AS(
                            SELECT DISTINCT
                            [BT].[{MainFieldName}] AS [TermCodeName]
                            FROM [TermRaw] [TR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [TR].[TermID]=[BT].[ID]
                            WHERE [BT].[{MainFieldName}] IS NOT NULL 
                        ),[GodownRaw] AS(
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesBillStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                        ),[Godown] AS(
                            SELECT DISTINCT
                            [GM].[{MainFieldName}] AS [GodownCodeName]
                            FROM [GodownRaw] [GR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GM] ON [GR].[GodownID]=[GM].[ID]
                            WHERE [GM].[{MainFieldName}] IS NOT NULL 
                        ),[AgentRaw] AS(
                            SELECT DISTINCT
                                [M].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AGID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesBillCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AGID] IS NOT NULL       
                        ),[Agent] AS(
                            SELECT DISTINCT
                            [AG].[{MainFieldName}] AS [AgentCodeName]
                            FROM [AgentRaw] [AGR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [AGR].[AGID]=[AG].[ID]
                            WHERE [AG].[{MainFieldName}] IS NOT NULL 
                        ),[SalesManRaw] AS(
                            SELECT DISTINCT
                                [D].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesBillCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SMID] IS NOT NULL    
                        ),[SalesMan] AS(
                            SELECT DISTINCT
                            [SM].[{MainFieldName}] AS [SalesManCodeName]
                            FROM [SalesManRaw] [SMR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [SMR].[SMID]=[SM].[ID]
                            WHERE [SM].[{MainFieldName}] IS NOT NULL 
                        ),[SalesPartnerRaw] AS(
                            SELECT DISTINCT
                                [D].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SPID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesBillCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SPID] IS NOT NULL  
                             
                        ),[SalesPartner] AS(
                            SELECT DISTINCT
                            [SP].[{MainFieldName}] AS [SalesPartnerCodeName]
                            FROM [SalesPartnerRaw] [SPR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [SPR].[SPID]=[SP].[ID]
                            WHERE [SP].[{MainFieldName}] IS NOT NULL 
                        ),[ProformaDetailRaw] AS(
                            SELECT DISTINCT
                                [D].[ProformaID] AS [ProformaID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[ProformaID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[ProformaID] AS [ProformaID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetailsAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[ProformaID] IS NOT NULL  
                        ),[ProformaDetail] AS(
                            SELECT DISTINCT
                            [PDT].[Sno] AS [ProformaDetailSno]
                            FROM [ProformaDetailRaw] [PDR]
                            LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesProformaDetails] [PDT] ON [PDR].[ProformaID]=[PDT].[ID]
                            WHERE [PDT].[Sno] IS NOT NULL 
                        ),[ChallanDetailRaw] AS(
                            SELECT DISTINCT
                                [D].[ChallanID] AS [ChallanID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[ChallanID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[ChallanID] AS [ChallanID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[ChallanID] IS NOT NULL  
                        ),[ChallanDetail] AS(
                            SELECT DISTINCT
                            [CDT].[Sno] AS [ChallanDetailSno]
                            FROM [ChallanDetailRaw] [CDR]
                            LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesChallanDetails] [CDT] ON [CDR].[ChallanID]=[CDT].[ID]
                            WHERE [CDT].[Sno] IS NOT NULL 
                        ),[DispatchOrderDetailRaw] AS(
                            SELECT DISTINCT
                                [D].[DispatchOrderID] AS [DispatchOrderID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DispatchOrderID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DispatchOrderID] AS [DispatchOrderID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DispatchOrderID] IS NOT NULL  
                        ),[DispatchOrderDetail] AS(
                            SELECT DISTINCT
                            [DDT].[Sno] AS [DispatchOrderDetailSno]
                            FROM [DispatchOrderDetailRaw] [DODR]
                            LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesDispatchOrderDetails] [DDT] ON [DODR].[DispatchOrderID]=[DDT].[ID]
                            WHERE [DDT].[Sno] IS NOT NULL 
                        ),[OrderDetailRaw] AS(
                            SELECT DISTINCT
                                [D].[OrderID] AS [OrderID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[OrderID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[OrderID] AS [OrderID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[OrderID] IS NOT NULL  
                        ),[OrderDetail] AS(
                            SELECT DISTINCT
                            [ODT].[Sno] AS [OrderDetailSno]
                            FROM [OrderDetailRaw] [ODR]
                            LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [ODT] ON [ODR].[OrderID]=[ODT].[ID]
                            WHERE [ODT].[Sno] IS NOT NULL 
                        ),[SaudaDetailRaw] AS(
                            SELECT DISTINCT
                                [D].[SaudaID] AS [SaudaID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SaudaID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SaudaID] AS [SaudaID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetailsAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SaudaID] IS NOT NULL  
                        ),[SaudaDetail] AS(
                            SELECT DISTINCT
                            [SDT].[Sno] AS [SaudaDetailSno]
                            FROM [SaudaDetailRaw] [SDR]
                            LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesSaudaDetails] [SDT] ON [SDR].[SaudaID]=[SDT].[ID]
                            WHERE [SDT].[Sno] IS NOT NULL 
                        ),[QuotationDetailRaw] AS(
                            SELECT DISTINCT
                                [D].[QuotationID] AS [QuotationID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[QuotationID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[QuotationID] AS [QuotationID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetailsAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[QuotationID] IS NOT NULL  
                        ),[QuotationDetail] AS(
                            SELECT DISTINCT
                            [QDT].[Sno] AS [QuotationDetailSno]
                            FROM [QuotationDetailRaw] [QDR]
                            LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesQuotationDetails] [QDT] ON [QDR].[QuotationID]=[QDT].[ID]
                            WHERE [QDT].[Sno] IS NOT NULL 
                        ),[LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetailsAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[SubledgerRaw] AS(
                            SELECT DISTINCT
                                [M].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [M]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesBillCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                        ),[Subledger] AS(
                            SELECT DISTINCT
                            [SL].[{MainFieldName}] AS [SubledgerCodeName]
                            FROM [SubledgerRaw] [SLR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [SLR].[SLID]=[SL].[ID]
                            WHERE [SL].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CancelByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AuditLockByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [D].[AuditLockByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[ReconcileByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [D].[ReconcileByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[ApprovalStatusUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AccountPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AccountPostUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[StockPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                            WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[StockPostUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PrintedbyID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesBillPrintingLog] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PrintedbyID] IS NOT NULL
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[ProductRaw] AS(
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesBillStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesBillSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                        ),[Product] AS(
                                SELECT DISTINCT
                                [P].[{MainFieldName}] AS [ProductCodeName]
                                FROM [ProductRaw] [PR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [PR].[PID]=[P].[ID]
                                WHERE [P].[{MainFieldName}] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION 
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[CUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[CUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ),[DetailRaw] AS(
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesBillTerms] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                        ),[Detail] AS(
                                SELECT DISTINCT
                                [SOD].[Sno] AS [DetailSno]   
                                FROM [DetailRaw] [DR]
                                LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [SOD] ON [DR].[DetailID]=[SOD].[ID]
                                WHERE [SOD].[Sno] IS NOT NULL  
                        ),[BatchRaw] AS(
                            SELECT DISTINCT
                                [D].[BatchID] AS [BatchID]
                            FROM {DBName}[SALES].[TBL_SalesBillBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchID] AS [BatchID]
                            FROM {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchID] IS NOT NULL  
                        ),[Batch] AS(
                            SELECT DISTINCT
                            [BTM].[PID] AS [Batch]
                            FROM [BatchRaw] [BTR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BTM] ON [BTR].[BatchID]=[BTM].[ID]
                            WHERE [BTM].[PID] IS NOT NULL 
                      
                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Godown' AS [ModuleName],  STRING_AGG(CAST( [GOD].[GodownCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Godown] [GOD]
                            WHERE [GOD].[GodownCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'LCMaster' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM  (
                                    --SELECT DISTINCT
                                        --[LCM].[Vno]
                                    --FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                                    --LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [D].[LCMasterID]=[LCM].[ID]
                                   -- WHERE [M].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LCM].[Vno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [SLD].[SubledgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Subledger] [SLD]
                            WHERE [SLD].[SubledgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [D].[PartyBranchID]=[PR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [AGT].[AgentCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Agent] [AGT]
                            WHERE [AGT].[AgentCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [SMN].[SalesManCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesMan] [SMN]
                            WHERE [SMN].[SalesManCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [SPT].[SalesPartnerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesPartner] [SPT]
                            WHERE [SPT].[SalesPartnerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMasterAdd] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [D].[ARID]=[AR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesBillMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [PD].[ProductCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Product] [PD]
                            WHERE [PD].[ProductCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'ProformaDetail' AS [ModuleName],  STRING_AGG(CAST( [PDT].[ProformaDetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [ProformaDetail] [PDT]
                            WHERE [PDT].[ProformaDetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'ChallanDetail' AS [ModuleName],  STRING_AGG(CAST( [CDT].[ChallanDetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [ChallanDetail] [CDT]
                            WHERE [CDT].[ChallanDetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'DispatchOrderDetail' AS [ModuleName],  STRING_AGG(CAST( [DODT].[DispatchOrderDetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [DispatchOrderDetail] [DODT]
                            WHERE [DODT].[DispatchOrderDetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'OrderDetail' AS [ModuleName],  STRING_AGG(CAST( [ODT].[OrderDetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [OrderDetail] [ODT]
                            WHERE [ODT].[OrderDetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SaudaDetail' AS [ModuleName],  STRING_AGG(CAST( [SDT].[SaudaDetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SaudaDetail] [SDT]
                            WHERE [SDT].[SaudaDetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'QuotationDetail' AS [ModuleName],  STRING_AGG(CAST( [QDT].[QuotationDetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [QuotationDetail] [QDT]
                            WHERE [QDT].[QuotationDetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Serial' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SER].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesBillSerial] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SER].[PID] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Waiter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [WTR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesBillRetailDetails] [D]
                                    LEFT OUTER JOIN {DBName}[RESTRO].[TBL_Waiter] [WTR] ON [D].[WaiterID]=[WTR].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [WTR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'ModeOfPayment' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [MOP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ModeOfPayment] [MOP] ON [D].[ModeOfPaymentID]=[MOP].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [MOP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'PurchaseBill' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [SBM].[Vno]
                                    FROM  {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[PURCHASE].[TBL_PurchaseBillMaster] [SBM] ON [D].[PBMID]=[SBM].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SBM].[Vno] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesReturn' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [SRM].[Vno]
                                    FROM  {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [SRM] ON [D].[SRMID]=[SRM].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SRM].[Vno] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'SalesExpiryBreakage' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [SEM].[Vno]
                                    FROM  {DBName}[SALES].[TBL_SalesBillSettlement] [D]
                                    LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [SEM] ON [D].[SEMID]=[SEM].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SEM].[Vno] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesBillOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesBillOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesBillOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Detail' AS ModuleName,  STRING_AGG(CAST( [DET].[DetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM Detail [DET]
                            WHERE [DET].[DetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesBillTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesBillDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesReturn(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesReturn",
                "#TempCopyMasterSalesReturn2",
                "#TempCopyMasterSalesReturn3",
                "#TempCopyMasterSalesReturn4",
                "#TempCopyMasterSalesReturn5",
                "#TempCopyMasterSalesReturn6",
                "#TempCopyMasterSalesReturn7",
                "#TempCopyMasterSalesReturn8",
                "#TempCopyMasterSalesReturn9",
                "#TempCopyMasterSalesReturn10",
                "#TempCopyMasterSalesReturn11",

            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesReturn ",
                " INTO #TempCopyMasterSalesReturn2 ",
                " INTO #TempCopyMasterSalesReturn3 ",
                " INTO #TempCopyMasterSalesReturn4 ",
                " INTO #TempCopyMasterSalesReturn5 ",
                " INTO #TempCopyMasterSalesReturn6 ",
                " INTO #TempCopyMasterSalesReturn7 ",
                " INTO #TempCopyMasterSalesReturn8 ",
                " INTO #TempCopyMasterSalesReturn9 ",
                " INTO #TempCopyMasterSalesReturn10 ",
                " INTO #TempCopyMasterSalesReturn11 ",


            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesReturn",
                "@TempCopyMasterSalesReturn2",
                "@TempCopyMasterSalesReturn3",
                "@TempCopyMasterSalesReturn4",
                "@TempCopyMasterSalesReturn5",
                "@TempCopyMasterSalesReturn6",
                "@TempCopyMasterSalesReturn7",
                "@TempCopyMasterSalesReturn8",
                "@TempCopyMasterSalesReturn9",
                "@TempCopyMasterSalesReturn10",
                "@TempCopyMasterSalesReturn11",


            };

                tempTableNameInsert = new[] { "", "", "", "", "", "", "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesReturn",
                "UTDT_TempCopyMasterSalesReturn2",
                "UTDT_TempCopyMasterSalesReturn3",
                "UTDT_TempCopyMasterSalesReturn4",
                "UTDT_TempCopyMasterSalesReturn5",
                "UTDT_TempCopyMasterSalesReturn6",
                "UTDT_TempCopyMasterSalesReturn7",
                "UTDT_TempCopyMasterSalesReturn8",
                "UTDT_TempCopyMasterSalesReturn9",
                "UTDT_TempCopyMasterSalesReturn10",
                "UTDT_TempCopyMasterSalesReturn11",


                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn5;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn6;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn7;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn8;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn9;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn10;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesReturn11;
                
                      

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[EffDate],[M].[RefVno],  [M].[RefDate],[M].[TranDate], [M].[DueDays],  [M].[DueDate],
                            --[BM].[Vno] AS [BillVno], 
                            [M].[IsCash], [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[{MainFieldName}] AS [Godown],
                            [LCM].[Vno] AS [LCMasterVno],
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [M].[IsNotLocal],[M].[IsABT],
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate],[M].[DCurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[Name] AS [AuditLockBy],
                            [M].[AuditLockDate], 
                            [RC].[Name] AS [ReconcileBy],
                            [M].[ReconcileDate], 
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],
                            [TBM].[{MainFieldName}] AS [Table],
                            [HRM].[{MainFieldName}] AS [HotelRoom],
                            [HBM].[{MainFieldName}] AS [HotelBed],
                            [M].[HospitalRoomID], [M].[HospitalBedID],
                            [GSM].[{MainFieldName}] AS [Guest],
                            [PTM].[{MainFieldName}] AS [Patient],
                            [M].[LocalPaxNo_Adult], [M].[LocalPaxNo_Child], [M].[ForeignPaxNo_Adult], [M].[ForeignPaxNo_Child],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [M].[AccountPostDate],[M].[IsAllowStockPost],
                            [SPU].[Name] AS [StockPostUser],
                            [M].[StockPostDate],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [BM] ON [M].[BillID]=[BM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [M].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [M].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [M].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [AD] ON [M].[AuditLockByID]=[AD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [RC] ON [M].[ReconcileByID]=[RC].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[RESTRO].[TBL_TableMaster] [TBM] ON [M].[TableID]=[TBM].[ID]
                        LEFT OUTER JOIN {DBName}[HMS].[TBL_Room] [HRM] ON [M].[HotelRoomID]=[HRM].[ID]
                        LEFT OUTER JOIN {DBName}[HMS].[TBL_Bed] [HBM] ON [M].[HotelBedID]=[HBM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GuestMaster] [GSM] ON [M].[GuestID]=[GSM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PatientMaster] [PTM] ON [M].[PatientID]=[PTM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [APU] ON [M].[AccountPostUserID] = [APU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUserID] = [SPU].[ID]
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            --[BD].[Sno] AS [BillDetailSno],
                         
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],[D].[StockRate],
                            [D].[Weight],[D].[WastagePercentage],[D].[Wastage],
                            [D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [WTR].[{MainFieldName}] AS [Waiter],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [BD] ON [D].[BillID]=[BD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[RESTRO].[TBL_Waiter] [WTR] ON [D].[WaiterID]=[WTR].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GO].[{MainFieldName}] AS [Godown],
                            [D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnStoneDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GO] ON [D].[GodownID]=[GO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Batch---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[BAT].[PID] AS [Batch],   --here using PID column as there is no code or name field in BatchMaster
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],[D].[BasicAmt],[D].[StockRate],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Serial---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[BDT].[Sno] AS [BatchSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[SER].[PID] AS [Serial],   --here using PID column as there is no code or name field in SerialMaster
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SerialMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------FreeKit Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[DetailSno] AS [SnoDetail],[D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[5]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------UOM Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[ID] AS [DetailID], --no Sno column, so putting other column like ID
                            --[BDT].[ID] AS [BatchDetailID],  --no Sno column, so putting other column like ID
                            [D].[CQty],
                            [CU].[Code] AS [CUOM],
                            [D].[UOMQty],
                            [UO].[Code] AS [UOM]
                        
                            {tempTableNameInsert[6]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnUOMDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [CU] ON [D].[CUOMID]=[CU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Commission---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [D].[TypeID],[D].[CommissionBasisID],
                            [AG].[{MainFieldName}] AS [Agent],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[7]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [D].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [D].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [D].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[8]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[9]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[10]}
                        FROM  {DBName}[SALES].[TBL_SalesReturnDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $$""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales Return ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesReturnMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales Return [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesReturnMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales Return [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesReturnMaster] [T]) 


                        SET @smt=@smt + 'Sales Return Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesReturnMaster]
                        (
                            [VNo], [VDate], [VTime], [EffDate],[RefVno],  [RefDate],[M].[TranDate], [DueDays],  [DueDate],  [BillID], [IsCash], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GodownID],
                            [LCMasterID],
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [GoodsGLID],
                            [GoodsSLID],
                            [IsNotLocal],[IsABT],
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate],[DCurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo], [PrintNo], 
                            [AuditLockDate],
                            [ReconcileByID],
                            [ReconcileDate],
                            [CancelByID],
                            [CancelDate], [CancelReason],
                            [TableID],
                            [HotelRoomID],
                            [HotelBedID],
                            [HospitalRoomID],[HospitalBedID],
                            [GuestID],
                            [PatientID],
                            [LocalPaxNo_Adult], [LocalPaxNo_Child], [ForeignPaxNo_Adult], [ForeignPaxNo_Child],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],
                            [IsAllowAccountPost],[AccountPostUserID],[AccountPostDate],
                            [IsAllowStockPost],[StockPostUserID],[StockPostDate],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[EffDate],[M].[RefVno], [M].[RefDate],[M].[TranDate], [M].[DueDays],  [M].[DueDate],  
                            [BM].[ID] AS [BillID],  
                            [M].[IsCash], [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[ID] AS [GodownID],
                            [LCM].[ID] AS [LCMasterID],
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [M].[IsNotLocal], [M].[IsABT],
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate],[M].[DCurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt], 
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[ID] AS [AuditLockByID],
                            [M].[AuditLockDate], 
                            [RC].[ID] AS [ReconcileByID],
                            [M].[ReconcileDate],                           
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],
                            [TBM].[ID] AS [TableID],
                            [HRM].[ID] AS [HotelRoomID],
                            [HBM].[ID] AS [HotelBedID],
                            [M].[HospitalRoomID], [M].[HospitalBedID],
                            [GSM].[ID] AS [GuestID],
                            [PTM].[ID] AS [PatientID],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [M].[AccountPostDate],[M].[IsAllowStockPost],
                            [SPU].[ID] AS [StockPostUserID],
                            [M].[StockPostDate],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [BM] ON [BillVno]=[BM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [M].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [FIN].[TBL_LCMaster] [LCM] ON [LCM].[MasterID]=[M].[ID] AND [M].[LCMasterVno]=[LCM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [M].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [M].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [RESTRO].[TBL_TableMaster] [TBM] ON [M].[Table]=[TBM].[{MainFieldName}]
                        LEFT OUTER JOIN [HMS].[TBL_Room] [HRM] ON [M].[HotelRoom]=[HRM].[{MainFieldName}]
                        LEFT OUTER JOIN [HMS].[TBL_Bed] [HBM] ON [M].[HotelBed]=[HBM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GuestMaster] [GSM] ON [M].[Guest]=[GSM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PatientMaster] [PTM] ON [M].[Patient]=[PTM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [AD] ON [M].[AuditLockBy]=[AD].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [RC] ON [M].[ReconcileBy]=[RC].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CLO] ON [M].[ClosedBy] = [CLO].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [APU] ON [M].[AccountPostUser] = [APU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUser] = [SPU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [GrossQty],[TareQty],
                            [BillID],
                            [PCSOnly],[FreePCSOnly],
                            [GoodsGLID],[GoodsSLID],
                            [IncludeVatRate],[IncludeVatAmount],[StockRate],
                            [Weight],[WastagePercentage],[Wastage],[MetalAmt],
                            [StoneQty],[StoneAmt],[WaiterID],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            [BD].[ID] AS [BillID],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],[D].[StockRate],
                            [D].[Weight],[D].[WastagePercentage],[D].[Wastage],
                            [D].[MetalAmt],[D].[StoneQty],[D].[StoneAmt],
                            [WTR].[ID] AS [WaiterID],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [BD] ON [BD].[MasterID]=[M].[ID] AND  [BD].[Sno]=[D].[BillDetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [RESTRO].[TBL_Waiter] [WTR] ON [D].[GoodsSubledger]=[WTR].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Stone Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnStoneDetails]([MasterID],[DetailID],[Sno],[PID],[GodownID],[Qty],[UOMID],[PCS],[Rate],[Amount],[Descriptions],[TrackingNo])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GO].[ID] AS [GodownID],
                            [D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[PCS],[D].[Rate],[D].[Amount],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[2]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GO] ON [D].[Godown]=[GO].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Batch---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnBatch]
                        (
                            [MasterID],
                            [DetailID],
                            [FreeKitDetailID],
                            [Sno],
                            [PID],
                            [BatchID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],[Rate],
                            [BasicAmt],[StockRate],[GrossQty],[TareQty],
                            [PCSOnly],[FreePCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [BAT].[ID] AS [BatchID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],
                            [D].[BasicAmt],[D].[StockRate],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BAT] ON [BAT].[Batch]=[D].[Batch]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Serial---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnSerial]
                        (
                            [MasterID],
                            [DetailID], 
                            [BatchDetailID], 
                            [FreeKitDetailID], 
                            [TypeID],[Sno],
                            [PID],
                            [SerialID],
                            [MFGDate],[EXPDate],[InstallationDate],[S1],
                            [S3],[S4],[S5],[S6],[S7],[S8],
                            [S9],[S10],[AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [SER].[ID] AS [SerialID],
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[Sno]=[D].[BatchSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_SerialMaster] [SER] ON [SER].[Serial]=[D].[Serial]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------FreeKit Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnFreeKitDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [DetailSno],[TypeID],[Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[RateBasisID],[BasicAmt],[GrossQty],
                            [TareQty],[PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[DetailSno] AS [SnoDetail],[D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[5]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------UOM Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnUOMDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [BatchDetailID],
                            [CQty],
                            [CUOMID],
                            [UOMQty],
                            [UOMID]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID],
                            [D].[CQty],
                            [CU].[ID] AS [CUOMID],
                            [D].[UOMQty],
                            [UO].[ID] AS [UOMID]
                        FROM {tempTableName[6]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[ID]=[D].[DetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[ID]=[D].[BatchDetailID]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [CU] ON [D].[CUOM]=[CU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Commission---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnCommission]
                        (
                            [MasterID],
                            [DetailID],
                            [TypeID],[CommissionBasisID],
                            [AGID],
                            [SPID],
                            [SMID],
                            [GLID],
                            [SLID],
                            [Rate],[Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[TypeID],[D].[CommissionBasisID],
                            [AG].[ID] AS [AGID],
                            [SP].[ID] AS [SPID],
                            [SM].[ID] AS [SMID],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[7]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Rate]=[D].[DetailRate]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [D].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [D].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [D].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [D].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Rate]

                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],
                            [TareQty],[GrossWeight],[WeighBridge_Weight],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[8]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[9]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesReturnDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[10]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesReturnMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE] 
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [GodownRaw] AS(
                            SELECT DISTINCT
                                [M].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GodownID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesReturnStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                        ),[Godown] AS(
                            SELECT DISTINCT
                            [GM].[{MainFieldName}] AS [GodownCodeName]
                            FROM [GodownRaw] [GR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GM] ON [GR].[GodownID]=[GM].[ID]
                            WHERE [GM].[{MainFieldName}] IS NOT NULL 
                        ),[AgentRaw] AS(
                            SELECT DISTINCT
                                [M].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AGID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AGID] IS NOT NULL  
                             
                        ),[Agent] AS(
                            SELECT DISTINCT
                            [AG].[{MainFieldName}] AS [AgentCodeName]
                            FROM [AgentRaw] [AGR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [AGR].[AGID]=[AG].[ID]
                            WHERE [AG].[{MainFieldName}] IS NOT NULL 
                        ),[SalesManRaw] AS(
                            SELECT DISTINCT
                                [M].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SMID] IS NOT NULL  
                             
                        ),[SalesMan] AS(
                            SELECT DISTINCT
                            [SM].[{MainFieldName}] AS [SalesManCodeName]
                            FROM [SalesManRaw] [SMR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [SMR].[SMID]=[SM].[ID]
                            WHERE [SM].[{MainFieldName}] IS NOT NULL 
                        ),[SalesPartnerRaw] AS(
                            SELECT DISTINCT
                                [M].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SPID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SPID] IS NOT NULL  
                             
                        ),[SalesPartner] AS(
                            SELECT DISTINCT
                            [SP].[{MainFieldName}] AS [SalesPartnerCodeName]
                            FROM [SalesPartnerRaw] [SPR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [SPR].[SPID]=[SP].[ID]
                            WHERE [SP].[{MainFieldName}] IS NOT NULL 
                        ),[LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                          
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[SubledgerRaw] AS(
                            SELECT DISTINCT
                                [M].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                   
                        ),[Subledger] AS(
                            SELECT DISTINCT
                            [SL].[{MainFieldName}] AS [SubledgerCodeName]
                            FROM [SubledgerRaw] [SLR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [SLR].[SLID]=[SL].[ID]
                            WHERE [SL].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[AuditLockByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[AuditLockByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ReconcileByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ReconcileByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                          
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountPostUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[StockPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[StockPostUserID] IS NOT NULL
                           
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[ProductRaw] AS(
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesReturnStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                        ),[Product] AS(
                                SELECT DISTINCT
                                [P].[{MainFieldName}] AS [ProductCodeName]
                                FROM [ProductRaw] [PR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [PR].[PID]=[P].[ID]
                                WHERE [P].[{MainFieldName}] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION 
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[CUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[CUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesReturnUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ),[DetailRaw] AS(
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnStoneDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesReturnFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[BatchDetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesReturnUOMDetails] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesReturnUOMDetails] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesReturnCommission] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesReturnTerms] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                        ),[Detail] AS(
                                SELECT DISTINCT
                                [SOD].[Sno] AS [DetailSno]   
                                FROM [DetailRaw] [DR]
                                LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [SOD] ON [DR].[DetailID]=[SOD].[ID]
                                WHERE [SOD].[Sno] IS NOT NULL  

                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Godown' AS [ModuleName],  STRING_AGG(CAST( [GOD].[GodownCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Godown] [GOD]
                            WHERE [GOD].[GodownCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'LCMaster' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM  (
                                    --SELECT DISTINCT
                                        --[LCM].[Vno]
                                    --FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    --LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                                   -- WHERE [M].[LCMasterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LCM].[Vno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [SLD].[SubledgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Subledger] [SLD]
                            WHERE [SLD].[SubledgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [AGT].[AgentCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Agent] [AGT]
                            WHERE [AGT].[AgentCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [SMN].[SalesManCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesMan] [SMN]
                            WHERE [SMN].[SalesManCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [SPT].[SalesPartnerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesPartner] [SPT]
                            WHERE [SPT].[SalesPartnerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Table' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TBM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[RESTRO].[TBL_TableMaster] [TBM] ON [M].[TableID]=[TBM].[ID]
                                    WHERE [M].[TableID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TBM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'HotelRoom' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [HRM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[HMS].[TBL_Room] [HRM] ON [M].[HotelRoomID]=[HRM].[ID]
                                    WHERE [M].[HotelRoomID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [HRM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'HotelBed' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [HBM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[HMS].[TBL_Bed] [HBM] ON [M].[HotelBedID]=[HBM].[ID]
                                    WHERE [M].[HotelBedID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [HBM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Guest' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [GM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_GuestMaster] [GM] ON [M].[GuestID]=[GM].[ID]
                                    WHERE [M].[GuestID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [GM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Patient' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PTM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PatientMaster] [PTM] ON [M].[PatientID]=[PTM].[ID]
                                    WHERE [M].[PatientID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PTM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [PD].[ProductCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Product] [PD]
                            WHERE [PD].[ProductCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'BillDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[BD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesReturnDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [BD] ON [D].[BillID]=[BD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BD].[Sno] IS NOT NULL
                                --) [T]
                           
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Waiter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [WTR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesReturnDetails] [M]
                                    LEFT OUTER JOIN {DBName}[RESTRO].[TBL_Waiter] [WTR] ON [M].[WaiterID]=[WTR].[ID]
                                    WHERE [M].[WaiterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [WTR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Batch' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[BAT].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesReturnBatch] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BAT].[PID] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Serial' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SER].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesReturnSerial] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SER].[PID] IS NOT NULL
                                --) [T]                            
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesReturnOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesReturnOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesReturnOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Detail' AS ModuleName,  STRING_AGG(CAST( [DET].[DetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM Detail [DET]
                            WHERE [DET].[DetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesReturnTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesReturnDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }

        public (string SelectQuery, string InsertQuery, string[] UtdtTableNames, string SelectDependentQuery) GetSalesExpiryBreakage(bool IsLocalServer, string SourceDB, bool IsCodeWiseCopy, string Names = "", bool? ExcludeInActive = null)
        {
            string MainFieldName = IsCodeWiseCopy ? "Code" : "Name";
            string MainAlterFieldName = !IsCodeWiseCopy ? "Code" : "Name";
            string DBName = SourceDB.IsNullOrWhiteSpacePs() ? "" : $@"{SourceDB}.";
            string[] tempTableName;
            string[] tempTableNameInsert;

            if (IsLocalServer)
            {
                tempTableName = new[]
                {
                "#TempCopyMasterSalesExpiryBreakage",
                "#TempCopyMasterSalesExpiryBreakage2",
                "#TempCopyMasterSalesExpiryBreakage3",
                "#TempCopyMasterSalesExpiryBreakage4",
                "#TempCopyMasterSalesExpiryBreakage5",
                "#TempCopyMasterSalesExpiryBreakage6",
                "#TempCopyMasterSalesExpiryBreakage7",
                "#TempCopyMasterSalesExpiryBreakage8",
                "#TempCopyMasterSalesExpiryBreakage9",
                "#TempCopyMasterSalesExpiryBreakage10",

            };

                tempTableNameInsert = new[]
                {
                " INTO #TempCopyMasterSalesExpiryBreakage ",
                " INTO #TempCopyMasterSalesExpiryBreakage2 ",
                " INTO #TempCopyMasterSalesExpiryBreakage3 ",
                " INTO #TempCopyMasterSalesExpiryBreakage4 ",
                " INTO #TempCopyMasterSalesExpiryBreakage5 ",
                " INTO #TempCopyMasterSalesExpiryBreakage6 ",
                " INTO #TempCopyMasterSalesExpiryBreakage7 ",
                " INTO #TempCopyMasterSalesExpiryBreakage8 ",
                " INTO #TempCopyMasterSalesExpiryBreakage9 ",
                " INTO #TempCopyMasterSalesExpiryBreakage10 ",


            };
            }
            else
            {
                tempTableName = new[]
                {
                "@TempCopyMasterSalesExpiryBreakage",
                "@TempCopyMasterSalesExpiryBreakage2",
                "@TempCopyMasterSalesExpiryBreakage3",
                "@TempCopyMasterSalesExpiryBreakage4",
                "@TempCopyMasterSalesExpiryBreakage5",
                "@TempCopyMasterSalesExpiryBreakage6",
                "@TempCopyMasterSalesExpiryBreakage7",
                "@TempCopyMasterSalesExpiryBreakage8",
                "@TempCopyMasterSalesExpiryBreakage9",
                "@TempCopyMasterSalesExpiryBreakage10",


            };

                tempTableNameInsert = new[] { "", "", "", "", "", "", "", "", "", "" }; // cleaner empty strings
            }
            string[] utdtTableName = { };
            if (!IsLocalServer)
            {
                utdtTableName = new string[]
                {
                "UTDT_TempCopyMasterSalesExpiryBreakage",
                "UTDT_TempCopyMasterSalesExpiryBreakage2",
                "UTDT_TempCopyMasterSalesExpiryBreakage3",
                "UTDT_TempCopyMasterSalesExpiryBreakage4",
                "UTDT_TempCopyMasterSalesExpiryBreakage5",
                "UTDT_TempCopyMasterSalesExpiryBreakage6",
                "UTDT_TempCopyMasterSalesExpiryBreakage7",
                "UTDT_TempCopyMasterSalesExpiryBreakage8",
                "UTDT_TempCopyMasterSalesExpiryBreakage9",
                "UTDT_TempCopyMasterSalesExpiryBreakage10",


                };
            }

            string SelectQuery = $""" 
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage2;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage3;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage4;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage5;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage6;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage7;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage8;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage9;
                        DROP TABLE IF EXISTS #TempCopyMasterSalesExpiryBreakage10;
                
                      

                        DECLARE  @Data TABLE (ID INT )

                        ;WITH CTE AS(
                            SELECT 
                                [M].[ID] 
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            --WHERE ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                    --AND  (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                        INSERT INTO @Data(ID)
                        SELECT
                            DISTINCT [ID] 
                        FROM [CTE];

                        SELECT 
                            [M].[ID], [M].[VNo], [M].[VDate], [M].[VTime], [M].[EffDate],[M].[RefVno],  [M].[RefDate],[M].[TranDate], [M].[DueDays],  [M].[DueDate], [M].[IsUpdateInAccounts],
                            --[BM].[Vno] AS [BillVno], 
                            [M].[IsCash], [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[{MainFieldName}] AS [Godown],
                            [LCM].[Vno] AS [LCMasterVno],
                            [L].[{MainFieldName}] AS [Ledger],
                            [AL].[{MainFieldName}] AS [AccountLedger], 
                            [PBR].[{MainFieldName}] AS [PartyBranch],
                            [SL].[{MainFieldName}] AS [SubLedger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [AR].[{MainFieldName}] AS [Area],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [C1].[{MainFieldName}] AS [Class1],
                            [C2].[{MainFieldName}] AS [Class2],
                            [C3].[{MainFieldName}] AS [Class3],
                            [C4].[{MainFieldName}] AS [Class4],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [M].[IsNotLocal],[M].[IsABT],
                            [PLM].[{MainFieldName}] AS [PriceList], 
                            [TS].[{MainFieldName}] AS [TermScheme], 
                            [BS].[{MainFieldName}] AS [BonusScheme], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate],[M].[DCurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt],
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[Name] AS [AuditLockBy],
                            [M].[AuditLockDate], 
                            [RC].[Name] AS [ReconcileBy],
                            [M].[ReconcileDate], 
                            [CN].[Name] AS [CancelBy],
                            [M].[CancelDate], [M].[CancelReason],
                            [TBM].[{MainFieldName}] AS [Table],
                            [HRM].[{MainFieldName}] AS [HotelRoom],
                            [HBM].[{MainFieldName}] AS [HotelBed],
                            [M].[HospitalRoomID], [M].[HospitalBedID],
                            [GSM].[{MainFieldName}] AS [Guest],
                            [PTM].[{MainFieldName}] AS [Patient],
                            [M].[LocalPaxNo_Adult], [M].[LocalPaxNo_Child], [M].[ForeignPaxNo_Adult], [M].[ForeignPaxNo_Child],
                            [Unit].[Code] AS [CompanyUnit],
                            [Division].[{MainFieldName}] AS [Division],
                            [Shift].[{MainFieldName}] AS [Shift],
                            [M].[ShiftDate],
                            [CM].[{MainFieldName}] AS [Counter],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[Name] AS [ApprovalStatusUser],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [M].[AccountPostDate],[M].[IsAllowStockPost],
                            [SPU].[Name] AS [StockPostUser],
                            [M].[StockPostDate],[M].[PageID]

                            {tempTableNameInsert[0]}
                        FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillMaster] [BM] ON [M].[BillID]=[BM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [M].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [M].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [AL] ON [M].[AccountGLID]=[AL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranchID]=[PBR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [M].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [M].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [M].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [M].[SLID]=[SL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C1] ON [M].[CLS1ID]=[C1].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C2] ON [M].[CLS2ID]=[C2].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C3] ON [M].[CLS3ID]=[C3].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [C4] ON [M].[CLS4ID]=[C4].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [M].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [M].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceListID]=[PLM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [AD] ON [M].[AuditLockByID]=[AD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [RC] ON [M].[ReconcileByID]=[RC].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [CN] ON [M].[CancelByID] = [CN].[ID]
                        LEFT OUTER JOIN {DBName}[RESTRO].[TBL_TableMaster] [TBM] ON [M].[TableID]=[TBM].[ID]
                        LEFT OUTER JOIN {DBName}[HMS].[TBL_Room] [HRM] ON [M].[HotelRoomID]=[HRM].[ID]
                        LEFT OUTER JOIN {DBName}[HMS].[TBL_Bed] [HBM] ON [M].[HotelBedID]=[HBM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GuestMaster] [GSM] ON [M].[GuestID]=[GSM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_PatientMaster] [PTM] ON [M].[PatientID]=[PTM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CompanyUnit] [Unit] ON [M].[UnitID] = [Unit].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [Shift] ON [M].[ShiftID] = [Shift].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [Division] ON [M].[DID] =[Division].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CM] ON [M].[CounterID] = [CM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUserID] = [ASU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [APU] ON [M].[AccountPostUserID] = [APU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUserID] = [SPU].[ID]
                        WHERE  [M].[ID]  IN(SELECT [T].[ID] FROM @Data [T])
                        ORDER BY [M].[ID]

                        ----------------------------------------------------Detail---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [GD].[{MainFieldName}] AS [Godown],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            --[BD].[Sno] AS [BillDetailSno],
                         
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[{MainFieldName}] AS [GoodsLedger],
                            [GSL].[{MainFieldName}] AS [GoodsSubledger],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],[D].[StockRate],
                            [WTR].[{MainFieldName}] AS [Waiter],
                            [D].[Descriptions],[D].[TrackingNo]

                            {tempTableNameInsert[1]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GD] ON [D].[GodownID]=[GD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [BD] ON [D].[BillID]=[BD].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [GL] ON [D].[GoodsGLID]=[GL].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [GSL] ON [D].[GoodsSLID]=[GSL].[ID]
                        LEFT OUTER JOIN {DBName}[RESTRO].[TBL_Waiter] [WTR] ON [D].[WaiterID]=[WTR].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Batch---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[BAT].[PID] AS [Batch],   --here using PID column as there is no code or name field in BatchMaster
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[Code] AS [FreeAltUOM],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[Code] AS [FreeUOM],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],[D].[BasicAmt],[D].[StockRate],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[2]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOMID]=[FAU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [FU] ON [D].[FreeUOMID]=[FU].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Serial---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            --[BDT].[Sno] AS [BatchSno],
                            --[FKT].[Sno] AS [FreeKitSno],
                            [D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            --[SER].[PID] AS [Serial],   --here using PID column as there is no code or name field in SerialMaster
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[3]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [FKT] ON [D].[FreeKitDetailID]=[FKT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SerialMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------FreeKit Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Sno] AS [DetailSno],
                            [D].[DetailSno] AS [SnoDetail],[D].[TypeID],[D].[Sno],
                            [P].[{MainFieldName}] AS [Product],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[Code] AS [AltUOM],
                            [D].[StockQty],[D].[Qty],
                            [UO].[Code] AS [UOM],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[4]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [D].[PID]=[P].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [AU] ON [D].[AltUOMID]=[AU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------UOM Details---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[ID] AS [DetailID], --no Sno column, so putting other column like ID
                            --[BDT].[ID] AS [BatchDetailID],  --no Sno column, so putting other column like ID
                            [D].[CQty],
                            [CU].[Code] AS [CUOM],
                            [D].[UOMQty],
                            [UO].[Code] AS [UOM]
                        
                            {tempTableNameInsert[5]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageUOMDetails] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [BDT] ON [D].[BatchDetailID]=[BDT].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [CU] ON [D].[CUOMID]=[CU].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UO] ON [D].[UOMID]=[UO].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Commission---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [D].[TypeID],[D].[CommissionBasisID],
                            [L].[{MainFieldName}] AS [Ledger],
                            [SL].[{MainFieldName}] AS [Subledger],
                            [AG].[{MainFieldName}] AS [Agent],
                            [SP].[{MainFieldName}] AS [SalesPartner],
                            [SM].[{MainFieldName}] AS [SalesMan],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        
                            {tempTableNameInsert[6]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [D].[AGID]=[AG].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [D].[SMID]=[SM].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [D].[SPID]=[SP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [D].[GLID]=[L].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [D].[SLID]=[SL].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        ----------------------------------------------------Other Info---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [TR].[{MainFieldName}] AS [Transport],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[{MainFieldName}] AS [LoadingPort],
                            [DP].[{MainFieldName}] AS [DischargePort],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]

                            {tempTableNameInsert[7]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageOtherInfo] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                        INNER JOIN @Data [DA] ON [D].[ID]=[DA].[ID]

                        ----------------------------------------------------Terms---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            --[DET].[Rate] AS [DetailRate],
                            [BT].[{MainFieldName}] AS [BillingTerm],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                            {tempTableNameInsert[8]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageTerms] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [D].[DetailID]=[DET].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]

                        
                        ----------------------------------------------------Document Attachment---------------------------------------
                        SELECT 
                            --[M].[ID] AS [MasterID],
                            [D].[Sno],
                            [DT].[{MainFieldName}] AS [DocumentType], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                            {tempTableNameInsert[9]}
                        FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageDocs] [D]
                        LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                        INNER JOIN @Data [DA] ON [D].[MasterID]=[DA].[ID]
                        
                       
 
                        """;

            string InsertQuery = $$""" 

                        
                        Declare @smt Nvarchar(max) = '*********************** Sales ExpiryBreakage ********************************' + CHAR(13) + CHAR(10)
                        DECLARE  @DataList TABLE (CodeName NVARCHAR(MAX))
                        
                        INSERT INTO @DataList(CodeName)
                        --SELECT 
                            --[M].[Vno]
                        --FROM {tempTableName[0]} [M]
                        --WHERE NOT EXISTS (SELECT 1 FROM [SALES].[TBL_SalesExpiryBreakageMaster] [SQ] WHERE [SQ].[Vno] = [M].[Vno])
                        
                   
                        --SELECT 
                            --@smt  = @smt + 'Sales ExpiryBreakage [Vno] ['+ [Vno]  + '] Is Already Exists ' + char(13) + char(10)
                        --FROM [SALES].[TBL_SalesExpiryBreakageMaster] [M]  WHERE [M].[Vno] IN (SELECT [T].[Vno] FROM [{tempTableName[0]}] [T] ) 

                        --SELECT 
                            --@smt  = @smt + 'Sales ExpiryBreakage [Vno] ['+ [Vno]  + '] Is Created' + char(13) + char(10)
                        --FROM [{tempTableName[0]}] [M] WHERE [M].[Vno] NOT IN (SELECT [T].[Vno] FROM [SALES].[TBL_SalesExpiryBreakageMaster] [T]) 


                        SET @smt=@smt + 'Sales ExpiryBreakage Is Created ' + char(13) + char(10)
                         

                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageMaster]
                        (
                            [VNo], [VDate], [VTime], [EffDate],[RefVno],  [RefDate],[M].[TranDate], [DueDays],  [DueDate],  [BillID], [IsCash], [IsUpdateInAccounts], 
                            [BillingCPerson], [BillingName], [BillingAdress], [BillingAdress2], [BillingCountryID],  
                            [BillingPhone],  [BillingMobile],[BillingMobile2], [BillingEmail], [BillingVatPanNo], 
                            [ShippingCPerson],[ShippingAdress], [ShippingAdress2],  [ShippingCountryID],
                            [ShippingPhone],[ShippingMobile], [ShippingMobile2], [ShippingEmail], 
                            [GodownID],
                            [LCMasterID],
                            [GLID],
                            [AccountGLID],
                            [PartyBranchID],
                            [SLID],
                            [AGID],
                            [ARID],
                            [SMID],
                            [SPID],
                            [CLS1ID],
                            [CLS2ID],
                            [CLS3ID],
                            [CLS4ID],
                            [GoodsGLID],
                            [GoodsSLID],
                            [IsNotLocal],[IsABT],
                            [PriceListID], 
                            [TermSchemeID], 
                            [BonusSchemeID], 
                            [RateCategoryID],[CurrencyID], [CurRate],[DCurRate], [BasicAmt],  [TAmt], [NetAmt],
                            [INotes], [Remarks], [TrackingNo], [PrintNo], 
                            [AuditLockDate],
                            [ReconcileByID],
                            [ReconcileDate],
                            [CancelByID],
                            [CancelDate], [CancelReason],
                            [TableID],
                            [HotelRoomID],
                            [HotelBedID],
                            [HospitalRoomID],[HospitalBedID],
                            [GuestID],
                            [PatientID],
                            [LocalPaxNo_Adult], [LocalPaxNo_Child], [ForeignPaxNo_Adult], [ForeignPaxNo_Child],
                            [FYID],
                            [UnitID],
                            [DID],
                            [ShiftID],
                            [ShiftDate],
                            [CounterID],
                            [ApprovalStatusID], [ApprovalStatusTypeID],
                            [ApprovalStatusUserID],
                            [ApprovalStatusLastDate],[ApprovalStatusUserRemarks],[ApprovalStatusSystemRemarks],
                            [IsAllowAccountPost],[AccountPostUserID],[AccountPostDate],
                            [IsAllowStockPost],[StockPostUserID],[StockPostDate],[PageID],
                            [CreatedbyID], [CreatedOn],[DeviceInfoI]

                        )

                        SELECT 
                            [M].[VNo], [M].[VDate], [M].[VTime], [M].[EffDate],[M].[RefVno], [M].[RefDate],[M].[TranDate], [M].[DueDays],  [M].[DueDate],   [M].[IsUpdateInAccounts],  
                            [BM].[ID] AS [BillID],  
                            [M].[IsCash], [M].[BillingCPerson], [M].[BillingName], [M].[BillingAdress], [M].[BillingAdress2], [M].[BillingCountryID],  
                            [M].[BillingPhone],  [M].[BillingMobile],[M].[BillingMobile2], [M].[BillingEmail], [M].[BillingVatPanNo], 
                            [M].[ShippingCPerson],[M].[ShippingAdress], [M].[ShippingAdress2],  [M].[ShippingCountryID],
                            [M].[ShippingPhone],[M].[ShippingMobile], [M].[ShippingMobile2], [M].[ShippingEmail], 
                            [GD].[ID] AS [GodownID],
                            [LCM].[ID] AS [LCMasterID],
                            [L].[ID] AS [GLID],
                            [AL].[ID] AS [AccountGLID], 
                            [PBR].[ID] AS [PartyBranchID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [AR].[ID] AS [ARID],
                            [SM].[ID] AS [SMID],
                            [SP].[ID] AS [SPID],
                            [C1].[ID] AS [CLS1ID],
                            [C2].[ID] AS [CLS2ID],
                            [C3].[ID] AS [CLS3ID],
                            [C4].[ID] AS [CLS4ID],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [M].[IsNotLocal], [M].[IsABT],
                            [PLM].[ID] AS [PriceListID], 
                            [TS].[ID] AS [TermSchemeID], 
                            [BS].[ID] AS [BonusSchemeID], 
                            [M].[RateCategoryID],[M].[CurrencyID], [M].[CurRate],[M].[DCurRate], [M].[BasicAmt],  [M].[TAmt], [M].[NetAmt], 
                            [M].[INotes], [M].[Remarks], [M].[TrackingNo],  [M].[PrintNo], 
                            [AD].[ID] AS [AuditLockByID],
                            [M].[AuditLockDate], 
                            [RC].[ID] AS [ReconcileByID],
                            [M].[ReconcileDate],                           
                            [CN].[ID] AS [CancelByID],
                            [M].[CancelDate], [M].[CancelReason],
                            [TBM].[ID] AS [TableID],
                            [HRM].[ID] AS [HotelRoomID],
                            [HBM].[ID] AS [HotelBedID],
                            [M].[HospitalRoomID], [M].[HospitalBedID],
                            [GSM].[ID] AS [GuestID],
                            [PTM].[ID] AS [PatientID],
                            [FYID]=(SELECT TOP 1 [T].[FYID] FROM [GM].[TBL_DefaultCompany] [T]), 
                            [Unit].[ID] AS [UnitID],
                            [Division].[ID] AS [DID],
                            [Shift].[ID] AS [ShiftID],
                            [M].[ShiftDate],
                            [CM].[ID] AS [CounterID],
                            [M].[ApprovalStatusID], [M].[ApprovalStatusTypeID],
                            [ASU].[ID] AS [ApprovalStatusUserID],
                            [M].[ApprovalStatusLastDate],[M].[ApprovalStatusUserRemarks],[M].[ApprovalStatusSystemRemarks],
                            [M].[IsAllowAccountPost],
                            [APU].[Name] AS [AccountPostUser],
                            [M].[AccountPostDate],[M].[IsAllowStockPost],
                            [SPU].[ID] AS [StockPostUserID],
                            [M].[StockPostDate],[M].[PageID],
                            @UserID AS [CreatedbyID],Getdate() AS [CreatedOn],@DeviceInfo AS [DeviceInfoI]

                        FROM {tempTableName[0]} [M]
                        --INNER JOIN @TempDataCodeName [TD] ON [M].[ID]=[TD].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesBillMaster] [BM] ON [BillVno]=[BM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [M].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [FIN].[TBL_LCMaster] [LCM] ON [LCM].[MasterID]=[M].[ID] AND [M].[LCMasterVno]=[LCM].[Vno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [M].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [AL] ON [M].[AccountLedger]=[AL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PartyBranch] [PBR] ON [M].[PartyBranch]=[PBR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [M].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [M].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [M].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Area] [AR] ON [M].[Area]=[AR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [M].[SubLedger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C1] ON [M].[Class1]=[C1].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C2] ON [M].[Class2]=[C2].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C3] ON [M].[Class3]=[C3].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ClassMaster] [C4] ON [M].[Class4]=[C4].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [M].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [M].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PriceListMaster] [PLM] ON [M].[PriceList]=[PLM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermScheme]=[TS].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusScheme]=[BS].[{MainFieldName}]
                        LEFT OUTER JOIN [RESTRO].[TBL_TableMaster] [TBM] ON [M].[Table]=[TBM].[{MainFieldName}]
                        LEFT OUTER JOIN [HMS].[TBL_Room] [HRM] ON [M].[HotelRoom]=[HRM].[{MainFieldName}]
                        LEFT OUTER JOIN [HMS].[TBL_Bed] [HBM] ON [M].[HotelBed]=[HBM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_GuestMaster] [GSM] ON [M].[Guest]=[GSM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_PatientMaster] [PTM] ON [M].[Patient]=[PTM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [AD] ON [M].[AuditLockBy]=[AD].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [RC] ON [M].[ReconcileBy]=[RC].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CLO] ON [M].[ClosedBy] = [CLO].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [CN] ON [M].[CancelBy] = [CN].[Name]
                        LEFT OUTER JOIN [GM].[TBL_CompanyUnit] [Unit] ON [M].CompanyUnit = [Unit].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_ShiftMaster] [Shift] ON [M].[Shift] = [Shift].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_DivisionMaster] [Division] ON [M].[Division] =[Division].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CounterMaster] [CM] ON [M].[Counter] = [CM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [ASU] ON [M].[ApprovalStatusUser] = [ASU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [APU] ON [M].[AccountPostUser] = [APU].[Name]
                        LEFT OUTER JOIN [GM].[TBL_UserMaster] [SPU] ON [M].[StockPostUser] = [SPU].[Name]
                        --INNER JOIN @DataList [DL] ON [M].[{MainFieldName}]=[DL].[CodeName] 
 

                        ----------------------------------------------------Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageDetails]
                        (
                            [MasterID],
                            [Sno],
                            [PID],
                            [GodownID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],
                            [Rate],[RateBasisID],
                            [BasicAmt],[TAmt],[NetAmt],
                            [GrossQty],[TareQty],
                            [BillID],
                            [PCSOnly],[FreePCSOnly],
                            [GoodsGLID],[GoodsSLID],
                            [IncludeVatRate],[IncludeVatAmount],[StockRate],
                            [WaiterID],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [GD].[ID] AS [GodownID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],
                            [D].[Rate],[D].[RateBasisID],
                            [D].[BasicAmt],[D].[TAmt],[D].[NetAmt],
                            [D].[GrossQty],[D].[TareQty],
                            [BD].[ID] AS [BillID],
                            [D].[PCSOnly],[D].[FreePCSOnly],
                            [GL].[ID] AS [GoodsGLID],
                            [GSL].[ID] AS [GoodsSLID],
                            [D].[IncludeVatRate],[D].[IncludeVatAmount],[D].[StockRate],
                            [WTR].[ID] AS [WaiterID],
                            [D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[1]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_GodownMaster] [GD] ON [D].[Godown]=[GD].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesBillDetails] [BD] ON [BD].[MasterID]=[M].[ID] AND  [BD].[Sno]=[D].[BillDetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [GL] ON [D].[GoodsLedger]=[GL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [GSL] ON [D].[GoodsSubledger]=[GSL].[{MainFieldName}]
                        LEFT OUTER JOIN [RESTRO].[TBL_Waiter] [WTR] ON [D].[GoodsSubledger]=[WTR].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]

                        ----------------------------------------------------Batch---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageBatch]
                        (
                            [MasterID],
                            [DetailID],
                            [FreeKitDetailID],
                            [Sno],
                            [PID],
                            [BatchID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [FreeAltStockQty],[FreeAltQty],
                            [FreeAltUOMID],
                            [FreeStockQty],[FreeQty],
                            [FreeUOMID],
                            [ExtraFreeStockQty],[ExtraFreeQty],[Rate],
                            [BasicAmt],[StockRate],[GrossQty],[TareQty],
                            [PCSOnly],[FreePCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[Sno],
                            [P].[ID] AS [PID],
                            [BAT].[ID] AS [BatchID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[FreeAltStockQty],[D].[FreeAltQty],
                            [FAU].[ID] AS [FreeAltUOMID],
                            [D].[FreeStockQty],[D].[FreeQty],
                            [FU].[ID] AS [FreeUOMID],
                            [D].[ExtraFreeStockQty],[D].[ExtraFreeQty],[D].[Rate],
                            [D].[BasicAmt],[D].[StockRate],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[FreePCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[3]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_BatchMaster] [BAT] ON [BAT].[Batch]=[D].[Batch]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Serial---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageSerial]
                        (
                            [MasterID],
                            [DetailID], 
                            [BatchDetailID], 
                            [FreeKitDetailID], 
                            [TypeID],[Sno],
                            [PID],
                            [SerialID],
                            [MFGDate],[EXPDate],[InstallationDate],[S1],
                            [S3],[S4],[S5],[S6],[S7],[S8],
                            [S9],[S10],[AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[BasicAmt],[GrossQty],[TareQty],
                            [PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID], 
                            [FKT].[ID] AS [FreeKitDetailID], 
                            [D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [SER].[ID] AS [SerialID],
                            [D].[MFGDate],[D].[EXPDate],[D].[InstallationDate],[D].[S1],
                            [D].[S2],[D].[S3],[D].[S4],[D].[S5],[D].[S6],[D].[S7],[D].[S8],
                            [D].[S9],[D].[S10],[D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[BasicAmt],[D].[GrossQty],[D].[TareQty],
                            [D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[4]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[Sno]=[D].[BatchSno]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [FKT] ON [FKT].[MasterID]=[M].[ID] AND  [FKT].[Sno]=[D].[FreeKitSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        --LEFT OUTER JOIN [GM].[TBL_SerialMaster] [SER] ON [SER].[Serial]=[D].[Serial]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FAU] ON [D].[FreeAltUOM]=[FAU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [FU] ON [D].[FreeUOM]=[FU].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------FreeKit Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageFreeKitDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [DetailSno],[TypeID],[Sno],
                            [PID],
                            [AltStockQty],[AltQty],
                            [AltUOMID],
                            [StockQty],[Qty],
                            [UOMID],
                            [Rate],[RateBasisID],[BasicAmt],[GrossQty],
                            [TareQty],[PCSOnly],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[DetailSno] AS [SnoDetail],[D].[TypeID],[D].[Sno],
                            [P].[ID] AS [PID],
                            [D].[AltStockQty],[D].[AltQty],
                            [AU].[ID] AS [AltUOMID],
                            [D].[StockQty],[D].[Qty],
                            [UO].[ID] AS [UOMID],
                            [D].[Rate],[D].[RateBasisID],[D].[BasicAmt],[D].[GrossQty],
                            [D].[TareQty],[D].[PCSOnly],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[5]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Sno]=[D].[DetailSno]
                        LEFT OUTER JOIN [GM].[TBL_Product] [P] ON [D].[Product]=[P].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [AU] ON [D].[AltUOM]=[AU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------UOM Detail---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageUOMDetails]
                        (
                            [MasterID],
                            [DetailID],
                            [BatchDetailID],
                            [CQty],
                            [CUOMID],
                            [UOMQty],
                            [UOMID]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [BDT].[ID] AS [BatchDetailID],
                            [D].[CQty],
                            [CU].[ID] AS [CUOMID],
                            [D].[UOMQty],
                            [UO].[ID] AS [UOMID]
                        FROM {tempTableName[6]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[ID]=[D].[DetailID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageBatch] [BDT] ON [BDT].[MasterID]=[M].[ID] AND  [BDT].[ID]=[D].[BatchDetailID]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [CU] ON [D].[CUOM]=[CU].[Code]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Sno]

                        ----------------------------------------------------Commission---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageCommission]
                        (
                            [MasterID],
                            [DetailID],
                            [TypeID],[CommissionBasisID],
                            [GLID],
                            [SLID],
                            [AGID],
                            [SPID],
                            [SMID],
                            [Rate],[Amount],[INotes],[Descriptions],[TrackingNo]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID], 
                            [D].[TypeID],[D].[CommissionBasisID],
                            [L].[ID] AS [GLID],
                            [SL].[ID] AS [SLID],
                            [AG].[ID] AS [AGID],
                            [SP].[ID] AS [SPID],
                            [SM].[ID] AS [SMID],
                            [D].[Rate],[D].[Amount],[D].[INotes],[D].[Descriptions],[D].[TrackingNo]
                        FROM {tempTableName[7]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        --LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND  [DET].[Rate]=[D].[DetailRate]
                        LEFT OUTER JOIN [GM].[TBL_Agent] [AG] ON [D].[Agent]=[AG].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesMan] [SM] ON [D].[SalesMan]=[SM].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_SalesPartner] [SP] ON [D].[SalesPartner]=[SP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Ledger] [L] ON [D].[Ledger]=[L].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_Subledger] [SL] ON [D].[Subledger]=[SL].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_UOM] [UO] ON [D].[UOM]=[UO].[Code]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [DET].[ID],[D].[Rate]

                        ----------------------------------------------------Other Info---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageOtherInfo]
                        (
                            [ID],
                            [TransportID],
                            [ModeOfTransportation],[ContainerNo],[CNNo],[CNDate],
                            [CNFreightTypeID],[CNFreightAmount],[AdvanceAmount],[BalanceFreightAmount],
                            [Package],[VehicleNo],[Driver],[DriverContactNo],
                            [LicenseNo],[CustomName],[DocumentNo],[DocumentBank],
                            [LCNO],[ContractNO],[IncoTerm],[PaymentTerm],
                            [LoadingPortID],
                            [DischargePortID],
                            [GateEntryNo],[GateEntryDate],[NetWeight],
                            [TareQty],[GrossWeight],[WeighBridge_Weight],
                            [ContractDate],[DeliveryPort],[CarriageBy],
                            [TermCondition],[ExportInvNo],[ExportInvDate]
                        )
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [TR].[ID] AS [TransportID],
                            [D].[ModeOfTransportation],[D].[ContainerNo],[D].[CNNo],[D].[CNDate],
                            [D].[CNFreightTypeID],[D].[CNFreightAmount],[D].[AdvanceAmount],[D].[BalanceFreightAmount],
                            [D].[Package],[D].[VehicleNo],[D].[Driver],[D].[DriverContactNo],
                            [D].[LicenseNo],[D].[CustomName],[D].[DocumentNo],[D].[DocumentBank],
                            [D].[LCNO],[D].[ContractNO],[D].[IncoTerm],[D].[PaymentTerm],
                            [LP].[ID] AS [LoadingPortID],
                            [DP].[ID] AS [DischargePortID],
                            [D].[GateEntryNo],[D].[GateEntryDate],[D].[NetWeight],
                            [D].[TareQty],[D].[GrossWeight],[D].[WeighBridge_Weight],
                            [D].[ContractDate],[D].[DeliveryPort],[D].[CarriageBy],
                            [D].[TermCondition],[D].[ExportInvNo],[D].[ExportInvDate]
                        FROM {tempTableName[8]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[ID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_TransportMaster] [TR] ON [D].[Transport]=[TR].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPort]=[LP].[{MainFieldName}]
                        LEFT OUTER JOIN [GM].[TBL_CommonMaster] [DP] ON [D].[DischargePort]=[DP].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 

                        ----------------------------------------------------Terms---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageTerms]([MasterID], [DetailID], [BTID], [TypeID], [Formulation], [CalculableAmount], [Rate], [Amt], [IsFixed])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [DET].[ID] AS [DetailID],
                            [BT].[ID] AS [BTID],
                            [D].[TypeID], [D].[Formulation],[D].[CalculableAmount],
                            [D].[Rate], [D].[Amt],[D].[IsFixed]
                        FROM {tempTableName[9]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageDetails] [DET] ON [DET].[MasterID]=[M].[ID] AND [D].[DetailRate]=[DET].[Rate]
                        LEFT OUTER JOIN [GM].[TBL_BillingTerm] [BT] ON [D].[BillingTerm]=[BT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName]  
                        --ORDER BY [DET].[ID],[D].[Rate]
                        

                        ----------------------------------------------------Document Attachment---------------------------------------
                        INSERT INTO [SALES].[TBL_SalesExpiryBreakageDocs]([MasterID], [Sno],[DocumentTypeID], [DocumentName], [DocumentLocation], [Descriptions], [Tag], [DocumentContent])
                        SELECT 
                            [M].[ID] AS [MasterID], 
                            [D].[Sno],
                            [DT].[ID] AS [DocumentTypeID], 
                            [D].[DocumentName], [D].[DocumentLocation], [D].[Descriptions], 
                            [D].[Tag], [D].[DocumentContent]
                        FROM {tempTableName[10]} [D]
                        LEFT OUTER JOIN [SALES].[TBL_SalesExpiryBreakageMaster] [M] ON [D].[MasterID]=[M].[ID]
                        LEFT OUTER JOIN [GM].[TBL_DocumentType] [DT] ON [D].[DocumentType]=[DT].[{MainFieldName}]
                        --INNER JOIN @DataList [DL] ON [D].[MasterCodeName]=[DL].[CodeName] 
                        --ORDER BY [M].[ID],[D].[Sno]
                       

                        SELECT  @smt
                        """;

            string SelectDependentQuery = $""" 

                        DECLARE  @Data TABLE (ID INT )
                        
                        ;WITH [CTE] AS (
                            SELECT 
                                [M].[ID],
                                CAST(',' + CAST([M].[ID] AS VARCHAR(20)) + ',' AS NVARCHAR(MAX)) AS PathCsv
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            --WHERE  ([M].[InActive] <>@ExcludeInActive OR @ExcludeInActive=0)
                                           --AND (@Names = '' OR [M].[Vno] IN (SELECT DISTINCT [Value] FROM {DBName}[Usys].[Ufn_SplitByString](@Names,'|;|')))
                        )
                           
                        INSERT INTO @Data(ID)
                        SELECT 
                            DISTINCT [ID]
                        FROM [CTE] 
                        WHERE [ID] IS NOT NULL AND [ID] <> 0
                        OPTION (MAXRECURSION 0)

                        ;WITH [GodownRaw] AS(
                            SELECT DISTINCT
                                [M].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GodownID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GodownID] AS [GodownID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GodownID] IS NOT NULL  
                            
                        ),[Godown] AS(
                            SELECT DISTINCT
                            [GM].[{MainFieldName}] AS [GodownCodeName]
                            FROM [GodownRaw] [GR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_GodownMaster] [GM] ON [GR].[GodownID]=[GM].[ID]
                            WHERE [GM].[{MainFieldName}] IS NOT NULL 
                        ),[AgentRaw] AS(
                            SELECT DISTINCT
                                [M].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AGID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AGID] AS [AGID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AGID] IS NOT NULL  
                             
                        ),[Agent] AS(
                            SELECT DISTINCT
                            [AG].[{MainFieldName}] AS [AgentCodeName]
                            FROM [AgentRaw] [AGR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Agent] [AG] ON [AGR].[AGID]=[AG].[ID]
                            WHERE [AG].[{MainFieldName}] IS NOT NULL 
                        ),[SalesManRaw] AS(
                            SELECT DISTINCT
                                [M].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SMID] AS [SMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SMID] IS NOT NULL  
                             
                        ),[SalesMan] AS(
                            SELECT DISTINCT
                            [SM].[{MainFieldName}] AS [SalesManCodeName]
                            FROM [SalesManRaw] [SMR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesMan] [SM] ON [SMR].[SMID]=[SM].[ID]
                            WHERE [SM].[{MainFieldName}] IS NOT NULL 
                        ),[SalesPartnerRaw] AS(
                            SELECT DISTINCT
                                [M].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SPID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SPID] AS [SPID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SPID] IS NOT NULL  
                             
                        ),[SalesPartner] AS(
                            SELECT DISTINCT
                            [SP].[{MainFieldName}] AS [SalesPartnerCodeName]
                            FROM [SalesPartnerRaw] [SPR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_SalesPartner] [SP] ON [SPR].[SPID]=[SP].[ID]
                            WHERE [SP].[{MainFieldName}] IS NOT NULL 
                        ),[LedgerRaw] AS(
                            SELECT DISTINCT
                                [M].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsGLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsGLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GLID] AS [GLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GLID] IS NOT NULL
                          
                        ),[Ledger] AS(
                            SELECT DISTINCT
                            [L].[{MainFieldName}] AS [LedgerCodeName]
                            FROM [LedgerRaw] [LR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Ledger] [L] ON [LR].[GLID]=[L].[ID]
                            WHERE [L].[{MainFieldName}] IS NOT NULL 
                        ),[SubledgerRaw] AS(
                            SELECT DISTINCT
                                [M].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[SLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[GoodsSLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[GoodsSLID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[SLID] AS [SLID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[SLID] IS NOT NULL
                   
                        ),[Subledger] AS(
                            SELECT DISTINCT
                            [SL].[{MainFieldName}] AS [SubledgerCodeName]
                            FROM [SubledgerRaw] [SLR]
                            LEFT OUTER JOIN {DBName}[GM].[TBL_Subledger] [SL] ON [SLR].[SLID]=[SL].[ID]
                            WHERE [SL].[{MainFieldName}] IS NOT NULL 
                        ),[ClassRaw] AS(
                            SELECT DISTINCT
                                [M].[CLS1ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS1ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS2ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS2ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS3ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS3ID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CLS4ID] AS [ClassID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[CLS4ID] IS NOT NULL
                        ),[Class] AS(
                                SELECT DISTINCT
                                [CLS].[{MainFieldName}] AS [ClassCodeName]
                                FROM [ClassRaw] [CLR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_ClassMaster] [CLS] ON [CLR].[ClassID]=[CLS].[ID]
                                WHERE [CLS].[{MainFieldName}] IS NOT NULL 
                        ),[UserRaw] AS(
                            SELECT DISTINCT
                                [M].[AuditLockByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[AuditLockByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[ReconcileByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [M].[ReconcileByID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[CancelByID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                          
                            UNION
                            SELECT DISTINCT
                                [M].[ApprovalStatusUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[ApprovalStatusUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[AccountPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[AccountPostUserID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [M].[StockPostUserID] AS [UserID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                            WHERE [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [M].[StockPostUserID] IS NOT NULL
                           
                        ),[User] AS(
                                SELECT DISTINCT
                                [US].[Name] AS [UserName]
                                FROM [UserRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UserMaster] [US] ON [UR].[UserID]=[US].[ID]
                                WHERE [US].[Name] IS NOT NULL 
                        ),[ProductRaw] AS(
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[PID] AS [PID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[PID] IS NOT NULL
                        ),[Product] AS(
                                SELECT DISTINCT
                                [P].[{MainFieldName}] AS [ProductCodeName]
                                FROM [ProductRaw] [PR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_Product] [P] ON [PR].[PID]=[P].[ID]
                                WHERE [P].[{MainFieldName}] IS NOT NULL 
                        ),[UOMRaw] AS(
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                           
                            UNION 
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeAltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeAltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[AltUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[AltUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[CUOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[CUOMID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[UOMID] AS [UOMID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageUOMDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[UOMID] IS NOT NULL
                        ),[UOM] AS(
                                SELECT DISTINCT
                                [UOM].[Code] AS [UOMCode]
                                FROM [UOMRaw] [UR]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_UOM] [UOM] ON [UR].[UOMID]=[UOM].[ID]
                                WHERE [UOM].[Code] IS NOT NULL 
                        ),[DetailRaw] AS(
                           
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[BatchDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[FreeKitDetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[FreeKitDetailID] IS NOT NULL
                            UNION
                            SELECT DISTINCT
                                [D].[DetailID] AS [DetailID]
                            FROM {DBName}[SALES].[TBL_SalesExpiryBreakageFreeKitDetails] [D]
                            WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[BatchDetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesExpiryBreakageUOMDetails] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[BatchDetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesExpiryBreakageUOMDetails] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesExpiryBreakageCommission] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                            --UNION
                            --SELECT DISTINCT
                                --[D].[DetailID] AS [DetailID]
                            --FROM {DBName}[SALES].[TBL_SalesExpiryBreakageTerms] [D]
                            --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T]) AND [D].[DetailID] IS NOT NULL
                        ),[Detail] AS(
                                SELECT DISTINCT
                                [SOD].[Sno] AS [DetailSno]   
                                FROM [DetailRaw] [DR]
                                LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesOrderDetails] [SOD] ON [DR].[DetailID]=[SOD].[ID]
                                WHERE [SOD].[Sno] IS NOT NULL  

                        ), [FinalData] AS(
                            SELECT DISTINCT
                                'Godown' AS [ModuleName],  STRING_AGG(CAST( [GOD].[GodownCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Godown] [GOD]
                            WHERE [GOD].[GodownCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'LCMaster' AS [ModuleName],  STRING_AGG(CAST( [T].[Vno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM  (
                                    --SELECT DISTINCT
                                        --[LCM].[Vno]
                                    --FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    --LEFT OUTER JOIN {DBName}[FIN].[TBL_LCMaster] [LCM] ON [M].[LCMasterID]=[LCM].[ID]
                                   -- WHERE [M].[LCMasterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LCM].[Vno] IS NOT NULL
                                --) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Ledger' AS [ModuleName],  STRING_AGG(CAST( [LED].[LedgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Ledger] [LED]
                            WHERE [LED].[LedgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Subledger' AS [ModuleName],  STRING_AGG(CAST( [SLD].[SubledgerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Subledger] [SLD]
                            WHERE [SLD].[SubledgerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PartyBranch' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PartyBranch] [PR] ON [M].[PartyBranchID]=[PR].[ID]
                                    WHERE [M].[PartyBranchID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Agent' AS [ModuleName],  STRING_AGG(CAST( [AGT].[AgentCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Agent] [AGT]
                            WHERE [AGT].[AgentCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesMan' AS [ModuleName],  STRING_AGG(CAST( [SMN].[SalesManCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesMan] [SMN]
                            WHERE [SMN].[SalesManCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'SalesPartner' AS [ModuleName],  STRING_AGG(CAST( [SPT].[SalesPartnerCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [SalesPartner] [SPT]
                            WHERE [SPT].[SalesPartnerCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Area' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [AR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_Area] [AR] ON [M].[ARID]=[AR].[ID]
                                    WHERE [M].[ARID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [AR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Class' AS [ModuleName],  STRING_AGG(CAST( [CLS].[ClassCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Class] [CLS]
                            WHERE [CLS].[ClassCodeName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'PriceList' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PL].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PriceListMaster] [PL] ON [M].[PriceListID]=[PL].[ID]
                                    WHERE [M].[PriceListID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PL].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'TermScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TermSchemeMaster] [TS] ON [M].[TermSchemeID]=[TS].[ID]
                                    WHERE [M].[TermSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'BonusScheme' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [BS].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [BS] ON [M].[BonusSchemeID]=[BS].[ID]
                                    WHERE [M].[BonusSchemeID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BS].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Table' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [TBM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[RESTRO].[TBL_TableMaster] [TBM] ON [M].[TableID]=[TBM].[ID]
                                    WHERE [M].[TableID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TBM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'HotelRoom' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [HRM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[HMS].[TBL_Room] [HRM] ON [M].[HotelRoomID]=[HRM].[ID]
                                    WHERE [M].[HotelRoomID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [HRM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'HotelBed' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [HBM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[HMS].[TBL_Bed] [HBM] ON [M].[HotelBedID]=[HBM].[ID]
                                    WHERE [M].[HotelBedID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [HBM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Guest' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [GM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_GuestMaster] [GM] ON [M].[GuestID]=[GM].[ID]
                                    WHERE [M].[GuestID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [GM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Patient' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [PTM].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_PatientMaster] [PTM] ON [M].[PatientID]=[PTM].[ID]
                                    WHERE [M].[PatientID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [PTM].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'CompanyUnit' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CU].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BonusSchemeMaster] [CU] ON [M].[UnitID]=[CU].[ID]
                                    WHERE [M].[UnitID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CU].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Shift' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [SH].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_ShiftMaster] [SH] ON [M].[ShiftID]=[SH].[ID]
                                    WHERE [M].[ShiftID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SH].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Division' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [DV].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_DivisionMaster] [DV] ON [M].[DID]=[DV].[ID]
                                    WHERE [M].[DID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DV].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Counter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [CO].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageMaster] [M]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CounterMaster] [CO] ON [M].[CounterID]=[CO].[ID]
                                    WHERE [M].[CounterID] IS NOT NULL AND [M].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [CO].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'User' AS [ModuleName],  STRING_AGG(CAST( [USR].[UserName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [User] [USR]
                            WHERE [USR].[UserName] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Product' AS [ModuleName],  STRING_AGG(CAST( [PD].[ProductCodeName] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [Product] [PD]
                            WHERE [PD].[ProductCodeName] IS NOT NULL
                            --UNION ALL
                            --SELECT DISTINCT
                                --'BillDetails' AS [ModuleName],  STRING_AGG(CAST( [T].[Sno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[BD].[Sno]
                                    --FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                                    --LEFT OUTER JOIN {DBName}[SALES].[TBL_SalesBillDetails] [BD] ON [D].[BillID]=[BD].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BD].[Sno] IS NOT NULL
                                --) [T]
                           
                            UNION ALL
                            SELECT DISTINCT
                                'UOM' AS [ModuleName],  STRING_AGG(CAST( [UO].[UOMCode] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM [UOM] [UO]
                            WHERE [UO].[UOMCode] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'Waiter' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM  (
                                    SELECT DISTINCT
                                        [WTR].[{MainFieldName}]
                                    FROM {DBName}[SALES].[TBL_SalesExpiryBreakageDetails] [D]
                                    LEFT OUTER JOIN {DBName}[RESTRO].[TBL_Waiter] [WTR] ON [D].[WaiterID]=[WTR].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [WTR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Batch' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[BAT].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageBatch] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [BAT] ON [D].[BatchID]=[BAT].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BAT].[PID] IS NOT NULL
                                --) [T]
                            --UNION ALL
                            --SELECT DISTINCT
                                --'Serial' AS [ModuleName],  STRING_AGG(CAST( [T].[PID] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            --FROM (
                                    --SELECT DISTINCT
                                         --[SER].[PID]
                                    --FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageSerial] [D]
                                    --LEFT OUTER JOIN {DBName}[GM].[TBL_BatchMaster] [SER] ON [D].[SerialID]=[SER].[ID]
                                    --WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [SER].[PID] IS NOT NULL
                                --) [T]                            
                            UNION ALL
                            SELECT DISTINCT
                                'Transport' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [TR].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_TransportMaster] [TR] ON [D].[TransportID]=[TR].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [TR].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'LoadingPort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [LP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [LP] ON [D].[LoadingPortID]=[LP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [LP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DischargePort' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [DP].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageOtherInfo] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_CommonMaster] [DP] ON [D].[DischargePortID]=[DP].[ID]
                                    WHERE [D].[ID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DP].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'Detail' AS ModuleName,  STRING_AGG(CAST( [DET].[DetailSno] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM Detail [DET]
                            WHERE [DET].[DetailSno] IS NOT NULL
                            UNION ALL
                            SELECT DISTINCT
                                'BillingTerm' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                    SELECT DISTINCT
                                         [BT].[{MainFieldName}]
                                    FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageTerms] [D]
                                    LEFT OUTER JOIN {DBName}[GM].[TBL_BillingTerm] [BT] ON [D].[BTID]=[BT].[ID]
                                    WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [BT].[{MainFieldName}] IS NOT NULL
                                ) [T]
                            UNION ALL
                            SELECT DISTINCT
                                'DocumentType' AS [ModuleName],  STRING_AGG(CAST( [T].[{MainFieldName}] AS NVARCHAR(MAX)),'|;|') AS [Names]
                            FROM (
                                SELECT DISTINCT
                                     [DT].[{MainFieldName}]
                                FROM  {DBName}[SALES].[TBL_SalesExpiryBreakageDocs] [D]
                                LEFT OUTER JOIN {DBName}[GM].[TBL_DocumentType] [DT] ON [D].[DocumentTypeID]=[DT].[ID]
                                WHERE [D].[MasterID] IN (SELECT [T].[ID]  FROM @Data [T])  AND [DT].[{MainFieldName}] IS NOT NULL
                            ) [T]
                            
                        )
                        
                        SELECT 
                            [ModuleName], [Names]	
                        FROM [FinalData]
                        WHERE [Names] IS NOT NULL AND [Names] <> ''
                        """;

            return (SelectQuery, InsertQuery, utdtTableName, SelectDependentQuery);
        }


        #endregion
    }
}
