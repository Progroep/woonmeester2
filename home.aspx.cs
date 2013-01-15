using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.UI.HtmlControls;

public partial class home : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        MasterPageFile = "~/sites/" + GeneralFunctions.getSiteDirectory() + "/Default.master";
    }

    public string ConnectionString = ConfigurationSettings.AppSettings["ConnectionStringSQL"];
    public string strSQL, strAction;
    public string strListingID;

    protected void Page_Load(object sender, EventArgs e)
    {
        Literal litBrowserTitle = (Literal)Master.FindControl("litBrowserTitle");
        Literal Description = (Literal)Master.FindControl("Description");
        Literal Keywords = (Literal)Master.FindControl("Keywords");
        Description.Text = "<meta name=\"description\" content=\"" + ConfigurationManager.AppSettings["defDescription"] + "\" />";
        Keywords.Text = "<meta name=\"keywords\" content=\"" + ConfigurationManager.AppSettings["defKeywords"] + "\" />";        
        litBrowserTitle.Text = "Home";

        strAction = Request.QueryString["action"];
        if (!String.IsNullOrEmpty(strAction))
        {
            HtmlControl body = (HtmlControl)Master.FindControl("body");
            body.Attributes.Add("onload", "window.location.href = '#message'");
        }

        Panel panFeatured = (Panel)Master.FindControl("panFeatured");
        panFeatured.Visible = true;

        if (!IsPostBack)
        {
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                strSQL = @"SELECT photoTypeID
                           FROM tblPhotoTypes
                           WHERE photoTypeName = 'Page'";

                myConnection.Open();
                SqlCommand photoTypeCommand = new SqlCommand(strSQL, myConnection);
                int intPhotoType = (int)photoTypeCommand.ExecuteScalar();
                myConnection.Close();

                #region Conection to news 
                strSQL = @"SELECT TOP 2 pageID, pageTitle, pageContent, photoName
                           FROM tblPages t1
                           LEFT JOIN tblPhotos
                           ON (pageID = photoParent AND photoTypeID = 6)
                           WHERE pageActive = 1
                           AND (IsDeleted IS NULL OR IsDeleted = 0)
                           AND pageType = 'news'
                           AND t1.siteID = " + GeneralFunctions.getSiteID() + " ORDER BY pagePublishDate DESC";

                SqlCommand newsCommand = new SqlCommand(strSQL, myConnection);
                SqlDataAdapter newsDataAdapter = new SqlDataAdapter(newsCommand);
                DataSet newsDataSet = new DataSet();
                newsDataAdapter.Fill(newsDataSet, "News");

                rptNews.DataSource = newsDataSet;
                rptNews.DataBind();
                #endregion

                #region The two blocks
                //The homepage ID
                strSQL = @"SELECT pageID
                           FROM tblPages
                           WHERE pageURL = 'home' AND siteID = " + GeneralFunctions.getSiteID();

                myConnection.Open();
                SqlCommand pidCommand = new SqlCommand(strSQL, myConnection);
                int homePageID = (int)pidCommand.ExecuteScalar();
                myConnection.Close();

                strSQL = @"SELECT TOP 2 *, (SELECT photoName
								            FROM tblPhotos
								            WHERE photoParent = pageID
								            AND photoTypeID = 7) as photoName
                           FROM tblPages t1
                           WHERE pageActive = 1 AND pageParent = " + 
                           homePageID + " AND siteID = " + GeneralFunctions.getSiteID() + 
                           " ORDER BY pageOrder";

                SqlCommand bCommand = new SqlCommand(strSQL, myConnection);
                SqlDataAdapter bDataAdapter = new SqlDataAdapter(bCommand);
                DataSet bDataSet = new DataSet();
                bDataAdapter.Fill(bDataSet, "Blocks");

                rptBlocks.DataSource = bDataSet;
                rptBlocks.DataBind();
                #endregion

                #region Connection to blog
                strSQL = @"SELECT TOP 3 postID, postTitle, postContent, postPublishDate
                           FROM tblPosts
                           WHERE postActive = 1
                           AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY postPublishDate DESC";

                SqlCommand blogCommand = new SqlCommand(strSQL, myConnection);
                SqlDataAdapter blogDataAdapter = new SqlDataAdapter(blogCommand);
                DataSet blogDataSet = new DataSet();
                blogDataAdapter.Fill(blogDataSet, "Blog");

                rptBlog.DataSource = blogDataSet;
                rptBlog.DataBind();
                #endregion

            }
        }
    }
}
