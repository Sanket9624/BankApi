namespace BankApi.Enums
{
    public enum Permissions
    {
        CreateRole,
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
        ViewCustomTransactions,
        ViewUsersWithStatus,
        ViewPermissions,
        AssignPermissions,
        TwoFactorStatus,
        ViewPersonalDetails,

        // Bank Permissions
        CreateBank,
        UpdateBank,
        DeleteBank,
        ViewBank,

        // Branch Permissions
        CreateBranch,
        UpdateBranch,
        DeleteBranch,
        ViewBranch
    }
}
