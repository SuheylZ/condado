using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Telerik.Web.Data.Extensions;

using DBG = System.Diagnostics.Debug;

namespace UserControls
{    
    public class ColumnSelectionWithAggregate
    {        
        public ListItem Item { get; set; }
        public bool IsAggregate { get; set; }
        public Konstants.AggregateFunctionType aggType { get; set; }
    }
    public class ColumnSelectionEventArgs : EventArgs
    {
        public bool IsSelected { get; set; }
        public List<ListItem> Items { get; set; }
        public bool IsCancel { get; set; }
    }

    public class ColumnSelectedEventArgs : EventArgs
    {
        public ListItemCollection AvailableItems { get; set; }
        public ListItemCollection SelectedItems { get; set; }
        public List<ColumnSelectionWithAggregate> SelectedItemsWithAggregate { get; set; }
    }

    public partial class UserControlsColumnSelectionLists : SalesUserControl
    {
        public event EventHandler<ColumnSelectionEventArgs> ItemsShifting = null;
        public event EventHandler<ColumnSelectedEventArgs> ItemsShifted = null;

        protected override void InnerInit()
        {
            lbAvailable.Items.Clear();
            lbChosen.Items.Clear();
            if (lblTitle.InnerText.Trim() == string.Empty)
                lblTitle.Visible = false;
        }

        private List<ListItem> GetSelectedItems(ListBox source)
        {
            List<ListItem> list = new List<ListItem>();
            foreach (ListItem item in source.Items)
                if (item.Selected)
                    list.Add(item);
            return list;
        }

        private void MoveItems(ListBox source, ListBox target, bool bSelection, bool IsAggregate = false, Konstants.AggregateFunctionType aggType = Konstants.AggregateFunctionType.None)
        {
            List<ListItem> list = GetSelectedItems(source);
            
            ColumnSelectionEventArgs args = new ColumnSelectionEventArgs();
            args.IsSelected = bSelection;
            args.Items = list;
            args.IsCancel = false;

            if (ItemsShifting != null)
                ItemsShifting(this, args);

            if (!args.IsCancel)
            {                
                //For removing the item from source
                if (!bSelection)
                {
                    foreach (var item in list)
                        source.Items.Remove(item);
                }
                else
                {                    
                    foreach (var item in args.Items)
                    {                       
                        string modifiedText = item.Text;
                        //Skip the items of group separators
                        if (item.Value == "-1") continue;
                        if (IsAggregate && aggType != Konstants.AggregateFunctionType.None)
                        {
                            //YA[June 6, 2013] If aggreagate function is selected, check for column name does it satisfy the aggregate function limitations
                            //like sum function is only usable with numeric fields, etc.
                            int tagFieldId = 0;
                            int.TryParse(item.Value, out tagFieldId);
                            SalesTool.DataAccess.Models.TagFields nField = Engine.TagFieldsActions.Get(tagFieldId);
                            //Sum and Avg aggregate functions only support numeric values
                            if (nField.FilterDataType != (byte)Konstants.FilterFieldDataType.Numeric && (aggType == Konstants.AggregateFunctionType.Sum || aggType == Konstants.AggregateFunctionType.Average))
                                continue;
                            else
                                modifiedText = GetGroupFunctionText(aggType, item.Text);
                            
                        }
                        ListItem toAddItem = new ListItem(modifiedText, item.Value);                       
                        target.Items.Add(toAddItem);
                    }
                }
                SelectAll(target, false);

                if (ItemsShifted != null)
                {
                    var selectedEventArgs = new ColumnSelectedEventArgs();
                    selectedEventArgs.AvailableItems = this.lbAvailable.Items;
                    selectedEventArgs.SelectedItems = this.lbChosen.Items;                                        
                    ItemsShifted(this, selectedEventArgs);
                }
            }
        }
        private void SelectAll(ListBox source, bool bSelect = true)
        {
            foreach (ListItem item in source.Items)
                item.Selected = bSelect;
        }

        public string TitleAvailable { set { lblLeftTitle.Text = value; } }
        public string TitleSelected { set { lblRightTitle.Text = value; } }
        public string Title { set { lblTitle.InnerText = value; } }

        public ListItemCollection AvailableItems { get { return lbAvailable.Items; } }
        public ListItemCollection SelectedItems { get { return lbChosen.Items; } }

