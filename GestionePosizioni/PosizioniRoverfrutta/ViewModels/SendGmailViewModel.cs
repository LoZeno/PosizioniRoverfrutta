using System;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using PosizioniRoverfrutta.Annotations;

namespace PosizioniRoverfrutta.ViewModels
{
    public class SendGmailViewModel : INotifyPropertyChanged
    {
        public string ReceiverList
        {
            get { return _receiverList; }
            set
            {
                _receiverList = value;
                OnPropertyChanged();
            }
        }

        public string Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
                OnPropertyChanged();
            }
        }

        public string Body
        {
            get { return _body; }
            set
            {
                _body = value;
                OnPropertyChanged();
            }
        }

        public string AttachmentPath
        {
            get { return _attachmentPath; }
            set
            {
                _attachmentPath = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendEmail
        {
            get { return sendEmail ?? (sendEmail = new DelegateCommand(SendGmail())); }
        }

        private Action SendGmail()
        {
            return delegate
            {
                var senderName = ConfigurationManager.AppSettings["senderName"];
                var senderEmail = ConfigurationManager.AppSettings["senderEmail"];
                var gmailPassword = ConfigurationManager.AppSettings["senderPassword"];

                var fromAddress = new MailAddress(senderEmail, senderName);

                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromAddress.Address, gmailPassword),
                    Timeout = 20000
                };

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = fromAddress;
                    mailMessage.Sender = fromAddress;
                    var destinations = _receiverList.Split(',');
                    foreach (var address in destinations)
                    {
                        mailMessage.To.Add(address);
                    }
                    mailMessage.Subject = _subject;
                    mailMessage.Body = _body;

                    var attachment = new Attachment(_attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                    smtp.Send(mailMessage);
                }

                Status = "Invio effettuato con successo";
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _receiverList;
        private string _subject;
        private string _attachmentPath;
        private string _body;
        private ICommand sendEmail;
        private string _status;
    }
}