using System.IO;
using System.Text;

namespace PDCoreNew.Writers
{
    public class Utf8StringWriter : StringWriter
    {
        public Utf8StringWriter(StringBuilder sb) : base(sb)
        {

        }

        public Utf8StringWriter() : base()
        {

        }

        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
