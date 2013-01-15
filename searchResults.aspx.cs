using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

public partial class searchResults : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        MasterPageFile = "~/sites/" + GeneralFunctions.getSiteDirectory() + "/Default.master";
    }

    public string strSearch, strType, strSQL;
    string[] arrSearch;
    string ConnectionString = ConfigurationSettings.AppSettings["ConnectionStringSQL"];

    protected void Page_Load(object sender, EventArgs e)
    {
        Literal litBrowserTitle = (Literal)Master.FindControl("litBrowserTitle");
        litBrowserTitle.Text = litPageTitle.Text = "Resultaten";
        strSearch = Request.QueryString["q"];
        strType = Request.QueryString["type"];

        bind();        
    }

    private void bind()
    {
        if (!String.IsNullOrEmpty(strSearch))
        {
            arrSearch = strSearch.Split(' ');
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                DataSet myDataSet = new DataSet();
                SqlDataAdapter myDataAdapter;

                if (String.IsNullOrEmpty(strType))
                {
                    strSQL = SelectListings();
                    strSQL += "; " + SelectPosts();
                    strSQL += "; " + SelectNews();
                    strSQL += "; " + SelectPages();
                    strSQL += "; " + SelectOffices();
                    strSQL += "; " + SelectAgents();

                    myDataAdapter = new SqlDataAdapter(strSQL, myConnection);
                    myDataAdapter.TableMappings.Add("Table", "Table");
                    myDataAdapter.TableMappings.Add("Table1", "Table");
                    myDataAdapter.TableMappings.Add("Table2", "Table");
                    myDataAdapter.TableMappings.Add("Table3", "Table");
                    myDataAdapter.TableMappings.Add("Table4", "Table");
                    myDataAdapter.TableMappings.Add("Table5", "Table");

                    myDataAdapter.Fill(myDataSet);
                }
                else
                {
                    if (strType == "listings")
                        strSQL = SelectListings();
                    else if (strType == "blog")
                        strSQL = SelectPosts();
                    else if (strType == "news")
                        strSQL = SelectNews();
                    else if (strType == "pages")
                        strSQL = SelectPages();
                    else if (strType == "kantoren")
                        strSQL = SelectOffices();
                    else if (strType == "agents")
                        strSQL = SelectAgents();

                    myDataAdapter = new SqlDataAdapter(strSQL, myConnection);
                    myDataAdapter.TableMappings.Add("Table", "Table");
                    myDataAdapter.Fill(myDataSet);
                }

                SearchListview.DataSource = myDataSet;
                SearchListview.DataBind();
                DataPager1.Visible = (DataPager1.PageSize < DataPager1.TotalRowCount);
            }
        }
    }

    protected void SearchListview_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
    {
        this.DataPager1.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        bind();
    }

    private string SelectListings()
    {
        string strSQLListings = @"SELECT 'listings' as ResultType, listingID as ResultID, 
                                  listingAddress + ' ' + listingAddressNumber as Title, 
                                  listingDescription as Content
                                  FROM tblListings
                                  WHERE (IsDeleted IS NULL OR IsDeleted = 0) AND (";

        for (int i = 0; i < arrSearch.Length; i++)
        {
            strSQLListings += "listingAddress LIKE '%" + arrSearch[i] + "%' "
                            + "OR listingDescription LIKE '%" + arrSearch[i] + "%' ";

            if (i < arrSearch.Length - 1)
                strSQLListings += " OR ";
        }
        strSQLListings += ") AND listingActive = 1 "
                        + "AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY listingAddress";

        return strSQLListings;
    }

    private string SelectPosts()
    {
        string strSQLListings = @"SELECT 'blog' as ResultType, postID as ResultID, 
                                  postTitle as Title, postContent as Content
                                  FROM tblPosts
                                  WHERE (";

        for (int i = 0; i < arrSearch.Length; i++)
        {
            strSQLListings += "postTitle LIKE '%" + arrSearch[i] + "%' "
                            + "OR postContent LIKE '%" + arrSearch[i] + "%' ";

            if (i < arrSearch.Length - 1)
                strSQLListings += " OR ";
        }
        strSQLListings += ") AND postActive = 1 "
                        + "AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY postPublishDate";

        return strSQLListings;
    }

    private string SelectNews()
    {
        string strSQLListings = @"SELECT 'news' as ResultType, newsID as ResultID, 
                                  newsTitle as Title, newsContent as Content
                                  FROM tblNews
                                  WHERE (";

        for (int i = 0; i < arrSearch.Length; i++)
        {
            strSQLListings += "newsTitle LIKE '%" + arrSearch[i] + "%' "
                            + "OR newsContent LIKE '%" + arrSearch[i] + "%' ";

            if (i < arrSearch.Length - 1)
                strSQLListings += " OR ";
        }
        strSQLListings += ") AND newsActive = 1 "
                        + "AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY newsPublishDate";

        return strSQLListings;
    }

    private string SelectPages()
    {
        string strSQLListings = @"SELECT 'pages' as ResultType, pageID as ResultID, 
                                  pageTitle as Title, pageContent as Content
                                  FROM tblPages
                                  WHERE pageSearch = 1 AND (";

        for (int i = 0; i < arrSearch.Length; i++)
        {
            strSQLListings += "pageTitle LIKE '%" + arrSearch[i] + "%' "
                            + "OR pageContent LIKE '%" + arrSearch[i] + "%' ";

            if (i < arrSearch.Length - 1)
                strSQLListings += " OR ";
        }
        strSQLListings += ") AND pageActive = 1 "
                        + "AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY pageTitle";

        return strSQLListings;
    }

    private string SelectOffices()
    {
        string strSQLListings = @"SELECT 'offices' as ResultType, officeID as ResultID, 
                                  officeName as Title, officeDescription as Content
                                  FROM tblOffices
                                  WHERE (";

        for (int i = 0; i < arrSearch.Length; i++)
        {
            strSQLListings += "officeName LIKE '%" + arrSearch[i] + "%' "
                            + "OR officeDescription LIKE '%" + arrSearch[i] + "%' "
                            + "OR officeAddress + officeAddressNumber LIKE  '%" + arrSearch[i] + "%' ";

            if (i < arrSearch.Length - 1)
                strSQLListings += " OR ";
        }
        strSQLListings += ") AND officeActive = 1 "
                        + "AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY officeName";

        return strSQLListings;
    }

    private string SelectAgents()
    {
        string strSQLListings = @"SELECT 'agents' as ResultType, agentID as ResultID, 
                                  agentName as Title, agentDescription as Content
                                  FROM tblAgents
                                  WHERE (";

        for (int i = 0; i < arrSearch.Length; i++)
        {
            strSQLListings += "agentName LIKE '%" + arrSearch[i] + "%' "
                            + "OR agentDescription LIKE '%" + arrSearch[i] + "%' ";

            if (i < arrSearch.Length - 1)
                strSQLListings += " OR ";
        }
        strSQLListings += ") AND agentActive = 1 "
                        + "AND siteID = " + GeneralFunctions.getSiteID() + " ORDER BY agentName";

        return strSQLListings;
    }
}
