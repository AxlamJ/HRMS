//using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
namespace HrManagement.Helpers
{
    public class Email
    {

        private readonly IConfiguration _configuraiton;
        private readonly string SERVER = "";
        private readonly string USER_NAME = "";
        private readonly string Pwd = "";
        private readonly int PORT = -1;
        private readonly bool SMTP_ENABLED = false;
        private string gstrFailedRecipients;
        private string gstrExceptionMessage;
        private bool gblnIsMailSentToNone;
        private readonly IWebHostEnvironment _env;


        public string FailedRecipients
        {
            get { return gstrFailedRecipients; }
        }
        public string ExceptionMessage
        {
            get { return gstrExceptionMessage; }
        }
        public bool IsMailSentToNone
        {
            get { return gblnIsMailSentToNone; }
        }
        public Email(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuraiton = configuration;
            GetSmtpSettings(out SERVER, out USER_NAME, out Pwd, out PORT, out SMTP_ENABLED);
            _env = env;
        }

        private void GetSmtpSettings(out string strSmtpServer, out string strSmtpUserName, out string strSmtpPassword,
    out int intSmtpPort, out bool blnSmtpEnabled)
        {
            try
            {
                strSmtpServer = _configuraiton["EmailConfigurations:SMTPServer"];
                strSmtpUserName = _configuraiton["EmailConfigurations:SMTPUserName"];
                strSmtpPassword = _configuraiton["EmailConfigurations:SMTPPassword"];
                intSmtpPort = Convert.ToInt32(_configuraiton["EmailConfigurations:SMTPPort"]);
                blnSmtpEnabled = Convert.ToBoolean(_configuraiton["EmailConfigurations:SMTPEnabled"]);
            }
            catch (Exception exc)
            {
                throw new System.Exception("Could not get Smtp Settings" + exc.Message);
            }
        }

