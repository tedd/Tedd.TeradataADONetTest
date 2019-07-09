using System;
using System.Data.SqlClient;
using Teradata.Client.Provider;

namespace Tedd.TeradataADONetTest
{
    class Program
    {
        static void Main(string[] args)
        {
        // Loosely based on https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-code-examples
            var user = Environment.GetEnvironmentVariable("USERNAME");
            Console.Write($"User [{user}]: ");
            var userIn = Console.ReadLine();
            user = String.IsNullOrWhiteSpace(userIn) ? user : userIn;

            Console.Write("Password: ");
            var pass = Console.ReadLine();

            var connStrbBuilder = new TdConnectionStringBuilder()
            {
                DataSource = "server",
                UserId = user,
                Password = pass,
                // Optional
                AuthenticationMechanism = "LDAP",
                ConnectionPooling = true,
                DataEncryption = true,
                IntegratedSecurity = false,
                ResponseBufferSize = 7340000
            };
            Console.WriteLine("Connection string:");
            Console.WriteLine(connStrbBuilder.ConnectionString);
            Console.WriteLine();
            Console.WriteLine("Result:");
            // "Data Encryption=True;Authentication Mechanism=LDAP;Response Buffer Size=7340000;User Id=xxx;Data Source=xxx;Password=xxxx;Connection Pooling=True;Integrated Security=False"


            // Provide the query string with a parameter placeholder.
            string queryString =
                "SELECT * FROM DBC.TablesV "
                    + "WHERE TableKind = ? "
                    + "SAMPLE 10;";

            // Specify the parameter value.
            var param = new TdParameter("", "V");

            // Create and open the connection in a using block. This
            // ensures that all resources will be closed and disposed
            // when the code exits.
            using (var connection = new TdConnection(connStrbBuilder.ConnectionString))
            {
                // Create the Command and Parameter objects.
                var command = new TdCommand(queryString, connection);
                command.Parameters.Add(param);

                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.ReadLine();
            }

        }
    }
}