        public void Evt_DeselectOne_Clicked(object sender, EventArgs e)
        {
            MoveItems(lbChosen, lbAvailable, false);
            //this.Sort();
        }
        public void Evt_DeselectAll_Clicked(object sender, EventArgs e)
        {
            SelectAll(lbChosen);
            MoveItems(lbChosen, lbAvailable, false);
            //this.Sort();
        }
        public void Evt_SelectAll_Clicked(object sender, EventArgs e)
        {
            SelectAll(lbAvailable);
            MoveItems(lbAvailable, lbChosen, true);
            //this.Sort2();
        }
        public void Evt_SelectOne_Clicked(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true);
            //this.Sort2();
        }

        //SZ [Mar 5, 2013] added so that the selected items can be retrieved
        public ListItemCollection GetClickedItems(bool bSelected = false)
        {
            ListItemCollection items = new ListItemCollection();
            ListBox lb = (bSelected) ? lbChosen : lbAvailable;

            foreach (ListItem li in lb.Items)
                if (li.Selected)
                    items.Add(li);
            return items;
        }

        public ListItemCollection ClickedItems
        {
            get
            {
                return GetClickedItems(true);
            }
            set
            {
                SetClickedItems(GetClickedItems(true), value, true);
            }
        }

        public void SetClickedItems(ListItemCollection finder, ListItemCollection replacer, bool bSelected = false)
        {
            ListBox lb = (bSelected) ? lbChosen : lbAvailable;

            DBG.Assert(finder.Count == replacer.Count);

            for (int i = 0; i < finder.Count; i++)
            {
                int index = lb.Items.IndexOf(finder[i]);

                if (index != -1)
                {
                    lb.Items.RemoveAt(index);
                    lb.Items.Insert(index, replacer[i]);
                }
            }
        }

        //SZ [Apr1, 2013] Added to perform the auto selection thru code
        public void SafeMove(string item, bool MoveToSelection = true)
        {
            ListBox
                source = MoveToSelection ? lbAvailable : lbChosen,
                target = MoveToSelection ? lbChosen : lbAvailable;

            ListItem I = source.Items.FindByValue(item);
            if (I != null)
            {
                target.Items.Add(I);
                source.Items.Remove(I);
            }
        }

        //WM - Thu, 09 May, 2013
        public void SafeMove(string[] values, bool MoveToSelection = true)
        {
            if (values.Length == 0)
            {
                return;
            }

            ListBox
                source = MoveToSelection ? lbAvailable : lbChosen,
                target = MoveToSelection ? lbChosen : lbAvailable;

            foreach (var v in values)
            {
                ListItem I = source.Items.FindByValue(v);
                if (I != null)
                {
                    target.Items.Add(I);
                    source.Items.Remove(I);
                }
            }
        }

        public void SafeMove(System.Collections.ArrayList items, bool MoveToSelection = true)
        {
            ListBox
                source = MoveToSelection ? lbAvailable : lbChosen,
                target = MoveToSelection ? lbChosen : lbAvailable;

            foreach (var item in items)
            {
                ListItem I = source.Items.FindByText(item.ToString());
                if (I != null)
                {
                    target.Items.Add(I);
                    source.Items.Remove(I);
                }
            }
        }
        //SZ [Apr1, 2013] Added to perform the resetting of selection
        public void Reset()
        {
            if (lbChosen.Items.Count > 0)
            {
                List<ListItem> items = new List<ListItem>();

                foreach (ListItem I in lbChosen.Items)
                    items.Add(I);

                foreach (ListItem I in items)
                    lbAvailable.Items.Add(I);

                lbChosen.Items.Clear();
            }
        }

        public void DataBindAvailable(object source, string textFieldName, string valueFieldName)
        {
            lbAvailable.DataTextField = textFieldName;
            lbAvailable.DataValueField = valueFieldName;
            lbAvailable.DataSource = source;
            lbAvailable.DataBind();
        }
        public void DataBindSelected(object source, string textFieldName, string valueFieldName)
        {
            lbChosen.DataTextField = textFieldName;
            lbChosen.DataValueField = valueFieldName;
            lbChosen.DataSource = source;
            lbChosen.DataBind();
        }
        //SZ [Apr1, 2013] Added to perform the sorting of items based on the text displayed
        public void Sort(bool available = true)
        {
            ListBox source = available ? lbAvailable : lbChosen;
            List<ListItem> t = new List<ListItem>();
            System.Comparison<ListItem> compare = new System.Comparison<ListItem>(InnerCompareListItems);
            foreach (ListItem I in lbAvailable.Items)
                t.Add(I);

            t.Sort(compare);
            source.Items.Clear();
            source.Items.AddRange(t.ToArray());
        }

