using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.Domain
{
    public class AnswerModel
    {
        public string Answer { get; set; }
        public List<Citation> Citations { get; set; }
        public string Question { get; set; }
    }
    public class Citation
    {
        public string CitationData { get; set; }
        public int CitationPageNumber { get; set; }
        public string CitationSource { get; set; }
    }
}
