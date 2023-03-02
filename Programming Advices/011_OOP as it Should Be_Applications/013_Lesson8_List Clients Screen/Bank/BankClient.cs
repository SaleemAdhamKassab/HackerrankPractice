using Lab.Classes;
using Labs.Bank.Db;
using Labs.Bank.Screens;

namespace Lab.Class.Bank
{
    public class BankClient : Person
    {
        public BankClient(string accountNumber,
                          string pinCode,
                          string firstName,
                          string lastName,
                          string email,
                          string phone,
                          double accountBalance,
                          enMode mode) : base(firstName, lastName, email, phone)
        {
            Mode = mode;
            AccountNumber = accountNumber;
            PinCode = pinCode;
            AccountBalance = accountBalance;
        }
        private readonly enMode Mode;
        public string AccountNumber { get; set; }
        public string PinCode { get; set; }
        public double AccountBalance { get; set; }
        private static BankClient _getEmptyClientObject() => new BankClient("", "", "", "", "", "", 0, enMode.EmptyMode);
        public static BankClient convertLineToClientObject(string lineData, string seperator)
        {
            string[] splitedLineData = lineData.Split(seperator);

            return new BankClient(splitedLineData[0],
                                  splitedLineData[1],
                                  splitedLineData[2],
                                  splitedLineData[3],
                                  splitedLineData[4],
                                  splitedLineData[5],
                                  double.Parse(splitedLineData[6]),
                                  enMode.UpdateMode);
        }
        public enum enMode { EmptyMode, UpdateMode }
        private static object _readClientOneInfo(string msg)
        {
            Console.Write(msg);
            return Console.ReadLine();
        }
        private static BankClient _readClientInfo(string accountNumber = "") =>
            new BankClient(accountNumber == null ? _readClientOneInfo("Enter Account Number: ").ToString() : accountNumber,
                _readClientOneInfo("Enter Pin Code: ").ToString(),
                _readClientOneInfo("Enter First Name: ").ToString(),
                _readClientOneInfo("Enter Last Name: ").ToString(),
                _readClientOneInfo("Enter Email: ").ToString(),
                _readClientOneInfo("Enter Phone: ").ToString(),
                double.Parse(_readClientOneInfo("Enter Account Balance: ").ToString()),
                enMode.UpdateMode);
        private static string _clientRecordForSaving(BankClient client)
        {
            return
                client.AccountNumber + "#//#" +
                client.PinCode + "#//#" +
                client.FirstName + "#//#" +
                client.LastName + "#//#" +
                client.Email + "#//#" +
                client.Phone + "#//#" +
                client.AccountBalance;
        }
        private static void _updateClientMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Update Clients Info: ");
            Console.WriteLine("_____________________");
        }
        private static char _confirmationMessage(string msg, string accountNumber)
        {
            Console.WriteLine("Are you sure to " + msg + " Acc. ({0}) Y/N ?", accountNumber);
            return char.Parse(Console.ReadLine().ToLower());
        }
        private static void _printBalanceTableHeader(int fileRwosCount, int lineLength)
        {
            string header = "Balances List (" + fileRwosCount + ") Client(s).";
            Console.SetCursorPosition((Console.WindowWidth - header.Length) / 2, Console.CursorTop);
            Console.WriteLine(header);

            Screen.printBreakLine("_", lineLength);
            Console.WriteLine();
            Console.WriteLine(
                Screen.padRight("Acc. Num", 20, ' ') +
                Screen.padRight("First Name", 20, ' ') +
                Screen.padRight("Balance", 20, ' '));

            Screen.printBreakLine("_", lineLength);
            Console.WriteLine();
        }
        private static void _printClienBalancetRow(BankClient client)
        {
            Console.Write(Screen.padRight(client.AccountNumber, 20, ' '));
            Console.Write(Screen.padRight(client.FirstName, 20, ' '));
            Console.Write(Screen.padRight(client.AccountBalance.ToString(), 20, ' '));
            Console.WriteLine();
        }
        private static int _clientsTotalBalances()
        {
            int totalBalances = 0;

            List<object> clientsList = FileDbContext.convertFileDataToList(FileDbContext.ConnectionString, "#//#");

            foreach (BankClient client in clientsList)
                totalBalances += int.Parse(client.AccountBalance.ToString());

            return totalBalances;
        }

