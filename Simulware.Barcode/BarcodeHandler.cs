using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web;
using Simulware.Barcode;

class BarcodeHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        
        var urlContent = context.Request.QueryString["val1"];
        var dmImage = finalDM(urlContent);
        context.Response.ContentType = "image/png";
        dmImage.Save(context.Response.OutputStream, ImageFormat.Png);
    }

    public bool IsReusable { get; }

    public Bitmap finalDM(string data)
    {
        Signature s = new Signature();
        string hexString = "Test";
        try
        {
            var sign = s.CreateSignature(data);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in sign)
            {
                sb.Append(b.ToString("X2"));
            }

            hexString = sb.ToString();
            Console.WriteLine("Signature len:" + sign.Length);
            Console.WriteLine("Signature dump " + hexString);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.ToString());
        }
        Simulware.Barcode.DataMatrix dm = new Simulware.Barcode.DataMatrix();
        return dm.genMatrix(hexString);
    }
}
