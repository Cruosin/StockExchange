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
            AVApiWrapper.ApiData data = new AVApiWrapper.ApiData(await AVApiWrapper.getApiData(HomeStockEntry.Text.ToString(), AlphaVantageKey));
            setData(data, HomeStockEntry.Text.ToString());
            this.CurrentPage = StockContentPage;
        }

        //Call Alpha Vantage API and receive data
        private async void NewStockSearch_Clicked()
        {
            AVApiWrapper.ApiData data = new AVApiWrapper.ApiData(await AVApiWrapper.getApiData(NewStockEntry.Text.ToString(), AlphaVantageKey));
            setData(data, NewStockEntry.Text.ToString());
        }

        //
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
    }
}