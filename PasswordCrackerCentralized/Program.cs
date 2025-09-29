using PasswordCrackerDecentralizedSlave.model;
using PasswordCrackerDecentralizedSlave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PasswordCrackerDecentralizedSlave
{
    class Program
    {
        static void Main() {
            //TCP client slave connection 
            Console.WriteLine("Slave client is ready to receive the order!");
            int port = 5000;
            // Connect to the master server on localhost (127.0.0.1)
            TcpClient client = new TcpClient("127.0.0.1", port);

            // Get the network stream to read and write data
            NetworkStream stream = client.GetStream();
            StreamReader streamReader = new StreamReader(stream);
            StreamWriter streamWriter = new StreamWriter(stream) { AutoFlush = true };
            // Create an instance of your cracking class
            Cracking cracker = new Cracking();


            // Read each line sent by the master until it signals the end
            List<UserInfoClearText> allResults = new List<UserInfoClearText>();
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line == "The chunks is empty") break; // stop when the chunk ends
                // Run cracking and collect results
                List<UserInfoClearText> results = cracker.RunCracking(line);
                allResults.AddRange(results);

            }

            // Send results back to the master
            foreach (var result in allResults)
            {
                streamWriter.WriteLine($"{result.UserName}:{result.Password}");
            }

            // Signal that results are done
            streamWriter.WriteLine("DONE");


            client.Close();

        }
    }
}
