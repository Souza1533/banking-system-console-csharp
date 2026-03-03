using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;

class Client
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; }
    public int Age { get; }

    public Client(string name, int age)
    {
        Name = name;
        Age = age;
    }
}

class BankAccount
{
    public int Id { get; }
    public Client Owner { get; }
    private decimal _balance;
    public decimal Balance => _balance;

    public BankAccount(int id, Client owner)
    {
        Id = id;
        Owner = owner;
        _balance = 0m;
    }

    public bool Deposit(decimal value)
    {
        if (value <= 0)
            return false;

        _balance += value;
        return true;
    }

    public bool Withdraw(decimal value)
    {
        if (value <= 0 || value > _balance)
            return false;

        _balance -= value;
        return true;
    }

    public bool TransferTo(decimal value, BankAccount destinatary)
    {
        if (destinatary == null)
            return false;

        if (value <= 0 || value > _balance)
            return false;

        _balance -= value;
        destinatary.Deposit(value);
        return true;
    }
}

interface IAccountStorage
{
    IReadOnlyList<BankAccount> BankAccounts { get; }
    void Add(BankAccount account);
    BankAccount CreateAccount(Client client);
    bool HasAccounts();
    BankAccount FindAccountByID(int id);
    void Remove(BankAccount account);
}

class AccountStorage : IAccountStorage
{
    private readonly List<BankAccount> _accounts = new();
    private int _nextId = 1;

    public IReadOnlyList<BankAccount> BankAccounts => _accounts;

    public void Add(BankAccount account)
    {
        _accounts.Add(account);
    }

    public BankAccount CreateAccount(Client client)
    {
        var account = new BankAccount(_nextId++, client);

        _accounts.Add(account);
        return account;
    }

    public BankAccount FindAccountByID(int id)
    {
        foreach (var account in _accounts)
        {
            if (account.Id == id)
                return account;
        }
        return null;
    }

    public bool HasAccounts()
    {
        return _accounts.Any();
    }

    public void Remove(BankAccount account)
    {
        _accounts.Remove(account);
    }
}

class ConsoleUI
{
    private readonly IAccountStorage _accounts;

    public ConsoleUI(IAccountStorage storage)
    {
        _accounts = storage;
    }

    public int ReadMenuChoice()
    {
        int value;
        while (!int.TryParse(Console.ReadLine(), out value))
        {
            Console.Write("❌ Opção inválida. Digite um número válido: ");
        }
        return value;
    }

    public int ReadInteger()
    {
        int value;
        while (!int.TryParse(Console.ReadLine(), out value))
        {
            Console.Write("❌ Opção inválida. Digite um número válido: ");
        }
        return value;
    }


    public decimal ReadDecimal()
    {
        decimal value;

        while (true)
        {
            var input = Console.ReadLine();
            input = input.Replace(',', '.');

            if (decimal.TryParse(
                input,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out value))
            {
                return value;
            }

            Console.Write("\n❌ Valor inválido. Digite um número válido: ");
        }
    }

