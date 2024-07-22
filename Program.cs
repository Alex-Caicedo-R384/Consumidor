using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Mail;
using System.Text;

public class MessageConsumer
{
    public void ReceiveMessage()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "Cola 1",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Recibido {0}", message);

                var emailSender = new EmailSender();
                emailSender.SendEmail("Nuevo Mensaje", message);
            };

            channel.BasicConsume(queue: "Cola 1",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Presiona la tecla enter para salir");
            Console.ReadLine();
        }
    }
}

public class EmailSender
{
    public void SendEmail(string subject, string body)
    {
        MailMessage mail = new MailMessage("chevyagcr@gmail.com", "chevyagcr@gmail.com");
        SmtpClient client = new SmtpClient();

        client.Port = 587;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Host = "smtp.gmail.com";
        client.EnableSsl = true;
        mail.Subject = subject;
        mail.Body = body;

        client.Credentials = new System.Net.NetworkCredential("chevyagcr@gmail.com", "hiie zbbc vrvt mnhh");

        try
        {
            client.Send(mail);
            Console.WriteLine("Correo enviado correctamente a chevyagcr@gmail.com");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error en enviar el mensaje " + ex.Message);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var consumer = new MessageConsumer();
        consumer.ReceiveMessage();
    }
}
