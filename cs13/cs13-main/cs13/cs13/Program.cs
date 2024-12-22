using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class CurrencyServer
{
    static Dictionary<string, double> currencyRates = new Dictionary<string, double>
    {
        { "USD_EURO", 0.92 },
        { "EURO_USD", 1.09 },
        { "USD_UAH", 36.92 },
        { "UAH_USD", 0.027 },
        { "EURO_UAH", 40.21 },
        { "UAH_EURO", 0.025 }
    };

    static void Main()
    {
        try
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Сервер курсу валют запущений.");
            Console.WriteLine("Очікування підключення клієнта...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Клієнт підключено.");
                NetworkStream stream = client.GetStream();

                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    Console.WriteLine($"Запит від клієнта: {request}");

                    string response = HandleRequest(request);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);

                    Console.WriteLine($"Відповідь надіслано: {response}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
                finally
                {
                    client.Close();
                    Console.WriteLine("Клієнт відключено.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критична помилка: {ex.Message}");
        }
    }

    static string HandleRequest(string request)
    {
        if (currencyRates.TryGetValue(request.ToUpper(), out double rate))
        {
            return $"Курс {request}: {rate}";
        }
        return "Запит некоректний або курс недоступний.";
    }
}
