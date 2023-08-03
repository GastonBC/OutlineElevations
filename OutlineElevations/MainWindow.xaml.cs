using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public class ViewPortItem : INotifyPropertyChanged
    {
        public ViewSheet Sheet { get; set; }

        private bool _Checked;
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked == value)
                    return;
                _Checked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Checked"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public partial class MainWindow : Window
    {
        private UIDocument uidoc = null;
        private Document doc = null;
        private List<Element> viewerCollector;
        private readonly ObservableCollection<ViewPortItem> SheetsCollection = new ObservableCollection<ViewPortItem>();


        public MainWindow(UIDocument uid)
        {
            InitializeComponent();

            uidoc = uid;
            doc = uidoc.Document;
            Sheet_lv.ItemsSource = SheetsCollection;

            viewerCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Viewers)
                .ToElements()
                .ToList();


            List<ViewSheet> SheetsCol = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Sheets)
                .WhereElementIsNotElementType()
                .Cast<ViewSheet>()
                .Where(i => !i.IsPlaceholder)
                .OrderBy(s => s.SheetNumber)
                .ToList();


            foreach (ViewSheet sheetInstance in SheetsCol)
            {
                ViewPortItem addition = new ViewPortItem
                {
                    Checked = false,
                    Sheet = sheetInstance
                };

                SheetsCollection.Add(addition);
            }
        }

        private void search_txt_changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Sheet_lv.ItemsSource = SheetsCollection.Where(s => s.Sheet.SheetNumber.ToLower().Contains(Search_tb.Text.ToLower()) ||
                                                               s.Sheet.Name.ToLower().Contains(Search_tb.Text.ToLower()));
        }

        private void onCheckBoxCheck(object sender, RoutedEventArgs e)
        {

            // deselect single item if a checkbox is checked or it will change your selection
            // as well
            if (Sheet_lv.SelectedItems.Count == 1)
            {
                Sheet_lv.SelectedItem = null;
            }

            else
            {
                foreach (object ListObj in Sheet_lv.SelectedItems)
                {
                    ViewPortItem ListItem = ListObj as ViewPortItem;
                    ListItem.Checked = true;
                }
            }
        }

        private void onCheckBoxUncheck(object sender, RoutedEventArgs e)
        {
            // deselect single item if a checkbox is checked or it will change your selection
            // as well
            if (Sheet_lv.SelectedItems.Count == 1)
            {
                Sheet_lv.SelectedItem = null;
            }

            else
            {
                foreach (object ListObj in Sheet_lv.SelectedItems)
                {
                    ViewPortItem ListItem = ListObj as ViewPortItem;
                    ListItem.Checked = false;
                }
            }
        }

        private void SelSheets_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            WeightGUI WeightWn = new WeightGUI(this, doc, SheetsCollection.ToList(), viewerCollector);
            WeightWn.ShowDialog();
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            Search_tb.Clear();
            Sheet_lv.SelectedItems.Clear();
        }

        private void SelAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ViewPortItem sheetElem in SheetsCollection)
            {
                sheetElem.Checked = true;
            }
        }

        private void DeselAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (ViewPortItem sheetElem in SheetsCollection)
            {
                sheetElem.Checked = false;
            }
        }
    }
}
