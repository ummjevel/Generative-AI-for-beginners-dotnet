using AgentsSK_20_CreativeWriter;
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

namespace AgentsSK_20_CreativeWriter;

/// <summary>
/// Interaction logic for testWindow02.xaml
/// </summary>
public partial class testWindow02 : Window
{

    private CancellationTokenSource _cts;  // For debouncing

    // Store the widths for each expander
    private Dictionary<string, double> expanderWidths = new Dictionary<string, double>
    {
        { "Code", 400 },
        { "Configuration", 250 }
    };
    private readonly double _collapsedWidth = 56; // Width for the collapsed header
    private AgenticWorkflowModel? agenticWorkflowModel;

    public testWindow02()
    {
        InitializeComponent();
        _cts = new CancellationTokenSource();
        agenticWorkflowModel = new AgenticWorkflowModel(this);

        this.DataContext = agenticWorkflowModel;
    }


    private async void AskButton_Click(object sender, RoutedEventArgs e)
    {
        // Disable the Ask button
        AskButton.IsEnabled = false;

        // Call the askQuestion method
        await agenticWorkflowModel?.askQuestion();

        // Re-enable the Ask button after the process is finished
        AskButton.IsEnabled = true;

    }

    private void QuestionBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter)
        {
            AskButton_Click(sender, e);
        }
    }
}
