namespace SalesTool.Reports
{
    partial class PrioritizedListReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Drawing.FormattingRule formattingRule1 = new Telerik.Reporting.Drawing.FormattingRule();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            this.labelsGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.labelsGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.pageHeader = new Telerik.Reporting.PageHeaderSection();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.currentTimeTextBox = new Telerik.Reporting.TextBox();
            this.pageInfoTextBox = new Telerik.Reporting.TextBox();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.reportFooter = new Telerik.Reporting.ReportFooterSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.reportSource = new Telerik.Reporting.SqlDataSource();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox26 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox28 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox34 = new Telerik.Reporting.TextBox();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.textBox36 = new Telerik.Reporting.TextBox();
            this.textBox37 = new Telerik.Reporting.TextBox();
            this.textBox38 = new Telerik.Reporting.TextBox();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox41 = new Telerik.Reporting.TextBox();
            this.textBox32 = new Telerik.Reporting.TextBox();
            this.textBox33 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // labelsGroupFooterSection
            // 
            this.labelsGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.25471699237823486D);
            this.labelsGroupFooterSection.Name = "labelsGroupFooterSection";
            this.labelsGroupFooterSection.Style.Visible = false;
            // 
            // labelsGroupHeaderSection
            // 
            this.labelsGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20003926753997803D);
            this.labelsGroupHeaderSection.Name = "labelsGroupHeaderSection";
            this.labelsGroupHeaderSection.PrintOnEveryPage = true;
            // 
            // pageHeader
            // 
            this.pageHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D);
            this.pageHeader.Name = "pageHeader";
            this.pageHeader.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(0.281132310628891D);
            this.pageFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.currentTimeTextBox,
            this.pageInfoTextBox});
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // currentTimeTextBox
            // 
            this.currentTimeTextBox.Format = "{0:D}";
            this.currentTimeTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.currentTimeTextBox.Name = "currentTimeTextBox";
            this.currentTimeTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.7992868423461914D), Telerik.Reporting.Drawing.Unit.Inch(0.26226440072059631D));
            this.currentTimeTextBox.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.currentTimeTextBox.StyleName = "PageInfo";
            this.currentTimeTextBox.Value = "=NOW()";
            // 
            // pageInfoTextBox
            // 
            this.pageInfoTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.83702278137207D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.pageInfoTextBox.Name = "pageInfoTextBox";
            this.pageInfoTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.7992868423461914D), Telerik.Reporting.Drawing.Unit.Inch(0.26226440072059631D));
            this.pageInfoTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageInfoTextBox.Style.Visible = false;
            this.pageInfoTextBox.StyleName = "PageInfo";
            this.pageInfoTextBox.Value = "=PageNumber";
            // 
            // reportHeader
            // 
            this.reportHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(0.29999995231628418D);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.reportHeader.Name = "reportHeader";
            this.reportHeader.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.reportHeader.Style.Font.Bold = true;
            this.reportHeader.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.94425296690315E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9370555896312E-05D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.8999996185302734D), Telerik.Reporting.Drawing.Unit.Inch(0.26222482323646545D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(15D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.Value = "Prioritized List Report";
            // 
            // reportFooter
            // 
            this.reportFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(0.25471699237823486D);
            this.reportFooter.Name = "reportFooter";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.45837295055389404D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detail.Name = "detail";
            // 
            // reportSource
            // 
            this.reportSource.ConnectionString = "ApplicationServices";
            this.reportSource.Name = "reportSource";
            this.reportSource.Parameters.AddRange(new Telerik.Reporting.SqlDataSourceParameter[] {
            new Telerik.Reporting.SqlDataSourceParameter("@userKey", System.Data.DbType.String, "=Parameters.userkey.Value")});
            this.reportSource.SelectCommand = "dbo.report_PrioritizedList";
            this.reportSource.SelectCommandType = Telerik.Reporting.SqlDataSourceCommandType.StoredProcedure;
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.79275357723236084D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0654807090759277D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3501700162887573D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0595842599868774D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0654804706573486D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.123598575592041D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0267360210418701D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8597486019134522D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox34);
            this.table1.Body.SetCellContent(0, 1, this.textBox35);
            this.table1.Body.SetCellContent(0, 2, this.textBox37);
            this.table1.Body.SetCellContent(0, 3, this.textBox38);
            this.table1.Body.SetCellContent(0, 4, this.textBox39);
            this.table1.Body.SetCellContent(0, 5, this.textBox40);
            this.table1.Body.SetCellContent(0, 6, this.textBox41);
            this.table1.Body.SetCellContent(0, 7, this.textBox32);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox2;
            tableGroup2.Name = "group5";
            tableGroup2.ReportItem = this.textBox36;
            tableGroup3.Name = "tableGroup1";
            tableGroup3.ReportItem = this.textBox20;
            tableGroup4.Name = "tableGroup2";
            tableGroup4.ReportItem = this.textBox22;
            tableGroup5.Name = "group";
            tableGroup5.ReportItem = this.textBox24;
            tableGroup6.Name = "group1";
            tableGroup6.ReportItem = this.textBox26;
            tableGroup7.Name = "group2";
            tableGroup7.ReportItem = this.textBox28;
            tableGroup8.Name = "group4";
            tableGroup8.ReportItem = this.textBox33;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.ColumnGroups.Add(tableGroup7);
            this.table1.ColumnGroups.Add(tableGroup8);
            this.table1.DataSource = this.reportSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox20,
            this.textBox22,
            this.textBox24,
            this.textBox26,
            this.textBox28,
            this.textBox34,
            this.textBox36,
            this.textBox35,
            this.textBox37,
            this.textBox38,
            this.textBox39,
            this.textBox40,
            this.textBox41,
            this.textBox33,
            this.textBox32});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.table1.Name = "table1";
            tableGroup9.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup9.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup9);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.3435525894165039D), Telerik.Reporting.Drawing.Unit.Inch(0.45833343267440796D));
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.79275363683700562D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666716337204D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox2.Style.Color = System.Drawing.Color.White;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.Value = "Account Id";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7853248119354248D), Telerik.Reporting.Drawing.Unit.Inch(0.22916668653488159D));
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3501700162887573D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox20.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox20.Style.Color = System.Drawing.Color.White;
            this.textBox20.Style.Font.Bold = true;
            this.textBox20.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox20.Value = "First Name";
            // 
            // textBox21
            // 
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0874084234237671D), Telerik.Reporting.Drawing.Unit.Inch(0.22916668653488159D));
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.059584379196167D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox22.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox22.Style.Color = System.Drawing.Color.White;
            this.textBox22.Style.Font.Bold = true;
            this.textBox22.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox22.Value = "Last Name";
            // 
            // textBox23
            // 
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4728245735168457D), Telerik.Reporting.Drawing.Unit.Inch(0.22916668653488159D));
            // 
            // textBox24
            // 
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0654805898666382D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox24.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox24.Style.Color = System.Drawing.Color.White;
            this.textBox24.Style.Font.Bold = true;
            this.textBox24.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox24.StyleName = "";
            this.textBox24.Value = "Birth Date";
            // 
            // textBox25
            // 
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1458327770233154D), Telerik.Reporting.Drawing.Unit.Inch(0.22916668653488159D));
            this.textBox25.StyleName = "";
            // 
            // textBox26
            // 
            this.textBox26.Name = "textBox26";
            this.textBox26.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.123598575592041D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox26.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox26.Style.Color = System.Drawing.Color.White;
            this.textBox26.Style.Font.Bold = true;
            this.textBox26.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox26.StyleName = "";
            this.textBox26.Value = "Campaign";
            // 
            // textBox27
            // 
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2083332538604736D), Telerik.Reporting.Drawing.Unit.Inch(0.22916668653488159D));
            this.textBox27.StyleName = "";
            // 
            // textBox28
            // 
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0267362594604492D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox28.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox28.Style.Color = System.Drawing.Color.White;
            this.textBox28.Style.Font.Bold = true;
            this.textBox28.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox28.StyleName = "";
            this.textBox28.Value = "Status";
            // 
            // textBox29
            // 
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1041663885116577D), Telerik.Reporting.Drawing.Unit.Inch(0.22916668653488159D));
            this.textBox29.StyleName = "";
            // 
            // textBox34
            // 
            formattingRule1.Filters.Add(new Telerik.Reporting.Filter("= Fields.RowId%2", Telerik.Reporting.FilterOperator.Equal, "1"));
            formattingRule1.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(176)))), ((int)(((byte)(219)))));
            this.textBox34.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox34.Name = "textBox34";
            this.textBox34.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.79275363683700562D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox34.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox34.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox34.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox34.Value = "=Fields.accountId";
            // 
            // textBox35
            // 
            this.textBox35.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0654807090759277D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox35.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox35.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox35.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox35.StyleName = "";
            this.textBox35.Value = "=Fields.dateCreated";
            // 
            // textBox36
            // 
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0654807090759277D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox36.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox36.Style.Color = System.Drawing.Color.White;
            this.textBox36.Style.Font.Bold = true;
            this.textBox36.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox36.StyleName = "";
            this.textBox36.Value = "Created On";
            // 
            // textBox37
            // 
            this.textBox37.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3501700162887573D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox37.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox37.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox37.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox37.Value = "=Fields.firstName";
            // 
            // textBox38
            // 
            this.textBox38.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.059584379196167D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox38.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox38.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox38.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox38.Value = "=Fields.lastName";
            // 
            // textBox39
            // 
            this.textBox39.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0654805898666382D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox39.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox39.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox39.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox39.Value = "=Fields.dateOfBirth";
            // 
            // textBox40
            // 
            this.textBox40.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.123598575592041D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox40.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox40.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox40.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox40.Value = "=Fields.leadCampaign";
            // 
            // textBox41
            // 
            this.textBox41.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0267362594604492D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox41.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox41.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox41.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox41.Value = "=Fields.leadStatus";
            // 
            // textBox32
            // 
            this.textBox32.ConditionalFormatting.AddRange(new Telerik.Reporting.Drawing.FormattingRule[] {
            formattingRule1});
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8597486019134522D), Telerik.Reporting.Drawing.Unit.Inch(0.22916673123836517D));
            this.textBox32.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(207)))), ((int)(((byte)(233)))));
            this.textBox32.Style.BorderColor.Top = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(145)))), ((int)(((byte)(206)))));
            this.textBox32.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox32.StyleName = "";
            this.textBox32.Value = "=Fields.SubStatus1";
            // 
            // textBox33
            // 
            this.textBox33.Name = "textBox33";
            this.textBox33.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8597486019134522D), Telerik.Reporting.Drawing.Unit.Inch(0.2291666567325592D));
            this.textBox33.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(97)))), ((int)(((byte)(148)))));
            this.textBox33.Style.Color = System.Drawing.Color.White;
            this.textBox33.Style.Font.Bold = true;
            this.textBox33.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox33.StyleName = "";
            this.textBox33.Value = "Sub Status";
            // 
            // PrioritizedListReport
            // 
            this.DataSource = this.reportSource;
            group1.GroupFooter = this.labelsGroupFooterSection;
            group1.GroupHeader = this.labelsGroupHeaderSection;
            group1.Name = "labelsGroup";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.labelsGroupHeaderSection,
            this.labelsGroupFooterSection,
            this.pageHeader,
            this.pageFooter,
            this.reportHeader,
            this.reportFooter,
            this.detail});
            this.Name = "Sample";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowNull = true;
            reportParameter1.Name = "userkey";
            reportParameter1.Value = "";
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule1.Style.Color = System.Drawing.Color.Black;
            styleRule1.Style.Font.Bold = true;
            styleRule1.Style.Font.Italic = false;
            styleRule1.Style.Font.Name = "Tahoma";
            styleRule1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(18D);
            styleRule1.Style.Font.Strikeout = false;
            styleRule1.Style.Font.Underline = false;
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Tahoma";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule3.Style.Font.Name = "Tahoma";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule4.Style.Font.Name = "Tahoma";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(9.5000009536743164D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.GroupHeaderSection labelsGroupHeaderSection;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooterSection;
        private Telerik.Reporting.PageHeaderSection pageHeader;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.TextBox currentTimeTextBox;
        private Telerik.Reporting.TextBox pageInfoTextBox;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.ReportFooterSection reportFooter;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.SqlDataSource reportSource;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox34;
        private Telerik.Reporting.TextBox textBox35;
        private Telerik.Reporting.TextBox textBox37;
        private Telerik.Reporting.TextBox textBox38;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox41;
        private Telerik.Reporting.TextBox textBox32;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox36;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox26;
        private Telerik.Reporting.TextBox textBox28;
        private Telerik.Reporting.TextBox textBox33;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox29;

    }
}