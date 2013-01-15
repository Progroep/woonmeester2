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


public partial class _Default_Master : System.Web.UI.MasterPage
{
    public int intID = 0;
    public string strListingID, strSQL, strTxtSearch, strPhoto, strRss, strLinkedin, strTwitter, strYoutube;
    public string strSearch = "Zoeken...";

    protected void Page_Load(object sender, EventArgs e)
    {
        intID = Convert.ToInt32(Request.QueryString["id"]);
        strTxtSearch = Request.QueryString["txtSearch"];
        if (!String.IsNullOrEmpty(strTxtSearch))
        {
            if (strTxtSearch == strSearch)
                txtSearch.Text = "";
            Response.Redirect("/resultaten.aspx?q=" + strTxtSearch);
        }

        string ConnectionString = ConfigurationManager.AppSettings["ConnectionStringSQL"];
        //string strSiteShortURL = ConfigurationManager.AppSettings["SiteShortURL"];
        //if (!String.IsNullOrEmpty(strSiteShortURL))
            //litSiteName.Text = " - " + strSiteShortURL;

        using (SqlConnection myConnection = new SqlConnection(ConnectionString))
        {
            butSearch.ImageUrl = "/sites/" + GeneralFunctions.getSiteDirectory() + "/images/searchbutton.jpg";

            #region Site specific data
            strSQL = @"SELECT *
                        FROM tblSites 
                        WHERE siteID = @sID ";
            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.AddWithValue("sID", GeneralFunctions.getSiteID());
            myConnection.Open();
            SqlDataReader dr = myCommand.ExecuteReader();
            while (dr.Read())
            {
                litSiteName.Text = dr["siteTitle"].ToString();
                litAnalytics.Text = dr["siteGoogleAnalytics"].ToString();
            }
            myConnection.Close();

            #endregion


            #region Connection from top menu
            strSQL = @"SELECT pageID, pageMenuTitle, pageParent, pageURL 
                      FROM tblPages 
                      WHERE pageParent = 0 
                      AND pageShowInList = 1 
                      AND pageActive = 1 
                      AND (IsDeleted IS NULL OR IsDeleted = 0)
                      AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY pageOrder DESC";
            SqlCommand menuCommand = new SqlCommand(strSQL, myConnection);
            SqlDataAdapter menuDataAdapter = new SqlDataAdapter(menuCommand);
            DataSet menuDataSet = new DataSet();
            menuDataAdapter.Fill(menuDataSet, "MainMenu");

            rptMenu.DataSource = menuDataSet;
            rptMenu.DataBind();
            //rptMenuFooter.DataBind();
            #endregion

            #region Conection to listing image
            //Select the Featured photo of the listing
            if (intID > 0 && Request.QueryString["type"] == "woning")
            {
                strSQL = @"SELECT photoName
                           FROM tblPhotos
                           WHERE photoTypeID = 1
                           AND photoParent = @id
                           AND photoOrder IS NULL
                           AND siteID = " + GeneralFunctions.getSiteID();

                SqlCommand cmd = new SqlCommand(strSQL, myConnection);
                cmd.Parameters.Add("@id", intID);
                myConnection.Open();
                object objPhoto = cmd.ExecuteScalar();

                if (objPhoto != null)
                    strPhoto = "/photos/home/big_" + objPhoto.ToString();
                else
                {
                    strSQL = @"SELECT TOP 1 photoName, photoURL
                               FROM tblPhotos
                               WHERE photoTypeID = 1
                               AND photoParent = @id
                               AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY photoOrder";

                    SqlCommand photoCommand = new SqlCommand(strSQL, myConnection);
                    photoCommand.Parameters.Add("@id", intID);
                    SqlDataAdapter photoDataAdapter = new SqlDataAdapter(photoCommand);
                    DataSet photoDataSet = new DataSet();
                    photoDataAdapter.Fill(photoDataSet, "Photo");

                    if (photoDataSet.Tables["Photo"].Rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(photoDataSet.Tables["Photo"].Rows[0]["photoName"].ToString()))
                            strPhoto = "/photos/big/" + photoDataSet.Tables["Photo"].Rows[0]["photoName"].ToString();
                        else
                            strPhoto = photoDataSet.Tables["Photo"].Rows[0]["photoURL"].ToString();
                    }
                }
                myConnection.Close();
            }
            #endregion

            #region Connection to Home page featured image
            if (String.IsNullOrEmpty(strPhoto) && Request.Url.AbsolutePath.IndexOf("home.aspx") > -1)
            {
                //Select the photo of featured post
                strSQL = @"SELECT TOP 1 *
                        FROM tblPosts t1 
                        LEFT JOIN tblPhotos t2
                        ON (t1.postID = t2.photoParent AND t2.photoTypeID = 4)
                        WHERE postFeatured = 1
                        AND photoName IS NOT NULL
                        AND postActive = 1 
                        AND t1.siteID = " + GeneralFunctions.getSiteID();

                SqlCommand photoCommand = new SqlCommand(strSQL, myConnection);
                SqlDataAdapter photoDataAdapter = new SqlDataAdapter(photoCommand);
                DataSet photoDataSet = new DataSet();
                photoDataAdapter.Fill(photoDataSet, "Photo");

                if (photoDataSet.Tables["Photo"].Rows.Count > 0)
                {
                    if(!String.IsNullOrEmpty(photoDataSet.Tables["Photo"].Rows[0]["photoName"].ToString()))
                        strPhoto = "/photos/home/medium/" + photoDataSet.Tables["Photo"].Rows[0]["photoName"];
                    else
                        strPhoto = photoDataSet.Tables["Photo"].Rows[0]["photoURL"].ToString();
                }
                else
                {
                    //Select a photo of a post any
                    strSQL = @"SELECT TOP 1 *
                            FROM tblPosts t1 
                            LEFT JOIN tblPhotos t2
                            ON (t1.postID = t2.photoParent AND t2.photoTypeID = 4)
                            WHERE postActive = 1
                            AND photoName IS NOT NULL
                            AND t1.siteID = " + GeneralFunctions.getSiteID() + " ORDER BY photoID DESC";

                    photoCommand.CommandText = strSQL;
                    photoDataAdapter.SelectCommand = photoCommand;
                    photoDataAdapter.Fill(photoDataSet, "Photo");
                    
                    if (photoDataSet.Tables["Photo"].Rows.Count > 0)
                    {
                        if(!String.IsNullOrEmpty(photoDataSet.Tables["Photo"].Rows[0]["photoName"].ToString()))
                            strPhoto = "/photos/home/medium/" + photoDataSet.Tables["Photo"].Rows[0]["photoName"];
                        else
                            strPhoto = photoDataSet.Tables["Photo"].Rows[0]["photoURL"].ToString();
                    }
                }
            }
            #endregion

            if (intID <= 0 && Request.QueryString["type"] != "woning")
                ucSearchFeatured.Visible = true;


            if (!String.IsNullOrEmpty(strPhoto) && (strPhoto.Contains("http") || CommonFunctions.CheckFileExists(strPhoto)))
            {
                imgListingPhoto.ImageUrl = strPhoto;
                imgListingPhoto.Attributes.Add("alt", "");
            }
            else
            {
                imgListingPhoto.Visible = false;
                ucSearchFeatured.Visible = false;
            }
            
            #region Connection to small house images
            strSQL = @"SELECT TOP 6 t1.listingID, t1.listingPrice, t1.listingAddress,
                        t1.listingAddressNumber, t2.photoName, t2.photoURL, t3.cityName
                        FROM tblListings t1
                        LEFT JOIN tblPhotos t2
                        ON (t1.ListingID = t2.photoParent AND t2.photoTypeID = 1)
                        AND t2.PhotoOrder = (SELECT MIN(PhotoOrder) 
                                            FROM tblPhotos 
                                            WHERE photoParent = t1.ListingID
                                            AND photoTypeID = 1)
                        LEFT JOIN tblCities t3
                        ON (t1.listingCityID = t3.cityID)
                        WHERE (t2.photoName IS NOT NULL
                        OR t2.photoURL IS NOT NULL)
                        AND listingActive = 1
                        AND (IsDeleted IS NULL OR IsDeleted = 0)
                        AND listingSellType = 1 AND t1.siteID = " + GeneralFunctions.getSiteID() + " ORDER BY listingPublishDate DESC";

            SqlCommand houseCommand = new SqlCommand(strSQL, myConnection);
            SqlDataAdapter houseDataAdapter = new SqlDataAdapter(houseCommand);
            DataSet houseDataSet = new DataSet();
            houseDataAdapter.Fill(houseDataSet, "Houses");

            rptHouses.DataSource = houseDataSet;
            rptHouses.DataBind();
            #endregion
        }
    }

    protected void Search_Click(object sender, EventArgs e)
    {
        if (txtSearch.Text == strSearch)
            txtSearch.Text = "";
        Response.Redirect("/resultaten.aspx?q=" + txtSearch.Text);
    }
}