using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleEvents
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("To Exit  Console: 'q'");
			Console.WriteLine("To Clear Console: 'c'");
			Console.WriteLine(" Send Payload: 's'");
			Console.WriteLine("=============================");
			Console.WriteLine("");
			Console.ResetColor();

			EventClient tClient = new EventClient();
			
			tClient.Execute();
			//tClient.AuthNamespaceExample();
			bool run = true;
			while (run)
			{
				string line = Console.ReadLine();
				if (!string.IsNullOrWhiteSpace(line))
				{
					char key = line.FirstOrDefault();
					
					switch (key)
					{
						case 'c':
						case 'C':
							Console.Clear();
							break;
						case 's':
						case 'S':
							tClient.SendMessageSamples();
							break;

						case 'q':
						case 'Q':
							run = false;
							break;
						
						
						default:
							break;
					}
				}
			}
			tClient.Close();
		}
	}
}
