using iCodify.Core.Extensions;
using iCodify.Dal;
using iCodify.Dal.Enums;
using System.Net.NetworkInformation;
using System.Text;
using static System.Console;

namespace iCodify.UI.TestConsole
{
	//internal class Program
	//{
	//    static void Main(string[] args)
	//    {
	//        WriteLine("Hello, World!");
	//        WriteLine("I'm okay with C#\n");
	//        Write("Start");
	//        WriteLine("Finish");
	//    }
	//}

	internal class Program
	{
		//[STAThread]
		static async Task Main(string[] args)
		{
			while (true)
			{
				try
				{
					ShowMenu();

					switch (ReadKey().KeyChar)
					{
						case '1': await OpperPostAsync(); break;
						case '2': OpperPost(); break;
						case '3': ShowMACAddress(); break;
						case '0': WriteLine(); return;

						default: ShowMenu(); break;
					}
				}
				catch (Exception ex)
				{
					WriteLine();
					WriteLine("There was an error: ");
					while (ex != null)
					{
						WriteLine(ex.Message);
						ex = ex.InnerException;
					}
				}

				WriteLine();
				WriteLine("Press <ENTER> to return to menu.");
				ReadLine();
			}
		}

		public static void ShowMenu()
		{
			Clear();
			WriteLine("PREREQ:");
			WriteLine("1. Set first req here,");
			WriteLine("2. Set second req here");
			WriteLine("");
			WriteLine("- Select which command to execute.");
			WriteLine("");
			WriteLine(" 1) Post to Opper Asynchronously.");
			WriteLine(" 2) Post to Opper Synchronously.");
			WriteLine(" 3) Show MAC Address.");
			WriteLine(" 0) Exit");

			WriteLine("");
			Write(" > ");
		}

		private static async Task OpperPostAsync()
		{
			WriteLine("\nWhat would you like to ask?");
			var question = ReadLine();

			var apiData = APIInfo.GetAPIData(APIModels.Opper);
			string jsonBody = $"{{\"name\": \"respond\", \"input\": \"{question}\"}}";


			using var client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(30)
			};

			var request = new HttpRequestMessage(HttpMethod.Post, apiData.Endpoint)
			{
				Content = new StringContent(jsonBody, Encoding.UTF8, apiData.MediaType)
			};

			request.Headers.Add("Authorization", $"Bearer {apiData.APIKey}");

			WriteLine("Sending request...");
			Thread.Sleep(1000);
			WriteLine($"Using the {apiData.Model} model - {apiData.Model.GetDescription()}");

			HttpResponseMessage response = await client.SendAsync(request);

			string responseBody = await response.Content.ReadAsStringAsync();

			WriteLine($"Status Code: {(int)response.StatusCode}");
			WriteLine("Response: " + responseBody);
		}

		private static void OpperPost()
		{
			string apiKey = "op-ESZCBUT1VRVO2F3TGTI8";
			string jsonBody = "{\"name\": \"respond\", \"input\": \"Please suggest a way to build a biological computer?\"}";

			using var client = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(30)
			};

			var request = new HttpRequestMessage(HttpMethod.Post, "https://api.opper.ai/v2/call")
			{
				Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
			};
			request.Headers.Add("Authorization", $"Bearer {apiKey}");

			// Skicka begäran synkront
			HttpResponseMessage response = client.Send(request);

			string responseBody = response.Content.ReadAsStringAsync().Result;

			WriteLine($"Status Code: {(int)response.StatusCode}");
			WriteLine("Response: " + responseBody);
		}

		public static void ShowMACAddress()
		{
			//var macAddr = NetworkInterface.GetAllNetworkInterfaces()
			//              .Where(ni => (ni.OperationalStatus == OperationalStatus.Up
			//                        && (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
			//                        || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)))
			//              .Select(ni => ni.GetPhysicalAddress().ToString())
			//              .FirstOrDefault();

			//var wifiMacAddr = NetworkInterface.GetAllNetworkInterfaces()
			//                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
			//                    .SelectMany(ni => ni.GetPhysicalAddress().ToString());

			var interfaces = NetworkInterface.GetAllNetworkInterfaces()
								.Where(ni => ni.OperationalStatus == OperationalStatus.Up);

			WriteLine($"\n\nThe Ethernet MAC address in use is: {GetFirstMACAddress()}\n");

			WriteLine("The addresses are:\n");

			foreach (var item in interfaces)
			{
				WriteLine($"Name: {item.Name}, Type: {item.NetworkInterfaceType}, MAC address: {item.GetPhysicalAddress().ToString()}");
			}
		}

		private static string GetFirstMACAddress()
		{
			return NetworkInterface.GetAllNetworkInterfaces()
						  .Where(ni => (ni.OperationalStatus == OperationalStatus.Up
									&& (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet
									|| ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)))
						  .Select(ni => ni.GetPhysicalAddress().ToString())
						  .FirstOrDefault() ?? string.Empty;
		}
	}
}
