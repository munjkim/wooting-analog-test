using System;
using WootingAnalogSDKNET;
using System.Threading;
using System.Collections.Generic;

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

				// This can be used to make the SDK give you keycodes from the Windows Virtual Key set that are translated based on the language set in Windows
				// By default the keycodes the SDK will give you are the HID keycodes
				//WootingAnalogSDK.SetKeycodeMode(KeycodeType.VirtualKeyTranslate);

				while (true) {
					var (keys, readErr) = WootingAnalogSDK.ReadFullBuffer(20);
					if (readErr == WootingAnalogResult.Ok)
					{
						
						foreach (var analog in keys)
						{
							// Not just millisecond, but also second, minute, hour, day, month, year
							Console.Write($"Timestamp: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - ");
							Console.Write($"({analog.Item1},{analog.Item1111112}) ");
							
							// We want to put a space between each key
							Console.Write(" ");
						}

						if (keys.Count > 0)
							Console.WriteLine();
					}
					else
					{
						Console.WriteLine($"Read failed with {readErr}");
						// We want to put more of a delay in when we get an error as we don't want to spam the log with the errors
						Thread.Sleep(1000);
					}

					// We want to have a bit of a delay so we don't spam the console with new values
					// This is just milliseconds, so it's not a long delay.
					// How about nanoseconds? No, that's too fast.

					Thread.Sleep(1);
				}
			}
			else {
				Console.WriteLine($"Analog SDK failed to initialise: {error}");
			}
		}
	}
}