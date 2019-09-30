using System.Threading.Tasks;
using WMP.Core.Data.SQLiteNet;
using USAID.Base;
using USAID.Interfaces;
using Xamarin.Forms;
using SQLite.Net;
using USAID.Services;
using USAID.Models;
using System.Collections.Generic;
using USAID.Pages;
using System.Linq;
using USAID.Repositories;
using USAID.Builders;

namespace USAID.ViewModels
{
	public class QuotesViewModel : BaseViewModel
	{
		private readonly IQuoteRepository _quoteRepo;
		private readonly IQuoteBuilder _quoteBuilder;
		public QuotesViewModel(IGALogger logger, IQuoteRepository quoteRepo, IQuoteBuilder quoteBuilder)
		{
			_quoteRepo = quoteRepo;
			_quoteBuilder = quoteBuilder;
		}

		public bool NoQuotesLabelVisible
		{
			get { return GetValue<bool>(); }
			set { SetValue(value); }
		}


		public List<Quote> Quotes
		{
			get { return GetValue<List<Quote>>(); }
			set { SetValue(value); }
		}

		private void LoadItems()
		{
			IsBusy = true;
			Quotes = _quoteRepo.All().ToList();
			NoQuotesLabelVisible = !Quotes.Any();
			IsBusy = false;
		}

		Command deleteCommand;

		public Command DeleteCommand
		{
			get { return deleteCommand ?? (deleteCommand = new Command(ExecuteDeleteCommand)); }
		}

		private void ExecuteDeleteCommand(object option)
		{
			//delete item
			Quote item = (Quote)option;
			_quoteRepo.Delete(item.Id);
			LoadItems();
		}

		Command loadCommand;

		public Command LoadCommand
		{
			get { return loadCommand ?? (loadCommand = new Command(ExecuteLoadCommand)); }
		}

		private void ExecuteLoadCommand(object option)
		{
			LoadItems();
		}

		Command quoteCommand;

		public Command QuoteCommand {
			get { return quoteCommand ?? (quoteCommand = new Command (ExecuteQuoteCommand)); }
		}

		private void ExecuteQuoteCommand (object option)
		{
			Quote quote = (Quote)option;
			_quoteBuilder.SetQuote (quote);
		}
	}
}

