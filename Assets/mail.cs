using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using UnityEngine;

public class mail : MonoBehaviour
{
    private MailMessage mailmsg = new MailMessage();

    void SendEmails()
    {
        mailmsg.From = new MailAddress("GIVE_YOUR_EMAIL_HERE");
        mailmsg.To.Add("GIVE_YOUR_DESTINATION_HERE");

        SmtpClient smtpServer = new SmtpClient("GIVE_SMTP_INFO_HERE");
        smtpServer.Port = 587;//GIVE CORRECT PORT HERE
        mailmsg.Subject = "WHATEVER_YOU_WANT_TEXT";
        mailmsg.Body = "WHATEVER_YOU_WANT_TEXT";
        smtpServer.Credentials = new System.Net.NetworkCredential("GIVE_SMTP_INFO_HERE", "GIVE_YOUR_EMAIL_PASSWORD_HERE") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };
        smtpServer.Send(mailmsg);
        //smtpServer.SendAsync(mail)
        Debug.Log("success");
    }

}