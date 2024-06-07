using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Mail;
using System.Text;

// Clase que consume mensajes de una cola de RabbitMQ
public class MessageConsumer
{
    // Método para recibir mensajes de la cola
    public void ReceiveMessage()
    {
        // Crear una conexión a RabbitMQ usando la configuración de fábrica con el host "localhost"
        var factory = new ConnectionFactory() { HostName = "localhost" };

        // Establecer la conexión y el canal de comunicación con RabbitMQ
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            // Declarar una cola llamada "Cola 1" con las siguientes propiedades:
            // - durable: false (la cola no persiste después de reiniciar RabbitMQ)
            // - exclusive: false (la cola no está restringida a esta conexión)
            // - autoDelete: false (la cola no se elimina automáticamente cuando no esté en uso)
            // - arguments: null (sin argumentos adicionales)
            channel.QueueDeclare(queue: "Cola 1",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // Crear un consumidor de eventos básicos para la cola
            var consumer = new EventingBasicConsumer(channel);

            // Definir el comportamiento al recibir un mensaje
            consumer.Received += (model, ea) =>
            {
                // Obtener el cuerpo del mensaje recibido como un arreglo de bytes
                var body = ea.Body.ToArray();
                // Convertir el cuerpo del mensaje a una cadena de texto usando codificación UTF-8
                var message = Encoding.UTF8.GetString(body);
                // Imprimir el mensaje recibido en la consola
                Console.WriteLine(" [x] Recibido '{0}'", message);

                // Crear una instancia de EmailSender y enviar el mensaje por correo electrónico
                var emailSender = new EmailSender();
                emailSender.SendEmail("Nuevo Mensaje", message);
            };

            // Configurar el canal para consumir mensajes de "Cola 1" automáticamente
            // - autoAck: true (el mensaje se reconoce automáticamente una vez recibido)
            // - consumer: el consumidor configurado anteriormente
            channel.BasicConsume(queue: "Cola 1",
                                 autoAck: true,
                                 consumer: consumer);

            // Mantener la aplicación en ejecución hasta que el usuario presione la tecla enter
            Console.WriteLine("Presiona la tecla enter para salir");
            Console.ReadLine();
        }
    }
}

// Clase que envía correos electrónicos
public class EmailSender
{
    // Método para enviar el correo electrónico
    public void SendEmail(string subject, string body)
    {
        // Crear un nuevo mensaje de correo
        MailMessage mail = new MailMessage("chevyagcr@gmail.com", "chevyagcr@gmail.com");
        // Crear un cliente SMTP para enviar el correo
        SmtpClient client = new SmtpClient();

        // Configurar el puerto para el cliente SMTP
        client.Port = 587;
        // Configurar el método de entrega del correo
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        // No usar las credenciales predeterminadas del sistema
        client.UseDefaultCredentials = false;
        // Configurar el host del servidor SMTP
        client.Host = "smtp.gmail.com";
        // Habilitar SSL para la conexión segura
        client.EnableSsl = true;
        // Establecer el asunto del correo
        mail.Subject = subject;
        // Establecer el cuerpo del correo
        mail.Body = body;

        // Establecer las credenciales para autenticar el envío del correo
        client.Credentials = new System.Net.NetworkCredential("chevyagcr@gmail.com", "hiie zbbc vrvt mnhh");

        try
        {
            // Intentar enviar el correo
            client.Send(mail);
            // Imprimir mensaje de éxito en la consola
            Console.WriteLine("Correo enviado correctamente a chevyagcr@gmail.com");
        }
        catch (Exception ex)
        {
            // Imprimir mensaje de error en caso de fallo
            Console.WriteLine("Error en enviar el mensaje " + ex.Message);
        }
    }
}

// Clase principal del programa
class Program
{
    static void Main(string[] args)
    {
        // Crear una instancia del consumidor de mensajes y recibir mensajes
        var consumer = new MessageConsumer();
        consumer.ReceiveMessage();
    }
}
