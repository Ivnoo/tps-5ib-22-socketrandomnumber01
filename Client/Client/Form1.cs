using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool next = false;
        public static string data;
        byte[] bytes = new byte[1024];

        private void Form1_Load(object sender, EventArgs e)
        {
            txtConnection.Text = "Nessuna connessione";
            txtConnection.ForeColor = System.Drawing.Color.Red;
        }

        private void btnConnessione_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text))
                errorProvider.SetError(txtUsername, "Devi inserire uno username!");
            else
                next = true;
            if (string.IsNullOrEmpty(txtPass.Text))
                errorProvider.SetError(txtPass, "Devi inserire una password!");
            else
                next = true;

            if (next)
            {
                string user = Convert.ToString(txtUsername.Text);
                string pass = Convert.ToString(txtPass.Text);
                string message = user + ";" + pass + ";<EOF>";
                string showToUser = "Ciao server, sono " + user + ", mi generi un numero?";

                byte[] bytes = new byte[1024];

                try
                {
                    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                    IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);

                    Socket send = new Socket(ipAddress.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                    try
                    {
                        send.Connect(remoteEP);
                        txtConnection.Text = "Socket aperta";
                        txtConnection.ForeColor = System.Drawing.Color.Green;

                        Console.WriteLine("Socket connected to {0}",
                            send.RemoteEndPoint.ToString());

                        listBox.Items.Add(showToUser);
                        byte[] msg = Encoding.ASCII.GetBytes(message);

                        int bytesSent = send.Send(msg);

                        int bytesRec = send.Receive(bytes);
                        Console.WriteLine("Echoed test = {0}",
                            Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        listBox.Items.Add(Encoding.ASCII.GetString(bytes, 0, bytesRec));
                        send.Shutdown(SocketShutdown.Both);
                        send.Close();
                    }
                    catch (ArgumentNullException ane)
                    {
                        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine("SocketException : {0}", se.ToString());
                    }
                    catch (Exception a)
                    {
                        Console.WriteLine("Unexpected exception : {0}", a.ToString());
                    }
                }
                catch (Exception a)
                {
                    Console.WriteLine(a.ToString());
                    txtConnection.Text = "Errore durante la connessione";
                    txtConnection.ForeColor = System.Drawing.Color.Red;
                }
            }
            
        }
    }
}
