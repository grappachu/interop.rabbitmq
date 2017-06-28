using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Grappachu.Core.Messaging;
using Grappachu.Core.Preview.UI;
using Grappachu.Interop.RabbitMQ.Samples.Queuing;

namespace Grappachu.Interop.RabbitMQ.Samples
{
    public partial class MainWindow
    {
        private readonly IConsumer<Message> _consumer;
        private readonly IPublisher<Message> _publisher;


        public MainWindow()
        {
            _publisher = new TestPublisher();
            _consumer = new TestConsumer();
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _consumer.MessageReceived += _consumer_MessageReceived;
        }

        private void _consumer_MessageReceived(object sender, Envelope<Message> e)
        {
            Dispatcher.BeginInvoke(new Action(() => { LstReceived.Items.Add(e.Message); }));
            e.GiveAck = true;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            _consumer.EndConsume();
        }

        private void BtnSend_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var msg = new Message {Content = TxtMessage.Text};
                _publisher.Publish(msg);
            }
            catch (Exception ex)
            {
                Dialogs.ShowError(ex);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }


        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _consumer.BeginConsume();
                BtnToggle.Content = "End Receive";
            }
            catch (Exception ex)
            {
                Dialogs.ShowError(ex);
            }
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _consumer.EndConsume();
                BtnToggle.Content = "Start Receive";
            }
            catch (Exception ex)
            {
                Dialogs.ShowError(ex);
            }
        }
    }
}