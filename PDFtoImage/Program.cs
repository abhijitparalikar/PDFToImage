using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace PDFtoImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning PDF conversion...");

            SpireMultiThreadAPI();
            SpireAPI();

            Console.WriteLine("PDF conversion complete...");
            Console.ReadLine();
        }

        public static void SpireAPI()
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string url = @"C:\Users\BuzzBuzzUser\Pictures\PDFSImages.pdf";
                //HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //MemoryStream memStream;
                //using (var stream = httpWebReponse.GetResponseStream())
                //{
                //    memStream = new MemoryStream();

                //    byte[] buffer = new byte[1024];
                //    int byteCount;
                //    do
                //    {
                //        byteCount = stream.Read(buffer, 0, buffer.Length);
                //        memStream.Write(buffer, 0, byteCount);
                //    } while (byteCount > 0);
                //}
                //memStream.Seek(0, SeekOrigin.Begin);

                watch.Stop();
                Console.WriteLine("It took {0} milliseconds to download the file", watch.ElapsedMilliseconds);
                watch.Restart();

                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                doc.LoadFromFile(url);
                //doc.LoadFromStream(memStream);
                //string fileName = url.Substring(url.LastIndexOf("/"));

                for (int i = 0; i < doc.Pages.Count; i++)
                {
                    //Console.WriteLine(doc.Pages[i].IsBlank());
                    var pdfImg = doc.SaveAsImage(i,Spire.Pdf.Graphics.PdfImageType.Bitmap,300,300);
                    
                    pdfImg.Save(string.Format(@"C:\Users\BuzzBuzzUser\Pictures\Saved Pictures\{0}.bmp", i));
                    //pdfImg.Save(string.Format(@"C:\Users\BuzzBuzzUser\Pictures\Saved Pictures\{0}.png", i));
                }

                watch.Stop();
                Console.WriteLine("PDF conversion took {0} milliseconds",watch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void SpireMultiThreadAPI()
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string url = @"C:\Users\BuzzBuzzUser\Pictures\PDFSImages.pdf";

                //HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                //HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //string fileName = url.Substring(url.LastIndexOf("/"));

                //MemoryStream memStream;
                //using (var stream = httpWebReponse.GetResponseStream())
                //{
                //    memStream = new MemoryStream();

                //    byte[] buffer = new byte[1024];
                //    int byteCount;
                //    do
                //    {
                //        byteCount = stream.Read(buffer, 0, buffer.Length);
                //        memStream.Write(buffer, 0, byteCount);
                //    } while (byteCount > 0);
                //}
                //memStream.Seek(0, SeekOrigin.Begin);

                watch.Stop();
                Console.WriteLine("It took {0} milliseconds to download the file", watch.ElapsedMilliseconds);
                watch.Restart();

                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();

                doc.LoadFromFile(url);
                //doc.LoadFromStream(memStream);
                Object thisLock = new Object();
                Dictionary<int, Spire.Pdf.PdfDocument> dictPDFDoc = new Dictionary<int, Spire.Pdf.PdfDocument>();
                
                for (int i = 0; i < doc.Pages.Count; i++)
                {
                    Spire.Pdf.PdfDocument pdfdoc = new Spire.Pdf.PdfDocument();
                    //pdfdoc.LoadFromStream(memStream);
                    pdfdoc.LoadFromFile(url);
                    dictPDFDoc.Add(i, pdfdoc);

                }

                foreach (var key in dictPDFDoc.Keys)
                {
                    Thread t = new Thread(() => SaveImage(dictPDFDoc[key], "", key));
                    t.Start();
                }

                watch.Stop();
                Console.WriteLine("PDF conversion took {0} milliseconds", watch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void SaveImage(Spire.Pdf.PdfDocument doc, string fileName, int index)
        {
            
            var pdfImg = doc.SaveAsImage(index, Spire.Pdf.Graphics.PdfImageType.Bitmap, 300, 300);
            
            pdfImg.Save(string.Format(@"C:\Users\BuzzBuzzUser\Pictures\Saved Pictures\ThreadedImg{0}.bmp", index));
        }
    }
}
