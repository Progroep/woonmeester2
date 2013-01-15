using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class _ListingLocation : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        MasterPageFile = "~/sites/" + GeneralFunctions.getSiteDirectory() + "/Default.master";
    }

    public string strSQL, strAddress, strCity;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        //Even de connectiestring uit config.xml halen
        string ConnectionString = ConfigurationSettings.AppSettings["ConnectionStringSQL"];
        int intID = Convert.ToInt32(Request.QueryString["id"]);

        //Data ophalen en in een adapter plaatsen
        using (SqlConnection myConnection = new SqlConnection(ConnectionString))
        {
            strSQL = @"SELECT * 
                       FROM tblListings t1
                       LEFT JOIN tblCities t2
                       ON (t1.listingCityID = t2.cityID)
                       WHERE ListingID = @id
                       AND (IsDeleted IS NULL OR IsDeleted = 0)";
            
            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.AddWithValue("@id", intID);
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet, "Listing");

            Literal litBrowserTitle = (Literal)Master.FindControl("litBrowserTitle");
            litBrowserTitle.Text = CommonFunctions.GetAddress(intID, false);
            litPageTitle.Text = litBrowserTitle.Text;

            strAddress = myDataSet.Tables["Listing"].Rows[0]["listingAddress"].ToString();
            strAddress += " " + myDataSet.Tables["Listing"].Rows[0]["listingAddressNumber"].ToString();
            strCity = myDataSet.Tables["Listing"].Rows[0]["cityName"].ToString();
        }
    }
}
