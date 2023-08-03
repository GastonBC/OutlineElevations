using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OutlineElevations
{
    public partial class WeightGUI : Window
    {
        MainWindow MainWn;
        Document doc;
        List<ViewPortItem> SheetSource;
        List<Element> viewerCollector;

        public WeightGUI(MainWindow _MainWn,
                            Document _doc,
                            List<ViewPortItem> _SheetSource,
                            List<Element> _viewerCollector)
        {
            InitializeComponent();

            SheetSource = _SheetSource;
            MainWn = _MainWn;
            doc = _doc;
            viewerCollector = _viewerCollector;

            lb.ItemsSource = Enumerable.Range(1, 17);
        }

        private void Confirm_b_Click(object sender, RoutedEventArgs e)
        {
            using (Transaction t = new Transaction(doc))
            {
                t.Start("Outline Elevations");


                // This will turn off all cropboxes in all sheets
                if (cb_cropboxconfirm.IsChecked ?? false)
                {
                    FilteredElementCollector allViewPorts = new FilteredElementCollector(doc);
                    allViewPorts.OfCategory(BuiltInCategory.OST_Viewports)
                                .ToElements();

                    foreach (Viewport singleVp in allViewPorts)
                    {
                        View viewVp = doc.GetElement(singleVp.ViewId) as View;
                        viewVp.CropBoxVisible = false;
                    }
                }

                // This loop will turn on the ones on the specified sheets
                foreach (ViewPortItem sheetInstance in SheetSource)
                {
                    if (sheetInstance.Checked) // Each sheet selected
                    {
                        //Each viewport in each sheet
                        foreach (ElementId viewPortId in sheetInstance.Sheet.GetAllViewports())
                        {


                            Viewport viewport = doc.GetElement(viewPortId) as Viewport;
                            View view = doc.GetElement(viewport.ViewId) as View;

                            ElementId cropbox = GetCropBoxFor(view);

                            ViewType viewType = view.ViewType;

                            // Turn off crop box for non sections or elevations (if there are any in this sheet)
                            if (viewType != ViewType.Elevation && viewType != ViewType.Section)
                            {
                                view.CropBoxVisible = false;
                                continue;
                            }

                            view.CropBoxVisible = true;
                            OverrideGraphicSettings graphicSettings = new OverrideGraphicSettings();
                            graphicSettings.SetProjectionLineWeight((int)lb.SelectedItem);

                            view.SetElementOverrides(cropbox, graphicSettings);
                        }
                    }
                }


                t.Commit();
            }

            this.Close();

        }

        // From Jeremy Tammik
        // https://thebuildingcoder.typepad.com/blog/2018/02/efficiently-retrieve-crop-box-for-given-view.html
        ElementId GetCropBoxFor(View view)
        {
            ParameterValueProvider provider
              = new ParameterValueProvider(new ElementId(
                (int)BuiltInParameter.ID_PARAM));

            FilterElementIdRule rule
              = new FilterElementIdRule(provider,
                new FilterNumericEquals(), view.Id);

            ElementParameterFilter filter
              = new ElementParameterFilter(rule);

            return new FilteredElementCollector(view.Document)
              .WherePasses(filter)
              .ToElementIds()
              .Where<ElementId>(a => a.IntegerValue
               != view.Id.IntegerValue)
              .FirstOrDefault<ElementId>();
        }

        private void Back_b_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWn.ShowDialog();
        }
    }
}