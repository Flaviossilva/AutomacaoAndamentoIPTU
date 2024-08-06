using AutomacaoIPTU.Business;

namespace AutomacaoIPTU
{
    public partial class Automator : Form
    {
        readonly Business.Business business = new();
        public Automator()
        {
            InitializeComponent();
        }

        private async void Automator_Load(object sender, EventArgs e)
        {
            var dia = DateTime.Now.DayOfWeek;
            if (dia.ToString().Contains("Tuesday") || dia.ToString().Contains("Friday"))
                await business.StartThreads(1);
            else
                await business.StartThreads(1);
            //await business.Start();
            business.Stop();
        }
    }
}
