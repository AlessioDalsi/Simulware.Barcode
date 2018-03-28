using System;
using System.Configuration;
using System.Drawing;
using DataMatrix.net;

namespace Simulware.Barcode
{
    public class DataMatrix
    {
        public Bitmap GenMatrix(string value)
        {
            
            DmtxImageEncoderOptions options = new DmtxImageEncoderOptions
            {
                ModuleSize = Convert.ToInt32(ConfigurationManager.AppSettings["ModuleSize"]),
                MarginSize = Convert.ToInt32(ConfigurationManager.AppSettings["MarginSize"]),

            };
            switch (ConfigurationManager.AppSettings["DMdim"])
            {
                case "small":
                    options.SizeIdx = DmtxSymbolSize.DmtxSymbol64x64;
                    break;
                case "medium":
                    options.SizeIdx = DmtxSymbolSize.DmtxSymbol104x104;
                    break;
                case "large":
                    options.SizeIdx = DmtxSymbolSize.DmtxSymbol144x144;
                    break;
            }
            return new DmtxImageEncoder().EncodeImage(value, options);
            //new DmtxImageEncoder().EncodeImage(Value, options).Save("DataMatrix.png");
        }
    }
}
