using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace PDFtoImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Beginning PDF conversion...");

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
                
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create("https://s3.amazonaws.com/DaypackUserContent/modelImages/2018-03-20T09:45:21-04:00-additional%20items.pdf");
                HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();

                MemoryStream memStream;
                using (var stream = httpWebReponse.GetResponseStream())
                {
                    memStream = new MemoryStream();
                    
                    byte[] buffer = new byte[1024];
                    int byteCount;
                    do
                    {
                        byteCount = stream.Read(buffer, 0, buffer.Length);
                        memStream.Write(buffer, 0, byteCount);
                    } while (byteCount > 0);
                }
                memStream.Seek(0, SeekOrigin.Begin);

                Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                //doc.LoadFromFile(@"C:\Users\BuzzBuzzUser\Pictures\cedar_II.pdf");
                doc.LoadFromStream(memStream);                

                for (int i = 0; i < doc.Pages.Count; i++)
                {
                    var pdfImg = doc.SaveAsImage(i);

                    pdfImg.Save(string.Format(@"C:\Users\BuzzBuzzUser\Pictures\Saved Pictures\cedar_II{0}.png", i));
                }

                watch.Stop();
                Console.WriteLine("PDF conversion took {0} milliseconds",watch.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void PDFSharpAPI()
        {
            const string filename = @"C:\Users\BuzzBuzzUser\Pictures\vista-ridge-parkside-floorplan-10-10-2017.pdf";
            PdfDocument document = PdfReader.Open(filename);

            int imageCount = 0;
            // Iterate pages
            foreach (PdfPage page in document.Pages)
            {
                // Get resources dictionary
                PdfDictionary resources = page.Elements.GetDictionary("/Resources");
                if (resources != null)
                {
                    // Get external objects dictionary
                    PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
                    if (xObjects != null)
                    {
                        var items = xObjects.Elements.Values;
                        // Iterate references to external objects
                        foreach (PdfItem item in items)
                        {
                            PdfReference reference = item as PdfReference;
                            if (reference != null)
                            {
                                PdfDictionary xObject = reference.Value as PdfDictionary;
                                // Is external object an image?
                                if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                                {
                                    ExportImage(xObject, ref imageCount);
                                }
                            }
                        }
                    }
                }
            }
        }
        static void ExportImage(PdfDictionary image, ref int count)
        {
            string filter = image.Elements.GetName("/Filter");
            switch (filter)
            {
                case "":
                    ExportJpegImage(image, ref count);
                    break;

                case "/DCTDecode":
                    ExportJpegImage(image, ref count);
                    break;

                case "/FlateDecode":
                    ExportAsPngImage(image, ref count);
                    break;
            }
        }

        static void ExportJpegImage(PdfDictionary image, ref int count)
        {
            // Fortunately JPEG has native support in PDF and exporting an image is just writing the stream to a file.
            byte[] stream = image.Stream.Value;
            FileStream fs = new FileStream(String.Format(@"C:\Users\BuzzBuzzUser\Pictures\Saved Pictures\Image{0}.jpeg", count++), FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(stream);
            bw.Close();
        }

        static void ExportAsPngImage(PdfDictionary image, ref int count)
        {
            int width = image.Elements.GetInteger(PdfImage.Keys.Width);
            int height = image.Elements.GetInteger(PdfImage.Keys.Height);
            int bitsPerComponent = image.Elements.GetInteger(PdfImage.Keys.BitsPerComponent);

            // TODO: You can put the code here that converts vom PDF internal image format to a Windows bitmap
            // and use GDI+ to save it in PNG format.
            // It is the work of a day or two for the most important formats. Take a look at the file
            // PdfSharp.Pdf.Advanced/PdfImage.cs to see how we create the PDF image formats.
            // We don't need that feature at the moment and therefore will not implement it.
            // If you write the code for exporting images I would be pleased to publish it in a future release
            // of PDFsharp.
        }
    }
}
