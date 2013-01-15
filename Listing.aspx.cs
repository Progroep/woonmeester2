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
using System.IO;

public partial class _Listing : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        MasterPageFile = "~/sites/" + GeneralFunctions.getSiteDirectory() + "/Default.master";
    }

    bool bolPhotoExist = false, bolSocialIcons = false;
    public string strSQL, strAddress, strCity, strAgentID, strAgentName, strAgentPhoto, strHousePhoto, strBanner,
                  strVideo, strPlan, strShedule, strAgentContact, strSiteURL = ConfigurationManager.AppSettings["SiteURL"];
    
    protected void Page_Load(object sender, EventArgs e)
    {
        //Even de connectiestring uit config.xml halen
        string ConnectionString = ConfigurationManager.AppSettings["ConnectionStringSQL"];
        int intID = Convert.ToInt32(Request.QueryString["id"]);

        Panel panFeatured = (Panel)Master.FindControl("panFeatured");
        panFeatured.Visible = true;

        UserControl ucSearchFeatured = (UserControl)Master.FindControl("ucSearchFeatured");
        //HtmlControl ucSearchFeatured = (HtmlControl)Master.FindControl("ucSearchFeatured");
        ucSearchFeatured.Visible = false;

        hlkTour.ImageUrl = "/sites/" + GeneralFunctions.getSiteDirectory() + "/images/but_bezichtiging.png";

        //Data ophalen en in een adapter plaatsen
        using (SqlConnection myConnection = new SqlConnection(ConnectionString))
        {
            string strSQL = "";

            strSQL = @"SELECT t1.*, t2.PhotoName, t3.CityName, t3.bannerID, t4.*,
                       t5.agentName, t5.agentID, t5.agentPhone, t5.agentEmail, t5.agentBanner
                       FROM tblListings t1
                       LEFT JOIN tblPhotos t2
                       ON (t1.ListingID = t2.photoParent AND t2.photoTypeID = 1) 
                       AND t2.PhotoOrder = (SELECT MIN(PhotoOrder) 
                                            FROM tblPhotos 
                                            WHERE photoParent = t1.ListingID
					                        AND photoTypeID = 1)
                       LEFT JOIN tblCities t3
                       ON t1.ListingCityID = t3.CityID
                       LEFT JOIN tblListingPriceType t4
                       ON t1.ListingPriceType = t4.ListingPriceTypeID
                       LEFT JOIN tblAgents t5
                       ON (t1.agentID = t5.agentID 
                           AND t5.agentActive = 1)
                       WHERE t1.ListingActive = 1 
                       AND t1.ListingID = @id
                       AND (t1.IsDeleted IS NULL OR t1.IsDeleted = 0)";

            SqlCommand myCommand = new SqlCommand(strSQL, myConnection);
            myCommand.Parameters.AddWithValue("@id", intID);
            SqlDataAdapter myDataAdapter = new SqlDataAdapter(myCommand);
            DataSet myDataSet = new DataSet();
            myDataAdapter.Fill(myDataSet, "Listing");

            Literal litBrowserTitle = (Literal)Master.FindControl("litBrowserTitle");
            litBrowserTitle.Text = CommonFunctions.GetAddress(intID,false);

            if (myDataSet.Tables["Listing"].Rows.Count > 0)
            {
                #region Description - Keywords Meta TAG
                Literal Description = (Literal)Master.FindControl("Description");
                Literal Keywords = (Literal)Master.FindControl("Keywords");
                if (!String.IsNullOrEmpty(myDataSet.Tables["Listing"].Rows[0]["ListingDescription"].ToString()))
                {
                    Description.Text = "<meta name=\"description\" content=\"" + CommonFunctions.createDescription(myDataSet.Tables["Listing"].Rows[0]["ListingDescription"].ToString()) + "\" />";
                    Keywords.Text = "<meta name=\"keywords\" content=\"" + CommonFunctions.createKeywords(myDataSet.Tables["Listing"].Rows[0]["ListingDescription"].ToString()) + "\" />";
                }
                else
                {
                    Description.Text = "<meta name=\"description\" content=\"" + ConfigurationManager.AppSettings["defDescription"] + "\" />";
                    Keywords.Text = "<meta name=\"keywords\" content=\"" + ConfigurationManager.AppSettings["defKeywords"] + "\" />";
                }
                #endregion

                litHouseTitle.Text = litPageTitle.Text = litBrowserTitle.Text;

                ListingDescription.Text = myDataSet.Tables["Listing"].Rows[0]["ListingDescription"].ToString();
                if (!String.IsNullOrEmpty(myDataSet.Tables["Listing"].Rows[0]["ListingSkarabeeID"].ToString()))
                    ListingDescription.Text = ListingDescription.Text.Replace("\n", "<br >");

                strAddress = myDataSet.Tables["Listing"].Rows[0]["listingAddress"].ToString();
                strAddress += " " + myDataSet.Tables["Listing"].Rows[0]["listingAddressNumber"].ToString();
                strCity = myDataSet.Tables["Listing"].Rows[0]["cityName"].ToString();
                hlkShedule.NavigateUrl = hlkTour.NavigateUrl = "/woning/" + intID + "/bezichtiging/" + CommonFunctions.PrepTitle(litPageTitle.Text) + ".aspx";
                strAgentName = myDataSet.Tables["Listing"].Rows[0]["agentName"].ToString();
                strAgentID = myDataSet.Tables["Listing"].Rows[0]["agentID"].ToString();
                strVideo = myDataSet.Tables["Listing"].Rows[0]["listingVideo"].ToString();
                strBanner = myDataSet.Tables["Listing"].Rows[0]["agentBanner"].ToString();

                if (!String.IsNullOrEmpty(strAgentName))
                {
                    litAgentName.Text = strAgentName;
                    litAgentEmail.Text = "E: <a href=\"/makelaars/" + strAgentID + "/contact/" + CommonFunctions.PrepTitle(myDataSet.Tables["Listing"].Rows[0]["agentName"].ToString()) + ".aspx\">" +
                                         CommonFunctions.sWord(myDataSet.Tables["Listing"].Rows[0]["agentEmail"].ToString(), 20) + "</a><br/>";
                    litAgentPhone.Text = "T: " + CommonFunctions.sWord(myDataSet.Tables["Listing"].Rows[0]["agentPhone"].ToString(), 20);
                }
                else
                    panAgent.Visible = false;

                //litBanner.Text = CommonFunctions.GetBanner(myDataSet.Tables["Listing"].Rows[0]["bannerID"].ToString());
                if(strBanner.Length > 20)
                    litBanner.Text = strBanner;
            }
            else
                Response.Redirect("/error.aspx");

            if (String.IsNullOrEmpty(strVideo))
                lnkVideo.Visible = false;

            #region Plans
            strSQL = @"SELECT *
                       FROM tblPlans
                       WHERE listingID = @id
                       ORDER BY planFileName";

            SqlCommand plansCommand = new SqlCommand(strSQL, myConnection);
            plansCommand.Parameters.AddWithValue("@id", intID);
            SqlDataAdapter plansDataAdapter = new SqlDataAdapter(plansCommand);

            DataSet plansDataSet = new DataSet();
            plansDataAdapter.Fill(plansDataSet, "Plans");

            if (plansDataSet.Tables["Plans"].Rows.Count > 0)
            {
                rptPlans.DataSource = plansDataSet;
                rptPlans.DataBind();
                lnkPlatteground.Visible = true;
            }
            #endregion

            //string[] files = Directory.GetFiles(Request.PhysicalApplicationPath + "/plans/");
            //foreach (string filename in files)
            //{
            //    FileInfo file = new FileInfo(filename);
            //    string strFile = file.Name.Split('_')[0];
            //    if (strFile == intID.ToString())
            //    {
            //        lnkPlatteground.Visible = true;
            //        if (file.Extension == ".pdf")
            //        {
            //            lnkPlatteground.Attributes.Add("href", "/plans/" + file.Name);
            //            lnkPlatteground.Attributes.Add("target", "_blank");
            //        }
            //        else
            //            strPlan = "/plans/" + file.Name;
            //    }
            //}

            string[]  files = Directory.GetFiles(Request.PhysicalApplicationPath + "/brochures/");
            foreach (string filename in files)
            {
                FileInfo file = new FileInfo(filename);
                string strFile = file.Name.Split('_')[0];
                if (strFile == intID.ToString())
                {
                    if (file.Extension == ".pdf")
                    {
                        lnkBrochure.Visible = true;
                        lnkBrochure.Attributes.Add("href", "/brochures/" + file.Name);
                        lnkBrochure.Attributes.Add("target", "_blank");
                    }
                }
            }            
            
            #region Select the photo of agent
            if (!String.IsNullOrEmpty(strAgentID))
            {
                strSQL = @"SELECT *
                           FROM tblAgents
                           LEFT JOIN tblPhotos
                           ON (photoParent = agentID AND photoTypeID = 3)
                           WHERE agentID = @agentID
                           AND agentActive = 1";

                SqlCommand agentCommand = new SqlCommand(strSQL, myConnection);
                agentCommand.Parameters.Add("@agentID", strAgentID);
                SqlDataAdapter agentDataAdapter = new SqlDataAdapter(agentCommand);
                DataSet agentDataSet = new DataSet();
                agentDataAdapter.Fill(agentDataSet, "Agent");

                if (agentDataSet.Tables["Agent"].Rows.Count > 0)
                {
                    imgAgent.ImageUrl = CommonFunctions.GetAgentImage(agentDataSet.Tables["Agent"].Rows[0]["photoName"].ToString());

                    #region Social Icons
                    if (!String.IsNullOrEmpty(agentDataSet.Tables["Agent"].Rows[0]["agentFacebook"].ToString()))
                    {
                        hlkFacebook.NavigateUrl = agentDataSet.Tables["Agent"].Rows[0]["agentFacebook"].ToString();
                        hlkFacebook.ImageUrl = "/sites/" + GeneralFunctions.getSiteDirectory() + "/images/ico_facebook.jpg";
                        bolSocialIcons = true;
                    }
                    else
                        imgFacebook.Visible = false;

                    if (!String.IsNullOrEmpty(agentDataSet.Tables["Agent"].Rows[0]["agentHyves"].ToString()))
                    {
                        hlkHyves.NavigateUrl = agentDataSet.Tables["Agent"].Rows[0]["agentHyves"].ToString();
                        hlkHyves.ImageUrl = "/sites/" + GeneralFunctions.getSiteDirectory() + "/images/ico_hyves.jpg";
                        bolSocialIcons = true;
                    }
                    else
                        imgHyves.Visible = false;

                    if (!String.IsNullOrEmpty(agentDataSet.Tables["Agent"].Rows[0]["agentLinkedin"].ToString()))
                    {
                        hlkLinkedin.NavigateUrl = agentDataSet.Tables["Agent"].Rows[0]["agentLinkedin"].ToString();
                        hlkLinkedin.ImageUrl = "/sites/" + GeneralFunctions.getSiteDirectory() + "/images/ico_linkedin.jpg";
                        bolSocialIcons = true;
                    }
                    else
                        imgLinkedin.Visible = false;

                    if (!String.IsNullOrEmpty(agentDataSet.Tables["Agent"].Rows[0]["agentTwitter"].ToString()))
                    {
                        hlkTwitter.NavigateUrl = agentDataSet.Tables["Agent"].Rows[0]["agentTwitter"].ToString();
                        hlkTwitter.ImageUrl = "/sites/" + GeneralFunctions.getSiteDirectory() + "/images/ico_twitter.jpg";
                        bolSocialIcons = true;
                    }
                    else
                        imgTwitter.Visible = false;

                    //if (!String.IsNullOrEmpty(agentDataSet.Tables["Agent"].Rows[0]["agentEmail"].ToString()))
                    //    hlkEmail.NavigateUrl = "/makelaars/" + strAgentID + "/contact/" + CommonFunctions.PrepTitle(agentDataSet.Tables["Agent"].Rows[0]["agentName"].ToString()) + ".aspx";
                    //else
                    //    imgEmail.Visible = false;

                    if (!bolSocialIcons)
                        divSocial.Visible = false;

                    #endregion

                    strAgentContact = "<b>" + agentDataSet.Tables["Agent"].Rows[0]["agentName"].ToString() + "</b><br/>"
                                    + "T: " + agentDataSet.Tables["Agent"].Rows[0]["agentPhone"].ToString();
                }
                else
                    divSocial.Visible = imgAgent.Visible = false;

            }
            #endregion

            #region Select the photos of the house
            strSQL = @"SELECT CASE WHEN photoName IS NOT NULL 
                       THEN '/photos/small/' + photoName 
                       ELSE REPLACE(photoURL ,'&','&amp;')
                       END AS photo,
                       CASE WHEN photoName IS NOT NULL 
                       THEN photoName 
                       ELSE REPLACE(photoURL ,'&','&amp;')
                       END AS photoTitle
                       FROM tblPhotos
                       WHERE photoTypeID = 1
                       AND (photoHomePage = 0
                       OR photoHomePage IS NULL)
                       AND photoParent = @id
                       AND photoOrder IS NOT NULL
                       ORDER BY photoOrder";

            SqlCommand photosCommand = new SqlCommand(strSQL, myConnection);
            photosCommand.Parameters.AddWithValue("@id", intID);
            SqlDataAdapter photosDataAdapter = new SqlDataAdapter(photosCommand);
            DataSet photosDataSet = new DataSet();
            photosDataAdapter.Fill(photosDataSet, "Photos");

            if (photosDataSet.Tables["Photos"].Rows.Count > 0)
            {
                rptPhotos.DataSource = photosDataSet;
                rptPhotos.DataBind();
            }
            else
                panPhotos.Visible = false;
            #endregion

            #region Select first photo of the house
//            strSQL = @"SELECT TOP 1 photoName , photoURL
//                       FROM tblPhotos
//                       WHERE photoTypeID = 1
//                       AND (photoHomePage = 0
//                       OR photoHomePage IS NULL)
//                       AND photoOrder IS NULL
//                       AND photoParent = @id";

//            SqlCommand photoCommand = new SqlCommand(strSQL, myConnection);
//            photoCommand.Parameters.Add("@id", intID);
//            SqlDataAdapter photoDataAdapter = new SqlDataAdapter(photoCommand);
//            DataSet photoDataSet = new DataSet();
//            photoDataAdapter.Fill(photoDataSet, "Photo");

//            if (photoDataSet.Tables[0].Rows.Count > 0)
//            {
//                if (!String.IsNullOrEmpty(photoDataSet.Tables[0].Rows[0]["photoName"].ToString()))
//                    strHousePhoto = "/photos/home/big_" + photoDataSet.Tables[0].Rows[0]["photoName"].ToString();
//                else
//                    strHousePhoto = photoDataSet.Tables[0].Rows[0]["photoURL"].ToString();
//            }
            #endregion

            #region Select the offices for shedule
            if (!String.IsNullOrEmpty(strAgentID))
            {
                strSQL = @"SELECT  TOP 1 t2.*, t3.cityName
                       FROM tblRelAgentOffice t1
                       LEFT JOIN tblOffices t2
                       ON (t1.officeID = t2.officeID
                           AND t2.officeActive = 1)
                       LEFT JOIN tblCities t3
                       ON t2.officeCityID = t3.cityID
                       WHERE agentID = @agentID";

                SqlCommand relationCommand = new SqlCommand(strSQL, myConnection);
                relationCommand.Parameters.AddWithValue("@agentID", strAgentID);
                SqlDataAdapter relationDataAdapter = new SqlDataAdapter(relationCommand);
                DataSet relationDataSet = new DataSet();
                relationDataAdapter.Fill(relationDataSet, "Relation");

                foreach (DataRow dr in relationDataSet.Tables[0].Rows)
                {
                    strShedule += "<strong>" + dr["officeName"].ToString() + "</strong>"
                                + "<br />" + dr["officeAddress"].ToString() + " "
                                + dr["officeAddressNumber"].ToString() + "<br />"
                                + dr["officePostalCode"].ToString() + " " + dr["cityName"].ToString() + "<br />";
                }
            }
            #endregion

            litOffices.Text = strShedule;
        }
    }

    #region UpdatePanel actions
    protected void lnkDetails_Click(object sender, EventArgs e)
    {
        //divVideo.Visible = divPlatteground.Visible = divBrochure.Visible = false;
        //divDetails.Visible = true;
        Response.Redirect("/woning/" + Request.QueryString["id"] + "/omschrijving/" + 
                          CommonFunctions.GetAddress(Convert.ToInt32(Request.QueryString["id"]), true) + ".aspx");
    }

    protected void lnkVideo_Click(object sender, EventArgs e)
    {
        divDetails.Visible = divPlatteground.Visible = divBrochure.Visible = false;
        divVideo.Visible = true;
        divVideo.Attributes.Add("onload", "mediaspace()");
    }

    protected void lnkPlatteground_Click(object sender, EventArgs e)
    {
        divDetails.Visible = divVideo.Visible = divBrochure.Visible = false;
        divPlatteground.Visible = true;
    }

    protected void lnkBrochure_Click(object sender, EventArgs e)
    {
        divDetails.Visible = divVideo.Visible = divPlatteground.Visible = false;
        divBrochure.Visible = true;
    }
    #endregion
}
