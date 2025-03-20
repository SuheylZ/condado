using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
//using Telerik.Web.Data.Extensions;

using DBG = System.Diagnostics.Debug;

namespace UserControls
{
    public class SelectionEventArgs : EventArgs
    {
        public bool IsSelected { get; set; }
        public List<ListItem> Items { get; set; }
        public bool IsCancel { get; set; }
    }

    public class SelectedEventArgs : EventArgs
    {
        public ListItemCollection AvailableItems { get; set; }
        public ListItemCollection SelectedItems { get; set; }
    }

    public partial class UserControlsSelectionLists : SalesUserControl
    {
        /// <summary>
        /// To identify that the data is grouped data
        /// </summary>
        bool IsGroupedData
        {
            get
            {
                bool lAns = false;
                bool.TryParse(hdnfieldIsGroupedData.Value, out lAns);
                return lAns;
            }
            set
            {
                hdnfieldIsGroupedData.Value = value.ToString();
            }
        }
        public event EventHandler<SelectionEventArgs> ItemsShifting = null;
        public event EventHandler<SelectedEventArgs> ItemsShifted = null;

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
                {
                    //YA[June 26, 2013] To avoid the grouped items
                    if(item.Value != "-1")
                        list.Add(item);
                }
            return list;
        }

        private void MoveItems(ListBox source, ListBox target, bool bSelection)
        {
            List<ListItem> list = GetSelectedItems(source);
            SelectionEventArgs args = new SelectionEventArgs();
            args.IsSelected = bSelection;
            args.Items = list;
            args.IsCancel = false;

            if (ItemsShifting != null)
                ItemsShifting(this, args);

            if (!args.IsCancel)
            {     
                //YA[June 26, 2013]
                //This dictionary will contain the list of items selected and there corresponding group item. 
                //This will be used to transfer the item to its corresponding group in the selected list.
                Dictionary<ListItem, ListItem> groupNamesWithItems = new Dictionary<ListItem, ListItem>();
                foreach (var item in list)
                {
                    if (IsGroupedData)
                    {
                        //YA[June 26, 2013]
                        //Get the index of selected item and go upward to find its group.
                        int indexOfItem = source.Items.IndexOf(item);
                        for (int i = indexOfItem - 1; i >= 0; i--)
                        {
                            if (source.Items[i].Value == "-1")
                            {
                                groupNamesWithItems.Add(item, source.Items[i]);
                                break;
                            }
                        }
                    }
                    source.Items.Remove(item);
                }

                foreach (var item in args.Items)
                {
                    if (groupNamesWithItems.Count > 0)
                    {
                        //YA[June 26, 2013]
                        //Add the item to its corresponding group.
                        target.Items.Insert(target.Items.IndexOf(groupNamesWithItems[item]) + 1, item);                                    
                    }
                    else
                        target.Items.Add(item);
                }

                SelectAll(target, false);

                if (ItemsShifted != null)
                {
                    var selectedEventArgs = new SelectedEventArgs();

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
            if (!IsGroupedData)
            this.Sort();
        }
        public void Evt_DeselectAll_Clicked(object sender, EventArgs e)
        {
            SelectAll(lbChosen);
            MoveItems(lbChosen, lbAvailable, false);
            if (!IsGroupedData)
            this.Sort();
        }
        public void Evt_SelectAll_Clicked(object sender, EventArgs e)
        {
            SelectAll(lbAvailable);
            MoveItems(lbAvailable, lbChosen, true);
            if (!IsGroupedData)
            Sort(false);
            
        }
        public void Evt_SelectOne_Clicked(object sender, EventArgs e)
        {
            MoveItems(lbAvailable, lbChosen, true);
            if (!IsGroupedData)
            Sort(false);
            
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
        public void SafeMove(string item, bool MoveToSelection=true)
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

        public void DataBindAvailable(IQueryable<object> source, string textFieldName, string valueFieldName)
        {
            lbAvailable.DataTextField = textFieldName;
            lbAvailable.DataValueField = valueFieldName;
            lbAvailable.DataSource = source.ToList();
            lbAvailable.DataBind();
        }
        public void DataBindSelected(IQueryable<object> source, string textFieldName, string valueFieldName)
        {
            lbChosen.DataTextField = textFieldName;
            lbChosen.DataValueField = valueFieldName;
            lbChosen.DataSource = source.ToList();
            lbChosen.DataBind();
        }
        //SZ [Apr1, 2013] Added to perform the sorting of items based on the text displayed
        public void Sort(bool available = true)
        {
            ListBox source = available ? lbAvailable : lbChosen;
            
            List<ListItem> t = new List<ListItem>();
            
            System.Comparison<ListItem> compare = new System.Comparison<ListItem>(InnerCompareListItems);
            foreach (ListItem I in source.Items)
                t.Add(I);

            t.Sort(compare);
            source.Items.Clear();
            source.Items.AddRange(t.ToArray());
        }


        // SZ [May 15, 2013] function has been commented out as the above function implements the same thing if Sort(false).
        // Sort(bool) had a bug which has been corrected. 
        //public void Sort2(bool chosen = true)
        //{
        //    ListBox source = chosen ? lbChosen : lbAvailable;
        //    List<ListItem> t = new List<ListItem>();
        //    System.Comparison<ListItem> compare = new System.Comparison<ListItem>(InnerCompareListItems);
        //    foreach (ListItem I in lbChosen.Items)
        //        t.Add(I);

        //    t.Sort(compare);
        //    source.Items.Clear();
        //    source.Items.AddRange(t.ToArray());
        //}
        //SZ [Apr1, 2013] Comparer function
        private static int InnerCompareListItems(ListItem x, ListItem y)
        {
            return string.Compare(x.Text, y.Text, false);
        }
        //YA[June 11, 2013] 
        /// <summary>
        /// Changing the color for grouped item text for available items, PreRender event is used to retain the attribute 
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
        //YA[June 26, 2013] 
        /// <summary>
        /// Changing the color for grouped item text for selected items, PreRender event is used to retain the attribute 
        /// values as value doesnot persist when page postbacks beacause the 
        /// OnPreRender method performs any necessary prerendering steps prior to saving view state and rendering content for the ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lbSelected_PreRender(object sender, EventArgs e)
        {
            foreach (ListItem item in lbChosen.Items)
            {
                //Check for grouped item values
                if (item.Value == "-1")
                    item.Attributes.Add("style", "background-color:grey");
            }
        }
        public void SetGroupFlag()
        {
            IsGroupedData = true;
        }
    }
}