        public async Task<string> SendEmailAsync(string? from,string? toname, string? to, string? cc, string? bcc, string? subject, string? body, string? AttachmentsPaths,bool IsBodyHtml , char delimitter)
        {
            to = to.Replace(",", ";");
            //cc = cc.Replace(",", ";");
            //bcc = bcc.Replace(",", ";");
            gblnIsMailSentToNone = false;
            string[] strTo = to.Split(delimitter);
            string[] strCC = null;
            string[] strBCC = null;
            MimeMessage message = new MimeMessage();

            try
            {
                body = body.Replace("<br />", "<br/>");
                body = body.Replace("<br/>", "<br>");
                //if (!cc.Equals(String.Empty))
                //    strCC = cc.Split(delimitter);

                //if (!bcc.Equals(String.Empty))
                //    strBCC = bcc.Split(delimitter);

                message.From.Add( new MailboxAddress("info@hrms.chestermerephysio.ca", from));

                foreach (string strToAddress in strTo)
                {
                    if (!string.IsNullOrWhiteSpace(strToAddress))
                        message.To.Add(new MailboxAddress(toname,strToAddress.Trim()));
                }

                var imagePath = Path.Combine(_env.WebRootPath, "images", "Revital-Health-1.png");

                // Create image attachment with Content-ID
                var image = new MimePart("image", "png")
                {
                    Content = new MimeContent(File.OpenRead(imagePath)),
                    ContentId = "signatureImage",
                    ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                    ContentTransferEncoding = ContentEncoding.Base64
                };
                //if (!cc.Equals(String.Empty))
                //{
                //    foreach (string strCCAddress in strCC)
                //    {
                //        if (!string.IsNullOrWhiteSpace(strCCAddress))
                //            message.CC.Add(new MailAddress(strCCAddress.Trim()));
                //    }
                //}

                //if (!bcc.Equals(String.Empty))
                //{
                //    foreach (string strBCCAddress in strBCC)
                //    {
                //        if (!string.IsNullOrWhiteSpace(strBCCAddress))
                //            message.Bcc.Add(new MailAddress(strBCCAddress.Trim()));
                //    }
                //}
                // Create HTML part

                message.Subject = subject;
                var html = new TextPart("html") { Text = body };
                // Create multipart/related container
                var multipart = new Multipart("related");
                multipart.Add(html);
                multipart.Add(image);

                message.Body = multipart;
                message.Priority = MessagePriority.Urgent;
                //message.IsBodyHtml = IsBodyHtml;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            try
            {
                // Updation: hst004
                if (SMTP_ENABLED)
                {
                    try
                    {
                        //SmtpClient client = new SmtpClient();
                        using var client = new SmtpClient();
                        bool EnableSsl = false;

                        try
                        {
                            EnableSsl = Convert.ToBoolean(_configuraiton["EmailConfigurations:EnableSsl"]);
                        }
                        catch (Exception)
                        {

                        }
                        //message.Sender = new MailAddress(USER_NAME);

                        await client.ConnectAsync(SERVER, PORT,EnableSsl);
                        await client.AuthenticateAsync(USER_NAME, Pwd);
                        var a = await client.SendAsync(message);
                        await client.DisconnectAsync(true);

                        //client.Credentials = new System.Net.NetworkCredential(USER_NAME, Pwd);
                        //client.Send(message);

                        return "";
                    }
                    //catch (SmtpFailedRecipientsException excFailRecipients)
                    //{

                    //    gstrExceptionMessage = excFailRecipients.Message;
                    //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == excFailRecipients.InnerExceptions.Length;

                    //    foreach (SmtpFailedRecipientException smptpFailedRecipient in excFailRecipients.InnerExceptions)
                    //    {
                    //        gstrFailedRecipients += smptpFailedRecipients.FailedRecipient + delimitter;
                    //    }

                    //    return "[INVALID]:" + gstrFailedRecipients + ":" + gblnIsMailSentToNone + ":" + gstrExceptionMessage;

                    //}
                    //catch (SmtpFailedRecipientException excFailRecipient)
                    //{
                    //    gstrExceptionMessage = excFailRecipient.Message;
                    //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == 1;

                    //    gstrFailedRecipients += excFailRecipient.FailedRecipient;

                    //    return "[INVALID]:" + gstrFailedRecipients + ":" + gblnIsMailSentToNone + ":" + gstrExceptionMessage;
                    //}
                    catch (Exception e1)
                    {
                        return e1.Message;
                    }
                }
                return "SMTP Disabled.";
            }
            // Updation: hst002
            //catch (SmtpFailedRecipientsException excFailRecipients)
            //{
            //    gstrExceptionMessage = excFailRecipients.Message;
            //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == excFailRecipients.InnerExceptions.Length;

            //    foreach (SmtpFailedRecipientException smptpFailedRecipient in excFailRecipients.InnerExceptions)
            //    {
            //        gstrFailedRecipients += smptpFailedRecipient.FailedRecipient + delimitter;
            //    }

            //    gstrFailedRecipients = gstrFailedRecipients.TrimEnd(delimitter);
            //    return gstrExceptionMessage + gstrFailedRecipients;
            //}
            //catch (SmtpFailedRecipientException excFailRecipient)
            //{
            //    gstrExceptionMessage = excFailRecipient.Message;
            //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == 1;

            //    gstrFailedRecipients += excFailRecipient.FailedRecipient;

            //    return gstrExceptionMessage + gstrFailedRecipients;
            //}
            catch (Exception mex)
            {
                gstrExceptionMessage = mex.Message;
                gstrFailedRecipients = String.Empty;
                gblnIsMailSentToNone = true;
                return gstrExceptionMessage;
            }
            // End Updation: hst002
        }
        public async Task<string> SendEmailMultipleAsync(string? from,Dictionary<string,string> to, string? cc, string? bcc, string? subject, string? body, string? AttachmentsPaths,bool IsBodyHtml , char delimitter)
        {
            //to = to.Replace(",", ";");
            //cc = cc.Replace(",", ";");
            //bcc = bcc.Replace(",", ";");
            gblnIsMailSentToNone = false;
            //string[] strTo = to.Split(delimitter);
            string[] strCC = null;
            string[] strBCC = null;
            MimeMessage message = new MimeMessage();

            try
            {
                body = body.Replace("<br />", "<br/>");
                body = body.Replace("<br/>", "<br>");
                //if (!cc.Equals(String.Empty))
                //    strCC = cc.Split(delimitter);

                //if (!bcc.Equals(String.Empty))
                //    strBCC = bcc.Split(delimitter);

                message.From.Add( new MailboxAddress("info@hrms.chestermerephysio.ca", from));

                foreach (var item in to)
                {
                    if (!string.IsNullOrWhiteSpace(item.Key))
                        message.To.Add(new MailboxAddress(item.Key.ToString().Trim(),item.Value.ToString().Trim()));
                }

                var imagePath = Path.Combine(_env.WebRootPath, "images", "Revital-Health-1.png");

                // Create image attachment with Content-ID
                var image = new MimePart("image", "png")
                {
                    Content = new MimeContent(File.OpenRead(imagePath)),
                    ContentId = "signatureImage",
                    ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                    ContentTransferEncoding = ContentEncoding.Base64
                };
                //if (!cc.Equals(String.Empty))
                //{
                //    foreach (string strCCAddress in strCC)
                //    {
                //        if (!string.IsNullOrWhiteSpace(strCCAddress))
                //            message.CC.Add(new MailAddress(strCCAddress.Trim()));
                //    }
                //}

                //if (!bcc.Equals(String.Empty))
                //{
                //    foreach (string strBCCAddress in strBCC)
                //    {
                //        if (!string.IsNullOrWhiteSpace(strBCCAddress))
                //            message.Bcc.Add(new MailAddress(strBCCAddress.Trim()));
                //    }
                //}
                // Create HTML part

                message.Subject = subject;
                var html = new TextPart("html") { Text = body };
                // Create multipart/related container
                var multipart = new Multipart("related");
                multipart.Add(html);
                multipart.Add(image);

                message.Body = multipart;
                message.Priority = MessagePriority.Urgent;
                //message.IsBodyHtml = IsBodyHtml;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            try
            {
                // Updation: hst004
                if (SMTP_ENABLED)
                {
                    try
                    {
                        //SmtpClient client = new SmtpClient();
                        using var client = new SmtpClient();
                        bool EnableSsl = false;

                        try
                        {
                            EnableSsl = Convert.ToBoolean(_configuraiton["EmailConfigurations:EnableSsl"]);
                        }
                        catch (Exception)
                        {

                        }
                        //message.Sender = new MailAddress(USER_NAME);

                        await client.ConnectAsync(SERVER, PORT,EnableSsl);
                        await client.AuthenticateAsync(USER_NAME, Pwd);
                        var a = await client.SendAsync(message);
                        await client.DisconnectAsync(true);

                        //client.Credentials = new System.Net.NetworkCredential(USER_NAME, Pwd);
                        //client.Send(message);

                        return "";
                    }
                    //catch (SmtpFailedRecipientsException excFailRecipients)
                    //{

                    //    gstrExceptionMessage = excFailRecipients.Message;
                    //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == excFailRecipients.InnerExceptions.Length;

                    //    foreach (SmtpFailedRecipientException smptpFailedRecipient in excFailRecipients.InnerExceptions)
                    //    {
                    //        gstrFailedRecipients += smptpFailedRecipients.FailedRecipient + delimitter;
                    //    }

                    //    return "[INVALID]:" + gstrFailedRecipients + ":" + gblnIsMailSentToNone + ":" + gstrExceptionMessage;

                    //}
                    //catch (SmtpFailedRecipientException excFailRecipient)
                    //{
                    //    gstrExceptionMessage = excFailRecipient.Message;
                    //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == 1;

                    //    gstrFailedRecipients += excFailRecipient.FailedRecipient;

                    //    return "[INVALID]:" + gstrFailedRecipients + ":" + gblnIsMailSentToNone + ":" + gstrExceptionMessage;
                    //}
                    catch (Exception e1)
                    {
                        return e1.Message;
                    }
                }
                return "SMTP Disabled.";
            }
            // Updation: hst002
            //catch (SmtpFailedRecipientsException excFailRecipients)
            //{
            //    gstrExceptionMessage = excFailRecipients.Message;
            //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == excFailRecipients.InnerExceptions.Length;

            //    foreach (SmtpFailedRecipientException smptpFailedRecipient in excFailRecipients.InnerExceptions)
            //    {
            //        gstrFailedRecipients += smptpFailedRecipient.FailedRecipient + delimitter;
            //    }

            //    gstrFailedRecipients = gstrFailedRecipients.TrimEnd(delimitter);
            //    return gstrExceptionMessage + gstrFailedRecipients;
            //}
            //catch (SmtpFailedRecipientException excFailRecipient)
            //{
            //    gstrExceptionMessage = excFailRecipient.Message;
            //    gblnIsMailSentToNone = message.To.Count + message.CC.Count == 1;

            //    gstrFailedRecipients += excFailRecipient.FailedRecipient;

            //    return gstrExceptionMessage + gstrFailedRecipients;
            //}
            catch (Exception mex)
            {
                gstrExceptionMessage = mex.Message;
                gstrFailedRecipients = String.Empty;
                gblnIsMailSentToNone = true;
                return gstrExceptionMessage;
            }
            // End Updation: hst002
        }

    }
}
