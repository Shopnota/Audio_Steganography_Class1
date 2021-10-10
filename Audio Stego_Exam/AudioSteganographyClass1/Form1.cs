using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioSteganographyClass1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string cover_audio_file = @"C:\Users\DCL\OneDrive\Desktop\Audio Stego_Exam\SampleData_AudioFiles_file_example_WAV_1MG.wav";
                string secret_message = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                Char[] secret_message_binary = messageBinaryFormat(secret_message);

                WaveAudio waveCoverAudio = new WaveAudio(new FileStream(cover_audio_file, FileMode.Open, FileAccess.Read));

                var leftstream = waveCoverAudio.GetLeftStream();
                var righttstream = waveCoverAudio.GetRightStream();

                int messageTrackFirstHalf = 0;
                for (int i = 0; i < secret_message_binary.Length; i++)
                {
                    int leftStreamValue = leftstream[i];
                    int evenodd = 1;
                    if (leftStreamValue < 0)
                    {
                        evenodd = -1;
                        leftStreamValue = leftStreamValue * evenodd; //if value is odd then we convert to even so that we can convert into binary easily
                    }
                    string binaryValueLeftStreamValue = Convert.ToString(leftStreamValue, 2).PadLeft(16, '0');
                    int newLeftStreamValue = Convert.ToInt32((binaryValueLeftStreamValue.Substring(0, binaryValueLeftStreamValue.Length - 1) + secret_message_binary[messageTrackFirstHalf]), 2) * evenodd;
                    leftstream[i] = (short)newLeftStreamValue;
                    messageTrackFirstHalf++;
                }

                waveCoverAudio.WriteFile(@"C:\Users\DCL\OneDrive\Desktop\Audio Stego_Exam\FinalAudio_For_exam.wav");
                MessageBox.Show("Success!!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail");
            }
        }

        public string messageBinary = "";
        public char[] messageBinaryFormat(string message)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in message.ToCharArray())
            {
                sb.Append(Convert.ToString(c, 2).PadLeft(8, '0'));
            }
            messageBinary = sb.ToString();

            //if (((messageBinary.Length) % 3) == 2)
            //{
            //    messageBinary += "0";
            //}
            //else if (((messageBinary.Length) % 3) == 1)
            //{
            //    messageBinary += "00";
            //}

            return messageBinary.ToCharArray();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string stego_audio = @"C:\Users\DCL\OneDrive\Desktop\FinalAudio.wav";
            WaveAudio waveStegoAudio = new WaveAudio(new FileStream(stego_audio, FileMode.Open, FileAccess.Read));
            var leftstream = waveStegoAudio.GetLeftStream();
            var righttstream = waveStegoAudio.GetRightStream();

            string binaryMessagelengthChar = "";
            for (int i = 0; i < 208; i++)
            {
                int leftStreamValue = leftstream[i];
                Console.WriteLine(leftStreamValue);
                int evenodd = 1;
                if (leftStreamValue < 0)
                {
                    evenodd = -1;
                    leftStreamValue = leftStreamValue * evenodd; //if value is odd then we convert to even so that we can convert into binary easily
                }
                string binaryValueLeftStreamValue = Convert.ToString(leftStreamValue, 2).PadLeft(16, '0');
                //binaryMessagelengthChars.Add(binaryValueLeftStreamValue[15]);
                binaryMessagelengthChar += binaryValueLeftStreamValue[15].ToString();
            }

            string totalSecretMessageBitCoreString = binaryMessagelengthChar;
            int initialFrame = 0;
            int perFrame = 8;
            string secret_message_final = "";
            for (int i = 0; i < 26; i++)
            {
                initialFrame = perFrame * i;
                string temp = totalSecretMessageBitCoreString.Substring(initialFrame, 8);

                secret_message_final += (char)Convert.ToInt32(temp, 2);
            }

            MessageBox.Show(secret_message_final.ToString());


        }
    }
}
