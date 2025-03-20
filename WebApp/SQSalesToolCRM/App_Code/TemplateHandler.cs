using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;

public class GridViewTemplate : ITemplate
{

    //A variable to hold the type of ListItemType.

    private ListItemType _templateType;
    //A variable to hold the column name.

    private string _columnName;
    //A vairable to determine item type.

    private string _itemType;
    //A vairable to determine additional information.

    private string _addlInfo;
    //Constructor where we define the template type and column name.
    public GridViewTemplate(ListItemType type, string colname, string itemType, string addlInfo)
    {
        //Stores the template type.
        _templateType = type;

        //Stores the column name.
        _columnName = colname;

        //Stores the item type.
        _itemType = itemType;

        //Stores the item type.
        _addlInfo = addlInfo;

    }


    private void ITemplate_InstantiateIn(System.Web.UI.Control container)
    {
        switch (_templateType)
        {
            case ListItemType.Header:
                //Creates a new label control and add it to the container.
                Label lbl = new Label();
                //Allocates the new label object.
                lbl.Text = _columnName;
                //Assigns the name of the column in the lable.
                container.Controls.Add(lbl);
                //Adds the newly created label control to the container.
                break; // TODO: might not be correct. Was : Exit Select

            case ListItemType.Item:
                if (_itemType == "text")
                {
                    //Creates a new text box control and add it to the container.
                    TextBox tb1 = new TextBox();
                    //Allocates the new text box object.
                    //tb1.DataBinding += New EventHandler(AddressOf tb1_DataBinding)
                    tb1.DataBinding += new EventHandler(tb1_DataBinding);
                    //Attaches the data binding event.
                    tb1.Columns = 4;
                    //Creates a column with size 4.
                    container.Controls.Add(tb1);
                    //Adds the newly created textbox to the container.
                }
                else if (_itemType == "campaign_group_options")
                {
                    DropDownList tb1 = new DropDownList();
                    tb1.ID = _columnName + "max";
                    for(int x = 0; x <= 100; x++)
                    {
                        if (x == 0)
                        {
                            ListItem li1 = new ListItem("Unlimited", "");
                            tb1.Items.Add(li1);
                        }
                        else
                        {
                            ListItem li1 = new ListItem((x - 1).ToString(), (x - 1).ToString());
                            tb1.Items.Add(li1);
                        }
                    }

                    DropDownList tb2 = new DropDownList();
                    tb2.ID = _columnName + "level";
                    for(int x = 1; x <= 4; x++)
                    {
                        if (x == 0)
                        {
                            ListItem li1 = new ListItem("Unlimited", "");
                            tb2.Items.Add(li1);
                        }
                        else
                        {
                            ListItem li1 = new ListItem(x.ToString(), x.ToString());
                            tb2.Items.Add(li1);
                        }
                    }

                    tb1.Width = new Unit("85px");
                    tb2.Width = new Unit("85px");

                    tb1.DataBinding += new EventHandler(tbx1_DataBinding);
                    tb2.DataBinding += new EventHandler(tbx2_DataBinding);

                    container.Controls.Add(new LiteralControl("<table class=CampaignGroupOptionsTable><tr><td>Max Quota</td><td>Level</td></tr><tr><td>"));
                    container.Controls.Add(tb1);
                    container.Controls.Add(new LiteralControl("</td><td>"));
                    container.Controls.Add(tb2);
                    container.Controls.Add(new LiteralControl("</td></tr></table>"));
                }
                else if (_itemType == "state_group_options")
                {
                    DropDownList tb1 = new DropDownList();
                    tb1.ID = _columnName + "priority";
                    for(int x = 0; x <= 99; x++)
                    {
                        ListItem li1 = new ListItem(x.ToString(), x.ToString());
                        tb1.Items.Add(li1);
                    }

                    tb1.Width = new Unit("85px");

                    tb1.DataBinding += new EventHandler(tbx1a_DataBinding);

                    container.Controls.Add(new LiteralControl("<table class=StateGroupOptionsTable><tr><td>Priority</td></tr><tr><td>"));
                    container.Controls.Add(tb1);
                    container.Controls.Add(new LiteralControl("</td></tr></table>"));
                }
                else if (_itemType == "age_group_options")
                {
                    DropDownList tb1 = new DropDownList();
                    tb1.ID = _columnName + "priority";
                    for(int x = 0; x <= 99; x++)
                    {
                        ListItem li1 = new ListItem(x.ToString(), x.ToString());
                        tb1.Items.Add(li1);
                    }

                    tb1.Width = new Unit("85px");

                    tb1.DataBinding += new EventHandler(tbx1a_DataBinding);

                    container.Controls.Add(new LiteralControl("<table class=AgeGroupOptionsTable><tr><td>Priority</td></tr><tr><td>"));
                    container.Controls.Add(tb1);
                    container.Controls.Add(new LiteralControl("</td></tr></table>"));
                }
                else if (_itemType == "agentid")
                {
                    Label lb1 = new Label();
                    lb1.ID = "agentid";
                    lb1.DataBinding += new EventHandler(lb1_DataBinding);
                    container.Controls.Add(lb1);
                }
                else if (_itemType == "campaigngroupid")
                {
                    Label lb1 = new Label();
                    lb1.ID = _columnName + "campaigngroupid";
                    lb1.DataBinding += new EventHandler(lb1_DataBinding);
                    container.Controls.Add(lb1);
                }
                else
                {
                    Label lb1 = new Label();
                    lb1.DataBinding += new EventHandler(lb1_DataBinding);
                    container.Controls.Add(lb1);
                }
                break; // TODO: might not be correct. Was : Exit Select

            case ListItemType.EditItem:
                break; // TODO: might not be correct. Was : Exit Select

            case ListItemType.Footer:
                break; // TODO: might not be correct. Was : Exit Select
        }

    }
    void ITemplate.InstantiateIn(System.Web.UI.Control container)
    {
        ITemplate_InstantiateIn(container);
    }

    private void tb1_DataBinding(object sender, EventArgs e)
    {
        TextBox txtdata = (TextBox)sender;

        GridViewRow container = (GridViewRow)txtdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName);

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            txtdata.Text = dataValue.ToString();
        }

    }

    private void tbx1_DataBinding(object sender, EventArgs e)
    {
        DropDownList dropdata = (DropDownList)sender;

        GridViewRow container = (GridViewRow)dropdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName + "max");

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            dropdata.Text = dataValue.ToString();
        }
    }

    private void tbx1a_DataBinding(object sender, EventArgs e)
    {
        DropDownList dropdata = (DropDownList)sender;

        GridViewRow container = (GridViewRow)dropdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName + "priority");

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            dropdata.Text = dataValue.ToString();
        }
    }

    private void tbx2_DataBinding(object sender, EventArgs e)
    {
        DropDownList dropdata = (DropDownList)sender;

        GridViewRow container = (GridViewRow)dropdata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName + "level");

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            dropdata.Text = dataValue.ToString();
        }
    }

    private void lb1_DataBinding(object sender, EventArgs e)
    {
        Label lbldata = (Label)sender;

        GridViewRow container = (GridViewRow)lbldata.NamingContainer;

        object dataValue = DataBinder.Eval(container.DataItem, _columnName);

        if ((!object.ReferenceEquals(dataValue, DBNull.Value)))
        {
            lbldata.Text = dataValue.ToString();
        }

    }

}
