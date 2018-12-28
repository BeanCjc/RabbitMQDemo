using System;
using System.Windows.Forms;
using RabbitMQ.Client;
using System.Threading;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;

namespace MimicWPOS
{
    public partial class Form1 : Form
    {
        static ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost", UserName = "xuanyakeji", Password = "123456" };
        static IConnection connection = factory.CreateConnection();
        static IModel channel = connection.CreateModel();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Consume();
        }

        public void Consume()
        {
            //var channel = connection.CreateModel();
            channel.ExchangeDeclare("ExchangeMiniWPOS", ExchangeType.Fanout, true, false, null);
            channel.QueueDeclare("QueueMiniWPOS", true, false, false, null);
            channel.QueueBind("QueueMiniWPOS", "ExchangeMiniWPOS", "", null);

            //返回的消息
            channel.ExchangeDeclare("ExchangeMiniWPOSReturn", ExchangeType.Fanout, true, false, null);
            channel.QueueDeclare("QueueMiniWPOSReturn", true, false, false, null);
            channel.QueueBind("QueueMiniWPOSReturn", "ExchangeMiniWPOSReturn", "", null);

            var propertites = channel.CreateBasicProperties();
            propertites.Persistent = true;
            propertites.DeliveryMode = 2;

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("QueueMiniWPOS", false, consumer);
            consumer.Received += (obj, deliver) =>
            {
                //消费信息
                var message = Encoding.UTF8.GetString(deliver.Body);
                //Thread.Sleep(200);
                var returnMessage = "\"" + message + "\" has consumed.";
                var returnBody = Encoding.UTF8.GetBytes(returnMessage);
                //channel.BasicPublish("ExchangeMiniWPOSReturn", "", propertites, returnBody);
                channel.BasicAck(deliver.DeliveryTag, false);

                // 消费完毕并显示UI
                richTextBox1.Invoke(new Action(() => richTextBox1.Text += message));
            };
        }

        //发送消息给web端
        private void Send_Message_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_Message.Text))
            {
                MessageBox.Show("Can not send empty message!");
            }
            else
            {
                //var channel = connection.CreateModel();
                channel.ExchangeDeclare("MessageExchange", ExchangeType.Fanout, true, false, null);
                channel.QueueDeclare("MessageQueue", true, false, false, null);
                channel.QueueBind("MessageQueue", "MessageExchange", "", null);
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2;
                var message = $"WinFormClient   {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\r\n {txt_Message.Text.Trim()}\r\n\r\n";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("MessageExchange", "", properties, body);
                richTextBox1.Text += message;
                txt_Message.Clear();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (connection != null)
            {
                connection.Dispose();
            }
            if (channel != null)
            {
                channel.Dispose();
            }
        }
    }
}