        //SZ [Apr1, 2013] Comparer function
        private static int InnerCompareListItems(ListItem x, ListItem y)
        {
            return string.Compare(x.Text, y.Text, false);
        }
        protected void btnCount_Click(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true,true,Konstants.AggregateFunctionType.Count);
        }
        protected void btnSum_Click(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true, true, Konstants.AggregateFunctionType.Sum);
        }
        protected void btnMin_Click(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true, true, Konstants.AggregateFunctionType.Min);
        }
        protected void btnMax_Click(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true, true, Konstants.AggregateFunctionType.Max);
        }
        protected void btnAverage_Click(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true, true, Konstants.AggregateFunctionType.Average);
        }
        protected void btnUp_Click(object sender, EventArgs e)
        {
            MoveUp();
        }
        protected void btnDown_Click(object sender, EventArgs e)
        {
            MoveDown();
        }

        //YA[May 14, 2013]
        /// <summary>
        /// Moves the selected items up one level
        /// </summary>
        private void MoveUp()
        {

            for (int i = 0; i < lbChosen.Items.Count; i++)
            {
                if (lbChosen.Items[i].Selected)//identify the selected item
                {
                    //swap with the top item(move up)
                    if (i > 0 && !lbChosen.Items[i - 1].Selected)
                    {
                        ListItem bottom = lbChosen.Items[i];
                        lbChosen.Items.Remove(bottom);
                        lbChosen.Items.Insert(i - 1, bottom);
                        lbChosen.Items[i - 1].Selected = true;
                    }
                }
            }
        }

        //YA[May 14, 2013]
        /// <summary>
        /// Moves the selected items one level down
        /// </summary>
        private void MoveDown()
        {
            int startindex = lbChosen.Items.Count - 1;
            for (int i = startindex; i > -1; i--)
            {
                if (lbChosen.Items[i].Selected)//identify the selected item
                {
                    //swap with the lower item(move down)
                    if (i < startindex && !lbChosen.Items[i + 1].Selected)
                    {
                        ListItem bottom = lbChosen.Items[i];
                        lbChosen.Items.Remove(bottom);
                        lbChosen.Items.Insert(i + 1, bottom);
                        lbChosen.Items[i + 1].Selected = true;
                    }
                }
            }
        }
        //YA[May 14, 2013]
        /// <summary>
        /// Add the group function to the original text
        /// </summary>
        /// <param name="nType">Aggregate function type</param>
        /// <param name="itemText">Original item text</param>
        /// <returns></returns>
        public string GetGroupFunctionText(Konstants.AggregateFunctionType nType, string itemText = "")
        {
            switch (nType)
            {
                case Konstants.AggregateFunctionType.Count:
                    itemText = "Count(" + itemText + ")";
                    break;
                case Konstants.AggregateFunctionType.Sum:
                    itemText = "Sum(" + itemText + ")";
                    break;
                case Konstants.AggregateFunctionType.Min:
                    itemText = "Min(" + itemText + ")";
                    break;
                case Konstants.AggregateFunctionType.Max:
                    itemText = "Max(" + itemText + ")";
                    break;
                case Konstants.AggregateFunctionType.Average:
                    itemText = "Avg(" + itemText + ")";
                    break;
            }
            return itemText;
        }
        //YA[June 06, 2013] 
        /// <summary>
        /// Changing the color for grouped item text, PreRender event is used to retain the attribute 
        /// values as value doesnot persist when page postbacks beacause the 
        /// OnPreRender method performs any necessary prerendering steps prior to saving view state and rendering content for the ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbAvailable_PreRender(object sender, EventArgs e)
        {
            foreach (ListItem item in lbAvailable.Items)
            {
                //Check for grouped item values
                if (item.Value == "-1")
                    item.Attributes.Add("style", "background-color:grey");
            }
        }
}
}