using System;
using System.Collections.Generic;
using USAID.Models;

namespace USAID.Interfaces
{
	public interface IEmailService
	{
		void CreateEmail (Email email);
		bool CanSendEmail();
	}
}