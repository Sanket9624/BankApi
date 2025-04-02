namespace BankApi.Enums
{
    public enum PermissionEnum
    {
        CreateRole = 1,
        DeleteRole,
        ViewRoles,

        CreateManager,
        DeleteManager,
        VerifyManager,

        ApproveAccount,
        ViewUsers,

        UpdateUser,
        DeleteUser,

        ApproveTransaction,
        RejectTransaction,
        ViewPendingTransactions,

        BankSummary,
        GetUserAccountDetails,
        GetUserDetailsByAccountNumber,
        GetUserDetailsByEmail,
        GetTotalAccounts,
        GetTransactions,

        MakeDeposit,
        MakeWithdrawal,
        MakeTransfer,
        ViewBalance,
        ViewTransactions,
        ViewCustomTransactions
    }
}