    public string ReadPersonName(string errorMessage)
    {
        while (true)
        {
            string input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.Write(errorMessage);
                continue;
            }

            bool isValid = input.All(c => char.IsLetter(c) || c == ' ');

            if (isValid)
                return input;

            Console.Write(errorMessage);
        }
    }

    public void CreateClientAndAccount()
    {
        Console.Write("Name -> ");
        var name = ReadPersonName("❌ Nome inválido: ");

        Console.Write("\nAge -> ");
        var age = ReadInteger();
        
        if (age <= 0 || age > 120)
        {
            Console.WriteLine("❌ Idade inválida.");
            return;
        }

        var client = new Client(name, age);
        _accounts.CreateAccount(client);

        Console.WriteLine("\n✅ Conta criada com sucesso!\n");
    }

    public BankAccount CheckID()
        {
            var id = ReadInteger();

            BankAccount FoundAccount = _accounts.FindAccountByID(id);
            return FoundAccount;
        }

    public void PrintAllAccounts()
    {
        if (_accounts.HasAccounts())
        {
            foreach (var account in _accounts.BankAccounts)
            {
                Console.WriteLine($"Conta ID: {account.Id} | Cliente: {account.Owner.Name} | Saldo: {account.Balance}");
            }
        }
        else
        {
            Console.WriteLine("\n❌ Nenhuma conta criada.\n");
            CreateClientAndAccount();
        }
    }

    private void ManageAccount()
    {
        if(!_accounts.HasAccounts())
        {
            Console.WriteLine("\n❌ Nenhuma conta criada.\n");
            return;
        }

        BankAccount selectedAccount = null;

        while (selectedAccount == null)
        {
            Console.WriteLine("\n----= Contas Disponíveis =----\n");
            PrintAllAccounts();

            Console.Write("\nSelecione o ID da conta:\n-> ");
            selectedAccount = CheckID();

            if (selectedAccount == null)
                Console.WriteLine("\n❌ Conta não encontrada.");
        }

        int index;
        do
        {
            Console.WriteLine($"\n----= Gerenciando Conta: {selectedAccount.Owner.Name} =----");
            Console.WriteLine("1- Depositar");
            Console.WriteLine("2- Transferir");
            Console.WriteLine("3- Deletar");
            Console.Write("\n4- VOLTAR\n\n-> ");

            index = ReadMenuChoice();

            switch (index)
            {
                case 1:
                    Console.Write("\nValor para depósito:\n-> ");
                    var money = ReadDecimal();

                    if (selectedAccount.Deposit(money))
                        Console.WriteLine("✅ Depósito concluído!\n");
                    else
                        Console.WriteLine("❌ Depósito inválido.\n");
                    break;

                case 2:

                    if (_accounts.BankAccounts.Count < 2) 
                    { 
                        Console.WriteLine("❌ E necessario ter 2 contas pelomenos para transferir!\n");
                        break;
                    }
                    
                    Console.Write("\nValor para transferência:\n-> ");
                    var value = ReadDecimal();


                    BankAccount destinatary = null;

                    while (destinatary == null)
                    {
                        PrintAllAccounts();
                        Console.Write("\nConta de destino (ID):\n-> ");

                        int destinataryId = ReadInteger();
                        destinatary = _accounts.FindAccountByID(destinataryId);

                        if (destinatary == null){
                            Console.WriteLine("❌ Conta não encontrada.");
                        }
                        else if (destinatary.Id == selectedAccount.Id)
                        {
                            Console.WriteLine("❌ Não é possível transferir para a mesma conta.");
                            destinatary = null;
                        }
                    }

                    if (selectedAccount.TransferTo(value, destinatary))
                        Console.WriteLine("✅ Transferência concluída!\n");
                    else
                        Console.WriteLine("❌ Transferência inválida.\n");
                    break;

                case 3:
                    Console.WriteLine("Tem certeza que deseja excluir sua conta?\n");
                    Console.Write("1 - Confirmar     2- Cancelar\n -> ");

                    int confirm;
                    do
                    {
                        confirm = ReadMenuChoice();

                        if (confirm == 1)
                        {
                            _accounts.Remove(selectedAccount);
                            Console.WriteLine("✅ Conta excluída com sucesso!");
                            return;
                        }
                        else if (confirm != 2)
                        {
                            Console.Write("\n❌ Valor inválido. Digite 1 ou 2: ");
                        }

                    } while (confirm != 2);

                break;

            }
        } while (index != 4);
    }

    public void DeleteAccount()
    {
        if (!_accounts.HasAccounts())
        {
            Console.WriteLine("\n❌ Nenhuma conta criada.\n");
            CreateClientAndAccount();
        }

        BankAccount selectedAccount = null;

        while (selectedAccount == null)
        {
            Console.WriteLine("\n----= Contas Disponíveis =----\n");
            PrintAllAccounts();

            Console.Write("\nSelecione o ID da conta que deseja remover:\n-> ");
            selectedAccount = CheckID();

            if (selectedAccount == null)
            {
                Console.WriteLine("\n❌ Conta não encontrada.");
            }
        
            else
            {
                _accounts.Remove(selectedAccount);
                Console.WriteLine("✅ Conta excluida com sucesso!");
                return;
            }
        }
    }

    public void Run()
    {
        int menuChoice;
        do
        {
            Console.WriteLine("\n===== Bank System =====");
            Console.WriteLine("1- Gerenciar Conta");
            Console.WriteLine("2- Criar Conta");
            Console.WriteLine("3- Listar Contas");
            Console.WriteLine("4- Deletar Conta");
            Console.Write("\n5- ENCERRAR SISTEMA\n\n-> ");

            menuChoice = ReadMenuChoice();

            switch (menuChoice)
            {
                case 1: ManageAccount(); break;
                case 2: CreateClientAndAccount(); break;
                case 3: PrintAllAccounts(); break;
                case 4: DeleteAccount(); break;
            }
        } while (menuChoice != 5);
    }
}

class Program
{
    static void Main()
    {
        IAccountStorage storage = new AccountStorage();
        new ConsoleUI(storage).Run();
    }
}