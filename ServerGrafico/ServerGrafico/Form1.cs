﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ServerGrafico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Random r = new Random();
        public static string data = null;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = @"accessList.csv";
            Dictionary<string, string> db = new Dictionary<string, string>();

            byte[] bytes = new Byte[1024];

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string[] doc = line.Split(';');
                db.Add(doc[0], doc[1]);
            }

            string user = "ivan";
            string pass = "1234";

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    listBox.Items.Add("Attendo connessione...");
                    listBox.Refresh();
                    Socket handler = listener.Accept();
                    data = null;

                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    string[] access = data.Split(';');

                    if (access[0]==user && access[1] == pass)
                    {
                        listBox.Items.Add("Accetto connessione per " + access[0] + ", utente presente nel database");
                        listBox.Refresh();
                        Console.WriteLine("Text received : {0}", data);
                        data = "Genero un numero randomico: " + r.Next();
                        listBox.Items.Add(data);
                        listBox.Refresh();
                    }
                    else
                    {
                        listBox.Items.Add("Rifiuto connessione per " + access[0] + ", utente non presente nel database");
                        listBox.Refresh();
                        data = "Mi dispiace, non sei presente nel database e quindi non posso fornirti un numero!";
                    }


                    byte[] msg = Encoding.ASCII.GetBytes(data);
                    handler.Send(msg);
                    Console.WriteLine("Messaggio inviato");
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception a)
            {
                Console.WriteLine(a.ToString());
            }
        }
    }
}