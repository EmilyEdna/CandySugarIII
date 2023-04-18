using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CandySugar.Com.Library.Internet;

namespace CandySugar.Com.Library.Mqtts
{
    public class MqttExten
    {
        private static MqttExten _Instance;
        public static MqttExten Instance
        {
            get
            {
                if (_Instance != null) return _Instance;
                else
                {
                    _Instance = new MqttExten();
                    return _Instance;
                }
            }
        }

        private IMqttClient client;
        private MqttClientOptions options;
        private List<string> Topic;
        /// <summary>
        /// MQTT 接收消息
        /// </summary>
        public Action<MqttApplicationMessageReceivedEventArgs> MqttReceive { get; set; }

        /// <summary>
        /// MQTT 连接成功
        /// </summary>
        public Action<MqttClientConnectedEventArgs> ConnectOk { get; set; }

        /// <summary>
        /// 重连成功
        /// </summary>
        public Action Reconnection { get; set; }

        public Action UseDisconnectedAction { get; set; }
        /// <summary>
        /// 接收的消息
        /// </summary>
        public Action<string> ReceiveMsg { get; set; }

        public MqttExten MqttInitAsync(string UserName, string pass, string IP, int port, string clientID)
        {
            options = new MqttClientOptionsBuilder().WithTcpServer(IP, port).WithClientId(clientID).WithCredentials(UserName, pass).Build();
            client = new MqttFactory().CreateMqttClient();
            client.DisconnectedAsync += (e) =>
            {
                if (UseDisconnectedAction == null)
                {
                    Reconnect();
                    UseDisconnectedAction?.Invoke();
                }
                return Task.CompletedTask;
            };
            client.ApplicationMessageReceivedAsync += (e) =>
            {
                var data = Encoding.Default.GetString(e.ApplicationMessage.Payload);
                ReceiveMsg?.Invoke(data);
                return Task.CompletedTask;
            };
            client.ConnectedAsync += (e) =>
            {
                ConnectOk?.Invoke(e);
                return Task.CompletedTask;
            };
            try
            {
                 client.ConnectAsync(options).Wait();
            }
            catch (Exception)
            {
            }
            return this;
        }

        public async void Close()
        {
            await client.DisconnectAsync();
        }

        /// <summary>
        /// 重连
        /// </summary>
        public void Reconnect()
        {
            UseDisconnectedAction = new Action(() =>
            {
                Thread.Sleep(2000);
                while (!InternetWork.GetNetworkState)
                {
                    Thread.Sleep(2000);
                }

                bool ErrorFlag = false;
                while (!client.IsConnected)
                {
                    try
                    {

                        client.ConnectAsync(options).Wait();
                        MqttSubscriptionAsync(Topic);
                     
                    }
                    catch (Exception)
                    {
                        if (!ErrorFlag)
                            ErrorFlag = true;
                    }
                    Thread.Sleep(3000);
                }

                if (client.IsConnected)
                {
                    if (Reconnection != null) Reconnection();
                }
                UseDisconnectedAction = null;
            });
        }

        /// <summary>
        /// Mqtt 订阅
        /// </summary>
        /// <param name="topic">需要订阅的主题</param>
        public void MqttSubscriptionAsync(string topic)
        {
            MqttSubscriptionAsync(new List<string> { topic });
        }

        /// <summary>
        /// Mqtt 订阅
        /// </summary>
        /// <param name="topic">需要订阅的主题</param>
        public async void MqttSubscriptionAsync(List<string> topic)
        {
            Topic = topic;
            if (client != null)
            {
                if (client.IsConnected)
                {
                    foreach (var item in topic)
                    {
                        await client.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(item).WithExactlyOnceQoS().Build());
                    }
                }
            }
        }

        /// <summary>
        /// Mqtt 发布
        /// </summary>
        /// <param name="topic">需要发布的主题</param>
        /// <param name="content">需要发布的内容</param>
        public async void MqttPublishAsync(string topic, string content)
        {
            if (client != null)
            {
                if (client.IsConnected)
                {
                    var msg = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(content).WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce).Build();
                    await client.PublishAsync(msg);
                }
            }
        }

        public bool IsConneted()
        {
            return client.IsConnected;
        }
    }
}
