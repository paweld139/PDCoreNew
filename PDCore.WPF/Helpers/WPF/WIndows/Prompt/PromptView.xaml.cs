using System.Windows;

namespace PDCore.WPF.Helpers.WPF.Windows.Prompt
{
    /// <summary>
    /// Interaction logic for PromptView.xaml
    /// </summary>
    public partial class PromptView : Window
    {
        public PromptView()
        {
            InitializeComponent();
        }

        public PromptView(string displayName, string title = "", bool isPassword = false, params object[] data) : this()
        {
            var viewModel = new PromptViewModel(displayName, title, isPassword, data);

            viewModel.OnRequestClose += (s, e) => Close();


            DataContext = viewModel;
        }

        public string ResponseText
        {
            get
            {
                return (DataContext as PromptViewModel).ResponseText;
            }
        }
    }
}
