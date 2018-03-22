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
                ModuleSize = 1,
                MarginSize = 4,
                SizeIdx = DmtxSymbolSize.DmtxSymbol52x52
            };
            return new DmtxImageEncoder().EncodeImage(value, options);
            //new DmtxImageEncoder().EncodeImage(Value, options).Save("DataMatrix.png");
        }
    }
}
