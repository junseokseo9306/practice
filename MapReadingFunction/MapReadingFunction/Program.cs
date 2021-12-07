using System;
using System.IO;
using System.Text;


namespace MapReadingFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            string stage1 = "Stage1\r\n#####\r\n#OoP#\r\n#####";
            string stage2 = "Stage2\r\n  #######  \r\n###  O  ###\r\n#    o    #\r\n# Oo P oO #\r\n###  o  ###\r\n #   O  #  \r\n ########  ";

            byte[] buffer1 = Encoding.UTF8.GetBytes(stage1);
            byte[] buffer2 = Encoding.UTF8.GetBytes(stage2);

            using (StreamReader READER1 = new StreamReader(new MemoryStream(buffer1)))
            using (StreamReader READER2 = new StreamReader(new MemoryStream(buffer2)))
            //using (StreamWriter WRITER = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true })
            {
                PrintStage.SaveAndPrintStage(READER1);
                PrintStage.SaveAndPrintStage(READER2);
            }
        }
    }
}
