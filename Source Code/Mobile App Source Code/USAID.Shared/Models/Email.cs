using System;
using System.Collections.Generic;

namespace USAID.Models
{
	public class Email
	{
		public List<string> ToAddresses {get;set;}
        public List<string> CcAddresses { get; set; }
		public string Subject {get;set;}
		public string Body {get;set;}
		public bool IsHtml {get;set;}
        public string AttachmentFileLocation { get; set; }
	}
}

