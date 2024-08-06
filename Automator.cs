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
            var dia = DateTime.Now.Day;
            if (dia % 2 == 0)
                await business.StartThreads(1);
            else
                await business.StartThreads(2);
            //await business.Start();
            business.Stop();
        }
    }
}
