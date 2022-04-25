using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Media;
using System.Net;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DISP
{
    partial class Program
    {
        static MqttClient client;
        static bool pillsTaken = false;
        static bool pillsPutBack = false;
        static int numberOfAlotMovements = 0;
        static string previousEvent = "tmp";

        static void Main(string[] args)
        {
            #region Connection

            client = new MqttClient("2fefc75adc594dab9da34a7efcbfc6df.s1.eu.hivemq.cloud", 8883, true, null, null, MqttSslProtocols.TLSv1_2);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.Connect("Morten_Dalsgaard", "HiveMQJalle", "DankMayMays123");
            Console.WriteLine("Client Connection status: " + client.IsConnected);
            #endregion

            #region Subscriptions
            client.Subscribe(new string[] { "zigbee2mqtt/0x680ae2fffec0cbbb", "MotionSensor", "WeightSensor" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            #endregion

            var led1 = new LED();
            led1.state = "OFF";
            client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led1)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            #region UI
            /*
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Osteoporosis medication tracking software user interface\n");

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            while (true)
            {


                Console.WriteLine("** Settings Menu **\n");
                Console.WriteLine("1. LED settings");

                var result = int.Parse(Console.ReadLine());
                switch (result)
                {
                    case 1:
                        Console.WriteLine("Type ON for turning the LED ON and type OFF for turning it OFF");
                        var res = Console.ReadLine();
                        if (res == "ON")
                        {
                            //var msg = "{\"state\":\"ON\",\"color\":{\"r\":200, \"g\":0, \"b\":0}}";
                            var led = new LED();
                            led.brightness = 254;
                            led.state = "ON";
                            led.color_mode = "rgb";
                            var col = new Color();
                            col.r = 200;
                            col.b = 0;
                            col.g = 0;
                            led.color = col;
                            client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                            Console.WriteLine("Turned LED ON");
                        }
                        if (res == "OFF")
                        {
                            //var msg = "{\"state\":\"OFF\"}";
                            var led = new LED();
                            led.state = "OFF";
                            client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                            Console.WriteLine("Turned LED OFF");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid Input \n");
                        break;
                }
                Console.ReadLine();
            }
            */
            #endregion

            var led3 = new LED();
            led3.brightness = 254;
            led3.state = "ON";
            led3.color_mode = "rgb";
            var col3 = new Color();
            col3.r = 200;
            col3.b = 0;
            col3.g = 0;
            led3.color = col3;

            while (true)
            {
                if (numberOfAlotMovements == 4)
                {
                    Console.WriteLine("In 2nd while loop");
                    while (true)
                    {
                        client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led3)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        col3.g = 200;
                        Thread.Sleep(2);

                        client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led3)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        col3.g = 0;

                        if (pillsTaken)
                        {
                            break;
                        }
                    }

                    var led2 = new LED();
                    led2.brightness = 254;
                    led2.state = "ON";
                    led2.color_mode = "rgb";
                    var col2 = new Color();
                    col2.r = 200;
                    col2.b = 200;
                    col2.g = 200;
                    led2.color = col2;
                    pillsTaken = true;
                    client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led2)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    while (true)
                    {
                        if (stopWatch.Elapsed.Seconds >= 20)
                        {
                            SystemSounds.Exclamation.Play();
                            //client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led2)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        }
                        
                        if (pillsPutBack)
                        {
                            break;
                        }

                    }

                    stopWatch.Stop();
                    Console.WriteLine("Out of loop");
                    Console.ReadLine();
                }

            }

        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            switch (e.Topic)
            {
                case "zigbee2mqtt/0x680ae2fffec0cbba":
                    var res = Encoding.UTF8.GetString(e.Message);
                    Console.WriteLine($"LED-STRIP MESSAGE: {res}");
                    break;
                case "MotionSensor":
                    var res2 = Encoding.UTF8.GetString(e.Message);
                    Console.WriteLine($"MOTIONSENSOR MESSAGE: {res2}");
                    DoMotionSensorStuff(res2);
                    break;
                case "WeightSensor":
                    var res3 = Encoding.UTF8.GetString(e.Message);
                    Console.WriteLine($"WEIGHTSENSOR MESSAGE: {res3}");
                    DoWeightSensorStuff(res3);
                    break;
                default:
                    Console.WriteLine("Reached default in switch statement");
                    break;
            }
        }

        static void DoMotionSensorStuff(string message)
        {
           
            if (!pillsTaken)
            {
                switch (message)
                {
                    case "Alot":
                        if (previousEvent == "Alot")
                        {
                            numberOfAlotMovements++;
                            Console.WriteLine(numberOfAlotMovements);
                        }
                        previousEvent = message;
                        break;
                    case "No":
                        previousEvent = message;
                        var led = new LED();
                        led.state = "OFF";
                        //client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        break;
                    case "Some":
                        previousEvent = message;
                        var led2 = new LED();
                        led2.brightness = 50;
                        led2.state = "ON";
                        led2.color_mode = "rgb";
                        var col2 = new Color();
                        col2.r = 0;
                        col2.b = 200;
                        col2.g = 200;
                        led2.color = col2;
                        //client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led2)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                        break;
                    default:
                        break;
                }
            }
        }

        static void DoWeightSensorStuff(string message)
        {
            
            if (message == "On")
            {
                var led = new LED();
                led.brightness = 254;
                led.state = "ON";
                led.color_mode = "rgb";
                var col = new Color();
                col.r = 200;
                col.b = 0;
                col.g = 200;
                led.color = col;
                pillsPutBack = true;
                client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }

            if (message == "Off")
            {
                var led2 = new LED();
                led2.brightness = 254;
                led2.state = "ON";
                led2.color_mode = "rgb";
                var col2 = new Color();
                col2.r = 200;
                col2.b = 200;
                col2.g = 200;
                led2.color = col2;
                pillsTaken = true;
                client.Publish("zigbee2mqtt/0x680ae2fffec0cbba/set", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(led2)), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            }

            

        }
    }
}
