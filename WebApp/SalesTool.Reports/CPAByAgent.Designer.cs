namespace SalesTool.Reports
{
    partial class CPAByAgent
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.GraphGroup graphGroup1 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup4 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphTitle graphTitle1 = new Telerik.Reporting.GraphTitle();
            Telerik.Reporting.CategoryScale categoryScale1 = new Telerik.Reporting.CategoryScale();
            Telerik.Reporting.NumericalScale numericalScale1 = new Telerik.Reporting.NumericalScale();
            Telerik.Reporting.GraphGroup graphGroup2 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup3 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            this.labelsGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.labelsGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.agentNameCaptionTextBox = new Telerik.Reporting.TextBox();
            this.validLeadsCaptionTextBox = new Telerik.Reporting.TextBox();
            this.medSupClosedCaptionTextBox = new Telerik.Reporting.TextBox();
            this.medSupPercentValidCaptionTextBox = new Telerik.Reporting.TextBox();
            this.mAPlanClosedCaptionTextBox = new Telerik.Reporting.TextBox();
            this.mAPlanPercentValiedCaptionTextBox = new Telerik.Reporting.TextBox();
            this.policiesClosedCaptionTextBox = new Telerik.Reporting.TextBox();
            this.policiesPercentValidCaptionTextBox = new Telerik.Reporting.TextBox();
            this.projectedPercentCloseCaptionTextBox = new Telerik.Reporting.TextBox();
            this.costAcquisitionCaptionTextBox = new Telerik.Reporting.TextBox();
            this.projectedCPACaptionTextBox = new Telerik.Reporting.TextBox();
            this.ReportSource = new Telerik.Reporting.SqlDataSource();
            this.reportFooter = new Telerik.Reporting.ReportFooterSection();
            this.graph1 = new Telerik.Reporting.Graph();
            this.cartesianCoordinateSystem1 = new Telerik.Reporting.CartesianCoordinateSystem();
            this.graphAxis2 = new Telerik.Reporting.GraphAxis();
            this.graphAxis1 = new Telerik.Reporting.GraphAxis();
            this.barSeries1 = new Telerik.Reporting.BarSeries();
            this.panel1 = new Telerik.Reporting.Panel();
            this.projectedCPASumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.costAcquisitionSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.projectedPercentCloseSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.policiesPercentValidSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.policiesClosedSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.mAPlanPercentValiedSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.mAPlanClosedSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.medSupPercentValidSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.medSupClosedSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.validLeadsSumFunctionTextBox = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.pageHeader = new Telerik.Reporting.PageHeaderSection();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.currentTimeTextBox = new Telerik.Reporting.TextBox();
            this.pageInfoTextBox = new Telerik.Reporting.TextBox();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.agentNameDataTextBox = new Telerik.Reporting.TextBox();
            this.validLeadsDataTextBox = new Telerik.Reporting.TextBox();
            this.medSupClosedDataTextBox = new Telerik.Reporting.TextBox();
            this.medSupPercentValidDataTextBox = new Telerik.Reporting.TextBox();
            this.mAPlanClosedDataTextBox = new Telerik.Reporting.TextBox();
            this.mAPlanPercentValiedDataTextBox = new Telerik.Reporting.TextBox();
            this.policiesClosedDataTextBox = new Telerik.Reporting.TextBox();
            this.policiesPercentValidDataTextBox = new Telerik.Reporting.TextBox();
            this.projectedPercentCloseDataTextBox = new Telerik.Reporting.TextBox();
            this.costAcquisitionDataTextBox = new Telerik.Reporting.TextBox();
            this.projectedCPADataTextBox = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // labelsGroupFooterSection
            // 
            this.labelsGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.273506224155426D);
            this.labelsGroupFooterSection.Name = "labelsGroupFooterSection";
            this.labelsGroupFooterSection.Style.Color = System.Drawing.Color.White;
            this.labelsGroupFooterSection.Style.Visible = false;
            // 
            // labelsGroupHeaderSection
            // 
            this.labelsGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.41886797547340393D);
            this.labelsGroupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.agentNameCaptionTextBox,
            this.validLeadsCaptionTextBox,
            this.medSupClosedCaptionTextBox,
            this.medSupPercentValidCaptionTextBox,
            this.mAPlanClosedCaptionTextBox,
            this.mAPlanPercentValiedCaptionTextBox,
            this.policiesClosedCaptionTextBox,
            this.policiesPercentValidCaptionTextBox,
            this.projectedPercentCloseCaptionTextBox,
            this.costAcquisitionCaptionTextBox,
            this.projectedCPACaptionTextBox});
            this.labelsGroupHeaderSection.Name = "labelsGroupHeaderSection";
            this.labelsGroupHeaderSection.PrintOnEveryPage = true;
            this.labelsGroupHeaderSection.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(112)))));
            // 
            // agentNameCaptionTextBox
            // 
            this.agentNameCaptionTextBox.CanGrow = true;
            this.agentNameCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.018867984414100647D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.agentNameCaptionTextBox.Name = "agentNameCaptionTextBox";
            this.agentNameCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1382921934127808D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.agentNameCaptionTextBox.Style.Font.Bold = true;
            this.agentNameCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.agentNameCaptionTextBox.StyleName = "Caption";
            this.agentNameCaptionTextBox.Value = "Agent Name";
            // 
            // validLeadsCaptionTextBox
            // 
            this.validLeadsCaptionTextBox.CanGrow = true;
            this.validLeadsCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1760281324386597D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.validLeadsCaptionTextBox.Name = "validLeadsCaptionTextBox";
            this.validLeadsCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.validLeadsCaptionTextBox.Style.Font.Bold = true;
            this.validLeadsCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.validLeadsCaptionTextBox.StyleName = "Caption";
            this.validLeadsCaptionTextBox.Value = "Valid Leads";
            // 
            // medSupClosedCaptionTextBox
            // 
            this.medSupClosedCaptionTextBox.CanGrow = true;
            this.medSupClosedCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0520563125610352D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.medSupClosedCaptionTextBox.Name = "medSupClosedCaptionTextBox";
            this.medSupClosedCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.medSupClosedCaptionTextBox.Style.Font.Bold = true;
            this.medSupClosedCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.medSupClosedCaptionTextBox.StyleName = "Caption";
            this.medSupClosedCaptionTextBox.Value = "Med Supp Plans Closed";
            // 
            // medSupPercentValidCaptionTextBox
            // 
            this.medSupPercentValidCaptionTextBox.CanGrow = true;
            this.medSupPercentValidCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9280846118927D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.medSupPercentValidCaptionTextBox.Name = "medSupPercentValidCaptionTextBox";
            this.medSupPercentValidCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.medSupPercentValidCaptionTextBox.Style.Font.Bold = true;
            this.medSupPercentValidCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.medSupPercentValidCaptionTextBox.StyleName = "Caption";
            this.medSupPercentValidCaptionTextBox.Value = "% of Valid (Med Supp)";
            // 
            // mAPlanClosedCaptionTextBox
            // 
            this.mAPlanClosedCaptionTextBox.CanGrow = true;
            this.mAPlanClosedCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8041126728057861D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.mAPlanClosedCaptionTextBox.Name = "mAPlanClosedCaptionTextBox";
            this.mAPlanClosedCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.mAPlanClosedCaptionTextBox.Style.Font.Bold = true;
            this.mAPlanClosedCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.mAPlanClosedCaptionTextBox.StyleName = "Caption";
            this.mAPlanClosedCaptionTextBox.Value = "MA Plans Closed";
            // 
            // mAPlanPercentValiedCaptionTextBox
            // 
            this.mAPlanPercentValiedCaptionTextBox.CanGrow = true;
            this.mAPlanPercentValiedCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.6801409721374512D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.mAPlanPercentValiedCaptionTextBox.Name = "mAPlanPercentValiedCaptionTextBox";
            this.mAPlanPercentValiedCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.mAPlanPercentValiedCaptionTextBox.Style.Font.Bold = true;
            this.mAPlanPercentValiedCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.mAPlanPercentValiedCaptionTextBox.StyleName = "Caption";
            this.mAPlanPercentValiedCaptionTextBox.Value = "% of Valid (MA)";
            // 
            // policiesClosedCaptionTextBox
            // 
            this.policiesClosedCaptionTextBox.CanGrow = true;
            this.policiesClosedCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.5561685562133789D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.policiesClosedCaptionTextBox.Name = "policiesClosedCaptionTextBox";
            this.policiesClosedCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.policiesClosedCaptionTextBox.Style.Font.Bold = true;
            this.policiesClosedCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.policiesClosedCaptionTextBox.StyleName = "Caption";
            this.policiesClosedCaptionTextBox.Value = "Policies Closed";
            // 
            // policiesPercentValidCaptionTextBox
            // 
            this.policiesPercentValidCaptionTextBox.CanGrow = true;
            this.policiesPercentValidCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4321966171264648D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.policiesPercentValidCaptionTextBox.Name = "policiesPercentValidCaptionTextBox";
            this.policiesPercentValidCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.policiesPercentValidCaptionTextBox.Style.Font.Bold = true;
            this.policiesPercentValidCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.policiesPercentValidCaptionTextBox.StyleName = "Caption";
            this.policiesPercentValidCaptionTextBox.Value = "% of Valid (Total)";
            // 
            // projectedPercentCloseCaptionTextBox
            // 
            this.projectedPercentCloseCaptionTextBox.CanGrow = true;
            this.projectedPercentCloseCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.3000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.projectedPercentCloseCaptionTextBox.Name = "projectedPercentCloseCaptionTextBox";
            this.projectedPercentCloseCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78425318002700806D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.projectedPercentCloseCaptionTextBox.Style.Font.Bold = true;
            this.projectedPercentCloseCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.projectedPercentCloseCaptionTextBox.StyleName = "Caption";
            this.projectedPercentCloseCaptionTextBox.Value = "Projected Close %";
            // 
            // costAcquisitionCaptionTextBox
            // 
            this.costAcquisitionCaptionTextBox.CanGrow = true;
            this.costAcquisitionCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.costAcquisitionCaptionTextBox.Name = "costAcquisitionCaptionTextBox";
            this.costAcquisitionCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.costAcquisitionCaptionTextBox.Style.Font.Bold = true;
            this.costAcquisitionCaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.costAcquisitionCaptionTextBox.StyleName = "Caption";
            this.costAcquisitionCaptionTextBox.Value = "Cost Per Acquisition";
            // 
            // projectedCPACaptionTextBox
            // 
            this.projectedCPACaptionTextBox.CanGrow = true;
            this.projectedCPACaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.97610092163086D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.projectedCPACaptionTextBox.Name = "projectedCPACaptionTextBox";
            this.projectedCPACaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.70000046491622925D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.projectedCPACaptionTextBox.Style.Font.Bold = true;
            this.projectedCPACaptionTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(11D);
            this.projectedCPACaptionTextBox.StyleName = "Caption";
            this.projectedCPACaptionTextBox.Value = "Projected CPA";
            // 
            // ReportSource
            // 
            this.ReportSource.ConnectionString = "ApplicationServices";
            this.ReportSource.Name = "ReportSource";
            this.ReportSource.SelectCommand = "dbo.report_CPA_By_Agent";
            this.ReportSource.SelectCommandType = Telerik.Reporting.SqlDataSourceCommandType.StoredProcedure;
            // 
            // reportFooter
            // 
            this.reportFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(5.18875789642334D);
            this.reportFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.graph1,
            this.panel1});
            this.reportFooter.Name = "reportFooter";
            this.reportFooter.Style.BackgroundColor = System.Drawing.Color.White;
            this.reportFooter.Style.Color = System.Drawing.Color.White;
            this.reportFooter.Style.Visible = true;
            // 
            // graph1
            // 
            graphGroup1.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.AgentName"));
            graphGroup1.Name = "agentNameGroup";
            graphGroup1.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.AgentName", Telerik.Reporting.SortDirection.Asc));
            this.graph1.CategoryGroups.Add(graphGroup1);
            this.graph1.CoordinateSystems.Add(this.cartesianCoordinateSystem1);
            this.graph1.DataSource = this.ReportSource;
            this.graph1.Legend.Position = Telerik.Reporting.GraphItemPosition.RightCenter;
            this.graph1.Legend.Style.LineColor = System.Drawing.Color.LightGray;
            this.graph1.Legend.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.graph1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.018867744132876396D), Telerik.Reporting.Drawing.Unit.Inch(0.68875765800476074D));
            this.graph1.Name = "graph1";
            this.graph1.PlotAreaStyle.LineColor = System.Drawing.Color.LightGray;
            this.graph1.PlotAreaStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.graph1.Series.Add(this.barSeries1);
            graphGroup4.Name = "seriesGroup";
            this.graph1.SeriesGroups.Add(graphGroup4);
            this.graph1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.61744213104248D), Telerik.Reporting.Drawing.Unit.Inch(4.1376967430114746D));
            this.graph1.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            graphTitle1.Position = Telerik.Reporting.GraphItemPosition.TopCenter;
            graphTitle1.Style.LineColor = System.Drawing.Color.LightGray;
            graphTitle1.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            graphTitle1.Text = "graph1";
            this.graph1.Titles.Add(graphTitle1);
            // 
            // cartesianCoordinateSystem1
            // 
            this.cartesianCoordinateSystem1.Name = "cartesianCoordinateSystem1";
            this.cartesianCoordinateSystem1.XAxis = this.graphAxis2;
            this.cartesianCoordinateSystem1.YAxis = this.graphAxis1;
            // 
            // graphAxis2
            // 
            this.graphAxis2.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.Visible = false;
            this.graphAxis2.Name = "graphAxis2";
            this.graphAxis2.Scale = categoryScale1;
            // 
            // graphAxis1
            // 
            this.graphAxis1.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.Visible = false;
            this.graphAxis1.Name = "graphAxis1";
            this.graphAxis1.Scale = numericalScale1;
            // 
            // barSeries1
            // 
            graphGroup2.Groupings.Add(new Telerik.Reporting.Grouping("=Fields.AgentName"));
            graphGroup2.Name = "agentNameGroup";
            graphGroup2.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.AgentName", Telerik.Reporting.SortDirection.Asc));
            this.barSeries1.CategoryGroup = graphGroup2;
            this.barSeries1.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries1.DataPointLabel = "=Sum(Fields.ValidLeads)";
            this.barSeries1.DataPointLabelAlignment = Telerik.Reporting.BarDataPointLabelAlignment.InsideBase;
            this.barSeries1.DataPointLabelStyle.Visible = false;
            this.barSeries1.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.barSeries1.DataPointStyle.Visible = true;
            this.barSeries1.Legend = "ValidLeads";
            this.barSeries1.LegendFormat = "";
            graphGroup3.Name = "seriesGroup";
            this.barSeries1.SeriesGroup = graphGroup3;
            this.barSeries1.Y = "=Sum(Fields.ValidLeads)";
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.projectedCPASumFunctionTextBox,
            this.costAcquisitionSumFunctionTextBox,
            this.projectedPercentCloseSumFunctionTextBox,
            this.policiesPercentValidSumFunctionTextBox,
            this.policiesClosedSumFunctionTextBox,
            this.mAPlanPercentValiedSumFunctionTextBox,
            this.mAPlanClosedSumFunctionTextBox,
            this.medSupPercentValidSumFunctionTextBox,
            this.medSupClosedSumFunctionTextBox,
            this.validLeadsSumFunctionTextBox,
            this.textBox1});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.018828392028808594D), Telerik.Reporting.Drawing.Unit.Inch(0.045990493148565292D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.6811733245849609D), Telerik.Reporting.Drawing.Unit.Inch(0.29999995231628418D));
            this.panel1.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(112)))));
            // 
            // projectedCPASumFunctionTextBox
            // 
            this.projectedCPASumFunctionTextBox.CanGrow = true;
            this.projectedCPASumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.9572734832763672D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.projectedCPASumFunctionTextBox.Name = "projectedCPASumFunctionTextBox";
            this.projectedCPASumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.70000046491622925D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.projectedCPASumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.projectedCPASumFunctionTextBox.StyleName = "Data";
            this.projectedCPASumFunctionTextBox.Value = "=Sum(Fields.ProjectedCPA)";
            // 
            // costAcquisitionSumFunctionTextBox
            // 
            this.costAcquisitionSumFunctionTextBox.CanGrow = true;
            this.costAcquisitionSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.0849456787109375D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.costAcquisitionSumFunctionTextBox.Name = "costAcquisitionSumFunctionTextBox";
            this.costAcquisitionSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.costAcquisitionSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.costAcquisitionSumFunctionTextBox.StyleName = "Data";
            this.costAcquisitionSumFunctionTextBox.Value = "=Sum(Fields.CostAcquisition)";
            // 
            // projectedPercentCloseSumFunctionTextBox
            // 
            this.projectedPercentCloseSumFunctionTextBox.CanGrow = true;
            this.projectedPercentCloseSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.2811713218688965D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.projectedPercentCloseSumFunctionTextBox.Name = "projectedPercentCloseSumFunctionTextBox";
            this.projectedPercentCloseSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78425318002700806D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.projectedPercentCloseSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.projectedPercentCloseSumFunctionTextBox.StyleName = "Data";
            this.projectedPercentCloseSumFunctionTextBox.Value = "=Sum(Fields.ProjectedPercentClose)";
            // 
            // policiesPercentValidSumFunctionTextBox
            // 
            this.policiesPercentValidSumFunctionTextBox.CanGrow = true;
            this.policiesPercentValidSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.46230411529541D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.policiesPercentValidSumFunctionTextBox.Name = "policiesPercentValidSumFunctionTextBox";
            this.policiesPercentValidSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80822497606277466D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.policiesPercentValidSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.policiesPercentValidSumFunctionTextBox.StyleName = "Data";
            this.policiesPercentValidSumFunctionTextBox.Value = "=Sum(Fields.PoliciesPercentValid)";
            // 
            // policiesClosedSumFunctionTextBox
            // 
            this.policiesClosedSumFunctionTextBox.CanGrow = true;
            this.policiesClosedSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.5849452018737793D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.policiesClosedSumFunctionTextBox.Name = "policiesClosedSumFunctionTextBox";
            this.policiesClosedSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.policiesClosedSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.policiesClosedSumFunctionTextBox.StyleName = "Data";
            this.policiesClosedSumFunctionTextBox.Value = "=Sum(Fields.PoliciesClosed)";
            // 
            // mAPlanPercentValiedSumFunctionTextBox
            // 
            this.mAPlanPercentValiedSumFunctionTextBox.CanGrow = true;
            this.mAPlanPercentValiedSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7075862884521484D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.mAPlanPercentValiedSumFunctionTextBox.Name = "mAPlanPercentValiedSumFunctionTextBox";
            this.mAPlanPercentValiedSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.mAPlanPercentValiedSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.mAPlanPercentValiedSumFunctionTextBox.StyleName = "Data";
            this.mAPlanPercentValiedSumFunctionTextBox.Value = "=Sum(Fields.MAPlanPercentValied)";
            // 
            // mAPlanClosedSumFunctionTextBox
            // 
            this.mAPlanClosedSumFunctionTextBox.CanGrow = true;
            this.mAPlanClosedSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8302278518676758D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.mAPlanClosedSumFunctionTextBox.Name = "mAPlanClosedSumFunctionTextBox";
            this.mAPlanClosedSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.mAPlanClosedSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.mAPlanClosedSumFunctionTextBox.StyleName = "Data";
            this.mAPlanClosedSumFunctionTextBox.Value = "=Sum(Fields.MAPlanClosed)";
            // 
            // medSupPercentValidSumFunctionTextBox
            // 
            this.medSupPercentValidSumFunctionTextBox.CanGrow = true;
            this.medSupPercentValidSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9528694152832031D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.medSupPercentValidSumFunctionTextBox.Name = "medSupPercentValidSumFunctionTextBox";
            this.medSupPercentValidSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.medSupPercentValidSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.medSupPercentValidSumFunctionTextBox.StyleName = "Data";
            this.medSupPercentValidSumFunctionTextBox.Value = "=Sum(Fields.MedSupPercentValid)";
            // 
            // medSupClosedSumFunctionTextBox
            // 
            this.medSupClosedSumFunctionTextBox.CanGrow = true;
            this.medSupClosedSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0755107402801514D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.medSupClosedSumFunctionTextBox.Name = "medSupClosedSumFunctionTextBox";
            this.medSupClosedSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.medSupClosedSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.medSupClosedSumFunctionTextBox.StyleName = "Data";
            this.medSupClosedSumFunctionTextBox.Value = "=Sum(Fields.MedSupClosed)";
            // 
            // validLeadsSumFunctionTextBox
            // 
            this.validLeadsSumFunctionTextBox.CanGrow = true;
            this.validLeadsSumFunctionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1981524229049683D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.validLeadsSumFunctionTextBox.Name = "validLeadsSumFunctionTextBox";
            this.validLeadsSumFunctionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.validLeadsSumFunctionTextBox.Style.Color = System.Drawing.Color.White;
            this.validLeadsSumFunctionTextBox.StyleName = "Data";
            this.validLeadsSumFunctionTextBox.Value = "=Sum(Fields.ValidLeads)";
            // 
            // textBox1
            // 
            this.textBox1.CanGrow = true;
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0094731198623776436D), Telerik.Reporting.Drawing.Unit.Inch(7.4955650575248E-08D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1382921934127808D), Telerik.Reporting.Drawing.Unit.Inch(0.23011010885238648D));
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox1.StyleName = "Caption";
            this.textBox1.Value = "Grand Total";
            // 
            // pageHeader
            // 
            this.pageHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(0.43773585557937622D);
            this.pageHeader.Name = "pageHeader";
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(0.43773585557937622D);
            this.pageFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.currentTimeTextBox,
            this.pageInfoTextBox});
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // currentTimeTextBox
            // 
            this.currentTimeTextBox.Format = "{0:D}";
            this.currentTimeTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D), Telerik.Reporting.Drawing.Unit.Inch(0.15864779055118561D));
            this.currentTimeTextBox.Name = "currentTimeTextBox";
            this.currentTimeTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.7992868423461914D), Telerik.Reporting.Drawing.Unit.Inch(0.26022014021873474D));
            this.currentTimeTextBox.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.currentTimeTextBox.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.currentTimeTextBox.StyleName = "PageInfo";
            this.currentTimeTextBox.Value = "=NOW()";
            // 
            // pageInfoTextBox
            // 
            this.pageInfoTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.83702278137207D), Telerik.Reporting.Drawing.Unit.Inch(0.15864779055118561D));
            this.pageInfoTextBox.Name = "pageInfoTextBox";
            this.pageInfoTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.7992868423461914D), Telerik.Reporting.Drawing.Unit.Inch(0.26022014021873474D));
            this.pageInfoTextBox.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.pageInfoTextBox.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.pageInfoTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageInfoTextBox.StyleName = "PageInfo";
            this.pageInfoTextBox.Value = "=PageNumber";
            // 
            // reportHeader
            // 
            this.reportHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(0.262264221906662D);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3});
            this.reportHeader.Name = "reportHeader";
            this.reportHeader.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.028301598504185677D), Telerik.Reporting.Drawing.Unit.Inch(3.940654278267175E-05D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.8999996185302734D), Telerik.Reporting.Drawing.Unit.Inch(0.26222482323646545D));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(15D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.Value = "CPA Report for Sales Tool Dashboard";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.41886794567108154D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.agentNameDataTextBox,
            this.validLeadsDataTextBox,
            this.medSupClosedDataTextBox,
            this.medSupPercentValidDataTextBox,
            this.mAPlanClosedDataTextBox,
            this.mAPlanPercentValiedDataTextBox,
            this.policiesClosedDataTextBox,
            this.policiesPercentValidDataTextBox,
            this.projectedPercentCloseDataTextBox,
            this.costAcquisitionDataTextBox,
            this.projectedCPADataTextBox});
            this.detail.Name = "detail";
            this.detail.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            // 
            // agentNameDataTextBox
            // 
            this.agentNameDataTextBox.CanGrow = true;
            this.agentNameDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.agentNameDataTextBox.Name = "agentNameDataTextBox";
            this.agentNameDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1382921934127808D), Telerik.Reporting.Drawing.Unit.Inch(0.41886794567108154D));
            this.agentNameDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(207)))), ((int)(((byte)(231)))));
            this.agentNameDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.agentNameDataTextBox.StyleName = "Data";
            this.agentNameDataTextBox.Value = "=Fields.AgentName";
            // 
            // validLeadsDataTextBox
            // 
            this.validLeadsDataTextBox.CanGrow = true;
            this.validLeadsDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2811321020126343D), Telerik.Reporting.Drawing.Unit.Inch(-3.4694469519536142E-18D));
            this.validLeadsDataTextBox.Name = "validLeadsDataTextBox";
            this.validLeadsDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.75205624103546143D), Telerik.Reporting.Drawing.Unit.Inch(0.4188677966594696D));
            this.validLeadsDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.validLeadsDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Dotted;
            this.validLeadsDataTextBox.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.validLeadsDataTextBox.StyleName = "Data";
            this.validLeadsDataTextBox.Value = "=Fields.ValidLeads";
            // 
            // medSupClosedDataTextBox
            // 
            this.medSupClosedDataTextBox.CanGrow = true;
            this.medSupClosedDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.1811323165893555D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.medSupClosedDataTextBox.Name = "medSupClosedDataTextBox";
            this.medSupClosedDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.72808438539505D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.medSupClosedDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.medSupClosedDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.medSupClosedDataTextBox.Style.BorderWidth.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.medSupClosedDataTextBox.StyleName = "Data";
            this.medSupClosedDataTextBox.Value = "=Fields.MedSupClosed";
            // 
            // medSupPercentValidDataTextBox
            // 
            this.medSupPercentValidDataTextBox.CanGrow = true;
            this.medSupPercentValidDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.9811322689056396D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.medSupPercentValidDataTextBox.Name = "medSupPercentValidDataTextBox";
            this.medSupPercentValidDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80411261320114136D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.medSupPercentValidDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.medSupPercentValidDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.medSupPercentValidDataTextBox.StyleName = "Data";
            this.medSupPercentValidDataTextBox.Value = "=Fields.MedSupPercentValid";
            // 
            // mAPlanClosedDataTextBox
            // 
            this.mAPlanClosedDataTextBox.CanGrow = true;
            this.mAPlanClosedDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8811321258544922D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.mAPlanClosedDataTextBox.Name = "mAPlanClosedDataTextBox";
            this.mAPlanClosedDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78014075756073D), Telerik.Reporting.Drawing.Unit.Inch(0.41886794567108154D));
            this.mAPlanClosedDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.mAPlanClosedDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.mAPlanClosedDataTextBox.Style.BorderWidth.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.mAPlanClosedDataTextBox.StyleName = "Data";
            this.mAPlanClosedDataTextBox.Value = "=Fields.MAPlanClosed";
            // 
            // mAPlanPercentValiedDataTextBox
            // 
            this.mAPlanPercentValiedDataTextBox.CanGrow = true;
            this.mAPlanPercentValiedDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7811322212219238D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.mAPlanPercentValiedDataTextBox.Name = "mAPlanPercentValiedDataTextBox";
            this.mAPlanPercentValiedDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7561689019203186D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.mAPlanPercentValiedDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.mAPlanPercentValiedDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.mAPlanPercentValiedDataTextBox.StyleName = "Data";
            this.mAPlanPercentValiedDataTextBox.Value = "=Fields.MAPlanPercentValied";
            // 
            // policiesClosedDataTextBox
            // 
            this.policiesClosedDataTextBox.CanGrow = true;
            this.policiesClosedDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.6811318397521973D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.policiesClosedDataTextBox.Name = "policiesClosedDataTextBox";
            this.policiesClosedDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.732197105884552D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.policiesClosedDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.policiesClosedDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.policiesClosedDataTextBox.Style.BorderWidth.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.policiesClosedDataTextBox.StyleName = "Data";
            this.policiesClosedDataTextBox.Value = "=Fields.PoliciesClosed";
            // 
            // policiesPercentValidDataTextBox
            // 
            this.policiesPercentValidDataTextBox.CanGrow = true;
            this.policiesPercentValidDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4811315536499023D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.policiesPercentValidDataTextBox.Name = "policiesPercentValidDataTextBox";
            this.policiesPercentValidDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80822503566741943D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.policiesPercentValidDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.policiesPercentValidDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.policiesPercentValidDataTextBox.StyleName = "Data";
            this.policiesPercentValidDataTextBox.Value = "=Fields.PoliciesPercentValid";
            // 
            // projectedPercentCloseDataTextBox
            // 
            this.projectedPercentCloseDataTextBox.CanGrow = true;
            this.projectedPercentCloseDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.3000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.projectedPercentCloseDataTextBox.Name = "projectedPercentCloseDataTextBox";
            this.projectedPercentCloseDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78425318002700806D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.projectedPercentCloseDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.projectedPercentCloseDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.projectedPercentCloseDataTextBox.StyleName = "Data";
            this.projectedPercentCloseDataTextBox.Value = "=Fields.ProjectedPercentClose";
            // 
            // costAcquisitionDataTextBox
            // 
            this.costAcquisitionDataTextBox.CanGrow = true;
            this.costAcquisitionDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.1000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.costAcquisitionDataTextBox.Name = "costAcquisitionDataTextBox";
            this.costAcquisitionDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.857160210609436D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.costAcquisitionDataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.costAcquisitionDataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.costAcquisitionDataTextBox.StyleName = "Data";
            this.costAcquisitionDataTextBox.Value = "=Fields.CostAcquisition";
            // 
            // projectedCPADataTextBox
            // 
            this.projectedCPADataTextBox.CanGrow = true;
            this.projectedCPADataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.97610092163086D), Telerik.Reporting.Drawing.Unit.Inch(0.018867924809455872D));
            this.projectedCPADataTextBox.Name = "projectedCPADataTextBox";
            this.projectedCPADataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.70000046491622925D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.projectedCPADataTextBox.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(165)))), ((int)(((byte)(209)))));
            this.projectedCPADataTextBox.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.None;
            this.projectedCPADataTextBox.Style.BorderWidth.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.projectedCPADataTextBox.StyleName = "Data";
            this.projectedCPADataTextBox.Value = "=Fields.ProjectedCPA";
            // 
            // CPAByAgent
            // 
            this.DataSource = this.ReportSource;
            group1.GroupFooter = this.labelsGroupFooterSection;
            group1.GroupHeader = this.labelsGroupHeaderSection;
            group1.Name = "labelsGroup";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.labelsGroupHeaderSection,
            this.labelsGroupFooterSection,
            this.reportFooter,
            this.pageHeader,
            this.pageFooter,
            this.reportHeader,
            this.detail});
            this.Name = "Report1";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowNull = true;
            reportParameter1.Name = "userkey";
            this.ReportParameters.Add(reportParameter1);
            this.Style.BackgroundColor = System.Drawing.Color.White;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule1.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(112)))));
            styleRule1.Style.Font.Name = "Tahoma";
            styleRule1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(18D);
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(58)))), ((int)(((byte)(112)))));
            styleRule2.Style.Color = System.Drawing.Color.White;
            styleRule2.Style.Font.Name = "Tahoma";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule3.Style.Color = System.Drawing.Color.Black;
            styleRule3.Style.Font.Name = "Tahoma";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule4.Style.Color = System.Drawing.Color.Black;
            styleRule4.Style.Font.Name = "Tahoma";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(9.70000171661377D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.SqlDataSource ReportSource;
        private Telerik.Reporting.GroupHeaderSection labelsGroupHeaderSection;
        private Telerik.Reporting.TextBox agentNameCaptionTextBox;
        private Telerik.Reporting.TextBox validLeadsCaptionTextBox;
        private Telerik.Reporting.TextBox medSupClosedCaptionTextBox;
        private Telerik.Reporting.TextBox medSupPercentValidCaptionTextBox;
        private Telerik.Reporting.TextBox mAPlanClosedCaptionTextBox;
        private Telerik.Reporting.TextBox mAPlanPercentValiedCaptionTextBox;
        private Telerik.Reporting.TextBox policiesClosedCaptionTextBox;
        private Telerik.Reporting.TextBox policiesPercentValidCaptionTextBox;
        private Telerik.Reporting.TextBox projectedPercentCloseCaptionTextBox;
        private Telerik.Reporting.TextBox costAcquisitionCaptionTextBox;
        private Telerik.Reporting.TextBox projectedCPACaptionTextBox;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooterSection;
        private Telerik.Reporting.ReportFooterSection reportFooter;
        private Telerik.Reporting.PageHeaderSection pageHeader;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.TextBox currentTimeTextBox;
        private Telerik.Reporting.TextBox pageInfoTextBox;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox agentNameDataTextBox;
        private Telerik.Reporting.TextBox validLeadsDataTextBox;
        private Telerik.Reporting.TextBox medSupClosedDataTextBox;
        private Telerik.Reporting.TextBox medSupPercentValidDataTextBox;
        private Telerik.Reporting.TextBox mAPlanClosedDataTextBox;
        private Telerik.Reporting.TextBox mAPlanPercentValiedDataTextBox;
        private Telerik.Reporting.TextBox policiesClosedDataTextBox;
        private Telerik.Reporting.TextBox policiesPercentValidDataTextBox;
        private Telerik.Reporting.TextBox projectedPercentCloseDataTextBox;
        private Telerik.Reporting.TextBox costAcquisitionDataTextBox;
        private Telerik.Reporting.TextBox projectedCPADataTextBox;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.TextBox projectedCPASumFunctionTextBox;
        private Telerik.Reporting.TextBox costAcquisitionSumFunctionTextBox;
        private Telerik.Reporting.TextBox projectedPercentCloseSumFunctionTextBox;
        private Telerik.Reporting.TextBox policiesPercentValidSumFunctionTextBox;
        private Telerik.Reporting.TextBox policiesClosedSumFunctionTextBox;
        private Telerik.Reporting.TextBox mAPlanPercentValiedSumFunctionTextBox;
        private Telerik.Reporting.TextBox mAPlanClosedSumFunctionTextBox;
        private Telerik.Reporting.TextBox medSupPercentValidSumFunctionTextBox;
        private Telerik.Reporting.TextBox medSupClosedSumFunctionTextBox;
        private Telerik.Reporting.TextBox validLeadsSumFunctionTextBox;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.Graph graph1;
        private Telerik.Reporting.CartesianCoordinateSystem cartesianCoordinateSystem1;
        private Telerik.Reporting.GraphAxis graphAxis2;
        private Telerik.Reporting.GraphAxis graphAxis1;
        private Telerik.Reporting.BarSeries barSeries1;

    }
}