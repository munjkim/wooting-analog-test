/*

using System;
using WootingAnalogSDKNET;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace analog_test
{
	class Program
	{
		static void callback(DeviceEventType eventType, DeviceInfo deviceInfo) {
			Console.WriteLine($"Device event cb called with: {eventType} {deviceInfo}");
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Hello Analog SDK!");

			// Initialise the SDK
			var (noDevices, error) = WootingAnalogSDK.Initialise();

			// If the number of devices is at least 0 it indicates the initialisation was successful
			if (noDevices >= 0) {
				Console.WriteLine($"Analog SDK Successfully initialised with {noDevices} devices!");

				// Subscribe to the DeviceEvent
				WootingAnalogSDK.DeviceEvent += callback;


				// Get a list of the connected devices and Associated information
				var (devices, infoErr) = WootingAnalogSDK.GetConnectedDevicesInfo();
				if (infoErr != WootingAnalogResult.Ok)
					Console.WriteLine($"Error getting devices: {infoErr}");

				foreach (DeviceInfo device in devices)
				{
					Console.WriteLine($"Device info has: {device}");
				}

				Console.Write("User name: ");
				string userName = Console.ReadLine();
				Console.Write("User ID: ");
				string userID = Console.ReadLine();
			
				// Ensure the ./data directory exists
				string directoryPath = "./data";
				if (!Directory.Exists(directoryPath)) {
					Directory.CreateDirectory(directoryPath);
				}
				
				// Ensure the CSV file has a header if it's a new file
				string fileName = $"{directoryPath}/{userID}_{userName}_{DateTime.Now:yyyyMMdd}.csv";
				if (!File.Exists(fileName)) {
					using (StreamWriter sw = File.CreateText(fileName)) {
						sw.WriteLine("unixtime,keyCode,keyValue");
					}
				}

				// This can be used to make the SDK give you keycodes from the Windows Virtual Key set that are translated based on the language set in Windows
				// By default the keycodes the SDK will give you are the HID keycodes
				//WootingAnalogSDK.SetKeycodeMode(KeycodeType.VirtualKeyTranslate);

				while (true) {
					var (keys, readErr) = WootingAnalogSDK.ReadFullBuffer(50);
					
					if (readErr == WootingAnalogResult.Ok)
					{
						long unixTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
						
						// if (keys.Count > 0) {
						// 	// Print the unixtimestamp
						// 	Console.Write($"UnixTimestamp: {DateTimeOffset.Now.ToUnixTimeMilliseconds()} - ");
						// }
						
						foreach (var analog in keys)
						{
							Console.Write($"{unixTimestamp} : {analog.Item1},{analog.Item2}\n");
							
							// Append the data to the CSV file
							using (StreamWriter sw = File.AppendText(fileName)) {
								sw.WriteLine($"{unixTimestamp},{analog.Item1},{analog.Item2}");
							}

						}
						
						// if (keys.Count > 0){
						// 	Console.WriteLine();
						// }
						
					}
					else
					{
						Console.WriteLine($"Read failed with {readErr}");
						// We want to put more of a delay in when we get an error as we don't want to spam the log with the errors
						Thread.Sleep(1000);
					}
					// We want to have a bit of a delay so we don't spam the console with new values
					// 1 milli second = 1000 micro second
					Thread.Sleep(1);
				}
			}
			else {
				Console.WriteLine($"Analog SDK failed to initialise: {error}");
			}
		}
	}
}
*/

using System;
using System.Diagnostics;
using WootingAnalogSDKNET;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace analog_test
{
	class Program
	{
		static void callback(DeviceEventType eventType, DeviceInfo deviceInfo) {
			Console.WriteLine($"Device event cb called with: {eventType} {deviceInfo}"); 
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Hello Analog SDK!");

			// Initialise the SDK
			var (noDevices, error) = WootingAnalogSDK.Initialise();

			// If the number of devices is at least 0 it indicates the initialisation was successful
			if (noDevices >= 0) {
				Console.WriteLine($"Analog SDK Successfully initialised with {noDevices} devices!");

				// Subscribe to the DeviceEvent
				WootingAnalogSDK.DeviceEvent += callback;

				// Get a list of the connected devices and Associated information
				var (devices, infoErr) = WootingAnalogSDK.GetConnectedDevicesInfo();
				if (infoErr != WootingAnalogResult.Ok)
					Console.WriteLine($"Error getting devices: {infoErr}");

				foreach (DeviceInfo device in devices)
				{
					Console.WriteLine($"Device info has: {device}");
				}

				Console.Write("User name: ");
				string userName = Console.ReadLine();
				Console.Write("User ID: ");
				string userID = Console.ReadLine();

				// Ensure the ./data directory exists
				string directoryPath = "./data";
				if (!Directory.Exists(directoryPath)) {
					Directory.CreateDirectory(directoryPath);
				}

				// Ensure the CSV file has a header if it's a new file
				string fileName = $"{directoryPath}/{userID}_{userName}_{DateTime.Now:yyyyMMdd}.csv";
				if (!File.Exists(fileName)) {
					using (StreamWriter sw = File.CreateText(fileName)) {
						sw.WriteLine("unixtime,keyCode,keyValue");
					}
				}

				// Initialize Stopwatch for high-resolution time measurement
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				// This can be used to make the SDK give you keycodes from the Windows Virtual Key set that are translated based on the language set in Windows
				// By default the keycodes the SDK will give you are the HID keycodes
				// WootingAnalogSDK.SetKeycodeMode(KeycodeType.VirtualKeyTranslate);

				while (true) {
					var (keys, readErr) = WootingAnalogSDK.ReadFullBuffer(50);

					if (readErr == WootingAnalogResult.Ok)
					{
						// Calculate the microsecond timestamp
						long microsecondTimestamp = stopwatch.ElapsedTicks * (1000000L / Stopwatch.Frequency);

						foreach (var analog in keys)
						{
							Console.Write($"{microsecondTimestamp} : {analog.Item1},{analog.Item2}\n");

							// Append the data to the CSV file
							using (StreamWriter sw = File.AppendText(fileName)) {
								sw.WriteLine($"{microsecondTimestamp},{analog.Item1},{analog.Item2}");
							}
						}
					}
					else
					{
						Console.WriteLine($"Read failed with {readErr}");
						// We want to put more of a delay in when we get an error as we don't want to spam the log with the errors
						Thread.Sleep(1000);
					}

					// Busy-wait loop for approximately 1 microsecond
					var targetTime = stopwatch.ElapsedTicks + (Stopwatch.Frequency / 1000000L);
					while (stopwatch.ElapsedTicks < targetTime)
					{
						// Busy-wait
					}
				}
			}
			else {
				Console.WriteLine($"Analog SDK failed to initialise: {error}");
			}
		}
	}
}