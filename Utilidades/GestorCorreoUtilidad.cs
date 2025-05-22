using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace InverPaper.Utilidades
{
    public class GestorCorreoUtilidad
    {
        private SmtpClient cliente;
        private MailMessage email;
        private string Host = "smtp.gmail.com";
        private int Port = 587;
        private string User = "inverpaperinfo@gmail.com";
        private string Password = "dccljgigbxzpvhir";
        private bool EnabledSSL = true;

        public GestorCorreoUtilidad ()
        {
            cliente = new SmtpClient(Host, Port)
            {
                EnableSsl = EnabledSSL,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(User, Password)
            };
        }

        public void EnviarCorreo(string destinatario, string asunto, string mensaje, bool esHtml = false, string rutaImagen = null)
        {
            email = new MailMessage(User, destinatario, asunto, mensaje);
            email.IsBodyHtml = esHtml;

            if (esHtml && !string.IsNullOrEmpty(rutaImagen))
            {
                // Crear una vista alterna para HTML
                AlternateView vistaHtml = AlternateView.CreateAlternateViewFromString(mensaje, null, System.Net.Mime.MediaTypeNames.Text.Html);

                // Adjuntar imagen como recurso embebido
                LinkedResource imagen = new LinkedResource(rutaImagen, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                imagen.ContentId = "logoImg";
                imagen.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

                vistaHtml.LinkedResources.Add(imagen);
                email.AlternateViews.Add(vistaHtml);
            }

            cliente.Send(email);
        }
    }
}