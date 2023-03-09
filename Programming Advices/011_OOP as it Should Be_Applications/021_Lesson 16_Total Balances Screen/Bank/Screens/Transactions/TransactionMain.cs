﻿using Lab.Bank.Screens.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.Bank.Screens.Transactions
{
    internal class TransactionMain : Screen
    {
        private static string[] _transactionsMenueOptions()
        {
            string[] mainMenueOptions = new string[4];
            mainMenueOptions[0] = "Deposit.";
            mainMenueOptions[1] = "Withdraw.";
            mainMenueOptions[2] = "Total Balance.";
            mainMenueOptions[3] = "Main Menue.";

            return mainMenueOptions;
        }
        private static void _printTransactionsMainMenu()
        {
            Console.Clear();
            drawScreenHeader("Transactions Main Menue Screen");
            printMenueOptions(_transactionsMenueOptions());
            printBreakLine("=", 100);
            Console.WriteLine();
        }
        public static void startTransactionsMenu()
        {
            int userChoose = 0;

            do
            {
                Console.Clear();
                _printTransactionsMainMenu();
                userChoose = readUserMenuChoose(false);

                switch (userChoose)
                {
                    case 1:
                        Deposit.deposit();
                        goBack();
                        break;
                    case 2:
                        Withdraw.withDraw();
                        goBack();
                        break;
                    case 3:
                        TotalBalances.ShowTotalBalances();
                        goBack();
                        break;

                    default:
                        break;
                }
            } while (userChoose != 4);
        }
    }
}