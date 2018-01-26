using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace StockExchange
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : TabbedPage
    {
        //create variables for the interface
        private Label DailyHighLabel = new Label();
        private Label DailyLowLabel = new Label();
        private Label StockNameLabel = new Label();
        private Label StockValueLabel = new Label();
        private Label StockPercentageLabel = new Label();
        private Entry HomeStockEntry = new Entry();
        private Entry NewStockEntry = new Entry();
        private Button StockSearchButton = new Button();
        private Button NewStockSearchButton = new Button();
        private ContentPage StockContentPage = new ContentPage();
        private string AlphaVantageKey = "B48ONNG0EIYLMY9G";

        public WelcomePage()
        {
            InitializeComponent();

            //bind variables to elements in the UI from .xaml
            DailyHighLabel = DailyHigh;
            StockSearchButton = StockSearch;
            DailyLowLabel = DailyLow;
            StockNameLabel = StockName;
            StockValueLabel = StockValue;
            StockPercentageLabel = Percentage;
            HomeStockEntry = HomeStock;
            NewStockSearchButton = NewStockSearch;
            NewStockEntry = NewStock;
            StockContentPage = StockCoursePage;
        }

        //Call Alpha Vantage API and receive data, switch page after process is complete
        private async void StockSearchButton_Clicked()
        {
            //check if the user entered something into the entry
            if (!String.IsNullOrEmpty(HomeStockEntry.Text))
            {
                string StockHandle = GetStockHandle(HomeStockEntry.Text);
                if (!StockHandle.Contains("Invalid"))
                {
                    AVApiWrapper.ApiData data = new AVApiWrapper.ApiData(await AVApiWrapper.getApiData(StockHandle, AlphaVantageKey));
                    setData(data, HomeStockEntry.Text.ToString());
                    this.CurrentPage = StockContentPage;
                }
                //user entered a not supported company
                else
                {
                    await DisplayAlert("Warning!", "The Company you are searching for is not supported in the current version." +
                        " Please choose a different one.", "Ok");
                }
            }
            //user did not enter anything, display a warning
            else
            {
                await DisplayAlert("Warning!", "Please enter a company into the field.", "Ok");
            }
        }

        //Call Alpha Vantage API and receive data
        private async void NewStockSearch_Clicked()
        {
            //check if the entry field is empty
            if (!String.IsNullOrEmpty(HomeStockEntry.Text))
            {
                string StockHandle = GetStockHandle(NewStockEntry.Text);
                if (!StockHandle.Contains("Invalid"))
                {
                    AVApiWrapper.ApiData data = new AVApiWrapper.ApiData(await AVApiWrapper.getApiData(StockHandle, AlphaVantageKey));
                    setData(data, NewStockEntry.Text.ToString());
                }
                //user entered a not supported company
                else
                {
                    await DisplayAlert("Warning!", "The Company you are searching for is not supported in the current version." +
                        " Please choose a different one.", "Ok");
                }
            }
            //user did not enter anything, display a warning
            else
            {
                await DisplayAlert("Warning!", "Please enter a company into the field.", "Ok");
            }
        }

        //fill page with numbers from the API
        private void setData(AVApiWrapper.ApiData data, string entryText)
        {
            DailyHighLabel.Text = data.dailyHigh.ToString();
            DailyLowLabel.Text = data.dailyLow.ToString();
            StockNameLabel.Text = entryText;
            StockValueLabel.Text = data.closingCourse.ToString();
            StockPercentageLabel.Text = data.percentageDifference.ToString();
        }

        //execute button clicked function in case return was pressed instead of the button
        private void HomeStock_Completed(object sender, EventArgs e)
        {
            StockSearchButton_Clicked();
        }

        //see upper comment
        private void NewStock_Completed(object sender, EventArgs e)
        {
            NewStockSearch_Clicked();
        }

        //converts title of Companies to handles used by Stock Exchanges
        //for this project a few(not all) companies are included
        public static string GetStockHandle(string UserEntry)
        {
            string Handle = null;
            switch (UserEntry)
            {
                case "Microsoft":
                    Handle = "MSFT";
                    break;
                case "Apple":
                    Handle = "AAPL";
                    break;
                case "Google":
                    Handle = "GOOG";
                    break;
                case "Amazon":
                    Handle = "AMZN";
                    break;
                case "Facebook":
                    Handle = "FB";
                    break;
                case "Verizon":
                    Handle = "VZ";
                    break;
                case "Disney":
                    Handle = "DIS";
                    break;
                    //user entry false, throw error
                default:
                    Handle = "Invalid";
                    break;
            }

            return Handle;
        }
    }
}