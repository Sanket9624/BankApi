-- Get total transaction count
CREATE PROCEDURE sp_GetTotalTransactionCount
AS
BEGIN
    SELECT COUNT(*) AS TotalTransactions FROM Transactions;
END;
GO

-- Get all transactions
CREATE PROCEDURE sp_GetAllTransactions
AS
BEGIN
    SELECT * FROM Transactions ORDER BY TransactionDate DESC;
END;
GO

-- Get all deposit transactions
CREATE PROCEDURE sp_GetDepositTransactions
AS
BEGIN
    SELECT * FROM Transactions WHERE Type = 'Deposit' ORDER BY TransactionDate DESC;
END;
GO

-- Get all withdrawal transactions
CREATE PROCEDURE sp_GetWithdrawTransactions
AS
BEGIN
    SELECT * FROM Transactions WHERE Type = 'Withdraw' ORDER BY TransactionDate DESC;
END;
GO

-- Get total deposit amount
CREATE PROCEDURE sp_GetTotalAmountDeposited
AS
BEGIN
    SELECT SUM(Amount) AS TotalDeposited FROM Transactions WHERE Type = 'Deposit';
END;
GO

-- Get total withdrawal amount
CREATE PROCEDURE sp_GetTotalAmountWithdrawn
AS
BEGIN
    SELECT SUM(Amount) AS TotalWithdrawn FROM Transactions WHERE Type = 'Withdraw';
END;
GO

-- Get total balance in all accounts
CREATE PROCEDURE sp_GetTotalBankBalance
AS
BEGIN
    SELECT SUM(Balance) AS TotalBalance FROM Account;
END;
GO

-- Get all transactions with user and account details
CREATE PROCEDURE sp_GetAllTransactionsWithDetails
AS
BEGIN
    SELECT 
        T.TransactionID,
        U.UserID,
        U.FirstName + ' ' + U.LastName as fullName,
        A.AccountNumber,
        A.AccountType,
        T.Amount,
        T.Type AS TransactionType,
        T.TransactionDate
    FROM Transactions T
    JOIN Account A ON T.SenderAccountId = A.AccountId OR T.ReceiverAccountId = A.AccountId
    JOIN Users U ON A.UserID = U.UserID
    ORDER BY T.TransactionDate DESC;
END;
GO

-- Get total deposits and withdrawals per user
CREATE PROCEDURE sp_GetTotalAmountPerUser
AS
BEGIN
SELECT 
    U.UserID,
    U.FirstName + ' ' + U.LastName AS fullName,
    SUM(CASE WHEN T.Type = 'Deposit' THEN T.Amount ELSE 0 END) AS TotalDeposited,
    SUM(CASE WHEN T.Type = 'Withdraw' THEN T.Amount ELSE 0 END) AS TotalWithdrawn
FROM Users U
JOIN Account A ON U.UserID = A.UserID
JOIN Transactions T ON A.AccountID = T.SenderAccountId OR A.AccountID = T.ReceiverAccountId
GROUP BY U.UserID, U.FirstName, U.LastName
ORDER BY TotalDeposited DESC;

END;
GO

-- Get bank manager overview with total users, transactions, deposits, withdrawals, and balance
CREATE PROCEDURE sp_GetBankManagerOverview
AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM Users) AS TotalUsers,
        (SELECT COUNT(*) FROM Account) AS TotalAccounts,
        (SELECT COUNT(*) FROM Transactions) AS TotalTransactions,
        (SELECT SUM(Amount) FROM Transactions WHERE Type = 'Deposit') AS TotalDeposited,
        (SELECT SUM(Amount) FROM Transactions WHERE Type = 'Withdraw') AS TotalWithdrawn,
        (SELECT SUM(Balance) FROM Account) AS TotalBankBalance;
END;
GO

-- Get transaction history for a specific user
CREATE PROCEDURE sp_GetUserTransactionHistory
    @UserID INT
AS
BEGIN
    SELECT 
        T.TransactionID,
        T.SenderAccountId,
        T.ReceiverAccountId,
        T.Amount,
        T.Type AS TransactionType,
        T.TransactionDate
    FROM Transactions T
    LEFT JOIN Account A ON T.SenderAccountId = A.AccountId OR T.ReceiverAccountId = A.AccountId
    WHERE A.UserID = @UserID
    ORDER BY T.TransactionDate DESC;
END;
GO

-- Get all accounts with user information
CREATE PROCEDURE sp_GetAllAccountsWithUserDetails
AS
BEGIN
    SELECT 
        A.AccountId,
        A.AccountNumber,
        A.AccountType,
        A.Balance,
        U.UserID,
        U.FirstName + ' ' + U.LastName as fullName,
        U.Email
    FROM Account A
    JOIN Users U ON A.UserID = U.UserID
    ORDER BY A.Balance DESC;
END;
GO