        //CRUD
        public static bool Add()
        {
            try
            {
                string accountNumber = _readClientOneInfo("Enter Account Number: ").ToString();

                while (IsClientExist(accountNumber))
                    accountNumber = _readClientOneInfo("Account Number (" + accountNumber + ") is already used, choose another one: ").ToString();

                BankClient client = _readClientInfo(accountNumber);

                FileDbContext.saveRowToFile(_clientRecordForSaving(client));
                Console.WriteLine("Client {0} Added Successfully", accountNumber);

                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }
        public static bool Update()
        {
            string accountNumber = _readClientOneInfo("Enter Client Account Number: ").ToString();

            while (!IsClientExist(accountNumber))
                accountNumber = _readClientOneInfo("Account Number " + accountNumber + " is Not found, Choose another one: ").ToString();


            BankClient client = Find(accountNumber);
            client.Print();
            _updateClientMenu();
            client = _readClientInfo(accountNumber);

            if (_confirmationMessage("update", accountNumber) == 'y')
            {
                Delete(accountNumber, true);
                FileDbContext.saveRowToFile(_clientRecordForSaving(client));
            }

            Console.WriteLine("Client {0} Updated Successfully", accountNumber);

            BankClient updatedClient = Find(client.AccountNumber);
            updatedClient.Print();

            return true;
        }
        public static bool Delete(string accountNumber = "", bool isUpdate = false)
        {
            try
            {
                char c = 'y';

                if (accountNumber == "")
                {
                    accountNumber = _readClientOneInfo("Enter Account Number: ").ToString();

                    while (!IsClientExist(accountNumber))
                        accountNumber = _readClientOneInfo("Account Number is not found, choose another one: ").ToString();
                }

                BankClient clientToDelete = Find(accountNumber);
                clientToDelete.Print();

                if (!isUpdate)
                    c = _confirmationMessage("delete", accountNumber);

                if (c == 'y')
                {
                    List<object> clientsList = FileDbContext.convertFileDataToList(FileDbContext.ConnectionString, "#//#");

                    List<object> clientsListAfterDeleteTheClient = new List<object>();

                    foreach (BankClient client in clientsList)
                        if (client.AccountNumber != accountNumber)
                            clientsListAfterDeleteTheClient.Add(client);

                    FileDbContext.saveListToFile(clientsListAfterDeleteTheClient);

                    if (!isUpdate)
                        Console.WriteLine("Client deleted Successfully");
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public static BankClient Find(string accountNumber, string pinCode = "")
        {
            BankClient client = _getEmptyClientObject();

            List<object> clientsList = FileDbContext.convertFileDataToList(FileDbContext.ConnectionString, "#//#");

            foreach (BankClient clientsLine in clientsList)
            {
                if (pinCode == "")
                {
                    if (clientsLine.AccountNumber == accountNumber)
                    {
                        client = clientsLine;
                        break;
                    }
                }
                else
                {
                    if (clientsLine.AccountNumber == accountNumber && clientsLine.PinCode == pinCode)
                    {
                        client = clientsLine;
                        break;
                    }
                }
            }

            return client;
        }
        public static bool IsClientExist(string accountNumber) => Find(accountNumber).Mode == enMode.EmptyMode ? false : true;
        public void Print()
        {
            Console.WriteLine();
            Console.WriteLine("Client Card: ");
            Console.WriteLine("____________________________________");
            Console.WriteLine("Acc. Number : {0}", AccountNumber);
            Console.WriteLine("Pin Code : {0}", PinCode);
            Console.WriteLine("First Name : {0}", FirstName);
            Console.WriteLine("Last Name : {0}", LastName);
            Console.WriteLine("Full Name : {0}", fullName());
            Console.WriteLine("Email : {0}", Email);
            Console.WriteLine("Phone : {0}", Phone);
            Console.WriteLine("Balance : {0}", AccountBalance);
            Console.WriteLine("____________________________________");
        }
        public static void ShowTotalBalances()
        {
            List<object> clients = FileDbContext.convertFileDataToList(FileDbContext.ConnectionString, "#//#");

            if (clients.Count == 0)
                Console.WriteLine("No Clients found");
            else
            {
                _printBalanceTableHeader(clients.Count, 50);

                foreach (BankClient client in clients)
                    _printClienBalancetRow(client);

                Console.WriteLine();
                Screen.printBreakLine("_", 50);
            }

            int clientsTotalBalances = _clientsTotalBalances();

            Console.WriteLine(("Total Balances = " + clientsTotalBalances).ToString().PadLeft(45));
            Console.WriteLine(("( " + Util.NumberToText(clientsTotalBalances) + " )").PadLeft(33));
        }
    }
}