if (!IsPostBack)
            {
                SeriesList sList = new SeriesList();
                sList.ChartType = ChartType.Line;
                sList.AxisFactor.YAxis.SetAxisStep(0, 150, 50);

                Random r = new Random();
                for (int i = 0; i < 1; i++)
                {
                    Series sr = new Series();
                    sr.Points.Width = 4;

                    for (int x = 0; x < 5; x++)
                    {
                        SeriesItem item = new SeriesItem();
                        item.YValue = r.Next(110);
                        item.Name = "item";
                        item.XValue = x;

                        sr.items.Add(item);
                    }

                    sList.SeriesCollection.Add(sr);
                }

                this.WHippoChart1.Titles.Label.ForeColor = System.Drawing.Color.SteelBlue;
                this.WHippoChart1.Titles.Label.Text = "웹 실시간 차트 데모";
                this.WHippoChart1.SeriesListDictionary.Add(sList);

                this.WHippoChart1.DesignType = ChartDesignType.Simple2;
                this.WHippoChart1.Direction = GraphAreaLocation.Vertical;
 
                this.WHippoChart1.DrawChart();
            }


