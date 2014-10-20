using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PosizioniRoverfrutta.ViewModels;
using QueryManager;

namespace PosizioniRoverfrutta.Windows
{
    /// <summary>
    /// Interaction logic for SendGmailAttachment.xaml
    /// </summary>
    public partial class SendGmailAttachment : BaseWindow
    {

        public SendGmailAttachment()
            : this(null, null)
        {
            
        }

        public SendGmailAttachment(IWindowManager windowManager, IDataStorage dataStorage)
            :base(windowManager, dataStorage)
        {
            InitializeComponent();
        }

        public SendGmailAttachment(IWindowManager windowManager, IDataStorage dataStorage, string attachmentPath)
            :this(windowManager, dataStorage)
        {
            var viewModel = new SendGmailViewModel();
            viewModel.AttachmentPath = attachmentPath;
            DataContext = viewModel;

            SetBindingsForTextBox("ReceiverList", ReceiversBox);
            SetBindingsForTextBox("Subject", SubjectBox);
            SetBindingsForTextBox("Body", BodyTextBox);
            SetBindingsForTextBlock("Status", StatusBlock);

            SetSendButtonBindings(viewModel);
        }

        private static void SetBindingsForTextBox(string property, TextBox control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(TextBox.TextProperty, binding);
        }        
        
        private static void SetBindingsForTextBlock(string property, TextBlock control)
        {
            var binding = new Binding(property)
            {
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            control.SetBinding(TextBlock.TextProperty, binding);
        }

        private void SetSendButtonBindings(SendGmailViewModel viewModel)
        {
            var sendBinding = new CommandBinding
            {
                Command = viewModel.SendEmail
            };
            CommandBindings.Add(sendBinding);

            SendButton.SetBinding(ButtonBase.CommandProperty, new Binding
            {
                Source = viewModel,
                Path = new PropertyPath("SendEmail")
            });
        }
    }
}
